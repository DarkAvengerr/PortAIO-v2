using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    internal class DrawDmg
    {
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        public static void DmgDraw(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                if (!MenuConfig.Dind) continue;

                Indicator.Unit = enemy;

                Indicator.DrawDmg(Dmg.GetComboDamage(enemy), enemy.Health <= Dmg.GetComboDamage(enemy)*1.65 ? Color.LawnGreen : Color.Yellow);
            }
        }
    }
}
