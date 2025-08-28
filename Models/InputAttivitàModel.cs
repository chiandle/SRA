using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class InputAttivitàModel
    {
        public Attivita Attività { get; set; } = new Attivita();
        public List<SelectListItem> ListaTipoAttività { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaStrutture{ get; set; } = new List<SelectListItem>();
        public string AnnoSelezionato { get; set;  }

        public List<AttivitàPersonaDisplay> ListaPersone { get; set; } = new List<Models.AttivitàPersonaDisplay>();

        public List<AttivitàApplicazioneDisplay> ListaApplicazioni { get; set; } = new List<Models.AttivitàApplicazioneDisplay>();

        private readonly SRAContext _context;

        public InputAttivitàModel() 
        {
            this.Attività.DataInizio = DateTime.Now;
            this.Attività.DataFine = DateTime.Now;
        }
        public InputAttivitàModel(SRAContext context)
        {
            _context = context;
            Attività = new Attivita();
            
            var listatipiattività = _context.Parametri.AsNoTracking().Where(t => t.TipoValore == "Tipo Attività").Select(v => v.Valore).ToList();
            this.ListaTipoAttività.Add(new SelectListItem("Seleziona il tipo di attività", "Seleziona il tipo di attività"));
            foreach (var tipoattività in listatipiattività)
            {
                this.ListaTipoAttività.Add(new SelectListItem(tipoattività, tipoattività));
            }

            HelperStrutture strutture = new HelperStrutture(_context);
            this.ListaStrutture = strutture.ListaStrutture;

            this.Attività.DataInizio = DateTime.Now;
            this.Attività.DataFine = DateTime.Now;



        }
    }

    public class AttivitàPersonaDisplay
    {
        public bool Selezionata { get; set; }
        public Guid IDPersona { get; set; }
        public String Nome { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime DataFine { get; set; }
        public int OreImpegno { get; set; }
        public bool IsOwner { get; set; }
      
    }

    public class AttivitàApplicazioneDisplay
    {
        public Guid IDApplicazione { get; set; }
        public String Nome { get; set; }
        public bool Selezionata { get; set; }
    }
}
