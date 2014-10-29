using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("achievementRowGroup", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementRowGroup
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("group", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlElement("row", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRow> Rows { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        public static AchievementRowGroup FromXElement(XElement el)
        {
            return new AchievementRowGroup
            {
                Id = el.Attributes("id").Select(a => a.Value).SingleOrDefault(),
                Name = el.Attributes("name").Select(a => a.Value).SingleOrDefault(),
                Groups = el.Elements(ns + "group").Select(e => AchievementRowGroup.FromXElement(e)).ToList(),
                Rows = el.Elements(ns + "row").Select(e => AchievementRow.FromXElement(e)).ToList(),
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Id == null ? null : new XAttribute("id", Id),
                Name == null ? null : new XAttribute("name", Name),
                Groups.Select(g => g.ToXElement(ns + "group")),
                Rows.Select(r => r.ToXElement(ns + "row"))
            );
        }
    }
}
