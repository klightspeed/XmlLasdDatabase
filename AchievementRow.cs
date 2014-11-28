using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("achievementRow", Namespace="http://tempuri.org/XmlLasdDatabase.xsd")]
    public class AchievementRow
    {
        protected static readonly XNamespace ns = "http://tempuri.org/XmlLasdDatabase.xsd";

        [XmlElement("description", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public string Description { get; set; }

        [XmlElement("descriptor", Namespace = "http://tempuri.org/XmlLasdDatabase.xsd")]
        public List<FormattedText> Descriptors { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("sourceid")]
        public string SourceId { get; set; }

        public void FindTerms(Dictionary<string, string> terms)
        {
            foreach (FormattedText text in Descriptors)
            {
                text.FindTerms(terms);
            }
        }

        public static AchievementRow FromXElement(XElement el)
        {
            return new AchievementRow
            {
                Description = el.Elements(ns + "description").Select(e => e.Value).SingleOrDefault(),
                Descriptors = el.Elements(ns + "descriptor").Select(e => FormattedText.FromXElement(e)).ToList(),
                Id = el.Attributes("id").Select(a => a.Value).SingleOrDefault(),
                SourceId = el.Attributes("sourceid").Select(a => a.Value).SingleOrDefault()
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Id == null ? null : new XAttribute("id", Id),
                SourceId == null ? null : new XAttribute("sourceid", SourceId),
                Description == null ? null : new XElement(ns + "description", Description),
                Descriptors.Select(d => d.ToXElement(ns + "descriptor"))
            );
        }

        public LASD.Entry ToLASD(string yearlevel, string kla, LASD.Group parent, string[] ancestors)
        {
            return new LASD.Entry
            {
                YearLevel = yearlevel,
                KLA = kla,
                SourceEntryID = this.SourceId ?? this.Id,
                ParentGroup = parent,
                ContentDescriptor = this.Description,
                AchievementDescriptors = this.Descriptors.Select(d => d.ToLASD()).ToArray(),
                EntryID = this.Id,
                IsEnabled = true,
                Groups = ancestors
            };
        }

        public static AchievementRow FromLASD(LASD.Entry entry)
        {
            return new AchievementRow
            {
                Id = entry.EntryID,
                SourceId = entry.SourceEntryID,
                Description = entry.ContentDescriptor,
                Descriptors = entry.AchievementDescriptors.Select(d => FormattedText.FromLASD(d)).ToList()
            };
        }
    }
}
