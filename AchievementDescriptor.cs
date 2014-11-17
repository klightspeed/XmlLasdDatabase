using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TSVCEO.LASDDatabase
{
    public class AchievementDescriptor
    {
        public string Text { get; set; }
        public XElement Xml
        {
            get
            {
                if (Text.StartsWith("<") && Text.Contains(">"))
                {
                    return XElement.Parse("<desc>" + Text + "</desc>", LoadOptions.PreserveWhitespace);
                }
                else
                {
                    return new XElement("desc", Text.Split('\n').Select(s => new XElement("p", s)));
                }
            }
            set
            {
                this.Text = String.Join("", value.Elements().Select(e => e.ToString(SaveOptions.DisableFormatting)));
            }
        }

        protected static readonly string[] suffixes = new string[]
        {
            "ing", "ed", "ly", "ise", "ised"
        };

        public static IEnumerable<XNode> RemoveTerms(XElement element)
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
                yield return new XElement(
                    element.Name,
                    element.Attributes(),
                    element.Nodes().SelectMany(n => n is XElement ? RemoveTerms((XElement)n) : new XNode[] { n })
                );
            }
        }

        public void RemoveTerms()
        {
            this.Xml = RemoveTerms(this.Xml).OfType<XElement>().Single();
        }

        protected static IEnumerable<XNode> UnescapeSpace(List<XNode> nodes)
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

        protected static XElement UnescapeSpace(XElement element)
        {
            return new XElement(element.Name, element.Attributes(), UnescapeSpace(element.Nodes().ToList()));
        }

        protected static XNode UnescapeSpace(XNode node)
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

        protected static XElement MergeFormatting(XElement element, string type)
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

        protected static IEnumerable<XNode> FindTerms(XNamespace ns, string text, Dictionary<string, string> terms)
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
                        int tmatchlen = term.Key.Length;
                        int wordend = lowertext.IndexOfAny(" :;,.".ToArray(), tmatchpos + tmatchlen);

                        if (tmatchpos == 0 || " :;,.".Contains(lowertext[tmatchpos - 1]))
                        {
                            if (wordend == tmatchpos + tmatchlen)
                            {
                                matchpos = tmatchpos;
                                matchlen = tmatchlen;
                                matchname = term.Value;
                            }
                            else if (wordend > (tmatchpos + tmatchlen))
                            {
                                string suffix = lowertext.Substring(tmatchpos + tmatchlen, wordend - (tmatchpos + tmatchlen));

                                if (suffixes.Contains(suffix))
                                {
                                    matchpos = tmatchpos;
                                    matchlen = tmatchlen + suffix.Length;
                                    matchname = term.Value;
                                }
                            }
                        }
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
                        yield return new XElement("space", new XAttribute(XNamespace.Xml + "space", "preserve"), " ");
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

        protected static XElement FindTerms(XNamespace ns, XElement element, Dictionary<string, string> terms)
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
                        ret.Add(FindTerms(ns, sb.ToString(), terms));
                        sb = new StringBuilder();
                    }

                    if (node is XElement)
                    {
                        ret.Add(FindTerms(ns, (XElement)node, terms));
                    }
                    else
                    {
                        ret.Add(node);
                    }
                }
            }

            if (sb.Length != 0)
            {
                ret.Add(FindTerms(ns, sb.ToString(), terms));
            }

            return ret;
        }

        public static XElement[] FindTerms(XNamespace ns, XElement[] elements, Dictionary<string, string> terms)
        {
            return elements.SelectMany(el => RemoveTerms(el))
                           .OfType<XElement>()
                           .Select(el => MergeFormatting(el, "b"))
                           .Select(el => MergeFormatting(el, "u"))
                           .Select(el => MergeFormatting(el, "i"))
                           .Select(el => UnescapeSpace(el))
                           .Select(el => FindTerms(ns, el, terms))
                           .ToArray();
        }

        public static XElement FindTerms(XElement input, IEnumerable<Term> terms)
        {
            Dictionary<string, string> keywords = terms.SelectMany(t => t.Keywords.Select(k => new { keyword = k.ToLower(), name = t.Name })).ToDictionary(kn => kn.keyword, kn => kn.name);
            return FindTerms("", input, keywords);
        }

        public void FindTerms(IEnumerable<Term> terms)
        {
            this.Xml = FindTerms(this.Xml, terms);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (!(obj is AchievementDescriptor))
            {
                return false;
            }
            else
            {
                return ((AchievementDescriptor)obj).Text == this.Text;
            }
        }

        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }
    }
}
