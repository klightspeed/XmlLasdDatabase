using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    [XmlType("gradeList", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("grades", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class GradeList
    {
        [XmlElement("grade", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<Grade> Grades { get; set; }
    }
}
