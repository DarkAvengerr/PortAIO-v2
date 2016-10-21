using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    internal class GravesSpell : Graves
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 40f, 3000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            SearchERange = E.Range + Me.GetRealAutoAttackRange() - 100;
        }
    }
}