using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SRA.Areas.Identity.Data;
using Windows.System;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

namespace SRA.Pages.NSElencoAttività
{
    [Authorize]
    public class IndexAttivitàModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;



        public IndexAttivitàModel(SRA.Models.SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }

        public List<VW_SingolaAttività_Display> ElencoAttività { get; set; } = new List<VW_SingolaAttività_Display>();
        public List<VW_AttivitàApplicazione> ElencoAttivitàApplicazioni { get; set; } = new List<VW_AttivitàApplicazione>();
        public List<SelectListItem> ListaAnni { get; set; } = new List<SelectListItem>();
        [BindProperty]
        public string AnnoSelezionato { get; set; }
        [BindProperty]
        public bool SoloPersonali { get; set; } = false;
        public List<SelectListItem> ListaStrutture { get; set; } = new List<SelectListItem>();
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "Filtra per Struttura Organizzativa")]

        public string StrutturaSelezionata { get; set; }

        [BindProperty]
        public string StatusMessage { get; set; }
        private SRAUser UtenteCollegato { get; set; }

        //public ApplicazioneIndexData ApplicazioneVM { get; set; }

        public async Task OnGetAsync(int annoselezionato, string strutturaselezionata, bool? solopersonali)
        {
            UtenteCollegato = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            SoloPersonali = solopersonali.HasValue ? solopersonali.Value : SoloPersonali;

            try
            {
                if (annoselezionato != 0)
                {
                    AnnoSelezionato = annoselezionato.ToString();
                }
                else
                {
                    AnnoSelezionato = DateTime.Now.Year.ToString();
                }
            }
            catch
            {
                AnnoSelezionato = "";
            }
            if (!String.IsNullOrEmpty(strutturaselezionata))
            {
                StrutturaSelezionata = strutturaselezionata;
            }
            else
            {
                StrutturaSelezionata = UtenteCollegato.CodiceStruttura;
            }
            //ApplicazioneVM = new ApplicazioneIndexData();
            //ApplicazioneVM.ApplicazioniModEnum = await _context.ElencoAttività
            //ElencoAttività = await _context.ElencoAttività.AsNoTracking().Where(a => !a.Cancellato.Value || a.Cancellato == null).OrderByDescending(d => d.DataInizio).ToListAsync();
            this.CaricaDatiPagina();

        }

        public void CaricaDatiPagina()
        {
            var idpersona = _context.Persone.AsNoTracking().Where(e => e.Email.ToLower() == UtenteCollegato.Email.ToLower()).Select(i => i.ID).FirstOrDefault();

            ListaAnni.Add(new SelectListItem("Tutti", "1"));
            for (var anno = 2024; anno <= DateTime.Now.Year; anno++)
            {
                ListaAnni.Add(new SelectListItem(anno.ToString(), anno.ToString()));
            }

            if (String.IsNullOrEmpty(AnnoSelezionato)) { AnnoSelezionato = DateTime.Now.Year.ToString(); }

            #region Definizione query OLD-Style
            var sqlquery = @"select  a.*,
                    CAST(CASE 
                    WHEN ap.IDPersona = '" + idpersona.ToString() + @"' THEN 1 
                    ELSE 0 
                END as bit) AS Assegnata 
            from ElencoAttività a
            left join (select distinct IDAttività, IDPersona from AttivitàPersone where IDPersona = '" + idpersona.ToString() + @"' ) ap on ap.IDAttività = a.ID
            where 1=1
            and (a.Cancellato = 0 or a.Cancellato is null)";
            if (AnnoSelezionato != "1")
            {
                sqlquery = sqlquery + " and (YEAR(a.DataInizio) = '" + AnnoSelezionato + "' " +
"or YEAR(a.DataFine) >=  '" + AnnoSelezionato + "')";
            }
            if (SoloPersonali)
            {
                sqlquery = sqlquery + @" and CAST(CASE 
                    WHEN ap.IDPersona = '" + idpersona.ToString() + @"' THEN 1 
                    ELSE 0 
                END as bit) = 1";
            }
            #endregion


            // Query base con LINQ
            var query =
    from a in _context.ElencoAttività
    where a.Cancellato == false || a.Cancellato == null
    select new VW_SingolaAttività_Display
    {
        ID = a.ID,
        Anno = a.Anno,
        Nome = a.Nome,
        Descrizione = a.Descrizione,
        DataInizio = a.DataInizio,
        DataFine = a.DataFine,
        CodiceStruttura = a.CodiceStruttura,
        Struttura = a.Struttura,
        Tipologia = a.Tipologia,
        Assegnata = _context.AttivitàPersone
            .Any(ap => ap.IDAttività == a.ID && ap.IDPersona == idpersona)
    };



            if (AnnoSelezionato != "1")
            {
                var anno = int.Parse(AnnoSelezionato);
                query = query.Where(a => a.DataInizio.Year == anno ||
                                        a.DataFine.Year >= anno);
            }

            if (SoloPersonali)
            {
                query = query.Where(a => a.Assegnata == true);
            }

            HelperStrutture strutture = new HelperStrutture(_context);
            this.ListaStrutture = strutture.ListaStrutture;
            this.ListaStrutture.Add(new SelectListItem("Tutte", "Tutte"));

            StrutturaSelezionata = DeterminaStrutturaEffettiva(StrutturaSelezionata);
            //if(String.IsNullOrEmpty(StrutturaSelezionata) && StrutturaSelezionata != "Tutte")
            //{
            //    StrutturaSelezionata = UtenteCollegato.CodiceStruttura;
            //}

            #region modifica Query OLD-STYLE
            if (StrutturaSelezionata != "Tutte")
            {
                sqlquery = sqlquery + " and a.CodiceStruttura = '" + StrutturaSelezionata + "'";
            }
            #endregion


            if (StrutturaSelezionata != "Tutte")
            {
                query = query.Where(a => a.CodiceStruttura == StrutturaSelezionata);
            }
            //ElencoAttività = _context.VW_ElencoAttività_Display
            //    .FromSqlRaw(sqlquery)
            //    .ToList();

            ElencoAttività = query.ToList();

            ElencoAttivitàApplicazioni = _context.VW_AttivitàApplicazioni.ToList();
        }


        private string DeterminaStrutturaEffettiva(string strutturaselezionata)
        {
            if (string.IsNullOrEmpty(strutturaselezionata) || strutturaselezionata == "Tutte")
            {
                return string.IsNullOrEmpty(strutturaselezionata) ?
                       UtenteCollegato.CodiceStruttura : "Tutte";
            }
            return strutturaselezionata;
        }
        public async Task<PartialViewResult> OnGetAggiungiModalePartial(Guid? idattivita, string? annoselezionato, string? strutturaselezionata)
        {

            UtenteCollegato = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var aggiungiattivitàmodel = new InputAttivitàModel(_context);

            if (!idattivita.HasValue && !(annoselezionato == DateTime.Now.Year.ToString() || annoselezionato == "Tutti"))
            {
                aggiungiattivitàmodel.AnnoSelezionato = annoselezionato;
                aggiungiattivitàmodel.Attività.DataInizio = new DateTime(Int32.Parse(annoselezionato), 1, 1);
                aggiungiattivitàmodel.Attività.DataFine = new DateTime(Int32.Parse(annoselezionato), 12, 31);

            }

            if (!String.IsNullOrEmpty(strutturaselezionata))
            {
                StrutturaSelezionata = strutturaselezionata;
            }

            aggiungiattivitàmodel.Attività.CodiceStruttura = UtenteCollegato.CodiceStruttura;
            aggiungiattivitàmodel.Attività.Struttura = UtenteCollegato.Struttura;

            if (idattivita.HasValue)
            {
                aggiungiattivitàmodel.Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).FirstOrDefault();

            }

            var listaapplicazionidisplay = _context.Applicazioni
                .Where(d => (d.DataAvvio <= aggiungiattivitàmodel.Attività.DataInizio || d.DataAvvio == null) && (d.DataDismissione >= aggiungiattivitàmodel.Attività.DataFine || d.DataDismissione == null) && d.Cancellato == false)
                .OrderBy(n => n.Nome).ToList();
            List<Guid> listaapplicazioniassegnate = _context.AttivitàApplicazioni.Where(m => m.IDAttività == idattivita).Select(b => b.IDApplicazione).ToList();
            bool applicazioneassegnata;
            foreach (var applicazione in listaapplicazionidisplay)
            {
                if (listaapplicazioniassegnate.Contains(applicazione.ID)) { applicazioneassegnata = true; }
                else { applicazioneassegnata = false; }
                aggiungiattivitàmodel.ListaApplicazioni.Add(new AttivitàApplicazioneDisplay
                {
                    IDApplicazione = applicazione.ID,
                    Nome = applicazione.Nome,
                    Selezionata = applicazioneassegnata
                });
            }

            aggiungiattivitàmodel.ListaApplicazioni = aggiungiattivitàmodel.ListaApplicazioni.OrderByDescending(s => s.Selezionata).ThenBy(n => n.Nome).ToList();


            var partialViewResult = new PartialViewResult
            {
                ViewName = "_AggiungiAttività",
                ViewData = new ViewDataDictionary<InputAttivitàModel>(ViewData, aggiungiattivitàmodel)
            };

            return await Task.FromResult(partialViewResult);
        }


        public async Task<IActionResult> OnPostAggiungiAttività(InputAttivitàModel model)
        {


            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            model.Attività.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
            model.Attività.DataUltimaModifica = DateTime.Now;
            model.Attività.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            model.Attività.Anno = model.Attività.DataInizio.Year;
            model.Attività.Struttura = _context.Strutture.Where(o => o.UO == model.Attività.CodiceStruttura).Select(d => d.Nome).First();

            if (model.Attività.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Attività);
                _context.SaveChanges();

            }
            else
            {
                if (model.Attività.DataFine == DateTime.MinValue)
                {
                    model.Attività.DataFine = DateTime.MaxValue;
                }
                var listapersonessegnate = _context.AttivitàPersone.AsNoTracking().Where(a => a.IDAttività == model.Attività.ID).ToList();
                foreach (var ap in listapersonessegnate)
                {
                    if (ap.DataInizio <= model.Attività.DataInizio)
                    {
                        ap.DataInizio = model.Attività.DataInizio;
                    }

                    if (ap.DataFine >= model.Attività.DataFine)
                    {
                        ap.DataFine = model.Attività.DataFine;
                    }
                    _context.Update(ap);
                    _context.SaveChanges();
                }
                _context.Update(model.Attività);
            }
            _context.SaveChanges();
            var applicazioniselezionate = _context.AttivitàApplicazioni.AsNoTracking().Where(a => a.IDAttività == model.Attività.ID).Select(at => at.IDApplicazione).ToList();

            foreach (var applicazione in model.ListaApplicazioni)
            {
                AttivitàApplicazione associazioneattivitàapplicazione = new AttivitàApplicazione
                {
                    IDAttività = model.Attività.ID,
                    IDApplicazione = applicazione.IDApplicazione
                };
                if (applicazione.Selezionata && !applicazioniselezionate.Contains(applicazione.IDApplicazione))
                {
                    _context.Add(associazioneattivitàapplicazione);
                    _context.SaveChanges();
                }
                else
                {
                    if (!applicazione.Selezionata && applicazioniselezionate.Contains(applicazione.IDApplicazione))
                    {
                        _context.Remove(associazioneattivitàapplicazione);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToPage("./IndexElencoAttività", new { AnnoSelezionato });
        }



        public async Task<IActionResult> OnGetEliminaAttivita(Guid idattivita)
        {
            UtenteCollegato = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            try
            {
                Attivita attivitàdacancellare = _context.ElencoAttività.Where(i => i.ID == idattivita).FirstOrDefault();
                bool cancellabile = !_context.AttivitàPersone.AsNoTracking().Where(a => a.ID == idattivita).Any() ||
                    !_context.WorkPackages.Where(a => a.IDAttività == idattivita).Any();
                if (attivitàdacancellare != null & cancellabile)
                {
                    attivitàdacancellare.Cancellato = true;
                    attivitàdacancellare.AutoreUltimaModifica = UtenteCollegato.Nome + " " + UtenteCollegato.Cognome;
                    attivitàdacancellare.DataUltimaModifica = DateTime.Now;
                    attivitàdacancellare.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                    _context.Update(attivitàdacancellare);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

            this.CaricaDatiPagina();
            return Page();
        }

    }
}
