using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputTipiRischioModel
    {
        private readonly SRAContext _context;
        public TipoRischio TipoRischio { get; set; } = new TipoRischio();
        
        public InputTipiRischioModel() { }

        public InputTipiRischioModel(SRAContext context)
        {
            _context = context;
            
        }
        }
    }
