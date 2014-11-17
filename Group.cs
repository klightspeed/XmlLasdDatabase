using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSVCEO.LASDDatabase
{
    public class Group
    {
        public string YearLevel { get; set; }
        public string KLA { get; set; }
        public Group Parent { get; set; }
        public string GroupName { get; set; }
        public Dictionary<string, Group> ChildGroups { get; set; }
        public List<Entry> ChildEntries { get; set; }

        public Group()
        {
            this.ChildEntries = new List<Entry>();
            this.ChildGroups = new Dictionary<string, Group>();
        }

        public Group(Group grp, Group parent)
        {
            YearLevel = grp.YearLevel;
            KLA = grp.KLA;
            GroupName = grp.GroupName;
            ChildGroups = grp.ChildGroups.ToDictionary(kvp => kvp.Key, kvp => new Group(kvp.Value, this));
            ChildEntries = grp.ChildEntries.Select(e => new Entry(e)).ToList();
        }

        public IEnumerable<Entry> GetRows()
        {
            foreach (Group grp in ChildGroups.Values)
            {
                foreach (Entry entry in grp.GetRows())
                {
                    if (entry.IsEnabled)
                    {
                        yield return new Entry
                        {
                            AchievementDescriptors = entry.AchievementDescriptors.Select(v => v).ToArray(),
                            ContentDescriptor = entry.ContentDescriptor,
                            Groups = new string[] { GroupName }.Concat(entry.Groups).ToArray(),
                            IsEnabled = true,
                            KLA = entry.KLA,
                            ParentGroup = entry.ParentGroup,
                            SourceEntryID = entry.SourceEntryID,
                            YearLevel = entry.YearLevel
                        };
                    }
                }
            }

            foreach (Entry entry in ChildEntries)
            {
                if (entry.IsEnabled)
                {
                    yield return new Entry
                    {
                        AchievementDescriptors = entry.AchievementDescriptors.Select(v => v).ToArray(),
                        ContentDescriptor = entry.ContentDescriptor,
                        Groups = new string[] { GroupName },
                        IsEnabled = true,
                        KLA = entry.KLA,
                        ParentGroup = entry.ParentGroup,
                        SourceEntryID = entry.SourceEntryID,
                        YearLevel = entry.YearLevel
                    };
                }
            }
        }
    }
}
