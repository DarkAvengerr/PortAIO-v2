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
    public class Darius : SCommon.PluginBase.Champion
    {
        public Darius()
            : base("Darius", "SAutoCarry - Darius")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Darius.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Darius.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Darius.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Darius.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Darius.Combo.UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Darius.Combo.UseTiamat", "Use Tiamat/Hydra").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.Darius.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Darius.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Darius.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Darius.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Darius.Harass.MinMana", "Min Mana Percent").SetValue(new Slider(30, 100, 0)));

            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Darius.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Darius.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Darius.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Darius.LaneClear.MinMana", "Min Mana Percent").SetValue(new Slider(50, 100, 0)));

            Menu misc = new Menu("Misc", "SAutoCarry.Darius.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Darius.Misc.AutoQHP", "Auto Harass Q HP").SetValue(new Slider(50, 100, 0)));
            misc.AddItem(new MenuItem("SAutoCarry.Darius.Misc.RKillSteal", "KS With R").SetValue(true));

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);

            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 425f);

            Spells[W] = new Spell(SpellSlot.W, 200f);

            Spells[E] = new Spell(SpellSlot.E, 550f);
            Spells[E].SetSkillshot(0.20f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);

            Spells[R] = new Spell(SpellSlot.R, 460f);

        }

        public void BeforeOrbwalk()
        {

            if (Spells[Q].IsReady() && ObjectManager.Player.HealthPercent < AutoQHP)
            {
                if (HeroManager.Enemies.Any(p => p.IsValidTarget(Spells[Q].Range)))
                    Spells[Q].Cast();
            }

            if (KillStealR)
                KillSteal();
        }

        public void Combo()
        {

            if (Spells[E].IsReady() && ComboUseR)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[E].SPredictionCast(t, HitChance.High);
            }

            if (Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].Cast(t);
            }

            if (Spells[R].IsReady() && ComboUseR)
            {
                var t = HeroManager.Enemies.Where(p => p.IsValidTarget(Spells[R].Range) && p.Health < CalculateDamageR(p)).OrderBy(q => q.GetPriority()).FirstOrDefault();
                if (t != null)
                    Spells[R].CastOnUnit(t);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            if (Spells[Q].IsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].IsReady() && HarassUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana)
                return;

            var minion = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)

                if (Spells[Q].IsReady() && LaneClearQ)
                {
                    if (MinionManager.GetMinions(Spells[Q].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Count() > 4)
                        Spells[Q].Cast();
                }

            if (Spells[W].IsReady() && LaneClearW)
            {
                if (Spells[W].GetDamage(minion) > minion.Health)
                {
                    Spells[W].Cast();
                    return;
                }
            }
        }

        public void KillSteal()
        {
            if (!Spells[R].IsReady())
                return;

            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[R].Range) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if (CalculateDamageR(target) > target.Health + 20)
                    Spells[R].CastOnUnit(target);
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

            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
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
                    else if (Items.HasItem(3748) && Items.CanUseItem(3748))
                        Items.UseItem(3748);

                    return;
                }
            }
        }

        public override double CalculateDamageR(AIHeroClient target)
        {
            if (Spells[R].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) * (1f + ObjectManager.Player.GetBuffCount("dariushemo") * 0.2f);

            return 0.0d;
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Combo.UseR").GetValue<bool>(); }
        }

        public bool ComboUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Combo.UseTiamat").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.UseW").GetValue<bool>(); }
        }

        public int AutoQHP
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Misc.AutoQHP").GetValue<Slider>().Value; }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.UseE").GetValue<bool>(); }
        }

        public bool HarassUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.UseR").GetValue<bool>(); }
        }

        public int HarassRStack
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.RStacks").GetValue<Slider>().Value; }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool LaneClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearW
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.LaneClear.UseW").GetValue<bool>(); }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool KillStealR
        {
            get { return ConfigMenu.Item("SAutoCarry.Darius.Misc.RKillSteal").GetValue<bool>(); }
        }
    }
}
