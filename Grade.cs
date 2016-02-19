using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("grade", Namespace="http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class Grade
    {
        protected static readonly XNamespace ns = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd";

        [XmlElement("level", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<AchievementLevel> Levels { get; set; }

        [XmlElement("kla", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<KeyLearningAreaReference> KLAs { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }

        [XmlAttribute("id")]
        public string YearLevelID { get; set; }

        public static Grade FromXElement(XElement el)
        {
            return new Grade
            {
                YearLevel = el.Attributes("yearLevel").Select(a => a.Value).SingleOrDefault(),
                YearLevelID = el.Attributes("id").Select(a => a.Value).SingleOrDefault(),
                Levels = el.Elements(ns + "level").Select(e => AchievementLevel.FromXElement(e)).ToList(),
                KLAs = el.Elements(ns + "kla").Select(e => KeyLearningAreaReference.FromXElement(e)).ToList()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                YearLevel == null ? null : new XAttribute("yearLevel", YearLevel),
                YearLevelID == null ? null : new XAttribute("id", YearLevelID),
                Levels.Select(l => l.ToXElement(ns + "level")),
                KLAs.Select(k => k.ToXElement(ns + "kla"))
            );
        }
    }
}
