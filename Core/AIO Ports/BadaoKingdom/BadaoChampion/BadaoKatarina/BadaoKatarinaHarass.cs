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
    public static class BadaoKatarinaHarass
    {
        public static void BadaoAcitvate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed || Player.IsChannelingImportantSpell())
                return;
            // Q
            if (Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget())
                {
                    Q.Cast(target);
                }
            }
            if (W.IsReady() && E.IsReady() && HarassWE.GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget())
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && HarassWE.GetValue<bool>())
            {
                if (WDaggers.Any(x => Player.Distance(x.Dagger.Position) <= 150 && Environment.TickCount - Game.Ping - x.CreationTime >= 1150))
                {
                    foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                    {
                        var vinasun = GetEVinasun().Where(x => x.Position.Distance(target.Position) <= 450).MinOrDefault(x => x.Position.Distance(target.Position));
                        if (vinasun != null)
                        {
                            E.Cast(vinasun.Position.To2D().Extend(target.Position.To2D(),150));
                        }
                    }

                }
                var EdaggerOthers = HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range + 200) && IsDaggerFixed(x));
                if (EdaggerOthers.Any())
                {
                    var target = EdaggerOthers.MinOrDefault(x => x.Health);
                    var targetdagger = GetFixedDagger(target);
                    if (targetdagger.Dagger != null)
                    {
                        CastEFixedDagger(targetdagger, target);
                    }
                }
            }
        }
    }
}
