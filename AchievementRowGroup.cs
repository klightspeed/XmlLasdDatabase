using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("achievementRowGroup", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementRowGroup
    {
        [XmlElement("group", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlElement("row", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRow> Rows { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
