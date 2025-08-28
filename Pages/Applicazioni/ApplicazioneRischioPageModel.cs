using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SRA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace SRA.Pages.Applicazioni
{
    public class ApplicazioneRischioPageModel : PageModel
    {
        //public List<AssociazioneAppRischio> AssociazioneAppRischioList;
        //public void PopolaAssociazioneApplicazioneRischio(SRAContext context, Applicazione Applicazione)
        //{
        //    var tutti_i_Rischi = context.Rischi;
        //    var ApplicazioneRischio = new HashSet<int>(
        //      Applicazione.Applicazione_Rischio.Select( c=> c.RischioID));
        //    AssociazioneAppRischioList = new List<AssociazioneAppRischio>();
        //    foreach (var rischio in tutti_i_Rischi)
        //    {
        //        AssociazioneAppRischioList.Add(new AssociazioneAppRischio
        //        {
        //            RischioID = rischio.ID,
        //            Nome = rischio.Nome,
        //            Associato = ApplicazioneRischio.Contains(rischio.ID)
        //        });

        //    }
        //}
        //public void AggiornaApplicazioneRischio(SRAContext context, string[] RischiSelezionati, Applicazione ApplicazioneDaAggiornare)
        //{
        //    if (RischiSelezionati == null)
        //    {
        //        ApplicazioneDaAggiornare.Applicazione_Rischio = new List<Applicazione_Rischio>();
        //        return;
        //    }
        //    var RischiSelezionatiHS = new HashSet<string>(RischiSelezionati);
        //    var applicazioneRischi = new HashSet<int>(ApplicazioneDaAggiornare.Applicazione_Rischio.Select(c => c.Rischio.ID));
        //    foreach (var rischio in context.Rischi)
        //    {
        //        if (RischiSelezionatiHS.Contains(rischio.ID.ToString()))
        //        {
        //            if (!applicazioneRischi.Contains(rischio.ID))
        //            {
        //                ApplicazioneDaAggiornare.Applicazione_Rischio.Add(new Applicazione_Rischio
        //                {
        //                    ApplicazioneID = ApplicazioneDaAggiornare.ID,
        //                    RischioID = rischio.ID
        //                });
        //            }
        //        }
        //        else
        //        {
        //            if (applicazioneRischi.Contains(rischio.ID))
        //            {
        //                Applicazione_Rischio rischioDaRimuovere = ApplicazioneDaAggiornare
        //                    .Applicazione_Rischio
        //                    .SingleOrDefault(i => i.RischioID == rischio.ID);
        //                context.Remove(rischioDaRimuovere);
        //            }
        //        }
        //    }
        //}

    }
}
