using SRA.Areas.Identity.Data;
using SRA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Amministratore")]
    public class GestioneGruppiModel : PageModel
    {
        private readonly UserManager<SRAUser> _userManager;
        private readonly SignInManager<SRAUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SRAUserContext _identitycontext;
        public GestioneGruppiModel(
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

        public class GruppoMgt
        {
            public string IDGruppo { get; set; }
            public string NomeGruppo { get; set; }
            public bool GruppoAssegnato { get; set; }
            public string Descrizione { get; set; }

        }

        public class InputGruppo
        {
            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$"), Required, StringLength(30)]
            public string NomeGruppodaCreare { get; set; }
            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$"), Required, StringLength(256)]
            public string DescrizioneGruppodaCreare { get; set; }
        }

        public InputGruppo inputGruppo;


        public List<GruppoMgt> ListaGruppiMgt { get; set; }
        public string StatusMessage { get; set; }
        public void OnGet(string statusmessage)
        {
            StatusMessage = statusmessage;

            List<Gruppo> Gruppi = new List<Gruppo>();
            Gruppi = _identitycontext.Gruppi.OrderBy(g => g.Name).ToList();

            ListaGruppiMgt = new List<GruppoMgt>();
           
            int contaUtenti;
            foreach (Gruppo gruppo in Gruppi)
            {

                var query = _identitycontext.UtentieGruppi.Where(g => g.GroupId == gruppo.ID);
                contaUtenti = query.Count();
                ListaGruppiMgt.Add(new GruppoMgt
                {
                    NomeGruppo = gruppo.Name,
                    IDGruppo = gruppo.ID,
                    Descrizione = gruppo.Description,
                    GruppoAssegnato = contaUtenti > 0,
                });
            }
        }

        public async Task<IActionResult> OnPostEliminaGruppo(string nomegruppo)
        {
                var gruppo = _identitycontext.Gruppi.Where(g => g.Name == nomegruppo).FirstOrDefault();
            if (gruppo != null)
            {
                var result = _identitycontext.Gruppi.Remove(gruppo);
                _identitycontext.SaveChanges();
            }


            var statusMessage = "ATTENZIONE - Il gruppo " + nomegruppo + " è stato eliminato";
            return RedirectToPage("./GestioneGruppi", new { statusmessage = statusMessage });
        }

        public async Task<IActionResult> OnPostCreaGruppo(string nomeGruppo, string descrizioneGruppo)
        {
            IdentityResult IR = null;
            string statusMessage = "";

            var boolEsisteGruppo = _identitycontext.Gruppi.Where(g => g.Name == nomeGruppo).Any();
            if (!boolEsisteGruppo)
            {
                Gruppo nuovogruppo = new Gruppo
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = nomeGruppo,
                    Description = descrizioneGruppo,
                    NormalizedName = nomeGruppo.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                _identitycontext.Add(nuovogruppo);
                _identitycontext.SaveChanges();
                statusMessage = "ATTENZIONE - Il gruppo " + nomeGruppo + " è stato creato";
            }
            else
            {
                statusMessage = "ATTENZIONE - Il gruppo " + nomeGruppo + " è già esistente";
            }

                
            return RedirectToPage("./GestioneGruppi", new { statusmessage = statusMessage });
        }
    }
}
