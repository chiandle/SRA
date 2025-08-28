using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputImpattoMisureModel
    {
        private readonly SRAContext _context;
        public List<SelectListItem>? ListaApplicazioni { get; set; } = new List<SelectListItem>();
        public List<SelectListItem>? ListaMisure { get; set; } = new List<SelectListItem>();
        public List<SelectListItem>? ListaRischi { get; set; } = new List<SelectListItem>();
        public ImpattoMisura ImpattoMisura { get; set; } = new ImpattoMisura();

        public InputImpattoMisureModel() { }
        public InputImpattoMisureModel(SRAContext context) 
        {
            _context = context;
            
            this.ListaApplicazioni.Add(new SelectListItem("Seleziona prima il rischio", ""));
            //var listaapplicazioni = _context.Applicazioni.Where(a => a.Stato != "Dismessa").ToList();
            //this.ListaApplicazioni.Add(new SelectListItem("Tutte", "Tutte"));
            //foreach (var applicazione in listaapplicazioni)
            //{
            //    ListaApplicazioni.Add(new SelectListItem(applicazione.Nome, applicazione.ID.ToString()));
            //}

            var listamisure = _context.Misure.Where(d => d.DataAttivazione <= DateTime.Now & d.DataDisattivazione > DateTime.Now).ToList();
            this.ListaMisure.Add(new SelectListItem("Seleziona la contromisura", ""));
            foreach (var misura in listamisure)
            {
                ListaMisure.Add(new SelectListItem(misura.Nome, misura.ID.ToString()));
            }
            this.ListaRischi.Add(new SelectListItem("Seleziona il rischio", ""));

            var listarischi = _context.Rischi.AsNoTracking().ToList();
            foreach (var rischio in listarischi)
            {
                this.ListaRischi.Add(new SelectListItem(rischio.Nome, rischio.ID.ToString()));
            }
        }
    }

    
}
