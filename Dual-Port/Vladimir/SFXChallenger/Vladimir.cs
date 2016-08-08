#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 Vladimir.cs is part of SFXChallenger.

 SFXChallenger is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXChallenger is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXChallenger. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion License

#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SFXChallenger.Abstracts;
using SFXChallenger.Args;
using SFXChallenger.Enumerations;
using SFXChallenger.Helpers;
using SFXChallenger.Library;
using SFXChallenger.Library.Logger;
using SFXChallenger.Managers;
using MinionManager = SFXChallenger.Library.MinionManager;
using MinionOrderTypes = SFXChallenger.Library.MinionOrderTypes;
using MinionTeam = SFXChallenger.Library.MinionTeam;
using MinionTypes = SFXChallenger.Library.MinionTypes;
using Orbwalking = SFXChallenger.SFXTargetSelector.Orbwalking;
using Spell = SFXChallenger.Wrappers.Spell;
using TargetSelector = SFXChallenger.SFXTargetSelector.TargetSelector;
using Utils = SFXChallenger.Helpers.Utils;

#endregion

using EloBuddy; namespace SFXChallenger.Champions
{
    internal class Vladimir : Abstracts.Champion
    {
        private MenuItem _eStacks;
        private Obj_AI_Minion _lastAaMinion;
        private float _lastAaMinionEndTime;

        protected override ItemFlags ItemFlags
        {
            get { return ItemFlags.Offensive | ItemFlags.Defensive | ItemFlags.Flee; }
        }

        protected override ItemUsageType ItemUsage
        {
            get { return ItemUsageType.Custom; }
        }

        protected override void OnLoad()
        {
            GapcloserManager.OnGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += OnDrawingDraw;
            Orbwalking.OnNonKillableMinion += OnOrbwalkingNonKillableMinion;
            Orbwalking.AfterAttack += OnOrbwalkingAfterAttack;
        }

        protected override void SetupSpells()
        {
            Q = new Spell(SpellSlot.Q, 600f, DamageType.Magical);
            Q.Range += GameObjects.EnemyHeroes.Select(e => e.BoundingRadius).DefaultIfEmpty(50).Min();
            Q.SetTargetted(0.25f, Q.Instance.SData.MissileSpeed);

            W = new Spell(SpellSlot.W, 175f, DamageType.Magical);

            E = new Spell(SpellSlot.E, 600f, DamageType.Magical) { Delay = 0.25f, Width = 600f };

            R = new Spell(SpellSlot.R, 700f, DamageType.Magical);
            R.SetSkillshot(0.25f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ultimate = new UltimateManager
            {
                Combo = true,
                Assisted = true,
                Auto = true,
                Flash = false,
                Required = true,
                Force = true,
                Gapcloser = false,
                GapcloserDelay = false,
                Interrupt = false,
                InterruptDelay = false,
                Spells = Spells,
                DamageCalculation =
                    (hero, resMulti, rangeCheck) =>
                        CalcComboDamage(
                            hero, rangeCheck, Menu.Item(Menu.Name + ".combo.q").GetValue<bool>(),
                            Menu.Item(Menu.Name + ".combo.e").GetValue<bool>(), true)
            };
        }

        protected override void AddToMenu()
        {
            Ultimate.AddToMenu(Menu);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", Menu.Name + ".combo"));
            ResourceManager.AddToMenu(
                comboMenu,
                new ResourceManagerArgs(
                    "combo-e", ResourceType.Health, ResourceValueType.Percent, ResourceCheckType.Minimum)
                {
                    Prefix = "E",
                    DefaultValue = 0
                });
            comboMenu.AddItem(new MenuItem(comboMenu.Name + ".q", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem(comboMenu.Name + ".e", "Use E").SetValue(true));

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", Menu.Name + ".harass"));
            ResourceManager.AddToMenu(
                harassMenu,
                new ResourceManagerArgs(
                    "harass-e", ResourceType.Health, ResourceValueType.Percent, ResourceCheckType.Minimum)
                {
                    Prefix = "E",
                    DefaultValue = 30
                });
            harassMenu.AddItem(new MenuItem(harassMenu.Name + ".q", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem(harassMenu.Name + ".e", "Use E").SetValue(true));

            var laneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", Menu.Name + ".lane-clear"));
            ResourceManager.AddToMenu(
                laneClearMenu,
                new ResourceManagerArgs(
                    "lane-clear-e", ResourceType.Health, ResourceValueType.Percent, ResourceCheckType.Minimum)
                {
                    Prefix = "E",
                    DefaultValue = 45
                });
            laneClearMenu.AddItem(new MenuItem(laneClearMenu.Name + ".q", "Use Q").SetValue(true));
            laneClearMenu.AddItem(new MenuItem(laneClearMenu.Name + ".e", "Use E").SetValue(true));
            laneClearMenu.AddItem(new MenuItem(laneClearMenu.Name + ".e-min", "E Min.").SetValue(new Slider(3, 1, 5)));

            var jungleClear = Menu.AddSubMenu(new Menu("Jungle Clear", Menu.Name + ".jungle-clear"));
            ResourceManager.AddToMenu(
                jungleClear,
                new ResourceManagerArgs(
                    "jungle-clear-e", ResourceType.Health, ResourceValueType.Percent, ResourceCheckType.Minimum)
                {
                    Prefix = "E",
                    DefaultValue = 25
                });
            jungleClear.AddItem(new MenuItem(jungleClear.Name + ".q", "Use Q").SetValue(true));
            jungleClear.AddItem(new MenuItem(jungleClear.Name + ".e", "Use E").SetValue(true));

            var lasthitMenu = Menu.AddSubMenu(new Menu("Last Hit", Menu.Name + ".lasthit"));
            lasthitMenu.AddItem(new MenuItem(lasthitMenu.Name + ".q", "Use Q").SetValue(true));
            lasthitMenu.AddItem(new MenuItem(lasthitMenu.Name + ".q-unkillable", "Q Unkillable").SetValue(true));

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", Menu.Name + ".flee"));
            fleeMenu.AddItem(new MenuItem(fleeMenu.Name + ".q", "Use Q").SetValue(true));

            var killstealMenu = Menu.AddSubMenu(new Menu("Killsteal", Menu.Name + ".killsteal"));
            killstealMenu.AddItem(new MenuItem(killstealMenu.Name + ".q", "Use Q").SetValue(true));

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", Menu.Name + ".miscellaneous"));

            var wGapcloserMenu = miscMenu.AddSubMenu(new Menu("W Gapcloser", miscMenu.Name + "w-gapcloser"));
            GapcloserManager.AddToMenu(
                wGapcloserMenu,
                new HeroListManagerArgs("w-gapcloser")
                {
                    IsWhitelist = false,
                    Allies = false,
                    Enemies = true,
                    DefaultValue = false,
                    Enabled = false
                }, true);
            BestTargetOnlyManager.AddToMenu(wGapcloserMenu, "w-gapcloser");

            ResourceManager.AddToMenu(
                miscMenu,
                new ResourceManagerArgs(
                    "auto-e", ResourceType.Health, ResourceValueType.Percent, ResourceCheckType.Minimum)
                {
                    Prefix = "E",
                    DefaultValue = 65
                });

            miscMenu.AddItem(new MenuItem(miscMenu.Name + ".e-auto", "Auto E Stacking").SetValue(false));

            IndicatorManager.AddToMenu(DrawingManager.Menu, true);
            IndicatorManager.Add(Q);
            IndicatorManager.Add(E);
            IndicatorManager.Add(R);
            IndicatorManager.Finale();

            _eStacks = DrawingManager.Add("E Stacks", true);
        }

        private void OnOrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            try
            {
                if (unit.IsMe)
                {
                    var minion = target as Obj_AI_Minion;
                    if (minion != null)
                    {
                        _lastAaMinion = minion;
                        _lastAaMinionEndTime = Game.Time + minion.LSDistance(Player) / Orbwalking.GetMyProjectileSpeed() +
                                               0.25f;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnOrbwalkingNonKillableMinion(AttackableUnit unit)
        {
            try
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (!Player.Spellbook.IsAutoAttacking && Menu.Item(Menu.Name + ".lasthit.q-unkillable").GetValue<bool>() &&
                        Q.LSIsReady() && Q.IsInRange(unit))
                    {
                        var target = unit as Obj_AI_Base;
                        if (target != null &&
                            (_lastAaMinion == null || target.NetworkId != _lastAaMinion.NetworkId ||
                             Game.Time > _lastAaMinionEndTime) &&
                            HealthPrediction.GetHealthPrediction(target, (int) (Q.Delay * 1000f)) > 0)
                        {
                            Q.CastOnUnit(target);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnEnemyGapcloser(object sender, GapcloserManagerArgs args)
        {
            try
            {
                if (args.UniqueId.Equals("w-gapcloser") && W.LSIsReady() &&
                    BestTargetOnlyManager.Check("w-gapcloser", W, args.Hero))
                {
                    if (args.End.LSDistance(Player.Position) <= W.Range * 0.9f)
                    {
                        W.Cast(args.End);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        protected override void OnPreUpdate() {}

        protected override void OnPostUpdate()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && !Player.Spellbook.IsAutoAttacking &&
                Menu.Item(Menu.Name + ".lasthit.q").GetValue<bool>() && Q.LSIsReady())
            {
                var m =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(
                            e =>
                                e.HealthPercent <= 75 &&
                                (_lastAaMinion == null || e.NetworkId != _lastAaMinion.NetworkId ||
                                 Game.Time > _lastAaMinionEndTime) &&
                                HealthPrediction.GetHealthPrediction(e, (int) (Q.Delay * 1000f)) < Q.GetDamage(e));
                if (m != null)
                {
                    Casting.TargetSkill(m, Q);
                }
            }

            if (Ultimate.IsActive(UltimateModeType.Assisted) && R.LSIsReady())
            {
                if (Ultimate.ShouldMove(UltimateModeType.Assisted))
                {
                    Orbwalking.MoveTo(Game.CursorPos, Orbwalker.HoldAreaRadius);
                }
                var target = TargetSelector.GetTarget(R);
                if (target != null && !RLogic(UltimateModeType.Assisted, target))
                {
                    RLogicSingle(UltimateModeType.Assisted);
                }
            }

            if (Ultimate.IsActive(UltimateModeType.Auto) && R.LSIsReady())
            {
                var target = TargetSelector.GetTarget(R);
                if (target != null && !RLogic(UltimateModeType.Auto, target))
                {
                    RLogicSingle(UltimateModeType.Auto);
                }
            }

            if (Menu.Item(Menu.Name + ".miscellaneous.e-auto").GetValue<bool>() && E.LSIsReady() &&
                ResourceManager.Check("auto-e") && !Player.LSIsRecalling() && !Player.LSInFountain())
            {
                var buff = GetEBuff();
                if (buff == null || buff.EndTime - Game.Time <= Game.Ping / 2000f + 0.5f)
                {
                    E.Cast();
                }
            }
        }

        private BuffInstance GetEBuff()
        {
            try
            {
                return
                    Player.Buffs.FirstOrDefault(
                        b => b.Name.Equals("vladimirtidesofbloodcost", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
            return null;
        }

        private Tuple<int, List<AIHeroClient>> GetEHits()
        {
            try
            {
                var hits =
                    GameObjects.EnemyHeroes.Where(
                        e =>
                            e.LSIsValidTarget() && e.LSDistance(Player) < E.Width * 0.8f ||
                            e.LSDistance(Player) < E.Width && e.LSIsFacing(Player)).ToList();
                return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
            return new Tuple<int, List<AIHeroClient>>(0, null);
        }

        protected override void Combo()
        {
            var single = false;
            var q = Menu.Item(Menu.Name + ".combo.q").GetValue<bool>() && Q.LSIsReady();
            var e = Menu.Item(Menu.Name + ".combo.e").GetValue<bool>() && E.LSIsReady() &&
                    ResourceManager.Check("combo-e");
            var r = Ultimate.IsActive(UltimateModeType.Combo) && R.LSIsReady();

            var rTarget = TargetSelector.GetTarget(R);
            if (r)
            {
                if (!RLogic(UltimateModeType.Combo, rTarget))
                {
                    RLogicSingle(UltimateModeType.Combo);
                    single = true;
                }
            }
            if (q)
            {
                Casting.TargetSkill(Q);
            }
            if (e)
            {
                if (GetEHits().Item1 > 0)
                {
                    E.Cast();
                }
            }

            ItemsSummonersLogic(rTarget, single);
        }

        protected override void Harass()
        {
            var q = Menu.Item(Menu.Name + ".harass.q").GetValue<bool>() && Q.LSIsReady();
            var e = Menu.Item(Menu.Name + ".harass.e").GetValue<bool>() && E.LSIsReady() &&
                    ResourceManager.Check("harass-e");

            if (q)
            {
                var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                foreach (var minion in from minion in minions
                    let damage = Q.GetDamage(minion)
                    where
                        minion.HealthPercent <= 75 &&
                        HealthPrediction.GetHealthPrediction(minion, (int) (Q.Delay * 1000f)) < damage ||
                        damage > minion.Health * 1.75f
                    select minion)
                {
                    Casting.TargetSkill(minion, Q);
                    break;
                }
            }
            if (e)
            {
                if (GetEHits().Item1 > 0)
                {
                    E.Cast();
                }
            }
        }

        private float CalcComboDamage(AIHeroClient target, bool rangeCheck, bool q, bool e, bool r)
        {
            try
            {
                if (target == null)
                {
                    return 0;
                }
                float damage = 0;
                if (q && (!rangeCheck || Q.IsInRange(target)))
                {
                    damage += Q.GetDamage(target) * 2;
                }
                if (e && (!rangeCheck || E.IsInRange(target)))
                {
                    damage += E.GetDamage(target) * 2;
                }
                if (r && R.LSIsReady() && (!rangeCheck || R.IsInRange(target, R.Range + R.Width)))
                {
                    damage *= 1.2f;
                    damage += R.GetDamage(target);
                }
                damage *= 1.1f;
                damage += ItemManager.CalculateComboDamage(target, rangeCheck);
                damage += SummonerManager.CalculateComboDamage(target, rangeCheck);
                return damage;
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
            return 0;
        }

        private bool RLogic(UltimateModeType mode, AIHeroClient target)
        {
            try
            {
                if (Ultimate.IsActive(mode))
                {
                    var pred = CPrediction.Circle(R, target, HitChance.High, false);
                    if (pred.TotalHits > 0 && Ultimate.Check(mode, pred.Hits))
                    {
                        R.Cast(pred.CastPosition);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
            return false;
        }

        private void RLogicSingle(UltimateModeType mode)
        {
            try
            {
                if (Ultimate.ShouldSingle(mode))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(t => Ultimate.CheckSingle(mode, t)))
                    {
                        var pred = CPrediction.Circle(R, target, HitChance.High, false);
                        if (pred.TotalHits > 0)
                        {
                            R.Cast(pred.CastPosition);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        protected override void LaneClear()
        {
            var q = Menu.Item(Menu.Name + ".lane-clear.q").GetValue<bool>() && Q.LSIsReady();
            var e = Menu.Item(Menu.Name + ".lane-clear.e").GetValue<bool>() && E.LSIsReady() &&
                    ResourceManager.Check("lane-clear-e");

            if (q)
            {
                Casting.Farm(Q, MinionManager.GetMinions(Q.Range), 1);
            }
            if (e)
            {
                Casting.FarmSelfAoe(
                    E, MinionManager.GetMinions(E.Range),
                    Menu.Item(Menu.Name + ".lane-clear.e-min").GetValue<Slider>().Value);
            }
        }

        protected override void JungleClear()
        {
            var q = Menu.Item(Menu.Name + ".jungle-clear.q").GetValue<bool>() && Q.LSIsReady();
            var e = Menu.Item(Menu.Name + ".jungle-clear.e").GetValue<bool>() && E.LSIsReady() &&
                    ResourceManager.Check("jungle-clear-e");

            if (q)
            {
                Casting.Farm(
                    Q,
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth),
                    1);
            }
            if (e)
            {
                Casting.FarmSelfAoe(
                    E,
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth),
                    1);
            }
        }

        protected override void Flee()
        {
            if (Menu.Item(Menu.Name + ".flee.q").GetValue<bool>() && Q.LSIsReady())
            {
                var target =
                    GameObjects.EnemyHeroes.Select(e => e as Obj_AI_Base)
                        .Concat(GameObjects.EnemyMinions)
                        .Where(e => e.LSIsValidTarget(Q.Range))
                        .OrderBy(e => e is AIHeroClient)
                        .FirstOrDefault();
                if (target != null)
                {
                    Casting.TargetSkill(target, Q);
                }
            }
        }

        protected override void Killsteal()
        {
            if (Menu.Item(Menu.Name + ".killsteal.q").GetValue<bool>() && Q.LSIsReady())
            {
                var target = GameObjects.EnemyHeroes.FirstOrDefault(e => e.LSIsValidTarget(Q.Range) && Q.IsKillable(e));
                if (target != null)
                {
                    Casting.TargetSkill(target, Q);
                }
            }
        }

        private void OnDrawingDraw(EventArgs args)
        {
            try
            {
                if (!Utils.ShouldDraw(true))
                {
                    return;
                }
                if (E.Level > 0 && _eStacks != null && _eStacks.GetValue<bool>())
                {
                    var buff = GetEBuff();
                    var stacks = buff != null ? buff.Count - 1 : -1;
                    if (stacks > -1)
                    {
                        var x = Player.HPBarPosition.X + 40;
                        var y = Player.HPBarPosition.Y - 25;
                        for (var i = 0; 4 > i; i++)
                        {
                            Drawing.DrawLine(
                                x + i * 20, y, x + i * 20 + 10, y, 10, i > stacks ? Color.DarkGray : Color.Orange);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }
    }
}