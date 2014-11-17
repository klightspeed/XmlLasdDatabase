using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TSVCEO.LASDDatabase
{
    public class Entry
    {
        public string EntryID { get; set; }
        public string SourceEntryID { get; set; }
        public bool IsEnabled { get; set; }
        public string YearLevel { get; set; }
        public string KLA { get; set; }
        public Group ParentGroup { get; set; }
        public string[] Groups { get; set; }
        public string ContentDescriptor { get; set; }
        public AchievementDescriptor[] AchievementDescriptors { get; set; }

        public Entry()
        {
        }

        protected Entry(Entry entry, bool load)
        {
            this.AchievementDescriptors = new AchievementDescriptor[entry.AchievementDescriptors.Length];
            this.Groups = entry.Groups.Select(v => v).ToArray();
            this.ParentGroup = entry.ParentGroup;

            if (load)
            {
                Load(entry);
            }
        }

        public Entry(Entry entry)
            : this(entry, true)
        {
        }

        public virtual void Load(Entry entry)
        {
            if (entry.AchievementDescriptors.Length != this.AchievementDescriptors.Length)
            {
                throw new InvalidOperationException();
            }

            this.YearLevel = entry.YearLevel;
            this.KLA = entry.KLA;
            this.IsEnabled = entry.IsEnabled;
            this.ContentDescriptor = entry.ContentDescriptor;
            this.AchievementDescriptors = entry.AchievementDescriptors.Select(v => v).ToArray();
            this.SourceEntryID = entry.SourceEntryID;
            this.EntryID = entry.EntryID;
        }

        public virtual void Enable()
        {
            this.IsEnabled = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (!(obj is Entry))
            {
                return false;
            }
            else if (obj.GetType() != this.GetType() && this.GetType().IsAssignableFrom(obj.GetType()))
            {
                return obj.Equals(this);
            }
            else
            {
                Entry other = (Entry)obj;
                return this.YearLevel == other.YearLevel &&
                    this.SourceEntryID == other.SourceEntryID &&
                    this.KLA == other.KLA &&
                    this.IsEnabled == other.IsEnabled &&
                    this.ContentDescriptor == other.ContentDescriptor &&
                    this.AchievementDescriptors.Length == other.AchievementDescriptors.Length &&
                    this.AchievementDescriptors.Zip(other.AchievementDescriptors, (t, o) => t.Equals(o)).All(v => v);
            }
        }

        public override int GetHashCode()
        {
            return this.YearLevel.GetHashCode() ^
                this.SourceEntryID.GetHashCode() ^
                this.KLA.GetHashCode() ^
                this.IsEnabled.GetHashCode() ^
                this.Groups.Aggregate(0, (a, g) => a ^ g.GetHashCode()) ^
                this.ContentDescriptor.GetHashCode() ^
                this.AchievementDescriptors.Aggregate(0, (a, s) => a ^ s.GetHashCode());
        }
    }
}
