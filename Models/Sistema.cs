using System;

namespace SRA.Models
{
    public class Sistema
    {
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public Guid? IDLuogoCustodia { get; set; }
        public Guid? IDCluster { get; set; }
        public string? SistemaOperativo { get; set; }
        public bool Inproduzione { get; set; }
        public DateTime? DataDismissione { get; set; }
    }
}
