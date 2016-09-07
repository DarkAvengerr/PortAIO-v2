#region

using System;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using Reforged_Riven.Extras;
using Reforged_Riven.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Update.Process
{
    internal class ModeHandler : Core
    {
      
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                if (args.Target is Obj_AI_Minion)
                {
                    // Minions
                    var minions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Player.AttackRange + 380));

                    foreach (var m in minions)
                    {
                        if (MenuConfig.LaneVisible
                            && m.CountEnemyHeroesInRange(1500) > 0 
                            || !Spells.Q.IsReady() 
                            || !MenuConfig.LaneQ
                            || m.IsUnderEnemyTurret()) continue;

                        AttackMove(m);
                        Logic.ForceItem();
                        Spells.Q.Cast(m);
                    }
                }

                // Turret
                var turret = GameObjects.EnemyTurrets.FirstOrDefault(x => x.IsValidTarget(360));

                if (turret != null && MenuConfig.LaneQ)
                {
                    if (Spells.Q.IsReady())
                    {
                        AttackMove(turret);
                        Logic.ForceCastQ(turret);
                    }
                }

                // Jungle
                var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Player.GetRealAutoAttackRange(Player) + 100));

                foreach (var m in mobs)
                {
                    if (m.Health < Player.GetAutoAttackDamage(m))
                    {
                        return;
                    }

                    if (Spells.Q.IsReady() && MenuConfig.JungleQ)
                    {
                        AttackMove(m);
                        Logic.ForceItem();
                        Spells.Q.Cast(m);
                    }

                    else if (!Spells.W.IsReady() || !MenuConfig.JungleW)
                    {
                        return;
                    }

                    Logic.ForceItem();
                    Spells.W.Cast(m);
                }
            }

            var heroes = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Player.AttackRange + 260));

            var targets = heroes as AIHeroClient[] ?? heroes.ToArray();

            if (Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                foreach (var target in targets)
                {
                    if (!Spells.Q.IsReady() || !Logic.InWRange(target)) continue;

                    AttackMove(target);
                    Logic.ForceItem();

                    if (MenuConfig.QChase
                        && !target.IsFacing(Player)
                        && target.IsMoving
                        && Player.IsFacing(target))
                    {
                        Logic.ForceCastQ(target);
                    }
                    else
                    {
                        Spells.Q.Cast(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode != OrbwalkingMode.Hybrid || Qstack < 2 || !Spells.Q.IsReady()) return;

            foreach (var target in targets)
            {
                AttackMove(target);
                Logic.ForceItem();
                Logic.ForceCastQ(target);
            }
        }
    }
}