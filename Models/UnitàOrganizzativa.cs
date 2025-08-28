using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class UnitàOrganizzativa
    {
        [Key]
        public string UO_ID { get; set; }
        public string UO_Padre { get; set; }
        public string Nome { get; set; }
        public DateTime Data_in { get; set; }
        public DateTime Data_fin { get; set; }
        public int Livello { get; set; }
        public string Livello1 { get; set; }
        public string Livello2 { get; set; }
        public string Livello3 { get; set; }
        public string Livello4 { get; set; }
        public string Livello5 { get; set; }

    }
}
