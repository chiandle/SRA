using SRA.Areas.Identity.Data;
using SRA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SRA.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Amministratore")]
    public partial class ModificaUtenteModel : PageModel
    {
        private readonly UserManager<SRAUser> _userManager;
        private readonly SignInManager<SRAUser> _signInManager;
        //private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SRAUserContext _identitycontext;
        private readonly SRAContext _contextlocal;

        public List<SelectListItem> ListaRuoliUtente { get; set; }

        public ModificaUtenteModel(
            SRAContext contextlocal,
            SRAUserContext identityContext,
            UserManager<SRAUser> userManager,
            SignInManager<SRAUser> signInManager,
            //IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager
            )
        {
            _contextlocal = contextlocal;
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _roleManager = roleManager;
            _identitycontext = identityContext;
        }

        public class RuoliCB
        {
            public string IDRUolo { get; set; }
            public string NomeRuolo { get; set; }
            public bool RuoloAssegnato { get; set; }
            public bool RuoloNonAssegnato { get; set; }

        }
        
        public class InputModel
        {
            public string Username { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Nome Utente Dominio")]
            public string SAMAccountName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Ruolo")]
            public string Ruolo {  get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Nome")]
            public string Nome { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Cognome")]
            public string Cognome { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Ruolo contrattuale")]
            public string? RuoloContrattuale { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Dipartimento/Direzione")]
            public string? Struttura { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Ufficio")]
            public string? CodiceStruttura { get; set; }
            [Display(Name = "Utente Attivo")]
            public bool Attivo { get; set; }
            [Display(Name = "Data scadenza")]
            [DataType(DataType.Date)]
            public DateTime? DataScadenza { get; set; }
            [Display(Name = "Fornitore")]
            public Guid IDFornitore { get; set; }
            
        }
        [BindProperty]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string? StatusMessage { get; set; }

        [BindProperty]
        public List<RuoliCB> ListaRuoliCB { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public List<string> ListaGruppi { get; set; }

        public List<SelectListItem> ListaFornitori { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(string username, string? statusmessage)
        {

            StatusMessage = statusmessage;
            var user = new SRAUser();
            user = _userManager.Users.Where(s => s.UserName == username).FirstOrDefault();

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }




            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Username = user.UserName,
                SAMAccountName = user.SAMAccountName,
                Email = email,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Struttura = user.Struttura,
                RuoloContrattuale = user.Ruolo,
                CodiceStruttura = user.CodiceStruttura,
                Attivo = user.Attivo,
                DataScadenza = user.DataScadenza
            };

            List<IdentityRole> Ruoli = new List<IdentityRole>();
            Ruoli = _roleManager.Roles.ToList();
            var roles = await _userManager.GetRolesAsync(user);
            Input.Ruolo = roles.First();

            var items = new List<SelectListItem>();
            
            foreach (IdentityRole ruoloconfigurato in Ruoli)
            {
                items.Add(new SelectListItem { Text = ruoloconfigurato.Name, Value = ruoloconfigurato.Name });
            }
            ListaRuoliUtente = items.ToList();


            //ListaRuoliCB = new List<RuoliCB>();

            //foreach (IdentityRole ruolo in Ruoli)
            //{

            //    ListaRuoliCB.Add(new RuoliCB
            //    {
            //        NomeRuolo = ruolo.Name,
            //        IDRUolo = ruolo.Id,
            //        RuoloAssegnato = await _userManager.IsInRoleAsync(user, ruolo.Name),
            //        RuoloNonAssegnato = !(await _userManager.IsInRoleAsync(user, ruolo.Name)),
            //    });

            //};

            

            var listagruppi = _identitycontext.UtentieGruppi.Where(u => u.UserId == user.Id).Join(_identitycontext.Gruppi,
                g1 => g1.GroupId,
                g2 => g2.ID,
                (g1, g2) => new { g2.ID, g2.Description }).ToList();

            ListaGruppi = new List<string>();
            foreach (var g in listagruppi)
            {
                ListaGruppi.Add(g.Description);
            }
            //if (ListaGruppi.Length > 0)
            //{
            //    ListaGruppi = ListaGruppi.Substring(0, ListaGruppi.Length - 1);
            //}
            

            //            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostModificaUtenteAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new SRAUser();
            user = _userManager.Users.Where(s => s.UserName == Input.Username).FirstOrDefault();
            var roles = await _userManager.GetRolesAsync(user);
            var ruoloassegnato  = roles.First();

            if (user == null)
            {
                return NotFound($"Impossibile trovare utente con ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);

            if (Input.Attivo != user.Attivo)
            {
                user.Attivo = Input.Attivo;
            }

            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            if (Input.SAMAccountName != user.SAMAccountName)
            {
                user.SAMAccountName = Input.SAMAccountName;
            }



            if (Input.Nome != user.Nome)
            {
                user.Nome = Input.Nome;
            }

            if (Input.Cognome != user.Cognome)
            {
                user.Cognome = Input.Cognome;
            }
            //if (Input.Ruolo != user.Ruolo)
            //{
            //    user.Ruolo = Input.Ruolo;
            //}
            if (Input.Struttura != user.Struttura)
            {
                user.Struttura = Input.Struttura;
            }
            if (Input.CodiceStruttura != user.CodiceStruttura)
            {
                user.CodiceStruttura = Input.CodiceStruttura;
            }

            
            IdentityResult result;

                if(Input.Ruolo != ruoloassegnato)
            {
                result = await _userManager.RemoveFromRoleAsync(user, ruoloassegnato);
                result = await _userManager.AddToRoleAsync(user, Input.Ruolo);
            }

            if (Input.DataScadenza != user.DataScadenza)
            {
                user.DataScadenza = Input.DataScadenza;
            }



            await _userManager.UpdateAsync(user);
            _contextlocal.SaveChangesAsync();
            StatusMessage = "Il profilo utente è stato aggiornato";
            return RedirectToPage("./ModificaUtente", new { username = user.UserName, statusmessage = StatusMessage });
        }

        public async Task<IActionResult> OnPostCambiaPasswordAsync(string username, string password)
        {
            //if (!ModelState.IsValid )
            //{
            //    StatusMessage = "Errore nei dati ";
            //    return Page();
            //}


            var user = new SRAUser();
            user = _userManager.Users.Where(s => s.UserName == Input.Username).FirstOrDefault();

            if (String.IsNullOrEmpty(password))
            {
                StatusMessage = "La password non può essere vuota";
                return RedirectToPage("./ModificaUtente", new { username = user.UserName, statusmessage = StatusMessage });
            }


            await _userManager.RemovePasswordAsync(user);

            await _userManager.AddPasswordAsync(user, password);
            await _userManager.SetLockoutEnabledAsync(user, false);


            StatusMessage = "La password è stata modificata";
            return RedirectToPage("./ModificaUtente", new { username = user.UserName, statusmessage = StatusMessage });

        }

        public PartialViewResult OnGetUtenteGruppoPartial(string username)
        {
            var utentegruppo = new UtenteeGruppoViewData();
            var user = _userManager.Users.Where(u => u.UserName == username).FirstOrDefault();
            if (user != null)
            {
                utentegruppo.UserId = user.Id;
                utentegruppo.Nome = user.Nome;
                utentegruppo.Cognome = user.Cognome;
            }
            var idgruppidaescludere = _identitycontext.UtentieGruppi.Where(u => u.UserId == user.Id).Select(g => g.GroupId).ToList();
            var listagruppi = _identitycontext.Gruppi.Where(e => !idgruppidaescludere.Contains(e.ID)).ToList();

            utentegruppo.ListaGruppi = new List<SelectListItem>();
            foreach (var g in listagruppi)
            {
                var item = new SelectListItem { Text = g.Name, Value = g.ID };
                utentegruppo.ListaGruppi.Add(item);
            }



            return new PartialViewResult
            {
                ViewName = "_UtentiGruppi",
                ViewData = new ViewDataDictionary<UtenteeGruppoViewData>(ViewData, utentegruppo)
            };
        }

        public async Task<IActionResult> OnPostModificaModalePartialAsync(UtenteeGruppoViewData model)
        {
            var loggedinuser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var user = _identitycontext.Users.Where(u => u.Id == model.UserId).FirstOrDefault();
            var gruppo = _identitycontext.Gruppi.Where(g => g.ID == model.GroupId).FirstOrDefault();
            UtenteeGruppo ug = new UtenteeGruppo
            {
                UserId = model.UserId,
                GroupId = model.GroupId
            };
            _identitycontext.UtentieGruppi.Add(ug);
            _identitycontext.SaveChanges();
            StatusMessage = "L'utente è stato aggiunto al gruppo: " + gruppo.Name;
            return RedirectToPage("./ModificaUtente", new { username = user.UserName, statusmessage = StatusMessage });

        }
        //public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }


        //    var userId = await _userManager.GetUserIdAsync(user);
        //    var email = await _userManager.GetEmailAsync(user);
        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var callbackUrl = Url.Page(
        //        "/Account/ConfirmEmail",
        //        pageHandler: null,
        //        values: new { userId = userId, code = code },
        //        protocol: Request.Scheme);
        //    await _emailSender.SendEmailAsync(
        //        email,
        //        "Confirm your email",
        //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //    StatusMessage = "Verification email sent. Please check your email.";
        //    return RedirectToPage();
        //}
    }
}

