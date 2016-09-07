using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace _SDK_TroopAIO
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Program
    {
        internal static Menu Menu;
        internal static Version Version;
        internal static AIHeroClient Me;
        internal static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();
        internal static bool EnableOrbwalker;

        public static void Main()
        {
            LeagueSharp.SDK.Bootstrap.Init();
            Events_OnLoad();
        }

        private static void Events_OnLoad()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            Me = GameObjects.Player;

            Menu = new Menu("TroopAIO - A Revolution of SDK Assemblys", "TroopAIO - A Revolution of SDK Assemblys", true).Attach();
            Menu.Add(new MenuSeparator("Version", "Version : " + Version));
            Menu.Add(new MenuSeparator("Updated for every Patch!", "Updated for every Patch!"));
            var Cred = Menu.Add(new Menu("Credits", "Credits"));
            {
                Cred.Add(new MenuSeparator("Everything has been done by trooperhdx with some help of StopMotionCuber!", "Everything has been done by trooperhdx with some help of StopMotionCuber!"));
            }
            var Sup = Menu.Add(new Menu("Supported Champions", "Supported Champions"));
            {
                Sup.Add(new MenuSeparator("Urgot", "Urgot"));
                Sup.Add(new MenuSeparator("Evelynn", "Evelynn"));
                Sup.Add(new MenuSeparator("Vladimir", "Vladimir"));
                Sup.Add(new MenuSeparator("Akali", "Akali"));
            }

            switch (Me.ChampionName)
            {
                case "Urgot":
                    new Plugins.Urgot();
                    EnableOrbwalker = true;
                    break;

                case "Vladimir":
                    new Plugins.Vladimir();
                    EnableOrbwalker = true;
                    break;

                case "Evelynn":
                    new Plugins.Evelynn();
                    EnableOrbwalker = true;
                    break;

                case "Akali":
                    new Plugins.Akali();
                    EnableOrbwalker = true;
                    break;

                default:
                    Menu.Add(new MenuSeparator("NotSupported", Me.ChampionName + " Isn't supported yet, feel free to suggest any Champion!"));
                    EnableOrbwalker = false;
                    break;
            }

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                Enemies.Add(enemy);
            }

            foreach (var ally in GameObjects.AllyHeroes)
            {
                Allies.Add(ally);
            }

            if (EnableOrbwalker == true)
            {
                Variables.Orbwalker.Enabled = true;
            }
            else if (EnableOrbwalker == false)
            {
                Variables.Orbwalker.Enabled = false;
            }
        }
    }
}