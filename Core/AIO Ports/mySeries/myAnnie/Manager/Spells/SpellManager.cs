using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 550f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.3f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");
        }

        internal static bool HaveBear => Me.HasBuff("InfernalGuardianTimer");

        internal static bool HaveStun => Me.HasBuff("Energized");

        internal static int BuffCounts
        {
            get
            {
                var count = 0;
                if (Me.HasBuff("Pyromania"))
                {
                    count = Me.GetBuffCount("Pyromania");
                }
                else if (!Me.HasBuff("Pyromania") || HaveStun)
                {
                    count = 0;
                }
                return count;
            }
        }
    }
}
