using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KipatBarzel.ViewModels
{
    public class CreateThreatViewModel
    {
       
        public List<SelectListItem> ThreatOrgs { get; set; }

        public int OrgId { get; set; }

       
        public List<SelectListItem> Types { get; set; }

        public int ThreaTypeId { get; set; }


        public List<SelectListItem> Threats { get; set; }

        public int ThreatId { get; set; }

        
    }
}
