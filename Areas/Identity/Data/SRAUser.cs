using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SRA.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SRAUser class
    public class SRAUser : IdentityUser
    {
        [PersonalData]
        public string? Ruolo { get; set; }
        [PersonalData]
        public string Cognome { get; set; }
        [PersonalData]
        public string Nome { get; set; }
        [PersonalData]
        public string? Struttura { get; set; }
        [PersonalData]
        public string? CodiceStruttura { get; set; }
        public bool Attivo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataScadenza { get; set; }
        [PersonalData]
        public string? SAMAccountName { get; set; }
        public string? CodiceFiscale { get; set; }
    }
}
