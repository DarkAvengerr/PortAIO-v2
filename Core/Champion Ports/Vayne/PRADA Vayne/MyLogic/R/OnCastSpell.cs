using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using PRADA_Vayne_Old.MyLogic.Q;
using PRADA_Vayne_Old.MyUtils;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PRADA_Vayne_Old.MyLogic.R
{
    public static partial class Events
    {
        public static void OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            /*if (spellbook.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.R && Program.ComboMenu.Item("QR").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1);
                    var tumblePos = target != null ? target.GetTumblePos() : Game.CursorPos;
                    Tumble.Cast(tumblePos);
                }
            }*/
        }
    }
}
