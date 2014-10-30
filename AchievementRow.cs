using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("achievementRow", Namespace="http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementRow
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("description", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string Description { get; set; }

        [XmlElement("descriptor", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<FormattedText> Descriptors { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        public void FindTerms(Dictionary<string, string> terms)
        {
            foreach (FormattedText text in Descriptors)
            {
                text.FindTerms(terms);
            }
        }

        public static AchievementRow FromXElement(XElement el)
        {
            return new AchievementRow
            {
                Description = el.Elements(ns + "description").Select(e => e.Value).SingleOrDefault(),
                Descriptors = el.Elements(ns + "descriptor").Select(e => FormattedText.FromXElement(e)).ToList(),
                Id = el.Attributes("id").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Id == null ? null : new XAttribute("id", Id),
                Description == null ? null : new XElement(ns + "description", Description),
                Descriptors.Select(d => d.ToXElement(ns + "descriptor"))
            );
        }
    }
}
