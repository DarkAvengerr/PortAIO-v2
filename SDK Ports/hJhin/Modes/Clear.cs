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
    class Clear
    {
        private static void ExecuteQ()
        {
            var minion = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Spells.Q.Range)).MinOrDefault(x=> x.Health);
            if (minion != null)
            {
                Spells.Q.CastOnUnit(minion);
            }
        }

        public static void ExecuteW()
        {
            var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Spells.W.Range)).ToList();

            var minionhit = Spells.W.GetLineFarmLocation(minions).MinionsHit;
            if (minionhit >= Config.Menu["laneclear.settings"]["lane.w.min.count"])
            {
                Spells.W.Cast(Spells.W.GetLineFarmLocation(minions).Position);
            }

            
        }

        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.Menu["laneclear.settings"]["lane.mana"])
            {
                return;
            }

            if (Spells.Q.IsReady() && Config.Menu["laneclear.settings"]["lane.q"])
            {
                ExecuteQ();
            }

            if (Spells.W.IsReady() && Config.Menu["laneclear.settings"]["lane.w"])
            {
                ExecuteW();
            }
        }
    }
}
