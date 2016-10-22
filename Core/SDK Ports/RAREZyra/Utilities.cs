#region copyright

// Copyright (c) KyonLeague 2016
// If you want to copy parts of the code, please inform the author and give appropiate credits
// File: Utilities.cs
// Author: KyonLeague
// Contact: "cryz3rx" on Skype 

#endregion

#region usage

using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;
using Keys = LeagueSharp.Common.Keys;
using KeyBindType = LeagueSharp.SDK.Enumerations.KeyBindType;
using Menu = LeagueSharp.SDK.UI.Menu;
using Spell = LeagueSharp.SDK.Spell;
using TargetSelector = LeagueSharp.SDK.TargetSelector;
using Version = System.Version;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RAREZyra
{
    internal class Utilities
    {
        
        internal static Menu MainMenu;
        internal static AIHeroClient Player;
        internal static TargetSelector targetSelector = Variables.TargetSelector;
        internal static Spell Q, W, E, R;
        internal const int FlashRange = 425, IgniteRange = 600;

        /// <summary>
        /// Prints your text into the chat.
        /// </summary>
        /// <param name="text">Used to give out the information as string</param>
        public static void PrintChat(string text)
        {
            Chat.Print("RAREZyra => {0}", text);
        }

        /// <summary>
        /// checks if there is an update for this assembly
        /// </summary>
        public static void UpdateCheck()
        {
            try
            {
                using (var web = new WebClient())
                {
                    var source = "https://raw.githubusercontent.com/KyonLeague/RAREScripts/master/RAREKarthus/RAREKarthus/RAREKarthus.csproj";

                    if (source == "") return;

                    var rawFile = web.DownloadString(source);
                    var checkFile =
                        new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match
                            (rawFile);

                    if (!checkFile.Success)
                    {
                        return;
                    }

                    var gitVersion =
                        new System.Version(
                            $"{checkFile.Groups[1]}.{checkFile.Groups[2]}.{checkFile.Groups[3]}.{checkFile.Groups[4]}");

                    if (gitVersion > Assembly.GetExecutingAssembly().GetName().Version)
                    {
                        PrintChat("Outdated! Newest Version: " + gitVersion);
                    }
                    else
                    {
                        PrintChat("You are on the newest version: " + gitVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// initialize the MainMenu for the Champion Menu. Should be called at first.
        /// </summary>
        public static void InitMenu()
        {
            MainMenu = new Menu("rarezyra", "rareZyra", true, Player.ChampionName).Attach();
            MainMenu.Separator("We love LeagueSharp.");
            MainMenu.Separator("Developer: @Kyon");
        }
    }

    internal static class Config
    {
        #region Static Fields

        private static int _cBlank = -1;

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// lets you create a new menupoint inside a <seealso cref="Menu"/>.
        /// </summary>
        /// <param name="subMenu">Your SubMenu to add it to</param>
        /// <param name="name">the so called ID</param>
        /// <param name="display">The displayed name inside the game</param>
        /// <param name="state">the default state of the menu</param>
        /// <returns>returns a <seealso cref="MenuBool"/> the can be used.</returns>
        public static MenuBool Bool(this Menu subMenu, string name, string display, bool state = true)
        {
            return subMenu.Add(new MenuBool(name, display, state));
        }

        public static MenuList List(this Menu subMenu, string name, string display, string[] array, int value = 0)
        {
            return subMenu.Add(new MenuList<string>(name, display, array) { Index = value });
        }

        public static MenuSeparator Separator(this Menu subMenu, string display)
        {
            _cBlank += 1;
            return subMenu.Add(new MenuSeparator("blank" + _cBlank, display));
        }

        public static MenuSlider Slider(this Menu subMenu, string name, string display,
            int cur, int min = 0, int max = 100)
        {
            return subMenu.Add(new MenuSlider(name, display, cur, min, max));
        }

        public static MenuKeyBind KeyBind(
            this Menu subMenu,
            string name,
            string display,
            System.Windows.Forms.Keys key,
            KeyBindType type = KeyBindType.Press)
        {
            return subMenu.Add(new MenuKeyBind(name, display, key, type));
        }
        #endregion
    }
}