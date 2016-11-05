// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryPoint.cs" company="LeagueSharp">
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
//   The entry point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_the_Boy_Who_Shattered_Time
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     The entry point.
    /// </summary>
    public class EntryPoint
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The invoke function.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        public static void Invoke(Menu menu)
        {
            var q = new Spell(SpellSlot.Q, 950f, TargetSelector.DamageType.Magical);
            q.SetSkillshot(0.25f, 60f, 1650f, true, SkillshotType.SkillshotLine);
            var w = new Spell(SpellSlot.W, 1600f, TargetSelector.DamageType.Magical);
            w.SetSkillshot(3.0f, 425f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ekko.Spells.Add(SpellSlot.Q, q);
            Ekko.Spells.Add(SpellSlot.W, w);
            Ekko.Spells.Add(SpellSlot.E, new Spell(SpellSlot.E, 325f));
            Ekko.Spells.Add(SpellSlot.R, new Spell(SpellSlot.R, 425f));

            Ekko.Orbwalker = new Orbwalking.Orbwalker(menu.AddSubMenu(new Menu("Orbwalker", "l33t.ekko.orbwalker")));
            TargetSelector.AddToMenu(menu.AddSubMenu(new Menu("Target Selector", "l33t.ekko.targetselector")));

            var combo = menu.AddSubMenu(new Menu("Combo", "l33t.ekko.combo"));
            {
                combo.AddItem(new MenuItem("l33t.ekko.combo.q", "Use Q")).SetValue(true);
                combo.AddItem(new MenuItem("l33t.ekko.combo.w", "Use W")).SetValue(true);
                combo.AddItem(new MenuItem("l33t.ekko.combo.e", "Use E")).SetValue(true);
                combo.AddItem(new MenuItem("l33t.ekko.combo.r", "Use R")).SetValue(true);
                combo.AddItem(new MenuItem("l33t.ekko.combo.space", string.Empty));
                combo.AddItem(new MenuItem("l33t.ekko.combo.whitc", "Use W if hit targets amount")).SetValue(new Slider(3, 1, 5));
                combo.AddItem(new MenuItem("l33t.ekko.combo.ehitc", "Use E into W if stun targets minimum")).SetValue(new Slider(3, 1, 5));
                combo.AddItem(new MenuItem("l33t.ekko.combo.spacew", string.Empty));
                combo.AddItem(
                    new MenuItem("l33t.ekko.combo.rbackenable", "Enable Use R if lose % health").SetValue(false));
                combo.AddItem(
                    new MenuItem("l33t.ekko.combo.rback", "Use R if lose % health").SetValue(new Slider(65, 1)));
                combo.AddItem(
                    new MenuItem("l33t.ekko.combo.rifhit", "Use R if hit enemies").SetValue(new Slider(3, 1, 5)));
                combo.AddItem(
                    new MenuItem("l33t.ekko.combo.rkill", "Use R if enemy is killable by burst").SetValue(true));
            }

            var harass = menu.AddSubMenu(new Menu("Harass", "l33t.ekko.harass"));
            {
                harass.AddItem(new MenuItem("l33t.ekko.harass.q", "Use Q")).SetValue(true);
                harass.AddItem(new MenuItem("l33t.ekko.harass.w", "Use W")).SetValue(true);
                harass.AddItem(new MenuItem("l33t.ekko.harass.e", "Use E")).SetValue(true);
                harass.AddItem(new MenuItem("l33t.ekko.harass.space", string.Empty));
                harass.AddItem(new MenuItem("l33t.ekko.harass.whitc", "Use W if hit targets minimum")).SetValue(new Slider(3, 1, 5));
                harass.AddItem(new MenuItem("l33t.ekko.harass.ehitc", "Use E into W if stun targets minimum")).SetValue(new Slider(3, 1, 5));
            }

            var ks = menu.AddSubMenu(new Menu("Kill Steal", "l33t.ekko.ks"));
            {
                ks.AddItem(new MenuItem("l33t.ekko.ks.use", "Use Killsteal")).SetValue(true);
                ks.AddItem(new MenuItem("l33t.ekko.ks.q", "Use Q")).SetValue(true);
                ks.AddItem(new MenuItem("l33t.ekko.ks.e", "Use E")).SetValue(true);
                ks.AddItem(new MenuItem("l33t.ekko.ks.r", "Use R")).SetValue(true);
                ks.AddItem(new MenuItem("l33t.ekko.ks.mr", "Min Enemies to kill with R")).SetValue(new Slider(3, 1, 5));
            }

            var flee = menu.AddSubMenu(new Menu("Flee", "l33t.ekko.flee"));
            {
                flee.AddItem(new MenuItem("l33t.ekko.flee.enable", "Enable")).SetValue(true);
                flee.AddItem(new MenuItem("l33t.ekko.flee.key", "Flee Key")).SetValue(new KeyBind('Z', KeyBindType.Press));
                flee.AddItem(new MenuItem("l33t.ekko.flee.q", "Use Q")).SetValue(true);
                flee.AddItem(new MenuItem("l33t.ekko.flee.w", "Use W")).SetValue(true);
                flee.AddItem(new MenuItem("l33t.ekko.flee.e", "Use E")).SetValue(true);
            }

            var farming = menu.AddSubMenu(new Menu("Farming", "l33t.ekko.farming"));
            {
                farming.AddItem(new MenuItem("l33t.ekko.farming.lhq", "Use Q while Last Hit")).SetValue(true);
                farming.AddItem(new MenuItem("l33t.ekko.farming.lcq", "Use Q while Lane Clear")).SetValue(true);
                farming.AddItem(new MenuItem("l33t.ekko.farming.space", string.Empty));
                farming.AddItem(new MenuItem("l33t.ekko.farming.lhqh", "[Last Hit] If Q hits and kills"))
                    .SetValue(new Slider(4, 1, 8));
                farming.AddItem(new MenuItem("l33t.ekko.farming.lcqh", "[Lane Clear] If Q hits"))
                    .SetValue(new Slider(4, 1, 8));
            }

            var drawing = menu.AddSubMenu(new Menu("Drawings", "l33t.ekko.drawing"));
            {
                drawing.AddItem(new MenuItem("l33t.ekko.drawing.q", "Draw Q"))
                    .SetValue(new Circle(true, Color.RoyalBlue));
                drawing.AddItem(new MenuItem("l33t.ekko.drawing.w", "Draw W")).SetValue(new Circle(false, Color.Purple));
                drawing.AddItem(new MenuItem("l33t.ekko.drawing.e", "Draw E")).SetValue(new Circle(false, Color.Plum));
                drawing.AddItem(new MenuItem("l33t.ekko.drawing.r", "Draw R")).SetValue(new Circle(true, Color.Teal));
                drawing.AddItem(new MenuItem("l33t.ekko.drawing.classic", "Draw Classic Circles")).SetValue(false);
            }

            var skin = menu.AddSubMenu(new Menu("Skin Changer", "l33t.ekko.skinchanger"));
            {
                skin.AddItem(new MenuItem("l33t.ekko.skinchanger.enable", "Enable")).SetValue(false);
                skin.AddItem(new MenuItem("l33t.ekko.skinchanger.list", "Available Skins:"))
                    .SetValue(new StringList(new[] { "Classic", "Sandstorm" }));
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += args =>
                {
                    var classic = menu.Item("l33t.ekko.drawing.classic").GetValue<bool>();
                    if (Ekko.Spells[SpellSlot.Q].IsReady() && menu.Item("l33t.ekko.drawing.q").GetValue<Circle>().Active)
                    {
                        if (classic)
                        {
                            Drawing.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.Q].Range,
                                menu.Item("l33t.ekko.drawing.q").GetValue<Circle>().Color);
                        }
                        else
                        {
                            Render.Circle.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.Q].Range,
                                menu.Item("l33t.ekko.drawing.q").GetValue<Circle>().Color);
                        }
                    }

                    if (Ekko.Spells[SpellSlot.W].IsReady() && menu.Item("l33t.ekko.drawing.w").GetValue<Circle>().Active)
                    {
                        if (classic)
                        {
                            Drawing.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.W].Range,
                                menu.Item("l33t.ekko.drawing.w").GetValue<Circle>().Color);
                        }
                        else
                        {
                            Render.Circle.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.W].Range,
                                menu.Item("l33t.ekko.drawing.w").GetValue<Circle>().Color);
                        }
                    }

                    if (Ekko.Spells[SpellSlot.E].IsReady() && menu.Item("l33t.ekko.drawing.e").GetValue<Circle>().Active)
                    {
                        if (classic)
                        {
                            Drawing.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.E].Range,
                                menu.Item("l33t.ekko.drawing.e").GetValue<Circle>().Color);
                        }
                        else
                        {
                            Render.Circle.DrawCircle(
                                Ekko.Player.Position,
                                Ekko.Spells[SpellSlot.E].Range,
                                menu.Item("l33t.ekko.drawing.e").GetValue<Circle>().Color);
                        }
                    }

                    if (Ekko.Spells[SpellSlot.R].IsReady() && Ekko.EkkoGhost != null && Ekko.EkkoGhost.IsValid
                        && menu.Item("l33t.ekko.drawing.r").GetValue<Circle>().Active)
                    {
                        if (classic)
                        {
                            Drawing.DrawCircle(
                                Ekko.EkkoGhost.Position,
                                Ekko.Spells[SpellSlot.R].Range,
                                menu.Item("l33t.ekko.drawing.r").GetValue<Circle>().Color);
                        }
                        else
                        {
                            Render.Circle.DrawCircle(
                                Ekko.EkkoGhost.Position,
                                Ekko.Spells[SpellSlot.R].Range,
                                menu.Item("l33t.ekko.drawing.r").GetValue<Circle>().Color);
                        }
                    }
                };
        }

        #endregion

        #region Methods
 
        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            if (Ekko.Spells[SpellSlot.R].IsReady() && Ekko.Menu.Item("l33t.ekko.combo.rbackenable").GetValue<bool>())
            {
                if (!Ekko.OldHealth.ContainsKey(Ekko.GameTime))
                {
                    Ekko.OldHealth.Add(Ekko.GameTime, Ekko.Player.Health);
                }

                foreach (var contents in Ekko.OldHealth.ToArray().Where(contents => contents.Key + 4000 < Ekko.GameTime))
                {
                    Ekko.OldHealth.Remove(contents.Key);
                }
            }

            if (Ekko.Menu.Item("l33t.ekko.ks.use").GetValue<bool>())
            {
                Mechanics.ProcessKillsteal();
            }

            Orbwalking.AfterAttack += (unit, target) =>
                {
                    if (Ekko.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LaneClear) && Ekko.GameTime - LastAfterAttackTick > Ekko.Player.AttackDelay * 1000)
                    {
                        var targetBase = target as Obj_AI_Minion;
                        if (unit.IsMe && targetBase != null && Ekko.Spells[SpellSlot.E].IsReady())
                        {
                            if (!targetBase.BaseSkinName.Contains("minion")
                                && !targetBase.BaseSkinName.Contains("Minion")
                                && !targetBase.BaseSkinName.Contains("ward")
                                && !targetBase.BaseSkinName.Contains("Ward")
                                && !targetBase.BaseSkinName.Contains("trinket")
                                && !targetBase.BaseSkinName.Contains("Trinket"))
                            {
                                if (targetBase.Health > Ekko.Player.GetAutoAttackDamage(targetBase)
                                    || (targetBase.GetAutoAttackDamage(Ekko.Player) < Ekko.Player.Health
                                        && targetBase.Health < Ekko.Player.GetAutoAttackDamage(targetBase)))
                                {
                                    var dashPosition = targetBase.Position;
                                    for (var i = 0; i <= Ekko.Spells[SpellSlot.E].Range; ++i)
                                    {
                                        var range = targetBase.Position.Extend(Ekko.Player.Position, i);
                                        if (range.Distance(targetBase.Position) < Ekko.Player.AttackRange)
                                        {
                                            dashPosition = range;
                                        }
                                    }

                                    if (dashPosition.IsValid())
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(
                                            (int)(Ekko.Spells[SpellSlot.E].Delay * 1000) + 500 + Game.Ping / 2,
                                            () =>
                                                {
                                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                                });
                                        Ekko.Spells[SpellSlot.E].Cast(dashPosition);
                                    }
                                }
                            }
                        }

                        LastAfterAttackTick = Ekko.GameTime;
                    }
                };
            switch (Ekko.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Mechanics.ProcessSpells();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Mechanics.ProcessFarm();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mechanics.ProcessSpells(true);
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Ekko.Menu.Item("l33t.ekko.flee.enable").GetValue<bool>()
                        && Ekko.Menu.Item("l33t.ekko.flee.key").GetValue<KeyBind>().Active)
                    {
                        Mechanics.ProcessFlee();
                        Ekko.MoveTo(Game.CursorPos, Ekko.Player.BoundingRadius);
                    }

                    break;
            }
        }

        #endregion

        public static int LastAfterAttackTick { get; set; }
    }
}