using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel == Interrupter2.DangerLevel.High && Program.E.IsReady() && Program.E.IsInRange(sender) && (sender.BaseSkinName != "Shyvana") && sender.BaseSkinName != "Vayne")
            {
                Console.WriteLine("Interrupter");
                Program.E.Cast(sender);
            }
        }
    }
}
