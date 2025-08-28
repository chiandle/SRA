using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Models
{
    public class Azienda
    {
        public Guid ID { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Ragione Sociale")]
        public string RagioneSociale { get; set; }
        [Display(Name = "Partita IVA")]
        public string? PartitaIVA { get; set; }     
        public bool Attiva {  get; set; }

    }
}
