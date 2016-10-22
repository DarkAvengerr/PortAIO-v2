using System;
using LeagueSharp;
using Reforged_Riven.Extras;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Update
{
    internal class Animation : Logic
    {
        public static float LastQ;

        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Environment.TickCount;
                    Qstack = 2;
                    break;
                case "Spell1b":
                    LastQ = Environment.TickCount;
                    Qstack = 3;
                    break;
                case "Spell1c":
                    LastQ = Environment.TickCount;
                    Qstack = 1;
                    break;
            }
        }
    }
}
