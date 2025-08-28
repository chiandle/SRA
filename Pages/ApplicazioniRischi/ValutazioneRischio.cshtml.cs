using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Pages.ApplicazioniRischi
{
    [Authorize]
    public class ValutazioneRischioModel : PageModel
    {
        private SRAContext _context;
        public VW_Applicazione_Rischio_Display VW_RecordApplicazioneRischio {  get; set; }
        [BindProperty]
        public Applicazione_Rischio RecordApplicazioneRischio { get; set; } = new Applicazione_Rischio();
        [BindProperty]

        public List<SelectListItem> ListaMisure { get; set; } = new List<SelectListItem>();
        [BindProperty]
        public Guid? MisuraApplicata { get; set; }
        [BindProperty]
        public string? Messaggio { get; set; }

        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;

        public ValutazioneRischioModel(SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }
        public void OnGet(string idapplicazione, string idrischio)
        {
           
            RecordApplicazioneRischio.IDApplicazione = Guid.Parse(idapplicazione);
            RecordApplicazioneRischio.IDRischio = Guid.Parse(idrischio);
            CaricaDatiPagina();
        }

        public async Task<IActionResult> OnPostSalvaValutazione()
        {
            if (!ModelState.IsValid)
            {
                Messaggio = "Dati non validi";

                CaricaDatiPagina();
                return Page();
            }
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            //RecordApplicazioneRischio.CalcoloImpatto();
            //RecordApplicazioneRischio.CalcoloProbabilità();
            var recorddamodificare = _context.Applicazioni_Rischi.AsNoTracking().Where(r => r.IDApplicazione == RecordApplicazioneRischio.IDApplicazione & r.IDRischio == RecordApplicazioneRischio.IDRischio).First();
            var ultimavalida = _context.Applicazioni_Rischi_Storico.AsNoTracking().
                Where(r => r.IDApplicazione == RecordApplicazioneRischio.IDApplicazione & r.IDRischio == RecordApplicazioneRischio.IDRischio).
                OrderByDescending(d => d.DataUltimaModifica).FirstOrDefault();
            RecordApplicazioneRischio.CalcoloValutazione();
            RecordApplicazioneRischio.Motivazione = recorddamodificare.Motivazione;
            RecordApplicazioneRischio.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
            RecordApplicazioneRischio.DataUltimaModifica = DateTime.Now;
            RecordApplicazioneRischio.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            _context.Update(RecordApplicazioneRischio);
            if (!RecordApplicazioneRischio.Equals(ultimavalida) && RecordApplicazioneRischio.ValutazioneValida)
            {
                Applicazione_Rischio_Storico storico = new Applicazione_Rischio_Storico(RecordApplicazioneRischio);
                storico.ID = Guid.NewGuid();
                storico.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
                storico.DataUltimaModifica = DateTime.Now;
                storico.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                storico.IDMisura = MisuraApplicata;
                _context.Add(storico);
            }
                    
            _context.SaveChanges();
            //CaricaDatiPagina();
            return  RedirectToPage("./ValutazioneRischio", new { idapplicazione = RecordApplicazioneRischio.IDApplicazione, idrischio = RecordApplicazioneRischio.IDRischio }); ;
        }

        public IActionResult OnPostSalvaMotivazione(string motivazione)
        {
            if (!ModelState.IsValid)
            {
                Messaggio = "Dati non validi";

                CaricaDatiPagina();
                return Page();
            }
            var recorddamodificare = _context.Applicazioni_Rischi.Where(r => r.IDApplicazione == RecordApplicazioneRischio.IDApplicazione & r.IDRischio == RecordApplicazioneRischio.IDRischio).First();
            recorddamodificare.Motivazione = motivazione;
            _context.Update(recorddamodificare);
            _context.SaveChanges();
            CaricaDatiPagina();
            return Page();
        }

        public void CaricaDatiPagina()
        {
            VW_RecordApplicazioneRischio = new VW_Applicazione_Rischio_Display();
            VW_RecordApplicazioneRischio = _context.VW_Applicazioni_Rischi_Display.AsNoTracking().Where(ar => ar.IDApplicazione == RecordApplicazioneRischio.IDApplicazione && ar.IDRischio == RecordApplicazioneRischio.IDRischio).First();
            RecordApplicazioneRischio = (Applicazione_Rischio)VW_RecordApplicazioneRischio;
            RecordApplicazioneRischio.ValutazioneValida = false;
            var listamisure = _context.VW_Impatto_Misure_Display.Where(m => m.IDRischio == RecordApplicazioneRischio.IDRischio && m.IDApplicazione == RecordApplicazioneRischio.IDApplicazione).ToList();
            this.ListaMisure.Add(new SelectListItem("Seleziona la contromisura", ""));
            foreach (var misura in listamisure)
            {
                ListaMisure.Add(new SelectListItem(misura.NomeMisura, misura.IDMisura.ToString()));
            }

        }
       
    }
}
