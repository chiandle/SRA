using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace SRA.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ACSModel : PageModel
    {
        public void OnGet()
        {

        }
        
        public async Task<ActionResult> OnPostAsync(IFormCollection collection)
        {
            string samlResponse = "";
            string redirect = "";
            AlCo.SPID.SAML.AuthResponse resp = new AlCo.SPID.SAML.AuthResponse();
            try
            {
                samlResponse = Encoding.UTF8.GetString(Convert.FromBase64String(collection["SAMLResponse"]));
                redirect = Encoding.UTF8.GetString(Convert.FromBase64String(collection["RelayState"]));

                resp.Deserialize(samlResponse);

            }
            catch (Exception ex)
            {
                return Page();
            }
            if (resp.RequestStatus == AlCo.SPID.SAML.SamlRequestStatus.Success)
            {
                CookieOptions options = new CookieOptions();
                options.Expires = resp.SessionIdExpireDate;
                Response.Cookies.Delete("SPID_COOKIE");
                Response.Cookies.Append("SPID_COOKIE", JsonConvert.SerializeObject(resp), options);

                var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, resp.User.Name??"", ClaimValueTypes.String, resp.Issuer),
                        new Claim(ClaimTypes.Surname, resp.User.FamilyName??"", ClaimValueTypes.String, resp.Issuer),
                        new Claim(ClaimTypes.Email, resp.User.Email??"", ClaimValueTypes.String, resp.Issuer),
                        new Claim("FiscalNumber", resp.User.FiscalNumber??"", ClaimValueTypes.String, resp.Issuer),
                        new Claim("SessionId", resp.SessionId??"", ClaimValueTypes.String, resp.Issuer),
                    };

                var userIdentity = new ClaimsIdentity(new GenericIdentity(resp.User.Name, "SPID"), claims);

                var userPrincipal = new ClaimsPrincipal(userIdentity);



                await HttpContext.SignInAsync("SPIDCookie", userPrincipal,
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = false,
                        AllowRefresh = false
                    });
            }

            ViewData["SAMLResponse"] = JsonConvert.SerializeObject(resp);
            ViewData["RelayState"] = redirect;
            return Page();

        }

    }

}