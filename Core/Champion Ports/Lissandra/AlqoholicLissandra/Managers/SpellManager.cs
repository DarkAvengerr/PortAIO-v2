using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Managers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AlqoholicLissandra.Menu;
    using AlqoholicLissandra.Spells;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class SpellManager
    {
        #region Fields

        /// <summary>
        ///     List of Spells
        /// </summary>
        private readonly List<SpellBase> spells = new List<SpellBase>();

        #endregion

        #region Constructors and Destructors

        internal SpellManager()
        {
            this.LoadSpells(new List<SpellBase>() { new Q(), new W(), new E(), new R() });
            Spells.Q = new Q();
            Spells.W = new W();
            Spells.E = new E();
            Spells.R = new R();

            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            if (AlqoholicMenu.MainMenu.Item("etomouse").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (Spells.E.SpellObject.IsReady() && !ObjectManager.Player.HasBuff("LissandraE"))
                {
                    Spells.E.Escape();
                }
            }

            switch (Program.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:

                    this.spells.Where(spell => CanCast(spell.SpellSlot, Orbwalking.OrbwalkingMode.Combo))
                        .ToList()
                        .ForEach(spell => spell.Combo());
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:

                    this.spells.Where(spell => CanCast(spell.SpellSlot, Orbwalking.OrbwalkingMode.Mixed))
                        .ToList()
                        .ForEach(spell => spell.Harass());
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:

                    if (AlqoholicMenu.MainMenu.Item("farmspells").GetValue<KeyBind>().Active)
                    {
                        this.spells.Where(spell => CanCast(spell.SpellSlot, Orbwalking.OrbwalkingMode.LaneClear))
                            .ToList()
                            .ForEach(spell => spell.Farm());
                    }

                    break;

                case Orbwalking.OrbwalkingMode.LastHit:

                    this.spells.Where(spell => CanCast(spell.SpellSlot, Orbwalking.OrbwalkingMode.LastHit))
                        .ToList()
                        .ForEach(spell => spell.LastHit());
                    break;
            }
        }

        /// <summary>
        ///     Load Spells Method
        /// </summary>
        /// <param name="spellList">
        ///     The Spells
        /// </param>
        private void LoadSpells(IEnumerable<SpellBase> spellList)
        {
            foreach (var spell in spellList)
            {
                AlqoholicMenu.GenerateSpellMenu(spell.SpellSlot);

                this.spells.Add(spell);
            }
        }

        private static bool CanCast(SpellSlot spell, Orbwalking.OrbwalkingMode orbwalkingMode)
        {
            var orbwalkingModeLower = Program.Orbwalker.ActiveMode.ToString().ToLower();
            var spellLower = spell.ToString().ToLower();

            if (Program.Orbwalker.ActiveMode != orbwalkingMode || !spell.IsReady())
            {
                return false;
            }

            if (spellLower.Equals("e") && ObjectManager.Player.HasBuff("LissandraE"))
            {
                return false;
            }

            if ((orbwalkingModeLower.Equals("laneclear") && spellLower.Equals("e"))
                || (orbwalkingModeLower.Equals("laneclear") && spellLower.Equals("r"))
                || (orbwalkingModeLower.Equals("mixed") && spellLower.Equals("r"))
                || (orbwalkingModeLower.Equals("lasthit") && !spellLower.Equals("q")))
            {
                return false;
            }

            return AlqoholicMenu.MainMenu.Item(orbwalkingModeLower + spellLower + "use").GetValue<bool>()
                   && AlqoholicMenu.MainMenu.Item(orbwalkingModeLower + spellLower + "mana")
                          .GetValue<Slider>()
                          .Value <= ObjectManager.Player.ManaPercent;
        }

        #endregion
    }
}