using System;
using System.Web;

namespace MiniBlog.Includes.Web
{
    public class RemoveHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose() { }

        void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("ETag");
            HttpContext.Current.Response.Headers.Remove("X-AspNetWebPages-Version");
        }
    }
}
