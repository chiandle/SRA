using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRA.Models
{
    public class DipendenteETL
    {
        public string DataCampionamento { get; set; }
        public string Afferenza { get; set; }
        public string DescrizioneAfferenza { get; set; }
        public string Matricola { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Sesso { get; set; }
        public string Responsabile { get; set; }
        public string DataInizioCollaborazione { get; set; }
        public string RuoloContrattuale { get; set; }
        public string Ruolo { get; set; }

        public string Attivita { get; set; }
        public string DescrizioneAttivita { get; set; }
        public string TipoImpegno { get; set; }
        public string PercentualePartTime { get; set; }
        public string Inquadramento { get; set; }
        public string Livello { get; set; }
        public string GerarchiaLivello { get; set; }
        public string DataNascita { get; set; }
        public string LuogoNascita { get; set; }
        public string CodiceFiscale { get; set; }
        public string eMail { get; set; }
        public string AreaScientificoDisciplinare { get; set; }
    }

}
