using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Spells
    {
        #region Properties

        internal static Spell E { get; private set; }

        internal static Spell Q { get; private set; }

        internal static Spell R { get; private set; }

        internal static Spell W { get; private set; }

        #endregion

        #region Methods

        internal static HitChance GetHitChance(string hitchancestr)
        {
            switch (Config.GetStringListValue(hitchancestr))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.VeryHigh;
            }
        }

        internal static double GetUltHeal(AIHeroClient target)
        {
            var baseHeal = new[] { 0, 150, 250, 350 }[R.Level] + (ObjectManager.Player.TotalMagicalDamage * 0.55);
            if (target.HealthPercent < 40)
            {
                return baseHeal * 1.5;
            }

            return baseHeal;
        }

        internal static double GetWHeal()
        {
            var amount = new[] { 0, 110, 140, 170, 200 }[W.Level] + (ObjectManager.Player.TotalMagicalDamage * 0.6);
            return amount;
        }

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.325f, 235f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 235f, 1750f, false, SkillshotType.SkillshotCircle);
        }

        #endregion
    }
}