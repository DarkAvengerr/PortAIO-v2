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
    public class Blitzcrank : SCommon.PluginBase.Champion
    {
        public Blitzcrank()
            : base("Blitzcrank", "SAutoCarry - Blitzcrank")
        {
            OnDraw += BeforeDraw;
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Blitzcrank.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.UseR", "Use R").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.UseRHit").Show(ar.GetNewValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.UseRHit", "Use R When Enemy Count >= ").SetValue(new Slider(2, 1, 5))).Show(combo.Item("SAutoCarry.Blitzcrank.Combo.UseR").GetValue<bool>());
            //
            Menu nograb = new Menu("Grab Filter", "SAutoCarry.Blitzcrank.Combo.Grabfilter");
            foreach (AIHeroClient enemy in HeroManager.Enemies)
                nograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Combo.Grabfilter.DontGrab" + enemy.ChampionName, string.Format("Dont Grab {0}", enemy.ChampionName)).SetValue(false));
            //
            combo.AddSubMenu(nograb);


            Menu harass = new Menu("Harass", "SAutoCarry.Blitzcrank.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Harass.UseE", "Use E").SetValue(true));


            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Blitzcrank.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Blitzcrank.LaneClear.UseR", "Use R").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Blitzcrank.LaneClear.MinMana", "Min Mana Percent").SetValue(new Slider(50, 100, 0)));

            Menu misc = new Menu("Misc", "SAutoCarry.Blitzcrank.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.InterruptQ", "Interrupt With Q").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.InterruptR", "Interrupt With R").SetValue(true));
            //
            Menu autograb = new Menu("Auto Grab (Q)", "SAutoCarry.Blitzcrank.Misc.AutoGrab");
            foreach (AIHeroClient enemy in HeroManager.Enemies)
                autograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.AutoGrab.DontGrab" + enemy.ChampionName, string.Format("Dont Grab {0}", enemy.ChampionName)).SetValue(false));
            autograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.AutoGrab.Immobile", "Auto Grab Immobile Target").SetValue(true));
            autograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.AutoGrab.Range", "Max. Grab Range").SetValue(new Slider(800, 1, 925)));
            autograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.AutoGrab.MinHp", "Min. HP Percent").SetValue(new Slider(40, 1, 100)));
            autograb.AddItem(new MenuItem("SAutoCarry.Blitzcrank.Misc.AutoGrab.Enabled", "Enabled").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            //
            misc.AddSubMenu(autograb);

            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);

            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 925f);
            Spells[Q].SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 0f);

            Spells[E] = new Spell(SpellSlot.E, 0f);

            Spells[R] = new Spell(SpellSlot.R, 550f);
        }

        public void BeforeDraw()
        {
            var text_pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            Drawing.DrawText((int)text_pos.X - 20, (int)text_pos.Y + 35, System.Drawing.Color.Aqua, "Auto Grab: " + (AutoGrabEnabled ? "On" : "Off"));
        }

        public void BeforeOrbwalk()
        {
            Spells[Q].From = (ObjectManager.Player.ServerPosition.To2D() + ((ObjectManager.Player.Direction.To2D() + ObjectManager.Player.Direction.To2D().Normalized().Perpendicular()) * ObjectManager.Player.BoundingRadius)).To3D();
            if (Spells[Q].IsReady() && AutoGrabEnabled && AutoGrabMinHealth <= ObjectManager.Player.HealthPercent && !ObjectManager.Player.UnderTurret(true))
            {
                var t = TargetSelector.GetTarget(AutoGrabRange, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null && !IsBlacklistedAutoGrab(t))
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void Combo()
        {
            if (Spells[W].IsReady() && ComboUseW)
            {
                var t = TargetSelector.GetTarget(1000, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].Cast();
            }

            if (Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null && !IsBlacklistedComboGrab(t))
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }

            if (Spells[R].IsReady() && ComboUseR)
            {
                var t = TargetSelector.GetTarget(Spells[R].Range - 10, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[R].Cast();
            }
        }

        public void Harass()
        {
            if (Spells[Q].IsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana)
                return;

            if (Spells[R].IsReady() && LaneClearR)
            {
                if (MinionManager.GetMinions(Spells[R].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Count() > 4)
                    Spells[R].Cast();
            }
        }

        public bool IsBlacklistedAutoGrab(AIHeroClient hero)
        {
            return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.AutoGrab.DontGrab" + hero.ChampionName).GetValue<bool>();
        }

        public bool IsBlacklistedComboGrab(AIHeroClient hero)
        {
            return ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.Grabfilter.DontGrab" + hero.ChampionName).GetValue<bool>();
        }

        protected override void OrbwalkingEvents_BeforeAttack(SCommon.Orbwalking.BeforeAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
            {
                if (Spells[E].IsReady() && HarassUseE)
                {
                    Spells[E].Cast();
                    args.Process = false;
                    return;
                }
            }

            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                if (Spells[E].IsReady() && ComboUseE)
                {
                    Spells[E].Cast();
                    args.Process = false;
                    return;
                }
            }
        }

        protected override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsEnemy && sender.IsChampion() && Data.IsImmobilizeBuff(args.Buff.Type) && AutoGrabMinHealth <= ObjectManager.Player.HealthPercent)
            {
                if (Spells[Q].IsReady() && sender.IsValidTarget(Spells[Q].Range) && AutoGrabImmobile && !IsBlacklistedAutoGrab(sender as AIHeroClient) && !Spells[Q].GetCollisionFlags(ObjectManager.Player.ServerPosition.To2D(), sender.ServerPosition.To2D()).HasFlag(SPrediction.Collision.Flags.Minions))
                    Spells[Q].Cast(sender.ServerPosition);
            }
        }

        protected override void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (InterruptQ && Spells[Q].IsReady() && sender.IsValidTarget(Spells[Q].Range) && AutoGrabMinHealth <= ObjectManager.Player.HealthPercent)
                Spells[Q].SPredictionCast(sender, HitChance.High);

            if (InterruptR && Spells[R].IsReady() && sender.IsValidTarget(Spells[R].Range))
                Spells[R].Cast();
        }


        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Combo.UseR").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Harass.UseE").GetValue<bool>(); }
        }

        public bool LaneClearR
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.LaneClear.UseR").GetValue<bool>(); }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool InterruptQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.InterruptQ").GetValue<bool>(); }
        }

        public bool InterruptR
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.InterruptR").GetValue<bool>(); }
        }

        public bool AutoGrabImmobile
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.AutoGrab.Immobile").GetValue<bool>(); }
        }

        public int AutoGrabRange
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.AutoGrab.Range").GetValue<Slider>().Value; }
        }

        public int AutoGrabMinHealth
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.AutoGrab.MinHp").GetValue<Slider>().Value; }
        }

        public bool AutoGrabEnabled
        {
            get { return ConfigMenu.Item("SAutoCarry.Blitzcrank.Misc.AutoGrab.Enabled").GetValue<KeyBind>().Active; }
        }
    }
}
