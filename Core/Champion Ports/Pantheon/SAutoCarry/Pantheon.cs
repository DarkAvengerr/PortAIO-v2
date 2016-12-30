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
using SUtility.Drawings;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Pantheon : SCommon.PluginBase.Champion
    {
        public Pantheon()
            : base("Pantheon", "SAutoCarry - Pantheon")
        {
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
            OnUpdate += BeforeOrbwalk;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Pantheon.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Pantheon.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Pantheon.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Pantheon.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Pantheon.Combo.UseTiamat", "Use Tiamat/Hydra").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.Pantheon.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Pantheon.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Pantheon.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("SAutoCarry.Pantheon.Harass.MinMana", "Min. Mana %").SetValue(new Slider(40, 0, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.Pantheon.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Pantheon.Misc.LaneClearQ", "Use Q On LaneClear").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Pantheon.Misc.LaneClear.MinMana", "Min Mana Percent").SetValue(new Slider(50, 100, 0)));
            misc.AddItem(new MenuItem("SAutoCarry.Pantheon.Misc.InterruptW", "Interrupt Spells With W").SetValue(true));
            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public void BeforeOrbwalk()
        {
            if (HarassToggle && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None)
                Harass();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 600f);
            Spells[Q].SetTargetted(0.2f, 1700f);
            Spells[W] = new Spell(SpellSlot.W, 600f);
            Spells[W].SetTargetted(0.2F, 1700f);
            Spells[E] = new Spell(SpellSlot.E, 600f);
            Spells[E].SetSkillshot(0.25f, 15f * 2 * (float)Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

            Spells[R] = new Spell(SpellSlot.R, 5500f);
        }

        public void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
            if (t != null)
            {
                if (Spells[Q].IsReady() && ComboUseQ)
                    Spells[Q].CastOnUnit(t);

                if (Spells[W].IsReady() && ComboUseW)
                    Spells[W].CastOnUnit(t);

                if (Spells[E].IsReady() && t.IsImmobilized() && ComboUseE)
                    Spells[E].Cast(Spells[E].GetPrediction(t).CastPosition);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassManaPercent)
                return;

            var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
            if (t != null)
            {
                if (Spells[Q].IsReady() && HarassUseQ)
                    Spells[Q].CastOnUnit(t);
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana)
                return;

            var minion = MinionManager.GetMinions(600f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if(minion != null)
            {
                if (Spells[W].IsReady())
                    Spells[W].CastOnUnit(minion);

                if (Spells[E].IsReady())
                    Spells[E].Cast(minion.ServerPosition);

                if (Spells[Q].IsReady())
                    Spells[Q].CastOnUnit(minion);
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(SCommon.Orbwalking.AfterAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                if (ComboUseTiamat)
                {
                    if (Items.HasItem(3077) && Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    else if (Items.HasItem(3074) && Items.CanUseItem(3074))
                        Items.UseItem(3074);
                    else if (Items.HasItem(3748) && Items.CanUseItem(3748)) //titanic
                        Items.UseItem(3748);
                }
            }
            else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
            {
                if (Spells[Q].IsReady() && LaneClearQ)
                {
                    var minions = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);
                    foreach (var minion in minions)
                    {
                        if (minion.Distance(ObjectManager.Player.ServerPosition) > 150 && minion.Health < Spells[Q].GetDamage(minion))
                        {
                            Spells[Q].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }
        }

        protected override void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if(InterruptW && sender.IsValidTarget(Spells[W].Range) && Spells[W].IsReady())
                Spells[W].CastOnUnit(sender);
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Combo.UseTiamat").GetValue<bool>(); }
        }
        
        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Combo.UseQ").GetValue<bool>(); }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public int HarassManaPercent
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool LaneClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Misc.LaneClearQ").GetValue<bool>(); }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool InterruptW
        {
            get { return ConfigMenu.Item("SAutoCarry.Pantheon.Misc.InterruptW").GetValue<bool>(); }
        }
    }
}