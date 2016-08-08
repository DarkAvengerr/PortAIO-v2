using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeShyvana
{
    using static Program;
    using static extension;
    class smite
    {
        public static void UpdateSmite()
        {
            if (AutoSmite && Smite.LSIsReady())
            {
                var creep = MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.Neutral).
                    Where(x => x.CharData.BaseSkinName == "SRU_Dragon" || x.CharData.BaseSkinName == "SRU_Baron");
                foreach (var x in creep.Where(y => Player.LSDistance(y.Position) <= Player.BoundingRadius + 500 + y.BoundingRadius))
                {
                    if (x != null && x.Health <= GetSmiteDamage())
                        Player.Spellbook.CastSpell(Smite, x);
                }
            }
        }
    }
}
