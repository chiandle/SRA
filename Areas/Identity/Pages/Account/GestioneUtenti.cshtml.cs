using SRA.Areas.Identity.Data;
using SRA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;


namespace SRA.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Amministratore")]
    public class GestioneUtentiModel : PageModel
    {
        private readonly UserManager<SRAUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GestioneUtentiModel(UserManager<SRAUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<SRAUser> ListaUtenti = new List<SRAUser>();
        public string OrdinaCognome { get; set; }
        public string OrdinaUsername { get; set; }
        public string OrdinaStruttura { get; set; }
        public string TipoUtente { get; set; }
        
        [BindProperty]
        public string Ruolo { get; set; }

        public async Task<IActionResult> OnGetAsync(string ordinamento, string UtenteDaCercare, string? ruolo)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var lista = _userManager.Users;
            if (String.IsNullOrEmpty(ruolo))
            {
                ListaUtenti = _userManager.Users.Where(u => !u.UserName.Contains("@stud")).ToList();
            }
            else
            {
                Ruolo = ruolo;
                ListaUtenti = _userManager.Users.Where(u => u.UserName.Contains("@stud")).ToList();
            }


            OrdinaCognome = String.IsNullOrEmpty(ordinamento) ? "cognome_desc" : "";
            OrdinaStruttura = ordinamento == "struttura" ? "Struttura_desc" : "struttura";
            OrdinaUsername = ordinamento == "username" ? "username_desc" : "username";

            switch (ordinamento)
            {
                case "cognome_desc":
                    ListaUtenti = ListaUtenti.OrderByDescending(s => s.Cognome).ToList();
                    break;
                case "struttura":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.Cognome).OrderBy(s => s.Struttura).ToList();
                    break;
                case "struttura_desc":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.Cognome).OrderByDescending(s => s.Struttura).ToList();
                    break;
                case "username":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.UserName).ToList();
                    break;
                case "username_desc":
                    ListaUtenti = ListaUtenti.OrderByDescending(s => s.UserName).ToList();
                    break;
            }

            
            
            //await CaricaDatiPagina(ordinamento, UtenteDaCercare);

            return Page();

        }


        public async void CaricaDatiPagina(string ordinamento, string UtenteDaCercare)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var lista = _userManager.Users;
            ListaUtenti = _userManager.Users.ToList();


            OrdinaCognome = String.IsNullOrEmpty(ordinamento) ? "cognome_desc" : "";
            OrdinaStruttura = ordinamento == "struttura" ? "Struttura_desc" : "struttura";
            OrdinaUsername = ordinamento == "username" ? "username_desc" : "username";

            switch (ordinamento)
            {
                case "cognome_desc":
                    ListaUtenti = ListaUtenti.OrderByDescending(s => s.Cognome).ToList();
                    break;
                case "struttura":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.Cognome).OrderBy(s => s.Struttura).ToList();
                    break;
                case "struttura_desc":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.Cognome).OrderByDescending(s => s.Struttura).ToList();
                    break;
                case "username":
                    ListaUtenti = ListaUtenti.OrderBy(n => n.UserName).ToList();
                    break;
                case "username_desc":
                    ListaUtenti = ListaUtenti.OrderByDescending(s => s.UserName).ToList();
                    break;
            }

            
        }
    }
}
