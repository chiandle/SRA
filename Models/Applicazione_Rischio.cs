using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SRA.Models
{
    public class Applicazione_Rischio :InfoModifica
    {
        public Guid IDApplicazione { get; set; }
        public Guid IDRischio { get; set; }
        [Column(TypeName = "TEXT")]
        public string Motivazione { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Ease_Discovery { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Ease_Exploit { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Awareness { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Intrusion_Detection { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Loss_Confidentiality { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Loss_Integrity { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Loss_Availability { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Loss_Accountability { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Financial_Damage { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Reputation_Damage { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Non_Compliance { get; set; }
        [RegularExpression("^[0-9]$", ErrorMessage = "Inserisci un intero tra 0 e 9")]
        public int? Privacy_Violation { get; set; }
        public double? Risk_Rating { get; set; }
        public double? Likelihood { get; set; }
        public double? Impact { get; set; }
        public bool ValutazioneValida { get; set; }
        //[NotMapped]
        //public Applicazione Applicazione { get; set; }
        //[NotMapped]
        //public Rischio Rischio { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Applicazione_Rischio_Storico other = (Applicazione_Rischio_Storico)obj;

            return Ease_Discovery == other.Ease_Discovery &&
                   Ease_Exploit == other.Ease_Exploit &&
                   Awareness == other.Awareness &&
                   Intrusion_Detection == other.Intrusion_Detection &&
                   Loss_Confidentiality == other.Loss_Confidentiality &&
                   Loss_Availability == other.Loss_Availability &&
                   Loss_Accountability == other.Loss_Accountability &&
                   Financial_Damage == other.Financial_Damage &&
                   Reputation_Damage == other.Reputation_Damage &&
                   Non_Compliance == other.Non_Compliance &&
                   Privacy_Violation == other.Privacy_Violation &&
                   Likelihood == other.Likelihood &&
                   Impact == other.Impact;
        }


        public void CalcoloProbabilità()
        {
            int?[] variables = { Ease_Discovery, Ease_Exploit, Awareness, Intrusion_Detection };
            int nonnulli = 0;
            int somma = 0;
            foreach (var variable in variables)
            {
                if (variable != null | variable == 0)
                {
                    nonnulli++;
                    somma = somma + variable.Value;
                }
            }
            this.Likelihood = Convert.ToDouble(somma) / Convert.ToDouble(nonnulli);
        }

        public void CalcoloImpatto()
        {
            int?[] variables = { Loss_Accountability, Loss_Availability, Loss_Confidentiality, Loss_Integrity,
            Financial_Damage, Reputation_Damage, Non_Compliance, Privacy_Violation};
            int nonnulli = 0;
            int somma = 0;
            foreach (var variable in variables)
            {
                if (variable != null | variable == 0)
                {
                    nonnulli++;
                    somma = somma + variable.Value;
                }
            }
            this.Impact = Convert.ToDouble(somma) / Convert.ToDouble(nonnulli);
            if (nonnulli == 4) {ValutazioneValida = true;}
        }

        public void CalcoloValutazione()
        {
            this.CalcoloProbabilità();
            this.CalcoloImpatto();

            
            if (Impact <= 3)
            {
                if (Likelihood <= 3) { Risk_Rating = 1; }
                else if (Likelihood <= 6) { Risk_Rating = 2; }
                else { Risk_Rating = 3; };
            }
            else if (Impact <= 6)
            {
                if (Likelihood <= 3) { Risk_Rating = 2; }
                else if (Likelihood <= 6) { Risk_Rating = 3; }
                else { Risk_Rating = 4; };
            }
            else
            {
                if (Likelihood <= 3) { Risk_Rating = 3; }
                else if (Likelihood <= 6) { Risk_Rating = 4; }
                else { Risk_Rating = 5; };
            };

        }
    }
}
