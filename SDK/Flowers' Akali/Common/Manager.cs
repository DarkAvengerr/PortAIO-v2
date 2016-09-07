using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Common
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.Enumerations;
    using SharpDX;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    internal static class Manager
    {
        /// <summary>
        /// Default Enable List
        /// </summary>
        public static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        /// <summary>
        ///  Get Enemy Minions
        /// </summary>
        /// <param name="From">Search Origin Position</param>
        /// <param name="Range">Search Minions Range</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetMinions(Vector3 From, float Range)
        {
            return GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Range, false, @From)).ToList();
        }

        /// <summary>
        /// Get Mobs
        /// </summary>
        /// <param name="From">Search Origin Position</param>
        /// <param name="Range">Search Minions Range</param>
        /// <param name="OnlyBig">Only Search Big Mob</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetMobs(Vector3 From, float Range, bool OnlyBig = false)
        {
            if (OnlyBig)
            {
                return GameObjects.Jungle.Where(x => x.IsValidTarget(Range, false, @From) && !GameObjects.JungleSmall.Contains(x)).ToList();
            }
            else
                return GameObjects.Jungle.Where(x => x.IsValidTarget(Range, false, @From)).ToList();
        }

        /// <summary>
        /// Search Enemies List
        /// </summary>
        /// <param name="Range">Search Enemies Range</param>
        /// <returns></returns>
        public static List<AIHeroClient> GetEnemies(float Range)
        {
            return GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Range) && !x.IsZombie && !x.IsDead).ToList();
        }

        /// <summary>
        /// Search Target
        /// </summary>
        /// <param name="Range">Search Target Range</param>
        /// <param name="DamageType">Spell Damage Type</param>
        /// <returns></returns>
        public static AIHeroClient GetTarget(float Range, DamageType DamageType = DamageType.Physical)
        {
            return Variables.TargetSelector.GetTarget(Range, DamageType);
        }

        /// <summary>
        /// Search Target
        /// </summary>
        /// <param name="Spell">Spell Target</param>
        /// <param name="Ignote">Ignote Shield</param>
        /// <returns></returns>
        public static AIHeroClient GetTarget(Spell Spell, bool Ignote = true)
        {
            return Variables.TargetSelector.GetTarget(Spell, Ignote);
        }

        /// <summary>
        /// Judge Target Is In Auto Attack Range
        /// </summary>
        /// <param name="Target">Target</param>
        /// <returns></returns>
        public static bool InAutoAttackRange(AttackableUnit target)
        {
            var baseTarget = (Obj_AI_Base)target;
            var myRange = GetAttackRange(GameObjects.Player);

            if (baseTarget != null)
            {
                return baseTarget.IsHPBarRendered && 
                    Vector2.DistanceSquared(baseTarget.ServerPosition.ToVector2(),
                    ObjectManager.Player.ServerPosition.ToVector2()) <= myRange * myRange;
            }

            return target.IsValidTarget() && 
                Vector2.DistanceSquared(target.Position.ToVector2(),
                ObjectManager.Player.ServerPosition.ToVector2()) 
                <= myRange * myRange;
        }

        /// <summary>
        /// Get Target Attack Range
        /// </summary>
        /// <param name="Target">Target</param>
        /// <returns></returns>
        public static float GetAttackRange(Obj_AI_Base Target)
        {
            if (Target != null)
            {
                return Target.GetRealAutoAttackRange();
            }
            else
                return 0f;
        }

        /// <summary>
        /// Get Damage
        /// </summary>
        /// <param name="Target">Target</param>
        /// <param name="CalCulateAttackDamage">CalCulate Attack Damage</param>
        /// <param name="CalCulateQDamage">CalCulate Q Damage</param>
        /// <param name="CalCulateWDamage">CalCulate W Damage</param>
        /// <param name="CalCulateEDamage">CalCulate E Damage</param>
        /// <param name="CalCulateRDamage">CalCulate R Damage</param>
        /// <returns></returns>
        public static double GetDamage(AIHeroClient Target, bool CalCulateAttackDamage = true,
            bool CalCulateQDamage = true, bool CalCulateWDamage = true,
            bool CalCulateEDamage = true, bool CalCulateRDamage = true)
        {
            if (CheckTarget(Target))
            {
                double Damage = 0d;

                if (CalCulateAttackDamage)
                {
                    Damage += GameObjects.Player.GetAutoAttackDamage(Target);
                }

                if (CalCulateQDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? GameObjects.Player.GetSpellDamage(Target, SpellSlot.Q) : 0d;
                }

                if (CalCulateWDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.W).IsReady() ? GameObjects.Player.GetSpellDamage(Target, SpellSlot.W) : 0d;
                }

                if (CalCulateEDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).IsReady() ? GameObjects.Player.GetSpellDamage(Target, SpellSlot.E) : 0d;
                }

                if (CalCulateRDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).IsReady() ? GameObjects.Player.GetSpellDamage(Target, SpellSlot.R) : 0d;
                }

                return Damage;
            }
            else
            {
                return 0d;
            }
        }

        /// <summary>
        /// Judge Target MoveMent Status (This Part From SebbyLib)
        /// </summary>
        /// <param name="Target">Target</param>
        /// <returns></returns>
        public static bool CanMove(AIHeroClient Target)
        {
            if (Target.MoveSpeed < 50 || Target.IsStunned || Target.HasBuffOfType(BuffType.Stun) ||
                Target.HasBuffOfType(BuffType.Fear) || Target.HasBuffOfType(BuffType.Snare) ||
                Target.HasBuffOfType(BuffType.Knockup) || Target.HasBuff("Recall") ||
                Target.HasBuffOfType(BuffType.Knockback) || Target.HasBuffOfType(BuffType.Charm) ||
                Target.HasBuffOfType(BuffType.Taunt) || Target.HasBuffOfType(BuffType.Suppression)
                || (Target.IsCastingInterruptableSpell() && !Target.IsMoving))
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Check Target
        /// </summary>
        /// <param name="Target">Target</param>
        /// <returns></returns>
        public static bool CheckTarget(AIHeroClient Target)
        {
            if (Target != null && !Target.IsDead && !Target.IsZombie && Target.IsHPBarRendered)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Combo Key Active
        /// </summary>
        public static bool InCombo
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo;
            }
        }

        /// <summary>
        /// Harass Key Active
        /// </summary>
        public static bool InHarass
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid;
            }
        }

        /// <summary>
        /// LaneClear Key Active
        /// </summary>
        public static bool InClear
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear;
            }
        }

        /// <summary>
        /// LastHit Key Active
        /// </summary>
        public static bool InLastHit
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == OrbwalkingMode.LastHit;
            }
        }

        /// <summary>
        /// None Key Active
        /// </summary>
        public static bool InNone
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == OrbwalkingMode.None;
            }
        }

        /// <summary>
        /// Send Message To Console
        /// </summary>
        /// <param name="Message"></param>
        public static void WriteConsole(string Message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Flowers'Akali : " + Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
