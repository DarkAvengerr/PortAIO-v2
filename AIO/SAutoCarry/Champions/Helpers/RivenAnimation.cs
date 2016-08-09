using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon.Database;
using SCommon.PluginBase;
using EloBuddy;

namespace SAutoCarry.Champions.Helpers
{
    public static class Animation
    {
        private static int s_LastAATick;
        private static bool s_CheckAA;
        private static bool s_DoAttack;

        public static int QStacks = 0;
        public static int LastQTick = 0;
        public static int LastETick = 0;
        public static bool CanCastAnimation = true;
        public static bool UltActive;
        public static long blResetQueued;

        public delegate void dOnAnimationCastable(string animname);
        public static event dOnAnimationCastable OnAnimationCastable;

        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                int t = 0;
                switch (args.Animation)
                {
                    case "Spell1a":
                        QStacks = 1;
                        CanCastAnimation = false;
                        LastQTick = Utils.TickCount;
                        t = 291;
                        break;
                    case "Spell1b":
                        QStacks = 2;
                        CanCastAnimation = false;
                        LastQTick = Utils.TickCount;
                        t = 291;
                        break;
                    case "Spell1c":
                        QStacks = 0;
                        SetAttack(false);
                        CanCastAnimation = false;
                        LastQTick = Utils.TickCount;
                        t = 393;
                        break;
                    case "Spell2":
                        CanCastAnimation = false;
                        t = 10;
                        break;
                    case "Spell3":
                        CanCastAnimation = true;
                        LastETick = Utils.TickCount;
                        break;
                    case "Spell4a":
                        t = 10;
                        CanCastAnimation = false;
                        UltActive = true;
                        break;
                    case "Spell4b":
                        t = 200;
                        CanCastAnimation = false;
                        UltActive = false;
                        break;
                    default:
                        t = -1;
                        break;
                }

                if (t > 0)
                {
                    if (Program.Champion.ConfigMenu.Item("MAUTOANIMCANCEL").GetValue<bool>() || Program.Champion.Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.None)
                        LeagueSharp.Common.Utility.DelayAction.Add(Math.Max(1, t - Game.Ping), () => CancelAnimation(args.Animation));
                }
                else if (t != -1)
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => OnAnimationCastable(args.Animation));
            }
        }

        public static void AfterAttack(SCommon.Orbwalking.AfterAttackArgs args)
        {
            if (s_CheckAA)
            {
                s_CheckAA = false;
                CanCastAnimation = true;

                if (args.Target.IsValidTarget() && args.Target.Type == GameObjectType.AIHeroClient && (Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || (Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo && Program.Champion.ConfigMenu.Item(String.Format("CMETHOD{0}", (args.Target as AIHeroClient).ChampionName)).GetValue<StringList>().SelectedIndex == 1)))
                {
                    if (ObjectManager.Player.HasBuff("RivenFengShuiEngine"))
                    {
                        {
                            if (Program.Champion.Spells[3].IsReady()) //r2
                            {
                                Program.Champion.Spells[3].Cast(args.Target.Position);
                                return;
                            }
                        }
                    }
                }
                 
                if(args.Target.IsValidTarget() && !Program.Champion.Spells[0].IsReady() && Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
                {
                    if (Program.Champion.Spells[2].IsReady() && !Program.Champion.Spells[0].IsReady(1000) && Program.Champion.ConfigMenu.Item("HEMODE").GetValue<StringList>().SelectedIndex == 2)
                    {
                        Program.Champion.Spells[2].Cast(ObjectManager.Player.ServerPosition + (args.Target.Position - ObjectManager.Player.ServerPosition).Normalized() * -Program.Champion.Spells[2].Range);
                        return;
                    }
                }

                var t = Target.Get(Program.Champion.Spells[0].Range + 50, true);
                if (Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || Program.Champion.ConfigMenu.Item("CFLASHKEY").GetValue<KeyBind>().Active)
                {
                    if (t != null && !(Program.Champion as Riven).IsDoingFastQ && Program.Champion.Spells[0].IsReady())
                    {
                        Program.Champion.Orbwalker.ForcedTarget = t;
                        Program.Champion.Spells[0].Cast(t.ServerPosition, true);
                        (Program.Champion as Riven).FastQCombo(true);
                        return;
                    }
                }
                if (s_DoAttack && Program.Champion.Spells[0].IsReady())
                {
                    if (t != null && (Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo || Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed || Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || Program.Champion.ConfigMenu.Item("CFLASHKEY").GetValue<KeyBind>().Active))
                    {
                        Program.Champion.Orbwalker.ForcedTarget = t;
                        //if (QStacks == 2)
                        //{
                        //    if (Program.Champion.Spells[1].IsReady() && Program.Champion.Spells[2].IsReady()) //e-q3-w
                        //    {
                        //        Program.Champion.Spells[2].Cast(t.ServerPosition);
                        //        Program.Champion.Spells[0].Cast(t.ServerPosition + (t.ServerPosition - ObjectManager.Player.ServerPosition).Normalized() * 40, true);
                        //        Program.Champion.Spells[1].Cast(true);
                        //    }
                        //}
                        //else
                            Program.Champion.Spells[0].Cast(t.ServerPosition, true);
                            return;
                        //Program.Champion.Orbwalker.ResetAATimer();

                    }
                    else if (Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
                    {
                        var minion = MinionManager.GetMinions(400, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).OrderBy(p => p.ServerPosition.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault();
                        if (minion != null)
                        {
                            if (minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion) * 2 && minion.IsJungleMinion())
                                SetAttack(false);
                            else
                            {
                                Program.Champion.Spells[0].Cast(minion.ServerPosition, true);
                                //Program.Champion.Orbwalker.ResetAATimer();
                                Program.Champion.Orbwalker.ForcedTarget = t;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if(!Program.Champion.Spells[0].IsReady() && !Program.Champion.Spells[1].IsReady() && !Program.Champion.Spells[3].IsReady() && (Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo || Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed || Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || Program.Champion.ConfigMenu.Item("CFLASHKEY").GetValue<KeyBind>().Active))
                    {
                        if((Program.Champion as Riven).IsCrestcentReady)
                            (Program.Champion as Riven).CastCrescent();
                    }
                    SetAttack(false);
                }
            }

            if(Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None && Program.Champion.Spells[0].IsReady())
            {
                if(Program.Champion.ConfigMenu.Item("LSEMIQJUNG").GetValue<bool>())
                {
                    if(args.Target is Obj_AI_Base)
                    {
                        var target = args.Target as Obj_AI_Base;
                        if(target != null && target.IsValidTarget() && target.IsJungleMinion())
                        {
                            Program.Champion.Spells[0].Cast(target.ServerPosition, true);
                        }
                    }
                }
            }

            if(Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo || Program.Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
            {
                if (!(Program.Champion as Riven).IsDoingFastQ && Program.Champion.Spells[1].IsReady() && args.Target.IsValidTarget(Program.Champion.Spells[1].Range))
                {
                    (Program.Champion as Riven).CastCrescent();
                    Program.Champion.Spells[1].Cast();
                }
            }
        }

        public static void CancelAnimation(string animname)
        {
            Program.Champion.Orbwalker.ResetAATimer();
            Chat.Say("/d");
            if(animname == "Spell2" || animname == "Spell1c")
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, ObjectManager.Player.Distance(Game.CursorPos) + 10));
            if (Program.Champion.Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.None || Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || Program.Champion.ConfigMenu.Item("CFLASHKEY").GetValue<KeyBind>().Active)
                OnAnimationCastable(animname);

            if (s_DoAttack)
                Program.Champion.Orbwalker.Orbwalk(Program.Champion.Orbwalker.GetTarget());

            CanCastAnimation = true;
        }

        public static void SetLastAATick(int tick)
        {
            s_LastAATick = Utils.TickCount;
            s_CheckAA = true;
        }

        public static void SetAttack(bool b)
        {
            s_DoAttack = b;
        }

        public static bool CanAttack()
        {
            return s_DoAttack;
        }
    }
}
