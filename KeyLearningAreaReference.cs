﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("keyLearningAreaReference", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class KeyLearningAreaReference
    {
        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("id")]
        public string SubjectID { get; set; }

        public static KeyLearningAreaReference FromXElement(XElement el)
        {
            return new KeyLearningAreaReference
            {
                Subject = el.Attributes("subject").Select(a => a.Value).SingleOrDefault(),
                SubjectID = el.Attributes("id").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Subject == null ? null : new XAttribute("subject", Subject),
                SubjectID == null ? null : new XAttribute("id", SubjectID)
            );
        }
    }
}