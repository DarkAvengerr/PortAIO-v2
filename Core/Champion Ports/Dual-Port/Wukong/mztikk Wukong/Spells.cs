using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong
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

        internal static double GetQDamage(Obj_AI_Base target)
        {
            var basedmg = new[] { 0, 30, 60, 90, 120, 150 }[Q.Level];
            var bonusdmg = ObjectManager.Player.TotalAttackDamage * 0.10;
            return ObjectManager.Player.CalcDamage(
                target, 
                Damage.DamageType.Physical, 
                ObjectManager.Player.TotalAttackDamage + basedmg + bonusdmg);
        }

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 375);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 640);
            R = new Spell(SpellSlot.R, 320);

            E.SetTargetted(0.5f, 2000f);
        }

        #endregion
    }
}