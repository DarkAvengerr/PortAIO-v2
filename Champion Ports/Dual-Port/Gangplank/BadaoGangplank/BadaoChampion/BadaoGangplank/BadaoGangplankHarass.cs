using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoGP;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankHarass
    {
        public static void BadaoActivate ()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;
            if (BadaoMainVariables.Q.IsReady() && BadaoGangplankVariables.HarassQ.GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
        }
    }
}
