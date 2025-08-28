using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SRA.Models
{
    public class InputApplicazioneRischioModel
    {
        public List<SelectListItem> ListaApplicazioni { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaRischi { get; set; } = new List<SelectListItem>();
        public string IDApplicazione { get; set; }
        public string IDRischio { get; set; }

        private readonly SRAContext _context;

        public InputApplicazioneRischioModel() { }
        public InputApplicazioneRischioModel(SRAContext context)
        {
            _context = context;
            var listaapplicazioni = _context.Applicazioni.Where(c => !c.Cancellato.Value || c.Cancellato == null).OrderBy(N => N.Nome).ToList();

            this.ListaApplicazioni.Add(new SelectListItem("Seleziona l'applicazione", "Seleziona l'applicazione"));
            foreach (var applicazione in listaapplicazioni)
            {
                this.ListaApplicazioni.Add(new SelectListItem(applicazione.Nome, applicazione.ID.ToString()));
            }

             

        }
    }
}
