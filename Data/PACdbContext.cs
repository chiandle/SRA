using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SRA.Models
{
    public class PACdbContext : DbContext
    {
        public PACdbContext(DbContextOptions<PACdbContext> options)
           : base(options)
        { }

        public virtual DbSet<PACAnagraficaUtente> PACAnagrafica { get; set; }

        public virtual DbSet<Struttura> PACStrutture { get; set; }

        public virtual DbSet<DipendentePAC> DbSetDipendentePAC { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PACAnagraficaUtente>()
                .ToTable("PAC_V_ANAG_SERVIZIO");
            modelBuilder.Entity<PACAnagraficaUtente>()
                .ToTable("PAC_STRUTTURE");
        }

    }
}
