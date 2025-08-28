using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SRA.Pages.Amministrazione
{
    [Authorize(Roles = "Operatore")]
    public class IndexModel : PageModel
    {
        
        public void OnGet()
        {
        }
    }
}
