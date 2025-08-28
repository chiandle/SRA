using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class VW_AttivitàPersona
    {
        [Key]
        public Guid ID { get; set; }
        public Guid IDPersona { get; set; }
        public Guid IDAttività { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string CodiceFiscale { get; set; }
        public string? Afferenza { get; set; }
        public string? CodiceAfferenza { get; set; }
        public string NomeAttività { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }
        public bool IsOwner {  get; set; }
        public int OreImpegno { get; set; }
        public bool Cancellato { get; set; }
    }

    public class AttivitaPersonaInputModel
    {
        private readonly SRAContext _context;
        public List<SelectListItem> ListaStrutture { get; set; } = new List<SelectListItem>();
        public string StrutturaSelezionata { get; set; }
        public List<SelectListItem> ListaPersone { get; set; } = new List<SelectListItem>();
        public bool Modifica { get; set; }
        public VW_AttivitàPersona RigaAttivitaPersona { get; set; } = new VW_AttivitàPersona();

        public AttivitaPersonaInputModel()
        {

        }

        public AttivitaPersonaInputModel(SRAContext context)
        {
            _context = context;
            HelperStrutture strutture = new HelperStrutture(_context);
            this.ListaStrutture = strutture.ListaStrutture;
        }
        
    }
}
