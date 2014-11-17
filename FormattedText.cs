﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("formattedText", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class FormattedText
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlAnyElement()]
        public XElement[] Elements { get; set; }

        public void FindTerms(Dictionary<string, string> terms)
        {
            Elements = AchievementDescriptor.FindTerms(ns, Elements, terms);
        }

        public static FormattedText FromXElement(XElement el)
        {
            return new FormattedText
            {
                Elements = el.Elements().ToArray()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name, Elements);
        }
    }
}
