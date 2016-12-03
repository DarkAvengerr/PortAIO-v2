using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1000f, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 600f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 600f, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 1500f, TargetSelector.DamageType.Magical);

            Q.SetSkillshot(0.25f, 50f, 1700, true, SkillshotType.SkillshotLine);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        internal static bool HaveShield => Me.HasBuff("ryzeqshield");

        internal static bool NoStack => Me.HasBuff("ryzeqiconnocharge");

        internal static bool HalfStack => Me.HasBuff("ryzeqiconhalfcharge");

        internal static bool FullStack => Me.HasBuff("ryzeqiconfullcharge");
    }
}
