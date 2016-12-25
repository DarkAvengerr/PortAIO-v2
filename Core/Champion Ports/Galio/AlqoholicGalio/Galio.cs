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

        private static HitChance hc;

        private static readonly List<AIHeroClient> EnemyList = HeroManager.Enemies.ToList();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initialises all Components
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("Galio Loaded");
            MenuManager.Init();
            UtilityManager.Init();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += UtilityManager.OnDraw;

            foreach (var enemy in EnemyList.Where(enemy => enemy != null))
            {
                ChampionDamage.Add(new Tuple<AIHeroClient, float, bool>(enemy, -1, false));
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
                SpellManager.Q.SPredictionCast(champion.Item1, hc, 0, 1, ObjectManager.Player.Position);
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
            if (MenuManager.ComboMenu.Item("comboR").GetValue<bool>() && SpellManager.R.IsReady()
                && rTargets >= MenuManager.RAmount)
            {
                if (SpellManager.W.IsReady())
                {
                    SpellManager.W.Cast(ObjectManager.Player);
                }
                SpellManager.R.Cast();
            }
            if (MenuManager.ComboMenu.Item("comboQ").GetValue<bool>() && SpellManager.Q.IsReady())
            {
                SpellManager.Q.SPredictionCast(qTarget, hc, 0, 1, ObjectManager.Player.Position);
            }

            if (MenuManager.ComboMenu.Item("comboE").GetValue<bool>() && SpellManager.E.IsReady())
            {
                SpellManager.E.SPredictionCast(eTarget, hc);
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
                SpellManager.Q.SPredictionCast(qTarget, hc, 0, 1, ObjectManager.Player.Position);
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

            switch (MenuManager.Prediction)
            {
                case 0:
                    hc = HitChance.Low;
                    break;
                case 1:
                    hc = HitChance.Medium;
                    break;
                case 2:
                    hc = HitChance.High;
                    break;
                case 3:
                    hc = HitChance.VeryHigh;
                    break;
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
                SpellManager.Q.SPredictionCast(qTarget, hc);
            }

            if (MenuManager.HarassMenu.Item("harassE").GetValue<bool>() && SpellManager.E.IsReady())
            {
                SpellManager.E.SPredictionCast(eTarget, hc);
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