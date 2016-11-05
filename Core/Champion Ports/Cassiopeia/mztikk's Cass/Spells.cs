using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal static class Spells
    {
        #region Properties

        internal static float ComboDmgMod { get; } = 0.8f;

        internal static Spell E { get; private set; }

        internal static bool FlashR { get; set; }

        internal static SpellDataInst Flash { get; } =
            ObjectManager.Player.Spellbook.Spells.FirstOrDefault(a => a.SData.Name == "SummonerFlash");

        internal static Vector3 LastQPos { get; set; }

        internal static Vector3 LastWPos { get; set; }

        internal static Spell Q { get; private set; }

        internal static float QCasted { get; set; } = 0f;

        internal static Spell R { get; private set; }

        internal static Spell W { get; private set; }

        internal static float WCasted { get; set; } = 0f;

        internal static float WMaxRange { get; } = 900;

        internal static float WMinRange { get; } = 500;

        #endregion

        #region Methods

        internal static double GetEDamage(Obj_AI_Base target)
        {
            var basedmg = (48 + (4 * ObjectManager.Player.Level)) + (ObjectManager.Player.TotalMagicalDamage * 0.1f);
            if (!target.HasBuffOfType(BuffType.Poison))
            {
                return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, basedmg);
            }

            var bonusdmg = new[] { 0, 10, 40, 70, 100, 130 }[E.Level]
                           + (ObjectManager.Player.TotalMagicalDamage * 0.35f);
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, basedmg + bonusdmg);
        }

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, WMaxRange);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 800);

            Q.SetSkillshot(0.75f, 130, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(
                0.85f, 
                180, 
                1500, 
                false, 
                SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        #endregion
    }
}