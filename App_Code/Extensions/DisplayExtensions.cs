using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace MiniBlog.Extensions
{
    public static class DisplayExtensions
    {
        #region To Time Ago Date Formatting

        private const long sPerMinute = 60;
        private const long sPerHour = sPerMinute * 60;
        private const long sPerDay = sPerHour * 24;
        private const long sPerMonth = sPerDay * 30;


        
        //------------------------ returns a shortened formatted string (ie, 2700 -> "2.7k")
        public static string toShortString(this int num)
        { 
            if(num < 1000) 
            {
                return "" + num;
            }
            else if(num < 10000) 
            {
                return Math.Round( (double) num / 1000, 1).ToString() + "k";
            } 
            else 
            {
                return Math.Round( (double) num / 1000, 0).ToString() + "k";
            }
  
        }

         

        public static string ToTimeAgo(this DateTime dt)
        {
            var ts = DateTime.Now - dt;
            var seconds = (long)Math.Floor(ts.TotalSeconds);


            if (seconds < sPerMinute)
            {
                //return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
                return "just now";
            }
            if (seconds < sPerMinute * 2)
            {
                return "a minute ago";
            }
            if (seconds < sPerMinute * 45) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (seconds < sPerMinute * 90) // 90 * 60
            {
                return "an hour ago";
            }
            if (seconds < sPerDay) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (seconds < sPerDay * 2) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (seconds < sPerMonth) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (seconds < sPerMonth * 12) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        #endregion

        #region Seo Friendly Strings (Slugs)

        // remove hyphens from string
        private static readonly Regex HyphenRegex = new Regex(@"( ?- ?)|[ ']",RegexOptions.Compiled);
        // replace all non alpha-numeric characters with hyphen
        private static readonly Regex RemoveRegex = new Regex(@"[^0-9a-zA-Z-]", RegexOptions.Compiled);
           
        public static string SeoFriendly(this string s)
        {
            return RemoveRegex.Replace(HyphenRegex.Replace(s, "-"), "").Trim('-').ToLower();
        }

        #endregion

        #region Converting Content to Abstracts

        public static IHtmlString ShortWithEllipses(string body, int maxChars = 1000)
        {
            //TODO: create function to cut a large string down to last word up to maxChars
            // and add ellipses to the end.
            return new HtmlString(body);
        }

        #endregion

        #region Page-Level CSS Classes via HTTP Context

        public static string PageClasses()
        {
            var r = HttpContext.Current.Request;
            var classes = new List<string>();

            classes.Add("is" + r.Browser.Browser);
            classes.Add(string.Format("ver{0}", r.Browser.Version.Replace(".", "")));

            if(r.IsAuthenticated) classes.Add("isLoggedIn");
            if(r.IsLocal) classes.Add("isLocal");
            if(r.Browser.Crawler) classes.Add("isBot");
            if(r.Browser.IsMobileDevice) classes.Add("isMobile");

            return string.Join(" ", classes);
        }

        #endregion

        #region Json Display Helpers

        /// <summary>
        /// Returns serialized Json string.  Default encoding.
        /// </summary>
        public static string Json(object data)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(data);
        }

        /// <summary>
        /// Returns serialized Json string for type T, utilizing 
        /// DataContract/DataMember attributes.  Default encoding. 
        /// </summary>
        public static string Json<T>(object data)
        {
            //var serializer = new JavaScriptSerializer();
            var serializer = new DataContractJsonSerializer(typeof(T));
            using(var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                stream.Position = 0;
                var sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
        }

    /// <summary>
    /// Returns an Html-Encoded string to be put inline with an Html Element with the
    /// data-attribute name "data-tp".  Ie, returns: data-tp="{ encoded Json }"
    /// </summary>
    public static IHtmlString TPData(object data)
    {
        var serializer = new JavaScriptSerializer();
        return new HtmlString(string.Format(
            "data-tp=\"{0}\"",
            HttpContext.Current.Server.HtmlEncode(serializer.Serialize(data))
            ));
    }

        #endregion


        #region Static Content &  Url Generation

        public static string CanonicalUrl(string rootUrl, string path, params object[] parameters)
        {
            if(parameters != null && parameters.Length > 0)
            {
                return string.Format(path, parameters).Replace("~", rootUrl);
            }
            return path.Replace("~", rootUrl);
        }

        //public static string StaticContentUrl(string basePath, string path, params object[] pathParts)
        //{
        //    if (basePath != null)
        //    {
        //        path = VirtualPathUtility.Combine(basePath, path);
        //    }

        //    // Make sure it's not a ~/ path, which the client couldn't handle
        //    if (HttpRuntime.AppDomainAppVirtualPath != null)
        //    {
        //        path = s_helperControl.ResolveClientUrl(path);
        //    }

        //    return BuildUrl(path, pathParts);
        //}

        //private static Control s_helperControl = CreateHelperControl();

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The object is used by the caller and does not go out of scope")]
        //private static Control CreateHelperControl()
        //{
        //    var control = new Control();
        //    control.AppRelativeTemplateSourceDirectory = "~/";
        //    return control;
        //}

        #endregion
    }

}