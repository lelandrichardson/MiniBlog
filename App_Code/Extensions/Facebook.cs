using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Extensions
{
    public static class Facebook
    {
        public const string FacebookAppId = "248957485211620"; //TODO: move to config file.
        public static HtmlString Like(
            string href, 
            string layout = "button_count",  //button_count,standard,box_count
            string width = "44", 
            bool showFaces = false, 
            bool send = false, 
            string font = "tahoma")
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("fb-like");
            var attr = new Dictionary<string, string>
                {
                    {"data-href", href},
                    {"data-layout", layout},
                    {"data-width", width},
                    {"data-show-faces", showFaces.ToString().ToLower()},
                    {"data-font", font}
                };

            tag.MergeAttributes(attr);

            return new HtmlString(tag.ToString(TagRenderMode.Normal));
        }

        public static HtmlString AsyncApiScript()
        {
            return new HtmlString(string.Format(
@"<script>(function(d, s, id) {{
  var js, fjs = d.getElementsByTagName(s)[0];
  if (d.getElementById(id)) return;
  js = d.createElement(s); js.id = id;
  js.src = ""//connect.facebook.net/en_US/all.js#xfbml=1&appId={0}"";
  fjs.parentNode.insertBefore(js, fjs);
}}(document, 'script', 'facebook-jssdk'));</script>",FacebookAppId));
        }
    }
}