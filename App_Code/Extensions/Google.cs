using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Extensions
{
    public static class Google
    {
        public static HtmlString PlusOne(string href, string annotation = null, string size = "medium")
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("g-plusone");
            var attr = new Dictionary<string, string>
                           {
                               {"data-href", href}
                           };
            if (size != null) attr.Add("data-size", size);
            if (annotation != null) attr.Add("data-annotation", annotation);
            tag.MergeAttributes(attr);
            return new HtmlString(tag.ToString(TagRenderMode.Normal));
        }
        public static HtmlString AsyncApiScript()
        {
            return new HtmlString(
@"<script type=""text/javascript"">
  (function() {
    var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
    po.src = 'https://apis.google.com/js/plusone.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
  })();
</script>");
        }
    }
}