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
    class OnSpellProcess
    {
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if ((Program.qmenu.Item("FastQs").GetValue<bool>() && Program.qmenu.Item("FastQ").GetValue<bool>()) && args.SData.Name == "VayneTumble")
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
        }
    }
}
