using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class DBMS
    {
        [Key]
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public string? Versione { get; set; }
        public Guid IDProduttore { get; set; }


    }
    public class VW_DBMS_Display : DBMS
    {
        public string Produttore { get; set; }
    }
}
