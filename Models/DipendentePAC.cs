using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRA.Models
{
    public class DipendentePAC
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Ruolo { get; set; }
        public string Livello { get; set; }
        public string DataNascita { get; set; }
        public string LuogoNascita { get; set; }
        [Key]
        public string CodiceFiscale { get; set; }
        public string eMail { get; set; }
        public string NomeAccount { get; set; }
        public string CellulareEsteso { get; set; }
        public string SSD { get; set; }

    }
}
