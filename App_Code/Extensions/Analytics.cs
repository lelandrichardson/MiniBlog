using System.Collections.Generic;
using System.Text;
using System.Web;
using MiniBlog.Includes.Data;

namespace MiniBlog.Extensions
{
    public class Analytics
    {
        private const string LoadScript =
@"(function() {
var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();";

        public HtmlString Render()
        {
            var b = new StringBuilder();

            b.AppendLine(@"<script type=""text/javascript"">");

            b.AppendLine("var _gaq = _gaq || [];");
            b.AppendLine(SetGaq("_setAccount", MiniBlogSettingsProvider.Settings["GoogleAnalyticsId"].ToString())); 
            //b.AppendLine(SetGaq("_setDomainName", "tech.pro"));//TODO: update
            b.AppendLine(SetGaq("_trackPageview"));

            foreach (var setting in Settings)
            {
                b.AppendLine(setting);
            }

            if(Tags.Count > 0)
            {
                b.AppendLine(TagsVariable());
            }


            b.AppendLine(LoadScript);

            b.AppendLine(@"</script>");

            return new HtmlString(b.ToString());
        }

        private List<string> Settings = new List<string>();
        public List<string> Tags = new List<string>(); 

        private string TagsVariable()
        {
            return SetCustomVar(1, "tags", string.Join("|", Tags), (int)Scope.Page);
        }
        /// <summary>
        /// Add Google analytics custom variable.
        /// </summary>
        /// <param name="index">The slot for the custom variable. <b>Required</b>. This is a number whose value can range from 1 - 5, inclusive. A custom variable should be placed in one slot only and not be re-used across different slots.</param>
        /// <param name="name">The name for the custom variable. <b>Required</b>. This is a string that identifies the custom variable and appears in the top-level Custom Variables report of the Analytics reports.</param>
        /// <param name="value">The value for the custom variable. <b>Required</b>. This is a string that is paired with a name. You can pair a number of values with a custom variable name. The value appears in the table list of the UI for a selected variable name. Typically, you will have two or more values for a given name. For example, you might define a custom variable name gender and supply male and female as two possible values.</param>
        /// <param name="scope">The scope for the custom variable. Optional. As described above, the scope defines the level of user engagement with your site.</param>
        public void AddCustomVariable(int index, string name, string value, Scope scope = Scope.Page)
        {
            Settings.Add(SetCustomVar(index, name, value, (int)scope));
        }

        public void AddEvent(string category, string action, string label, int value, bool nonInteraction = true)
        {
            Settings.Add(TrackEvent(category, action, label, value, nonInteraction));
        }

        private static string SetCustomVar(int index, string name, string value, int scope)
        {
            return string.Format("_gaq.push(['_setCustomVar',{0},'{1}','{2}',{3}]);", index, name, value, scope);
        }

        private static string TrackEvent(string category, string action, string label, int value, bool nonInteraction = true)
        {
            return string.Format("_gaq.push(['_trackEvent','{0}','{1}','{2}',{3},{4}", category, action, label, value,nonInteraction);
        }

        private static string SetGaq(params string[] vars)
        {
            return string.Format("_gaq.push(['{0}']);", string.Join("', '", vars));
        }

        public enum Scope
        {
            Visitor = 1, Session = 2, Page = 3
        }
    }

    
}