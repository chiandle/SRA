using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;

namespace SRA.Models
{
    public class SRAUserContext : IdentityDbContext<SRAUser>
    {
        public SRAUserContext(DbContextOptions<SRAUserContext> options)
            : base(options)
        {
        }
        public virtual DbSet<SRA.Models.UtenteeRuolo> UtentieRuoli { get; set; }
        public virtual DbSet<SRA.Models.UtenteeGruppo> UtentieGruppi { get; set; }
        public virtual DbSet<SRA.Models.Gruppo> Gruppi { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<UtenteeRuolo>()
                       .HasKey(c => new { c.IDRuolo, c.IDUtente });
            builder.Entity<UtenteeGruppo>()
                        .HasKey(c => new { c.GroupId, c.UserId });


            builder.Entity<Gruppo>().ToTable("AspNetGroups");
            builder.Entity<UtenteeGruppo>().ToTable("AspNetUserGroups");
        }
    }
}
