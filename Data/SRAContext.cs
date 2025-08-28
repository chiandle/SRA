using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using SRA.Models;

namespace SRA.Models
{
    public class SRAContext : DbContext
    {
        public SRAContext(DbContextOptions<SRAContext> options)
            : base(options)
        {
        }

        public DbSet<SRA.Models.Applicazione> Applicazioni { get; set; }
        public DbSet<SRA.Models.Modulo> Moduli { get; set; }
        public DbSet<SRA.Models.BaseDati> BasiDati { get; set; }
        public DbSet<SRA.Models.Sistema> Sistemi { get; set; }
        public DbSet<SRA.Models.Azienda> Aziende { get; set; }
        public DbSet<SRA.Models.ModuloBasedati> ModuliBasidati { get; set; }
        public DbSet<SRA.Models.DBMS> DBMSs { get; set; }
        public DbSet<SRA.Models.VW_DBMS_Display> VW_DBMSs_Display { get; set; }
        public DbSet<SRA.Models.Rischio> Rischi { get; set; }
        public DbSet<SRA.Models.Misura> Misure { get; set; }
        public DbSet<SRA.Models.TipoRischio> TipiRischio { get; set; }
        public DbSet<SRA.Models.ImpattoMisura> ImpattoMisure { get; set; }
        public DbSet<SRA.Models.Applicazione_Rischio> Applicazioni_Rischi { get; set; }
        public DbSet<SRA.Models.Applicazione_Rischio_Storico> Applicazioni_Rischi_Storico { get; set; }
        public DbSet<SRA.Models.VW_Applicazione_Rischio_Storico_Display> VW_Applicazioni_Rischi_Storico_Display { get; set; }
        public DbSet<SRA.Models.Attivita> ElencoAttività { get; set; }
        public DbSet<VW_SingolaAttività_Display> VW_ElencoAttività_Display { get; set; }
        public DbSet<SRA.Models.Persona> Persone { get; set; }
        public DbSet<SRA.Models.AttivitàPersona> AttivitàPersone { get; set; }
        public DbSet<SRA.Models.AttivitàApplicazione> AttivitàApplicazioni { get; set; }
        public DbSet<SRA.Models.VW_AttivitàPersona> VW_AttivitàPersone { get; set; }
        public DbSet<SRA.Models.WorkPackage> WorkPackages { get; set; }
        public DbSet<SRA.Models.VW_AttivitàWP> VW_AttivitàWPs { get; set; }

        public DbSet<SRA.Models.VW_StrutturaPersona> VW_StrutturePersona { get; set; }

        //public DbSet<SRA.Models.Applicazione_Rischio> Applicazioni_Rischi { get; set; }
        //public DbSet<SRA.Models.Contromisura> Contromisure { get; set; }
        //public DbSet<SRA.Models.Impatto_Contromisura> Impatto_Contromisure { get; set; }
        public DbSet<SRA.Models.Parametro> Parametri { get; set; }
        public DbSet<SRA.Models.VW_Modulo_Display> VW_Moduli_Display { get; set; }
        public DbSet<SRA.Models.VW_BaseDati_Display> VW_BasiDati_Display { get; set; }
        public DbSet<SRA.Models.VW_Rischio_Display> VW_Rischi_Display { get; set; }
        //public DbSet<SRA.Models.VW_Misura_Display> VW_Misure_Display { get; set; }
        public DbSet<VW_Applicazione_Rischio_Display> VW_Applicazioni_Rischi_Display { get; set; }
        public DbSet<VW_Impatto_Misura_Display> VW_Impatto_Misure_Display { get; set; }

        public DbSet<Struttura> Strutture { get; set; }
        public DbSet<VW_AttivitàApplicazione> VW_AttivitàApplicazioni { get; set; }



        public virtual DbSet<SRA.Models.Evento> LogEventi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<ModuloBasedati>()
                .HasKey(c => new { c.IDModulo, c.IDBaseDati });
            modelBuilder.Entity<Applicazione_Rischio>()
                .HasKey(c => new { c.IDApplicazione, c.IDRischio });
            modelBuilder.Entity<AttivitàApplicazione>()
                .HasKey(c => new { c.IDApplicazione, c.IDAttività });
            modelBuilder.Entity<ImpattoMisura>()
                .HasKey(c => new { c.IDMisura, c.IDApplicazione, c.IDRischio });
            modelBuilder.Entity<Parametro>()
                .HasKey(c => new { c.TipoValore, c.Valore });
            modelBuilder.Entity<Applicazione_Rischio>()
            .Property(e => e.Motivazione)
            .HasColumnType("TEXT");
            modelBuilder.Entity<Applicazione_Rischio_Storico>()
        .Ignore(e => e.Cancellato).Property( e => e.Motivazione).HasColumnType("TEXT");
            modelBuilder.Entity<Misura>().Ignore(e => e.Cancellato);
            modelBuilder.Entity<ImpattoMisura>().Ignore(e => e.Cancellato);
            modelBuilder.Entity<Applicazione_Rischio>().Ignore(e => e.Cancellato);
            modelBuilder.Entity<VW_Applicazione_Rischio_Display>().Ignore(e => e.Cancellato);
            modelBuilder.Entity<VW_Impatto_Misura_Display>().Ignore(e => e.Cancellato);

            //UseTpcMappingStrategy (Table per Concrete) indica di mappare una classe su un oggetto del DB con lo stesso
            //nome. In questo caso serve per far si che le classi ereditate siano mappate sulla vista creata nel DB.
            //Altrimenti Entity si aspetta una sola tabella per la classe radice e tutte le derivate con una colonna Discrimator
            //che serve per discriminare a quale classe appartiene ogni record della tabella.
            modelBuilder.Entity<Modulo>().UseTpcMappingStrategy();
            modelBuilder.Entity<BaseDati>().UseTpcMappingStrategy().ToTable("BasiDati");
            modelBuilder.Entity<Rischio>().UseTpcMappingStrategy();
            modelBuilder.Entity<ImpattoMisura>().UseTpcMappingStrategy();
            modelBuilder.Entity<Applicazione_Rischio>().UseTpcMappingStrategy();
            modelBuilder.Entity<VW_BaseDati_Display>().ToTable("VW_BasiDati_Display");
            modelBuilder.Entity<VW_Applicazione_Rischio_Storico_Display>().ToTable("VW_Applicazioni_Rischi_Storico_Display");
            modelBuilder.Entity<DBMS>().UseTpcMappingStrategy();
            modelBuilder.Entity<VW_StrutturaPersona>()
                .HasKey(c => new { c.IDPersona, c.IDStruttura });
            modelBuilder.Entity<VW_AttivitàApplicazione>()
                .HasKey(a => new { a.IDattività, a.IDApplicazione });


        }

    }
}
