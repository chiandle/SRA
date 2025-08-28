using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class Attivita :InfoModifica
    {
        [Key]
        public Guid ID {  get; set; }
        public int Anno { get; set; }
        public string Struttura { get; set; }
        public string CodiceStruttura { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public string Nome {  get; set; }
        public string? Descrizione { get; set; }
        public string Tipologia { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }


    }
}
