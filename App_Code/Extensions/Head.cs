using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Extensions
{
    public static class Head
    {
        public static HtmlString RssLink(string href, string title = null)
        {
            return Link("alternate", href, "application/rss+xml", title);
        }

        public static HtmlString Canonical(string href, string title = null)
        {
            return Link("canonical", href,null,title);
        }

        public static HtmlString Link(string rel, string href, string type = null, string title = null)
        {
            var tag = new TagBuilder("link");
            tag.MergeAttribute("rel", rel);
            tag.MergeAttribute("href", href);
            if (type != null) tag.MergeAttribute("type", type);
            if (title != null) tag.MergeAttribute("title", title);

            return new HtmlString(tag.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString Meta(string name, string content)
        {
            var tag = new TagBuilder("meta");
            tag.MergeAttribute("name", name);
            tag.MergeAttribute("content", content);

            return new HtmlString(tag.ToString(TagRenderMode.SelfClosing));
        }
    }
}