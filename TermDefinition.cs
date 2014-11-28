using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("termDefinition", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class TermDefinition
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("keyword", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<string> Keywords { get; set; }

        [XmlElement("description", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public FormattedText Description { get; set; }

        [XmlAttribute("name", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string Name { get; set; }

        public static TermDefinition FromXElement(XElement el)
        {
            return new TermDefinition
            {
                Keywords = el.Elements(ns + "keyword").Select(e => e.Value).ToList(),
                Description = el.Elements(ns + "description").Select(e => FormattedText.FromXElement(e)).SingleOrDefault(),
                Name = el.Attributes("name").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Name == null ? null : new XAttribute("name", Name),
                Keywords.Select(k => new XElement(ns + "keyword", k)),
                Description == null ? null : Description.ToXElement(ns + "description")
            );
        }

        public LASD.Term ToLASD()
        {
            return new LASD.Term
            {
                Name = this.Name,
                Keywords = this.Keywords.ToArray(),
                Description = this.Description.ToXElement("desc", true).Elements().ToArray()
            };
        }

        public static TermDefinition FromLASD(LASD.Term term)
        {
            return new TermDefinition
            {
                Name = term.Name,
                Keywords = term.Keywords.ToList(),
                Description = FormattedText.FromXElement(new XElement("desc", term.Description))
            };
        }
    }
}
