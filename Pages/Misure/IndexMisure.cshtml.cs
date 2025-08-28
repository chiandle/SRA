using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Areas.Identity.Data;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Pages.Misure
{
    [Authorize]
    public class IndexMisureModel : PageModel
    {
        private readonly SRAContext _context;
        private readonly UserManager<SRAUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextaccessor;


        public IndexMisureModel(SRAContext context, UserManager<SRAUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextaccessor = httpContextAccessor;
        }
        public List<Misura> Misure {  get; set; }
        public void OnGet()
        {
            Misure = _context.Misure.ToList();
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idmisura)
        {
            var aggiungimisuramodel = new InputMisureModel(_context);
            if (idmisura.HasValue)
            {
                aggiungimisuramodel.Misura = _context.Misure.AsNoTracking().Where(a => a.ID == idmisura).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleMisure",
                ViewData = new ViewDataDictionary<InputMisureModel>(ViewData, aggiungimisuramodel)
            };
        }

        public async Task<IActionResult> OnPostAggModMisuraAsync(InputMisureModel model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (model.Misura.DataDisattivazione == DateTime.MinValue)
            {
                model.Misura.DataDisattivazione = DateTime.MaxValue;
            }
            if(model.Misura.DataAttivazione == DateTime.MinValue)
            {
                model.Misura.DataAttivazione = DateTime.Now.Date;
            }
            model.Misura.DataUltimaModifica = DateTime.Now;
            model.Misura.AutoreUltimaModifica = user.Nome + " " + user.Cognome;
            model.Misura.IPUltimaModifica = _httpcontextaccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (model.Misura.ID.Equals(Guid.Empty))
            {
                _context.Add(model.Misura);
                _context.SaveChanges();

            }
            else
            _context.Update(model.Misura);
            _context.SaveChanges();

            return RedirectToPage("./IndexMisure");
        }
    }
}
