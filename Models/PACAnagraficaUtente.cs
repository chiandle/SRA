using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class PACAnagraficaUtente
    {
        public string Matricola { get; set; }
        [Key]
        public string CodiceFiscale { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }

        public string Ruolo { get; set; }
        public string email { get; set; }
        public string Afferenza { get; set; }
        public string Sede { get; set; }
        public string Sede_cod { get; set; }
        public string? Telefono { get; set; }
        public string? Cellulare { get; set; }
        public string? AccountDominio { get; set; }

    }
}
