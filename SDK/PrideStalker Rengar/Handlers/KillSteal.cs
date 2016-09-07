using LeagueSharp.SDK;
using PrideStalker_Rengar.Main;
using System.Linq;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Handlers
{
    internal class KillSteal : Core
    {
       
        public static void Killsteal()
        {
            if (Spells.E.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.E.Range) && x.Health < Spells.E.GetDamage(x)))
                {
                    Spells.E.CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            if (Spells.W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.W.Range) && x.Health < Spells.W.GetDamage(x)))
                {
                     Spells.W.Cast(target);   
                }
            }

            if (!MenuConfig.KillStealSummoner || !Spells.Ignite.IsReady()) return;

            foreach (var target in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(600f)).Where(target => target.Health < Dmg.IgniteDmg))
            {
                GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
            }
        }
    }
}
