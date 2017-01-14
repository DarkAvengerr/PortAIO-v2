using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Spells
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AlqoholicKarthus.Menu;

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

        /// <summary>
        ///     SpellManager Constructor
        /// </summary>
        internal SpellManager()
        {
            try
            {
                this.LoadSpells(new List<SpellBase>() { new Q(), new W(), new E(), new R() });
                Spells.Q = new Q();
                Spells.W = new W();
                Spells.E = new E();
                Spells.R = new R();
            }
            catch (Exception e)
            {
                Console.WriteLine("@SpellManager.cs: Cannot initialise the spells - {0}", e);
            }

            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            var predictionMode = AlqoholicMenu.MainMenu.Item("prediction.mode").GetValue<StringList>().SelectedIndex;

            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || Shop.IsOpen)
            {
                return;
            }

            switch (Program.Orbwalker.ActiveMode)
            {

                case Orbwalking.OrbwalkingMode.Combo:

                    this.spells.Where(spell => CanCast(spell.SpellSlot, Orbwalking.OrbwalkingMode.Combo))
                        .ToList()
                        .ForEach(spell => spell.Combo(predictionMode));
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

            if (spellLower.Equals("q") && AlqoholicMenu.MainMenu.Item("q.auto").GetValue<KeyBind>().Active)
            {
                return true;
            }

            if (spellLower.Equals("r") && AlqoholicMenu.MainMenu.Item("comboruse").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.ToList())
                {
                    if (!AlqoholicMenu.MainMenu.Item("dontr" + enemy.ChampionName).GetValue<bool>())
                    {
                        return false;
                    }

                    if (
                        !(ObjectManager.Player.Distance(
                            HeroManager.Enemies.ToList()
                              .OrderBy(x => x.Distance(ObjectManager.Player.Position))
                              .FirstOrDefault()) < AlqoholicMenu.MainMenu.Item("rsafespace").GetValue<Slider>().Value))
                    {
                        continue;
                    }
                    var damageWithReduction = Spells.R.SpellObject.GetDamage(enemy)
                                              * (1
                                                 - (Convert.ToDouble(
                                                     AlqoholicMenu.MainMenu.Item("rdmgreduction")
                                                         .GetValue<Slider>()
                                                         .Value / 100)));
                    return damageWithReduction >= enemy.Health;
                }
            }

            if (Program.Orbwalker.ActiveMode != orbwalkingMode || !spell.IsReady())
            {
                return false;
            }

            try
            {
                return AlqoholicMenu.MainMenu.Item(orbwalkingModeLower + spellLower + "use").GetValue<bool>()
                       && AlqoholicMenu.MainMenu.Item(orbwalkingModeLower + spellLower + "mana")
                              .GetValue<Slider>()
                              .Value <= ObjectManager.Player.ManaPercent;
            }
            catch (Exception e)
            {
                Console.WriteLine("@SpellManager.cs: Cannot get CanCast for slot {0} - {1}", spell, e);
                throw;
            }
        }

        #endregion
    }
}