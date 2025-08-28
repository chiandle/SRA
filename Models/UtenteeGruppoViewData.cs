using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SRA.Models
{
    public class UtenteeGruppoViewData : UtenteeGruppo
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public List<SelectListItem> ListaGruppi { get; set; }
    }
}
