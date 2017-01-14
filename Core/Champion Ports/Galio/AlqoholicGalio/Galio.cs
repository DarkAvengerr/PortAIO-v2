// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Galio.cs" company="LeagueSharp">
//   Copyright (C) 2016 LeagueSharp
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
namespace AlqoholicGalio
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Managers;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SPrediction;

    using OrbwalkingMode = LeagueSharp.Common.Orbwalking.OrbwalkingMode;

    #endregion

    /// <summary>
    ///     Galio Class
    /// </summary>
    internal class Galio
    {
        #region Constants

        private const float QRadius = 235f;

        #endregion

        #region Static Fields

        public static List<Tuple<AIHeroClient, float, bool>> ChampionDamage = new List<Tuple<AIHeroClient, float, bool>>();

        private static HitChance hitChance;

        private static int predictionMode;

        private static readonly List<AIHeroClient> EnemyList = HeroManager.Enemies.ToList();

        #endregion

        #region Public Methods and Operators

        public static void Cast(Spell spell, Obj_AI_Base target)
        {
            predictionMode = MenuManager.PredictionMode;
            hitChance = HitChance.Low;

            switch (MenuManager.HitChance)
            {
                case 0:
                    hitChance = HitChance.Low;
                    break;
                case 1:
                    hitChance = HitChance.Medium;
                    break;
                case 2:
                    hitChance = HitChance.High;
                    break;
                case 3:
                    hitChance = HitChance.VeryHigh;
                    break;
            }

            switch (predictionMode)
            {
                case 0: //OKTW
                    var coreType = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                    var aoe = false;

                    if (spell.Type == SkillshotType.SkillshotCircle)
                    {
                        coreType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                        aoe = true;
                    }

                    if (spell.Width > 80 && !spell.Collision)
                    {
                        aoe = true;
                    }

                    var predInput = new SebbyLib.Prediction.PredictionInput
                                        {
                                            Aoe = aoe, Collision = spell.Collision, Speed = spell.Speed,
                                            Delay = spell.Delay, Range = spell.Range,
                                            From = ObjectManager.Player.ServerPosition, Radius = spell.Width,
                                            Unit = target, Type = coreType
                                        };

                    var poutput = SebbyLib.Prediction.Prediction.GetPrediction(predInput);

                    switch (hitChance)
                    {
                        case HitChance.VeryHigh:
                            if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                            {
                                spell.Cast(poutput.CastPosition);
                                Console.WriteLine("OKTW: Casting Spell {0} - {1}", spell.Slot, poutput.CastPosition);
                                Console.WriteLine("With Hitchance of: {0}", hitChance);
                            }
                            else if (predInput.Aoe && poutput.AoeTargetsHitCount > 1
                                     && poutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                            {
                                spell.Cast(poutput.CastPosition);
                                Console.WriteLine("OKTW: Casting Spell {0} - {1}", spell.Slot, poutput.CastPosition);
                                Console.WriteLine("With Hitchance of: {0}", hitChance);
                            }
                            break;
                        case HitChance.High:
                            if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                            {
                                spell.Cast(poutput.CastPosition);
                                Console.WriteLine("OKTW: Casting Spell {0} - {1}", spell.Slot, poutput.CastPosition);
                                Console.WriteLine("With Hitchance of: {0}", hitChance);
                            }
                            break;
                        case HitChance.Medium:
                            if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                            {
                                spell.Cast(poutput.CastPosition);
                                Console.WriteLine("OKTW: Casting Spell {0} - {1}", spell.Slot, poutput.CastPosition);
                                Console.WriteLine("With Hitchance of: {0}", hitChance);
                            }
                            break;
                    }

                    break;

                case 1: //SPrediction
                    var hero = target as AIHeroClient;
                    if (hero != null && hero.IsValid)
                    {
                        var t = hero;
                        spell.SPredictionCast(t, hitChance);
                        Console.WriteLine("SPrediction: Casting Spell {0} - {1}", spell.Slot, target);
                        Console.WriteLine("With Hitchance of: {0}", hitChance);
                    }
                    else
                    {
                        spell.CastIfHitchanceEquals(target, HitChance.High);
                        Console.WriteLine("SPrediction: Casting Spell {0} - {1}", spell.Slot, target);
                        Console.WriteLine("With Hitchance of: {0}", hitChance);
                    }
                    break;

                case 2: //Common
                    spell.CastIfHitchanceEquals(target, hitChance);
                    Console.WriteLine("Common: Casting Spell {0} - {1}", spell.Slot, target);
                    Console.WriteLine("With Hitchance of: {0}", hitChance);
                    break;
            }
        }

        /// <summary>
        ///     Initialises all Components
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("Galio Loaded");
            MenuManager.Init();
            DrawingManager.Init();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += DrawingManager.OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            foreach (var enemy in EnemyList.Where(enemy => enemy != null))
            {
                ChampionDamage.Add(new Tuple<AIHeroClient, float, bool>(enemy, -1, false));
            }

            if (MenuManager.PredictionMode == 1)
            {
                SPrediction.Prediction.Initialize(MenuManager.Menu);
            }
            else if (MenuManager.PredictionMode == 0)
            {
                MenuManager.Menu.AddItem(new MenuItem("OKTWLOADED", "OKTW LOADED"));
            }
            else if (MenuManager.PredictionMode == 2)
            {
                MenuManager.Menu.AddItem(new MenuItem("COMMONLOADED", "COMMON LOADED"));
            }
        }

        #endregion

        #region Methods

        private static void AutoHarass()
        {
            if (LeagueSharp.Common.Utility.CountEnemiesInRange(SpellManager.Q.Range) < 1)
            {
                return;
            }

            ChampionDamage = ChampionDamage.OrderBy(t => t.Item3).ThenBy(t => t.Item2).ToList();

            foreach (var champion in ChampionDamage.Where(x => x.Item3))
            {
                if (!MenuManager.AutoHarass || !SpellManager.Q.IsReady()
                    || !(ObjectManager.Player.ManaPercent >= MenuManager.HarassMana) || champion.Item3 == false)
                {
                    return;
                }
                Cast(SpellManager.Q, champion.Item1);
            }
        }

        /// <summary>
        ///     Combo Method
        /// </summary>
        private static void Combo()
        {
            var qTarget = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, TargetSelector.DamageType.Magical);
            var rTargets = LeagueSharp.Common.Utility.CountEnemiesInRange(SpellManager.R.Range - 80f);

            if (qTarget == null)
            {
                return;
            }
            while (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Surpressed)
            {
                MenuManager.Orbwalker.ActiveMode = OrbwalkingMode.None;
            }
            if (MenuManager.ComboMenu.Item("comboQ").GetValue<bool>() && SpellManager.Q.IsReady())
            {
                Cast(SpellManager.Q, qTarget);
            }

            if (MenuManager.ComboMenu.Item("comboE").GetValue<bool>() && SpellManager.E.IsReady())
            {
                Cast(SpellManager.E, eTarget);
            }
            if (MenuManager.ComboMenu.Item("comboR").GetValue<bool>() && SpellManager.R.IsReady()
                && rTargets >= MenuManager.RAmount)
            {
                if (SpellManager.W.IsReady())
                {
                    SpellManager.W.Cast(ObjectManager.Player);
                }
                else
                {
                    SpellManager.R.Cast();
                }
            }
        }

        /// <summary>
        ///     Escape Method
        /// </summary>
        private static void Escape()
        {
            var qTarget = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
            var prediction = SpellManager.Q.GetPrediction(qTarget);

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (MenuManager.EscapeMenu.Item("escapeE").GetValue<bool>())
            {
                SpellManager.E.Cast(Game.CursorPos);
            }

            if (MenuManager.EscapeMenu.Item("escapeQ").GetValue<bool>() && prediction.Hitchance >= HitChance.High)
            {
                Cast(SpellManager.Q, qTarget);
            }
        }

        /// <summary>
        ///     Every time the game updates, this event is called
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(EventArgs args)
        {
            UpdateList();

            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            switch (MenuManager.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case OrbwalkingMode.None:
                    break;
                default:
                    Escape();
                    AutoHarass();
                    break;
            }

            if (MenuManager.EscapeMenu.Item("escapeKey").GetValue<KeyBind>().Active)
            {
                Escape();
            }
        }

        /// <summary>
        ///     Auto Harass Method
        /// </summary>
        private static void Harass()
        {
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, TargetSelector.DamageType.Magical);

            var qTarget = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);

            if (ObjectManager.Player.ManaPercent <= MenuManager.HarassMenu.Item("harassMana").GetValue<Slider>().Value)
            {
                return;
            }

            if (qTarget == null || eTarget == null)
            {
                return;
            }

            if (MenuManager.HarassMenu.Item("harassQ").GetValue<bool>() && SpellManager.Q.IsReady())
            {
                Cast(SpellManager.Q, qTarget);
            }

            if (MenuManager.HarassMenu.Item("harassE").GetValue<bool>() && SpellManager.E.IsReady())
            {
                Cast(SpellManager.E, eTarget);
            }
        }

        /// <summary>
        ///     Lane Clear Method
        /// </summary>
        private static void LaneClear()
        {
            var eMinions = MinionManager.GetMinions(ObjectManager.Player.Position, SpellManager.E.Range);
            var qMinions = MinionManager.GetMinions(ObjectManager.Player.Position, SpellManager.Q.Range);

            var farmPosQ = SpellManager.Q.GetCircularFarmLocation(qMinions, QRadius);
            var farmPosE = SpellManager.E.GetLineFarmLocation(eMinions, QRadius);

            var playerMana = ObjectManager.Player.ManaPercent;
            var laneClearMana = MenuManager.LaneClearMenu.Item("laneClearMana").GetValue<Slider>().Value;

            var minionsHit = new List<int> { -1, -1 };

            if (farmPosQ.MinionsHit < MenuManager.FarmMinions && farmPosE.MinionsHit < MenuManager.FarmMinions
                || eMinions == null || qMinions == null)
            {
                return;
            }

            if (MenuManager.LaneClearMenu.Item("laneClearQ").GetValue<bool>() && SpellManager.Q.IsReady())
            {
                minionsHit[0] = farmPosQ.MinionsHit;
            }
            else
            {
                minionsHit[0] = -1;
            }

            if (MenuManager.LaneClearMenu.Item("laneClearE").GetValue<bool>() && SpellManager.E.IsReady())
            {
                minionsHit[1] = farmPosE.MinionsHit;
            }
            else
            {
                minionsHit[1] = -1;
            }

            var index = minionsHit.IndexOf(minionsHit.Max());

            switch (index)
            {
                case 0:
                    if (MenuManager.LaneClearMenu.Item("laneClearQ").GetValue<bool>() && SpellManager.Q.IsReady()
                        && (playerMana >= laneClearMana))
                    {
                        SpellManager.Q.Cast(farmPosQ.Position);
                    }
                    break;
                case 1:
                    if (MenuManager.LaneClearMenu.Item("laneClearE").GetValue<bool>() && SpellManager.E.IsReady()
                        && (playerMana >= laneClearMana))
                    {
                        SpellManager.E.Cast(farmPosE.Position);
                    }
                    break;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                {
                    return;
                }
                if (!MenuManager.WMenu.Item("wAuto").GetValue<bool>() || !SpellManager.W.IsReady())
                {
                    return;
                }
                if (!sender.IsEnemy)
                {
                    return;
                }
                var wTarget =
                    HeroManager.Allies.ToList()
                        .Where(ally => ally.Distance(ObjectManager.Player.Position) <= SpellManager.W.Range)
                        .OrderByDescending(ally => ally.HealthPercent)
                        .FirstOrDefault();

                if (wTarget != null
                    && (((AIHeroClient)args.Target).Health / sender.GetSpellDamage(wTarget, args.SData.Name) * 100)
                    > MenuManager.WMenu.Item("wIncDamage").GetValue<Slider>().Value)
                {
                    SpellManager.W.Cast(wTarget);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void UpdateList()
        {
            for (var i = 0; i < EnemyList.Count; i++)
            {
                ChampionDamage[i] = new Tuple<AIHeroClient, float, bool>(
                    EnemyList[i],
                    SpellManager.Q.GetDamage(EnemyList[i]),
                    false);
                if (Vector3.Distance(ChampionDamage[i].Item1.Position, ObjectManager.Player.Position)
                    <= SpellManager.Q.Range)
                {
                    ChampionDamage[i] = new Tuple<AIHeroClient, float, bool>(
                        EnemyList[i],
                        SpellManager.Q.GetDamage(EnemyList[i]),
                        true);
                }
            }
        }

        #endregion
    }
}