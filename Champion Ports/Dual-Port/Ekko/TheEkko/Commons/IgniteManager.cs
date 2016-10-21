using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko.Commons
{
    public static class IgniteManager
    {

        private static MenuItem _igniteUsage, _igniteKillsteal, _igniteSituation, _igniteMaxAutoattacks, _igniteOnlyCombo, _igniteSpellsCooldown, _igniteCloseFight, _igniteCloseFightHealth;
        private static SpellDataInst _ignite;
        private static Spell _igniteSpell;

        /// <summary>
        /// Adds to the menu and stuffs
        /// </summary>
        public static void Initialize(Menu rootMenu, string menuName = "Ignite")
        {
            _ignite = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (_ignite == null) return;
            _igniteSpell = new Spell(_ignite.Slot);
            var igniteMenu = new Menu(menuName, menuName);
            rootMenu.AddSubMenu(igniteMenu);
            _igniteUsage = igniteMenu.AddMItem("Use Ignite", true);
            _igniteKillsteal = igniteMenu.AddMItem("Killsteal", false);
            _igniteOnlyCombo = igniteMenu.AddMItem("Only in combo", false);
            _igniteSituation = igniteMenu.AddMItem("Situation analysis", false);
            igniteMenu.AddMItem("Don't use if killable by");
            _igniteMaxAutoattacks = igniteMenu.AddMItem("X Autoattacks", new Slider(1, 0, 5));
            _igniteSpellsCooldown = igniteMenu.AddMItem("Don't use if skills up", false);
            _igniteCloseFight = igniteMenu.AddMItem("Ignore above in close fight", true);
            _igniteCloseFightHealth = igniteMenu.AddMItem("Close fight health diff %", new Slider(20));
        }

        /// <summary>
        /// Does it's checks and ignites if all conditions are met. Uses Menu settings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="target"></param>
        /// <param name="otherDamage">Damage that will hit for sure in the near future, e.g. brand passive</param>
        /// <param name="otherDamageTime">How much time will pass till the otherDamage applies (checks on healthreg ...)</param>
        public static void Update(IMainContext context, AIHeroClient target, float otherDamage = 0f, float otherDamageTime = 3f)
        {
            if (_ignite == null || !_igniteUsage.GetValue<bool>()) return;
            if (context.GetOrbwalker().ActiveMode != Orbwalking.OrbwalkingMode.Combo && _igniteOnlyCombo.GetValue<bool>()) return;

            if (_igniteKillsteal.GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies)
                    UpdateOnHero(enemy, otherDamage, otherDamageTime, _igniteKillsteal.GetValue<bool>());
            }
            else if (target != null)
                UpdateOnHero(target, otherDamage, otherDamageTime, _igniteKillsteal.GetValue<bool>());

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
            var distance = ObjectManager.Player.Distance(target);
            if (distance > 600) return;
            var enemyHealth = target.AllShield + target.Health;
            if (GetDamage() < enemyHealth + (target.HPRegenRate * 5)) return;

            if (_igniteSpellsCooldown.GetValue<bool>() && (ObjectManager.Player.GetSpell(SpellSlot.Q).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.W).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.E).State == SpellState.Ready || ObjectManager.Player.GetSpell(SpellSlot.R).State == SpellState.Ready))
                if (_igniteCloseFight.GetValue<bool>() && (ObjectManager.Player.HealthPercent - target.HealthPercent) < _igniteCloseFightHealth.GetValue<Slider>().Value)
                    return;



            //Console.WriteLine("could ignite");
            if (ks && enemyHealth < GetDamage() / 5f)
                UseIgnite(target);

            var fixedDamage = otherDamage + (target.Health - HealthPrediction.GetHealthPrediction(target, (int)otherDamageTime)) - target.HPRegenRate * otherDamageTime;
            if (distance < ObjectManager.Player.AttackRange)
                fixedDamage += (float)(_igniteMaxAutoattacks.GetValue<Slider>().Value * ObjectManager.Player.GetAutoAttackDamage(target));
            //Console.WriteLine("fixed dmg < enemy health: " + (fixedDamage < enemyHealth) + " " + fixedDamage + " " + enemyHealth + " situation " + (!_igniteSituation.GetValue<bool>() || !IsDeadForSure(target)) + " s2 " + IsDeadForSure(target));


            if (fixedDamage < enemyHealth && (!_igniteSituation.GetValue<bool>() || !IsDeadForSure(target)))
                UseIgnite(target);
        }

        public static float GetDamage()
        {
            return 50 + ObjectManager.Player.Level * 20;
        }

        /// <summary>
        /// Does some checks on the situation, like health, enemy flash up, enemy distance ...
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fleeRange"></param>
        /// <param name="dangerousHealthPercent"></param>
        /// <returns></returns>
        private static bool IsDeadForSure(AIHeroClient target, float fleeRange = 550, float dangerousHealthPercent = 0.25f) //Todo: make better
        {
            var myHealthPercent = ObjectManager.Player.Health + ObjectManager.Player.AllShield / ObjectManager.Player.MaxHealth;
            var enemyHealthPercent = target.Health + target.AllShield / target.MaxHealth;
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
            //Console.WriteLine("usiong ingote");
            if (_ignite == null || _ignite.State != SpellState.Ready) return;
            _igniteSpell.Cast(target);
        }

        public static bool CanBeUsed()
        {
            return _igniteSpell != null && _igniteSpell.Instance.State == SpellState.Ready;
        }

    }
}
