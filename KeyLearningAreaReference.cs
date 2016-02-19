using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("keyLearningAreaReference", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class KeyLearningAreaReference
    {
        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("id")]
        public string SubjectID { get; set; }

        [XmlAttribute("filename")]
        public string Filename { get; set; }

        [XmlAttribute("sourceurl")]
        public string SourceURL { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("hash")]
        public string Hash { get; set; }

        public static KeyLearningAreaReference FromXElement(XElement el)
        {
            return new KeyLearningAreaReference
            {
                Subject = el.Attributes("subject").Select(a => a.Value).SingleOrDefault(),
                SubjectID = el.Attributes("id").Select(a => a.Value).SingleOrDefault(),
                Filename = el.Attributes("filename").Select(a => a.Value).SingleOrDefault(),
                SourceURL = el.Attributes("sourceurl").Select(a => a.Value).SingleOrDefault(),
                Version = el.Attributes("version").Select(a => a.Value).SingleOrDefault(),
                Hash = el.Attributes("hash").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Subject == null ? null : new XAttribute("subject", Subject),
                SubjectID == null ? null : new XAttribute("id", SubjectID),
                Filename == null ? null : new XAttribute("filename", Filename),
                SourceURL == null ? null : new XAttribute("sourceurl", SourceURL),
                Version == null ? null : new XAttribute("version", Version),
                Hash == null ? null : new XAttribute("hash", Hash)
            );
        }
    }
}
