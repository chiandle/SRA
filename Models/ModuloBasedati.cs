using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRA.Models
{
    public class ModuloBasedati
    {
        public Guid IDModulo { get; set; }
        public Guid IDBaseDati { get; set; }
        public string TipoAccesso { get; set; }
        
        //public Modulo Modulo { get; set; }
        //public Basedati Basedati { get; set; }

    }
}
