using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora.Manager.Spells
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 400f + 350f);
            W = new Spell(SpellSlot.W, 750f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500f);

            Q.SetSkillshot(0.25f, 50f, 1200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 80f, 2000f, false, SkillshotType.SkillshotLine);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        public static double GetPassiveDamage(AIHeroClient target, int? passiveCount = null)
        {
            var passive = (0.03f +
                            Math.Min(
                                Math.Max(
                                    .028f,
                                    .027 +
                                    .001f * ObjectManager.Player.Level * ObjectManager.Player.FlatPhysicalDamageMod /
                                    100f), .45f)) * target.MaxHealth;

            return passiveCount * passive ?? Passive.PassiveManager.PassiveCount(target) * passive;
        }
    }
}