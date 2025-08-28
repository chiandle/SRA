using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Pages.ImpattoMisure
{
    [Authorize]
    public class IndexImpattoMisureModel : PageModel
    {
        private readonly SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;
        public List<VW_Impatto_Misura_Display> ImpattoMisure { get; set; } = new List<VW_Impatto_Misura_Display>();

        public class ChiaveValoreJSON
        {
            [Key]
            public string ID { get; set; }
            public string Nome { get; set; }

        }
        public IndexImpattoMisureModel(SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }
        public async Task OnGetAsync()
        {
            ImpattoMisure = await _context.VW_Impatto_Misure_Display.AsNoTracking().ToListAsync();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idmisura, Guid? idapplicazione, Guid? idrischio)
        {
            var aggiungiimpattomisuramodel = new InputImpattoMisureModel(_context);
            if (idmisura.HasValue && idapplicazione.HasValue && idrischio.HasValue)
            {
                aggiungiimpattomisuramodel.ImpattoMisura = _context.ImpattoMisure.AsNoTracking().Where(a => a.IDMisura == idmisura & a.IDApplicazione ==  idapplicazione & a.IDRischio == idrischio)
                    .FirstOrDefault();
                var listaapplicazioni = _context.Applicazioni.Where(a => a.Stato != "Dismessa").OrderBy(n => n.Nome).ToList();

                aggiungiimpattomisuramodel.ListaApplicazioni = new List<SelectListItem>();

                foreach (var applicazione in listaapplicazioni)
                {
                    aggiungiimpattomisuramodel.ListaApplicazioni.Add(new SelectListItem(applicazione.Nome, applicazione.ID.ToString()));
                }
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleImpattoMisure",
                ViewData = new ViewDataDictionary<InputImpattoMisureModel>(ViewData, aggiungiimpattomisuramodel)
            };
        }


        public IActionResult OnGetApplicazioniRischi(string idrischio)
        {
            List<ChiaveValoreJSON> applicazionirischio = new List<ChiaveValoreJSON>();
            applicazionirischio = _context.VW_Applicazioni_Rischi_Display.Where(s => s.IDRischio.ToString() == idrischio).Select(s => new ChiaveValoreJSON
            {
                ID = s.IDApplicazione.ToString(),
                Nome = s.NomeApplicazione
            }).OrderBy(o => o.Nome).ToList();
            var json = new JsonResult(applicazionirischio);
            return json;
        }
        public async Task<IActionResult> OnPostAggModImpattoMisuraAsync(InputImpattoMisureModel model)
        {
            if (!ModelState.IsValid) 
            {
                return RedirectToPage();
            }

            var recordesistente = _context.ImpattoMisure
              .Where(i => i.IDApplicazione == model.ImpattoMisura.IDApplicazione & i.IDMisura == model.ImpattoMisura.IDMisura & i.IDRischio == model.ImpattoMisura.IDRischio)
              .Any();
            if (recordesistente)
            {
                _context.Update(model.ImpattoMisura);
            }
            else
            {
                _context.Add(model.ImpattoMisura);
            }
            _context.SaveChanges();
            

            return RedirectToPage();
        }

        }
}
