using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Pages.Rischi
{
    [Authorize]
    public class IndexRischiModel : PageModel
    {
        private readonly SRAContext _context;
        public IndexRischiModel(SRAContext context)
        {
            _context = context;
        }
        public List<VW_Rischio_Display> Rischi {  get; set; }
        public void OnGet()
        {
            Rischi = _context.VW_Rischi_Display.ToList();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idrischio)
        {
            var aggiungirischiomodel = new InputRischiModel(_context);
            if (idrischio.HasValue)
            {
                aggiungirischiomodel.Rischio = _context.Rischi.AsNoTracking().Where(a => a.ID == idrischio).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleRischi",
                ViewData = new ViewDataDictionary<InputRischiModel>(ViewData, aggiungirischiomodel)
            };
        }

        public async Task<IActionResult> OnPostAggModRischioAsync(InputRischiModel model)
        {

            if (model.Rischio.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Rischio);
                _context.SaveChanges();

            }
            else
            _context.Update(model.Rischio);
            _context.SaveChanges();

            return RedirectToPage("./IndexRischi");
        }
    }
}
