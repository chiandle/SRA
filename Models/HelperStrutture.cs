using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRA.Models
{
    public class HelperStrutture
    {
        private readonly SRA.Models.SRAContext _context;
        public string StrutturaSelezionata { get; set; }
        public List<SelectListItem> ListaStrutture { get; set; } = new List<SelectListItem>();

        public HelperStrutture(SRAContext context)
        {
            _context = context;
            var listastrutture = _context.Strutture.ToList();
            foreach (var struttura in listastrutture)
            {
                ListaStrutture.Add(new SelectListItem(struttura.Nome, struttura.UO));

            }
        }


        public HelperStrutture(SRAContext context, Guid idpersona)
        {
            _context = context;


            var listastrutture = _context.VW_StrutturePersona.Where(s => s.IDPersona == idpersona).ToList();
            foreach (var struttura in listastrutture)
            {
                ListaStrutture.Add(new SelectListItem(struttura.NomeStruttura, struttura.IDStruttura));
            }


        }
    }
}
