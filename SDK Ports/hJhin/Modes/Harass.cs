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
    class Harass
    {

        private static void ExecuteQ()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.CastOnUnit(enemy);
            }
        }

        private static void ExecuteW()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.W.Range)))
            {
                var pred = Spells.W.GetPrediction(enemy);
                if (pred.Hitchance >= Provider.HikiChance())
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }
        }

        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.Menu["harass.settings"]["harass.mana"])
            {
                return;
            }

            if (Spells.Q.IsReady() && Config.Menu["harass.settings"]["harass.q"])
            {
                ExecuteQ();
            }

            if (Spells.W.IsReady() && Config.Menu["harass.settings"]["harass.w"])
            {
                ExecuteW();
            }
        }

    }
}
