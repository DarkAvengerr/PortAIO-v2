#region

using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using Swiftly_Teemo.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Handler
{
    internal class Killsteal : Core
    {
        public static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
            {
                if (!target.IsValidTarget()) continue;

                if (Spells.Q.IsReady() && target.Health < Spells.Q.GetDamage(target))
                {
                    Spells.Q.Cast(target);
                }
            }

            if (!MenuConfig.KillStealSummoner) return;

            foreach (var target in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(600f)))
            {
                if (target.Health < Dmg.IgniteDmg && Spells.Ignite.IsReady())
                {
                    GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }
        }
    }
}

