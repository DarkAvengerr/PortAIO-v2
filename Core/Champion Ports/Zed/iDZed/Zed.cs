// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zed.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//   
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//   
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   TODO The zed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Eventing.Reader;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Activator;
    using Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    /// TODO The zed.
    /// </summary>
    internal static class Zed
    {
        #region Static Fields

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// TODO The menu.
        /// </summary>
        public static Menu Menu;

        /// <summary>
        /// TODO The _orbwalker.
        /// </summary>
        private static Orbwalking.Orbwalker _orbwalker;

        /// <summary>
        /// TODO The w shadow spell.
        /// </summary>
        public static readonly SpellDataInst WShadowSpell = Player.Spellbook.GetSpell(SpellSlot.W);

        /// <summary>
        /// TODO The r shadow spell.
        /// </summary>
        private static readonly SpellDataInst RShadowSpell = Player.Spellbook.GetSpell(SpellSlot.R);

        /// <summary>
        /// TODO The e cast range.
        /// </summary>
        public static readonly float ERange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange - 10;

        /// <summary>
        /// TODO The r cast range.
        /// </summary>
        public static readonly float RRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange;

        /// <summary>
        /// TODO The _spells.
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> _spells = new Dictionary<SpellSlot, Spell>
                                                                          {
                                                                              {
                                                                                  SpellSlot.Q, 
                                                                                  new Spell(SpellSlot.Q, 925f)
                                                                              }, 
                                                                              {
                                                                                  SpellSlot.W, 
                                                                                  new Spell(SpellSlot.W, 550f)
                                                                              }, 
                                                                              {
                                                                                  SpellSlot.E, 
                                                                                  new Spell(SpellSlot.E, ERange)
                                                                              }, 
                                                                              {
                                                                                  SpellSlot.R, 
                                                                                  new Spell(SpellSlot.R, RRange)
                                                                              }
                                                                          };

        /// <summary>
        /// TODO The _orbwalking modes dictionary.
        /// </summary>
        private static Dictionary<Orbwalking.OrbwalkingMode, OnOrbwalkingMode> _orbwalkingModesDictionary;

        #endregion

        #region Delegates

        /// <summary>
        /// TODO The on orbwalking mode.
        /// </summary>
        private delegate void OnOrbwalkingMode();

        #endregion

        #region Properties

        // private static bool _deathmarkKilled = false;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The on load.
        /// </summary>
        public static void OnLoad()
        {
            if (Player.ChampionName != "Zed")
            {
                return;
            }

            Chat.Print("iDZed loaded!");
            ShadowManager.OnLoad();
            _orbwalkingModesDictionary = new Dictionary<Orbwalking.OrbwalkingMode, OnOrbwalkingMode>
                                             {
                                                 { Orbwalking.OrbwalkingMode.Combo, Combo }, 
                                                 { Orbwalking.OrbwalkingMode.Mixed, Harass }, 
                                                 { Orbwalking.OrbwalkingMode.LastHit, LastHit }, 
                                                 { Orbwalking.OrbwalkingMode.LaneClear, Laneclear }, 
                                                 { Orbwalking.OrbwalkingMode.None, () => { } }
                                             };
            InitMenu();
            InitSpells();
            InitEvents();
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The cast e.
        /// </summary>
        private static void CastE()
        {
            if (!_spells[SpellSlot.E].IsReady())
            {
                return;
            }

            if (
                HeroManager.Enemies.Count(
                    hero =>
                    hero.IsValidTarget()
                    && (hero.Distance(Player.ServerPosition) <= _spells[SpellSlot.E].Range
                        || (ShadowManager.WShadow.ShadowObject != null
                            && hero.Distance(ShadowManager.WShadow.Position) <= _spells[SpellSlot.E].Range)
                        || (ShadowManager.RShadow.ShadowObject != null
                            && hero.Distance(ShadowManager.RShadow.Position) <= _spells[SpellSlot.E].Range))) > 0)
            {
                _spells[SpellSlot.E].Cast();
            }
        }

        /// <summary>
        /// TODO The cast q.
        /// </summary>
        /// <param name="target">
        /// TODO The target.
        /// </param>
        private static void CastQ(AIHeroClient target)
        {
            if (_spells[SpellSlot.Q].IsReady())
            {
                if (GetMarkedTarget() != null)
                {
                    target = GetMarkedTarget();
                }

                if (ShadowManager.WShadow.Exists
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(
                        ShadowManager.WShadow.Position, 
                        ShadowManager.WShadow.Position);
                    if (MenuHelper.IsMenuEnabled("com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (ShadowManager.WShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (ShadowManager.WShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
                else if (ShadowManager.RShadow.Exists
                         && ShadowManager.RShadow.ShadowObject.Distance(target.ServerPosition)
                         < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(
                        ShadowManager.RShadow.Position, 
                        ShadowManager.RShadow.Position);
                    if (MenuHelper.IsMenuEnabled("com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (ShadowManager.RShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (ShadowManager.RShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
                else
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                    if (MenuHelper.IsMenuEnabled("com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (Player.Distance(target) <= _spells[SpellSlot.Q].Range
                                && target.IsValidTarget(_spells[SpellSlot.Q].Range))
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (Player.Distance(target) <= _spells[SpellSlot.Q].Range
                            && target.IsValidTarget(_spells[SpellSlot.Q].Range))
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// TODO The cast w.
        /// </summary>
        /// <param name="target">
        /// TODO The target.
        /// </param>
        private static void CastW(AIHeroClient target)
        {
            if (!HasEnergy(new[] { SpellSlot.W, SpellSlot.Q }))
            {
                return;
            }

            if (ShadowManager.WShadow.IsUsable)
            {
                if (_spells[SpellSlot.W].IsReady() && WShadowSpell.ToggleState == 0
                    && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    var position = Player.ServerPosition.To2D()
                        .Extend(target.ServerPosition.To2D(), _spells[SpellSlot.W].Range);
                    if (position.Distance(target) <= _spells[SpellSlot.Q].Range)
                    {
                        if (IsPassWall(Player.ServerPosition, target.ServerPosition))
                        {
                            return;
                        }

                        _spells[SpellSlot.W].Cast(position);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                }
            }

            if (ShadowManager.CanGoToShadow(ShadowManager.WShadow) && WShadowSpell.ToggleState == 2)
            {
                if (Menu.Item("com.idz.zed.combo.swapw").GetValue<bool>()
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }

        /// <summary>
        /// TODO The combo.
        /// </summary>
        private static void Combo()
        {
            var target = GetAssasinationTarget();

            switch (Menu.Item("com.idz.zed.combo.mode").GetValue<StringList>().SelectedIndex)
            {
                case 0: // Line mode
                    if (Menu.Item("com.idz.zed.combo.user").GetValue<bool>() && _spells[SpellSlot.R].IsReady()
                        && (target.Health + 20
                            >= _spells[SpellSlot.Q].GetDamage(target) + _spells[SpellSlot.E].GetDamage(target)
                            + ObjectManager.Player.GetAutoAttackDamage(target)))
                    {
                        if (!HasEnergy(new[] { SpellSlot.W, SpellSlot.R, SpellSlot.Q, SpellSlot.E }))
                        {
                            return;
                        }

                        if (ShadowManager.WShadow.Exists)
                        {
                            CastQ(target);
                            CastE();
                        }
                        else
                        {
                            DoLineCombo(target);
                        }
                    }
                    else
                    {
                        DoNormalCombo(target);
                    }

                    break;
                case 1: // triangle mode
                    if (Menu.Item("com.idz.zed.combo.user").GetValue<bool>() && _spells[SpellSlot.R].IsReady()
                        && (target.Health + 20
                            >= _spells[SpellSlot.Q].GetDamage(target) + _spells[SpellSlot.E].GetDamage(target)
                            + ObjectManager.Player.GetAutoAttackDamage(target)))
                    {
                        if (!HasEnergy(new[] { SpellSlot.W, SpellSlot.R }))
                        {
                            return;
                        }

                        if (ShadowManager.WShadow.Exists)
                        {
                            CastQ(target);
                            CastE();
                        }
                        else
                        {
                            DoTriangleCombo(target);
                        }
                    }
                    else
                    {
                        DoNormalCombo(target);
                    }

                    break;
            }
        }

        /// <summary>
        /// TODO The do line combo.
        /// </summary>
        /// <param name="target">
        /// TODO The target.
        /// </param>
        private static void DoLineCombo(AIHeroClient target)
        {
            if (ShadowManager.RShadow.IsUsable)
            {
                if (MenuHelper.IsMenuEnabled("checkQWE"))
                {
                    if (_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.W].IsReady()
                        && _spells[SpellSlot.E].IsReady())
                    {
                        if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                        {
                            _spells[SpellSlot.R].Cast(target);
                        }
                    }
                }
                else
                {
                    if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                    {
                        _spells[SpellSlot.R].Cast(target);
                    }
                }
            }

            if (GetMarkedTarget() != null)
            {
                target = GetMarkedTarget();
            }

            ItemManager.UseDeathmarkItems();
            ItemManager.UseSummonerSpells();

            if (ShadowManager.RShadow.Exists && ShadowManager.WShadow.IsUsable)
            {
                var wCastLocation = Player.ServerPosition
                                    - Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 400;

                if (ShadowManager.WShadow.IsUsable && WShadowSpell.ToggleState == 0
                    && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    _spells[SpellSlot.W].Cast(wCastLocation);

                    // Maybe add a delay giving the target a chance to flash / zhonyas then it will place w at best location for more damage
                    _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                }
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.RShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
            else if (ShadowManager.RShadow.Exists && !ShadowManager.WShadow.IsUsable && !ShadowManager.WShadow.Exists)
            {
                CastQ(target);
                CastE();
            }

            if (ShadowManager.CanGoToShadow(ShadowManager.WShadow) && WShadowSpell.ToggleState == 2)
            {
                // && !_deathmarkKilled)
                if (MenuHelper.IsMenuEnabled("com.idz.zed.combo.swapw")
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }

        /// <summary>
        /// TODO The do normal combo.
        /// </summary>
        /// <param name="target">
        /// TODO The target.
        /// </param>
        private static void DoNormalCombo(AIHeroClient target)
        {
            if (MenuHelper.IsMenuEnabled("com.idz.zed.combo.usew")
                && (_spells[SpellSlot.Q].IsReady() || _spells[SpellSlot.E].IsReady()))
            {
                CastW(target);
                if (Menu.Item("com.idz.zed.combo.useq").GetValue<bool>())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(105, () => CastQ(target));
                }

                if (Menu.Item("com.idz.zed.combo.usee").GetValue<bool>())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(105, CastE);
                }
            }
            else
            {
                CastQ(target);
                CastE();
            }
        }

        /// <summary>
        /// TODO The do triangle combo.
        /// </summary>
        /// <param name="target">
        /// TODO The target.
        /// </param>
        private static void DoTriangleCombo(AIHeroClient target)
        {
            // I'm dumb, this triangular combo is only good for targets the Zhonyas, we can still use it for that i guess :^)
            if (ShadowManager.RShadow.IsUsable && !target.HasBuffOfType(BuffType.Invulnerability))
            {
                // Cast Ultimate m8 :S
                if (MenuHelper.IsMenuEnabled("checkQWE"))
                {
                    if (_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.W].IsReady()
                        && _spells[SpellSlot.E].IsReady())
                    {
                        if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                        {
                            _spells[SpellSlot.R].Cast(target);
                        }
                    }
                }
                else
                {
                    if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                    {
                        _spells[SpellSlot.R].Cast(target);
                    }
                }
            }

            if (GetMarkedTarget() != null)
            {
                target = GetMarkedTarget();
            }

            ItemManager.UseDeathmarkItems();
            ItemManager.UseSummonerSpells();

            if (ShadowManager.RShadow.Exists && ShadowManager.WShadow.IsUsable)
            {
                var bestWPosition = VectorHelper.GetBestPosition(
                    target, 
                    VectorHelper.GetVertices(target)[0], 
                    VectorHelper.GetVertices(target)[1]);

                // Maybe add a delay giving the target a chance to flash / zhonyas then it will place w at best perpendicular location m8
                if (WShadowSpell.ToggleState == 0 && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    _spells[SpellSlot.W].Cast(bestWPosition);

                    // Allow half a second for the target to flash / zhonyas? :S
                    _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                }
            }

            if (WShadowSpell.ToggleState == 2)
            {
                _spells[SpellSlot.W].Cast();
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.RShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
            else if (ShadowManager.RShadow.Exists && !ShadowManager.WShadow.IsUsable && !ShadowManager.WShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
        }

        /// <summary>
        /// TODO The drawing_ on draw.
        /// </summary>
        /// <param name="args">
        /// TODO The args.
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (MenuHelper.IsMenuEnabled("drawShadows"))
            {
                foreach (var shadow in
                    ShadowManager._shadowsList.Where(sh => sh.State != ShadowState.NotActive && sh.ShadowObject != null)
                    )
                {
                    Render.Circle.DrawCircle(shadow.Position, 60f, Color.Orange);
                }
            }

            foreach (var spell in
                _spells.Where(
                    s => Menu.Item("com.idz.zed.drawing.draw" + GetStringFromSpellSlot(s.Key)).GetValue<Circle>().Active)
                )
            {
                var value = Menu.Item("com.idz.zed.drawing.draw" + GetStringFromSpellSlot(spell.Key)).GetValue<Circle>();

                Render.Circle.DrawCircle(
                    Player.Position, 
                    spell.Value.Range, 
                    spell.Value.IsReady() ? value.Color : Color.Aqua);
            }
        }

        /// <summary>
        /// TODO The game_ on update.
        /// </summary>
        /// <param name="args">
        /// TODO The args.
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            OnFlee();

            _orbwalkingModesDictionary[_orbwalker.ActiveMode]();
        }

        /// <summary>
        /// TODO The get assasination target.
        /// </summary>
        /// <param name="range">
        /// TODO The range.
        /// </param>
        /// <param name="damageType">
        /// TODO The damage type.
        /// </param>
        /// <returns>
        /// </returns>
        private static AIHeroClient GetAssasinationTarget(
            float range = 0, 
            TargetSelector.DamageType damageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(range) < 0.00001)
            {
                range = _spells[SpellSlot.R].IsReady()
                            ? _spells[SpellSlot.R].Range
                            : _spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range / 2f;
            }

            if (!Menu.Item("AssassinActive").GetValue<bool>())
            {
                return TargetSelector.GetTarget(range, damageType);
            }

            var assassinRange = Menu.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy =
                HeroManager.Enemies.Where(
                    enemy =>
                    enemy.Team != Player.Team && !enemy.IsDead && enemy.IsVisible
                    && Menu.Item("Assassin" + enemy.ChampionName) != null
                    && Menu.Item("Assassin" + enemy.ChampionName).GetValue<bool>()
                    && Player.Distance(enemy) < assassinRange);

            if (Menu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            var objAiHeroes = vEnemy as AIHeroClient[] ?? vEnemy.ToArray();

            var target = !objAiHeroes.Any() ? TargetSelector.GetTarget(range, damageType) : objAiHeroes[0];

            return target;
        }

        /// <summary>
        /// TODO The get hitchance.
        /// </summary>
        /// <returns>
        /// </returns>
        private static HitChance GetHitchance()
        {
            switch (Menu.Item("com.idz.zed.misc.hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        /// <summary>
        /// TODO The get marked target.
        /// </summary>
        /// <returns>
        /// </returns>
        private static AIHeroClient GetMarkedTarget()
        {
            return
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    x.IsValidTarget(_spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range)
                    && x.HasBuff("zedulttargetmark") && x.IsVisible);
        }

        /// <summary>
        /// TODO The get string from spell slot.
        /// </summary>
        /// <param name="sp">
        /// TODO The sp.
        /// </param>
        /// <returns>
        /// </returns>
        private static string GetStringFromSpellSlot(SpellSlot sp)
        {
            switch (sp)
            {
                case SpellSlot.Q:
                    return "Q";
                case SpellSlot.W:
                    return "W";
                case SpellSlot.E:
                    return "E";
                case SpellSlot.R:
                    return "R";
                default:
                    return "unk";
            }
        }

        /// <summary>
        /// TODO The harass.
        /// </summary>
        private static void Harass()
        {
            if (!Menu.Item("com.idz.zed.harass.useHarass").GetValue<bool>())
            {
                return;
            }

            var target = TargetSelector.GetTarget(
                _spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range, 
                TargetSelector.DamageType.Physical);
            switch (Menu.Item("com.idz.zed.harass.harassMode").GetValue<StringList>().SelectedIndex)
            {
                case 0: // "W-E-Q"
                    if (_spells[SpellSlot.W].IsReady() && ShadowManager.WShadow.IsUsable
                        && WShadowSpell.ToggleState == 0
                        && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0
                        && Player.Distance(target) <= _spells[SpellSlot.W].Range + 300
                        && _spells[SpellSlot.Q].IsReady())
                    {
                        _spells[SpellSlot.W].Cast(target.ServerPosition);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                    else if ((!_spells[SpellSlot.W].IsReady() || WShadowSpell.ToggleState != 0) && _spells[SpellSlot.Q].IsReady() && target.Distance(Player) < _spells[SpellSlot.Q].Range)
                    {
                        if (Menu.Item("fast.harass").GetValue<bool>())
                        {
                            _spells[SpellSlot.Q].Cast(target.Position);
                        }
						else
						{
							if (_spells[SpellSlot.Q].GetPrediction(target).Hitchance < HitChance.High)
                            {
                                _spells[SpellSlot.Q].Cast(target.Position);
                            }
						}
                    }
                    else if (WShadowSpell.ToggleState != 0 && !_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.E].IsReady())
                    {
                        foreach (var shdw in ShadowManager._shadowsList.Where(x => x.Type == ShadowType.Normal))
                        {
                            _spells[SpellSlot.E].Cast();
                        }
                    }
                    

                    break;
            }
        }

        /// <summary>
        /// TODO The has energy.
        /// </summary>
        /// <param name="spells">
        /// TODO The spells.
        /// </param>
        /// <returns>
        /// </returns>
        private static bool HasEnergy(IEnumerable<SpellSlot> spells)
        {
            if (!Menu.Item("energyManagement").GetValue<bool>())
            {
                return true;
            }

            var totalCost = spells.Sum(slot => Player.Spellbook.GetSpell(slot).SData.Mana);
            return Player.Mana >= totalCost;
        }

        /// <summary>
        /// TODO The init events.
        /// </summary>
        private static void InitEvents()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
        }

        /// <summary>
        /// TODO The init menu.
        /// </summary>
        private static void InitMenu()
        {
            Menu = new Menu("iDZed - Reworked", "com.idz.zed", true);

            // ReSharper disable once ObjectCreationAsStatement
            new AssassinManager();

            var orbwalkMenu = new Menu(":: Orbwalker", "com.idz.zed.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Menu.AddSubMenu(orbwalkMenu);

            var comboMenu = new Menu(":: Combo", "com.idz.zed.combo");
            {
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("com.idz.zed.combo.useqpred", "Q Prediction: On = slower, off = faster").SetValue(
                        false));
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.user", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.swapw", "Swap W For Follow").SetValue(false));
                comboMenu.AddItem(new MenuItem("com.idz.zed.combo.swapr", "Swap R On kill").SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("com.idz.zed.combo.mode", "Combo Mode").SetValue(
                        new StringList(new[] { "Line Mode", "Triangle Mode" })));
            }
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu(":: Harass", "com.idz.zed.harass");
            {
                harassMenu.AddItem(new MenuItem("com.idz.zed.harass.useHarass", "Use Harass").SetValue(true));
                harassMenu.AddItem(new MenuItem("fast.harass", "Q Prediction: On = slower, off = faster").SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("com.idz.zed.harass.harassMode", "Harass Mode").SetValue(
                        new StringList(new[] { "W-E-Q" })));
            }

            Menu.AddSubMenu(harassMenu);

            var lastHitMenu = new Menu(":: LastHit", "com.idz.zed.lasthit");
            {
                lastHitMenu.AddItem(new MenuItem("com.idz.zed.lasthit.useQ", "Use Q in LastHit").SetValue(true));
                lastHitMenu.AddItem(new MenuItem("com.idz.zed.lasthit.useE", "Use E in LastHit").SetValue(true));
            }

            Menu.AddSubMenu(lastHitMenu);

            var laneclearMenu = new Menu(":: Laneclear", "com.idz.zed.laneclear");
            {
                laneclearMenu.AddItem(new MenuItem("com.idz.zed.laneclear.useQ", "Use Q in laneclear").SetValue(true));
                laneclearMenu.AddItem(
                    new MenuItem("com.idz.zed.laneclear.qhit", "Min minions for Q").SetValue(new Slider(3, 1, 10)));
                laneclearMenu.AddItem(new MenuItem("com.idz.zed.laneclear.useE", "Use E in laneclear").SetValue(true));
                laneclearMenu.AddItem(
                    new MenuItem("com.idz.zed.laneclear.ehit", "Min minions for E").SetValue(new Slider(3, 1, 10)));
            }

            Menu.AddSubMenu(laneclearMenu);

            var drawMenu = new Menu(":: Drawing", "com.idz.zed.drawing");
            {
                foreach (var slot in _spells.Select(entry => entry.Key))
                {
                    drawMenu.AddItem(
                        new MenuItem(
                            "com.idz.zed.drawing.draw" + GetStringFromSpellSlot(slot), 
                            "Draw " + GetStringFromSpellSlot(slot) + " Range").SetValue(new Circle(true, Color.Aqua)));
                }

                drawMenu.AddItem(new MenuItem("drawShadows", "Draw Shadows").SetValue(true));
            }

            Menu.AddSubMenu(drawMenu);

            var fleeMenu = new Menu(":: Flee", "com.idz.zed.flee");
            {
                fleeMenu.AddItem(
                    new MenuItem("fleeActive", "Flee Key").SetValue(
                        new KeyBind("P".ToCharArray()[0], KeyBindType.Press)));
                fleeMenu.AddItem(new MenuItem("autoEFlee", "Auto E when fleeing").SetValue(true));
            }

            Menu.AddSubMenu(fleeMenu);

            var miscMenu = new Menu(":: Misc", "com.idz.zed.misc");
            {
                miscMenu.AddItem(new MenuItem("energyManagement", "Use Energy Management").SetValue(true));
                miscMenu.AddItem(new MenuItem("safetyChecks", "Check Safety for shadow swapping").SetValue(true));
                miscMenu.AddItem(
                    new MenuItem("com.idz.zed.misc.hitchance", "Q Hitchance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2)));
                miscMenu.AddItem(new MenuItem("checkQWE", "Check Other Spells before ult").SetValue(true));
            }

            Menu.AddSubMenu(miscMenu);

            ItemManager.OnLoad(Menu);
            ZedEvader.OnLoad(Menu);

            Menu.AddToMainMenu();
        }

        /// <summary>
        /// TODO The init spells.
        /// </summary>
        private static void InitSpells()
        {
            _spells[SpellSlot.Q].SetSkillshot(0.25F, 50F, 1600F, false, SkillshotType.SkillshotLine);
            _spells[SpellSlot.W].SetSkillshot(0.75F, 75F, 1000F, false, SkillshotType.SkillshotCircle);
            //_spells[SpellSlot.E].SetSkillshot(0f, 220f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        /// <summary>
        /// TODO The is pass wall.
        /// </summary>
        /// <param name="start">
        /// TODO The start.
        /// </param>
        /// <param name="end">
        /// TODO The end.
        /// </param>
        /// <returns>
        /// </returns>
        private static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 25)
            {
                var pos = start.To2D().Extend(Player.ServerPosition.To2D(), -i);
                if (pos.IsWall())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// TODO The laneclear.
        /// </summary>
        private static void Laneclear()
        {
            var allMinionsQ = MinionManager.GetMinions(
                Player.ServerPosition, 
                _spells[SpellSlot.Q].Range, 
                MinionTypes.All, 
                MinionTeam.NotAlly);
            var allMinionsE = MinionManager.GetMinions(
                Player.ServerPosition, 
                _spells[SpellSlot.Q].Range, 
                MinionTypes.All, 
                MinionTeam.NotAlly);
            if (Menu.Item("com.idz.zed.laneclear.useQ").GetValue<bool>() && _spells[SpellSlot.Q].IsReady()
                && !Player.Spellbook.IsAutoAttacking)
            {
                var bestPositionQ =
                    MinionManager.GetBestLineFarmLocation(
                        allMinionsQ.Select(x => x.ServerPosition.To2D()).ToList(), 
                        _spells[SpellSlot.Q].Width, 
                        _spells[SpellSlot.Q].Range);
                if (bestPositionQ.MinionsHit >= Menu.Item("com.idz.zed.laneclear.qhit").GetValue<Slider>().Value)
                {
                    _spells[SpellSlot.Q].Cast(bestPositionQ.Position);
                }
            }

            if (Menu.Item("com.idz.zed.laneclear.useE").GetValue<bool>() && _spells[SpellSlot.E].IsReady()
                && !Player.Spellbook.IsAutoAttacking)
            {
                var eLocation =
                    MinionManager.GetBestLineFarmLocation(
                        allMinionsE.Select(x => x.ServerPosition.To2D()).ToList(), 
                        _spells[SpellSlot.E].Width, 
                        _spells[SpellSlot.E].Range);
                if (eLocation.MinionsHit >= Menu.Item("com.idz.zed.laneclear.ehit").GetValue<Slider>().Value)
                {
                    _spells[SpellSlot.E].Cast();
                }
            }
        }

        /// <summary>
        /// TODO The last hit.
        /// </summary>
        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, 1000f, MinionTypes.All, MinionTeam.NotAlly);
            if (Menu.Item("com.idz.zed.lasthit.useQ").GetValue<bool>() && _spells[SpellSlot.Q].IsReady())
            {
                var qMinion =
                    allMinions.FirstOrDefault(
                        x => _spells[SpellSlot.Q].IsInRange(x) && x.IsValidTarget(_spells[SpellSlot.Q].Range));

                if (qMinion != null && _spells[SpellSlot.Q].GetDamage(qMinion) > qMinion.Health
                    && !Orbwalking.InAutoAttackRange(qMinion))
                {
                    _spells[SpellSlot.Q].Cast(qMinion);
                }
            }

            if (Menu.Item("com.idz.zed.lasthit.useE").GetValue<bool>() && _spells[SpellSlot.E].IsReady())
            {
                var minions =
                    MinionManager.GetMinions(
                        Player.ServerPosition, 
                        _spells[SpellSlot.E].Range, 
                        MinionTypes.All, 
                        MinionTeam.NotAlly)
                        .FindAll(
                            minion =>
                            !Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _spells[SpellSlot.E].GetDamage(minion));
                if (minions.Count >= 1)
                {
                    _spells[SpellSlot.E].Cast();
                }
            }
        }

        /// <summary>
        /// TODO The on create object.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="args">
        /// TODO The args.
        /// </param>
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
            }

            if (Menu.Item("com.idz.zed.combo.swapr").GetValue<bool>())
            {
                if (sender.Name == "Zed_Base_R_buf_tell.troy")
                {
                    // _deathmarkKilled = true;
                    if (RShadowSpell.ToggleState == 2 && ShadowManager.CanGoToShadow(ShadowManager.RShadow))
                    {
                        _spells[SpellSlot.R].Cast();
                    }
                }
            }
        }

        /// <summary>
        /// TODO The on flee.
        /// </summary>
        private static void OnFlee()
        {
            if (!MenuHelper.GetKeybindValue("fleeActive"))
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_spells[SpellSlot.W].IsReady() && ShadowManager.WShadow.IsUsable)
            {
                _spells[SpellSlot.W].Cast(Game.CursorPos);
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.CanGoToShadow(ShadowManager.WShadow))
            {
                _spells[SpellSlot.W].Cast();
            }

            CastE();
        }

        /// <summary>
        /// TODO The on spell cast.
        /// </summary>
        /// <param name="sender1">
        /// TODO The sender 1.
        /// </param>
        /// <param name="args">
        /// TODO The args.
        /// </param>
        private static void OnSpellCast(Obj_AI_Base sender1, GameObjectProcessSpellCastEventArgs args)
        {
            var sender = sender1 as AIHeroClient;
            if (sender != null && sender.IsEnemy && sender.Team != Player.Team)
            {
                if (args.SData.Name == "ZhonyasHourglass" && sender.HasBuff("zedulttargetmark")
                    && Player.Distance(sender, true) < _spells[SpellSlot.W].Range - 20 * _spells[SpellSlot.W].Range - 20)
                {
                    var bestPosition = VectorHelper.GetBestPosition(
                        sender, 
                        VectorHelper.GetVertices(sender, true)[0], 
                        VectorHelper.GetVertices(sender, true)[1]);
                    if (_spells[SpellSlot.W].IsReady() && WShadowSpell.ToggleState == 0
                        && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                    {
                        _spells[SpellSlot.W].Cast(bestPosition);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                }
            }
        }

        #endregion
    }
}