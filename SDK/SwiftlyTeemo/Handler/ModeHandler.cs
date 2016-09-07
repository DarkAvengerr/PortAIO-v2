using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using Swiftly_Teemo.Menu;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Handler
{
    class ModeHandler : Core
    {
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (Orbwalker.ActiveMode == OrbwalkingMode.Combo || Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
            {
                var a = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.Q.Range));
                var targets = a as AIHeroClient[] ?? a.ToArray();

                foreach (var target in targets)
                {
                    Spells.Q.Cast(target);
                }
            }

            if (Orbwalker.ActiveMode != OrbwalkingMode.LaneClear || !(args.Target is Obj_AI_Minion)) return;
            {
                var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Spells.Q.Range));

                foreach (var m in mobs)
                {
                    if(!m.IsValid) return;

                    Spells.Q.Cast(m);
                }

                if (!MenuConfig.LaneQ) return;
                var minions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Spells.Q.Range));

                foreach (var m in minions)
                {
                    if(m.Health > Spells.Q.GetDamage(m)) return;

                    Spells.Q.Cast(m);
                }
            }
        }
    }
}
