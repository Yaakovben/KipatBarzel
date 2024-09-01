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

        // ���� ����� ����
        public IActionResult DefenceAmmuntion()
        {
            List<DefenceAmmunition> defenceAmmunitions = Data.Get.DefenceAmmunitions.ToList();
            return View(defenceAmmunitions);
        }
        // ����� ���� ������ �����
        public IActionResult updateDefenceAmmiunition(int dfid, int amount)
        {
            DefenceAmmunition? da = Data.Get.DefenceAmmunitions.Find(dfid);
            da.Amount = amount;
            Data.Get.SaveChanges();
            return RedirectToAction(nameof(DefenceAmmuntion));
        }

        // ����� ���� ������ ������ ���� ����
        public IActionResult CreateDefenceAmmuntion()
        {
            return View(new DefenceAmmunition());
        }

        // ����� ������ ���� �����
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateDefenceAmmuntion(DefenceAmmunition defence)
        {
            Data.Get.DefenceAmmunitions.Add(defence);
            Data.Get.SaveChanges();
            return RedirectToAction("DefenceAmmuntion");
        }

        // ���� ������
        public IActionResult Threat()
        {
            List<Threat> threats = Data.Get.Threats.
                Include(t => t.Type).
                Include(t => t.TerrorOrg).ToList();
            return View(threats);
        }

        // ����� ���� ������ ����
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
        //����� ����
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

        
        // ����� ����� "���" ��� �����
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
            // ���� ���������
            Task task = Task.Run(async () =>
            {
                // ����� ������� �� 2 �����
                int timer = t.ResponceTime;

                // �� ��� ��� ���� ����� = �� ����� ����� 
                while (!cts.IsCancellationRequested && timer > 0)
                {
                    Console.WriteLine($"{t.Id} threat is {timer} seconds away");
                    await Task.Delay(2000);
                    timer -= 2;
                }
                // �� ����� ����� ��� ����
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

        //���� ������ ��� �� ��"�
        public IActionResult warRoom()
        {
            return View(Data.Get.Threats
                .Include(t => t.Type)
                .Include(t => t.TerrorOrg)
                .ToList()
                .Where(t => t.Status != Utils.ThreatStatus.inActive));
        }


        // ����� ������ 
        public IActionResult kipa(int tid, int did)
        {
            // ���� �� �����
            Threat t = Data.Get.Threats.Find(tid);
            // ���� ����
            DefenceAmmunition? da = Data.Get.DefenceAmmunitions.Find(did);
            //����� ������ ������
            if(t== null || da == null)
            {
                return NotFound();
            }
            if(da.Amount < 1)
            {
                return BadRequest($"{da.Name} ��� ������ ������ �����");
            }
            //���� �� ����� ������ �� ������� 
            ThreatMap[tid.ToString()].Cancel();
            ThreatMap.Remove(tid.ToString());
            // ����� ���� �������
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
