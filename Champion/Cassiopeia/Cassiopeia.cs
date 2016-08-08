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
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Cassiopeia : SCommon.PluginBase.Champion
    {
        public Cassiopeia()
            : base("Cassiopeia", "SAutoCarry - Cassiopeia")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Cassiopeia.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Combo.UseR", "Use R Min").SetValue(new Slider(1, 1, 5)));


            Menu harass = new Menu("Harass", "SAutoCarry.Cassiopeia.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Harass.MinMana", "Min Mana Percent").SetValue(new Slider(30, 100, 0)));

            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Cassiopeia.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Cassiopeia.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Cassiopeia.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Cassiopeia.LaneClear.UseE", "Use E").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Cassiopeia.LaneClear.Toggle", "Enabled Spell Farm").SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
            laneclear.AddItem(new MenuItem("SAutoCarry.Cassiopeia.LaneClear.MinMana", "Min Mana Percent").SetValue(new Slider(50, 100, 0)));

            Menu misc = new Menu("Misc", "SAutoCarry.Cassiopeia.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Misc.EKillSteal", "KS With E").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Cassiopeia.Misc.KSOnlyPoision").Show(ar.GetNewValue<bool>());
            misc.AddItem(new MenuItem("SAutoCarry.Cassiopeia.Misc.KSOnlyPoision", "Only KS If Target Has poison").SetValue(true)).Show(misc.Item("SAutoCarry.Cassiopeia.Misc.EKillSteal").GetValue<bool>());

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);

            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 850f);
            Spells[Q].SetSkillshot(0.75f, Spells[Q].Instance.SData.CastRadius, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spells[W] = new Spell(SpellSlot.W, 850f);
            Spells[W].SetSkillshot(0.5f, Spells[W].Instance.SData.CastRadius, Spells[W].Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 750f);
            Spells[E].SetTargetted(0.2f, float.MaxValue);

            Spells[R] = new Spell(SpellSlot.R, 460f);
            Spells[R].SetSkillshot(0.3f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        public void BeforeOrbwalk()
        {
            if (KillStealE)
                KillSteal();
        }

        public void Combo()
        {
            if (Spells[Q].LSIsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }

            if (!Spells[Q].LSIsReady() && Spells[W].LSIsReady() && ComboUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].LSIsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null && t.HasBuffOfType(BuffType.Poison))
                    Spells[E].CastOnUnit(t);
            }


            if (Spells[R].LSIsReady())
            {
                if (ComboUseRMin == 1)
                {
                    var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                    if (t != null)
                    {
                        var pred = Spells[R].GetSPrediction(t);
                        if (pred.HitChance >= HitChance.High)
                        {
                            if (t.LSIsFacing(ObjectManager.Player))
                                Spells[R].Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    var pred = Spells[R].GetAoeSPrediction();
                    int c = 0;
                    if (pred.HitCount >= ComboUseRMin)
                    {
                        foreach(var col in pred.CollisionResult.Units)
                        {
                            if (col.Type == GameObjectType.AIHeroClient)
                            {
                                if (col.LSIsFacing(ObjectManager.Player))
                                    c++;
                            }
                        }
                    }

                    if (c >= ComboUseRMin / 2)
                        Spells[R].Cast(pred.CastPosition);
                }
            }
        }


        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            if (Spells[Q].LSIsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }

            if (!Spells[Q].LSIsReady() && Spells[W].LSIsReady() && HarassUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].LSIsReady() && HarassUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null && t.HasBuffOfType(BuffType.Poison))
                    Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana || !LaneClearToggle)
                return;

            if (Spells[Q].LSIsReady() && LaneClearQ)
            {
                var farm = Spells[Q].GetCircularFarmLocation(MinionManager.GetMinions(Spells[Q].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                if (farm.MinionsHit > 1)
                    Spells[Q].Cast(farm.Position);
            }

            if (!Spells[Q].LSIsReady() && Spells[W].LSIsReady() && LaneClearW)
            {
                var farm = Spells[W].GetCircularFarmLocation(MinionManager.GetMinions(Spells[W].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                if (farm.MinionsHit > 2)
                    Spells[W].Cast(farm.Position);
            }

            if (Spells[E].LSIsReady() && LaneClearE)
            {
                var minion = MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Where(p => p.HasBuffOfType(BuffType.Poison) && Spells[E].IsKillable(p)).FirstOrDefault();
                Spells[E].CastOnUnit(minion);
            }

        }

        public void KillSteal()
        {
            if (!Spells[E].LSIsReady())
                return;

            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells[E].Range) && !x.HasBuffOfType(BuffType.Invulnerability) && (!KillStealOnlyPoison || x.HasBuffOfType(BuffType.Poison))))
            {
                if ((ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.E)) > target.Health + 20)
                    Spells[E].CastOnUnit(target);
            }
        }


        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Combo.UseW").GetValue<bool>(); }
        }

        public int ComboUseRMin
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Combo.UseR").GetValue<Slider>().Value; }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Harass.UseW").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Harass.UseE").GetValue<bool>(); }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool LaneClearQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearW
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.LaneClear.UseW").GetValue<bool>(); }
        }

        public bool LaneClearE
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.LaneClear.UseE").GetValue<bool>(); }
        }

        public bool LaneClearToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.LaneClear.Toggle").GetValue<KeyBind>().Active; }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool KillStealE
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Misc.EKillSteal").GetValue<bool>(); }
        }

        public bool KillStealOnlyPoison
        {
            get { return ConfigMenu.Item("SAutoCarry.Cassiopeia.Misc.KSOnlyPoision").GetValue<bool>(); }
        }
    }
}
