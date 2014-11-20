using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("keyLearningArea", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    [XmlRoot("kla", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
    public class KeyLearningArea
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("group", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlArray("terms", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        [XmlElement("term", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<TermDefinition> Terms { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }

        [XmlAttribute("yearLevelId")]
        public string YearLevelID { get; set; }

        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("subjectId")]
        public string SubjectID { get; set; }

        public void FindTerms()
        {
            Dictionary<string, string> terms = Terms.SelectMany(t => t.Keywords.Select(k => new { keyword = k.ToLower(), name = t.Name })).ToDictionary(kn => kn.keyword, kn => kn.name);

            foreach (AchievementRowGroup group in Groups)
            {
                group.FindTerms(terms);
            }
        }

        public static KeyLearningArea FromXElement(XElement el)
        {
            return new KeyLearningArea
            {
                YearLevel = el.Attributes("yearLevel").Select(a => a.Value).SingleOrDefault(),
                YearLevelID = el.Attributes("yearLevelId").Select(a => a.Value).SingleOrDefault(),
                Subject = el.Attributes("subject").Select(a => a.Value).SingleOrDefault(),
                SubjectID = el.Attributes("subjectId").Select(a => a.Value).SingleOrDefault(),
                Groups = el.Elements(ns + "group").Select(e => AchievementRowGroup.FromXElement(e)).ToList(),
                Terms = el.Elements(ns + "terms").Select(e => e.Elements(ns + "term").Select(t => TermDefinition.FromXElement(t)).ToList()).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                YearLevel == null ? null : new XAttribute("yearLevel", YearLevel),
                YearLevelID == null ? null : new XAttribute("yearLevelId", YearLevelID),
                Subject == null ? null : new XAttribute("subject", Subject),
                SubjectID == null ? null : new XAttribute("subjectId", SubjectID),
                new XElement(ns + "terms", Terms.Select(t => t.ToXElement(ns + "term"))),
                Groups.Select(g => g.ToXElement(ns + "group"))
            );
        }

        public XDocument ToXDocument()
        {
            return new XDocument(ToXElement(ns + "kla"));
        }

        public LASD.Group ToLASDGroup()
        {
            LASD.Group group = new LASD.Group
            {
                YearLevel = this.YearLevel,
                KLA = this.Subject,
                Parent = null,
                GroupName = this.Subject,
                ChildEntries = new List<LASD.Entry>()
            };

            group.ChildGroups = this.Groups.ToDictionary(
                g => g.Id,
                g => g.ToLASD(this.YearLevel, this.Subject, group)
            );

            return group;
        }

        public List<LASD.Term> GetLASDTerms()
        {
            return this.Terms.Select(t => t.ToLASD()).ToList();
        }
    }
}
