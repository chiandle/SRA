using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SRA.Models;

namespace SRA.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        

        private readonly SRA.Models.SRAContext _context;

        public IndexModel(SRA.Models.SRAContext context)
        {
            _context = context;
        }

        public IList<Applicazione> Applicazione { get;set; }

        public async Task OnGetAsync()
        {
            Applicazione = await _context.Applicazioni.ToListAsync();
        }
    }
}
