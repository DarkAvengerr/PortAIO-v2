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

            if (Player.Mana < 4)
            {
                if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                {
                    Helper.CastE(AssassinateTarget);
                }
            }
            if (mode == "E")
            {
                if (Player.Mana == 4)
                {
                    if (Variables.E.IsReady() && AssassinateTarget.IsValidTarget(Variables.E.Range) && !AssassinateTarget.IsZombie)
                    {
                        Helper.CastE(AssassinateTarget);
                    }
                }
            }
           
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
                return;

            if (args.Unit.IsMe)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Variables.AssassinateKey.GetValue<KeyBind>().Active)
                return;
            if (Variables.ComboMode.GetValue<StringList>().SelectedValue != "Q" && Player.Mana == 4)
            {
                if (Helper.HasItem())
                    Helper.CastItem();
            }
            else if (Variables.Q.IsReady())
            {
                Variables.Q.Cast(target as Obj_AI_Base);
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
                Variables.Orbwalker.GetOrbwalkingPoint().To2D().IsValid() ?
                Variables.Orbwalker.GetOrbwalkingPoint() : Game.CursorPos, 90, 50, false, false);

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
            if (!Player.HasBuff("RengarR"))
            {
                if (Player.Mana < 4)
                {
                    if (Variables.Q.IsReady() && AssassinateTarget.IsValidTarget(Variables.Q.Range) && !AssassinateTarget.IsZombie)
                    {
                        if (!Player.IsDashing() && Orbwalking.CanMove(90)
                           && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                        {
                            Variables.Q.Cast(AssassinateTarget);
                        }
                    }
                    if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(500) && !AssassinateTarget.IsZombie)
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
                if (Player.Mana == 4)
                {
                    if (Variables.ComboMode.GetValue<StringList>().SelectedValue == "E")
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
                    if (Variables.ComboMode.GetValue<StringList>().SelectedValue == "W")
                    {
                        if (Variables.W.IsReady() && AssassinateTarget.IsValidTarget(500) && !AssassinateTarget.IsZombie)
                        {
                            Variables.W.Cast(AssassinateTarget);
                        }
                    }
                    if (Variables.ComboMode.GetValue<StringList>().SelectedValue == "Q")
                    {
                        if (Variables.Q.IsReady() && AssassinateTarget.IsValidTarget(Variables.Q.Range) && !AssassinateTarget.IsZombie)
                        {
                            if (!Player.IsDashing() && Orbwalking.CanMove(90)
                               && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(AssassinateTarget)))
                            {
                                Variables.Q.Cast(AssassinateTarget);
                            }
                        }
                    }
                }
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
