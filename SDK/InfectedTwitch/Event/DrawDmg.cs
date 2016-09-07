using System;
using System.Linq;
using Infected_Twitch.Menus;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Event
{
    using Core;

    internal class DrawDmg : Core
    {
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private static readonly Dmg dmg = new Dmg();

        public static void OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1350)))
            {
                if (!MenuConfig.DrawDmg) continue;

                Indicator.Unit = enemy;
                Indicator.DrawDmg(dmg.EDamage(enemy), Color.LawnGreen);
            }
        }
    }
}