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
    class Play
    {
        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Animation)
                {
                    case "Spell1a":
                        EloBuddy.Player.DoEmote(Emote.Dance);
                        break;
                }
            }
        }
    }
}
