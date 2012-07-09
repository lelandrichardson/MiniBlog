using System.Web;

namespace MiniBlog.Extensions
{
    public static class LinkedIn
    {
        public static HtmlString Share(string url, string counterPosition = "right")
        {
            //counterposition: right, top, (nothing)
            return new HtmlString(
                string.Format(@"<script type=""IN/Share"" data-url=""{0}"" data-counter=""{1}""></script>",url, counterPosition));
        }
        public static HtmlString AsyncApiScript()
        {
            return new HtmlString(
@"<script src=""//platform.linkedin.com/in.js"" type=""text/javascript""></script>");
        }
    }
}