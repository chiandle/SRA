using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class WorkPackage :InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public Guid IDAttività { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizio { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFine { get; set; }
        public bool Notifiche { get; set; }

        public int GiorniPreavviso { get; set; } = 0;
        public int GiorniNotifiche { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataUltimaNotifica { get; set; }
        public bool Completato { get; set; }
        


    }

}
