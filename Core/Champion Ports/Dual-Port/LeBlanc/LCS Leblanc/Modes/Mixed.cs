using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCS_LeBlanc.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Modes
{
    internal static class Mixed
    {
        public static void Init()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range - 50)))
                {
                    Spells.Q.CastOnUnit(enemy);
                }
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.harass") && !Spells.Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.W.Range)))
                {
                    var hit = Spells.W.GetPrediction(enemy);
                    if (hit.Hitchance >= HitChance.Medium)
                    {
                        Spells.W.Cast(hit.CastPosition);
                    }
                }
            }
        }
    }
}
