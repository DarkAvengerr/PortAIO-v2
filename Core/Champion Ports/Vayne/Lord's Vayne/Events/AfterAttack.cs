using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class AfterAttack
    {

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe) return;

            if (Program.orbwalker.ActiveMode.ToString() == "LaneClear"
                && 100 * (Program.Player.Mana / Program.Player.MaxMana) > Program.qmenu.Item("Junglemana").GetValue<Slider>().Value)
            {
                var mob =
                    MinionManager.GetMinions(
                        Program.Player.ServerPosition,
                        Program.E.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();
                var Minions = MinionManager.GetMinions(
                    Program.Player.Position.Extend(Game.CursorPos, Program.Q.Range),
                    Program.Player.AttackRange,
                    MinionTypes.All);
                var useQ = Program.qmenu.Item("UseQJ").GetValue<bool>();
                int countMinions = 0;
                foreach (var minions in
                    Minions.Where(
                        minion =>
                        minion.Health < Program.Player.GetAutoAttackDamage(minion)
                        || minion.Health < Program.Q.GetDamage(minion) + Program.Player.GetAutoAttackDamage(minion) || minion.Health < Program.Q.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions >= 2 && useQ && Program.Q.IsReady() && Minions != null) Program.Q.Cast(Program.Player.Position.Extend(Game.CursorPos, Program.Q.Range / 2));
                if (useQ && Program.Q.IsReady() && Orbwalking.InAutoAttackRange(mob) && mob != null && Program.qmenu.Item("FastQ").GetValue<bool>())
                {
                    Program.Q.Cast(Game.CursorPos);
                    EloBuddy.Player.DoEmote(Emote.Dance);

                }
                else if (useQ && Program.Q.IsReady() && Orbwalking.InAutoAttackRange(mob) && mob != null && !Program.qmenu.Item("FastQ").GetValue<bool>())
                {
                    Program.Q.Cast(Game.CursorPos);

                }
            }


            if (!(target is AIHeroClient)) return;

            Program.tar = (AIHeroClient)target;

            if (Program.menu.Item("aaqaa").GetValue<KeyBind>().Active)
            {
                if (Program.Q.IsReady())
                {

                    Program.Q.Cast(Game.CursorPos);
                    EloBuddy.Player.DoEmote(Emote.Dance);

                }

                Orbwalking.Orbwalk(TargetSelector.GetTarget(625, TargetSelector.DamageType.Physical), Game.CursorPos);
            }

            // Condemn.FlashE();

            if (Program.menu.Item("zzrot").GetValue<KeyBind>().Active)
            {
                Misc.zzRotCondemn.RotE();
            }


            if (Program.emenu.Item("UseEaa").GetValue<KeyBind>().Active)
            {
                Program.E.Cast((Obj_AI_Base)target);
                Program.emenu.Item("UseEaa").SetValue<KeyBind>(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle));
            }
            if ((Program.orbwalker.ActiveMode.ToString() == "Combo") && Program.qmenu.Item("UseQC").GetValue<bool>() && !Program.qmenu.Item("FastQ").GetValue<bool>()
                    || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.qmenu.Item("hq").GetValue<bool>()))

            {
                if (Program.qmenu.Item("restrictq").GetValue<bool>())
                {
                    var after = ObjectManager.Player.Position
                                + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    //Chat.Print("After: {0}", after);
                    var disafter = Vector3.DistanceSquared(after, Program.tar.Position);
                    //Chat.Print("DisAfter: {0}", disafter);
                    //Chat.Print("first calc: {0}", (disafter) - (630*630));
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Program.Q.Cast(Game.CursorPos);


                    }

                    if (Vector3.DistanceSquared(Program.tar.Position, ObjectManager.Player.Position) > 630 * 630
                        && disafter < 630 * 630)
                    {
                        Program.Q.Cast(Game.CursorPos);


                    }
                }
                else
                {
                    Program.Q.Cast(Game.CursorPos);



                }
                //Q.Cast(Game.CursorPos);
            }

            if ((Program.orbwalker.ActiveMode.ToString() == "Combo") && Program.qmenu.Item("UseQC").GetValue<bool>() && Program.qmenu.Item("UseQC").GetValue<bool>() && Program.qmenu.Item("FastQ").GetValue<bool>()
                   || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.qmenu.Item("hq").GetValue<bool>()))
            {
                if (Program.qmenu.Item("restrictq").GetValue<bool>())
                {
                    var after = ObjectManager.Player.Position
                                + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    //Chat.Print("After: {0}", after);
                    var disafter = Vector3.DistanceSquared(after, Program.tar.Position);
                    //Chat.Print("DisAfter: {0}", disafter);
                    //Chat.Print("first calc: {0}", (disafter) - (630*630));
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Program.Q.Cast(Game.CursorPos);
                        EloBuddy.Player.DoEmote(Emote.Dance);

                    }

                    if (Vector3.DistanceSquared(Program.tar.Position, ObjectManager.Player.Position) > 630 * 630
                        && disafter < 630 * 630)
                    {
                        Program.Q.Cast(Game.CursorPos);
                        EloBuddy.Player.DoEmote(Emote.Dance);

                    }
                }
                else
                {
                    Program.Q.Cast(Game.CursorPos);
                    EloBuddy.Player.DoEmote(Emote.Dance);


                }
                //Q.Cast(Game.CursorPos);
            }
        }


        public static Vector3 Normalize(Vector3 A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector3(new Vector2((float)(A.X / distance)), (float)(A.Y / distance));

        }
    }
}

          /* if (Program.Q.IsReady())
                {
                switch (Program.qmenu.Item("QMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            QLogic.Gosu.Run();
                        }
                        break;
                    case 1:
                        {
                            QLogic.Cursor.Run();
                        }
                        break;
                }
            }
            */
 
