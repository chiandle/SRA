using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace SRA.Pages.Attività
{
    [Authorize]
    public class AttivitàWPsModel : PageModel
    {
        public Attivita Attività { get; set; } = new Attivita();
        public List<VW_AttivitàWP> ListaAttivitàWPs { get; set; } = new List<Models.VW_AttivitàWP>();
        public List<WorkPackage> ListaWPs { get; set; } = new List<WorkPackage>();
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        public string OwnerCorrente { get; set; } = "Non assegnato";

        private readonly SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;

        public AttivitàWPsModel (SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }

        public void OnGet(Guid? idattivita)
        {

            if (idattivita.HasValue)
            {
                Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).FirstOrDefault();
                CaricaDatiPagina();
            }

        }

        private void CaricaDatiPagina()
        {
            ListaWPs = _context.WorkPackages.AsNoTracking().Where(a => a.IDAttività == Attività.ID & a.Cancellato == false).ToList();
            ListaAttivitàWPs = _context.VW_AttivitàWPs.AsNoTracking().Where(a => a.IDAttività == Attività.ID).OrderBy(c => c.DataInizio).ToList();
        }
        
        public PartialViewResult OnGetModWPModalePartial(Guid? idattivita, Guid? idwp)
        {
            var assegnazioneWPmodel = new AttivitaWPInputModel();
            Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).First();

            if (idwp    .HasValue)
            {
                assegnazioneWPmodel.RigaAttivitaWP = _context.VW_AttivitàWPs.Where(i => i.IDWP == idwp).First();
                assegnazioneWPmodel.Modifica = true;
            }
            else
            {
                assegnazioneWPmodel.RigaAttivitaWP.IDWP = Guid.NewGuid();
                assegnazioneWPmodel.RigaAttivitaWP.IDAttività = idattivita.Value;
                assegnazioneWPmodel.RigaAttivitaWP.NomeAttività = Attività.Nome;
                assegnazioneWPmodel.Modifica = false;
                var listaWPs = _context.WorkPackages.Where(a => a.IDAttività == Attività.ID).ToList();
                assegnazioneWPmodel.ListaWPs.Add(new SelectListItem("Seleziona il Work Package", ""));
                foreach ( var a in listaWPs)
                {
                    assegnazioneWPmodel.ListaWPs.Add(new SelectListItem(a.Nome, a.ID.ToString()));

                }
            }
            
            return new PartialViewResult
            {
                ViewName = "_ModAssegnazioneWP",
                ViewData = new ViewDataDictionary<AttivitaWPInputModel>(ViewData, assegnazioneWPmodel)
            };
        }




        public async Task<IActionResult> OnPostModAssegnazioneWP(AttivitaWPInputModel model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            bool esiste = _context.WorkPackages.AsNoTracking().Where(i => i.ID == model.RigaAttivitaWP.IDWP).Any();
            Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == model.RigaAttivitaWP.IDAttività).First();
            if (model.RigaAttivitaWP.Completato)
            {
                model.RigaAttivitaWP.DataFine = DateTime.Now.Date;
            }
            WorkPackage workpackage = new WorkPackage
            {
                ID = model.RigaAttivitaWP.IDWP,
                IDAttività = model.RigaAttivitaWP.IDAttività,
                Nome = model.RigaAttivitaWP.Nome,
                Descrizione = model.RigaAttivitaWP.Descrizione,
                DataInizio = model.RigaAttivitaWP.DataInizio,
                DataFine = model.RigaAttivitaWP.DataFine,
                Notifiche = model.RigaAttivitaWP.Notifiche,
                GiorniNotifiche = model.RigaAttivitaWP.GiorniNotifiche,
                GiorniPreavviso = model.RigaAttivitaWP.GiorniPreavviso,
                DataUltimaNotifica = model.RigaAttivitaWP.DataUltimaNotifica,
                Completato = model.RigaAttivitaWP.Completato,
                AutoreUltimaModifica = user.Nome + " " + user.Cognome,
                DataUltimaModifica = DateTime.Now,
                IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString()

            };
            if (workpackage.DataInizio == DateTime.MinValue)
            {
                workpackage.DataInizio = Attività.DataInizio;
            }
            if (workpackage.DataFine == DateTime.MinValue || workpackage.DataFine > Attività.DataFine)
            {
                workpackage.DataFine = Attività.DataFine;
            }

            if (esiste)
            {
                _context.Update(workpackage);
            }
            else
            {
                _context.Add(workpackage);
            }
            _context.SaveChanges();

            CaricaDatiPagina();

            return Page();
        }

        public async Task<IActionResult> OnGetEliminaWP(Guid idattivita, Guid idwp)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            try
            {
                WorkPackage wpdacancellare = _context.WorkPackages.Where(i => i.ID == idwp).FirstOrDefault();
                if (wpdacancellare != null)
                {
                    wpdacancellare.Cancellato = true;
                    wpdacancellare.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
                    wpdacancellare.DataUltimaModifica = DateTime.Now;
                    wpdacancellare.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                    _context.Update(wpdacancellare);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

            Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita).FirstOrDefault();
            if (Attività != null) { 
            CaricaDatiPagina();
            return Page();
            }
            else
            {
                return new RedirectToPageResult("./IndexElencoAttività");
            }
        }
    }
}
