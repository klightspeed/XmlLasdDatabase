using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSVCEO.LASDDatabase;

namespace TSVCEO.XmlLasdDatabase
{
    public class XmlLasdDatabase : ILASDDatabase
    {
        protected Dictionary<string, Grade> Grades { get; set; }
        protected Dictionary<string, Dictionary<string, KeyLearningArea>> KLAs { get; set; }
        protected int MaxGroupDepth { get; set; }

        public XmlLasdDatabase(IEnumerable<Grade> grades, IEnumerable<KeyLearningArea> klas)
        {
            Grades = grades.ToDictionary(g => g.YearLevel, g => g);
            KLAs = Grades.ToDictionary(g => g.Key, g => new Dictionary<string, KeyLearningArea>());

            foreach (KeyLearningArea kla in klas)
            {
                if (!KLAs.ContainsKey(kla.YearLevel))
                {
                    KLAs[kla.YearLevel] = new Dictionary<string, KeyLearningArea>();
                }

                KLAs[kla.YearLevel][kla.Subject] = kla;
            }

            MaxGroupDepth = 0;
            AchievementRowGroup[] groups = KLAs.Values.SelectMany(g => g.Values).SelectMany(k => k.Groups).ToArray();

            while (groups.Length != 0)
            {
                MaxGroupDepth++;

                groups = groups.SelectMany(g => g.Groups).ToArray();
            }
        }

        public IEnumerable<string> GetYearLevels()
        {
            return Grades.Keys;
        }

        public IEnumerable<string> GetKLAs(string YearLevel)
        {
            return Grades[YearLevel].KLAs.Select(kla => kla.Subject);
        }

        public IEnumerable<string> GetAchievementLevels(string YearLevel)
        {
            return Grades[YearLevel].Levels.Select(level => level.Name);
        }

        public IEnumerable<Term> GetLASDTerms(string YearLevel, string KLA)
        {
            return KLAs[YearLevel][KLA].GetLASDTerms();
        }

        public Group GetLASDTree(string YearLevel, string KLA)
        {
            return KLAs[YearLevel][KLA].ToLASDGroup();
        }

        public int GetMaxGroupDepth()
        {
            return MaxGroupDepth;
        }
    }
}
