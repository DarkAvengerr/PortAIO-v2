using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.Database;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Orbwalking;
using SUtility.Drawings;
using EloBuddy;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;

namespace SAutoCarry.Champions
{
    public class DrMundo : SCommon.PluginBase.Champion
    {
        public DrMundo()
            : base ("DrMundo", "SAutoCarry - Dr. Mundo")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.DrMundo.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.DrMundo.Combo.UseQ", "Use Q").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseQHP").Show(ar.GetNewValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.DrMundo.Combo.UseQHP", "Use Q Min HP %").SetValue(new Slider(10, 0, 100))).Show(combo.Item("SAutoCarry.DrMundo.Combo.UseQ").GetValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.DrMundo.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.DrMundo.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.DrMundo.Combo.UseR", "Use R Min HP % <").SetValue(new Slider(20, 0, 100)));

            Menu harass = new Menu("Harass", "SAutoCarry.DrMundo.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.DrMundo.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.DrMundo.Harass.MinHP", "Min HP %").SetValue(new Slider(30, 0, 100)));
            harass.AddItem(new MenuItem("SAutoCarry.DrMundo.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            
            Menu laneclear = new Menu("LaneClear", "SAutoCarry.DrMundo.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.DrMundo.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.DrMundo.LaneClear.UseW", "Use W").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.DrMundo.LaneClear.UseWMin").Show(ar.GetNewValue<bool>());
            laneclear.AddItem(new MenuItem("SAutoCarry.DrMundo.LaneClear.UseWMin", "Use W Min. Minion").SetValue(new Slider(3, 0, 12))).Show(laneclear.Item("SAutoCarry.DrMundo.LaneClear.UseW").GetValue<bool>());
            laneclear.AddItem(new MenuItem("SAutoCarry.DrMundo.LaneClear.UseE", "Use E").SetValue(false));

            Menu misc = new Menu("Misc", "SAutoCarry.DrMundo.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.DrMundo.Misc.KillStealQ", "KS With Q").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.DrMundo.Misc.AutoR", "Enable Auto R").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.DrMundo.Misc.AutoRPercent").Show(ar.GetNewValue<bool>());
            misc.AddItem(new MenuItem("SAutoCarry.DrMundo.Misc.AutoRPercent", "Auto R When HP % <").SetValue(new Slider(20, 0, 100))).Show(misc.Item("SAutoCarry.DrMundo.Misc.AutoR").GetValue<bool>());

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 900f);
            Spells[Q].SetSkillshot(0.25f, 65f, 1500f, true, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 325f);

            Spells[E] = new Spell(SpellSlot.E);

            Spells[R] = new Spell(SpellSlot.R);
        }

        public void BeforeOrbwalk()
        {
            if (KillStealQ)
                KillSteal();

            if (Spells[R].LSIsReady() && AutoR && ObjectManager.Player.HealthPercent < AutoRPercent)
                Spells[R].Cast();

            if (HarassToggle)
                Harass();

            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None && ObjectManager.Player.HasBuff("BurningAgony") && Spells[W].LSIsReady())
                Spells[W].Cast();
        }

        public void Combo()
        {
            if (Spells[Q].LSIsReady() && ComboUseQ && ObjectManager.Player.HealthPercent > ComboUseQHP)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.Low);
            }

            if (Spells[W].LSIsReady() && ComboUseW && !ObjectManager.Player.HasBuff("BurningAgony"))
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[W].Cast();
            }
            else if (Spells[W].LSIsReady() && ObjectManager.Player.HasBuff("BurningAgony") && !HeroManager.Enemies.Any(p => p.LSIsValidTarget(400f)))
                Spells[W].Cast();

            if (Spells[R].LSIsReady() && ObjectManager.Player.HealthPercent < ComboUseRHP)
                Spells[R].Cast();
        }

        public void Harass()
        {
            if (ObjectManager.Player.HealthPercent < HarassMinHP)
                return;

            if (Spells[Q].LSIsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void LaneClear()
        {
            var minion = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion.IsJungleMinion())
            {
                if (Spells[Q].LSIsReady())
                    Spells[Q].Cast(minion.ServerPosition);

                if (Spells[W].LSIsReady() && !ObjectManager.Player.HasBuff("BurningAgony"))
                    Spells[W].Cast();
            }
            else
            {
                if (Spells[Q].LSIsReady() && LaneClearUseQ && Spells[Q].IsKillable(minion))
                    Spells[Q].Cast(minion.ServerPosition);

                if (Spells[W].LSIsReady() && !ObjectManager.Player.HasBuff("BurningAgony") && LaneClearUseW && ObjectManager.Get<Obj_AI_Minion>().Count(p => p.LSIsValidTarget(Spells[W].Range) && MinionManager.IsMinion(p)) >= LaneClearUseWMinMinion)
                    Spells[W].Cast();
            }
        }

        public void KillSteal()
        {
            if (!Spells[Q].LSIsReady())
                return;

            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells[Q].Range) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if ((ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q)) > target.Health + 20)
                    Spells[Q].SPredictionCast(target, HitChance.High);
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(AfterAttackArgs args)
        {
            if (Spells[E].LSIsReady() && !args.Target.IsDead)
            {
                if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && ComboUseE)
                    Spells[E].Cast();
                else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear && LaneClearUseE)
                    Spells[E].Cast();
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseQ").GetValue<bool>(); }
        }

        public int ComboUseQHP
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseQHP").GetValue<Slider>().Value; }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseE").GetValue<bool>(); }
        }

        public int ComboUseRHP
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Combo.UseR").GetValue<Slider>().Value; }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Harass.UseQ").GetValue<bool>(); }
        }

        public int HarassMinHP
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Harass.MinHP").GetValue<Slider>().Value; }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public bool LaneClearUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.LaneClear.UseW").GetValue<bool>(); }
        }

        public int LaneClearUseWMinMinion
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.LaneClear.UseWMin").GetValue<Slider>().Value; }
        }

        public bool LaneClearUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.LaneClear.UseE").GetValue<bool>(); }
        }

        public bool KillStealQ
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Misc.KillStealQ").GetValue<bool>(); }
        }

        public bool AutoR
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Misc.AutoR").GetValue<bool>(); }
        }

        public int AutoRPercent
        {
            get { return ConfigMenu.Item("SAutoCarry.DrMundo.Misc.AutoRPercent").GetValue<Slider>().Value; }
        }
    }
}
