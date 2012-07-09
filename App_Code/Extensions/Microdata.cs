using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MiniBlog.Extensions
{
    public static class MicroData
    {
        //HTML Attributes
        public static HtmlString Item(string itemType)
        {
            var rvd = new Dictionary<string, string> { { "itemscope", "" }, { "itemtype", itemType.StartsWith("http") ? itemType : string.Concat(SchemaUrl, itemType) } };
            return new HtmlString(ToHtmlAttributes(rvd));
        }

        private const string SchemaUrl = @"http://schema.org/";
        public static HtmlString Prop(string propName, string itemType)
        {
            return new HtmlString(string.Format("itemscope itemprop=\"{0}\" itemtype=\"{1}\"", propName, itemType.StartsWith("http") ? itemType : string.Concat(SchemaUrl, itemType)));
        }
        public static HtmlString Prop(string propName)
        {
            //return: itemprop="<propName>"
            return new HtmlString(string.Format("itemprop=\"{0}\"", propName));
        }

        //HTML Elements

        public static HtmlString Wrap(string propName, string content)
        {
            var builder = new TagBuilder("span");
            builder.MergeAttribute("itemprop", propName);
            builder.InnerHtml = content;
            return new HtmlString(builder.ToString(TagRenderMode.Normal));
        }


        public static HtmlString Meta(string propName, object content) { return Meta(propName, content.ToString()); }
        public static HtmlString Meta(string propName, DateTime content) { return Meta(propName, content.ToString("s")); }
        public static HtmlString Meta(string propName, string content)
        {
            var builder = new TagBuilder("meta");
            builder.MergeAttribute("itemprop", propName);
            builder.MergeAttribute("content", content);
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }



        public static HtmlString Date(string propName, DateTime date)
        {
            var builder = new TagBuilder("time");
            builder.InnerHtml = date.ToString(DefaultDateFormatString);
            builder.MergeAttribute("itemprop", propName);
            builder.MergeAttribute("datetime", date.ToString("yyyy-MM-dd"));
            return new HtmlString(builder.ToString(TagRenderMode.Normal));
        }

        private const string DefaultDateFormatString = "MMM-dd-yyyy";
        public static HtmlString DateTime(string propName, DateTime date, string format = "")
        {
            var builder = new TagBuilder("time");
            builder.InnerHtml = date.ToString(DefaultDateFormatString);
            builder.MergeAttribute("itemprop", propName);
            builder.MergeAttribute("datetime", date.ToString("s"));
            return new HtmlString(builder.ToString(TagRenderMode.Normal));
        }

        private static string ToHtmlAttributes(IDictionary<string, string> dictionary)
        {
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                sb.AppendFormat("{0}=\"{1}\" ", pair.Key, pair.Value);
            }
            return sb.ToString();
        }

        private static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            var result = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(htmlAttributes))
                {
                    result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }
            return result;
        }

    }
}