using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.Commons.ComboSystem;
using EloBuddy;

namespace TheBrand.Commons
{
    public static class HealManager
    {
        private static bool _enabled;
        private static bool _onlyCombo;
        private static Spell _heal;

        public static void Initialize(Menu healMenu, ComboProvider combo, bool autoUpdate = true)
        {
            if (!HasHeal()) return;
            _heal = GetHeal();

            healMenu.AddMItem("Only in combo", true, (sender, args) => _onlyCombo = args.GetNewValue<bool>());
            healMenu.AddMItem("Enabled", false, (sender, args) => _enabled = args.GetNewValue<bool>());
            healMenu.AddMItem("(WIP - I highly recommend using an Activator!)");
            if (autoUpdate)
                Game.OnUpdate += _ => Update(combo);
        }

        public static void Update(ComboProvider combo)
        {
            if (combo.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && _onlyCombo) return;

            var healthPrediction = HealthPrediction.GetHealthPrediction(ObjectManager.Player, 1);
            if (healthPrediction <= 0 && 75 + ObjectManager.Player.Level * 15 + healthPrediction > 0 && !ObjectManager.Player.IsDead) //Todo: grievious wounds and anti heal buff
            {
                UseHeal();
            } //todo continue
        }

        /// <summary>
        /// Note: does NOT check the menu options
        /// </summary>
        /// <param name="target"></param>
        public static void UseHeal()
        {
            if (_heal == null || _heal.GetState() != SpellState.Ready) return;
            _heal.Cast(Game.CursorPos);
        }

        public static bool HasHeal()
        {
            return ObjectManager.Player.Spellbook.Spells.Any(spell => spell.Name == "summonerheal");
        }

        public static Spell GetHeal()
        {
            var spellDataInst = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerheal");
            if (spellDataInst != null) return new Spell(spellDataInst.Slot);
            return null;
        }
    }
}
