using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace DZLib.Menu
{
    public static class ModesMenuExtensions
    {
        private static string BaseName = $"dzlib.champion.{ObjectManager.Player.ChampionName}.";

        public static void SetBaseName(string name)
        {
            BaseName = name;
        }
        
        public static string GetCurrentBaseName()
        {
            return BaseName;
        }

        public static void AddManaManager(this LeagueSharp.Common.Menu menu, Mode mode, SpellSlot[] spellList, int[] manaCosts)
        {
            var mmMenu = new LeagueSharp.Common.Menu("Mana Manager", BaseName + ObjectManager.Player.ChampionName.ToLowerInvariant() + ".mm." + GetStringFromMode(mode));
            for (var i = 0; i < spellList.Count(); i++)
            {
                mmMenu.AddItem(
                    new MenuItem(
                        BaseName + ObjectManager.Player.ChampionName.ToLowerInvariant() + ".manamanager." + GetStringFromSpellSlot(spellList[i]).ToLowerInvariant() + "mana" + GetStringFromMode(mode).ToLowerInvariant(),
                        GetStringFromSpellSlot(spellList[i]) + " Mana").SetValue(new Slider(manaCosts[i])));
            }
            menu.AddSubMenu(mmMenu);
        }

        public static void AddModeMenu(this LeagueSharp.Common.Menu menu, Mode mode, SpellSlot[] spellList, bool[] values)
        {
            for (var i = 0; i < spellList.Count(); i++)
            {
                menu.AddItem(
                    new MenuItem(
                        BaseName + ObjectManager.Player.ChampionName.ToLowerInvariant() + ".use" + GetStringFromSpellSlot(spellList[i]).ToLowerInvariant() + GetStringFromMode(mode),
                        "Use " + GetStringFromSpellSlot(spellList[i]) + " " + GetFullNameFromMode(mode)).SetValue(values[i]));
            }
        }

        public static void AddDrawMenu(this LeagueSharp.Common.Menu menu, Dictionary<SpellSlot, Spell> dictionary, Color myColor)
        {
            foreach (var entry in dictionary)
            {
                var slot = entry.Key;
                if (entry.Value.Range < 4000f)
                {
                    menu.AddItem(new MenuItem(BaseName + ".drawing.draw" + GetStringFromSpellSlot(slot), "Draw " + GetStringFromSpellSlot(slot)).SetValue(new Circle(true, myColor)));
                }
            }
        }

        public static void AddHitChanceSelector(this LeagueSharp.Common.Menu menu)
        {
            menu.AddItem(
                    new MenuItem(BaseName + "customhitchance", "Hitchance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2)));
        }

        public static void AddHeroMenu(this LeagueSharp.Common.Menu menu, string menuDisplayName, string name, bool allies)
        {
            var _menu = new LeagueSharp.Common.Menu(menuDisplayName, BaseName + ObjectManager.Player.ChampionName.ToLowerInvariant() + ".noult");
            foreach (var player in ObjectManager.Get<AIHeroClient>().Where(h => !h.IsMe && allies ? h.IsAlly : h.IsEnemy))
            {
                _menu.AddItem(new MenuItem(BaseName + ObjectManager.Player.ChampionName.ToLowerInvariant() + "."+ name +"." + player.ChampionName, player.ChampionName).SetValue(false));
            }
            menu.AddSubMenu(_menu);
        }

        public static String GetStringFromSpellSlot(SpellSlot sp)
        {
            return sp.ToString();
        }

        static String GetStringFromMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Combo:
                    return "C";
                case Mode.Harrass:
                    return "H";
                case Mode.Lasthit:
                    return "LH";
                case Mode.Laneclear:
                    return "LC";
                case Mode.Farm:
                    return "F";
                default:
                    return "unk";
            }
        }
        static String GetFullNameFromMode(Mode mode)
        {
            return mode.ToString();
        }

        public enum Mode
        {
            Combo,
            Harrass,
            Lasthit,
            Laneclear,
            Farm
        }
    }
}
