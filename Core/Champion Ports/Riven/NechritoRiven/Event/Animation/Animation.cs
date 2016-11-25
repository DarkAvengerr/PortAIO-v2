using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.Animation
{
    #region

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using NechritoRiven.Core;

    using Orbwalking = Orbwalking;

    #endregion

    internal class Animation : Core
    {
        #region Public Methods and Operators

        private static AIHeroClient Target => TargetSelector.GetTarget(ObjectManager.Player.AttackRange + 50, TargetSelector.DamageType.Physical);

        private static Obj_AI_Minion Mob => (Obj_AI_Minion)MinionManager.GetMinions(ObjectManager.Player.AttackRange + 50, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();

        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 2;

                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(ResetDelay(MenuConfig.Qd), Reset);

                        Console.WriteLine("Q1 Delay: " + ResetDelay(MenuConfig.Qd));
                    }
                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 3;

                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(ResetDelay(MenuConfig.Q2D), Reset);

                        Console.WriteLine("Q2 Delay: " + ResetDelay(MenuConfig.Q2D));
                    }
                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 1;

                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(ResetDelay(MenuConfig.Qld), Reset);

                        Console.WriteLine("Q3 Delay: " 
                         + ResetDelay( MenuConfig.Qld)
                         + Environment.NewLine + ">----END----<");
                    }
                    break;
            }
        }

        #endregion

        #region Methods

        private static void Emotes()
        {
            switch (MenuConfig.EmoteList.SelectedIndex)
            {
                case 0:
                    Chat.Say("/l");
                    break;
                case 1:
                    Chat.Say("/t");
                    break;
                case 2:
                    Chat.Say("/j");
                    break;
                case 3:
                    Chat.Say("/d");
                    break;
            }
        }

        private static int ResetDelay(int qDelay)
        {
            if (MenuConfig.CancelPing)
            {
                return qDelay + Game.Ping / 2;
            }

            if ((Target != null && Target.IsMoving) || (Mob != null && Mob.IsMoving) || IsGameObject)
            {
                return (int)(qDelay * 1.15);
            }

            return qDelay;
        }
        
        private static void Reset()
        {
            Emotes();
            Orbwalking.ResetAutoAttackTimer();
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        private static bool SafeReset()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                return false;
            }

            return !ObjectManager.Player.HasBuffOfType(BuffType.Stun)
                && !ObjectManager.Player.HasBuffOfType(BuffType.Snare) 
                && !ObjectManager.Player.HasBuffOfType(BuffType.Knockback)
                && !ObjectManager.Player.HasBuffOfType(BuffType.Knockup);
        }

        #endregion
    }
}