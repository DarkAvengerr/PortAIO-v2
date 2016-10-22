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
    class Combo
    {
        private static readonly int MinRange = Config.Menu["combo.settings"]["combo.w.min"];
        private static readonly int MaxRange = Config.Menu["combo.settings"]["combo.w.max"];

        public static void ExecuteQ()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x=> x.IsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.Cast(enemy);
            }
        }

        public static void ExecuteW()
        {
            if (Config.Menu["combo.settings"]["combo.w.mark"])
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.W.Range) &&
                    (x.IsStunnable() || x.IsEnemyImmobile())))
                {
                    Spells.W.Cast(enemy);
                }
            }

            else
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValid && x.Distance(ObjectManager.Player) < MaxRange
                    && x.Distance(ObjectManager.Player) > MinRange && Spells.W.GetPrediction(x).Hitchance
                    >= Provider.HikiChance() && x.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }
           
        public static void ExecuteE()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.E.Range) && x.IsEnemyImmobile()))
            {
                var pred = Spells.E.GetPrediction(enemy);
                if (pred.Hitchance >= Provider.HikiChance())
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }
        }

        public static void Execute()
        {
            if (Spells.Q.IsReady() && Config.Menu["combo.settings"]["combo.q"])
            {
                ExecuteQ();
            }
            if (Spells.W.IsReady() && Config.Menu["combo.settings"]["combo.w"])
            {
                ExecuteW();
            }
            if (Spells.E.IsReady() && Config.Menu["combo.settings"]["combo.e"])
            {
                ExecuteE();
            }
        }
    }
}
