using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRA.Models
{
    public class BaseDati :InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public Guid? IDSistema { get; set; }
        public string? Descrizione { get; set; }
        public Guid? IDDbms { get; set; }
        public string? NomeIstanza { get; set; }
        public string? RuoloBasedati { get; set; }
        public float? Dimensione { get; set; }
        public DateTime? DataRilevazione { get; set; }
        public bool Dismessa { get; set; }
        public DateTime? DataDismissione { get; set; }

    }
}
