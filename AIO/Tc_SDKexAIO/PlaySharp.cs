using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Config;

    public class PlaySharp
    {

        #region

        public static Menu Menu { get; set; }

        public static Menu ChampionMenu { get; set; }

        public static AIHeroClient Player { get; set; }

        public static List<AIHeroClient> Enemies = new List<AIHeroClient>();

        public static List<AIHeroClient> Allies = new List<AIHeroClient>();

        #endregion

        #region 支持英雄

        private static string[] SupList =
        {
            "Jinx", "Jhin", "Teemo", "Ezreal", "Diana", "Quinn", "LeeSin"
        };

        #endregion

        static void Main(string[] args)
        {
            Bootstrap.Init(args);
            Events.OnLoad += Events_OnLoad;
        }

        private static void Events_OnLoad(object sender, EventArgs args)
        {

            Player = GameObjects.Player;

            foreach (var enemy in GameObjects.EnemyHeroes) { Enemies.Add(enemy); }

            foreach (var ally in GameObjects.AllyHeroes) { Allies.Add(ally); }

            if (!SupList.Contains(GameObjects.Player.ChampionName))
            {
                Write(GameObjects.Player.ChampionName + "Not Support!");
                {
                    return;
                }
            }

            Write(GameObjects.Player.ChampionName + "Load OK!");

            Menu = new Menu("TcAioSDK", "Tc AIO SDKEx", true).Attach();
            Menu.GetSeparator("By: CjShu");
            Menu.GetSeparator("Credit: Sebby, NightMoon, xQx");
            Menu.Add(new MenuSeparator("Version", "版本號 : " + Assembly.GetExecutingAssembly().GetName().Version));
            
            Toolss.Tools.Init();

            ChampionMenu = Menu.Add(new Menu($"aio.{GameObjects.Player.ChampionName.ToLower()}", $"[TcSDKAio]: {GameObjects.Player.ChampionName}"));

            switch (Player.ChampionName)
            {
                case "Jinx":
                    Champions.Jinx.Init();
                    break;
                case "Jhin":
                    Champions.Jhin.Init();
                    break;
                case "Teemo":
                    Champions.Teemo.Init();
                    break;
                case "Ezreal":
                    Champions.Ezreal.Init();
                    break;
                case "Diana":
                    Champions.Diana.Init();
                    break;
                case "Quinn":
                    Champions.Quinn.Init();
                    break;
                case "LeeSin":
                    Champions.LeeSin.Init();
                    break;
            }
        }

        public static void Write(string mdg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Tc_SDKexAIO :" + mdg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}