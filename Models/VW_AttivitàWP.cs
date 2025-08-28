using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class VW_AttivitàWP
    {
        [Key]
        public Guid IDWP { get; set; }
        public Guid IDAttività { get; set; }
        [Required]
        public string Nome { get; set; }
        public string NomeAttività { get; set; }
        public string Descrizione { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }
        public bool Notifiche {  get; set; }
        public int GiorniNotifiche { get; set; }
        public int GiorniPreavviso { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataUltimaNotifica { get; set; }
        public bool Completato { get; set; }

    }

    public class AttivitaWPInputModel
    {
        public List<SelectListItem> ListaWPs { get; set; } = new List<SelectListItem>();
        public bool Modifica { get; set; }
        public VW_AttivitàWP RigaAttivitaWP { get; set; } = new VW_AttivitàWP();

    }
}
