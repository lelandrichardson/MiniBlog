using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.WebPages;
using MiniBlog.Extensions;
using MiniBlog.Includes.Data;
using StackExchange.Profiling;

namespace MiniBlog.Framework
{
    public abstract class WebView : WebPage
    {
        #region Public Properties

        /// <summary>
        /// General purpose dynamic object to use in view.
        /// </summary>
        public dynamic ViewBag
        {
            get { return Page.ViewBag ?? (Page.ViewBag = new SafeExpando()); }
        }

        /// <summary>
        /// Dynamic Wrapper around MiniBlogSettingsProvider global object
        /// </summary>
        public dynamic Settings
        {
            get { return Page.Settings ?? (Page.Settings = new DynamicMiniBlogSettings()); }
        }


        #endregion


        #region Css Functionality
        public enum InsertAt { Top, Bottom }
        /// <summary>
        /// Adds css file to header
        /// </summary>
        /// <param name="pathToCssFile">relative content path</param>
        public void Css(string pathToCssFile, WebView.InsertAt insertAt = InsertAt.Top)
        {
            //TODO: other options?
            //add css file
            List<string> paths = Page.CssPaths ?? (Page.CssPaths = new List<string>());

            if (insertAt == InsertAt.Top)
            {
                paths.Insert(0, pathToCssFile);
            }
            else
            {
                paths.Add(pathToCssFile);
            }

        }


        public IHtmlString RenderCss()
        {
            if (Page.CssPaths == null)
            {
                return Html.Raw(string.Empty);
            }

            var b = new StringBuilder();
            foreach (string s in Page.CssPaths)
            {
                b.AppendLine(string.Format(@"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />", Href(s)));
            }

            return Html.Raw(b.ToString());
        }

        #endregion

        #region Js Functionality
        public void Js(string pathToJsFile)
        {
            //TODO:
        }

        public IHtmlString RenderJs()
        {
            //TODO:
            return Html.Raw("");
        }

        #endregion

        #region Alert Boxes
        private const string AlertCloseButton = @"<button class=""close"" data-dismiss=""alert"">×</button>";
        /// <summary>
        /// Insert bootstrap-styled Alert HTML Element to top of page
        /// </summary>
        /// <param name="alertType">one of "alert-info", "alert-error", "alert-help"</param>
        /// <param name="closable">whether or not to include an "X" box on the alert</param>
        public void AddAlert(string alertText, string alertType = "alert-info", bool closable = true)
        {
            var alerts = Page.Alerts ?? (Page.Alerts = new List<string>());
            var builder = new TagBuilder("div");
            builder.AddCssClass(alertType);
            builder.AddCssClass("alert");
            builder.InnerHtml = closable ? string.Format("{0}\n{1}", AlertCloseButton, alertText) : alertText;

            alerts.Add(builder.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Render alerts added using <see cref="AddAlert"/> method.
        /// </summary>
        public IHtmlString RenderAlerts()
        {
            return Html.Raw(Page.Alerts != null ? string.Join("\n", Page.Alerts) : string.Empty);
        }


        #endregion

        #region NavBar





        #endregion

        #region Json Helpers

        /// <summary>
        /// Parses the Request.InputSream (JSON Encoded) content into a dynamic object.
        /// </summary>
        public dynamic GetJson()
        {
            //return new JsonReader().Read();

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            var json = new StreamReader(Request.InputStream).ReadToEnd();
            return serializer.Deserialize(json, typeof(object));

        }

        /// <summary>
        /// Returns serialized JSON string
        /// </summary>
        //public string Json(object data)
        //{
        //    return DisplayExtensions.Json(data);
        //}

        /// <summary>
        /// Serializes the <param name="data">data</param> object into a JSON string
        /// and writes it to the response.
        /// </summary>
        public void JsonResult(object data)
        {
            Response.ContentType = "application/json";
            Response.Write(DisplayExtensions.Json(data));
            Response.End();
        }

        /// <summary>
        /// Returns an Html-Encoded string to be put inline with an Html Element with the
        /// data-attribute name "data-tp".  Ie, returns: data-tp="{ encoded Json }"
        /// </summary>
        //public IHtmlString TPData(object data)
        //{
        //    return DisplayExtensions.TPData(data);
        //}

        #endregion

        #region MarkDown

        public static IHtmlString FromMarkdown(string raw)
        {
            return MarkdownExtensions.FromMarkdown(raw);
        }

        #endregion

        private Analytics _googleAnalytics;
        public Analytics GoogleAnalytics
        {
            get { return _googleAnalytics ?? (_googleAnalytics = new Analytics()); }
        }

        public string Meta(string key, string value)
        {
            //add meta tag

            return string.Format(@"<meta key=""{0}"" value=""{1}"" />", key, value);
        }

        #region URL Generation

        public string SlugUrl(string slug)
        {
            return Href(string.Format("~/{0}/", slug));
        }

        public string CanonicalUrl(string path, params object[] parameters)
        {
            return DisplayExtensions.CanonicalUrl(
                (string) MiniBlogSettingsProvider.Settings["CanonicalRoot"], 
                path,
                parameters);
        }
        //public string SubHref(string relativePath, string subdomain = null)
        //{
        //    return UrlFactory.SubdomainPath(relativePath, subdomain ?? Identity.SubDomain);
        //}
        //public string Static(string relativePath, params object[] pathParts)
        //{
        //    return UrlFactory.StaticContent(relativePath, pathParts);
        //}

        #endregion
    }
}