using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GodModeOn_Vayne.Combo
{
    class Harrash
    {
        public static void Do()
        {
            var Qharrash = Program.menu.Item("QH").GetValue<bool>();
                    var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
                    if(Qharrash)
                if (target != null)
                {
                    Program.Q.Cast(Game.CursorPos, false);
                }
            }
        }
    }

