using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class Evento
    {
        [Key]
        public Guid IDLogErrore { get; set; }
        public string TipoEvento { get; set; }
        public string IPSorgente { get; set; }
        public DateTime DataOra { get; set; }
        public int Severità { get; set; }
        public int? NumeroinFinestra { get; set; }
        public string CategoriaEvento { get; set; }
        public string TestoEvento { get; set; }
        public string? NomeUtente { get; set; }

    }
}
