using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRA.Models
{
    public class Applicazione_Rischio_Storico : InfoModifica
    {
        [Key]
        public Guid ID { get; set; }
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
        public Guid? IDMisura { get; set; }
        //[NotMapped]
        //public Applicazione Applicazione { get; set; }
        //[NotMapped]
        //public Rischio Rischio { get; set; }

        public Applicazione_Rischio_Storico() { }

        public Applicazione_Rischio_Storico(Applicazione_Rischio valutazione)
        {
            IDApplicazione = valutazione.IDApplicazione;
            IDRischio = valutazione.IDRischio;
            Motivazione = valutazione.Motivazione;
            Ease_Discovery = valutazione.Ease_Discovery;
            Ease_Exploit = valutazione.Ease_Exploit;
            Awareness = valutazione.Awareness;
            Intrusion_Detection = valutazione.Intrusion_Detection;
            Loss_Confidentiality = valutazione.Loss_Confidentiality;
            Loss_Integrity = valutazione.Loss_Integrity;
            Loss_Availability = valutazione.Loss_Availability;
            Loss_Accountability = valutazione.Loss_Accountability;
            Financial_Damage = valutazione.Financial_Damage;
            Reputation_Damage = valutazione.Reputation_Damage;
            Non_Compliance = valutazione.Non_Compliance;
            Privacy_Violation = valutazione.Privacy_Violation;
            Risk_Rating = valutazione.Risk_Rating;
            Likelihood = valutazione.Likelihood;
            Impact = valutazione.Impact;

        }

    }
}
