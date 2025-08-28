using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using SRA.Areas.Identity.Data;
using SRA.Models;

namespace SRA.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Amministratore")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<SRAUser> _signInManager;
        private readonly UserManager<SRAUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private PACdbContext _pacdbcontext;

        public RegisterModel(
            UserManager<SRAUser> userManager,
            SignInManager<SRAUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager, 
            PACdbContext pacdbcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _pacdbcontext = pacdbcontext;
        }

        public List<SelectListItem> ListaRuoliUtente { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public string Messaggio { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Il campo è obbligatorio.")]
            [AllowedValues("Amministratore", "Operatore", ErrorMessage = "Il valore non è valido.")]
            [Display(Name = "Ruolo")]
            public string RuoloUtente { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            
        }

        public async Task<IActionResult> OnGet(string tipoutente)
        {
            CaricaDatiPagina();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            Messaggio = "";
            if (ModelState.IsValid)
            {
                var nuovoutente = new SRAUser();
                var sqlqry = @"select Matricola, CodiceFiscale, Nome, Cognome, Ruolo, email, Afferenza, Sede, Sede_cod, TelefonoFisso as Telefono, CellulareEsteso as Cellulare, AccountDominio from PAC_V_ANAG_SERVIZIO s
where s.email = '" + Input.Email + "'";
                PACAnagraficaUtente anagraficadapac = _pacdbcontext.PACAnagrafica.FromSqlRaw(sqlqry).FirstOrDefault();
                if (anagraficadapac == null)
                {
                    Messaggio = "Utente non trovato o indirizzo Email non valido";
                    
                }
                nuovoutente.Nome = anagraficadapac.Nome;
                nuovoutente.Cognome = anagraficadapac.Cognome;
                nuovoutente.UserName = anagraficadapac.AccountDominio + "@os.uniroma3.it";
                nuovoutente.Email = Input.Email;
                nuovoutente.NormalizedEmail = Input.Email.ToUpper();
                nuovoutente.SAMAccountName = anagraficadapac.AccountDominio;
                nuovoutente.Struttura = anagraficadapac.Sede;
                nuovoutente.CodiceStruttura = anagraficadapac.Sede_cod;
                nuovoutente.Attivo = true;

                var result = await _userManager.CreateAsync(nuovoutente, "L@StessaPErTutt1");
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente creato");
                    var createduser = _userManager.Users.Where(s => s.SAMAccountName == nuovoutente.SAMAccountName).FirstOrDefault();

                    if (createduser != null)
                    {
                        result = await _userManager.AddToRoleAsync(createduser, Input.RuoloUtente);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    Messaggio = "Errore nella creazione dell'utente";
                }
            }
            else
            {
                // If we got this far, something failed, redisplay form
                Messaggio = "Dati non validi";
                
            }

            if (String.IsNullOrEmpty(Messaggio))
            {
                return RedirectToPage("./GestioneUtenti");
            }
            else
            {
                CaricaDatiPagina();
                return Page();
            }




        }

        private void CaricaDatiPagina()
        {
            Input = new InputModel();
            List<IdentityRole> Ruoli = new List<IdentityRole>();
            Ruoli = _roleManager.Roles.ToList();

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Seleziona il ruolo", Value = "Seleziona il ruolo" });

            foreach (IdentityRole ruoloconfigurato in Ruoli)
            {
                items.Add(new SelectListItem { Text = ruoloconfigurato.Name, Value = ruoloconfigurato.Name });
            }
            ListaRuoliUtente = items.ToList();
        }
    }
}
