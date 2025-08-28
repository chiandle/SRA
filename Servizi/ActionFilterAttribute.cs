using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;

namespace SRA.Servizi
{
    public class AvoidMultiplePostFilter : IPageFilter
    {
        private readonly IConfiguration _config;

        public AvoidMultiplePostFilter(IConfiguration config)
        {
            _config = config;
        }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            try 
            {
                if (context.HandlerMethod != null)
                {
                    if (context.HandlerMethod.MethodInfo.Name.Contains("Post"))
                    {
                        if (context.HttpContext.Request.HasFormContentType && context.HttpContext.Request.Form.ContainsKey("__RequestVerificationToken"))
                        {
                            var currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
                            var lastToken = context.HttpContext.Session.GetString("LastProcessedToken");

                            if (lastToken == currentToken)
                            {
                                context.ModelState.AddModelError(string.Empty, "Looks like you accidentally submitted the same form twice.");
                            }
                            else
                            {
                                context.HttpContext.Session.SetString("LastProcessedToken", currentToken);
                            }
                        }
                    }
                }
            } 
            catch (NullReferenceException ex) 
            {
                
            }
            
        }


    }
}
