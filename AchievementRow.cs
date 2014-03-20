using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("achievementRow", Namespace="http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementRow
    {
        [XmlElement("description", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string Description { get; set; }

        [XmlElement("descriptor", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<string> Descriptors { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
