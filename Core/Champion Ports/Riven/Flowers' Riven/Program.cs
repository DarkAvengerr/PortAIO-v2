using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;

    internal class Program
    {
        public static void Main()
        {
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }

            Chat.Print("<font color='#00a8ff'>Flowers' Riven Reborn Load! by NightMoon</font>");

            SpellManager.Init();
            MenuManager.Init();
            EventManager.Init();
        }
    }
}
