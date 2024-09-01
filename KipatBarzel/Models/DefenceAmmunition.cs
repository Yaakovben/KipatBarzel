using System.ComponentModel.DataAnnotations;

namespace KipatBarzel.Models
{
    public class DefenceAmmunition
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = " שם הנשק")]
        public string Name { get; set; }
        [Display(Name = "כמות")]
        public int Amount { get; set; }

    }
}
