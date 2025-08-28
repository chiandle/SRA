using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class InputDBMSsModel
    {
        private readonly SRAContext _context;
        public DBMS DBMS { get; set; } = new DBMS();
        public List<SelectListItem> ListaAziende { get; set; } = new List<SelectListItem>();

        public InputDBMSsModel() { }
        public InputDBMSsModel (SRAContext context)
        {
            _context = context;
            DBMS = new DBMS();

            var listaziende = _context.Aziende.Where(a => a.Attiva.Equals(true)).ToList();
            this.ListaAziende.Add(new SelectListItem("Seleziona l'azienda", ""));
            foreach (var azienda in listaziende)
            {
                ListaAziende.Add(new SelectListItem(azienda.Nome, azienda.ID.ToString()));
            }

        }
    }
}
