using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    public static class Combo
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        public static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsMe)
                return;
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            var mode = Variables.ComboMode.GetValue<StringList>().SelectedValue;
            if (Helper.HasItem())
            {
                if (args.Duration - 100 - Game.Ping / 2 > 0)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((args.EndTick - Environment.TickCount - Game.Ping - 150),() => Helper.CastItem());
                }
                else
                {
                    Helper.CastItem();
                }
            }

            if (Player.Mana < 5)
            {
                var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                {
                    Helper.CastE(targetE);
                }
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                {
                    if (Variables.E.IsReady())
                        Helper.CastE(target);
                }
            }
            if (mode == "Auto" || mode == "Snare")
            {
                if (Player.Mana == 5)
                {
                    var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                    if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                    {
                        Helper.CastE(targetE);
                    }
                    foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                    {
                        if (Variables.E.IsReady())
                            Helper.CastE(target);
                    }
                }
            }
            if (mode == "Always Q" || mode == "Burst")
            {
                if (Player.Mana == 5)
                {
                    Variables.Q.Cast();
                }
            }
            
            //Chat.Say("dash");
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 0 && Player.Mana == 5)
            {
                if (Helper.HasItem())
                    Helper.CastItem();
            }
            else if (Variables.Q.IsReady())
            {
                Variables.Q.Cast();
            }
            else if (Helper.HasItem())
            {
                Helper.CastItem();
            }
            else if (Variables.E.IsReady())
            {
                var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                {
                    Helper.CastE(targetE);
                }
                foreach (var tar in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                {
                    if (Variables.E.IsReady())
                        Helper.CastE(tar);
                }
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            if (args.Unit.IsMe && Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }

            if (!Variables.ComboResetAA.GetValue<bool>())
                return;

            Helper.QbeforeAttack(args.Target);

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            if (Helper.HasSmite && Variables.ComboSmite.GetValue<bool>())
            {
                if (Helper.hasSmiteRed || Helper.hasSmiteBlue)
                {
                    var target = TargetSelector.GetTarget(650, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && !target.IsZombie && Player.Distance(target.Position) <= Player.BoundingRadius + 500 + target.BoundingRadius)
                    {
                        Player.Spellbook.CastSpell(Variables.Smite, target);
                    }
                }
            }

            if (ItemData.Youmuus_Ghostblade.GetItem().IsReady() && Variables.ComboYoumuu.GetValue<bool>() 
                && Player.HasBuff("RengarR"))
            {
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
            if (!Player.HasBuff("RengarR"))
            {
                if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 0) // snare
                {
                    if (Player.Mana < 5)
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }

                    }
                    else
                    {
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 1) // burst
                {
                    if (Player.Mana < 5)
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }

                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0
                           && !Player.HasBuff("rengarpassivebuff"))
                        {
                            if (Orbwalking.CanMove(90) && !Orbwalking.CanAttack())
                            {
                                Variables.Q.Cast();
                            }
                        }
                        if (Variables.Q.IsReady() && Player.IsDashing())
                        {
                            Variables.Q.Cast();
                        }

                        if (!Player.HasBuff("rengarpassivebuff") && !Player.IsDashing()
                            && Orbwalking.CanMove(90)
                            && !HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x)))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 2) // auto
                {
                    if (Player.Mana < 5)
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }

                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0
                            && !Player.HasBuff("rengarpassivebuff"))
                        {
                            if (Orbwalking.CanMove(90) && !Orbwalking.CanAttack())
                            {
                                Variables.Q.Cast();
                            }

                        }
                        if (!Player.HasBuff("rengarpassivebuff") && !Player.IsDashing()
                            && Orbwalking.CanMove(90)
                            && !HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x)))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 3) // always Q
                {
                    if (Player.Mana < 5)
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }

                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0
                            && !Player.HasBuff("rengarpassivebuff"))
                        {
                            if (Orbwalking.CanMove(90) && !Orbwalking.CanAttack())
                            {
                                Variables.Q.Cast();
                            }
                        }
                        if (Variables.Q.IsReady() && Player.IsDashing())
                        {
                            Variables.Q.Cast();
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 4) // ap mode
                {
                    if (Player.Mana < 5)
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && HeroManager.Enemies.Any(x => x.IsValidTarget() && Orbwalking.InAutoAttackRange(x))))
                        {
                            var targetE = TargetSelector.GetTarget(Variables.E.Range, TargetSelector.DamageType.Physical);
                            if (Variables.E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie)
                            {
                                Helper.CastE(targetE);
                            }
                            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Variables.E.Range) && !x.IsZombie))
                            {
                                if (Variables.E.IsReady())
                                    Helper.CastE(target);
                            }
                        }

                    }
                    else
                    {
                        var targetW = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                        if (Variables.W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
                        {
                            Variables.W.Cast(targetW);
                        }
                    }
                }
                else Chat.Say("stupid");
            }
        
    }
    }
}
