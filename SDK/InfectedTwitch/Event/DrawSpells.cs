#region

using System;
using System.Drawing;
using Infected_Twitch.Menus;
using LeagueSharp;
using LeagueSharp.SDK.Utils;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Event
{
    using Core;
    internal class DrawSpells : Core
    {
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead || !HasPassive || !MenuConfig.DrawTimer) return;

            var passiveTime = Math.Max(0, Player.GetBuff("TwitchHideInShadows").EndTime) - Game.Time;

            Render.Circle.DrawCircle(Player.Position, passiveTime * Player.MoveSpeed, Color.Gray);
        }
    }
}
