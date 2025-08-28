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

namespace SRA.Pages.DBMSs
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
        //public DBMSIndexData DBMS { get; set; }
        public int DBMSID { get; set; }

        public List<VW_DBMS_Display> DBMSs { get; set; } = new List<VW_DBMS_Display>();


        public async Task OnGetAsync(int? id)
        {
            DBMSs = await _context.VW_DBMSs_Display.AsNoTracking()
                .ToListAsync();
            //DBMS = new DBMSIndexData();
            //DBMS.DBMSsEnum = await _context.DBMSs
            //    .Include(m => m.Applicazione)
            //    .Include(m => m.DBMS_Basidati)
            //      .ThenInclude(m => m.Basedati)
            //     .AsNoTracking()
            //    .ToListAsync();
            //if (id != null)
            //{
            //    DBMSID = id.Value;
            //}

        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? iddbms)
        {
            var aggiungidbmsmodel = new InputDBMSsModel(_context);
            if (iddbms.HasValue)
            {
                aggiungidbmsmodel.DBMS = _context.DBMSs.AsNoTracking().Where(a => a.ID == iddbms).FirstOrDefault();
            }

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleDBMSs",
                ViewData = new ViewDataDictionary<InputDBMSsModel>(ViewData, aggiungidbmsmodel)
            };
        }

        public async Task<IActionResult> OnPostAggModDBMSAsync(InputDBMSsModel model)
        {

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            DBMS nuovadbms = new DBMS
            {
                Nome = model.DBMS.Nome,
                Versione = model.DBMS.Versione,
                IDProduttore = model.DBMS.IDProduttore

            };
            if (model.DBMS.ID.Equals(Guid.Empty))
            {
                
                nuovadbms.ID = Guid.NewGuid();
                try
                {
                    _context.Add(nuovadbms);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                nuovadbms.ID = model.DBMS.ID;
                try
                {
                    _context.Update(nuovadbms);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToPage("./IndexDBMSs");
        }
    }
}
