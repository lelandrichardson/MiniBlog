using System;
using System.Text;
using System.Web;
using System.Xml;

namespace MiniBlog.Framework
{
    public static class Rss
    {
        public static XmlTextWriter CreateFeedWriter()
        {
            return new XmlTextWriter(HttpContext.Current.Response.OutputStream, Encoding.UTF8);
        }

        public static void End(XmlTextWriter xtwFeed)
        {
            xtwFeed.WriteEndElement();
            xtwFeed.WriteEndElement();
            xtwFeed.WriteEndDocument();
            xtwFeed.Flush();
            xtwFeed.Close();
            HttpContext.Current.Response.End();
        }

        public static void Start(XmlTextWriter xtwFeed, string title, string mainUrl, string imageUrl)
        {
            xtwFeed.WriteStartDocument();
            // The mandatory rss tag
            xtwFeed.WriteStartElement("rss");
            xtwFeed.WriteAttributeString("version", "2.0");
            //xtwFeed.WriteAttributeString("xmlns:media", "http://search.yahoo.com/mrss/");
            // The channel tag contains RSS feed details
            xtwFeed.WriteStartElement("channel");
            xtwFeed.WriteElementString("title", title);
            xtwFeed.WriteElementString("link", mainUrl);
            xtwFeed.WriteElementString("description", "");
            //xtwFeed.WriteElementString("copyright", "");
            xtwFeed.WriteElementString("pubDate", DateTime.Now.ToString("r"));


            xtwFeed.WriteStartElement("image");
                xtwFeed.WriteElementString("url", imageUrl);
                xtwFeed.WriteElementString("link", mainUrl);
                xtwFeed.WriteElementString("title", title);
            xtwFeed.WriteEndElement();
        }

        public static void Item(XmlTextWriter w, 
            string title, 
            string description, 
            string link, 
            string guid, 
            DateTime pubDate)
        {
            w.WriteStartElement("item");
                w.WriteElementString("title", title);
                w.WriteElementString("description", description);
                w.WriteElementString("link", link);
                w.WriteElementString("guid", guid);
                w.WriteElementString("pubDate", pubDate.ToString("r"));
            w.WriteEndElement();
        }
    }
}