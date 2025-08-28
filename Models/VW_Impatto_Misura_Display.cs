using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRA.Models
{
    public class VW_Impatto_Misura_Display : ImpattoMisura
    {
        public string NomeApplicazione { get; set; }
        public string NomeRischio { get; set; }
        public string NomeMisura { get; set; }
        

    }
}
