// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mechanics.cs" company="LeagueSharp">
//   Copyright (C) 2015 L33T
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
//   The <c>Ekko</c> Mechanics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_the_Boy_Who_Shattered_Time
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The <c>Ekko</c> Mechanics.
    /// </summary>
    public class Mechanics
    {
        #region Static Fields

        /// <summary>
        ///     Last Q Tick.
        /// </summary>
        private static int lastQCastTick;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the flee minion.
        /// </summary>
        public static Obj_AI_Minion FleeMinion { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        private static AIHeroClient Player
        {
            get
            {
                return Ekko.Player;
            }
        }

        /// <summary>
        ///     Gets the spells.
        /// </summary>
        private static IDictionary<SpellSlot, Spell> Spells
        {
            get
            {
                return Ekko.Spells;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Process the farming.
        /// </summary>
        public static void ProcessFarm()
        {
            if (Spells[SpellSlot.Q].IsReady())
            {
                if (Ekko.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LaneClear)
                    && Ekko.Menu.Item("l33t.ekko.farming.lcq").GetValue<bool>())
                {
                    var farmLocation =
                        MinionManager.GetBestLineFarmLocation(
                            GameObjects.EnemyMinions.Where(
                                m => m.IsValidTarget() && m.Distance(Player.Position) <= Spells[SpellSlot.Q].Range)
                                .Select(m => m.Position.To2D())
                                .ToList(), 
                            Spells[SpellSlot.Q].Width, 
                            Spells[SpellSlot.Q].Range);

                    if (farmLocation.MinionsHit >= Ekko.Menu.Item("l33t.ekko.farming.lcqh").GetValue<Slider>().Value)
                    {
                        Spells[SpellSlot.Q].Cast(farmLocation.Position);
                    }
                }
                else if (Ekko.Menu.Item("l33t.ekko.farming.lhq").GetValue<bool>() && Ekko.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LastHit))
                {
                    var farmLocation =
                        GetBestLineFarmLocation(
                            GameObjects.EnemyMinions.Where(
                                m => m.IsValidTarget() && m.Distance(Player.Position) <= Spells[SpellSlot.Q].Range)
                                .ToList(), 
                            Spells[SpellSlot.Q].Width, 
                            Spells[SpellSlot.Q].Range);

                    if (farmLocation.MinionsHit >= Ekko.Menu.Item("l33t.ekko.farming.lhqh").GetValue<Slider>().Value
                        && farmLocation.Minions.Count(m => m.Health <= Damages.GetDamageQ(m))
                        >= Ekko.Menu.Item("l33t.ekko.farming.lhqh").GetValue<Slider>().Value)
                    {
                        Spells[SpellSlot.Q].Cast(farmLocation.Position);
                    }
                }
            }
        }

        /// <summary>
        ///     Processes Flee.
        /// </summary>
        public static void ProcessFlee()
        {
            var targets =
                GameObjects.EnemyHeroes.Where(
                    h => h.IsValidTarget() && h.Distance(Player.Position) <= Spells[SpellSlot.Q].Range).ToList();
            if (targets.Any())
            {
                if (Spells[SpellSlot.Q].IsReady() && Ekko.Menu.Item("l33t.ekko.flee.q").GetValue<bool>())
                {
                    var line = MinionManager.GetBestLineFarmLocation(
                        targets.Select(h => h.Position.To2D()).ToList(), 
                        Spells[SpellSlot.Q].Width - 5f, 
                        Spells[SpellSlot.Q].Range);
                    if (line.MinionsHit >= targets.Count / 2 - 1 && line.MinionsHit >= 1)
                    {
                        Spells[SpellSlot.Q].Cast(line.Position);
                    }
                }

                if (Spells[SpellSlot.W].IsReady() && Ekko.Menu.Item("l33t.ekko.flee.w").GetValue<bool>())
                {
                    Spells[SpellSlot.W].Cast(Spells[SpellSlot.W].GetPrediction(Player, true).CastPosition);
                }
            }

            if (Spells[SpellSlot.E].IsReady() && Ekko.Menu.Item("l33t.ekko.flee.e").GetValue<bool>())
            {
                var closestTarget = targets.OrderBy(t => t.Distance(Player.Position)).FirstOrDefault();
                var farestMinion = closestTarget != null && closestTarget.IsValidTarget()
                                       ? GameObjects.EnemyMinions.Where(
                                           m => m.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f)
                                             .OrderByDescending(m => m.Distance(closestTarget.Position))
                                             .ThenBy(m => m.Distance(Game.CursorPos))
                                             .FirstOrDefault()
                                       : GameObjects.EnemyMinions.Where(
                                           m => m.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f)
                                             .Where(m => m.Distance(Game.CursorPos) < m.Distance(Ekko.Player.Position))
                                             .OrderBy(m => m.Distance(Game.CursorPos))
                                             .FirstOrDefault();
                if (farestMinion != null && farestMinion.IsValidTarget())
                {
                    Spells[SpellSlot.E].Cast(farestMinion.Position);
                    FleeMinion = farestMinion;
                }
            }

            if (Player.AttackRange > 125 && FleeMinion.IsValidTarget())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, FleeMinion);
            }
        }

        /// <summary>
        ///     Process <c>Killsteal</c>.
        /// </summary>
        public static void ProcessKillsteal()
        {
            var lowestTarget = GameObjects.EnemyHeroes.OrderBy(h => h.Health).FirstOrDefault();
            if (!lowestTarget.IsValidTarget() || lowestTarget == null)
            {
                return;
            }

            var targetPos =
                Prediction.GetPrediction(
                    lowestTarget, 
                    Spells[SpellSlot.Q].Delay, 
                    lowestTarget.BoundingRadius, 
                    lowestTarget.MoveSpeed).UnitPosition;
            if (Spells[SpellSlot.Q].IsReady() && Ekko.Menu.Item("l33t.ekko.ks.q").GetValue<bool>()
                && targetPos.Distance(Player.Position) <= Spells[SpellSlot.Q].Range
                && lowestTarget.Health <= Damages.GetDamageQ(lowestTarget))
            {
                var pred = Spells[SpellSlot.Q].GetPrediction(
                    lowestTarget, 
                    false, 
                    Spells[SpellSlot.Q].Range - 50f, 
                    new[] { CollisionableObjects.YasuoWall });
                if (!pred.CollisionObjects.Contains(ObjectManager.Player))
                {
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Spells[SpellSlot.Q].Cast(pred.CastPosition);
                        lastQCastTick = Ekko.GameTime;
                    }
                }
            }

            if (Spells[SpellSlot.E].IsReady() && Ekko.Menu.Item("l33t.ekko.ks.e").GetValue<bool>()
                && lowestTarget.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f
                && lowestTarget.Health <= Player.GetAutoAttackDamage(lowestTarget) + Damages.GetDamageE(lowestTarget))
            {
                var dash = Player.Position.Extend(targetPos, Spells[SpellSlot.E].Range);
                if (dash.IsWall())
                {
                    var longestDash = Player.Position;
                    while (!longestDash.IsWall())
                    {
                        longestDash = longestDash.Extend(targetPos, 1f);
                    }

                    if (longestDash.Distance(targetPos) <= 425f)
                    {
                        Spells[SpellSlot.E].Cast(dash);
                    }
                }
                else
                {
                    if (dash.Distance(targetPos) <= 425f)
                    {
                        Spells[SpellSlot.E].Cast(dash);
                    }
                }
            }

            if (Spells[SpellSlot.R].IsReady() && Ekko.EkkoGhost != null && Ekko.EkkoGhost.IsValid
                && Ekko.Menu.Item("l33t.ekko.ks.r").GetValue<bool>())
            {
                var enemies =
                    GameObjects.EnemyHeroes.Where(
                        e =>
                        e != null && e.IsValidTarget()
                        && Prediction.GetPrediction(e, 0.25f, e.BoundingRadius, e.MoveSpeed)
                               .UnitPosition.Distance(Ekko.EkkoGhost.Position) <= Spells[SpellSlot.R].Range).ToList();
                if (enemies.Any())
                {
                    if (enemies.Count() >= Ekko.Menu.Item("l33t.ekko.ks.mr").GetValue<Slider>().Value)
                    {
                        var count = enemies.Count(e => e.Health <= Damages.GetDamageR(e));
                        if (count >= Ekko.Menu.Item("l33t.ekko.ks.mr").GetValue<Slider>().Value)
                        {
                            Spells[SpellSlot.R].Cast();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Processes combo.
        /// </summary>
        /// <param name="harass">
        ///     Indicates which mode.
        /// </param>
        public static void ProcessSpells(bool harass = false)
        {
            var target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var targetPos =
                Prediction.GetPrediction(target, Spells[SpellSlot.Q].Delay, target.BoundingRadius, target.MoveSpeed)
                    .UnitPosition;

            if (Spells[SpellSlot.Q].IsReady()
                && Ekko.Menu.Item(harass ? "l33t.ekko.harass.q" : "l33t.ekko.combo.q").GetValue<bool>()
                && targetPos.Distance(Player.Position) <= Spells[SpellSlot.Q].Range)
            {
                var pred = Spells[SpellSlot.Q].GetPrediction(
                    target, 
                    false, 
                    Spells[SpellSlot.Q].Range - 50f, 
                    new[] { CollisionableObjects.YasuoWall });
                if (!pred.CollisionObjects.Contains(ObjectManager.Player))
                {
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Spells[SpellSlot.Q].Cast(pred.CastPosition);
                        lastQCastTick = Ekko.GameTime;
                    }
                }
            }

            if (Spells[SpellSlot.E].IsReady()
                && Ekko.Menu.Item(harass ? "l33t.ekko.harass.e" : "l33t.ekko.combo.e").GetValue<bool>())
            {
                if (Ekko.EkkoField != null && Ekko.EkkoField.IsValid)
                {
                    var targets =
                        GameObjects.EnemyHeroes.Where(e => e.Distance(Ekko.EkkoField.Position) <= 375f).ToList();
                    if (targets.Count()
                        >= Ekko.Menu.Item(harass ? "l33t.ekko.harass.ehitc" : "l33t.ekko.combo.ehitc")
                               .GetValue<Slider>()
                               .Value)
                    {
                        var farTarget =
                            targets.Where(t => t.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f)
                                .OrderBy(t => t.Distance(Player.Position))
                                .LastOrDefault();
                        if (farTarget != null && farTarget.IsValidTarget())
                        {
                            Spells[SpellSlot.E].Cast(farTarget);
                        }
                    }

                    if (Ekko.EkkoField.Position.Distance(Player.Position) <= 375f + Spells[SpellSlot.E].Range
                        && targetPos.Distance(Ekko.EkkoField.Position) <= 375f)
                    {
                        Spells[SpellSlot.E].Cast(Ekko.EkkoField.Position);
                    }

                    if (Ekko.EkkoField.Position.Distance(Player.Position) <= 375f + Spells[SpellSlot.E].Range)
                    {
                        if (
                            targetPos.Distance(
                                Player.Position.Extend(Ekko.EkkoField.Position, Spells[SpellSlot.E].Range)) <= 425f)
                        {
                            Spells[SpellSlot.E].Cast(Ekko.EkkoField.Position);
                        }
                    }
                }

                if (targetPos.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f)
                {
                    var dash = Player.Position.Extend(targetPos, Spells[SpellSlot.E].Range);
                    if (dash.IsWall())
                    {
                        var longestDash = Player.Position;
                        for (var i = 1; i < Spells[SpellSlot.E].Range; ++i)
                        {
                            if (!Player.Position.Extend(targetPos, i).IsWall())
                            {
                                longestDash = Player.Position.Extend(targetPos, i);
                            }
                        }

                        if (longestDash.Distance(targetPos) <= 425f)
                        {
                            Spells[SpellSlot.E].Cast(dash);
                        }
                    }
                    else
                    {
                        if (dash.Distance(targetPos) <= 425f)
                        {
                            Spells[SpellSlot.E].Cast(dash);
                        }
                    }
                }
            }

            if (Spells[SpellSlot.W].IsReady()
                && Ekko.Menu.Item(harass ? "l33t.ekko.harass.w" : "l33t.ekko.combo.w").GetValue<bool>())
            {
                if (Player.Distance(targetPos) <= Spells[SpellSlot.Q].Range - Player.AttackRange
                    && Ekko.GameTime - lastQCastTick < 7000 + 1000 * Spells[SpellSlot.Q].Level - 1)
                {
                    // q + w
                    var targetPosition =
                        Prediction.GetPrediction(
                            target, 
                            3f, 
                            target.BoundingRadius, 
                            target.MoveSpeed * new float[] { 40, 50, 60, 70, 80 }[Spells[SpellSlot.W].Level - 1])
                            .UnitPosition;
                    if (targetPosition.Distance(Player.Position) <= Spells[SpellSlot.W].Range)
                    {
                        var castPos = targetPos + (targetPos - targetPosition);
                        if (!castPos.IsWall())
                        {
                            Spells[SpellSlot.W].Cast(targetPos + (targetPos - targetPosition));
                        }
                    }
                }

                var targets =
                    GameObjects.EnemyHeroes.Where(e => e.Distance(Player.Position) <= Spells[SpellSlot.W].Range)
                        .ToList();
                if (targets.Count()
                    >= Ekko.Menu.Item(harass ? "l33t.ekko.harass.whitc" : "l33t.ekko.combo.whitc")
                           .GetValue<Slider>()
                           .Value)
                {
                    // multi w
                    var predictions =
                        targets.Where(wTarget => wTarget != null && wTarget.IsValidTarget())
                            .Select(
                                wTarget =>
                                Prediction.GetPrediction(
                                    wTarget, 
                                    Spells[SpellSlot.E].Delay + Game.Ping / 2f, 
                                    wTarget.BoundingRadius, 
                                    wTarget.MoveSpeed))
                            .ToList();
                    var center = predictions.Aggregate(
                        new Vector3(), 
                        (current, position) => current + position.UnitPosition);

                    if (!center.IsWall() && center.Distance(Player.Position) <= Spells[SpellSlot.W].Range
                        && predictions.All(p => p.UnitPosition.Distance(center) <= Spells[SpellSlot.W].Range))
                    {
                        Spells[SpellSlot.W].Cast(center);
                    }
                }

                {
                    // w on slowed.
                    var targetPosition =
                        Prediction.GetPrediction(target, 3f, target.BoundingRadius, target.MoveSpeed).UnitPosition;
                    if (!targetPosition.IsWall()
                        && targetPosition.Distance(Player.Position) <= Spells[SpellSlot.W].Range
                        && target.Distance(Player.Position) <= Spells[SpellSlot.E].Range + 425f)
                    {
                        Spells[SpellSlot.W].Cast(targetPosition);
                    }
                }
            }

            if (!harass)
            {
                if (Spells[SpellSlot.R].IsReady() && Ekko.Menu.Item("l33t.ekko.combo.r").GetValue<bool>()
                    && Ekko.EkkoGhost != null && Ekko.EkkoGhost.IsValid)
                {
                    var ultimateDelay =
                        Prediction.GetPrediction(target, 0.25f, target.BoundingRadius, target.MoveSpeed).UnitPosition;
                    if (Ekko.Menu.Item("l33t.ekko.combo.rkill").GetValue<bool>())
                    {
                        if (ultimateDelay.Distance(Ekko.EkkoGhost.Position) <= Spells[SpellSlot.R].Range)
                        {
                            var damage = Damages.GetDamageE(target) + Damages.GetDamageQ(target)
                                         + Damages.GetDamageR(target) + Player.GetAutoAttackDamage(target);
                            if (damage >= target.Health)
                            {
                                Spells[SpellSlot.R].Cast();
                            }
                        }
                    }

                    if (target.Health <= Damages.GetDamageR(target)
                        && target.Health
                        >= Damages.GetDamageQ(target) + Player.GetAutoAttackDamage(target) + Damages.GetDamageE(target)
                        && ultimateDelay.Distance(Ekko.EkkoGhost.Position) <= Spells[SpellSlot.R].Range)
                    {
                        Spells[SpellSlot.R].Cast();
                    }

                    if (
                        GameObjects.EnemyHeroes.Where(e => e != null && e.IsValidTarget())
                            .Count(
                                e =>
                                Prediction.GetPrediction(e, 0.25f, e.BoundingRadius, e.MoveSpeed)
                                    .UnitPosition.Distance(Ekko.EkkoGhost.Position) <= Spells[SpellSlot.R].Range)
                        >= Ekko.Menu.Item("l33t.ekko.combo.rifhit").GetValue<Slider>().Value)
                    {
                        Spells[SpellSlot.R].Cast();
                    }

                    if (Ekko.Menu.Item("l33t.ekko.combo.rbackenable").GetValue<bool>()
                        && Ekko.OldHealth.ContainsKey(Ekko.GameTime - 4000))
                    {
                        if (Ekko.OldHealth[Ekko.GameTime - 4000] / Ekko.Player.MaxHealth * 100 > 5
                            && Ekko.Player.Health / Ekko.OldHealth[Ekko.GameTime - 4000] * 100
                            <= Ekko.Menu.Item("l33t.ekko.combo.rback").GetValue<Slider>().Value)
                        {
                            Spells[SpellSlot.R].Cast();
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates and gets the best line farming location.
        /// </summary>
        /// <param name="minionsList">
        ///     The Minions
        /// </param>
        /// <param name="width">
        ///     The Width
        /// </param>
        /// <param name="range">
        ///     The Range
        /// </param>
        /// <returns>
        ///     The Farming Location Container.
        /// </returns>
        private static FarmingLocation GetBestLineFarmLocation(
            IReadOnlyList<Obj_AI_Minion> minionsList, 
            float width, 
            float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.To2D();
            var minions =
                minionsList.ToDictionary<Obj_AI_Minion, Obj_AI_Minion, IDictionary<int, Vector2>>(
                    minion => minion, 
                    minion => new Dictionary<int, Vector2> { { 0, minion.Position.To2D() } });
            var minionsCollection = new List<Obj_AI_Minion>();

            var max = minionsList.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionsList[j].Position.To2D() != minionsList[i].Position.To2D())
                    {
                        minions[minionsList[j]].Add(
                            minions[minionsList[j]].Count, 
                            (minionsList[j].Position.To2D() + minionsList[i].Position.To2D()) / 2);
                    }
                }
            }

            foreach (var minion in minions)
            {
                foreach (var pos in minion.Value.Values)
                {
                    if (pos.Distance(startPos, true) <= range * range)
                    {
                        var endPos = startPos + range * (pos - startPos).Normalized();

                        var minionsCount =
                            minionsList.Where(
                                pos2 => pos2.Position.To2D().Distance(startPos, endPos, true, true) <= width * width)
                                .ToList();

                        if (minionsCount.Count() >= minionCount)
                        {
                            result = endPos;
                            minionCount = minionsCount.Count();
                            minionsCollection = minionsCount;
                        }
                    }
                }
            }

            return new FarmingLocation(result, minionCount, minionsCollection);
        }

        #endregion

        /// <summary>
        ///     The Farming Location Container.
        /// </summary>
        public class FarmingLocation
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="FarmingLocation" /> class.
            /// </summary>
            /// <param name="result">
            ///     The result position
            /// </param>
            /// <param name="minionCount">
            ///     The Minion Count
            /// </param>
            /// <param name="list">
            ///     The Minions List
            /// </param>
            public FarmingLocation(Vector2 result, int minionCount, List<Obj_AI_Minion> list)
            {
                this.Position = result;
                this.MinionsHit = minionCount;
                this.Minions = list;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the minions.
            /// </summary>
            public List<Obj_AI_Minion> Minions { get; private set; }

            /// <summary>
            ///     Gets the minions hit.
            /// </summary>
            public int MinionsHit { get; private set; }

            /// <summary>
            ///     Gets the position.
            /// </summary>
            public Vector2 Position { get; private set; }

            #endregion
        }
    }
}