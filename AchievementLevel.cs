using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("achievementLevel", Namespace="http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class AchievementLevel
    {
        [XmlAttribute("abbreviation")]
        public string Abbreviation { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public static AchievementLevel FromXElement(XElement el)
        {
            return new AchievementLevel
            {
                Abbreviation = el.Attributes("abbreviation").Select(a => a.Value).SingleOrDefault(),
                Name = el.Attributes("name").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Abbreviation == null ? null : new XAttribute("abbreviation", Abbreviation),
                Name == null ? null : new XAttribute("name", Name)
            );
        }
    }
}
