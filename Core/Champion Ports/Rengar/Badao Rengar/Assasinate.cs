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
    public static class Assasinate
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static AIHeroClient AssassinateTarget
        {
            get
            {
                return HeroManager.Enemies
                .FirstOrDefault(x => Variables.AssasinateTarget.GetValue<StringList>().SelectedValue == x.ChampionName + "(" + x.Name + ")");
            }
        }
        public static void BadaoActivate()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
                return;
            if (!sender.IsMe)
                return;

            var mode = Variables.ComboMode.GetValue<StringList>().SelectedValue;
            if (Helper.HasItem())
            {
                if (args.Duration - 100 - Game.Ping / 2 > 0)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((args.EndTick - Environment.TickCount - Game.Ping - 150), () => Helper.CastItem());
                }
                else
                {
                    Helper.CastItem();
                }
            }

            if (Player.Mana < 5)
            {
                if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                {
                    Helper.CastE(AssassinateTarget);
                }
            }
            if (mode == "Auto" || mode == "Snare")
            {
                if (Player.Mana == 5)
                {
                    if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                    {
                        Helper.CastE(AssassinateTarget);
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

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
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

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
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
                if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                {
                    Helper.CastE(AssassinateTarget);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
                return;
            
            Orbwalking.Orbwalk((AssassinateTarget != null && Orbwalking.InAutoAttackRange(AssassinateTarget))? AssassinateTarget : null,
                Variables.Orbwalker._orbwalkingPoint.To2D().IsValid() ?
                Variables.Orbwalker._orbwalkingPoint : Game.CursorPos, 90, 50, false, false);

            if (!AssassinateTarget.IsValidTarget())
                return;

            if (Helper.HasSmite && Variables.ComboSmite.GetValue<bool>())
            {
                if (Helper.hasSmiteRed || Helper.hasSmiteBlue)
                {
                    if (!AssassinateTarget.IsZombie && Player.Distance(AssassinateTarget.Position) <= Player.BoundingRadius + 500 + AssassinateTarget.BoundingRadius)
                    {
                        Player.Spellbook.CastSpell(Variables.Smite, AssassinateTarget);
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
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                    else
                    {
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 1) // burst
                {
                    if (Player.Mana < 5)
                    {
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Orbwalking.InAutoAttackRange(AssassinateTarget) && !Player.HasBuff("rengarpassivebuff"))
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
                            && !Orbwalking.InAutoAttackRange(AssassinateTarget))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 2) // auto
                {
                    if (Player.Mana < 5)
                    {
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Orbwalking.InAutoAttackRange(AssassinateTarget) && !Player.HasBuff("rengarpassivebuff"))
                        {
                            if (Orbwalking.CanMove(90) && !Orbwalking.CanAttack())
                            {
                                Variables.Q.Cast();
                            }

                        }
                        if (!Player.HasBuff("rengarpassivebuff") && !Player.IsDashing()
                            && Orbwalking.CanMove(90)
                            && !Orbwalking.InAutoAttackRange(AssassinateTarget))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                }
                else if (Variables.ComboMode.GetValue<StringList>().SelectedIndex == 3) // always Q
                {
                    if (Player.Mana < 5)
                    {
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                    else
                    {
                        if (Variables.Q.IsReady() && Orbwalking.InAutoAttackRange(AssassinateTarget) && !Player.HasBuff("rengarpassivebuff"))
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
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                        if (Player.IsDashing() || Orbwalking.CanMove(90)
                            && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                            {
                                Helper.CastE(AssassinateTarget);
                            }
                        }
                    }
                    else
                    {
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(Variables.W.Range) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                    }
                }
                else Chat.Say("stupid");
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Variables.DrawAssasinate.GetValue<bool>())
            {
                var x = Drawing.WorldToScreen(new Vector3(Player.Position.X,Player.Position.Y - 50, Player.Position.Z));
                Drawing.DrawText(x[0], x[1], Color.White, Variables.AssasinateTarget.GetValue<StringList>().SelectedValue);
            }

            if (Variables.AssassinateKey.GetValue<KeyBind>().Active && AssassinateTarget != null)
            {
                var x = Drawing.WorldToScreen(Player.Position);
                var y = Drawing.WorldToScreen(AssassinateTarget.Position);
                Drawing.DrawLine(x, y, 1, Color.Red);
            }
        }
    }
}
