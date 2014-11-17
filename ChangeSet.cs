using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TSVCEO.LASDDatabase
{
    public class InsertedEntry : Entry
    {
        public string InsertBefore { get; set; }

        public InsertedEntry(Entry entry, string insertbefore)
            : base(entry)
        {
            this.InsertBefore = insertbefore;
        }

        public InsertedEntry(Entry entry)
            : base(entry)
        {
        }

        public InsertedEntry()
        {
        }
    }

    public class EntryModification
    {
        public string EntryID { get; set; }
        public bool MoveEntry { get; set; }
        public InsertedEntry Before { get; set; }
        public InsertedEntry After { get; set; }
    }

    public class ChangeSet
    {
        public string StackTrace { get; set; }
        public string ChangeSetID { get; set; }
        public ChangeSet RevertFrom { get; set; }
        public ChangeSet ParentChangeSet { get; set; }
        public string OldAssessmentName { get; set; }
        public string NewAssessmentName { get; set; }
        public string OldAssessmentPurpose { get; set; }
        public string NewAssessmentPurpose { get; set; }
        public List<EntryModification> Modifications { get; set; }

        public ChangeSet()
        {
            Modifications = new List<EntryModification>();
            this.ChangeSetID = Guid.NewGuid().ToString();
            this.StackTrace = new StackTrace().ToString();
        }
    }
}
