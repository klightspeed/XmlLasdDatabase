using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("keyLearningArea", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class KeyLearningArea
    {
        [XmlElement("group", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }
    }
}
