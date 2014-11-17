using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TSVCEO.LASDDatabase
{
    public interface ILASDDatabase
    {
        IEnumerable<string> GetYearLevels();
        IEnumerable<string> GetKLAs(string YearLevel);
        IEnumerable<string> GetAchievementLevels(string YearLevel);
        IEnumerable<Term> GetLASDTerms(string YearLevel, string KLA);
        Group GetLASDTree(string YearLevel, string KLA);
        ILASDTable GetLASDTable(string YearLevel);
        void SaveLASDTree(string YearLevel, string KLA, Group rootgroup);
        int GetMaxGroupDepth();
    }
}
