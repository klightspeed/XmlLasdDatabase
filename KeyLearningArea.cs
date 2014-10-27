using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("keyLearningArea", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class KeyLearningArea
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("group", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }

        [XmlAttribute("yearLevelId")]
        public string YearLevelID { get; set; }

        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("subjectId")]
        public string SubjectID { get; set; }

        public static KeyLearningArea FromXElement(XElement el)
        {
            return new KeyLearningArea
            {
                YearLevel = el.Attributes("yearLevel").Select(a => a.Value).SingleOrDefault(),
                YearLevelID = el.Attributes("yearLevelId").Select(a => a.Value).SingleOrDefault(),
                Subject = el.Attributes("subject").Select(a => a.Value).SingleOrDefault(),
                SubjectID = el.Attributes("subjectId").Select(a => a.Value).SingleOrDefault(),
                Groups = el.Elements(ns + "group").Select(e => AchievementRowGroup.FromXElement(e)).ToList()
            };
        }
    }
}
