using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GTMJ_Creator.XmlLasdDatabase
{
    public class AchievementDescriptor : FormattedText
    {
        public LASDEntry.AchievementDescriptor ToAchievementDescriptor()
        {
            return new LASDEntry.AchievementDescriptor
            {
                Xml = new XElement("desc",
                    Elements.Select(el =>
                    {
                        if (el.Name.LocalName == "p")
                        {
                            return new XElement("p", GetTextRun(el));
                        }
                        else if (el.Name.LocalName == "ul")
                        {
                            return new XElement("ul",
                                el.Elements(ns + "li").Select(e => new XElement("li", GetTextRun(e)))
                            );
                        }
                        else
                        {
                            return null;
                        }
                    })
                )
            };
        }
    }
}
