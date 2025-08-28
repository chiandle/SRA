using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SRA.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using SRA.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace SRA.Pages.Applicazioni
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        public string FiltroNome { get; set; }
        public string FiltroStato { get; set; }
        public string FiltroTipoGestione { get; set; }
        public string FiltroTipoApplicazione { get; set; }
        [TempData] 
        public string Messaggio { get; set; }
        public string OrdinaNome { get; set; }
        public string OrdinaStato { get; set; }
        public string OrdinaData { get; set; }
        public string OrdinamentoCorrente { get; set; }
        public string FiltroCorrente { get; set; }

        public InputApplicazioneModel DatiModel { get; set; }
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;


        public IndexModel(SRA.Models.SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }

        public IList<Applicazione> Applicazioni { get; set; }

        //public ApplicazioneIndexData ApplicazioneVM { get; set; }

        public async Task OnGetAsync(string ordinamento)
        {
            
            CaricaDatiPagina();

        }

        public void CaricaDatiPagina()
        {
            //string[] valoriPossibili = { "nome_desc", "stato", "stato_desc", "data", "data_desc" };
            //if (!valoriPossibili.Contains(ordinamento))
            //{
            //    if (String.IsNullOrEmpty(ordinamento))
            //    {
            //        OrdinaNome = String.IsNullOrEmpty(ordinamento) ? "nome_desc" : "";
            //        OrdinaStato = ordinamento == "stato" ? "stato_desc" : "stato";
            //        OrdinaData = ordinamento == "data" ? "data_desc" : "data";
            //        OrdinamentoCorrente = ordinamento;

            //    }
            //    Applicazioni = _context.Applicazioni.OrderBy(N => N.Nome).Where(c => !c.Cancellato.Value || c.Cancellato == null)
            //    .ToList();
            //    return;
            //}
            
            DatiModel = new InputApplicazioneModel(_context);
            //ApplicazioneVM = new ApplicazioneIndexData();
            //ApplicazioneVM.ApplicazioniModEnum = await _context.Applicazioni
            Applicazioni = _context.Applicazioni.AsNoTracking().Where(c => !c.Cancellato.Value || c.Cancellato == null).OrderBy(N => N.Nome)
                .ToList();
            //OrdinaNome = String.IsNullOrEmpty(ordinamento) ? "nome_desc" : "";
            //OrdinaStato = ordinamento == "stato" ? "stato_desc" : "stato";
            //OrdinaData = ordinamento == "data" ? "data_desc" : "data";
            //OrdinamentoCorrente = ordinamento;
            //switch (ordinamento)
            //{
            //    case "nome_desc":
            //        Applicazioni = Applicazioni.OrderByDescending(n => n.Nome).ToList();
            //        break;
            //    case "stato":
            //        Applicazioni = Applicazioni.OrderBy(n => n.Stato).ToList();
            //        break;
            //    case "stato_desc":
            //        Applicazioni = Applicazioni.OrderByDescending(n => n.Stato).ToList();
            //        break;
            //    case "data":
            //        Applicazioni = Applicazioni.OrderBy(n => n.DataAvvio).ToList();
            //        break;
            //    case "data_desc":
            //        Applicazioni = Applicazioni.OrderByDescending(n => n.DataAvvio).ToList();
            //        break;
            //    default:
            //        Console.WriteLine("Numero non riconosciuto");
            //        break;
            //}
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idapplicazione)
        {
            var aggiungiapplicazionemodel = new InputApplicazioneModel(_context);
            if(idapplicazione.HasValue)
            {
                aggiungiapplicazionemodel.Applicazione = _context.Applicazioni.AsNoTracking().Where(a => a.ID == idapplicazione).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_AggiungiApplicazione",
                ViewData = new ViewDataDictionary<InputApplicazioneModel>(ViewData, aggiungiapplicazionemodel)
            };
        }

        
        public IActionResult OnPostAggiungiApplicazione(InputApplicazioneModel model)
        {
            if (model.Applicazione.Stato == "Seleziona lo stato dell'applicazione") { model.Applicazione.Stato = null; }
            if (model.Applicazione.TipoApplicazione == "Seleziona il tipo di applicazione") { model.Applicazione.TipoApplicazione = null; }
            if (model.Applicazione.TipoGestione == "Seleziona il tipo di gestione") { model.Applicazione.TipoGestione = null; }
            if (model.Applicazione.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Applicazione);
                _context.SaveChanges();

            }
            else 
                _context.Update(model.Applicazione);
            _context.SaveChanges();

            return RedirectToPage("./IndexApplicazioni");
        }

        public async Task<IActionResult> OnGetEliminaApplicazione(Guid idapplicazione)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            try
            {
                Applicazione applicazionedacancellare = _context.Applicazioni.Where(i => i.ID == idapplicazione).FirstOrDefault();
                bool cancellabile = !(_context.Applicazioni_Rischi.AsNoTracking().Where(a => a.IDApplicazione == idapplicazione).Any()
                    || _context.Moduli.AsNoTracking().Where(m => m.IDApplicazione == idapplicazione).Any());
                if (applicazionedacancellare != null & cancellabile)
                {
                    applicazionedacancellare.Cancellato = true;
                    applicazionedacancellare.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
                    applicazionedacancellare.DataUltimaModifica = DateTime.Now;
                    applicazionedacancellare.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                    _context.Update(applicazionedacancellare);
                    _context.SaveChanges();
                    Messaggio = "L'applicazione \"" + applicazionedacancellare.Nome + "\" è stata cancellata correttamente";
                }
                else
                {
                    Messaggio = "L'applicazione " + applicazionedacancellare.Nome + " non è cancellabile, sono presenti associazioni con moduli o rischi";
                }

            }
            catch (Exception ex)
            {

            }

            CaricaDatiPagina();

            return RedirectToPage("./IndexApplicazioni");
        }

    }
}
