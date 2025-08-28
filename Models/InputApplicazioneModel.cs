using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SRA.Models
{
    public class InputApplicazioneModel
    {
        public Applicazione Applicazione { get; set; } = new Applicazione();
        public List<SelectListItem> ListaStatoApplicazione { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaTipoApplicazione { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaTipoGestione { get; set; } = new List<SelectListItem>();
        

        private readonly SRAContext _context;

        public InputApplicazioneModel() { }
        public InputApplicazioneModel(SRAContext context)
        {
            _context = context;
            Applicazione = new Applicazione();

            var listastatiapplicazione = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Stato Applicazione").Select(v => v.Valore).ToList();
            this.ListaStatoApplicazione.Add(new SelectListItem("Seleziona lo stato dell'applicazione", ""));
            foreach (var statoapplicazione in listastatiapplicazione)
            {
                this.ListaStatoApplicazione.Add(new SelectListItem(statoapplicazione, statoapplicazione));
            }

            var listatipiapplicazione = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Tipo Applicazione").Select(v => v.Valore).ToList();
            this.ListaTipoApplicazione.Add(new SelectListItem("Seleziona il tipo di applicazione", ""));
            foreach (var tipoapplicazione in listatipiapplicazione)
            {
                this.ListaTipoApplicazione.Add(new SelectListItem(tipoapplicazione, tipoapplicazione));
            }

            //var listaproduttori = _context.Aziende.AsNoTracking().ToList();
            
            var listatipigestione = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Tipo Gestione").Select(v => v.Valore).ToList();
            this.ListaTipoGestione.Add(new SelectListItem("Seleziona il tipo di gestione", ""));
            foreach (var tipogestione in listatipigestione)
            {
                this.ListaTipoGestione.Add(new SelectListItem(tipogestione, tipogestione));
            }

            

        }
    }
}
