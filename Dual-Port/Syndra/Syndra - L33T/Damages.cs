using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public static class Damages
    {
        public static double GetDarkSphereDamage(this Obj_AI_Base target)
        {
            if (Mechanics.Spells[SpellSlot.Q].Level < 5)
            {
                return EntryPoint.Player.CalcDamage(
                    target, Damage.DamageType.Magical,
                    new[] { 50, 95, 140, 185, 230 }[Mechanics.Spells[SpellSlot.Q].Level - 1] +
                    EntryPoint.Player.FlatMagicDamageMod * .6f);
            }
            return EntryPoint.Player.CalcDamage(
                target, Damage.DamageType.Magical, 264.5 + EntryPoint.Player.FlatMagicDamageMod * .69f);
        }

        public static double GetForceOfWillDamage(this Obj_AI_Base target)
        {
            return EntryPoint.Player.CalcDamage(
                target, Damage.DamageType.Magical,
                new[] { 80, 120, 160, 200, 240 }[Mechanics.Spells[SpellSlot.W].Level - 1] +
                EntryPoint.Player.FlatMagicDamageMod * .7f);
        }

        public static double GetScatterTheWeakDamage(this Obj_AI_Base target)
        {
            return EntryPoint.Player.CalcDamage(
                target, Damage.DamageType.Magical,
                new[] { 70, 115, 160, 205, 250 }[Mechanics.Spells[SpellSlot.E].Level - 1] +
                EntryPoint.Player.FlatMagicDamageMod * .4f);
        }

        public static double GetUnleashedPowerDamage(this Obj_AI_Base target)
        {
            return Mechanics.Spells[SpellSlot.R].IsReady()
                ? EntryPoint.Player.CalcDamage(
                    target, Damage.DamageType.Magical,
                    45 + Mechanics.Spells[SpellSlot.R].Level * 45 + EntryPoint.Player.FlatMagicDamageMod * .2f) *
                  Mechanics.Spells[SpellSlot.R].Instance.Instance.Ammo
                : 0d;
        }

        public static double GetIgniteDamage(this AIHeroClient target)
        {
            return Mechanics.IgniteSpell.Slot != EloBuddy.SpellSlot.Unknown && Mechanics.IgniteSpell.IsReady()
                ? EntryPoint.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0d;
        }
    }
}