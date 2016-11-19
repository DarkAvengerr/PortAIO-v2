using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Champions;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Common
{
    internal class CommonSkinManager
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Skins", "MenuSkin");
            {
                MenuLocal.AddItem(new MenuItem("Settings.Skin", "Skin:").SetValue(false)).ValueChanged +=
                    (sender, args) =>
                    {
                        if (!args.GetNewValue<bool>())
                        {
                            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
                        }
                    };

                string[] strSkins = new[] { "" };

                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "caitlyn":
                        {
                            strSkins = new[] { "Classic Caitlyn", "Resistance Caitlyn", "Sheriff Caitlyn", "Safari Caitlyn", "Artic Warfare Caitlyn", "Office Caitlyn", "Headhunter Caitlyn", "Red Caitlyn", "Green Caitlyn", "Blue Caitlyn",  "Lunar Wraith Caitlyn" };
                            break;
                        }

                    case "corki":
                        {
                            strSkins = new[] { "Classic Caitlyn", "Resistance Caitlyn", "Sheriff Caitlyn", "Artic Warfare Caitlyn", "Office Caitlyn", "Headhunter Caitlyn", "Safari Caitlyn", "Lunar Wraith Caitlyn", "sss", "aaa", "bbb", "ccc" };
                            break;
                        }

                    case "ezreal":
                        {
                            strSkins = new[] { "Classic Ezreal", "Nottingham Ezreal", "Striker Ezreal", "Frosted Ezreal", "Explorer Ezreal", "Pulsefire Ezreal", "TPA Ezreal", "Deboniar Ezreal", "Ace of Spades Ezreal", "Arcade Ezreal" };
                            break;
                        }

                    case "graves":
                        {
                            strSkins = new[] { "Classic Graves", "Hired Gun Graves", "Mafia Graves", "Pool Party Graves", "Cutthroat Graves", "Jailbreak Graves Ezreal", "Riot Ezreal" };
                            break;
                        }

                    case "kalista":
                        {
                            strSkins = new[] { "Classic Kalista", "Blood Moon Kalista", "Championship Kalista" };
                            break;
                        }

                    case "kogmaw":
                        {
                            strSkins = new[] { "Classic Kog'Maw", "Caterpillar Kog'Maw", "Sonoran Kog'Maw", "Monarch Kog'Maw", "Reindeer Kog'Maw", "Lion Dance Kog'Maw", "Deep Sea Kog'Maw", "Jurassic Kog'Maw", "Battlecast Kog'Maw" };
                            break;
                        }

                    case "jinx":
                        {
                            strSkins = new[] { "Classic Jinx", "Mafia Jinx", "Firecracker Jinx", "Slayer Jinx" };
                            break;
                        }

                    case "lucian":
                        {
                            strSkins = new[] { "Classic Lucian", "Hired Gun Lucian", "Striker Lucian", "Chrome Pack: Yellow Lucian", "Chrome Pack: Red Lucian", "Chrome Pack: Blue Lucian", "Project: Lucian" };
                            break;
                        }

                    case "missfortune":
                        {
                            strSkins = new[] { "Classic Miss Fortune", "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9" };
                            break;
                        }
                    case "sivir":
                        {
                            strSkins = new[] { "Classic Sivir", "Warrior Princess Sivir", "Spectacutar Sivir", "Huntress Sivir", "Bandit Sivir", "PAX Sivir", "Showstorm Sivir", "Warden Sivir", "Victorious Sivir" };
                            break;
                        }

                    case "tristana":
                        {
                            strSkins = new[] { "Classic Tristana", "Riot Girl Tristana", "Earnest Elf Tristana", "Firefighter Tristana", "Guerilla Tristana", "Buccaneer Tristana", "Rocket Girl Tristana", "Chrome Tristana I", "Chrome Tristana II", "Chrome Tristana III", "Dragon Trainer Tristana" };
                            break;
                        }

                    case "twitch":
                        {
                            strSkins = new[] { "Classic Twitch", "Kingpin Twitch", "Whistler Village Twitch", "Medieval Twitch", "Gangster Twitch", "Vandal Twitch", "Pickpocket Twitch", "SSW Twitch" };
                            break;
                        }

                    case "urgot":
                        {
                            strSkins = new[] { "Classic Urgot", "Gaint Enemy Crabgot", "Butcher Urgot", "Battlecast Urgot" };
                            break;
                        }

                    case "vayne":
                        {
                            strSkins = new[] { "Classic Vayne", "Vindicator Vayne", "Aristocrat Vayne", "Dragonslayer Vayne", "Heartseeker Vayne", "SKT T1 Vayne", "Arclight Vayne" };
                            break;
                        }
                }

                MenuLocal.AddItem(new MenuItem("Settings.SkinID", "Skin Name:").SetValue(new StringList(strSkins, 0)));
            }
            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (MenuLocal.Item("Settings.Skin").GetValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, MenuLocal.Item("Settings.SkinID").GetValue<StringList>().SelectedIndex);
            }
        }
    }
}
