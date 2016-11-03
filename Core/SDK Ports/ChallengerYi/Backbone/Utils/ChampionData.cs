using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Data.DataTypes;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Utils
{
    internal class ChampionData
    {
        internal ChampionData(string name, List<SpellSlot> cc, List<SpellSlot> gapclosers)
        {
            ChampionName = name;
            Crowdcontrol = cc;
            Gapclosers = gapclosers;
        }
        internal string ChampionName;
        internal List<SpellSlot> Crowdcontrol;
        internal List<SpellSlot> Gapclosers;
    }
}
