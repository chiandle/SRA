using System;
using System.Globalization;

namespace SRA.Models
{
    public class VW_AttivitàApplicazione
    {
        public Guid IDattività {  get; set; }
        public Guid IDApplicazione { get; set; }
        public string NomeAttività { get; set; }
        public string NomeApplicazione { get; set; }
    }
}
