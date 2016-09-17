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

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NidaleeTheBestialHuntress
{
    internal class ManaManager
    {
        private readonly AIHeroClient _player;
        private Menu _menu;

        public ManaManager()
        {
            _player = ObjectManager.Player;
        }

        /// <summary>
        ///     Adds the options to the main menu
        /// </summary>
        /// <param name="attachMenu"></param>
        public void AddToMenu(ref Menu attachMenu)
        {
            _menu = attachMenu;
            CreateMenu();

            //Chat.Print(string.Format("{0} loaded by {1}", "Mana Manager", "iJabba"));
        }

        /// <summary>
        ///     Actually creates the menu
        /// </summary>
        public void CreateMenu()
        {
            var manaMenu = new Menu("Mana Manager", "manaManager");
            {
                manaMenu.AddItem(new MenuItem("minHarassMana", "Min Mana for harass").SetValue(new Slider(40)));
                manaMenu.AddItem(new MenuItem("minLaneclearMana", "Min Mana for laneclear").SetValue(new Slider(40)));
            }

            _menu.AddSubMenu(manaMenu);
        }

        public bool CanUseSpell(Spell spell)
        {
            return _player.Mana >= _player.Spellbook.GetSpell(spell.Slot).SData.Mana;
        }

        public bool CanDoCombo()
        {
            return _player.Mana >=
                   _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + _player.Spellbook.GetSpell(SpellSlot.W).SData.Mana +
                   _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana + _player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
        }

        public bool CanHarass()
        {
            return !(_player.ManaPercentage() <= _menu.Item("minHarassMana").GetValue<Slider>().Value);
        }

        public bool CanLaneclear()
        {
            return !(_player.ManaPercentage() <= _menu.Item("minLaneclearMana").GetValue<Slider>().Value);
        }
    }
}