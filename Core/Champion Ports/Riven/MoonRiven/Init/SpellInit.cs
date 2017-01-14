using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellInit : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 260f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f);

            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);

            IgniteSlot = Me.GetSpellSlot("SummonerDot");
            FlashSlot = Me.GetSpellSlot("SummonerFlash");

            if (IgniteSlot != SpellSlot.Unknown)
            {
                Ignite = new Spell(IgniteSlot, 600f);
            }

            if (FlashSlot != SpellSlot.Unknown)
            {
                Flash = new Spell(FlashSlot, 425f);
            }
        }
    }
}