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

using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Corki : SCommon.PluginBase.Champion
    {
        public Corki()
            : base("Corki", "SAutoCarry - Corki")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Corki.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Corki.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Corki.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Corki.Combo.UseR", "Use R").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.Corki.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Corki.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Corki.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Corki.Harass.UseR", "Use R").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Corki.Harass.RStacks").Show(ar.GetNewValue<bool>());
            harass.AddItem(new MenuItem("SAutoCarry.Corki.Harass.RStacks", "Keep R Stacks").SetValue(new Slider(3, 0, 7))).Show(harass.Item("SAutoCarry.Corki.Harass.UseR").GetValue<bool>());
            harass.AddItem(new MenuItem("SAutoCarry.Corki.Harass.MinMana", "Min Mana Percent").SetValue(new Slider(30, 100, 0)));

            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Corki.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Corki.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Corki.LaneClear.UseR", "Use R").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Corki.LaneClear.MinMana", "Min Mana Percent").SetValue(new Slider(50, 100, 0)));

            Menu misc = new Menu("Misc", "SAutoCarry.Corki.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Corki.Misc.RKillSteal", "KS With R").SetValue(true));

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 825f);
            Spells[Q].SetSkillshot(0.35f, 150f, 1000f, false, SkillshotType.SkillshotCircle);

            Spells[W] = new Spell(SpellSlot.W, 0f);

            Spells[E] = new Spell(SpellSlot.E, 500f);

            Spells[R] = new Spell(SpellSlot.R, 1225f);
            Spells[R].SetSkillshot(0.25f, 40f, 1950f, true, SkillshotType.SkillshotLine);

        }

        public void BeforeOrbwalk()
        {
            if (KillStealR)
                KillSteal();
        }

        public void Combo()
        {

            if (Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }

            if (Spells[R].IsReady() && ComboUseR && Spells[R].Instance.Ammo > 0)
            {
                var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[R].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].IsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if(t != null && ObjectManager.Player.IsFacing(t))
                    Spells[E].Cast();
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
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null && ObjectManager.Player.IsFacing(t))
                    Spells[E].Cast();
            }

            if (Spells[R].IsReady() && HarassUseR && Spells[R].Instance.Ammo > HarassRStack)
            {
                var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                {
                    var pred = Spells[R].GetSPrediction(t);
                    if (pred.HitChance == HitChance.Collision)
                    {
                        var col = pred.CollisionResult.Units.FirstOrDefault();
                        if (col != null && col.Distance(ObjectManager.Player.ServerPosition) < Spells[R].Range)
                        {
                            if (col.Distance(pred.UnitPosition) < (ObjectManager.Player.HasBuff("corkimissilebarragecounterbig") ? 150f : 75f))
                                Spells[R].Cast(pred.CastPosition);
                        }
                    }
                    else if (pred.HitChance >= HitChance.High)
                        Spells[R].Cast(pred.CastPosition);
                }
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
                    var farm = Spells[R].GetCircularFarmLocation(MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                    if (farm.MinionsHit > 4)
                        Spells[Q].Cast(farm.Position);
                }

            if (Spells[R].IsReady() && LaneClearR)
            {
                var farm = Spells[R].GetCircularFarmLocation(MinionManager.GetMinions(Spells[R].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                if (farm.MinionsHit > 4)
                    Spells[R].Cast(farm.Position);
            }
        }

        public void KillSteal()
        {
            if (!Spells[R].IsReady() || Spells[R].Instance.Ammo == 0)
                return;

            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[R].Range) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if ((ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)) > target.Health + 20)
                    Spells[R].SPredictionCast(target, HitChance.High);
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Combo.UseR").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Harass.UseE").GetValue<bool>(); }
        }

        public bool HarassUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Harass.UseR").GetValue<bool>(); }
        }

        public int HarassRStack
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Harass.RStacks").GetValue<Slider>().Value; }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool LaneClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearR
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.LaneClear.UseR").GetValue<bool>(); }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool KillStealR
        {
            get { return ConfigMenu.Item("SAutoCarry.Corki.Misc.RKillSteal").GetValue<bool>(); }
        }
    }
}
