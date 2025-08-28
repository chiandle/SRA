using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Editing;
using static SRA.Pages.ImpattoMisure.IndexImpattoMisureModel;

namespace SRA.Pages.Attività
{
    [Authorize]
    public class AttivitàPersoneModel : PageModel
    {
        public Attivita Attività { get; set; } = new Attivita();
        public List<VW_AttivitàPersona> ListaAttivitàPersone { get; set; } = new List<Models.VW_AttivitàPersona>();
        public List<PersonaAssegnata> ListaPersone { get; set; } = new List<PersonaAssegnata>();
        public List<Struttura> ListaStruttre { get; set; } = new List<Struttura>();
        [BindProperty]
        public string StrutturaSelezionata { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        public class PersonaAssegnata
        {
            public Guid ID { get; set; }
            public string Nome { get; set; }
            public string Cognome { get; set; }


            public override bool Equals(object obj)
            {
                if (obj is PersonaAssegnata other)
                {
                    return ID == other.ID && Nome == other.Nome && Cognome == other.Cognome;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ID, Nome, Cognome);
            }

        }

        public string OwnerCorrente { get; set; } = "Non assegnato";

        private readonly SRAContext _context;

        public AttivitàPersoneModel (SRAContext context)
        {
            _context = context;
        }

        public void OnGet(Guid? idattivita)
        {

            if (idattivita.HasValue)
            {
                Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).FirstOrDefault();
                CaricaDatiPagina();
            }

        }

        
        private void CaricaDatiPagina()
        {
            ListaAttivitàPersone = _context.VW_AttivitàPersone.Where(a => a.IDAttività == Attività.ID && a.Cancellato == false).OrderBy(c => c.Cognome).ThenByDescending(d => d.DataInizio).ToList();
            ListaPersone = ListaAttivitàPersone.Select(a => new PersonaAssegnata { ID = a.IDPersona, Nome = a.Nome, Cognome = a.Cognome }).Distinct().ToList();
            if (ListaAttivitàPersone.Count() > 0)
            {
                var ownercorrente = ListaAttivitàPersone.Where(i => i.IsOwner).OrderByDescending(d => d.DataInizio).FirstOrDefault();
                if (ownercorrente != null)
                {
                    OwnerCorrente = ownercorrente.Nome + " " + ownercorrente.Cognome;
                }

            }
        }
        
        public PartialViewResult OnGetModAssegnazioniModalePartial(Guid? idriga, Guid? idattivita, Guid? idpersona)
        {
            var assegnazionepersonamodel = new AttivitaPersonaInputModel(_context);

            
            Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).First();

            if (idriga.HasValue)
            {
                assegnazionepersonamodel.RigaAttivitaPersona = _context.VW_AttivitàPersone.Where(i => i.ID == idriga).First();
                assegnazionepersonamodel.Modifica = true;
            }
            else
            {
                assegnazionepersonamodel.RigaAttivitaPersona.ID = Guid.NewGuid();
                assegnazionepersonamodel.RigaAttivitaPersona.IDAttività = idattivita.Value;
                assegnazionepersonamodel.RigaAttivitaPersona.NomeAttività = Attività.Nome;
                assegnazionepersonamodel.Modifica = false;
                var listapersone = _context.Persone.Where(a => a.CodiceAfferenza == Attività.CodiceStruttura).ToList();
                assegnazionepersonamodel.StrutturaSelezionata = Attività.CodiceStruttura;
                assegnazionepersonamodel.ListaPersone.Add(new SelectListItem("Seleziona la persona", ""));
                foreach ( var a in listapersone)
                {
                    assegnazionepersonamodel.ListaPersone.Add(new SelectListItem(a.Cognome + " " + a.Nome, a.ID.ToString()));

                }
            }
            
            return new PartialViewResult
            {
                ViewName = "_ModAssegnazionePersonale",
                ViewData = new ViewDataDictionary<AttivitaPersonaInputModel>(ViewData, assegnazionepersonamodel)
            };
        }


        public IActionResult OnGetPersoneinStruttura(string idstruttura)
        {
            List<ChiaveValoreJSON> personeinstruttura = new List<ChiaveValoreJSON>();
            personeinstruttura = _context.Persone.Where(s => s.CodiceAfferenza == idstruttura).Select(s => new ChiaveValoreJSON
            {
                ID = s.ID.ToString(),
                Nome = s.Cognome + ' ' + s.Nome
            }).OrderBy(o => o.Nome).ToList();
            var json = new JsonResult(personeinstruttura);
            return json;
        }

        public IActionResult OnPostElimina(Guid? idriga, Guid? idattivita)
        {
            try
            {
                AttivitàPersona assegnazionedacancellare = _context.AttivitàPersone.Where(i => i.ID == idriga).FirstOrDefault();
                assegnazionedacancellare.Cancellato = true;
                _context.Update(assegnazionedacancellare);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            if (idattivita.HasValue)
            {
                Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == idattivita.Value).FirstOrDefault();
                CaricaDatiPagina();
            }
            this.CaricaDatiPagina();
            return Page();
        }

        public IActionResult OnPostModAssegnazionePersonale(AttivitaPersonaInputModel model)
        {
            bool esiste = _context.AttivitàPersone.AsNoTracking().Where(i => i.ID == model.RigaAttivitaPersona.ID).Any();
            Attività = _context.ElencoAttività.AsNoTracking().Where(a => a.ID == model.RigaAttivitaPersona.IDAttività).First();
            var persona = _context.Persone.Where(p => p.ID == model.RigaAttivitaPersona.IDPersona).First();

            AttivitàPersona assegnazione = new AttivitàPersona
            {
                ID = model.RigaAttivitaPersona.ID,
                IDAttività = model.RigaAttivitaPersona.IDAttività,
                IDPersona = model.RigaAttivitaPersona.IDPersona,
                CodiceAfferenza = persona.CodiceAfferenza,
                Afferenza = persona.Afferenza,
                DataInizio = model.RigaAttivitaPersona.DataInizio,
                DataFine = model.RigaAttivitaPersona.DataFine,
                Isowner = model.RigaAttivitaPersona.IsOwner,
                OreImpegno = model.RigaAttivitaPersona.OreImpegno
            };

            if (esiste)
            {
                if (Attività.DataInizio > assegnazione.DataInizio || Attività.DataFine < assegnazione.DataInizio)
                {
                    assegnazione.DataInizio = Attività.DataInizio;
                }
                if (Attività.DataFine < assegnazione.DataFine || Attività.DataInizio > assegnazione.DataFine)
                {
                    assegnazione.DataFine = Attività.DataFine;
                }
                _context.Update(assegnazione);
            }
            else
            {
                if(assegnazione.DataInizio == DateTime.MinValue)
                {
                    assegnazione.DataInizio = Attività.DataInizio;
                }
                if (assegnazione.DataFine == DateTime.MinValue || assegnazione.DataFine > Attività.DataFine)
                {
                    assegnazione.DataFine = Attività.DataFine;
                }
                _context.Add(assegnazione);
            }
            _context.SaveChanges();

            CaricaDatiPagina();

            return Page();
        }
    }
}
