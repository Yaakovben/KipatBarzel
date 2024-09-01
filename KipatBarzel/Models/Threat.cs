using KipatBarzel.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KipatBarzel.Models
{
    public class Threat
    {
        public Threat() 
        {
            Status = ThreatStatus.inActive;
        }

        [Key]
        public int Id { get; set; }

        [NotMapped]

        [Display(Name = "זמן תגובה")]
        public int ResponceTime 
        { 
            get 
            {
                return (int)(((double)TerrorOrg.Distance / (double)Type.Speed) * 3600);
            }  
        }

        [Display(Name ="שם הארגון")]
        public TerrorOrg TerrorOrg { get; set; }

        [Display(Name ="סוג הטיל")]
        public ThreatAmmuntion Type { get; set; }
        [Display(Name = "סטטוס")]
        public ThreatStatus Status { get; set; } // inActiv / failed / succeeded
        [Display(Name = "שעת הירי")]
        public DateTime FireTime { get; set; }  

        
        
    }
}
