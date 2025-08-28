using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputModuliModel
    {
        private readonly SRAContext _context;
        public Modulo Modulo { get; set; } = new Modulo();
        public List<SelectListItem> ListaApplicazioni { get; set; } = new List<SelectListItem>();
        public List<ModuloBasedatiDisplay> ListaBasiDati { get; set; } = new List<Models.ModuloBasedatiDisplay>();
        public List<SelectListItem> ListaAziende { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaTitolodUso { get; set; } = new List<SelectListItem>();
        public Guid? IDApplicazioneSelezionata { get; set; }

        public InputModuliModel() { }
        public InputModuliModel(SRAContext context) 
        {
            _context = context;
            var listaapplicazioni = _context.Applicazioni.Where(a => a.Stato != "Dismessa").ToList();
            
            foreach (var applicazione in listaapplicazioni)
            {
                ListaApplicazioni.Add(new SelectListItem(applicazione.Nome, applicazione.ID.ToString()));
            }

            var listaziende = _context.Aziende.Where(a => a.Attiva.Equals(true)).ToList();
            this.ListaAziende.Add(new SelectListItem("Seleziona l'azienda", ""));
            foreach (var azienda in listaziende)
            {
                ListaAziende.Add(new SelectListItem(azienda.Nome, azienda.ID.ToString()));
            }
            this.ListaTitolodUso.Add(new SelectListItem("Seleziona il titolo d'uso", ""));

            var listatitoliduso = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Titolo d'uso").Select(v => v.Valore).ToList();
            foreach (var tipotitoloduso in listatitoliduso)
            {
                this.ListaTitolodUso.Add(new SelectListItem(tipotitoloduso, tipotitoloduso));
            }
        }
    }

    public class ModuloBasedatiDisplay
    {
        public Guid IDBasedati { get; set; }
        public String Nome { get; set; }
        public bool Selezionata { get; set; }
    }
}
