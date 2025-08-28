using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputRischiModel
    {
        private readonly SRAContext _context;
        public Rischio Rischio { get; set; } = new Rischio();
        public List<SelectListItem> ListaTipiRischio { get; set; } = new List<SelectListItem>();

        public InputRischiModel() { }

        public InputRischiModel(SRAContext context)
        {
            _context = context;
            var listatipirischio = _context.TipiRischio.ToList();

            this.ListaTipiRischio.Add(new SelectListItem("Seleziona il tipo", "Seleziona il tipo"));

            foreach (var tiporischio in listatipirischio)
            {
                ListaTipiRischio.Add(new SelectListItem(tiporischio.Nome, tiporischio.ID.ToString()));
            }
        }
        }
    }
