using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using BadaoShen;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenAuto
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (AIHeroClient hero in HeroManager.Allies.Where(x => !x.IsMe && x.BadaoIsValidTarget(float.MaxValue, false)))
            {
                if (BadaoShenHelper.UseRAuto(hero))
                {
                    BadaoMainVariables.R.Cast(hero);
                }
            }
        }
    }
}
