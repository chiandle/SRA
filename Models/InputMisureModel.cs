using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputMisureModel
    {
        private readonly SRAContext _context;
        public Misura Misura { get; set; } = new Misura();
       

        public InputMisureModel() { }

        public InputMisureModel(SRAContext context)
        {
            _context = context;

        }
        }
    }
