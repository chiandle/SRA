using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;

namespace SRA.Pages.BasiDati
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;
             
        public IndexModel(SRA.Models.SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpcontextaccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpcontextaccessor;
        }
        //public BaseDatiIndexData BaseDati { get; set; }
        public int BaseDatiID { get; set; }

        public List<VW_BaseDati_Display> BasiDati { get; set; } = new List<VW_BaseDati_Display>();


        public async Task OnGetAsync(int? id)
        {
            BasiDati = await _context.VW_BasiDati_Display.AsNoTracking()
                .ToListAsync();
            //BaseDati = new BaseDatiIndexData();
            //BaseDati.BasiDatiEnum = await _context.BasiDati
            //    .Include(m => m.Applicazione)
            //    .Include(m => m.BaseDati_Basidati)
            //      .ThenInclude(m => m.Basedati)
            //     .AsNoTracking()
            //    .ToListAsync();
            //if (id != null)
            //{
            //    BaseDatiID = id.Value;
            //}

        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idbasedati)
        {
            var aggiungibasedatimodel = new InputBasiDatiModel(_context);
            if (idbasedati.HasValue)
            {
                aggiungibasedatimodel.BaseDati = _context.BasiDati.AsNoTracking().Where(a => a.ID == idbasedati).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleBasiDati",
                ViewData = new ViewDataDictionary<InputBasiDatiModel>(ViewData, aggiungibasedatimodel)
            };
        }

        public async Task<IActionResult> OnPostAggModBaseDatiAsync(InputBasiDatiModel model)
        {

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            BaseDati nuovabasedati = new BaseDati
            {
                Nome = model.BaseDati.Nome,
                IDSistema = model.BaseDati.IDSistema,
                Descrizione = model.BaseDati.Descrizione,
                IDDbms = model.BaseDati.IDDbms,
                NomeIstanza = model.BaseDati.NomeIstanza,
                RuoloBasedati = model.BaseDati.RuoloBasedati,
                Dimensione = model.BaseDati.Dimensione,
                DataRilevazione = model.BaseDati.DataRilevazione,
                Dismessa = model.BaseDati.Dismessa,
                DataDismissione = model.BaseDati.DataDismissione,
                AutoreUltimaModifica = user.Nome + " " + user.Cognome,
                DataUltimaModifica = DateTime.Now,
                IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString()

            };
            if (model.BaseDati.ID.Equals(Guid.Empty))
            {
                
                nuovabasedati.ID = Guid.NewGuid();
                try
                {
                    _context.Add(nuovabasedati);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                nuovabasedati.ID = model.BaseDati.ID;
                try
                {
                    _context.Update(nuovabasedati);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToPage("./IndexBasiDati");
        }
    }
}
