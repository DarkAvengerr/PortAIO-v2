using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class CastSpellManager : Logic
    {
        internal static void Init(Spellbook spell, SpellbookCastSpellEventArgs Args)
        {
            if (spell.Owner.IsMe)
            {
                if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
                {
                    LastCastTime = Utils.TickCount;
                }
            }
        }
    }
}