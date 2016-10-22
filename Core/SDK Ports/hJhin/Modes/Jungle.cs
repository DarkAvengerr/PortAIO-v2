using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Extensions;
using LeagueSharp;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hJhin.Modes
{
    class Jungle
    {
        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.Menu["jungle.settings"]["jungle.mana"])
            {
                return;
            }

            if (Spells.Q.IsReady() && Config.Menu["jungle.settings"]["jungle.q"])
            {
                foreach (var minion in GameObjects.JungleLarge.Where(x=> x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.CastOnUnit(minion);
                }
            }

            if (Spells.W.IsReady() && Config.Menu["jungle.settings"]["jungle.w"])
            {
                foreach (var minion in GameObjects.JungleLarge.Where(x => x.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(minion);
                }
            }
        }
    }
}
