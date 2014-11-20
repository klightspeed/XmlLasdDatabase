using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("formattedText", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class FormattedText
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlAnyElement()]
        public XElement[] Elements { get; set; }

        public void FindTerms(Dictionary<string, string> terms)
        {
            Elements = LASD.AchievementDescriptor.FindTerms(ns, Elements, terms);
        }

        protected XElement StripNamespaces(XElement el)
        {
            return new XElement(el.Name.LocalName,
                el.Attributes().Where(a => !a.IsNamespaceDeclaration).Select(a => new XAttribute(a.Name.LocalName, a.Value)),
                el.Nodes().Select(n => (n is XElement) ? StripNamespaces((XElement)n) : n)
            );
        }

        public static FormattedText FromXElement(XElement el)
        {
            return new FormattedText
            {
                Elements = el.Elements().ToArray()
            };
        }

        public XElement ToXElement(XName name, bool stripNamespaces = false)
        {
            XElement el = new XElement(name, Elements);

            if (stripNamespaces)
            {
                el = StripNamespaces(el);
            }

            return el;
        }

        public LASD.AchievementDescriptor ToLASD()
        {
            return new LASD.AchievementDescriptor
            {
                Xml = StripNamespaces(this.ToXElement("desc"))
            };
        }
    }
}
