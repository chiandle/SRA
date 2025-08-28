using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScottPlot;
using SRA.Models;
using System;
using ScottPlot.Plottables;
using System.Linq;
using System.Threading.Tasks;
using HarfBuzzSharp;
using System.Collections.Generic;
using Windows.UI.Text;

namespace SRA.Pages.ApplicazioniRischi
{
    [Authorize]
    public class StoricoRischioModel : PageModel
    {
        private SRAContext _context;
        [BindProperty]
        public string GraficoRiskRatingUrl { get; set; }
        public string GraficoLikelihoodUrl { get; set; }
        public string GraficoImpactUrl { get; set; }
        public List<VW_Applicazione_Rischio_Storico_Display> ListaStorico { get; set; }
        public Applicazione Applicazione { get; set; }
        public Rischio Rischio { get; set; }

        public StoricoRischioModel(SRAContext context)
        {
            _context = context;
        }
        public async Task OnGet(Guid idapplicazione, Guid idrischio)
        {
            
            ListaStorico = _context.VW_Applicazioni_Rischi_Storico_Display.Where(i => i.IDApplicazione == idapplicazione & i.IDRischio == idrischio).ToList();
            Applicazione = _context.Applicazioni.Where(i => i.ID == idapplicazione).First();
            Rischio = _context.Rischi.Where(i => i.ID == idrischio).First();


            if (ListaStorico.Count() == 0) 
            {
                GraficoRiskRatingUrl = "Nessun dato";
                return; 
            }
            DateTime[] dataRiskRatingX = new DateTime[ListaStorico.Count()];
            double[] dataRiskRatingY = new double[ListaStorico.Count()];
            DateTime[] dataLikelihoodX = new DateTime[ListaStorico.Count()];
            double[] dataLikelihoodY = new double[ListaStorico.Count()]; 
            DateTime[]dataImpactX = new DateTime[ListaStorico.Count()];
            double[] dataImpactY = new double[ListaStorico.Count()]; 
            int idata = 0;
            foreach (var item in ListaStorico) 
            {
                
                dataRiskRatingX[idata] = item.DataUltimaModifica ?? DateTime.Now;
                dataRiskRatingY[idata] = item.Risk_Rating ?? 0.0;
                dataLikelihoodX[idata] = item.DataUltimaModifica ?? DateTime.Now;
                dataLikelihoodY[idata] = item.Likelihood ?? 0.0;
                dataImpactX[idata] = item.DataUltimaModifica ?? DateTime.Now;
                dataImpactY[idata] = item.Impact ?? 0.0;
                idata++;
            }

            
            var nomefile = "./Temp/" + Guid.NewGuid().ToString() + ".png";

            ScottPlot.Plot myPlot = new();
            myPlot.Add.Scatter(dataRiskRatingX, dataRiskRatingY);
            myPlot.Axes.DateTimeTicksBottom();
            myPlot.Axes.Title.Label.Text = "Valutazione del rischio";
            myPlot.SavePng(nomefile, 400, 300);
            GraficoRiskRatingUrl = myPlot.GetPngHtml(400, 300);

            myPlot = new();
            myPlot.Add.Scatter(dataLikelihoodX, dataLikelihoodY);
            myPlot.Axes.DateTimeTicksBottom();
            myPlot.Axes.Title.Label.Text = "Probabilità";

            myPlot.SavePng(nomefile, 400, 300);
            GraficoLikelihoodUrl = myPlot.GetPngHtml(400, 300);

            myPlot = new();
            myPlot.Add.Scatter(dataImpactX, dataImpactY);
            myPlot.Axes.DateTimeTicksBottom();
            myPlot.Axes.Title.Label.Text = "Impatto";

            myPlot.SavePng(nomefile, 400, 300);
            GraficoImpactUrl = myPlot.GetPngHtml(400, 300);

            return;
        }
    }
}
