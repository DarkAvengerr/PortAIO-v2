using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Extensions
{
    internal static class DamageCalculator
    {
        private static readonly double[] RQ = { 100, 200, 300 };
        private static readonly double[] RW = { 150, 300, 450 };
        private static readonly double[] RE = { 100, 200, 300 };

        private static float UltimateDamage(AIHeroClient enemy, SpellSlot slot)
        {
            const float damage = 0f;

            if (!slot.LSIsReady() || ObjectManager.Player.Spellbook.GetSpell(slot).State != SpellState.NotLearned)
            {
                return 0f;
            }

            if (Utilities.UltimateKey() == "Q")
            {
                return
                    (float)
                        ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, RQ[Spells.R.Level - 1] + .60*
                                                                                          ObjectManager.Player
                                                                                              .FlatMagicDamageMod);
            }
            if (Utilities.UltimateKey() == "W")
            {
                return
                    (float)
                        ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                            RW[Spells.R.Level - 1] + .90*ObjectManager.Player.FlatMagicDamageMod);
            }
            if (Utilities.UltimateKey() == "E")
            {
                return
                    (float)
                        ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, RQ[Spells.R.Level - 1] + .60 *
                                                                                          ObjectManager.Player
                                                                                              .FlatMagicDamageMod);
            }

            return damage;
        }

        public static float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Spells.Q.LSIsReady())
            {
                damage += Spells.Q.GetDamage(hero);
                if (hero.HasMaliceBuff() || hero.HasSecondMaliceBuff())
                {
                    damage += Spells.Q.GetDamage(hero);
                }
            }
            if (Spells.W.LSIsReady())
            {
                damage += Spells.W.GetDamage(hero);
            }
            if (Spells.E.LSIsReady())
            {
                damage += Spells.W.GetDamage(hero);
            }
            if (Spells.R.LSIsReady())
            {
                damage += UltimateDamage(hero,SpellSlot.R);
            }
            return (float)damage;
        }

        
    }
}
