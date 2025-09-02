using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using SRA.Areas.Identity.Data;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SRA.Pages.Attività
{
    public class PromemoriaAttivitàModel : PageModel
    {
        private SRAContext _context;
        private UserManager<SRAUser> _userManager;
        

        public PromemoriaAttivitàModel(SRAContext context, UserManager<SRAUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;

        }
        [BindProperty]
        public Guid IDWP { get; set; }
        public List<VW_AttivitàWP> ListaWP { get; set; } = new List<VW_AttivitàWP>();
        private SRAUser UtenteCollegato { get; set; }
        public async Task OnGetAsync()
        {
            UtenteCollegato = await _userManager.GetUserAsync(User);
            this.CaricaDatiPagina();
        }
        
        public void CaricaDatiPagina()
        {
            var idpersona = _context.Persone.AsNoTracking().Where(e => e.Email.ToLower() == UtenteCollegato.Email.ToLower()).Select(i => i.ID).FirstOrDefault();

            ListaWP = _context.VW_AttivitàWPs.Join(
                _context.AttivitàPersone,
                wp => wp.IDAttività,
                ap => ap.IDAttività,
                (wp, ap) => new { wp, ap }
            ).Where(joined => joined.ap.IDPersona == idpersona && joined.ap.Cancellato == false).Select(joined => joined.wp).OrderBy(o => o.Completato).ToList();
        }
        public async Task<IActionResult> OnPostSegnaCompletatoAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }
            try
            {
                var wpdaaggiornare = _context.WorkPackages.Where(w => w.ID == IDWP).FirstOrDefault();
                wpdaaggiornare.Completato = true;
                _context.Update(wpdaaggiornare);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToPage();
            }
            UtenteCollegato = await _userManager.GetUserAsync(User);
            var idpersona = _context.Persone.AsNoTracking().Where(e => e.Email.ToLower() == UtenteCollegato.Email.ToLower()).Select(i => i.ID).FirstOrDefault();

            ListaWP = _context.VW_AttivitàWPs.Join(
                _context.AttivitàPersone,
                wp => wp.IDAttività,
                ap => ap.IDAttività,
                (wp, ap) => new { wp, ap }
            ).Where(joined => joined.ap.IDPersona == idpersona && joined.ap.Cancellato == false).Select(joined => joined.wp).OrderBy(o => o.Completato).ToList();
            return Page();
        }
    }
}
