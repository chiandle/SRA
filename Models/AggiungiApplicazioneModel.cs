using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SRA.Models
{
    public class AggiungiApplicazioneModel
    {
        public Applicazione Applicazione { get; set; } = new Applicazione();
        public List<SelectListItem> ListaTipoApplicazione { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaTipoGestione { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaTitolodUso { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaProduttore { get; set; } = new List<SelectListItem>();

        private readonly SRAContext _context;

        public AggiungiApplicazioneModel() { }
        public AggiungiApplicazioneModel(SRAContext context)
        {
            _context = context;
            Applicazione = new Applicazione();
            var listatipiapplicazione = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Tipo Applicazione").Select(v => v.Valore).ToList();
            this.ListaTipoApplicazione.Add(new SelectListItem("Seleziona il tipo di applicazione", "Seleziona il tipo di applicazione"));
            foreach (var tipoapplicazione in listatipiapplicazione)
            {
                this.ListaTipoApplicazione.Add(new SelectListItem(tipoapplicazione, tipoapplicazione));
            }

            var listatipigestione = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Tipo Gestione").Select(v => v.Valore).ToList();
            this.ListaTipoGestione.Add(new SelectListItem("Seleziona il tipo di gestione", "Seleziona il tipo di gestione"));
            foreach (var tipogestione in listatipigestione)
            {
                this.ListaTipoGestione.Add(new SelectListItem(tipogestione, tipogestione));
            }

            var listatitoliduso = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Titolo d'uso").Select(v => v.Valore).ToList();
            this.ListaTitolodUso.Add(new SelectListItem("Seleziona il titolo d'uso", "Seleziona il titolo d'uso"));
            foreach (var tipotitoloduso in listatitoliduso)
            {
                this.ListaTitolodUso.Add(new SelectListItem(tipotitoloduso, tipotitoloduso));
            }

        }
    }
}
