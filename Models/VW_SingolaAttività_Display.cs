using System.ComponentModel.DataAnnotations;
using System;

namespace SRA.Models
{
    public class VW_SingolaAttività_Display : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public int Anno { get; set; }
        public string Struttura { get; set; }
        public string CodiceStruttura { get; set; }
        public string Nome { get; set; }
        public string? Descrizione { get; set; }
        public string Tipologia { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }
        public bool Assegnata { get; set; }
    }
}
