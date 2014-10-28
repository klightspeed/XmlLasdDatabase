using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("formattedText", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class FormattedText
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlAnyElement()]
        public XElement[] Elements { get; set; }

        public static AchievementDescriptor FromXElement(XElement el)
        {
            return new AchievementDescriptor
            {
                Elements = el.Elements().ToArray()
            };
        }

        protected XElement GetTermRef(XElement element)
        {
            XAttribute name = element.Attribute("name");

            return new XElement("term", name, element.Value);
        }

        protected IEnumerable<XNode> GetTextRun(XElement element)
        {
            foreach (XNode node in element.Nodes())
            {
                if (node is XText)
                {
                    yield return node;
                }
                else if (node is XElement)
                {
                    XElement el = (XElement)node;
                    if (el.Name.LocalName == "b")
                    {
                        yield return new XElement("b", GetTextRun(el));
                    }
                    else if (el.Name.LocalName == "u")
                    {
                        yield return new XElement("u", GetTextRun(el));
                    }
                    else if (el.Name.LocalName == "i")
                    {
                        yield return new XElement("i", GetTextRun(el));
                    }
                    else if (el.Name.LocalName == "term")
                    {
                        yield return GetTermRef(el);
                    }
                }
            }
        }

    }
}
