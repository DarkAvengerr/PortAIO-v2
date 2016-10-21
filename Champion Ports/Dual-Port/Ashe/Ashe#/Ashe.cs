// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ashe.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Main Ashe
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AsheSharp.Source
{
    using System;
    using System.Drawing;

    using global::AsheSharp.Source.Logic;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// Main Ashe
    /// </summary>
    internal class Ashe
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ashe"/> class.
        /// </summary>
        public Ashe()
        {
            // Create a new menu
            Menu = new Menu(ChampionName, ChampionName, true);

            // Orbwaker
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            // Target Selector
            TargetSelector.AddToMenu(Menu.SubMenu("TargetSelector"));

            // Combo
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("QSlider", "Stacks to use Q").SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            Menu.SubMenu("Combo")
                .AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            // Harass
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassWCombo", "Use W").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("ManaSlider", "Mana to Harass").SetValue(new Slider(70)));
            Menu.SubMenu("Harass")
                .AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind('X', KeyBindType.Press)));
            Menu.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "Harass (Toggle)!").SetValue(new KeyBind('V', KeyBindType.Toggle)));

            // Misc
            Menu.SubMenu("Misc").AddItem(new MenuItem("blueTrinket", "Buy Blue Trinket on Level 6").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Baseult", "Baseult").SetValue(true));

            // Drawings
            Menu.SubMenu("Drawings")
                .AddItem(new MenuItem("drawW", "Draw W Range Circle").SetValue(new Circle(true, Color.Aqua, W.Range)));

            Menu.AddToMainMenu();

            // Spells
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1200);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.5f, 100, 902, true, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.5f, 100, 1600, false, SkillshotType.SkillshotLine);

            // Load Logics
            new Combo();
            new Harass();
            new Misc();

            // Listen to additional events
            Drawing.OnDraw += this.Drawing_OnDraw;
        }

        /// <summary>
        /// The drawing_ on draw.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.IsDead && W.Level > 0 && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);
            }
        }

        #endregion

        #region Static Fields

        /// <summary>
        /// The menu.
        /// </summary>
        public static Menu Menu;

        /// <summary>
        /// The orbwalker.
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// The Q Spell
        /// </summary>
        public static Spell Q;

        /// <summary>
        /// The W Spell
        /// </summary>
        public static Spell W;

        /// <summary>
        /// The E Spell
        /// </summary>
        public static Spell E;

        /// <summary>
        /// The R Spell
        /// </summary>
        public static Spell R;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the champion name.
        /// </summary>
        public static string ChampionName
        {
            get
            {
                return "Ashe";
            }
        }

        // Player
        /// <summary>
        /// Gets the player.
        /// </summary>
        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion
    }
}