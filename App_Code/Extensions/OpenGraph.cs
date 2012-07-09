using System.Text;
using System.Web;
using System.Web.Mvc;
using MiniBlog.Includes.Data;

namespace MiniBlog.Extensions
{
    public static class OpenGraph
    {
        public static HtmlString Set(string title, string type, string url, string image)
        {
            var s = new StringBuilder();
            s.AppendLine(OptionElement("type", type));
            s.AppendLine(OptionElement("title", title));
            s.AppendLine(OptionElement("url", url));
            s.AppendLine(OptionElement("image", image));
            var appId = MiniBlogSettingsProvider.Settings["OpenGraphAppId"];
            if(appId != null)
            {
                s.AppendLine(OptionElement("app_id", (string)appId));
            }
            s.AppendLine(OptionElement("site_name", (string)MiniBlogSettingsProvider.Settings["BlogName"]));

            return new HtmlString(s.ToString());
        }

        public static HtmlString Description(string description)
        {
            return Option("description", description);
        }
        
        private static string OptionElement(string property, string value)
        {
            var builder = new TagBuilder("meta");
            builder.MergeAttribute("content", value);
            builder.MergeAttribute("property",string.Format("og:{0}",property));
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static HtmlString Option(string property, string content)
        {
            return new HtmlString(OptionElement(property,content));
        }

    }
}