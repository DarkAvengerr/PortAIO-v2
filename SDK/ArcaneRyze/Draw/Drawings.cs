#region

using System;
using LeagueSharp;
using static Arcane_Ryze.Core;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Draw
{
    internal class Drawings
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static void OnDraw(EventArgs args)
        {
            if(Player.IsDead)
            {
                return;
            }
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            // Can't do much with this rn because L# fucked in the head
        }
    }
}
