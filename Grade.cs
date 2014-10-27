using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("grade", Namespace="http://tempuri.org/XmlLasdDatabase.xsd")]
    public class Grade
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("level", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementLevel> Levels { get; set; }

        [XmlElement("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
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
    }
}
