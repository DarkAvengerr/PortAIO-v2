using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoMainVariables;
    using static BadaoKatarinaVariables;
    using static BadaoKatarinaHelper;
    public static class BadaoKatarinaAuto
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (AutoKs.GetValue<bool>() && !(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !ComboCancelRForKS.GetValue<bool>() && Player.IsChannelingImportantSpell()))
            {
                var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (targetQ.IsValidTarget() && Q.IsReady() && GetQDamage(targetQ) >= targetQ.Health)
                {
                    Q.Cast(targetQ);
                }

                var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (targetE.IsValidTarget() && E.IsReady() && GetEDamage(targetE) >= targetE.Health)
                {
                    E.Cast(targetE);
                }

                var targetEQ = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (targetEQ.IsValidTarget() && E.IsReady() && Q.IsReady() && GetEDamage(targetEQ) + GetQDamage(targetEQ) >= targetEQ.Health)
                {
                    E.Cast(targetEQ);
                }

                if (Q.IsReady() && E.IsReady())
                {
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range + Q.Range + 150) && GetQDamage(x) >= x.Health))
                    {
                        var nearest = GetEVinasun().MinOrDefault(x => x.Position.Distance(hero.Position));
                        if (nearest != null && nearest.Position.Distance(hero.Position) <= 150 + Q.Range)
                        {
                            var pos = nearest.Position.To2D().Extend(hero.Position.To2D(), 150);
                            E.Cast(pos);
                        }
                    }
                }
            }
        }
    }
}
