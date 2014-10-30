using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("formattedText", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class FormattedText
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlAnyElement()]
        public XElement[] Elements { get; set; }

        protected IEnumerable<XNode> RemoveTerms(XElement element)
        {
            if (element.Name.LocalName == "term")
            {
                foreach (XNode node in element.Nodes())
                {
                    if (node is XElement)
                    {
                        foreach (XNode n in RemoveTerms((XElement)node))
                        {
                            yield return n;
                        }
                    }
                    else
                    {
                        yield return node;
                    }
                }
            }
            else
            {
                yield return element;
            }
        }

        protected IEnumerable<XNode> UnescapeSpace(List<XNode> nodes)
        {
            yield return UnescapeSpace(nodes[0]);

            if (nodes.Count >= 2)
            {
                for (int i = 1; i < nodes.Count - 1; i++)
                {
                    if (nodes[i] is XElement)
                    {
                        if (((XElement)nodes[i]).Name.LocalName == "space" && nodes[i - 1] is XText && nodes[i + 1] is XText)
                        {
                            yield return new XText(" ");
                        }
                        else
                        {
                            yield return UnescapeSpace(nodes[i]);
                        }
                    }
                    else
                    {
                        yield return nodes[i];
                    }
                }

                yield return UnescapeSpace(nodes[nodes.Count - 1]);
            }
        }

        protected XElement UnescapeSpace(XElement element)
        {
            return new XElement(element.Name, element.Attributes(), UnescapeSpace(element.Nodes().ToList()));
        }

        protected XNode UnescapeSpace(XNode node)
        {
            if (node is XElement)
            {
                return UnescapeSpace((XElement)node);
            }
            else
            {
                return node;
            }
        }

        protected XElement MergeFormatting(XElement element, string type)
        {
            XElement ret = new XElement(element.Name, element.Attributes());
            List<XNode> nodes = element.Nodes().Select(n =>
            {
                if (n is XElement)
                {
                    return MergeFormatting((XElement)n, type);
                }
                else
                {
                    return n;
                }
            }).ToList();

            if (nodes.Count == 1 && nodes[0] is XElement)
            {
                XElement child = (XElement)nodes[0];
                if (element.Name.LocalName != "p" && element.Name.LocalName != "li")
                {
                    if (child.Name.LocalName == type)
                    {
                        return new XElement(child.Name, child.Attributes(),
                            new XElement(element.Name, element.Attributes(), child.Nodes())
                        );
                    }
                }
            }

            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                if (nodes[i + 1] is XElement && nodes[i] is XElement)
                {
                    XElement el1 = (XElement)nodes[i];
                    XElement el2 = (XElement)nodes[i + 1];

                    if (el1.Name.LocalName == type && el2.Name.LocalName == type)
                    {
                        nodes[i] = new XElement(el1.Name, el1.Attributes(), el2.Attributes(), el1.Nodes(), el2.Nodes());
                        nodes[i + 1] = null;
                    }
                }
            }

            ret.Add(nodes.Where(n => n != null));

            return ret;
        }

        protected IEnumerable<XNode> FindTerms(string text, Dictionary<string, string> terms)
        {
            string lowertext = text.ToLower();
            int startpos = 0;

            do
            {
                int matchpos = text.Length;
                int matchlen = 0;
                string matchname = null;

                foreach (KeyValuePair<string, string> term in terms)
                {
                    int tmatchpos = lowertext.IndexOf(term.Key, startpos);

                    if (tmatchpos >= startpos &&
                        (tmatchpos < matchpos ||
                         (tmatchpos == matchpos && term.Key.Length > matchlen)))
                    {
                        matchpos = tmatchpos;
                        matchlen = term.Key.Length;
                        matchname = term.Value;
                    }
                }

                if (matchlen != 0)
                {
                    if (matchpos != startpos)
                    {
                        int prematchpos = matchpos;

                        if (text[startpos] == ' ')
                        {
                            yield return new XElement(ns + "space", new XAttribute(XNamespace.Xml + "space", "preserve"), " ");
                            while (startpos < matchpos && text[startpos] == ' ')
                            {
                                startpos++;
                            }
                        }

                        while ((prematchpos - startpos) >= 2 && text[prematchpos - 1] == ' ')
                        {
                            prematchpos--;
                        }

                        yield return new XText(text.Substring(startpos, prematchpos - startpos));

                        if (text[prematchpos] == ' ')
                        {
                            yield return new XElement(ns + "space", new XAttribute(XNamespace.Xml + "space", "preserve"), " ");
                        }
                    }

                    yield return new XElement(ns + "term",
                        new XAttribute("name", matchname),
                        new XText(text.Substring(matchpos, matchlen))
                    );
                }
                else
                {
                    if (text[startpos] == ' ')
                    {
                        yield return new XElement(ns + "space", new XAttribute(XNamespace.Xml + "space", "preserve"), " ");
                        while (startpos < matchpos && text[startpos] == ' ')
                        {
                            startpos++;
                        }
                    }

                    yield return new XText(text.Substring(startpos, text.Length - startpos));
                }

                startpos = matchpos + matchlen;
            }
            while (startpos < text.Length);
        }

        protected XElement FindTerms(XElement element, Dictionary<string, string> terms)
        {
            XElement ret = new XElement(element.Name, element.Attributes());
            StringBuilder sb = new StringBuilder();

            foreach (XNode node in element.Nodes())
            {
                if (node is XText)
                {
                    sb.Append(((XText)node).Value);
                }
                else
                {
                    if (sb.Length != 0)
                    {
                        ret.Add(FindTerms(sb.ToString(), terms));
                        sb = new StringBuilder();
                    }

                    if (node is XElement)
                    {
                        ret.Add(FindTerms((XElement)node, terms));
                    }
                    else
                    {
                        ret.Add(node);
                    }
                }
            }

            if (sb.Length != 0)
            {
                ret.Add(FindTerms(sb.ToString(), terms));
            }

            return ret;
        }

        public void FindTerms(Dictionary<string, string> terms)
        {
            Elements = Elements.SelectMany(el => RemoveTerms(el))
                               .OfType<XElement>()
                               .Select(el => MergeFormatting(el, "b"))
                               .Select(el => MergeFormatting(el, "u"))
                               .Select(el => MergeFormatting(el, "i"))
                               .Select(el => UnescapeSpace(el))
                               .Select(el => FindTerms(el, terms))
                               .ToArray();
        }

        public static FormattedText FromXElement(XElement el)
        {
            return new FormattedText
            {
                Elements = el.Elements().ToArray()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name, Elements);
        }

    }
}
