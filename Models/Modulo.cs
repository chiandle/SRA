using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRA.Models
{
    public class Modulo : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome { get; set; }
        [Display(Name = "In produzione")]
        public bool Inproduzione { get; set; }
        [Display(Name = "Data di Avvio")]
        [DataType(DataType.Date)]
        public DateTime? DataAvvio { get; set; }
        [Display(Name = "Data Dismissione")]
        [DataType(DataType.Date)]
        public DateTime? DataDismissione { get; set; }
        public Guid? IDApplicazione { get; set; }
        public Guid? IDProduttore { get; set; }
        public Guid? IDFornitore { get; set; }
        public string? TitolodUso { get; set; }

    }
}
