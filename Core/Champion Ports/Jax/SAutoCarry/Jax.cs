using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.Database;
using SCommon.PluginBase;
using SPrediction;
using SCommon.Orbwalking;
using SCommon.Maths;

using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Jax : SCommon.PluginBase.Champion
    {
        public Jax()
            : base("Jax", "SAutoCarry - Jax")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Jax.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.UseE", "Use E").SetValue(true)).ValueChanged += (s, ar) => { ConfigMenu.Item("SAutoCarry.Jax.Combo.EStun").Show(ar.GetNewValue<bool>()); ConfigMenu.Item("SAutoCarry.Jax.Combo.EBeforeJump").Show(ar.GetNewValue<bool>()); };
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.EStun", "Stun With E After Jump").SetValue(new KeyBind('T', KeyBindType.Toggle)).Show(combo.Item("SAutoCarry.Jax.Combo.UseE").GetValue<bool>()));
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.EBeforeJump", "Use E Before Q").SetValue(new KeyBind('Z', KeyBindType.Toggle)).Show(combo.Item("SAutoCarry.Jax.Combo.UseE").GetValue<bool>()));
            combo.AddItem(new MenuItem("SAutoCarry.Jax.Combo.UseTiamat", "Use Tiamat/Hydra").SetValue(true));


            Menu harass = new Menu("Harass", "SAutoCarry.Jax.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Jax.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Jax.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Jax.Harass.UseE", "Use E").SetValue(true));


            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Jax.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.LaneClear.UseQ", "Use Q").SetValue(false)).SetTag(0);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.LaneClear.UseW", "Use W").SetValue(true)).SetTag(0);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.LaneClear.UseE", "Use E").SetValue(true)).SetTag(0);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.JungleClear.UseQ", "Use Q").SetValue(true)).SetTag(1);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.JungleClear.UseW", "Use W").SetValue(true)).SetTag(1);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.JungleClear.UseE", "Use E").SetValue(true)).SetTag(1);
            laneclear.AddItem(new MenuItem("SAutoCarry.Jax.LaneClear.Mode", "Show Settings for: ").SetValue(new StringList(new[] { "Lane Clear", "Jungle Clear" }))).SetTag(2).ValueChanged += (s, ar) => LaneClearMenuSwitch(ar.GetNewValue<StringList>().SelectedIndex);

            Menu misc = new Menu("Misc", "SautoCarry.Jax.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Jax.Misc.WJump", "Ward Jump").SetValue(new KeyBind('G', KeyBindType.Press)));

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);

            ConfigMenu.AddToMainMenu();

            LaneClearMenuSwitch(ConfigMenu.Item("SAutoCarry.Jax.LaneClear.Mode").GetValue<StringList>().SelectedIndex);
        }

        private void LaneClearMenuSwitch(int tag)
        {
            ConfigMenu.SubMenu("SAutoCarry.Jax.LaneClear").Items.ForEach(p => { if (p.Tag != 2) p.Show(p.Tag == tag); });
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 700f);

            Spells[W] = new Spell(SpellSlot.W);

            Spells[E] = new Spell(SpellSlot.E, ObjectManager.Player.BoundingRadius * 2 + 187.5f);

            Spells[R] = new Spell(SpellSlot.R);
        }

        public void BeforeOrbwalk()
        {
            if (WardJumpActive)
                WardJump();
        }

        public void Combo()
        {
            if (Spells[E].IsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                {
                    if(!ObjectManager.Player.HasBuff("jaxleapstrike") || ComboEStun)
                        Spells[E].Cast();
                }
            }

            if (Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                {
                    if (ComboEBeforeQ)
                    {
                        if (Spells[E].IsReady() && ComboUseE && !ObjectManager.Player.HasBuff("jaxleapstrike"))
                            Spells[E].Cast();
                    }
                    if (!t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(t)) || Spells[Q].IsKillable(t))
                        Spells[Q].CastOnUnit(t);
                }
            }
        }

        public void Harass()
        {
            if (Spells[Q].IsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].CastOnUnit(t);
            }

            if (Spells[E].IsReady() && HarassUseE)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null && !ObjectManager.Player.HasBuff("jaxleapstrike"))
                    Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            var minion = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (minion.IsJungleMinion())
                {
                    if (Spells[Q].IsReady() && minion.IsJungleMinion() && JungleClearQ)
                        Spells[Q].CastOnUnit(minion);
                }
                else
                {
                    if (Spells[Q].IsReady() && Spells[Q].IsKillable(minion) && LaneClearQ)
                        Spells[Q].CastOnUnit(minion);
                }
            }

            if (Spells[E].IsReady())
            {
                if (LaneClearE)
                {
                    if (MinionManager.GetMinions(Spells[E].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Count() > 4)
                        Spells[E].Cast();
                }
                
                if (JungleClearE)
                {
                    if (MinionManager.GetMinions(Spells[E].Range + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any(p => p.GetJunglePriority() == 1))
                        Spells[E].Cast();
                }
            }
        }

        public void WardJump()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Spells[Q].IsReady())
            {
                var poly = SCommon.Maths.ClipperWrapper.DefineCircle(ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, Spells[Q].Range - 300).To2D(), 300);
                var unit = ObjectManager.Get<Obj_AI_Base>()
                    .Where(p => p.IsAlly && !p.IsMe && p.IsTargetable && !p.Name.Contains("turret")
                        && p.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < Spells[Q].Range 
                        && !poly.IsOutside(p.ServerPosition.To2D()))
                    .OrderByDescending(q => q.Distance(ObjectManager.Player.ServerPosition))
                    .FirstOrDefault();

                if (unit != null)
                {
                    Spells[Q].CastOnUnit(unit);
                    return;
                }

                var slot = Items.GetWardSlot().SpellSlot;
                if (slot != SpellSlot.Unknown)
                    ObjectManager.Player.Spellbook.CastSpell(slot, ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 600));
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(SCommon.Orbwalking.AfterAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
            {
                if (Spells[W].IsReady() && HarassUseW)
                {
                    Spells[W].Cast();
                    return;
                }
            }
            else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                if (Spells[W].IsReady() && ComboUseW)
                {
                    Spells[W].Cast();
                    return;
                }

                if (ComboUseTiamat)
                {
                    if (Items.HasItem(3077) && Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    else if (Items.HasItem(3074) && Items.CanUseItem(3074))
                        Items.UseItem(3074);
                    else if (Items.HasItem(3748) && Items.CanUseItem(3748)) //titanic
                        Items.UseItem(3748);

                    return;
                }
            }
            else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
            {
                if (args.Target is Obj_AI_Base && Spells[W].IsReady() && args.Target.Health > ObjectManager.Player.GetAutoAttackDamage(args.Target as Obj_AI_Base))
                {
                    if (args.Target.IsJungleMinion())
                    {
                        if (JungleClearW)
                            Spells[W].Cast();
                    }
                    else
                    {
                        if (LaneClearW)
                            Spells[W].Cast();
                    }
                }
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboEStun
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.EStun").GetValue<KeyBind>().Active; }
        }

        public bool ComboEBeforeQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.EBeforeJump").GetValue<KeyBind>().Active; }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Harass.UseW").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Harass.UseE").GetValue<bool>(); }
        }

        public bool LaneClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearW
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.LaneClear.UseW").GetValue<bool>(); }
        }

        public bool LaneClearE
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.LaneClear.UseE").GetValue<bool>(); }
        }

        public bool JungleClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.JungleClear.UseQ").GetValue<bool>(); }
        }

        public bool JungleClearW
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.JungleClear.UseW").GetValue<bool>(); }
        }

        public bool JungleClearE
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.JungleClear.UseE").GetValue<bool>(); }
        }

        public bool ComboUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Combo.UseTiamat").GetValue<bool>(); }
        }

        public bool WardJumpActive
        {
            get { return ConfigMenu.Item("SAutoCarry.Jax.Misc.WJump").GetValue<KeyBind>().Active; }
        }
    }
}
