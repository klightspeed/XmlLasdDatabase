using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("keyLearningAreaReference", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class KeyLearningAreaReference
    {
        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("filename")]
        public string Filename { get; set; }
    }
}
