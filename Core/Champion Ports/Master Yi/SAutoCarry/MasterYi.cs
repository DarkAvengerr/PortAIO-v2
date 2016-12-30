using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SPrediction;
using SCommon.Orbwalking;
using SCommon.Evade;
using SUtility.Drawings;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class MasterYi : SCommon.PluginBase.Champion
    {
        public TargetedSpellEvader m_targetedEvader;
        public MasterYi()
            : base ("MasterYi", "SAutoCarry - Master Yi")
        {
            OnCombo += Combo;
            OnLaneClear += LaneClear;
            Orbwalker.SetChannelingWait(false);
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.MasterYi.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.MasterYi.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.MasterYi.Combo.UseW", "Use W (For AA Reset)").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.MasterYi.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.MasterYi.Combo.Tiamat", "Use Tiamat/Hydra").SetValue(true));

            Menu jungleclear = new Menu("JungleClear", "SAutoCarry.MasterYi.JungleClear");
            jungleclear.AddItem(new MenuItem("SAutoCarry.MasterYi.JungleClear.UseQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("SAutoCarry.MasterYi.JungleClear.UseE", "Use E").SetValue(true));

            Menu misc = new Menu("Misc", "SAutoCarry.MasterYi.Misc");
            m_targetedEvader = new TargetedSpellEvader(TargetedSpell_Evade, misc);
            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t, 4), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(jungleclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 600f);

            Spells[W] = new Spell(SpellSlot.W);

            Spells[E] = new Spell(SpellSlot.E);

            Spells[R] = new Spell(SpellSlot.R);
        }

        public void Combo()
        {
            if(Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[Q].CastOnUnit(t);
            }
        }

        public void LaneClear()
        {
            var mob = MinionManager.GetMinions(500, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if(mob != null)
            {
                if (Spells[Q].IsReady() && JungleClearUseQ)
                    Spells[Q].CastOnUnit(mob);

                if (Spells[E].IsReady() && JungleClearUseE)
                    Spells[E].Cast();
            }
        }

        private void TargetedSpell_Evade(DetectedTargetedSpellArgs args)
        {
            if (Spells[Q].IsReady())
            {
                if (Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.Combo || !m_targetedEvader.DisableInComboMode)
                {
                    if (args.Caster.IsValidTarget(Spells[Q].Range))
                        Spells[Q].CastOnUnit(args.Caster);
                    else
                    {
                        var hero = HeroManager.Enemies.Where(p => p.IsValidTarget(Spells[Q].Range)).OrderBy(q => ObjectManager.Player.Distance(q.ServerPosition)).FirstOrDefault();
                        if (hero != null)
                            Spells[Q].CastOnUnit(hero);
                        else
                        {
                            var minion = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None).OrderBy(p => ObjectManager.Player.ServerPosition.Distance(p.ServerPosition)).FirstOrDefault();
                            if (minion != null)
                                Spells[Q].CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        public override double CalculateAADamage(AIHeroClient target, int aacount = 2)
        {
            return base.CalculateAADamage(target, aacount) + (Spells[E].IsReady() ? Spells[E].GetDamage(target) * aacount : 0);
        }

        protected override void OrbwalkingEvents_BeforeAttack(BeforeAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && args.Target.Type == GameObjectType.AIHeroClient)
            {
                if  (Spells[E].IsReady() && ComboUseE)
                    Spells[E].Cast();
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(AfterAttackArgs args)
        {
            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && args.Target.Type == GameObjectType.AIHeroClient)
            {
                if (ComboUseTiamat)
                {
                    if (ComboUseTiamat)
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                        {
                            Items.UseItem(3077);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                            return;
                        }
                        else if (Items.HasItem(3074) && Items.CanUseItem(3074))
                        {
                            Items.UseItem(3074);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                            return;
                        }
                        else if (Items.HasItem(3748) && Items.CanUseItem(3748)) //titanic
                        {
                            Items.UseItem(3748);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                            return;
                        }
                    }
                }

                if(ComboUseW && Spells[W].IsReady())
                {
                    Spells[W].Cast();
                    args.ResetAATimer = true;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                    return;
                }
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.Combo.Tiamat").GetValue<bool>(); }
        }

        public bool JungleClearUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.JungleClear.UseQ").GetValue<bool>(); }
        }

        public bool JungleClearUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.MasterYi.JungleClear.UseE").GetValue<bool>(); }
        }
    }
}
