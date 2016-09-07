using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using Plugings;
    using System;
    using System.Linq;
    using System.Reflection;

    public class Program
    {
        public static Menu Menu { get; set; }
        public static AIHeroClient Me { get; set; }

        private static string[] SupportList = 
        {
            "Ahri", "Akali", "Ashe", "Blitzcrank", "Darius", "Ezreal", "Graves",
            "Hecarim", "Illaoi", "Karma", "Morgana", "Riven", "Ryze", "Sivir", "Tristana",
            "TwistedFate", "Twitch", "Vayne", "Viktor", "Vladimir"
        };

        public static void Main()
        {
            Bootstrap.Init();
            Events_OnLoad();
        }

        private static void Events_OnLoad()
        {
            if (!SupportList.Contains(GameObjects.Player.ChampionName))
            {
                Manager.WriteConsole(GameObjects.Player.ChampionName + " Not Support!", true);
                DelayAction.Add(2000, () => Variables.Orbwalker.Enabled = false);
                return;
            }

            Manager.WriteConsole(GameObjects.Player.ChampionName + " Load!  Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString(), true);

            Me = GameObjects.Player;

            Menu = new Menu("Flowers_Series", "Flowers' Series", true).Attach();
            Menu.Add(new MenuSeparator("Credit", "Credit: NightMoon"));
            Menu.Add(new MenuSeparator("Version", "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            Utility.Tools.Inject();

            switch (Me.ChampionName)
            {
                case "Ahri":
                    Ahri.Init();
                    break;
                case "Akali":
                    Akali.Init();
                    break;
                case "Ashe":
                    Ashe.Init();
                    break;
                case "Blitzcrank":
                    Blitzcrank.Init();
                    break;
                case "Darius":
                    Darius.Init();
                    break;
                case "Ezreal":
                    Ezreal.Init();
                    break;
                case "Graves":
                    Graves.Init();
                    break;
                case "Hecarim":
                    Hecarim.Init();
                    break;
                case "Illaoi":
                    Illaoi.Init();
                    break;
                case "Karma":
                    Karma.Init();
                    break;
                case "Morgana":
                    Morgana.Init();
                    break;
                case "Riven":
                    Riven.Init();
                    break;
                case "Ryze":
                    Ryze.Init();
                    break;
                case "Sivir":
                    Sivir.Init();
                    break;
                case "Tristana":
                    Tristana.Init();
                    break;
                case "TwistedFate":
                    TwistedFate.Init();
                    break;
                case "Twitch":
                    Twitch.Init();
                    break;
                case "Vayne":
                    Vayne.Init();
                    break;
                case "Viktor":
                    Viktor.Init();
                    break;
                case "Vladimir":
                    Vladimir.Init();
                    break;
                default:
                    break;
            }
        }
    }
}
