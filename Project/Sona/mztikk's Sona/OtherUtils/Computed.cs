using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona
{
    using LeagueSharp;

    internal static class Computed
    {
        internal static readonly Random RDelay = new Random();

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            Spells.LastSpellSlot = args.Slot;
        }
    }
}
