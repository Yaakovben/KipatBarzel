using KipatBarzel.DAL;
using KipatBarzel.Models;
using KipatBarzel.Utils;
using KipatBarzel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace KipatBarzel.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, CancellationTokenSource> ThreatMap = new Dictionary<string, CancellationTokenSource>();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // הצגת מערכת הגנה
        public IActionResult DefenceAmmuntion()
        {
            List<DefenceAmmunition> defenceAmmunitions = Data.Get.DefenceAmmunitions.ToList();
            return View(defenceAmmunitions);
        }
        // עדכון כמות תחמושת ההגנה
        public IActionResult updateDefenceAmmiunition(int dfid, int amount)
        {
            DefenceAmmunition? da = Data.Get.DefenceAmmunitions.Find(dfid);
            da.Amount = amount;
            Data.Get.SaveChanges();
            return RedirectToAction(nameof(DefenceAmmuntion));
        }

        // פתיחת טופס להוספת תחמושת הגנה חדשה
        public IActionResult CreateDefenceAmmuntion()
        {
            return View(new DefenceAmmunition());
        }

        // הוספת תחמושת חדשה להגנה
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateDefenceAmmuntion(DefenceAmmunition defence)
        {
            Data.Get.DefenceAmmunitions.Add(defence);
            Data.Get.SaveChanges();
            return RedirectToAction("DefenceAmmuntion");
        }

        // הצגת איומים
        public IActionResult Threat()
        {
            List<Threat> threats = Data.Get.Threats.
                Include(t => t.Type).
                Include(t => t.TerrorOrg).ToList();
            return View(threats);
        }

        // פתיחת טופס להוספת איום
        public IActionResult CreateThreat()
        {
            List<ThreatAmmuntion>? ta = Data.Get.ThreatAmmuntions.ToList();
            List<TerrorOrg>? orgList = Data.Get.TerrorOrgs.ToList();

            CreateThreatViewModel model = new CreateThreatViewModel
            {
                Types = ta.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList(),
                ThreatOrgs = orgList.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList()
            };
            return View(model);
        }
        //הוספת איום
        [HttpPost]
        public IActionResult CreateThreat(CreateThreatViewModel model)
        {
            TerrorOrg? org = Data.Get.TerrorOrgs.Find(model.OrgId);
            ThreatAmmuntion? threatType = Data.Get.ThreatAmmuntions.Find(model.ThreaTypeId);

            if (org == null || threatType == null)
            {
                return NotFound();
            }
            
            Threat newThreat = new Threat
            {
                TerrorOrg = org,
                Type = threatType,
            };

            Data.Get.Threats.Add(newThreat);
            Data.Get.SaveChanges();

            return RedirectToAction(nameof(Threat));
        }

        
        // עדכון כפתור "שגר" בצד אוייב
        public IActionResult Launch(int id)
        {
            Threat? t = Data.Get.Threats.Find(id);
            if (t == null)
            {
                return NotFound();
            }
            t.Status = Utils.ThreatStatus.active;
            t.FireTime = DateTime.Now;
            Data.Get.SaveChanges();

           
            CancellationTokenSource cts = new CancellationTokenSource();
            // ריצה אסנכרונית
            Task task = Task.Run(async () =>
            {
                // הדפסת הנתונים כל 2 שניות
                int timer = t.ResponceTime;

                // כל עוד אין בקשת ביטול = לא ביקשו ליירט 
                while (!cts.IsCancellationRequested && timer > 0)
                {
                    Console.WriteLine($"{t.Id} threat is {timer} seconds away");
                    await Task.Delay(2000);
                    timer -= 2;
                }
                // אם ביקשו ליירט אות הטיל
                if(cts.IsCancellationRequested)
                {
                    t.Status = Utils.ThreatStatus.failed; 
                }
                else
                {
                    t.Status = Utils.ThreatStatus.succeeded;
                    cts.Cancel();
                  
                }
               
                ThreatMap.Remove(t.Id.ToString());
                Data.Get.SaveChanges();
            }, cts.Token);

         
            ThreatMap[t.Id.ToString()] = cts;


            return RedirectToAction(nameof(Threat));
        }

        //הצגת איומים בצד של צה"ל
        public IActionResult warRoom()
        {
            return View(Data.Get.Threats
                .Include(t => t.Type)
                .Include(t => t.TerrorOrg)
                .ToList()
                .Where(t => t.Status != Utils.ThreatStatus.inActive));
        }


        // יירוט איומים 
        public IActionResult kipa(int tid, int did)
        {
            // למצא את האיום
            Threat t = Data.Get.Threats.Find(tid);
            // למצא הגנה
            DefenceAmmunition? da = Data.Get.DefenceAmmunitions.Find(did);
            //לוודא ששניהם קיימים
            if(t== null || da == null)
            {
                return NotFound();
            }
            if(da.Amount < 1)
            {
                return BadRequest($"{da.Name} אזל מהמלאי תחמושת ההגנה");
            }
            //לבטל את הטאסק ולמחוק את הדקשנרי 
            ThreatMap[tid.ToString()].Cancel();
            ThreatMap.Remove(tid.ToString());
            // הפחתת כמות המירטים
            --da.Amount;
           

            Data.Get.SaveChanges();
            Thread.Sleep(500);

            return RedirectToAction(nameof(warRoom));

        }





























        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
