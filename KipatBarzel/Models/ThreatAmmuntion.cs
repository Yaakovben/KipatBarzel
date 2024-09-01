using System.ComponentModel.DataAnnotations;

namespace KipatBarzel.Models
{
    public class ThreatAmmuntion
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Speed { get; set; }

    }
}
