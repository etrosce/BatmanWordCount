using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatmanWordCount
{
    public static class HtmlExtensions
    {
        #region Public Methods

        public static void ToClearText(this HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // avoid comments
                    break;

                case HtmlNodeType.Document:
                    foreach (HtmlNode subnode in node.ChildNodes)
                    {
                        subnode.ToClearText(outText);
                    }
                    break;

                case HtmlNodeType.Text:
                    // avoid script and style tags
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
                        outText.Write(HtmlEntity.DeEntitize(html) + " ");
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write(Environment.NewLine);
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        foreach (HtmlNode subnode in node.ChildNodes)
                        {
                            subnode.ToClearText(outText);
                        }
                    }
                    break;
            }
        }

        public static string ToClearText(this HtmlNode node)
        {
            using (StringWriter sw = new StringWriter())
            {
                node.ToClearText(sw);
                return sw.ToString();
            }
        }

        public static IEnumerable<string> ParseLinks(this HtmlAgilityPack.HtmlNode node)
        {
            HtmlNodeCollection hrefs = node.SelectNodes("//a[@href]");
            if (hrefs == null)
                return Enumerable.Empty<string>();

            return hrefs.Select(href => href.Attributes["href"].Value)
                .Where(l => l.StartsWith("/wiki/") && !l.Contains(":")).Distinct();
        }
        #endregion

    }
}
