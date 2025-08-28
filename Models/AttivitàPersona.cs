using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class AttivitàPersona :InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public Guid IDAttività { get; set; }
        public Guid IDPersona { get; set; }
        public int OreImpegno {  get; set; }
        public bool Isowner {  get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }
        public string Afferenza { get; set; }
        public string CodiceAfferenza { get; set; }

        
    }

}
