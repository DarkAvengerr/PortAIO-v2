using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne.MyUtils;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Program.E.IsReady() || !(sender is AIHeroClient))
            {
                return;
            }
            if ((args.Target != null && args.Target.IsMe) || ObjectManager.Player.Distance(args.End, true) < 350*350)
            {
                if (args.SData.Name == "RenektonDice")
                {
                    Program.E.Cast(sender);
                }
                if (sender.BaseSkinName == "Leona" && args.Slot == SpellSlot.E)
                {
                    Program.E.Cast(sender);
                }
                if (sender.BaseSkinName == "Alistar" && args.Slot == SpellSlot.W)
                {
                    Program.E.Cast(sender);
                }
                if (sender.BaseSkinName == "Diana" && args.Slot == SpellSlot.R)
                {
                    Program.E.Cast(sender);
                }
                if (sender.BaseSkinName == "Shyvana" && args.Slot == SpellSlot.R)
                {
                    Program.E.Cast(sender);
                }
                if (sender.BaseSkinName == "Akali" && args.Slot == SpellSlot.R && args.SData.CooldownTime > 2.5)
                {
                    Program.E.Cast(sender);
                }
                if (args.SData.Name.ToLower().Contains("flash") && sender.IsMelee)
                {
                    Program.E.Cast(sender);
                }
            }
        }
    }
}
