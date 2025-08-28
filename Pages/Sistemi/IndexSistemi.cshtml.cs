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

namespace SRA.Pages.Sistemi
{
    [Authorize]
    public class IndexSistemiModel : PageModel
    {
        private readonly SRAContext _context;
        public IndexSistemiModel(SRAContext context)
        {
            _context = context;
        }
        public List<Sistema> Sistemi {  get; set; }
        public void OnGet()
        {
            Sistemi = _context.Sistemi.ToList();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idsistema)
        {
            var aggiungiSistemamodel = new InputSistemiModel();
            if (idsistema.HasValue)
            {
                aggiungiSistemamodel.Sistema = _context.Sistemi.AsNoTracking().Where(a => a.ID == idsistema).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleSistemi",
                ViewData = new ViewDataDictionary<InputSistemiModel>(ViewData, aggiungiSistemamodel)
            };
        }

        public async Task<IActionResult> OnPostAggModSistemaAsync(InputSistemiModel model)
        {

            if (model.Sistema.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Sistema);
                _context.SaveChanges();

            }
            else
            _context.Update(model.Sistema);
            _context.SaveChanges();

            return RedirectToPage("./IndexSistemi");
        }
    }
}
