using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Models
{
    public class Rischio
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public Guid IDTipoRischio { get; set; }
        public string DescrizioneRischio { get; set; }
        public bool Globale { get; set; }
        //public ICollection<Applicazione_Rischio> Applicazione_Rischio { get; set; }

    }
}
