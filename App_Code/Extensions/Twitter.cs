using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Extensions
{
    public static class Twitter
    {
        public const string TwitterUserName = "TechPro"; //TODO: move to config file.

        public static HtmlString Button(
            string href, 
            string text = null, 
            string relatedUsers = null, 
            string countPosition = "horizontal", 
            string size = null)
        {
            var tag = new TagBuilder("a");
            tag.AddCssClass("twitter-share-button");
            tag.InnerHtml = "Tweet";
            var attr = new Dictionary<string, string>
                           {
                               {"href","https://twitter.com/share"},
                               {"data-url",href},
                               {"data-via",TwitterUserName},
                               {"data-lang","en"},
                               {"data-count",countPosition}, //"horizontal", "vertical", or "none"
                           };

            if (text != null) attr.Add("data-text", text);
            if (size != null) attr.Add("data-size", size);
            if (relatedUsers != null) attr.Add("data-related", relatedUsers); //could fill with asker/answerer... comma separated

            tag.MergeAttributes(attr);
            
            return new HtmlString(tag.ToString(TagRenderMode.Normal));
        }

        public static HtmlString AsyncApiScript()
        {
            return new HtmlString(
                @"<script>!function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0];if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src=""//platform.twitter.com/widgets.js"";fjs.parentNode.insertBefore(js,fjs);}}(document,""script"",""twitter-wjs"");</script>");
        }
    }
}