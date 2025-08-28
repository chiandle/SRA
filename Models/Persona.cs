using System;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class Persona : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
        public string CodiceFiscale { get; set; }
        public string Matricola { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Afferenza {  get; set; }
        public string CodiceAfferenza { get; set; }
        public bool Cancellato { get; set; }
        public string Email { get; set; }
        public DateTime? DataFineRapporto { get; set; }

    }
}
