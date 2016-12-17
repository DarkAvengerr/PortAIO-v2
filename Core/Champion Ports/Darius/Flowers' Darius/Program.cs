using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius
{
    using System;
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static void Main()
        {
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Darius")
            {
                return;
            }

            Chat.Print("<font color='#00a8ff'>Flowers' Darius Load! by NightMoon</font>");

            SpellManager.Init();
            MenuManager.Init();
            EventManager.Init();
        }
    }
}
