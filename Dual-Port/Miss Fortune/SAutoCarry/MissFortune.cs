using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Maths;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class MissFortune : SCommon.PluginBase.Champion
    {
        private GameObject m_lastTarget;
        public MissFortune()
            : base ("MissFortune", "SAutoCarry - Miss Fortune")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            m_lastTarget = ObjectManager.Player;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.MissFortune.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.MissFortune.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.MissFortune.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.MissFortune.Combo.UseE", "Use E").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.MissFortune.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.MissFortune.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.MissFortune.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("SAutoCarry.MissFortune.Harass.MinMana", "Min. Mana").SetValue(new Slider(30, 0, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.MissFortune.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.MissFortune.Misc.AntiGapCloserE", "AntiGapCloser With E").SetValue(true));

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 650f);

            Spells[W] = new Spell(SpellSlot.W, 0f);

            Spells[E] = new Spell(SpellSlot.E, 800f);
            Spells[E].SetSkillshot(0.25f, 100f, 0f, false, SkillshotType.SkillshotCircle);

            Spells[R] = new Spell(SpellSlot.R, 0f);
        }

        public void BeforeOrbwalk()
        {
            if (HarassToggle)
                Harass();
        }

        public void Combo()
        {
            if (Spells[E].IsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[E].SPredictionCast(t, HitChance.High);
            }
        }

        public void Harass()
        {
            if (Spells[Q].IsReady() && HarassUseQ)
                BounceQ();
        }

        private void BounceQ()
        {
            var minions = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly).Where(p => !p.IsMoving);
            List<Obj_AI_Base> nonKillableMinions = new List<Obj_AI_Base>();
            foreach (var minion in minions)
            {
                if (Spells[Q].IsKillable(minion)) //prio killable minions to deal more dmg
                {
                    var hitbox1 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 40, 500f);
                    var hitbox2 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 20, 500f);
                    //var hitbox3 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 110, 300f);
                    //var hitbox4 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 160, 150f);

                    if (HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 500f && p.NetworkId == m_lastTarget.NetworkId && !hitbox1.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                    {
                        Spells[Q].CastOnUnit(minion);
                        return;
                    }
                    else if (!ObjectManager.Get<Obj_AI_Base>().Any(p => p.IsValidTarget(1200f) && p.IsEnemy && !p.IsChampion() && !hitbox2.IsOutside(p.ServerPosition.To2D()))
                        && HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 500f && (TargetSelector.SelectedTarget == null || TargetSelector.SelectedTarget.NetworkId == p.NetworkId) && !hitbox2.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                    {
                        Spells[Q].CastOnUnit(minion);
                        return;
                    }
                    else if (!ObjectManager.Get<Obj_AI_Base>().Any(p => p.IsValidTarget(1200f) && p.IsEnemy && !p.IsChampion() && !hitbox1.IsOutside(p.ServerPosition.To2D()))
                        && HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 500f && (TargetSelector.SelectedTarget == null || TargetSelector.SelectedTarget.NetworkId == p.NetworkId) && !hitbox1.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                    {
                        Spells[Q].CastOnUnit(minion);
                        return;
                    }
                }
                else
                    nonKillableMinions.Add(minion);
            }

            foreach (var minion in nonKillableMinions)
            {
                var hitbox1 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 40, 500f);
                var hitbox2 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 20, 500f);
                //var hitbox3 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 110, 300f);
                //var hitbox4 = ClipperWrapper.DefineSector(minion.ServerPosition.To2D(), (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized(), 160, 150f);

                if (HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 300 && p.NetworkId == m_lastTarget.NetworkId && !hitbox1.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                {
                    Spells[Q].CastOnUnit(minion);
                    return;
                }
                else if (!ObjectManager.Get<Obj_AI_Base>().Any(p => p.IsValidTarget(1200f) && p.IsEnemy && !p.IsChampion() && !hitbox2.IsOutside(p.ServerPosition.To2D()))
                    && HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 500f && (TargetSelector.SelectedTarget == null || TargetSelector.SelectedTarget.NetworkId == p.NetworkId) && !hitbox2.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                {
                    Spells[Q].CastOnUnit(minion);
                    return;
                }
                else if (!ObjectManager.Get<Obj_AI_Base>().Any(p => p.IsValidTarget(1200f) && p.IsEnemy && !p.IsChampion() && !hitbox1.IsOutside(p.ServerPosition.To2D()))
                    && HeroManager.Enemies.Any(p => minion.Distance(ObjectManager.Player.ServerPosition.To2D()) < 500f && (TargetSelector.SelectedTarget == null || TargetSelector.SelectedTarget.NetworkId == p.NetworkId) && !hitbox1.IsOutside(SCommon.Prediction.Prediction.GetFastUnitPosition(p, 0.3f))))
                {
                    Spells[Q].CastOnUnit(minion);
                    return;
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Spells[E].IsReady() && gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < Spells[E].Range && AntiGapCloserE)
                Spells[E].Cast(gapcloser.End);
        }

        protected override void OrbwalkingEvents_BeforeAttack(SCommon.Orbwalking.BeforeAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && Spells[W].IsReady() && ComboUseW)
            {
                Spells[W].Cast();
                args.Process = false;
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(SCommon.Orbwalking.AfterAttackArgs args)
        {
            if (!args.Target.IsDead && args.Target.Type == GameObjectType.AIHeroClient && args.Target.IsValidTarget(Spells[Q].Range) && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && ComboUseQ)
                Spells[Q].CastOnUnit(args.Target as AIHeroClient);
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack())
                m_lastTarget = args.Target;
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Combo.UseE").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool AntiGapCloserE
        {
            get { return ConfigMenu.Item("SAutoCarry.MissFortune.Misc.AntiGapCloserE").GetValue<bool>(); }
        }
    }
}
