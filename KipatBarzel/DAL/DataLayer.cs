using KipatBarzel.Models;
using Microsoft.EntityFrameworkCore;

namespace KipatBarzel.DAL
{
    public class DataLayer : DbContext
    {
        public DataLayer(string cs) : base(GetOptions(cs))
        {
            Database.EnsureCreated();

            Seed();
        }

        private void Seed()
        {
            if (!DefenceAmmunitions.Any())
            {
                DefenceAmmunitions.AddRange(
                    new DefenceAmmunition { Name = "טמיר", Amount = 200 },
                    new DefenceAmmunition { Name = "קלע דוד", Amount = 100 }
                    );
                SaveChanges();
            }
            if (!TerrorOrgs.Any())
            {
                TerrorOrgs.AddRange(
                    new TerrorOrg { Name = "חמאס", Location = "לבנון", Distance = 70 },
                    new TerrorOrg { Name = "חיזבאללה", Location = "לבנון", Distance = 100 },
                    new TerrorOrg { Name = " חות'ים", Location = "תימן", Distance = 2377 },
                    new TerrorOrg { Name = "איראן", Location = "איראן", Distance = 1600 }
                    );
                SaveChanges();
            }
            if (!ThreatAmmuntions.Any())
            {
                ThreatAmmuntions.AddRange(
                    new ThreatAmmuntion { Name = "בליסטי", Speed = 18000 },
                    new ThreatAmmuntion { Name = "רקטה", Speed = 880 },
                    new ThreatAmmuntion { Name ="כטב''ם",Speed = 300 }

                    );
            }
            if (!Threats.Any())
            {
                TerrorOrg? hizballa = TerrorOrgs.FirstOrDefault(h => h.Name == "חיזבאלה");
                ThreatAmmuntion? balisty = ThreatAmmuntions.FirstOrDefault(b => b.Name == "טיל בליסטי");
                if (hizballa != null && balisty != null)
                {
                    Threats.AddRange(
                        new Threat
                        {
                            TerrorOrg = hizballa,
                            Type = balisty
                        }
                        );
                    SaveChanges();

                }


            }

        }

        public DbSet<DefenceAmmunition> DefenceAmmunitions { get; set; }
        public DbSet<TerrorOrg> TerrorOrgs { get; set; }
		public DbSet<Threat> Threats { get; set; }
		public DbSet<ThreatAmmuntion> ThreatAmmuntions { get; set; }




		private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions
                .UseSqlServer(new DbContextOptionsBuilder(), connectionString)
                .Options;
        }
    }
}
