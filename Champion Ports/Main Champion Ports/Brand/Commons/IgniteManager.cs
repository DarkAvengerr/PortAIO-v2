using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.Commons.ComboSystem;
using EloBuddy;

namespace TheBrand.Commons
{
    public static class IgniteManager
    {

        private static MenuItem _igniteUsage, _igniteKillsteal, _igniteSituation, _igniteMaxAutoattacks, _igniteOnlyCombo, _igniteSpellsCooldown, _igniteCloseFight, _igniteCloseFightHealth;
        private static SpellDataInst _ignite;
        private static Spell _igniteSpell;
        public static float LastIgniteTime;
        public static AIHeroClient LastIgniteTarget;

        /// <summary>
        /// Adds to the menu and stuffs
        /// </summary>
        public static void Initialize(Menu igniteMenu, ComboProvider combo, bool autoUpdate = false)
        {
            _ignite = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (_ignite == null) return;
            _igniteSpell = new Spell(_ignite.Slot);
            _igniteUsage = igniteMenu.AddMItem("Use Ignite", true);
            _igniteKillsteal = igniteMenu.AddMItem("Killsteal", false);
            _igniteOnlyCombo = igniteMenu.AddMItem("Only in combo", true);
            _igniteSituation = igniteMenu.AddMItem("Situation analysis", true);
            igniteMenu.AddMItem("Don't use if killable by");
            _igniteMaxAutoattacks = igniteMenu.AddMItem("X Autoattacks", new Slider(1, 0, 5));
            _igniteSpellsCooldown = igniteMenu.AddMItem("Don't use if skills up", true);
            _igniteCloseFight = igniteMenu.AddMItem("Ignore above in close fight", true);
            _igniteCloseFightHealth = igniteMenu.AddMItem("Close fight health diff %", new Slider(20));
            if (autoUpdate)
                Game.OnUpdate += _ => Update(combo);
        }

        /// <summary>
        /// Does it's checks and ignites if all conditions are met. Uses Menu settings
        /// </summary>
        /// <param name="combo">The combo provider</param>
        /// <param name="otherDamage">Damage that will hit for sure in the near future, e.g. brand passive</param>
        /// <param name="otherDamageTime">How much time will pass till the otherDamage applies (checks on healthreg ...)</param>
        public static void Update(ComboProvider combo, float otherDamage = 0f, float otherDamageTime = 3000f)
        {
            if (_ignite == null || _igniteUsage == null || !_igniteUsage.GetValue<bool>()) return;
            if (combo.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && _igniteOnlyCombo.GetValue<bool>()) return;

            if (_igniteKillsteal.GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies)
                    UpdateOnHero(enemy, otherDamage, otherDamageTime, _igniteKillsteal.GetValue<bool>());
            }
            else if (combo.Target.IsValidTarget(600))
                UpdateOnHero(combo.Target, otherDamage, otherDamageTime, _igniteKillsteal.GetValue<bool>());

        }

        /// <summary>
        /// Does it's checks and ignites if all conditions are met
        /// </summary>
        /// <param name="target"></param>
        /// <param name="otherDamage">Damage that will hit for sure in the near future, e.g. brand passive</param>
        /// <param name="otherDamageTime">How much time will pass till the otherDamage applies (checks on healthreg ...)</param>
        /// <param name="ks">Uses ignite to kill even if enemy would die</param>
        private static void UpdateOnHero(AIHeroClient target, float otherDamage, float otherDamageTime, bool ks)
        {
            var enemyHealth = target.Health; // + attackshield when fixed PJSalt
            if (ks && enemyHealth < GetDamage() / 5f)
                UseIgnite(target);

            var fixedDamage = otherDamage + (target.Health - HealthPrediction.GetHealthPrediction(target, (int) otherDamageTime));
            if (Orbwalking.InAutoAttackRange(target))
                fixedDamage += (float)(_igniteMaxAutoattacks.GetValue<Slider>().Value * ObjectManager.Player.GetAutoAttackDamage(target));

            enemyHealth -= fixedDamage;

            if (GetDamage() < enemyHealth + (target.HPRegenRate * 2.5f)) return;

            if (_igniteSpellsCooldown.GetValue<bool>() && (ObjectManager.Player.GetSpell(SpellSlot.Q).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.W).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.E).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.R).State == SpellState.Ready))
                if (_igniteCloseFight.GetValue<bool>() && (ObjectManager.Player.HealthPercent - target.HealthPercent) < _igniteCloseFightHealth.GetValue<Slider>().Value)
                    return;

            if (fixedDamage < enemyHealth && (!_igniteSituation.GetValue<bool>() || !IsDeadForSure(target)))
                UseIgnite(target);
        }

        public static float GetDamage()
        {
            return 50 + ObjectManager.Player.Level * 20;
        }

        public static float GetRemainingDamage(AIHeroClient target)
        {
            var ignitebuff = target.GetBuff("summonerdot");
            if (ignitebuff == null) return 0;
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.True, ((int)(ignitebuff.EndTime - Game.Time) + 1) * GetDamage() / 5);
        }

        /// <summary>
        /// Does some checks on the situation, like health, enemy flash up, enemy distance ...40
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fleeRange"></param>
        /// <param name="dangerousHealthPercent"></param>
        /// <returns></returns>
        private static bool IsDeadForSure(AIHeroClient target, float fleeRange = 550, float dangerousHealthPercent = 0.25f) //Todo: make better
        {
            var myHealthPercent = ObjectManager.Player.HealthPercent/100f;
            var enemyHealthPercent = target.HealthPercent/100f;
            if (myHealthPercent < enemyHealthPercent + dangerousHealthPercent)
                return false;



            if (target.Spellbook.Spells.Any(spell => spell.Name == "summonerflash" && spell.State == SpellState.Ready))
                return false;

            var distance = target.Distance(ObjectManager.Player);

            return !(distance >= fleeRange);
        }

        /// <summary>
        /// Note: does NOT check the menu options
        /// </summary>
        /// <param name="target"></param>
        public static void UseIgnite(AIHeroClient target)
        {
            if (_ignite == null || _ignite.GetState() != SpellState.Ready) return;
            _igniteSpell.Cast(target);
            LastIgniteTarget = target;
            LastIgniteTime = Game.Time;
        }

        public static bool CanBeUsed()
        {
            return _igniteSpell != null && _igniteSpell.Instance.State == SpellState.Ready;
        }

        public static bool HasIgnite()
        {
            return ObjectManager.Player.Spellbook.Spells.Any(spell => spell.Name == "summonerdot");
        }

    }
}
