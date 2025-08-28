using SRA.Areas.Identity.Data;
using SRA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SRA.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Amministratore")]
    public class GestioneRuoliModel : PageModel
    {
        private readonly UserManager<SRAUser> _userManager;
        private readonly SignInManager<SRAUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SRAUserContext _identitycontext;
        public GestioneRuoliModel(
            UserManager<SRAUser> userManager,
            SignInManager<SRAUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            SRAUserContext identitycontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identitycontext = identitycontext;
            _roleManager = roleManager;
        }

        public class RuoloMgt
        {
            public string IDRUolo { get; set; }
            public string NomeRuolo { get; set; }
            public bool RuoloAssegnato { get; set; }

        }

        public class InputRuolo
        {
            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$"), Required, StringLength(30)]
            public string NomeRuolodaCreare { get; set; }
        }
        public InputRuolo inputRuolo;


        public List<RuoloMgt> ListaRuoliMgt { get; set; }
        public string StatusMessage { get; set; }
        public void OnGet(string statusmessage)
        {
            StatusMessage = statusmessage;

            List<IdentityRole> Ruoli = new List<IdentityRole>();
            Ruoli = _roleManager.Roles.ToList();

            ListaRuoliMgt = new List<RuoloMgt>();
            var sqlRuoli = @"SELECT r.id as IDRuolo, ur.UserId as IDUtente
  FROM AspNetRoles r
  inner join AspNetUserRoles ur on r.Id = ur.RoleId
where r.id = @rID";

            int contaUtenti;
            foreach (IdentityRole ruolo in Ruoli)
            {

                contaUtenti = _identitycontext.UtentieRuoli.FromSqlRaw(sqlRuoli, new SqlParameter("rID", ruolo.Id)).Count();

                ListaRuoliMgt.Add(new RuoloMgt
                {
                    NomeRuolo = ruolo.Name,
                    IDRUolo = ruolo.Id,
                    RuoloAssegnato = contaUtenti > 0,
                });
            }
        }

        public async Task<IActionResult> OnPostEliminaRuolo(string nomeRuolo)
        {
            var ruolo = await _roleManager.FindByNameAsync(nomeRuolo);
            var result = await _roleManager.DeleteAsync(ruolo);

            var statusMessage = "ATTENZIONE - Il ruolo " + nomeRuolo + " è stato eliminato";
            return RedirectToPage("./GestioneRuoli", new { statusmessage = statusMessage });
        }

        public async Task<IActionResult> OnPostCreaRuolo(string nomeRuolo)
        {
            IdentityResult IR = null;

            var boolEsisteRuolo = _roleManager.RoleExistsAsync(nomeRuolo);
            if (!await boolEsisteRuolo)
            {
                IR = await _roleManager.CreateAsync(new IdentityRole(nomeRuolo));
            }

            var statusMessage = "ATTENZIONE - Il ruolo " + nomeRuolo + " è stato creato";
            return RedirectToPage("./GestioneRuoli", new { statusmessage = statusMessage });
        }
    }
}
