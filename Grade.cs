using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("grade", Namespace="http://tempuri.org/XmlLasdDatabase.xsd")]
    public class Grade
    {
        [XmlElement("level", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementLevel> Levels { get; set; }

        [XmlElement("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<KeyLearningAreaReference> KLAs { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }
    }
}
