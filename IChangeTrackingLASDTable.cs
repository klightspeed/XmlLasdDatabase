using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSVCEO.LASDDatabase;

namespace GTMJ_Creator.Backend
{
    public interface IChangeTrackingLASDTable : ILASDTable
    {
        ChangeSet GetLastChangeSet();
        IEnumerable<ChangeSet> GetAllChangeSets();
        bool RevertToChangeSet(string revertto);
    }
}
