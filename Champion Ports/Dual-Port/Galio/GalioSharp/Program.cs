// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeagueSharp">
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
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GalioSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     The name of our champion.
        /// </summary>
        private const string ChampionName = "Galio";

        /// <summary>
        ///     The incoming damage dictionary.
        /// </summary>
        private static readonly Dictionary<float, float> IncomingDamage = new Dictionary<float, float>();

        /// <summary>
        ///     The player.
        /// </summary>
        private static AIHeroClient player;

        /// <summary>
        ///     The config.
        /// </summary>
        private static Menu config;

        /// <summary>
        ///     The orbwalker.
        /// </summary>
        private static Orbwalking.Orbwalker orbwalker;

        /// <summary>
        ///     The q spell.
        /// </summary>
        private static Spell q;

        /// <summary>
        ///     The w spell.
        /// </summary>
        private static Spell w;

        /// <summary>
        ///     The e spell.
        /// </summary>
        private static Spell e;

        /// <summary>
        ///     The r spell.
        /// </summary>
        private static Spell r;

        /// <summary>
        ///     The ignite slot.
        /// </summary>
        private static SpellSlot igniteSlot;

        /// <summary>
        ///     The flash slot.
        /// </summary>
        private static SpellSlot flashSlot;

        /// <summary>
        ///     Gets the incoming damage.
        /// </summary>
        public static float IncomingDamageSum
        {
            get
            {
                return IncomingDamage.Sum(e => e.Value);
            }
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main()
        {
            Game_OnGameLoad();
        }

        /// <summary>
        /// The game_ on game load.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnGameLoad()
        {
            player = ObjectManager.Player;
            if (player.ChampionName != ChampionName)
            {
                return;
            }

            q = new Spell(SpellSlot.Q, 900f);
            q.SetSkillshot(0.25f, 200f, 1300f, false, SkillshotType.SkillshotCircle);
            w = new Spell(SpellSlot.W, 800f);
            w.SetTargetted(0.25f, 20f);
            e = new Spell(SpellSlot.E, 1200f);
            e.SetSkillshot(0.25f, 120f, 1200f, false, SkillshotType.SkillshotLine);
            r = new Spell(SpellSlot.R, 600f);

            igniteSlot = player.GetSpellSlot("SummonerDot");
            flashSlot = player.GetSpellSlot("SummonerFlash");

            config = new Menu(ChampionName, ChampionName, true);
            TargetSelector.AddToMenu(config.SubMenu("Target Selector"));
            orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalking"));

            config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("UseRNCombo", "Use R on at least").SetValue(
                        new StringList(new[] { "1 target", "2 target", "3 target", "4 target", "5 target" })));
            config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "Combo!").SetValue(
                        new KeyBind(config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            config.SubMenu("Ultimate").AddItem(new MenuItem("UseAutoUlt", "Auto Ult").SetValue(true));
            config.SubMenu("Ultimate")
                .AddItem(
                    new MenuItem("UseRNAuto", "Min Targets").SetValue(
                        new StringList(new[] { "2 target", "3 target", "4 target", "5 target" })));
            config.SubMenu("Ultimate").AddItem(new MenuItem("FlashUlt", "Flash Ult").SetValue(true));

            config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            config.SubMenu("Harass")
                .AddItem(new MenuItem("HarassManaCheck", "Don't harass if mana < %").SetValue(new Slider(80)));
            config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "Harass!").SetValue(
                        new KeyBind(config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "Harass (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            config.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
            config.SubMenu("Farm").AddItem(new MenuItem("UseEFarm", "Use E").SetValue(true));
            config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("FarmActive", "Farm!").SetValue(
                        new KeyBind(config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "Use Q").SetValue(true));
            config.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "Use E").SetValue(true));
            config.SubMenu("JungleFarm")
                .AddItem(
                    new MenuItem("JungleFarmActive", "JungleFarm!").SetValue(
                        new KeyBind(config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            config.SubMenu("Misc")
                .AddItem(new MenuItem("InterruptSpellsR", "Interrupt dangerous spells using R").SetValue(true));
            config.SubMenu("Misc").AddItem(new MenuItem("AutoW", "Block damage with W").SetValue(true));

            config.AddToMainMenu();

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            //Obj_AI_Base.OnAggro += Obj_AI_Turret_OnAggro;
            Game.OnUpdate += Game_OnUpdate;
        }

        /// <summary>
        /// The ObjAITurret event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Obj_AI_Turret_OnAggro(Obj_AI_Base sender, Obj_AI_Base args)
        {
            if (!w.IsReady())
            {
                return;
            }

            foreach (
                var hero in
                    HeroManager.Allies.Where(
                        hero => args.NetworkId == hero.NetworkId && player.Distance(hero, true) < w.RangeSqr))
            {
                w.Cast(hero);
            }
        }

        /// <summary>
        /// The Game Update event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (config.SubMenu("Misc").Item("AutoW").GetValue<bool>())
            {
                AutoW();
            }

            if (config.SubMenu("Combo").Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (config.SubMenu("Harass").Item("HarassActive").GetValue<KeyBind>().Active
                || (config.SubMenu("Harass").Item("HarassActiveT").GetValue<KeyBind>().Active
                    && !player.HasBuff("Recall")))
            {
                Harass();
            }

            if (config.SubMenu("Farm").Item("FarmActive").GetValue<KeyBind>().Active)
            {
                Farm();
            }

            if (config.SubMenu("JungleFarm").Item("JungleFarmActive").GetValue<KeyBind>().Active)
            {
                JungleFarm();
            }

            if (config.SubMenu("Ultimate").Item("UseAutoUlt").GetValue<bool>())
            {
                AutoUlt(config.SubMenu("Ultimate").Item("FlashUlt").GetValue<bool>());
            }
        }

        /// <summary>
        /// The Interrupter event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender, 
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (config.SubMenu("Misc").Item("InterruptSpellsR").GetValue<bool>()
                && args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                if (r.IsReady() && player.Distance(sender, true) < r.RangeSqr)
                {
                    r.Cast();
                }
            }
        }

        /// <summary>
        ///     The combo.
        /// </summary>
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(e.Range, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            var useQ = config.SubMenu("Combo").Item("UseQCombo").GetValue<bool>();
            var useE = config.SubMenu("Combo").Item("UseECombo").GetValue<bool>();
            var useR = config.SubMenu("Combo").Item("UseRCombo").GetValue<bool>();

            var minRTargets = config.SubMenu("Combo").Item("UseRNCombo").GetValue<StringList>().SelectedIndex + 1;

            if (Utility.CountEnemiesInRange(r.Range) <= 1)
            {
                if (useR && GetComboDamage(target) > target.Health && r.IsReady()
                    && (!q.IsReady() && q.IsKillable(target)) || (e.IsReady() && e.IsKillable(target)))
                {
                    r.Cast();
                }

                if (player.Spellbook.GetSpell(SpellSlot.R).State != SpellState.Surpressed)
                {
                    if (useQ && q.IsReady())
                    {
                        q.Cast(target);
                    }

                    if (useE && e.IsReady())
                    {
                        e.Cast(target);
                    }
                }
            }
            else
            {
                if (useR && r.IsReady())
                {
                    if (Utility.CountEnemiesInRange(r.Range) >= minRTargets)
                    {
                        r.Cast();
                    }
                }

                if (player.Spellbook.GetSpell(SpellSlot.R).State != SpellState.Surpressed)
                {
                    if (useQ && q.IsReady())
                    {
                        q.Cast(target);
                    }

                    if (useE && e.IsReady())
                    {
                        e.Cast(target);
                    }
                }
            }

            if (player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Cooldown)
            {
                if (useQ && q.IsReady())
                {
                    q.Cast(target);
                }

                if (useE && e.IsReady())
                {
                    e.Cast(target);
                }
            }

            if (player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
            {
                player.Spellbook.CastSpell(igniteSlot, target);
            }
        }

        /// <summary>
        ///     The harass.
        /// </summary>
        private static void Harass()
        {
            if (player.ManaPercent < config.SubMenu("Harass").Item("HarassManaCheck").GetValue<Slider>().Value)
            {
                return;
            }

            var target = TargetSelector.GetTarget(e.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (config.SubMenu("Harass").Item("UseQHarass").GetValue<bool>() && q.IsReady())
                {
                    q.CastIfHitchanceEquals(target, HitChance.High);
                    return;
                }

                if (config.SubMenu("Harass").Item("UseEHarass").GetValue<bool>() && e.IsReady())
                {
                    e.Cast(target);
                }
            }
        }

        /// <summary>
        ///     The jungle farm.
        /// </summary>
        private static void JungleFarm()
        {
            var useQ = config.SubMenu("JungleFarm").Item("UseQJFarm").GetValue<bool>();
            var useE = config.SubMenu("JungleFarm").Item("UseEJFarm").GetValue<bool>();

            var mobs = MinionManager.GetMinions(
                player.ServerPosition, 
                e.Range, 
                MinionTypes.All, 
                MinionTeam.Neutral, 
                MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && q.IsReady())
                {
                    q.Cast(mob.Position);
                }
                else if (useE && e.IsReady())
                {
                    e.Cast(mob.Position);
                }
            }
        }

        /// <summary>
        ///     The farm.
        /// </summary>
        private static void Farm()
        {
            var allMinions = MinionManager.GetMinions(player.ServerPosition, e.Range);
            var rangedMinions = MinionManager.GetMinions(player.ServerPosition, e.Range, MinionTypes.Ranged);

            var useQ = config.SubMenu("Farm").Item("UseQFarm").GetValue<bool>();
            var useE = config.SubMenu("Farm").Item("UseEFarm").GetValue<bool>();

            if (useQ && q.IsReady())
            {
                var qLocation = q.GetCircularFarmLocation(allMinions, q.Range);
                var q2Location = q.GetCircularFarmLocation(rangedMinions, q.Range);
                var bestLocation = (qLocation.MinionsHit > q2Location.MinionsHit + 1) ? qLocation : q2Location;

                if (bestLocation.MinionsHit >= 3)
                {
                    q.Cast(bestLocation.Position);
                }
            }

            if (useE && e.IsReady())
            {
                var eLocation = e.GetLineFarmLocation(allMinions, e.Range);
                var e2Location = e.GetLineFarmLocation(rangedMinions, e.Range);
                var bestLocation = (eLocation.MinionsHit > e2Location.MinionsHit + 1) ? eLocation : e2Location;

                if (bestLocation.MinionsHit >= 2)
                {
                    e.Cast(bestLocation.Position);
                }
            }
        }

        /// <summary>
        /// The Combo damage.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        private static float GetComboDamage(AIHeroClient target)
        {
            var result = 0f;
            if (q.IsReady())
            {
                result += q.GetDamage(target);
            }

            if (e.IsReady())
            {
                result += e.GetDamage(target);
            }

            if (r.IsReady())
            {
                result += r.GetDamage(target);
            }

            return result;
        }

        /// <summary>
        /// The auto w.
        /// </summary>
        private static void AutoW()
        {
            // Check spell arrival
            var itemsToRemove = IncomingDamage.Where(entry => entry.Key < Game.Time).ToArray();
            foreach (var item in itemsToRemove)
            {
                IncomingDamage.Remove(item.Key);
            }

            if (w.IsReady())
            {
                foreach (var hero in
                    HeroManager.Allies.Where(
                        hero =>
                        hero.Distance(player) < w.Range && (IncomingDamageSum > 100 || IncomingDamageSum > hero.Health))
                    )
                {
                    w.Cast(hero);
                }
            }
        }

        /// <summary>
        /// The get combinations.
        /// </summary>
        /// <param name="allValues">
        /// The all values.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static IEnumerable<List<Vector2>> GetCombinations(IReadOnlyCollection<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }

            return collection;
        }

        /// <summary>
        /// The automatic ult function.
        /// </summary>
        /// <param name="flashUlt">
        /// The boolean that defines if flash should be used.
        /// </param>
        private static void AutoUlt(bool flashUlt)
        {
            var minTargets = config.SubMenu("Ultimate").Item("UseRNAuto").GetValue<StringList>().SelectedIndex + 2;

            if (flashUlt && minTargets >= 2)
            {
                var heroPositions = HeroManager.Enemies.Select(hero => hero.Position.To2D()).ToList();

                var subGroups = GetCombinations(heroPositions);

                foreach (var subGroup in subGroups)
                {
                    if (subGroup.Count >= minTargets)
                    {
                        var circle = MEC.GetMec(subGroup);

                        if (r.IsReady() && flashSlot.IsReady() && circle.Center.Distance(player) <= 425
                            && circle.Radius <= r.Range)
                        {
                            player.Spellbook.CastSpell(flashSlot, circle.Center.To3D());
                            LeagueSharp.Common.Utility.DelayAction.Add(50, () => r.Cast());
                        }
                    }
                }
            }
            else
            {
                if (player.CountEnemiesInRange(r.Range) >= minTargets && r.IsReady())
                {
                    r.Cast();
                }
            }
        }

        /// <summary>
        /// The OnprocessSpellCast event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!config.SubMenu("Misc").Item("AutoW").GetValue<bool>() || !w.IsReady())
            {
                return;
            }

            if (sender.IsMe && args.SData.Name == "GalioIdolOfDurand")
            {
                w.Cast(player);
            }

            if (sender.IsEnemy)
            {
                foreach (var hero in HeroManager.Allies)
                {
                    if (sender is AIHeroClient && args.SData.IsAutoAttack() && args.Target == hero)
                    {
                        IncomingDamage.Add(
                            hero.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time, 
                            (float)sender.GetAutoAttackDamage(hero));
                    }

                    if (sender is AIHeroClient && args.Target == hero)
                    {
                        var attacker = sender as AIHeroClient;
                        var slot = attacker.GetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R))
                            {
                                IncomingDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(hero, slot));
                            }
                        }
                    }
                }
            }
        }
    }
}