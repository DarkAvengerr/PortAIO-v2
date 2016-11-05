using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.External.Translation.Languages
{
    interface IVHRLanguage
    {
        string GetName();

        Dictionary<string, string> GetTranslations();
    }
}
