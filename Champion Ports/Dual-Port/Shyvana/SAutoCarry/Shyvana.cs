using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
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
    public class Shyvana : SCommon.PluginBase.Champion
    {
        public Shyvana()
            : base("Shyvana", "SAutoCarry - Shyvana")
        {
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
            OnLastHit += LastHit;
            OnUpdate += BeforeOrbwalk;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Shyvana.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseRWhenRange", "Use R When Enemy Out Of X Range").SetValue(new Slider(0, 0, 1000)));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseROnlyToWall", "Use R Only To Wall").SetValue(false));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseRHitCount", "Use R Whenever Can Hit X Enemies").SetValue(new Slider(3, 1, 5)));
            combo.AddItem(new MenuItem("SAutoCarry.Shyvana.Combo.UseTiamat", "Use Tiamat/Hydra").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.Shyvana.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Shyvana.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Shyvana.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Shyvana.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Shyvana.Harass.UseTiamat", "Use Tiamat/Hydra").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Shyvana.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));

            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Shyvana.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Shyvana.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Shyvana.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Shyvana.LaneClear.UseE", "Use E").SetValue(true));

            Menu misc = new Menu("Misc", "SAutoCarry.Shyvana.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Shyvana.Misc.LastHitE", "Last Hit E").SetValue(true));
            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
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
            Spells[Q] = new Spell(SpellSlot.Q);

            Spells[W] = new Spell(SpellSlot.W);

            Spells[E] = new Spell(SpellSlot.E, 925f);
            Spells[E].SetSkillshot(0.25f, 60, 1700f, false, SkillshotType.SkillshotLine);

            Spells[R] = new Spell(SpellSlot.R, 1000f);
            Spells[R].SetSkillshot(0.25f, 150f, 1500f, false, SkillshotType.SkillshotLine);

        }

        public void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if(t != null)
            {
                if (Spells[E].IsReady() && ComboUseE)
                    Spells[E].SPredictionCast(t, HitChance.Low);

                if(Spells[R].IsReady() && ComboUseR)
                {
                    var aoePred = Spells[R].GetAoeSPrediction();
                    if(aoePred.HitCount >= ComboUseRHitCount)
                    {
                        Spells[R].Cast(aoePred.CastPosition);
                        return;
                    }

                    if(t.Distance(ObjectManager.Player.ServerPosition) > ComboUseRWhenRange)
                    {
                        if (ComboUseROnlyToWall)
                        {
                            var pred = Spells[R].GetSPrediction(t);
                            if (pred.HitChance >= HitChance.Medium)
                            {
                                var endPosition = ObjectManager.Player.ServerPosition.To2D() + (pred.CastPosition - ObjectManager.Player.ServerPosition.To2D()).Normalized() * 1000f;
                                if (IsWallStunable(pred.UnitPosition, endPosition))
                                    Spells[R].Cast(pred.CastPosition);
                            }
                        }
                        else
                            Spells[R].SPredictionCast(t, HitChance.High);
                    }
                }
            }
        }

        public void Harass()
        {
            var t = TargetSelector.GetTarget(Spells[E].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if (t != null)
            {
                if (Spells[E].IsReady() && HarassUseE)
                    Spells[E].Cast(Spells[E].GetSPrediction(t).CastPosition);
            }
        }

        public void LaneClear()
        {
            if (Spells[E].IsReady() && LaneClearUseE)
            {
                var farm = Spells[E].GetLineFarmLocation(MinionManager.GetMinions(Spells[E].Range));
                if (farm.MinionsHit > 2)
                {
                    Spells[E].Cast(farm.Position);
                    return;
                }
                var jungle = MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (jungle != null)
                    Spells[E].Cast(jungle.ServerPosition);
            }
        }

        public void LastHit()
        {
            if(Spells[E].IsReady() && LastHitE)
            {
                var minion = MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).Where(p => p.Distance(ObjectManager.Player.ServerPosition) > ObjectManager.Player.AttackRange && p.Health < Spells[E].GetDamage(p)).FirstOrDefault();
                if (minion != null)
                    Spells[E].Cast(minion.ServerPosition);
            }
        }

        private bool IsWallStunable(Vector2 from, Vector2 to)
        {
            float count = from.Distance(to);
            for (uint i = 0; i <= count; i += 25)
            {
                Vector2 pos = from.Extend(ObjectManager.Player.ServerPosition.To2D(), -i);
                var colFlags = NavMesh.GetCollisionFlags(pos.X, pos.Y);
                if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                    return true;
            }
            return false;
        }

        protected override void OrbwalkingEvents_BeforeAttack(SCommon.Orbwalking.BeforeAttackArgs args)
        {
            if (args.Target is AIHeroClient)
            {
                if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
                {
                    if (Spells[W].IsReady() && ComboUseW)
                        Spells[W].Cast();
                }
                else if(Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed || HarassToggle)
                {
                    if (Spells[W].IsReady() && HarassUseW)
                        Spells[W].Cast();
                }
            }
            else if(args.Target is Obj_AI_Minion)
            {
                if(Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
                {
                    if (Spells[W].IsReady() && LaneClearUseW)
                        Spells[W].Cast();
                }
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

                if (Spells[Q].IsReady() && ComboUseQ)
                    Spells[Q].Cast();
            }
            else if(Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed || HarassToggle)
            {
                if (args.Target is AIHeroClient)
                {
                    if (HarassUseTiamat)
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        else if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);
                        else if (Items.HasItem(3748) && Items.CanUseItem(3748)) //titanic
                            Items.UseItem(3748);
                    }

                    if (Spells[Q].IsReady() && HarassUseQ)
                        Spells[Q].Cast();
                }
            }
            else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
            {
                if (!args.Target.IsDead && LaneClearUseQ && Spells[Q].IsReady())
                    Spells[Q].Cast();
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseR").GetValue<bool>(); }
        }

        public int ComboUseRWhenRange
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseRWhenRange").GetValue<Slider>().Value; }
        }

        public bool ComboUseROnlyToWall
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseROnlyToWall").GetValue<bool>(); }
        }

        public int ComboUseRHitCount
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseRHitCount").GetValue<Slider>().Value; }
        }

        public bool ComboUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Combo.UseTiamat").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Harass.UseW").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Harass.UseE").GetValue<bool>(); }
        }

        public bool HarassUseTiamat
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Harass.UseTiamat").GetValue<bool>(); }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public bool LaneClearUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.LaneClear.UseW").GetValue<bool>(); }
        }

        public bool LaneClearUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.LaneClear.UseE").GetValue<bool>(); }
        }

        public bool LastHitE
        {
            get { return ConfigMenu.Item("SAutoCarry.Shyvana.Misc.LastHitE").GetValue<bool>(); }
        }
    }
}