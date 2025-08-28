using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Pages.ApplicazioniRischi
{
    [Authorize]
    public class IndexApplicazioniRischiModel : PageModel
    {
        private SRAContext _context;
        public List<VW_Applicazione_Rischio_Display> ListaApplicazioniRischi {  get; set; }

        public class RischioDto
        {
            public string IDRischio { get; set; }
            public string NomeRischio { get; set; }
        }
        public IndexApplicazioniRischiModel(SRAContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            ListaApplicazioniRischi = _context.VW_Applicazioni_Rischi_Display.ToList();
        }


        public PartialViewResult OnGetAggiungiModalePartial()
        {
            var aggiungivalutazione = new InputApplicazioneRischioModel(_context);
            
            return new PartialViewResult
            {
                ViewName = "_AggiungiApplicazioneRischio",
                ViewData = new ViewDataDictionary<InputApplicazioneRischioModel>(ViewData, aggiungivalutazione)
            };
        }

        public IActionResult OnGetElencoRischiDisponibili(string idapplicazione)
        {
            var guididapplicazione = Guid.Parse(idapplicazione);
            var listarischi = (from rischio in _context.Rischi
                               where !(from ar in _context.Applicazioni_Rischi
                                       where ar.IDApplicazione == guididapplicazione
                                       select ar.IDRischio).Contains(rischio.ID)
                               select new RischioDto
                               {
                                   IDRischio = rischio.ID.ToString(),
                                   NomeRischio = rischio.Nome
                               }).Distinct().ToList();
            var jsonresult = JsonConvert.SerializeObject(listarischi);
            return Content(jsonresult, "application/json");
        }

        public IActionResult OnPostAggiungiApplicazioneRischio(InputApplicazioneRischioModel model)
        {
            Applicazione_Rischio valutazionedaaggiungere = new Applicazione_Rischio();
            valutazionedaaggiungere.IDApplicazione = Guid.Parse(model.IDApplicazione);
            valutazionedaaggiungere.IDRischio = Guid.Parse(model.IDRischio);
            _context.Add(valutazionedaaggiungere);
            _context.SaveChanges();

            return RedirectToPage("./IndexApplicazioniRischi");
        }



    }
}
