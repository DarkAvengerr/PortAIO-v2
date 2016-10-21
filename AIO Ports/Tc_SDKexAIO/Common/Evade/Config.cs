// Copyright 2014 - 2014 Esk0r
// Config.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Common.Evade
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;

    internal static class Config
    {
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int PathFindingDistance = 60;
        public const int PathFindingDistance2 = 35;
        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;
        public const int CrossingTimeOffset = 250;
        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 80;
        public const int EvadingRouteChangeTimeOffset = 250;
        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;
        public static Menu Menu;

        public static void CreateMenu()
        {
            Menu = PlaySharp.Menu.Add(new Menu("Evade", "Evade#"));
            Menu.Add(new MenuSeparator("Credit", "Evade#  Credit By Esk0r"));

            var evadeSpells = Menu.Add(new Menu("evadeSpells", "Evade spells"));
            {
                foreach (var spell in EvadeSpellDatabase.Spells)
                {
                    var subMenu = evadeSpells.Add(new Menu(spell.Name, spell.Name));
                    {
                        subMenu.Add(new MenuSlider("DangerLevel" + spell.Name, "Danger level", spell.DangerLevel, 1, 5));
                        subMenu.Add(new MenuBool("Enabled" + spell.Name, "Enabled", true));
                    }
                }
            }

            var skillShots = Menu.Add(new Menu("Skillshots", "Skillshots"));
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.Team != GameObjects.Player.Team)
                    {
                        foreach (var spell in SpellDatabase.Spells)
                        {
                            if (string.Equals(spell.ChampionName, hero.ChampionName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                var subMenu = skillShots.Add(new Menu(spell.MenuItemName, spell.MenuItemName));
                                {
                                    subMenu.Add(new MenuSlider("DangerLevel" + spell.MenuItemName, "Danger level", spell.DangerValue, 1, 5));
                                    subMenu.Add(new MenuBool("IsDangerous" + spell.MenuItemName, "Is Dangerous", spell.IsDangerous));
                                    subMenu.Add(new MenuBool("Draw" + spell.MenuItemName, "Draw", true));
                                    subMenu.Add(new MenuBool("Enabled" + spell.MenuItemName, "Enabled", !spell.DisabledByDefault));
                                }
                            }
                        }
                    }
                }
            }

            var shielding = Menu.Add(new Menu("Shielding", "Ally shielding"));
            {
                foreach (var ally in ObjectManager.Get<AIHeroClient>())
                {
                    if (ally.IsAlly && !ally.IsMe)
                    {
                        shielding.Add(new MenuBool("shield" + ally.ChampionName, "Shield " + ally.ChampionName, true));
                    }
                }
            }

            var collision = Menu.Add(new Menu("Collision", "Collision"));
            {
                collision.Add(new MenuBool("MinionCollision", "Minion collision"));
                collision.Add(new MenuBool("HeroCollision", "Hero collision"));
                collision.Add(new MenuBool("YasuoCollision", "Yasuo wall collision", true));
                collision.Add(new MenuBool("EnableCollision", "Enabled"));
            }

            var drawings = Menu.Add(new Menu("Drawings", "Drawings"));
            {
                drawings.Add(new MenuSlider("Border", "Border Width", 2, 1, 5));
                drawings.Add(new MenuBool("ShowEvadeStatus", "Show Evade Status"));
                drawings.Add(new MenuBool("EnableDrawings", "Enabled", true));
            }

            var misc = Menu.Add(new Menu("Misc", "Misc"));
            {
                misc.Add(new MenuList<string>("BlockSpells", "Block spells while evading", new[] { "Only dangerous", "Always", "No" }));
                misc.Add(new MenuBool("DisableFow", "Disable fog of war dodging"));
            }

            var Enable = Menu.Add(new MenuKeyBind("Enabled", "Enabled", System.Windows.Forms.Keys.K, LeagueSharp.SDK.Enumerations.KeyBindType.Toggle));
            Enable.Active = true;

            Menu.Add(new MenuKeyBind("OnlyDangerous", "Dodge only dangerous", System.Windows.Forms.Keys.Space, LeagueSharp.SDK.Enumerations.KeyBindType.Press));
        }
    }
}
