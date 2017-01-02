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
        private static readonly Dmg dmg = new Dmg();

        public static void OnEndScene(EventArgs args)
        {
        }
    }
}