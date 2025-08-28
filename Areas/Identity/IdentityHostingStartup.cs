using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SRA.Areas.Identity.Data;
using SRA.Models;

[assembly: HostingStartup(typeof(SRA.Areas.Identity.IdentityHostingStartup))]
namespace SRA.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) =>
            //{
            //    services.AddDbContext<SRAUserContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("SRAUserContextConnection")));

            //    services.AddDefaultIdentity<SRAUser>()
            //        .AddEntityFrameworkStores<SRAUserContext>();
            //});
        }
    }
}