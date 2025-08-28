using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputBasiDatiModel
    {
        private readonly SRAContext _context;
        public BaseDati BaseDati { get; set; } = new BaseDati();
        public List<SelectListItem> ListaSistemi { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaDbms { get; set; } = new List<SelectListItem>();
        public InputBasiDatiModel() { }
        public InputBasiDatiModel(SRAContext context) 
        {
            _context = context;
            var listalocazioni = _context.Sistemi.Where(a => a.Inproduzione == true).ToList();
            ListaSistemi.Add(new SelectListItem("Seleziona il sistema", "Seleziona il sistema"));
            foreach (var locazione in listalocazioni)
            {
                ListaSistemi.Add(new SelectListItem(locazione.Nome, locazione.ID.ToString()));
            }

            var listadbms = _context.VW_DBMSs_Display.ToList();

            ListaDbms.Add(new SelectListItem("Seleziona la base dati", "Seleziona la base dati"));
            foreach (var dbms in listadbms)
            {
                ListaDbms.Add(new SelectListItem(dbms.Nome, dbms.ID.ToString()));
            }
        }
    }
}
