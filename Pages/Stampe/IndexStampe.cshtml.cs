using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Hosting.Internal;
using SRA.Areas.Identity.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Windows.Devices.Midi;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using SRA.Models;

namespace SRA.Pages.Stampe
{
    [Authorize]
    public class IndexStampeModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;
        private IWebHostEnvironment _hostingEnvironment;

        public IndexStampeModel(SRA.Models.SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public string Elemento { get; set; }
        [BindProperty]
        public string FormatoFile { get; set; }
        public List<SelectListItem> ListaAnni { get; set; } = new List<SelectListItem>();
        [BindProperty]
        public string AnnoSelezionato { get; set; }


        public void OnGet(int annoselezionato)
        {
            try
            {
                if (annoselezionato != 0)
                {
                    AnnoSelezionato = annoselezionato.ToString();
                }
                else
                {
                    AnnoSelezionato =  DateTime.Now.Year.ToString();
                }
            }
            catch
            {
                AnnoSelezionato = DateTime.Now.Year.ToString();
            }
            ListaAnni.Add(new SelectListItem("Tutti", "Tutti"));
            for (var anno = 2024; anno <= DateTime.Now.Year; anno++)
            {
                ListaAnni.Add(new SelectListItem(anno.ToString(), anno.ToString()));
            }

            if (AnnoSelezionato is null || AnnoSelezionato == "") { AnnoSelezionato = DateTime.Now.Year.ToString(); }
            FormatoFile = "PDF";
        }

        public async Task<IActionResult> OnPostStampa()
        {
            string mimetype ="";
            string estensionefile = "";
            string nomefile = "";
            var memory = new MemoryStream();
            
            switch (Elemento)
            {
                case "Applicazioni":
                    nomefile = "ElencoApplicazioni_";
                    switch (FormatoFile)
                    {
                        case "docx":
                            mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            estensionefile = ".docx";
                            memory = CreaDocxAttività(AnnoSelezionato);
                            break;
                        case "PDF":
                            mimetype = "application/pdf";
                            estensionefile = ".pdf";
                            break;

                    }
                    break;
                case "Attività":
                    nomefile = "ElencoAttività_";
                    switch (FormatoFile)
                    {
                        case "docx":
                            mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            estensionefile = ".docx";
                            memory = CreaDocxAttività(AnnoSelezionato);
                            break;
                        case "PDF":
                            mimetype = "application/pdf";
                            estensionefile = ".pdf";
                            break;

                    }
                    break;
                default:
                    return RedirectToPage("./IndexStampe");
                    break;
            }
            switch(FormatoFile)
            {
                case "docx":
                    mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    estensionefile = ".docx";

                    break;
                case "PDF":
                    mimetype = "application/pdf";
                    estensionefile = ".pdf";
                    break;

            }

            string sWebRootFolder = _hostingEnvironment.WebRootPath + "\\Temp\\";
            string sFileName = nomefile +
                DateTime.Today.ToString("ddMMyyyy") + "_Ore_" + DateTime.Now.ToString("HHmm") + estensionefile;
            //sFileName = Regex.Replace(sFileName, "[^A-Za-z0-9_.]", "");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            //FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            
            //using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //System.IO.File.Delete(Path.Combine(sWebRootFolder, sFileName));
            memory.Position = 0;
            return File(memory, mimetype, sFileName);
        }

        public MemoryStream CreaDocxAttività(string AnnoSelezionato)
        {
            MemoryStream memStream = new MemoryStream();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                Paragraph paragraph = new Paragraph();
                // Imposta la formattazione del paragrafo (centrato)
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                Justification justification = new Justification() { Val = JustificationValues.Center };
                paragraphProperties.Append(justification);
                paragraph.PrependChild(paragraphProperties);
                // Imposta la formattazione del testo (grassetto, corpo 14)
                Run run = new Run();
                RunProperties runProperties = new RunProperties();
                Bold bold = new Bold();
                FontSize fontSize = new FontSize() { Val = "28" }; // 14 punti (2 * 14 = 28 half-points)
                runProperties.Append(bold);
                runProperties.Append(fontSize);

                Text text = new Text("Elenco attività");
                run.Append(runProperties);
                run.Append(text);
                paragraph.Append(run);
                body.Append(paragraph);
                paragraph = new Paragraph();
                body.Append(paragraph);
                var listaattività = new List<Attivita>();
                if (AnnoSelezionato != "Tutti")
                {
                    listaattività = _context.ElencoAttività.Where(c => c.Cancellato == false &&  (c.DataInizio.Year.ToString() == AnnoSelezionato || c.DataFine.Year.ToString() == AnnoSelezionato)).ToList();
                }
                else
                {
                    listaattività = _context.ElencoAttività.Where(c => c.Cancellato == false ).ToList();
                }
                    runProperties = new RunProperties();
                fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                runProperties.Append(fontSize);

                foreach ( var attività in listaattività)
                {
                    // Aggiunge nome Attività
                    paragraph = new Paragraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    bold = new Bold();
                    fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                    runProperties.Append(bold);
                    runProperties.Append(fontSize);
                    text = new Text(attività.Nome);
                    run.Append(runProperties);
                    run.Append(text);
                    paragraph.Append(run);
                    body.Append(paragraph);

                    //Aggiunge date
                    paragraph = new Paragraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                    runProperties.Append(fontSize);
                    var datafine = attività.DataFine == DateTime.MaxValue || attività.DataFine.ToString("dd/MM/yyyy") == "31/12/9999" ? " Nessuna scadenza" : " a: " + attività.DataFine.ToString("dd/MM/yyyy");
                    text = new Text("Periodo da: " + attività.DataInizio.ToString("dd/MM/yyyy") + datafine);
                    run.Append(runProperties);
                    run.Append(text);
                    paragraph.Append(run);
                    body.Append(paragraph);

                    //Aggiunge Struttura
                    paragraph = new Paragraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                    runProperties.Append(fontSize);
                    text = new Text("Struttura: " + attività.Struttura);
                    run.Append(runProperties);
                    run.Append(text);
                    paragraph.Append(run);
                    body.Append(paragraph);


                    //Aggiunge Tipo Attività
                    paragraph = new Paragraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                    runProperties.Append(fontSize);
                    text = new Text("Tipo attività: " + attività.Tipologia);
                    run.Append(runProperties);
                    run.Append(text);
                    paragraph.Append(run);
                    body.Append(paragraph);

                    // Aggiunge descrizione
                    paragraph = new Paragraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    fontSize = new FontSize() { Val = "24" }; // 12 punti (2 * 12 = 28 half-points)
                    runProperties.Append(fontSize);
                    text = new Text(attività.Descrizione);
                    run.Append(runProperties);
                    run.Append(text);
                    paragraph.Append(run);
                    body.Append(paragraph);

                    paragraph = new Paragraph();
                    body.Append(paragraph);

                }
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
            return memStream;
        }
    }
}
