using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.QLogic
{
    class Bursts
    {
        public static void Burst()
        {
           
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
           else
                if (Program.Q.IsReady() && target.IsValidTarget(600) && !Program.qmenu.Item("FastQ").GetValue<bool>() &&
                    target.GetBuffCount("vaynesilvereddebuff") == 2)
                {
                    Program.Q.Cast(Game.CursorPos);

                }
                else if (Program.Q.IsReady() && target.IsValidTarget(600) && Program.qmenu.Item("FastQ").GetValue<bool>() &&
                    target.GetBuffCount("vaynesilvereddebuff") == 2 )
                {
                    Program.Q.Cast(Game.CursorPos);
                    EloBuddy.Player.DoEmote(Emote.Dance);
                }
            }
        
    }
}
