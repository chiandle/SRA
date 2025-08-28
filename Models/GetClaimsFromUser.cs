using SRA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using SRA.Areas.Identity.Data;

namespace SRA.Models
{
    public class GetClaimsFromUser : IGetClaimsProvider
    {
        public GetClaimsFromUser(IHttpContextAccessor accessor, UserManager<SRAUser> usermanager, SRAUserContext context)
        {
            //UserId = accessor.HttpContext?
            //    .User.Claims.SingleOrDefault(x =>
            //        x.Type == ClaimTypes.NameIdentifier)?.Value;
            //var currentuser = usermanager.Users.FirstOrDefault(u => u.Id == UserId);
            //if (currentuser != null)
            //{
            //    var listaruoli = context.Roles.Join(context.UserRoles,
            //        r => r.Id,
            //        ur => ur.RoleId,
            //        (r, ur) => new { r.Id, r.Name, ur.UserId }).Where(u => u.UserId == UserId).Select(r => r.Name).ToList();
            //    Amministratore = listaruoli.Contains("Amministratore");
            //    Responsabile = listaruoli.Contains("Responsabile");
            //    Supervisore = listaruoli.Contains("Supervisore");

            //    Struttura = currentuser.Struttura;
            //}
            //else
            //{
            //    Struttura = "NULL";
            //}
        }


        public string UserId { get; private set; }
        public string Struttura { get; private set; }
        public bool Amministratore { get; private set; } = false;
        public bool Responsabile { get; private set; } = false;
        public bool Supervisore { get; private set; } = false;
    }
}
