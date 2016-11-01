using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    #region

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core;
    using Menus;

    using Orbwalking = Orbwalking;

    #endregion

    internal class Animation : Core
    {
        #region Public Methods and Operators

        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange + 50, TargetSelector.DamageType.Physical);

            var mob = MinionManager.GetMinions(
              ObjectManager.Player.AttackRange + 50,
              MinionTypes.All,
              MinionTeam.Neutral).FirstOrDefault();

            var isMoving = (target != null && target.IsMoving) || (mob != null && mob.IsMoving);

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 2;
                    if (SafeReset())
                    {
                        if (isMoving)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add((int)((MenuConfig.Qd + Ping)* 1.133), Reset);
                            Console.WriteLine("Q1 Slow Delay: " + (MenuConfig.Qd + Ping) * 1.133);
                        }
                        else
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Qd + Ping, Reset);
                            Console.WriteLine("Q1 Fast Delay: " + (MenuConfig.Qd + Ping));
                        }
                    }

                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 3;
                    if (SafeReset())
                    {
                        if (isMoving)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add((int)((MenuConfig.Q2D + Ping)* 1.133), Reset);
                            Console.WriteLine("Q2 Slow Delay: " + (MenuConfig.Q2D + Ping)* 1.133);
                        }
                        else
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Q2D + Ping, Reset);
                            Console.WriteLine("Q2 Fast Delay: " + (Ping + MenuConfig.Q2D));
                        }
                    }

                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 1;
                    if (SafeReset())
                    {
                        if (isMoving)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add((int)((MenuConfig.Qld + Ping)* 1.133), Reset);
                            Console.WriteLine("Q3 Slow Delay: " + (MenuConfig.Qld + Ping)* 1.133);
                            Console.WriteLine(">----END----<");

                        }
                        else
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Qld + Ping, Reset);
                            Console.WriteLine("Q3 Fast Delay: " + (MenuConfig.Qld + Ping));
                            Console.WriteLine(">----END----<");
                        }
                    }

                    break;
            }
        }

        #endregion

        #region Methods

        private static void Emotes()
        {
            if (ObjectManager.Player.HasBuffOfType(BuffType.Stun) 
                || ObjectManager.Player.HasBuffOfType(BuffType.Snare)
                || ObjectManager.Player.HasBuffOfType(BuffType.Knockback))
            {
                return;
            }

            switch (MenuConfig.EmoteList.SelectedIndex)
            {
                case 0:
                    EloBuddy.Player.DoEmote(Emote.Laugh);
                    break;
                case 1:
                    EloBuddy.Player.DoEmote(Emote.Taunt);
                    break;
                case 2:
                    EloBuddy.Player.DoEmote(Emote.Joke);
                    break;
                case 3:
                    EloBuddy.Player.DoEmote(Emote.Dance);
                    break;
                case 4:
                    break;
            }
        }

       // private static int AtkSpeed => (int)(1400 / Player.AttackSpeedMod * 3.75);

        private static int Ping => MenuConfig.CancelPing 
                                   ? Game.Ping / 2
                                   : 0;
        
        private static void Reset()
        {
            Emotes();
            Orbwalking.ResetAutoAttackTimer();
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, ObjectManager.Player.Position.Extend(ObjectManager.Player.Direction, 400));
        }

        private static bool SafeReset()
        {
            return Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None || MenuConfig.AnimSemi;
        }

        #endregion
    }
}