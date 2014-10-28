using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("achievementRow", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementDescriptor
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
                }
            }
        }
        
        public LASDEntry.AchievementDescriptor ToAchievementDescriptor()
        {
            return new LASDEntry.AchievementDescriptor
            {
                Xml = new XElement("desc",
                    Elements.Select(el =>
                    {
                        if (el.Name.LocalName == "p")
                        {
                            return new XElement("p", GetTextRun(el));
                        }
                        else if (el.Name.LocalName == "ul")
                        {
                            return new XElement("ul",
                                el.Elements(ns + "li").Select(e => new XElement("li", GetTextRun(e)))
                            );
                        }
                        else
                        {
                            return null;
                        }
                    })
                )
            };
        }
    }
}
