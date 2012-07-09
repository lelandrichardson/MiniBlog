using System.Web;
using System.Web.Security;

namespace MiniBlog.Extensions
{
    public static class ResponseExtensions
    {
        //TODO: add to config file
        public const string RedirectParam = "redirect";
        public static void RedirectFromUrl(this HttpResponseBase response, bool endResponse = true)
        {
            var req = HttpContext.Current.Request;
            if (req[RedirectParam] == null)
            {
                response.Redirect("~/");
            }
            else
            {
                response.Redirect(req[RedirectParam],endResponse);
            }
        }
        
        

        public static void End(this HttpResponseBase responseBase, int statuscode)
        {
            responseBase.StatusCode = statuscode;
            responseBase.End();
        }
    }
}