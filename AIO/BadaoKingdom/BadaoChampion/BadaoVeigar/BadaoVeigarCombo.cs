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
 namespace BadaoKingdom.BadaoChampion.BadaoVeigar
{
    public static class BadaoVeigarCombo
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            // Q
            if (BadaoVeigarVariables.ComboQ.GetValue<bool>() && BadaoMainVariables.Q.IsReady())
            {
                var targetQ = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Magical);
                if (targetQ.IsValidTarget())
                {
                    BadaoVeigarHelper.CastQTarget(targetQ);
                }
            }
            // W
            if (BadaoVeigarVariables.ComboWAlways.GetValue<bool>() && BadaoMainVariables.W.IsReady())
            {
                var targetW = HeroManager.Enemies.Where(x => x.IsValidTarget(BadaoMainVariables.W.Range) &&
                    BadaoVeigarConfig.config.Item("WC" + x.NetworkId).GetValue<bool>()).OrderBy(x => x.Health).FirstOrDefault();
                if (targetW.IsValidTarget())
                    BadaoMainVariables.W.Cast(targetW);
            }
            if (BadaoVeigarVariables.ComboWOnCC.GetValue<bool>() && BadaoMainVariables.W.IsReady())
            {
                var targetW = HeroManager.Enemies.Where(x => x.IsValidTarget(BadaoMainVariables.W.Range) &&
                    BadaoVeigarConfig.config.Item("WC" + x.NetworkId).GetValue<bool>()).OrderBy(x => x.Health)
                    .FirstOrDefault(x => x.IsMovementImpaired());
                if (targetW.IsValidTarget())
                    BadaoMainVariables.W.Cast(targetW);
            }
            //E
            if (BadaoVeigarVariables.ComboE.GetValue<bool>() && BadaoMainVariables.E.IsReady())
            {
                var targetE = HeroManager.Enemies.Where(x => x.IsValidTarget(BadaoMainVariables.E.Range + 300) &&
                    BadaoVeigarConfig.config.Item("EC" + x.NetworkId).GetValue<bool>() 
                    && Prediction.GetPrediction(x,1.25f).UnitPosition.Distance(ObjectManager.Player.Position) <= BadaoMainVariables.E.Range + 300)
                    .OrderBy(x => x.Health).FirstOrDefault();
                if (targetE.IsValidTarget())
                {
                    BadaoVeigarHelper.CastETarget(targetE, BadaoVeigarVariables.ExtraEDistance.GetValue<Slider>().Value);
                }
            }
            //R
            if (BadaoVeigarVariables.ComboRAlways.GetValue<bool>() && BadaoMainVariables.R.IsReady())
            {
                var targetR = HeroManager.Enemies.Where(x => x.IsValidTarget(BadaoMainVariables.R.Range) &&
                    BadaoVeigarConfig.config.Item("RC" + x.NetworkId).GetValue<bool>()).OrderBy(x => x.Health).FirstOrDefault();
                if (targetR.IsValidTarget())
                    BadaoMainVariables.R.Cast(targetR);
            }
            if (BadaoVeigarVariables.ComboRKillable.GetValue<bool>() && BadaoMainVariables.R.IsReady())
            {
                var targetR = HeroManager.Enemies.Where(x => x.IsValidTarget(BadaoMainVariables.R.Range) &&
                    BadaoVeigarConfig.config.Item("RC" + x.NetworkId).GetValue<bool>()
                    && BadaoVeigarHelper.GetRDamage(x) >= x.Health)
                    .OrderBy(x => x.Health).FirstOrDefault();
                if (targetR.IsValidTarget())
                    BadaoMainVariables.R.Cast(targetR);
            }
        }
    }
}
