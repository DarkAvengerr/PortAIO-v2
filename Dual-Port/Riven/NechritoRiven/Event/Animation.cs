using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    #region

    using System;

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

            Console.WriteLine((Ping() + MenuConfig.Qd - AtkSpeed()).ToString());

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 2;
                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Ping() + MenuConfig.Qd - AtkSpeed(), Reset);
                    }

                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 3;
                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Ping() + MenuConfig.Q2D - AtkSpeed(), Reset);
                    }

                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 1;
                    if (SafeReset())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Ping() + MenuConfig.Qld - AtkSpeed(), Reset);
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
            }
        }

        private static int AtkSpeed()
        {
            return (int)((Player.Level + Player.AttackSpeedMod) * 0.625);
        }

        private static int Ping()
        {
            int ping;

            if (!MenuConfig.CancelPing)
            {
                ping = 5;
            }
            else
            {
                ping = Game.Ping / 2;
            }

            return ping;
        }

        private static void Reset()
        {
            Emotes();
            Orbwalking.LastAaTick = 0;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        private static bool SafeReset()
        {
            return Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None
                || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Flee
                || MenuConfig.AnimSemi;
        }

        #endregion
    }
}