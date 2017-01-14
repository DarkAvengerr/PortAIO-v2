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
        private static AIHeroClient enemy;

        // AIHeroClient enemy;

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
                
            }

            //QLogic
            if ((Program.qmenu.Item("UseQC").GetValue<bool>() && Program.orbwalker.ActiveMode.ToString() == "Combo") || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.qmenu.Item("hq").GetValue<bool>()))
            {
                var value = Program.qmenu.Item("QMode", true).GetValue<StringList>().SelectedIndex;
                var FastQ = Program.qmenu.Item("FastQ").GetValue<bool>();
                var htarget = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);

                if (value == 0)
                {
                    QLogic.Gosu.Run();

                }

                if (value == 1)
                {
                    if (!FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.SideQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.SideQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }
                }

                if (value == 2)
                {
                    if (FastQ)
                    {
                        Program.Q.Cast(Game.CursorPos);
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }

                    if (!FastQ)
                    {
                        Program.Q.Cast(Game.CursorPos);
                    }
                }

                if (value == 3)
                {
                    if (FastQ)
                    {
                        if (ObjectManager.Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                        {
                            Program.Q.Cast(Game.CursorPos);
                            EloBuddy.Player.DoEmote(Emote.Dance);
                        }
                    }

                    if (!FastQ)
                    {
                        if (ObjectManager.Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                        {
                            Program.Q.Cast(Game.CursorPos);
                        }
                    }
                }

                if (value == 4)
                {
                    if (!FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.SafeQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.SafeQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }
                }

                if (value == 5)
                {
                    if (!FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.AggresiveQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (FastQ)
                    {
                        Program.Q.Cast(QLogic.QLogic.AggresiveQ(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }
                }

                if (value == 6)
                {
                    if (!FastQ)
                    {
                        QLogic.Bursts.Burst();
                    }
                    if (FastQ)
                    {
                        QLogic.Bursts.Burst();
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }
                }
                if (value == 7)
                {
                   
                    if (!FastQ)
                    {
                        QLogic.Hiki.SafePositionQ(htarget);
                    }
                    if (FastQ)
                    {
                        QLogic.Hiki.SafePositionQ(htarget);
                        EloBuddy.Player.DoEmote(Emote.Dance);
                    }
                    if (value == 8)
                    {
                      
                    }
                }
             }
          }
      }
  }


       
 
