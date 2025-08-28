using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Pages.TipiRischio
{
    public class IndexTipiRischioModel : PageModel
    {
        private readonly SRAContext _context;
        public IndexTipiRischioModel(SRAContext context)
        {
            _context = context;
        }
        public List<TipoRischio> TipiRischio {  get; set; }
        public void OnGet()
        {
            TipiRischio = _context.TipiRischio.ToList();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idtiporischio)
        {
            var aggiungitiporischiomodel = new InputTipiRischioModel();
            if (idtiporischio.HasValue)
            {
                aggiungitiporischiomodel.TipoRischio = _context.TipiRischio.AsNoTracking().Where(a => a.ID == idtiporischio).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleTipiRischio",
                ViewData = new ViewDataDictionary<InputTipiRischioModel>(ViewData, aggiungitiporischiomodel)
            };
        }

        public async Task<IActionResult> OnPostAggModTipoRischioAsync(InputTipiRischioModel model)
        {

            if (model.TipoRischio.ID.Equals(Guid.Empty))
            {
                _context.Add(model.TipoRischio);
                _context.SaveChanges();

            }
            else
            _context.Update(model.TipoRischio);
            _context.SaveChanges();

            return RedirectToPage("./IndexTipiRischio");
        }
    }
}
