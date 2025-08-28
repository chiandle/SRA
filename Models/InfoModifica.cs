using System;

namespace SRA.Models
{
    public class InfoModifica
    {
        public bool? Cancellato { get; set; } = false;
        public DateTime? DataUltimaModifica { get; set; }
        public string? AutoreUltimaModifica { get; set; }
        public string? IPUltimaModifica { get; set; }


    }
}
