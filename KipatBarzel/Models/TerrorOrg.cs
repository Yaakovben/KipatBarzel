using System.ComponentModel.DataAnnotations;

namespace KipatBarzel.Models
{
    public class TerrorOrg
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Distance { get; set; }
    }
}
