using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRA.Models
{
    public class Struttura
    {
       
        [Key]
        public string UO { get; set; }
        public string UO_Padre { get; set; }
        public string Nome { get; set; }
        public DateTime DataInizio {  get; set; }
        public DateTime DataFine { get; set; }
        public int Livello { get; set; }
        public string Livello1 { get; set; }
        public string? Livello2 { get; set; }
        public string? Livello3 { get; set; }
        public string? Livello4 { get; set; }
        public string? Livello5 { get; set; }

        
        

    }
}
