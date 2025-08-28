using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SRA.Models
{
    public class CSAdbContext : DbContext
    {
        public CSAdbContext(DbContextOptions<CSAdbContext> options)
           : base(options)
        { }

        public virtual DbSet<StruttOrg> DBSetStrutture { get; set; }
        public virtual DbSet<UnitàOrganizzativa> DBSetUnitaOrganizzativa { get; set; }

        public virtual DbSet<DipendenteETL> DBSetETL { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DipendenteETL>()
                .HasKey(c => new { c.DataCampionamento, c.CodiceFiscale });
            //modelBuilder.Entity<ElettoratoAttivoCSA>()
            //    .HasKey(c => new {c.CodiceFiscale });

        }

    }


}
