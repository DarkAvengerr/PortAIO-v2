using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Menus;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    using Core;

    internal class Anim : Core
    {
        private static int Ping()
        {
            int ping;

            if (!MenuConfig.CancelPing)
            {
                ping = 0;
            }
            else
            {
                ping = Game.Ping/2;
            }

            return ping;
        }

        private static void Emotes()
        {
            if (!MenuConfig.EmoteEnable)
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
            }
        }

        private static bool SafeReset =>
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Flee &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None;

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
                //    Emotes();
                    if (SafeReset)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Qd + Ping(), Reset);
                    }
                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 3;
                  //  Emotes();
                    if (SafeReset)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Q2d + Ping(), Reset);
                    }
                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    Qstack = 1;
               //     Emotes();
                    if (SafeReset)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(MenuConfig.Qld + Ping(), Reset);
                    }
                    break;
            }
        }
        private static void Reset()
        {
            Emotes();
            //  EloBuddy.Player.DoEmote(Emote.Dance);
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
            Orbwalking.LastAaTick = 0;
        }
    }
}
