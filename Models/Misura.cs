using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class Misura : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome {  get; set; }
        [Display(Name = "Data Attivazione")]
        [DataType(DataType.Date)]

        public DateTime DataAttivazione { get; set; }
        [Display(Name = "Data Disattivazione")]
        [DataType(DataType.Date)]
        public DateTime DataDisattivazione { get; set; }
        public string Descrizione { get; set; }

    }
}
