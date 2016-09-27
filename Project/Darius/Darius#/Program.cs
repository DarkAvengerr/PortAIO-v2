using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DariusSharp
{
    internal class Program
    {
        //Player
        private static AIHeroClient Player = ObjectManager.Player;
        private const string ChampionName = "Darius";

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            //Return if Player is not playing Darius..
            if (Player.ChampionName != ChampionName)
                return;

            //Initizalize 
            SpellHandler.Initialize();
            ConfigHandler.Initialize();

            //Subscribe to events
            Orbwalking.AfterAttack += ComboHandler.ExecuteAfterAttack;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Combo
            if (ConfigHandler.KeyLinks["comboActive"].Value.Active)
                ComboHandler.ExecuteCombo();

            //Harass
            if (ConfigHandler.KeyLinks["harassActive"].Value.Active)
                ComboHandler.ExecuteHarass();

            //Additionals (Killsteal)
            ComboHandler.ExecuteAdditionals();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            // Draw our circles
            foreach (var circle in ConfigHandler.CircleLinks.Values.Select(link => link.Value))
            {
                if (circle.Active)
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, circle.Radius, circle.Color);
            }
        }
    }
}
