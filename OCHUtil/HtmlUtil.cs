using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCHUtil
{
    public class HtmlUtil
    {
        public static string ConvertFromFile(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            return CovertFromDocument(doc);
        }

        public static string ConvertFromHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            return CovertFromDocument(doc);
        }

        public static string CovertFromDocument(HtmlDocument doc)
        {
            StringBuilder sw = new StringBuilder();
            ConvertTo(doc.DocumentNode, sw);
            return sw.ToString();
        }

        private static void ConvertTo(HtmlNode node, StringBuilder outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Append(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                        case "br":
                            // treat paragraphs as crlf
                            outText.Append(Environment.NewLine);
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }

        private static void ConvertContentTo(HtmlNode node, StringBuilder outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }
    }
}
