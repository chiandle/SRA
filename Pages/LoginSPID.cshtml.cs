using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlCo.SPID.SAML;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SRA.Models;


namespace SRA.Pages
{
    public class LoginSPIDModel : PageModel
    {
        public void OnGet()
        {
            AlCo.SPID.SAML.AuthRequestOptions options = new AlCo.SPID.SAML.AuthRequestOptions()
            {
                //Request Unique Identifier
                UUID = Guid.NewGuid().ToString(),
                //Service Provider Unique Identifier (url like https://www.dotnetcode.it can be ok)
                SPUID = "https://localhost:44329/",
                //SPID Authentication Level (1/2/3)
                SPIDLevel = AlCo.SPID.SAML.SPIDLevel.SPIDL1,
                //Identity Provider Url For SAML Request
                Destination = "http://localhost:8088/sso",
                //Service Provider Consumer Service Index - Normally Callback URL (refer to Service Provider Metadata)
                AssertionConsumerServiceIndex = 1,
                //Service Provider Consuming Service Index - Normally Requested User Data(refer to Service Provider Metadata)
                AttributeConsumingServiceIndex = 1
            };

            AlCo.SPID.SAML.AuthRequest request = new AlCo.SPID.SAML.AuthRequest(options);


            //string rsaXmlKey = "<![CDATA[<RSAKeyValue><Modulus>rBPwxOB3QM+Rhz+/...</RSAKeyValue>";
            var filePath = @"D:\Soffitta\spid-testenv2\certificati\spid-ALCo.pfx";
            X509Certificate2 signinCert = new X509Certificate2(filePath, "esportato", X509KeyStorageFlags.Exportable);

            //            string samlRequest = request.GetSignedAuthRequest(signinCert, rsaXmlKey);
            string samlRequest = request.GetSignedAuthRequest(signinCert);



            string returnUrl = "/";

            if (!string.IsNullOrEmpty(HttpContext.Request.Query["redirectUrl"]))
            {
                returnUrl = HttpContext.Request.Query["redirectUrl"];
            }

            ViewData["FormUrlAction"] = options.Destination;
            ViewData["SAMLRequest"] = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(samlRequest));
            ViewData["RelayState"] = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(returnUrl));


        }
    }
}