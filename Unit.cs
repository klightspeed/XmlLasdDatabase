using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("unit", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("unit", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class Unit
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("name", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string UnitName { get; set; }

        [XmlElement("focus", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string UnitFocus { get; set; }

        [XmlElement("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<KeyLearningArea> KeyLearningAreas { get; set; }

        public static Unit FromXElement(XElement el)
        {
            return new Unit
            {
                UnitName = el.Elements(ns + "name").Select(e => e.Value).SingleOrDefault(),
                UnitFocus = el.Elements(ns + "focus").Select(e => e.Value).SingleOrDefault(),
                KeyLearningAreas = el.Elements(ns + "kla").Select(e => KeyLearningArea.FromXElement(e)).ToList()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                new XElement(ns + "name", UnitName),
                new XElement(ns + "focus", UnitFocus),
                KeyLearningAreas.Select(kla => kla.ToXElement(ns + "kla"))
            );
        }

        public XDocument ToXDocument()
        {
            return new XDocument(this.ToXElement(ns + "unit"));
        }

        public static Unit FromLASDTable(LASD.ILASDTable table)
        {
            return new Unit
            {
                UnitName = table.UnitName,
                UnitFocus = table.UnitFocus,
                KeyLearningAreas = table.GetKLAs().Select(kla => KeyLearningArea.FromLASDGroup(table.GetKLAGroup(kla), table.GetTerms(kla))).ToList()
            };
        }
    }
}
