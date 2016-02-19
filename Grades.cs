using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("gradeList", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    [XmlRoot("grades", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class GradeList
    {
        protected static readonly XNamespace ns = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd";

        [XmlElement("grade", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
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

        public XDocument ToXDocument()
        {
            return new XDocument(ToXElement(ns + "grades"));
        }
    }
}
