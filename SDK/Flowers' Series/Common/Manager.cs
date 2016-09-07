using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Common
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Manager
    {
        /// <summary>
        /// Auto Enable List
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
                return GameObjects.Jungle.Where(x => x.IsValidTarget(Range, false, @From) && (x.Name.Contains("Crab") || !GameObjects.JungleSmall.Contains(x))).ToList();
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
                    GameObjects.Player.ServerPosition.ToVector2()) <= myRange * myRange;
            }

            return target.IsValidTarget() &&
                Vector2.DistanceSquared(target.Position.ToVector2(),
                GameObjects.Player.ServerPosition.ToVector2())
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
        /// <param name="target">Target</param>
        /// <param name="CalCulateAttackDamage">CalCulate Attack Damage</param>
        /// <param name="CalCulateQDamage">CalCulate Q Damage</param>
        /// <param name="CalCulateWDamage">CalCulate W Damage</param>
        /// <param name="CalCulateEDamage">CalCulate E Damage</param>
        /// <param name="CalCulateRDamage">CalCulate R Damage</param>
        /// <returns></returns>
        public static double GetDamage(AIHeroClient target, bool CalCulateAttackDamage = true,
            bool CalCulateQDamage = true, bool CalCulateWDamage = true,
            bool CalCulateEDamage = true, bool CalCulateRDamage = true)
        {
            if (CheckTarget(target))
            {
                double Damage = 0d;

                if (CalCulateAttackDamage)
                {
                    Damage += GameObjects.Player.GetAutoAttackDamage(target);
                }

                if (CalCulateQDamage)
                {
                    if (GameObjects.Player.ChampionName == "Ahri")
                    {
                        Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) * 2 : 0d;
                    }
                    else if (GameObjects.Player.ChampionName == "Viktor")
                    {
                        Damage += GameObjects.Player.HasBuff("ViktorPowerTransferReturn") ? GetQAttackDamage(target) : 0d;
                    }
                    else if (GameObjects.Player.ChampionName == "Vladimir")
                    {
                        Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? GameObjects.Player.HasBuff("vladimirqfrenzy") ? GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) * 2 : GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) : 0d;
                    }
                    else
                    {
                        Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) : 0d;
                    }
                }

                if (CalCulateWDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.W).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.W) : 0d;
                }

                if (CalCulateEDamage)
                {
                    Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.E) : 0d;
                }

                if (CalCulateRDamage)
                {
                    if (GameObjects.Player.ChampionName == "Ahri")
                    {
                        Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.R) * 3 : 0d;
                    }
                    else
                    {
                        Damage += GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).IsReady() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.R) : 0d;
                    }
                }

                if (GameObjects.Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown && GameObjects.Player.GetSpellSlot("SummonerDot").IsReady())
                {
                    Damage += 50 + 20 * GameObjects.Player.Level - (target.HPRegenRate / 5 * 3);
                }

                if (target.ChampionName == "Moredkaiser")
                    Damage -= target.Mana;

                // exhaust
                if (GameObjects.Player.HasBuff("SummonerExhaust"))
                    Damage = Damage * 0.6f;

                // blitzcrank passive
                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                    Damage -= target.Mana / 2f;

                // kindred r
                if (target.HasBuff("KindredRNoDeathBuff"))
                    Damage = 0;

                // tryndamere r
                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // kayle r
                if (target.HasBuff("JudicatorIntervention"))
                    Damage = 0;

                // zilean r
                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // fiora w
                if (target.HasBuff("FioraW"))
                    Damage = 0;

                return Damage;
            }
            else
            {
                return 0d;
            }
        }

        /// <summary>
        /// Viktor QA Damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static double GetQAttackDamage(AIHeroClient target)
        {
            double[] AttackDamage = new double[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };

            return AttackDamage[GameObjects.Player.Level - 1] + GameObjects.Player.TotalMagicalDamage * 0.5 + GameObjects.Player.TotalAttackDamage;
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
        /// Draw MiniMap Circle, This Part From LeagueSharp.Common
        /// </summary>
        /// <param name="range">Spell Range</param>
        public static void DrawEndScene(float range)
        {
            var pointList = new List<Vector3>();
            for (var i = 0; i < 30; i++)
            {
                var angle = i * Math.PI * 2 / 30;
                pointList.Add(
                    new Vector3(
                        GameObjects.Player.Position.X + range * (float)Math.Cos(angle), GameObjects.Player.Position.Y + range * (float)Math.Sin(angle),
                        GameObjects.Player.Position.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);
                var aon1Screen = Drawing.WorldToScreen(a);
                var bon1Screen = Drawing.WorldToScreen(b);

                Drawing.DrawLine(aon1Screen.X, aon1Screen.Y, bon1Screen.X, bon1Screen.Y, 1, System.Drawing.Color.White);
                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, 1, System.Drawing.Color.White);
            }
        }

        /// <summary>
        /// Combo Key Active
        /// </summary>
        public static bool InCombo
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == LeagueSharp.SDK.Enumerations.OrbwalkingMode.Combo;
            }
        }

        /// <summary>
        /// Harass Key Active
        /// </summary>
        public static bool InHarass
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == LeagueSharp.SDK.Enumerations.OrbwalkingMode.Hybrid;
            }
        }

        /// <summary>
        /// LaneClear Key Active
        /// </summary>
        public static bool InClear
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == LeagueSharp.SDK.Enumerations.OrbwalkingMode.LaneClear;
            }
        }

        /// <summary>
        /// LastHit Key Active
        /// </summary>
        public static bool InLastHit
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == LeagueSharp.SDK.Enumerations.OrbwalkingMode.LastHit;
            }
        }

        /// <summary>
        /// None Key Active
        /// </summary>
        public static bool InNone
        {
            get
            {
                return Variables.Orbwalker.ActiveMode == LeagueSharp.SDK.Enumerations.OrbwalkingMode.None;
            }
        }

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="Message">The Message</param>
        public static void WriteConsole(string Message, bool Chat = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Flowers' Series : " + Message);
            Console.ForegroundColor = ConsoleColor.White;

            if (Chat)
            {
                EloBuddy.Chat.Print("Flowers' Series : " + Message);
            }
        }
    }
}
