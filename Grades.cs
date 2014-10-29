using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("gradeList", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("grades", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class GradeList
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("grade", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<Grade> Grades { get; set; }

        public static GradeList FromXElement(XElement el)
        {
            return new GradeList
            {
                Grades = el.Elements(ns + "grade").Select(e => Grade.FromXElement(e)).ToList()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name, Grades.Select(g => g.ToXElement(ns + "grade")));
        }
    }
}
