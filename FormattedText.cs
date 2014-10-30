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

        protected XElement FindTerms(XElement element, Dictionary<string, string> terms)
        {
            XElement ret = new XElement(element.Name, element.Attributes());

            foreach (XNode node in element.Nodes())
            {
                if (node is XElement)
                {
                    ret.Add(FindTerms((XElement)node, terms));
                }
                else if (node is XText)
                {
                    string text = ((XText)node).Value;
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
                                ret.Add(new XText(text.Substring(startpos, matchpos - startpos)));
                            }

                            ret.Add(new XElement(ns + "term",
                                new XAttribute("name", matchname),
                                new XText(text.Substring(matchpos, matchlen))
                            ));
                        }
                        else
                        {
                            ret.Add(new XText(text.Substring(startpos, text.Length - startpos)));
                        }

                        startpos = matchpos + matchlen;
                    }
                    while (startpos < text.Length);
                }
                else
                {
                    ret.Add(node);
                }
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
