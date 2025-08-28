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

namespace SRA.Pages.Aziende
{
    [Authorize]
    public class IndexAziendeModel : PageModel
    {
        private readonly SRAContext _context;
        public IndexAziendeModel(SRAContext context)
        {
            _context = context;
        }
        public List<Azienda> Aziende {  get; set; }
        public void OnGet()
        {
            Aziende = _context.Aziende.ToList();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idazienda)
        {
            var aggiungiaziendamodel = new InputAziendeModel();
            if (idazienda.HasValue)
            {
                aggiungiaziendamodel.Azienda = _context.Aziende.AsNoTracking().Where(a => a.ID == idazienda).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleAziende",
                ViewData = new ViewDataDictionary<InputAziendeModel>(ViewData, aggiungiaziendamodel)
            };
        }

        public async Task<IActionResult> OnPostAggModAziendaAsync(InputAziendeModel model)
        {

            if (model.Azienda.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Azienda);
                _context.SaveChanges();

            }
            else
            _context.Update(model.Azienda);
            _context.SaveChanges();

            return RedirectToPage("./IndexAziende");
        }
    }
}
