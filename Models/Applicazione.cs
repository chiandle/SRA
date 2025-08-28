using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SRA.Models
{
    public class Applicazione : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Stato Applicazione")]
        public string? Stato { get; set; }
        [Display(Name = "Data di Avvio")]
        [DataType(DataType.Date)]
        public DateTime? DataAvvio { get; set; }
        [Display(Name = "Data Dismissione")]
        [DataType(DataType.Date)]
        public DateTime? DataDismissione { get; set; }
        [Display(Name = "Tipo Applicazione")]
        public string? TipoApplicazione { get; set; }
        [Display(Name = "Tipo Gestione")]
        public string? TipoGestione { get; set; }
        [Display(Name = "Titolo d'Uso")]
        public string? TitolodUso { get; set; }
        public Guid IDProduttore { get; set; }
        public Guid? IDFornitore { get; set; }


    }
}
