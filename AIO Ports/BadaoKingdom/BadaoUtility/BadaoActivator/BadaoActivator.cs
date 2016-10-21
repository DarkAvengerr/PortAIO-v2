using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoUtility.BadaoActivator
{
    public static class BadaoActivator
    {
        public static void BadaoActivate()
        {
            BadaoActivatorConfig.BadaoActivate();
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var targetA = TargetSelector.GetTarget(550, TargetSelector.DamageType.Physical);
                if (targetA.BadaoIsValidTarget())
                {
                    if (BadaoActivatorHelper.UseBotrk())
                    {
                        ItemData.Blade_of_the_Ruined_King.GetItem().Cast(targetA);
                        ItemData.Bilgewater_Cutlass.GetItem().Cast(targetA);
                    }
                    if (BadaoActivatorHelper.UseYoumuu())
                        ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }
            }
        }
    }
}
