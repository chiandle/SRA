using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SRA.Models;
using Microsoft.AspNetCore.Builder;
using SRA.Servizi;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SRA.Areas.Identity.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;


var builder = WebApplication.CreateBuilder(args);

string connectionStringDBLocal = "";
var key = CifraturaAES.key;
var iv = CifraturaAES.iv;
int posizione = 0;
int fine = 0;
string testo = "";
string testodecifrato = "";

if (builder.Environment.EnvironmentName == "Development")
{
    connectionStringDBLocal = builder.Configuration.GetConnectionString("SRAContext");
    

}
else
{
    connectionStringDBLocal = builder.Configuration.GetConnectionString("SRAContext");
}


key = CifraturaAES.key;
iv = CifraturaAES.iv;
posizione = connectionStringDBLocal.IndexOf("Password=") + 9;
fine = connectionStringDBLocal.IndexOf(";", posizione);
testo = connectionStringDBLocal.Substring(posizione, fine - posizione);
testodecifrato = CifraturaAES.Decifra(key, iv, testo);
connectionStringDBLocal = connectionStringDBLocal.Replace(testo, testodecifrato);
builder.Services.AddDbContext<SRAContext>(options =>
    options.UseSqlServer(connectionStringDBLocal)
   .EnableSensitiveDataLogging()
   .LogTo(Console.WriteLine, LogLevel.Information));

var connectionStringPAC = builder.Configuration.GetConnectionString("PACdbContextConnection");
key = CifraturaAES.key;
iv = CifraturaAES.iv;
posizione = connectionStringPAC.IndexOf("Password=") + 9;
fine = connectionStringPAC.IndexOf(";", posizione);
testo = connectionStringPAC.Substring(posizione, fine - posizione);
testodecifrato = CifraturaAES.Decifra(key, iv, testo);
connectionStringPAC = connectionStringPAC.Replace(testo, testodecifrato);
builder.Services.AddDbContext<PACdbContext>(options =>
    options.UseSqlServer(connectionStringPAC));



builder.Services.AddDbContext<SRAUserContext>(options =>
   options.UseSqlServer(connectionStringDBLocal)
   .EnableSensitiveDataLogging()
   .LogTo(Console.WriteLine, LogLevel.Information));

var connstrCSA = builder.Configuration.GetConnectionString("UGOVConnect");
posizione = connstrCSA.IndexOf("Password=") + 9;
fine = connstrCSA.IndexOf(";", posizione);
testo = connstrCSA.Substring(posizione, fine - posizione);
testodecifrato = CifraturaAES.Decifra(key, iv, testo);
connstrCSA = connstrCSA.Replace(testo, testodecifrato);
OracleConfiguration.OracleDataSources.Add("CSAdb", builder.Configuration.GetConnectionString("UGOVContext"));
builder.Services.AddDbContext<CSAdbContext>(options =>
options.UseOracle(connstrCSA));





builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<SRAUser, IdentityRole>()
    .AddEntityFrameworkStores<SRAUserContext>()
    //.AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.Configure<LDAPconfig>(builder.Configuration.GetSection("LDAP"));
builder.Services.AddScoped<ILDAPAuthenticationService, LdapAuthenticationService>();


builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.AddScoped<IGetClaimsProvider, GetClaimsFromUser>();




builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(new AvoidMultiplePostFilter(builder.Configuration));
    });

//Aggiunge gestione sessioni
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var livellodisgnostica = builder.Configuration["LivelloDiagnostica"];

builder.Logging.ClearProviders(); // Rimuove i provider di logging predefiniti
builder.Logging.AddConsole();     // Aggiunge il logging alla console
builder.Logging.SetMinimumLevel(LogLevel.Information); // Configura il livello minimo di log//builder.Logging.ClearProviders();

//if (livellodisgnostica == "Produzione")
//{
//    builder.Logging.AddEventLog();
//}
//else if (livellodisgnostica == "Test")
//{
//    builder.Logging.AddConsole();
//}
//else if (livellodisgnostica == "Sviluppo")
//{
//    builder.Logging.AddConsole();
//}



//builder.Services.AddMvc()
//    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;

var app = builder.Build();

//app.UseStatusCodePagesWithRedirects("/Errors/{0}");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
if (builder.Environment.EnvironmentName == "Development")
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else if (livellodisgnostica == "Sviluppo")
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


var supportedCultures = new[] { "it-IT" };
var localizationOptions = new RequestLocalizationOptions()
.SetDefaultCulture(supportedCultures[0])
.AddSupportedCultures(supportedCultures)
.AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);



//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapRazorPages();

//    endpoints.MapGet("/", async context =>
//    {
//        var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
//        var user = await userManager.GetUserAsync(context.User);

//        if (user != null)
//        {
//            if (await userManager.IsInRoleAsync(user, "Admin"))
//            {
//                context.Response.Redirect("/Admin");
//            }
//            else
//            {
//                context.Response.Redirect("/User");
//            }
//        }
//        else
//        {
//            context.Response.Redirect("/Identity/Account/Login");
//        }
//    });
//});

//app.MapGet("/", async context =>
//{
//    var userManager = context.RequestServices.GetRequiredService<UserManager<SRAUser>>();
//    var user = await userManager.GetUserAsync(context.User);

//    if (user != null)
//    {
//        if (await userManager.IsInRoleAsync(user, "Admin"))
//        {
//            context.Response.Redirect("/Gestione/ElencoStudenti");
//        }
//        else
//        {
//            context.Response.Redirect("/");
//        }
//    }
//    else
//    {
//        context.Response.Redirect("/Identity/Account/Login");
//    }
//}
//        );

//Aggiunge Sessioni nell'App
app.UseSession();
app.MapRazorPages();

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
//    await next();
//});



app.Run();
