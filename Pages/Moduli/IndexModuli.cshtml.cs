using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Models;

namespace SRA.Pages.Moduli
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SRA.Models.SRAContext _context;
        public Guid? IDApplicazioneSelezionata {  get; set; }

        public string FiltroNome { get; set; }
        public string FiltroStato { get; set; }
        public string FiltroTipoGestione { get; set; }
        public string FiltroTipoApplicazione { get; set; }

        public string OrdinaNome { get; set; }
        public string OrdinaStato { get; set; }
        public string OrdinaData { get; set; }

        public string OrdinamentoCorrente { get; set; }
        public string FiltroCorrente { get; set; }

        public InputModuliModel DatiModel { get; set; }
        public IndexModel(SRA.Models.SRAContext context)
        {
            _context = context;
        }
        //public ModuloIndexData Modulo { get; set; }
        public int ModuloID { get; set; }

        public List<VW_Modulo_Display> Moduli { get; set; } = new List<VW_Modulo_Display>();


        public async Task OnGetAsync(Guid? idapplicazioneselezionata, string ordinamento)
        {
            DatiModel = new InputModuliModel(_context);

            if (idapplicazioneselezionata == null) 
            {
                Moduli = await _context.VW_Moduli_Display.OrderBy(n => n.Nome).ToListAsync();
            }
            else
            {
                IDApplicazioneSelezionata = idapplicazioneselezionata;
                Moduli = await _context.VW_Moduli_Display.Where(a => a.IDApplicazione==idapplicazioneselezionata).OrderBy(n => n.Nome).ToListAsync();
            }

            
        }

        public PartialViewResult OnGetAggiungiModalePartial(Guid? idmodulo, Guid? idapplicazioneselezionata)
        {
            IDApplicazioneSelezionata = idapplicazioneselezionata;
            var aggiungimodulomodel = new InputModuliModel(_context);
            aggiungimodulomodel.IDApplicazioneSelezionata = IDApplicazioneSelezionata;
            aggiungimodulomodel.Modulo.IDApplicazione = IDApplicazioneSelezionata;

            if (idmodulo.HasValue)
            {
                aggiungimodulomodel.Modulo = _context.Moduli.AsNoTracking().Where(a => a.ID == idmodulo).FirstOrDefault();
            }

            var listabasidatidisplay = _context.VW_BasiDati_Display.Where(d => d.Dismessa == false).OrderBy(n => n.Nome).ToList();
            List<Guid> listabasidatiassegnate = _context.ModuliBasidati.Where(m => m.IDModulo == idmodulo).Select(b => b.IDBaseDati).ToList();
            bool basedatiassegnata;
            foreach (var basedati in listabasidatidisplay)
            {
                if(listabasidatiassegnate.Contains(basedati.ID)) {basedatiassegnata = true;}
                else { basedatiassegnata = false;}
                aggiungimodulomodel.ListaBasiDati.Add(new ModuloBasedatiDisplay
                {
                    IDBasedati = basedati.ID,
                    Nome = basedati.Nome,
                    Selezionata = basedatiassegnata
                });
            }

            aggiungimodulomodel.ListaBasiDati = aggiungimodulomodel.ListaBasiDati.OrderByDescending(s => s.Selezionata).ThenBy(n => n.Nome).ToList();

            return new PartialViewResult
            {
                ViewName = "_InterfacciaModaleModuli",
                ViewData = new ViewDataDictionary<InputModuliModel>(ViewData, aggiungimodulomodel)
            };
        }

        public async Task<IActionResult> OnPostAggModModuloAsync(InputModuliModel model)
        {

            if (model.Modulo.ID.Equals(Guid.Empty))
            {
                model.Modulo.ID = Guid.NewGuid();
                _context.Add(model.Modulo);
                _context.SaveChanges();

            }
            else
            {
                _context.Update(model.Modulo);
                _context.SaveChanges();
            }
            foreach (var basedati in model.ListaBasiDati)
                {
                    ModuloBasedati associazionemodulodb = new ModuloBasedati
                    {
                        IDModulo = model.Modulo.ID,
                        IDBaseDati = basedati.IDBasedati
                    };
                    if (basedati.Selezionata)
                    {
                        _context.Add(associazionemodulodb);
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (_context.ModuliBasidati.Where(mdb => mdb.IDModulo == model.Modulo.ID & mdb.IDBaseDati == basedati.IDBasedati).Any())
                        {
                            _context.Remove(associazionemodulodb);
                            _context.SaveChanges();
                        }
                    }
            }

            return RedirectToPage("./IndexModuli", new { idapplicazioneselezionata = model.IDApplicazioneSelezionata });
        }
    }
}
