using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LASD = TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    [XmlType("achievementRowGroup", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
    public class AchievementRowGroup
    {
        protected static readonly XNamespace ns = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd";

        [XmlElement("group", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<AchievementRowGroup> Groups { get; set; }

        [XmlElement("row", Namespace = "http://gtmj.tsv.catholic.edu.au/XmlLasdDatabase.xsd")]
        public List<AchievementRow> Rows { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        public void FindTerms(Dictionary<string, string> terms)
        {
            foreach (AchievementRowGroup grp in Groups)
            {
                grp.FindTerms(terms);
            }

            foreach (AchievementRow row in Rows)
            {
                row.FindTerms(terms);
            }
        }

        public static AchievementRowGroup FromXElement(XElement el)
        {
            return new AchievementRowGroup
            {
                Id = el.Attributes("id").Select(a => a.Value).SingleOrDefault(),
                Name = el.Attributes("name").Select(a => a.Value).SingleOrDefault(),
                Groups = el.Elements(ns + "group").Select(e => AchievementRowGroup.FromXElement(e)).ToList(),
                Rows = el.Elements(ns + "row").Select(e => AchievementRow.FromXElement(e)).ToList(),
            };
        }

        public XElement ToXElement(XName name)
        {
            return new XElement(name,
                Id == null ? null : new XAttribute("id", Id),
                Name == null ? null : new XAttribute("name", Name),
                Groups.Select(g => g.ToXElement(ns + "group")),
                Rows.Select(r => r.ToXElement(ns + "row"))
            );
        }

        public LASD.Group ToLASD(string yearlevel, string kla, LASD.Group parent)
        {
            LASD.Group group = new LASD.Group
            {
                KLA = kla,
                YearLevel = yearlevel,
                Parent = parent,
                GroupName = this.Name
            };

            group.ChildGroups = this.Groups.ToDictionary(g => g.Id, g => g.ToLASD(yearlevel, kla, group));
            group.ChildEntries = this.Rows.Select(e => e.ToLASD(yearlevel, kla, group)).ToList();

            return group;
        }

        public static AchievementRowGroup FromLASD(string id, LASD.Group group)
        {
            return new AchievementRowGroup
            {
                Id = id,
                Name = group.GroupName,
                Groups = group.ChildGroups.Select(kvp => AchievementRowGroup.FromLASD(kvp.Key, kvp.Value)).ToList(),
                Rows = group.ChildEntries.Select(e => AchievementRow.FromLASD(e)).ToList()
            };
        }
    }
}
