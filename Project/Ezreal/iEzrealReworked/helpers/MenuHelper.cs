// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iEzrealReworked.helpers
{
    internal static class MenuHelper
    {
        public static bool IsEnabledAndReady(this Spell spell, Mode mode)
        {
            if (ObjectManager.Player.IsDead)
            {
                return false;
            }
            try
            {
                var manaPercentage =
                    GetSliderValue(
                        "com.iezreal.manamanager." + GetStringFromSpellSlot(spell.Slot).ToLowerInvariant() + "mana" +
                        GetStringFromMode(mode).ToLowerInvariant());
                var enabledCondition =
                    IsMenuEnabled(
                        "com.iezreal.use" + GetStringFromSpellSlot(spell.Slot).ToLowerInvariant() +
                        GetStringFromMode(mode));
                return spell.IsReady() && (ObjectManager.Player.ManaPercentage() >= manaPercentage) && enabledCondition;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public static void AddManaManager(this Menu menu, Mode mode, SpellSlot[] spellList, int[] manaCosts)
        {
            var mmMenu = new Menu("Mana Manager", "com.iezreal.mm." + GetStringFromMode(mode));
            for (var i = 0; i < spellList.Count(); i++)
            {
                mmMenu.AddItem(
                    new MenuItem(
                        "com.iezreal.manamanager." + GetStringFromSpellSlot(spellList[i]).ToLowerInvariant() + "mana" +
                        GetStringFromMode(mode).ToLowerInvariant(), GetStringFromSpellSlot(spellList[i]) + " Mana")
                        .SetValue(new Slider(manaCosts[i])));
            }
            menu.AddSubMenu(mmMenu);
        }

        //TODO make it better?
        public static void AddKillstealMenu(this Menu menu, SpellSlot[] spells, bool[] values)
        {
            for (int i = 0; i < spells.Count(); i++)
            {
                menu.AddItem(
                    new MenuItem(
                        "com.iezreal.killsteal.use" + GetStringFromSpellSlot(spells[i]).ToLowerInvariant(),
                        "Use " + GetStringFromSpellSlot(spells[i]) + " Killsteal").SetValue(values[i]));
            }
        }

        public static void AddModeMenu(this Menu menu, Mode mode, SpellSlot[] spellList, bool[] values)
        {
            for (var i = 0; i < spellList.Count(); i++)
            {
                menu.AddItem(
                    new MenuItem(
                        "com.iezreal.use" + GetStringFromSpellSlot(spellList[i]).ToLowerInvariant() +
                        GetStringFromMode(mode),
                        "Use " + GetStringFromSpellSlot(spellList[i]) + " " + GetFullNameFromMode(mode)).SetValue(
                            values[i]));
            }
        }

        public static void AddDrawMenu(this Menu menu, Dictionary<SpellSlot, Spell> dictionary, Color myColor)
        {
            foreach (var entry in dictionary)
            {
                var slot = entry.Key;
                if (entry.Value.Range < 4000f)
                {
                    menu.AddItem(
                        new MenuItem(
                            "com.iezreal.drawing.draw" + GetStringFromSpellSlot(slot),
                            "Draw " + GetStringFromSpellSlot(slot)).SetValue(new Circle(true, myColor)));
                }
            }
        }

        public static void AddHitChanceSelector(this Menu menu)
        {
            menu.AddItem(
                new MenuItem("com.iezreal.customhitchance", "Hitchance").SetValue(
                    new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2)));
        }

        public static bool IsMenuEnabled(string item)
        {
            return iEzrealReworked.Menu.Item(item).GetValue<bool>();
        }

        public static int GetSliderValue(string item)
        {
            return iEzrealReworked.Menu.Item(item) != null
                ? iEzrealReworked.Menu.Item(item).GetValue<Slider>().Value
                : -1;
        }

        public static Color GetCicleColour(string item)
        {
            return iEzrealReworked.Menu.Item(item) != null
                ? iEzrealReworked.Menu.Item(item).GetValue<Circle>().Color
                : Color.DarkRed;
        }

        public static bool GetKeybindValue(string item)
        {
            return iEzrealReworked.Menu.Item(item).GetValue<KeyBind>().Active;
        }

        public static HitChance GetHitchance()
        {
            switch (iEzrealReworked.Menu.Item("com.iezreal.customhitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        public static string GetStringFromSpellSlot(SpellSlot sp)
        {
            //TODO Test if this works
            //return sp.ToString();
            switch (sp)
            {
                case SpellSlot.Q:
                    return "Q";
                case SpellSlot.W:
                    return "W";
                case SpellSlot.E:
                    return "E";
                case SpellSlot.R:
                    return "R";
                default:
                    return "unk";
            }
        }

        public static string GetStringFromMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Combo:
                    return "C";
                case Mode.Harass:
                    return "H";
                case Mode.Lasthit:
                    return "LH";
                case Mode.Laneclear:
                    return "LC";
                case Mode.Farm:
                    return "F";
                case Mode.Killsteal:
                    return "K";
                case Mode.Jungleclear:
                    return "JC";
                default:
                    return "unk";
            }
        }

        public static string GetFullNameFromMode(Mode mode)
        {
            return mode.ToString();
        }
    }

    internal enum Mode
    {
        Combo,
        Harass,
        Lasthit,
        Laneclear,
        Farm,
        Killsteal,
        Jungleclear
    }
}