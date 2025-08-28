using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SRA.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Configuration;
using SRA.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SRA.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<SRAUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILDAPAuthenticationService _authService;
        private readonly SRAContext _contextlocal;

        public LoginModel(SignInManager<SRAUser> signInManager, ILogger<LoginModel> logger,
            UserManager<SRAUser> userManager,
            IConfiguration config,
            ILDAPAuthenticationService authService, SRAContext contextlocal)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _config = config;
            _authService = authService;
            _contextlocal = contextlocal;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            public string NomeUtente { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Ricordati di me")]
            public bool RememberMe { get; set; }
        }

        public class LDAPLoginResult
        {
            public bool LoggedIn { get; set; }
            public string Fqdn { get; set; }
            public string Domain { get; set; }
            public string Error { get; set; }
        }
        public async Task OnGetAsync(string returnUrl = null)
        {


            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var ipsorgente = HttpContext.Connection.RemoteIpAddress.ToString();
            var ultimoerrore = _contextlocal.LogEventi.Where(e => e.IPSorgente == ipsorgente & e.TipoEvento == "Errore").OrderByDescending(d => d.DataOra).FirstOrDefault();
            int numeroerroriinfinestra = 0;

            if (ultimoerrore != null)
            {
                var deltaT = (DateTime.Now - ultimoerrore.DataOra).TotalMinutes;

                if (ultimoerrore.Severità > 2 & ultimoerrore.NumeroinFinestra >= 5 & deltaT < 10)
                {
                    RegistraEvento("Errore", ipsorgente, 6, "Login", "Tentativo bloccato per eccessiva numerosità", Input.NomeUtente);
                    ModelState.AddModelError(string.Empty, "Numero di tentativi esaurito");
                    return Page();
                }
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid) { return Page(); }

            //var usaLDAP = _config["UsaLDAP"] ?? "S";

            
            SRAUser user = new SRAUser();

           

            var validationpattern = @"^[a-zA-Z0-9._]*$";
            var errore = new Evento();
            try
            {
                if (!Regex.IsMatch(Input.NomeUtente, validationpattern))
                {
                    ModelState.AddModelError(string.Empty, "Caratteri non ammessi presenti nel nome utente");
                    RegistraEvento("Errore", ipsorgente, 6, "Login", "Caratteri non ammessi presenti nel nome utente", Input.NomeUtente);
                    return Page();
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Eccezione in verifica nome utente");
                RegistraEvento("Errore", ipsorgente, 7, "Login", "Eccezione in verifica nome utente", "ND");
                return Page();
            }


            
            LDAPLoginResult loginresult = new LDAPLoginResult();
            try
            {
                loginresult = await LDAPLogin(Input.NomeUtente, Input.Password);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Tentativo di accesso non valido");
                RegistraEvento("Errore", ipsorgente, 7, "Login", "Eccezione in accesso LDAP", Input.NomeUtente);
                return Page();
            }
            if (loginresult.LoggedIn)
            {
                user = await _userManager.FindByNameAsync(loginresult.Fqdn);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Utente non trovato o indirizzo Email non valido");
                    _logger.LogError("Utente non trovato o indirizzo Email non valido:" + Input.NomeUtente + " {DT} \r\n Utente non trovato in elenco disabili GOMP",
   DateTime.UtcNow.ToLongTimeString());
                    RegistraEvento("Errore", ipsorgente, 3, "Login", "Utente non trovato in elenco disabili GOMP", Input.NomeUtente);

                    return Page();

                }
                else
                {
                    if (!user.Attivo || (user.DataScadenza.HasValue && user.DataScadenza < DateTime.Now.Date))
                    {
                        ModelState.AddModelError(string.Empty, "Tentativo di accesso non valido");
                        _logger.LogError("Tentativo di accesso non valido utente:" + Input.NomeUtente + " {DT} \r\n Utente non attivo",
               DateTime.UtcNow.ToLongTimeString());
                        RegistraEvento("Errore", ipsorgente, 3, "Login", "Utente non attivo", Input.NomeUtente);
                        return Page();
                    }
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Tentativo di accesso non valido");
                RegistraEvento("Errore", ipsorgente, 3, "Login", "Tentativo di accesso LDAP fallito", Input.NomeUtente);
                return Page();
            }
            if (loginresult.LoggedIn)
            {
                RegistraEvento("Informazione", ipsorgente, 0, "Login", "Tentativo di accesso riuscito", Input.NomeUtente);
                

                await _signInManager.SignInAsync(user, Input.RememberMe);
                _logger.LogInformation("Accesso valido utente:" + Input.NomeUtente + " {DT}", DateTime.UtcNow.ToLongTimeString());
                if (returnUrl.Contains("Logout"))
                {
                    return LocalRedirect("/");
                }
                else
                {
                    return LocalRedirect(returnUrl);
                }
            }



            // If we got this far, something failed, redisplay form
            return Page();
        }


        public async Task<LDAPLoginResult> LDAPLogin(string samaccountname, string password)
        {
            LDAPLoginResult result = new LDAPLoginResult();
            result.LoggedIn = false;
            var ambientetest = _config["AmbienteTest"] ?? "N";


            try
            {
                LDAPUser ldapUser = new LDAPUser();

                ldapUser = _authService.Login(samaccountname, password);
                if (ldapUser != null)
                {
                    result.Fqdn = samaccountname + "@os.uniroma3.it";
                    result.Domain = "os.uniroma3.it";
                    result.LoggedIn = true;
                }
                else
                {
                    result.Error = "Errore di autenticazione";
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
            }
            return result;
        }

        public void RegistraEvento(string tipoevento, string ipsorgente, int severità, string categoriaevento, string testoevento, string? nomeutente)
        {
            var evento = new Evento();
            evento.IDLogErrore = Guid.NewGuid();
            evento.TipoEvento = tipoevento;
            evento.IPSorgente = ipsorgente;
            evento.DataOra = DateTime.Now;
            var finestra = evento.DataOra.Subtract(TimeSpan.FromMinutes(30));
            if (tipoevento == "Errore")
            {
                evento.NumeroinFinestra = _contextlocal.LogEventi.Where(e => e.IPSorgente == ipsorgente & e.DataOra.CompareTo(finestra) > 0 & tipoevento != "Informazione").Count();
            }
            evento.Severità = severità;
            evento.CategoriaEvento = categoriaevento;
            evento.TestoEvento = testoevento;
            evento.NomeUtente = nomeutente;
            _contextlocal.Add(evento);
            _contextlocal.SaveChanges();
        }
    }

}
