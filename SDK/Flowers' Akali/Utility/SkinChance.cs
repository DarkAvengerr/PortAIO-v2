using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Collections.Generic;

    internal class SkinChance
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        private static int SkinID;

        internal static void Inject()
        {
            SkinID = Me.SkinId;

            var SkinMenu = Menu.Add(new Menu("SkinChance", "Skin Chance"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled", false));

                switch (Me.ChampionName)
                {
                    case "Akali":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", AkaliSkin));
                        break;
                    case "Blitzcrank":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", BlitzcrankSkin));
                        break;
                    case "Darius":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", DariusSkin));
                        break;
                    case "Ezreal":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", EzrealSkin));
                        break;
                    case "Sivir":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", SivirSkin));
                        break;
                    case "Tristana":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", TristanaSkin));
                        break;
                    case "Twitch":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", TwitchSkin));
                        break;
                    case "Varus":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", VarusSkin));
                        break;
                    case "Vayne":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", VayneSkin));
                        break;
                    default:
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" }));
                        break;
                }
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["SkinChance"]["Enable"])
            {
            }
            else if (!Menu["SkinChance"]["Enable"])
            {
            }
        }

        private static IEnumerable<string> AkaliSkin = new[]
        {
            "Classic", "Stinger", "Crimson", "All-Star", "Nurse", "Blood Moon", "Silverfang", "Headhunter"
        };
        private static IEnumerable<string> BlitzcrankSkin = new[]
        {
            "Classic", "Definitely Not", "Boom Boom", "Rusty", "Goalkeeper", "Piltover Customs", "iBlitzcrank", "Riot", "Battle Boss"
        };
        private static IEnumerable<string> DariusSkin = new[]
        {
            "Classic", "Lord", "Bioforge", "Woad King", "Dunkmaster", "Academy"
        };
        private static IEnumerable<string> EzrealSkin = new[]
        {
            "Classic", "Frosted", "Striker", "Nottingham", "Explorer", "Pulsefire", "TPA", "Debonair", "Ace of Spades"
        };
        private static IEnumerable<string> SivirSkin = new[]
        {
            "Classic", "Warrior Princess", "Spectacular", "Huntress", "Bandit", "PAX", "Snowstorm", "Warden", "Victorious"
        };
        private static IEnumerable<string> TristanaSkin = new[]
        {
            "Classic", "Riot Girl", "Earnest Elf", "Firefighter", "Guerilla", "Buccaneer", "Rocket Girl", "Dragon Trainer"
        };
        private static IEnumerable<string> TwitchSkin = new[]
        {
            "Classic", "Kingpin", "Whistler", "Medieval", "Gangster", "Vandal", "Pickpocket", "SSW"
        };
        private static IEnumerable<string> VarusSkin = new[]
        {
            "Classic", "Blight Crystal", "Arclight", "Arctic Ops", "Heartseeker", "Swiftbolt", "Dark Star"
        };
        private static IEnumerable<string> VayneSkin = new[]
        {
            "Classic", "Vindicator", "Aristocrat", "Dragonslayer", "Heartseeker", "SKT T1", "Arclight"
        };
    }
}