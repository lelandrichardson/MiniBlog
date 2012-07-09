using System;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownSharp;

namespace MiniBlog.Extensions
{

    // much of htis was taken from http://www.codetunnel.com/blog/post/24/mardownsharp-and-encoded-markdown
    public static class MarkdownExtensions
    {
        private static Regex _tags = new Regex("<[^>]*(>|$)",
    RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static Regex _whitelist = new Regex(@"
    ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
    ^<(b|h)r\s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_a = new Regex(@"
    ^<a\s
    href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
    (\stitle=""[^""<>]+"")?\s?>$|
    ^</a>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_img = new Regex(@"
    ^<img\s
    src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
    (\swidth=""\d{1,3}"")?
    (\sheight=""\d{1,3}"")?
    (\salt=""[^""<>]*"")?
    (\stitle=""[^""<>]*"")?
    \s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// sanitize any potentially dangerous tags from the provided raw HTML input using
        /// a whitelist based approach, leaving the "safe" HTML tags
        /// CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public static string Sanitize(string html)
        {
            if (String.IsNullOrEmpty(html)) return html;
            string tagname;
            Match tag;
            // match every HTML tag in the input
            MatchCollection tags = _tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();
                if (!(_whitelist.IsMatch(tagname) || _whitelist_a.IsMatch(tagname) || _whitelist_img.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                    System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
            }
            return html;
        }

        public static IHtmlString FromMarkdown(string text)
        {
            string html = (new Markdown()).Transform(text);
            //html = Sanitize(html);
            return new HtmlString(html);
        }

        /// <summary>
        /// This is used to give a preview of the markdown content.
        /// This will strip html tags and truncate the remaining string, based on length provided.
        /// Then will check for the last remaining whole word.  Note: this is not fool-proof.
        /// </summary>
        /// <param name="numchars">the maximum number of characters you want returned.</param>
        /// <returns></returns>
        public static string FromMarkdownPreview(string markdown, int numchars = 100)
        {
            string text = Regex.Replace(markdown, "<(.|\n)*?>", string.Empty);
            if (text.Length > numchars)
            {
                text = text.Substring(0, numchars);
                int index = text.LastIndexOf(' ');
                if (index > 0)
                {
                    text = text.Substring(0, index); 
                }
                text += "...";
            }

            return text;
        }
    }

}