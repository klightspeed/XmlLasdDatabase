using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Security.Cryptography;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("keyLearningArea", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    [XmlRoot("kla", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class KeyLearningArea
    {
        protected static readonly XNamespace ns = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd";

        [XmlElement("group", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlArray("terms", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        [XmlElement("term", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<TermDefinition> Terms { get; set; }

        [XmlElement("achievementStandard", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public FormattedText AchievementStandard { get; set; }

        [XmlAttribute("yearLevel")]
        public string YearLevel { get; set; }

        [XmlAttribute("yearLevelId")]
        public string YearLevelID { get; set; }

        [XmlAttribute("subject")]
        public string Subject { get; set; }

        [XmlAttribute("subjectId")]
        public string SubjectID { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("sourceDocumentUrl")]
        public string SourceDocumentURL { get; set; }

        public string GetHash()
        {
            using (MD5 md5 = MD5.Create())
            {
                UTF8Encoding utf8 = new UTF8Encoding(false);
                md5.Initialize();
                XDocument xdoc = this.ToXDocument();

                using (MemoryStream memstrm = new MemoryStream())
                {
                    using (XmlWriter writer = XmlTextWriter.Create(memstrm, new XmlWriterSettings { CloseOutput = false, Indent = false, Encoding = utf8 }))
                    {
                        xdoc.Save(writer);
                    }

                    byte[] data = memstrm.ToArray();
                    string xml = utf8.GetString(data);

                    byte[] hash = md5.ComputeHash(data);
                    string hashstr = String.Join("", hash.Select(b => b.ToString("x2")));
                    return hashstr;
                }
            }
        }

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
                Version = el.Attributes("version").Select(a => a.Value).SingleOrDefault(),
                SourceDocumentURL = el.Attributes("sourceDocumentUrl").Select(a => a.Value).SingleOrDefault(),
                Groups = el.Elements(ns + "group").Select(e => AchievementRowGroup.FromXElement(e)).ToList(),
                Terms = el.Elements(ns + "terms").Select(e => e.Elements(ns + "term").Select(t => TermDefinition.FromXElement(t)).ToList()).SingleOrDefault(),
                AchievementStandard = el.Elements(ns + "achievementStandard").Select(s => FormattedText.FromXElement(s)).SingleOrDefault(),
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                YearLevel == null ? null : new XAttribute("yearLevel", YearLevel),
                YearLevelID == null ? null : new XAttribute("yearLevelId", YearLevelID),
                Subject == null ? null : new XAttribute("subject", Subject),
                SubjectID == null ? null : new XAttribute("subjectId", SubjectID),
                Version == null ? null : new XAttribute("version", Version),
                SourceDocumentURL == null ? null : new XAttribute("sourceDocumentUrl", SourceDocumentURL),
                AchievementStandard?.ToXElement(ns + "achievementStandard", true),
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

        public static KeyLearningArea FromLASDGroup(LASD.Group group, IEnumerable<LASD.Term> terms)
        {
            return new KeyLearningArea
            {
                Subject = group.KLA,
                YearLevel = group.YearLevel,
                Terms = terms.Select(t => TermDefinition.FromLASD(t)).ToList(),
                Groups = group.ChildGroups.Select(kvp => AchievementRowGroup.FromLASD(kvp.Key, kvp.Value)).ToList()
            };
        }

        public List<LASD.Term> GetLASDTerms()
        {
            return this.Terms.Select(t => t.ToLASD()).ToList();
        }
    }
}
