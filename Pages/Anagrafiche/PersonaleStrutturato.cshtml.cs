using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;
using SVPUtil.Data;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Util;
using System.Collections.Generic;


namespace SRA.Pages.Anagrafiche
{

    public class PersonaleStrutturatoModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;
        private readonly PACdbContext _pacdbcontext;
        private readonly CSAdbContext _csadatacontext;
        private IWebHostEnvironment _hostingEnvironment;
        public DateTime DataAnalisi { get; set; }
        private SRAUser UtenteCollegato { get; set; }

        private string Inquadramento(string livello)
        {
            string inquadramento;

            switch (livello.Substring(0, 1))
            {
                case "B":
                    inquadramento = "B";
                    break;
                case "C":
                    inquadramento = "C";
                    break;
                case "D":
                    inquadramento = "D";
                    break;
                case "E":
                    inquadramento = "E";
                    break;
                case "Z":
                    inquadramento = "CEL";
                    break;
                case "0":
                    inquadramento = "Dirigente";
                    break;
                default:
                    inquadramento = "N.D.";
                    break;
            }

            return inquadramento;
        }


        public PersonaleStrutturatoModel(SRA.Models.SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor,
            PACdbContext pacdbcontext,
            CSAdbContext csadatacontext,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
            _pacdbcontext = pacdbcontext;
            _csadatacontext = csadatacontext;
            _hostingEnvironment = hostingEnvironment;

        }
        public void OnGet(DateTime? data)
        {

            DataAnalisi = data == null ? DateTime.Now : data.Value;
            //var costanti = new Costanti(datacampionamento);
            //var listadipendenti = _csadatacontext.DBSetETL.FromSqlRaw(costanti.qryDipendentiETL).AsNoTracking().ToList();

            //var listaPAC = _pacdbcontext.DbSetDipendentePAC.FromSqlRaw(costanti.qryDipendentiPAC).AsNoTracking().ToList();
            //ListaStrutture listastrutture = new ListaStrutture();
            //listastrutture.Lista = _csadatacontext.DBSetUnitaOrganizzativa.FromSqlRaw(costanti.qryStrutture).AsNoTracking().ToList();


        }

        public async Task<IActionResult> OnPostEsportaTAB(DateTime? data)
        {
            UtenteCollegato = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            string datacampionamento;
            DataAnalisi = data == null ? DateTime.Now : data.Value;
            datacampionamento = DataAnalisi.Date.ToString("dd/MM/yyyy");

            string sWebRootFolder = _hostingEnvironment.WebRootPath + "\\Temp\\";
            string sFileName = @"PersonaleTAB_"
                               + datacampionamento
                               + ".xlsx";
            sFileName = Regex.Replace(sFileName, "[^A-Za-z0-9_. ]", "");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
            XSSFSheet excelSheet = workbook.CreateSheet("TAB") as XSSFSheet;

            ICellStyle stileData = workbook.CreateCellStyle();
            var newDataFormat = workbook.CreateDataFormat();
            stileData.DataFormat = newDataFormat.GetFormat("dd/MM/yyyy");

            var evidenziata = workbook.CreateCellStyle();
            evidenziata.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            evidenziata.FillPattern = FillPattern.SolidForeground;

            IRow row = excelSheet.CreateRow(0);
            int icol = 0;
            row.CreateCell(icol).SetCellValue("Data Campionamento");
            icol++;
            row.CreateCell(icol).SetCellValue("Codice Fiscale");
            icol++;
            row.CreateCell(icol).SetCellValue("Matricola");
            icol++;
            row.CreateCell(icol).SetCellValue("Nome");
            icol++;
            row.CreateCell(icol).SetCellValue("Cognome");
            icol++;
            row.CreateCell(icol).SetCellValue("Sesso");
            icol++;
            row.CreateCell(icol).SetCellValue("Data Nascita");
            icol++;
            row.CreateCell(icol).SetCellValue("Luogo Nascita");
            icol++;
            row.CreateCell(icol).SetCellValue("Direzione afferenza");
            icol++;
            row.CreateCell(icol).SetCellValue("Afferenza");
            icol++;
            row.CreateCell(icol).SetCellValue("Descrizione Afferenza");
            icol++;
            row.CreateCell(icol).SetCellValue("Responsabile");
            icol++;
            row.CreateCell(icol).SetCellValue("Data inizio collaborazione");
            icol++;
            row.CreateCell(icol).SetCellValue("Tipo Impegno");
            icol++;
            row.CreateCell(icol).SetCellValue("Percentuale Part Time");
            icol++;
            row.CreateCell(icol).SetCellValue("Ruolo Contrattuale");
            icol++;
            row.CreateCell(icol).SetCellValue("Ruolo");
            icol++;
            row.CreateCell(icol).SetCellValue("Inquadramento");
            icol++;
            row.CreateCell(icol).SetCellValue("Livello");
            icol++;
            row.CreateCell(icol).SetCellValue("Gerarchia Livello");
            icol++;
            row.CreateCell(icol).SetCellValue("Attività");
            icol++;
            row.CreateCell(icol).SetCellValue("Descrizione Attività");
            icol++;
            row.CreateCell(icol).SetCellValue("eMail");
            icol++;
            row.CreateCell(icol).SetCellValue("Nome Account");
            icol++;
            row.CreateCell(icol).SetCellValue("Cellulare Esteso");
            icol++;
            row.CreateCell(icol).SetCellValue("Area Scientifico Disciplinare");
            icol++;
            row.CreateCell(icol).SetCellValue("SSD");
            int idip = new int();
            var cultureInfo = new CultureInfo("it-IT");

            var anno = Int32.Parse(datacampionamento.Substring(datacampionamento.Length - 4));

            var costanti = new Costanti(datacampionamento);
            var listadipendenti = _csadatacontext.DBSetETL.FromSqlRaw(costanti.qryDipendentiETL).AsNoTracking().ToList();

            var listaPAC = _pacdbcontext.DbSetDipendentePAC.FromSqlRaw(costanti.qryDipendentiPAC).AsNoTracking().ToList();
            ListaStrutture listastrutture = new ListaStrutture();
            foreach (DipendenteETL dipendente in listadipendenti)
            {
                idip++;

                var datainiziocollaborazione = DateTime.Parse(dipendente.DataInizioCollaborazione, cultureInfo);
                var datanascita = DateTime.Parse(dipendente.DataNascita, cultureInfo);
                row = excelSheet.CreateRow(idip);
                var dipendentePAC = listaPAC.Where(d => d.CodiceFiscale == dipendente.CodiceFiscale).FirstOrDefault();
                icol = 0;
                row.CreateCell(icol).SetCellValue(dipendente.DataCampionamento);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.CodiceFiscale);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Matricola);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Nome);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Cognome);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Sesso);
                icol++;
                //row.CreateCell(icol).SetCellValue(dipendente.DataNascita);
                var cellDataNascita = row.CreateCell(icol);
                cellDataNascita.CellStyle = stileData;
                cellDataNascita.SetCellValue(datanascita);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.LuogoNascita);
                var strutturaafferenza = listastrutture.Lista.Where(a => a.UO_ID == dipendente.Afferenza).First();
                var direzioneafferenza = "";
                if (strutturaafferenza != null)
                {
                    direzioneafferenza = String.IsNullOrEmpty(strutturaafferenza.Livello2) & strutturaafferenza.Livello1 == "Direzione Generale" ? "Direzione Generale" : strutturaafferenza.Livello2;
                }
                else
                {
                    direzioneafferenza = "Direzione non trovata";
                }
                icol++;
                row.CreateCell(icol).SetCellValue(direzioneafferenza);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Afferenza);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.DescrizioneAfferenza);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Responsabile);
                icol++;
                //row.CreateCell(icol).SetCellValue(dipendente.DataInizioCollaborazione);
                var cellDataInizioRapporto = row.CreateCell(icol);
                cellDataInizioRapporto.CellStyle = stileData;
                cellDataInizioRapporto.SetCellValue(datainiziocollaborazione);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.TipoImpegno);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.PercentualePartTime);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.RuoloContrattuale);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Ruolo);
                icol++;
                if (anno >= 2024)
                {
                    row.CreateCell(icol).SetCellValue(dipendente.Inquadramento);
                }
                else
                {
                    row.CreateCell(icol).SetCellValue(this.Inquadramento(dipendente.Inquadramento));
                }

                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Livello);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.GerarchiaLivello);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.Attivita);
                icol++;
                row.CreateCell(icol).SetCellValue(dipendente.DescrizioneAttivita);

                if (dipendentePAC != null)
                {
                    icol++;
                    row.CreateCell(icol).SetCellValue(dipendentePAC.eMail);
                    icol++;
                    row.CreateCell(icol).SetCellValue(dipendentePAC.NomeAccount);
                    icol++;
                    row.CreateCell(icol).SetCellValue(dipendentePAC.CellulareEsteso);

                }
                else
                {
                    icol++;
                    row.CreateCell(icol).SetCellValue(dipendente.eMail);
                    icol++;
                    row.CreateCell(icol);
                    icol++;
                    row.CreateCell(icol);
                }
                icol++;
                row.CreateCell(icol);
                icol++;
                row.CreateCell(icol);


            }

            uint icolt = 1;

            XSSFTable xssfTable = excelSheet.CreateTable();
            CT_Table ctTable = xssfTable.GetCTTable();
            AreaReference myDataRange = new AreaReference(new CellReference(0, 0), new CellReference(idip, icol));
            ctTable.@ref = myDataRange.FormatAsString();
            ctTable.id = 1;
            ctTable.name = "Tabella1";
            ctTable.displayName = "Tabella1";
            ctTable.tableStyleInfo = new CT_TableStyleInfo();
            ctTable.tableStyleInfo.name = "TableStyleMedium2"; // TableStyleMedium2 is one of XSSFBuiltinTableStyle
            ctTable.tableStyleInfo.showRowStripes = true;
            ctTable.tableColumns = new CT_TableColumns();
            ctTable.tableColumns.tableColumn = new List<CT_TableColumn>();
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "DataCampionamento" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Codice Fiscale" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Matricola" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Nome" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Cognome" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Sesso" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Data Nascita" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Luogo Nascita" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Direzione afferenza" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Afferenza" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Descrizionea Afferenza" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Responsabile" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Data inizio collaborazione" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Tipo Impegno" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Percentuale Part Time" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Ruolo Contrattuale" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Ruolo" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Inquadramento" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Livello" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "GerarchiaLivello" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Attività" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Descrizione Attività" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "eMail" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Nome Account" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Cellulare Esteso" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "Area Scientifico Disciplinare" });
            icolt++;
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = icolt, name = "SSD" });

            ctTable.autoFilter = new CT_AutoFilter();

            workbook.Write(fs, false);
        }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            System.IO.File.Delete(Path.Combine(sWebRootFolder, sFileName));
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
    }
}
