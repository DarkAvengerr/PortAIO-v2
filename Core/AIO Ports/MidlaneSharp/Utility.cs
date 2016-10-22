/*
 Copyright 2015 - 2015 SPrediction
 Utility.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MidlaneSharp
{
    public static class UtilityCore
    {
        #region Champion Priority Arrays
        public static string[] lowPriority =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Tahm", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

        public static string[] mediumPriority =
            {
                "Aatrox", "Akali", "Darius", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fiora", "Fizz",
                "Galio", "Gangplank", "Gragas", "Heimerdinger", "Irelia", "Jax", "Jayce", "Kassadin", "Kayle", "Kha'Zix",
                "Lee Sin", "Lissandra", "Maokai", "Mordekaiser", "Morgana", "Nocturne", "Nidalee", "Pantheon", "Poppy",
                "RekSai", "Rengar", "Riven", "Rumble", "Ryze", "Shaco", "Swain", "Trundle", "Tryndamere", "Udyr",
                "Urgot", "Vladimir", "Vi", "XinZhao", "Yasuo", "Zilean"
            };

        public static string[] highPriority =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs"
            };
        #endregion

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        private static string[] JungleMinions;

        static UtilityCore()
        {
            if (LeagueSharp.Common.Utility.Map.GetMap().Type.Equals(LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline))
            {
                JungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                JungleMinions = new string[]
                    {
                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon",
                        "SRU_Baron", "Sru_Crab"
                    };
            }
        }

        public static int GetPriority(string championName)
        {
            if (lowPriority.Contains(championName))
                return 1;

            if (mediumPriority.Contains(championName))
                return 2;

            if (highPriority.Contains(championName))
                return 3;

            return 2;
        }

        public static bool IsJungleMinion(this Obj_AI_Base unit)
        {
            return JungleMinions.Contains(unit.Name);
        }

        public static bool IsImmobilizeBuff(BuffType type)
        {
            return type == BuffType.Snare || type == BuffType.Stun || type == BuffType.Charm || type == BuffType.Knockup || type == BuffType.Suppression;
        }

        public static bool IsImmobileTarget(AIHeroClient target)
        {
            return target.Buffs.Count(p => IsImmobilizeBuff(p.Type)) > 0 || target.IsChannelingImportantSpell();
        }

        public static bool IsActive(this Spell s)
        {
            return ObjectManager.Player.Spellbook.GetSpell(s.Slot).ToggleState == 2;
        }

        public static async Task CastWithDelay(this Spell s, int delay)
        {
            System.Threading.Thread.Sleep(delay);
            s.Cast();
        }

        public static async Task DelayAction(Action act, int delay = 1)
        {
            System.Threading.Thread.Sleep(delay);
            act();
        }
    }

    /// <summary>
    /// Necessary utilities for SPrediction
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Checks if the given bufftype is immobilizer 
        /// </summary>
        /// <param name="type">Buff type</param>
        /// <returns></returns>
        internal static bool IsImmobilizeBuff(BuffType type)
        {
            return type == BuffType.Snare || type == BuffType.Stun || type == BuffType.Charm || type == BuffType.Knockup || type == BuffType.Suppression;
        }

        /// <summary>
        /// Checks if the given target is immobile
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static bool IsImmobileTarget(Obj_AI_Base target)
        {
            return target.Buffs.Count(p => IsImmobilizeBuff(p.Type)) > 0;
        }

        /// <summary>
        /// Gets left immobile time of given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static float LeftImmobileTime(Obj_AI_Base target)
        {
            return target.Buffs.Where(p => p.IsActive && IsImmobilizeBuff(p.Type) && p.EndTime >= Game.Time).Max(q => q.EndTime - Game.Time);
        }

        /// <summary>
        /// Converts <see cref="Prediction.Result"/> to <see cref="Prediction.Vector.Result"/> with given from position
        /// </summary>
        /// <param name="pResult">Prediction result as <see cref="Prediction.Result"/></param>
        /// <param name="from">Vector source position</param>
        /// <returns>Converted prediction result as <see cref="Prediction.Vector.Result"/></returns>
        internal static VectorPrediction.Result AsVectorResult(this Prediction.Result pResult, Vector2 from)
        {
            return new VectorPrediction.Result
            {
                CastSourcePosition = from,
                CastTargetPosition = pResult.CastPosition,
                UnitPosition = pResult.UnitPosition,
                HitChance = pResult.HitChance,
                CollisionResult = pResult.CollisionResult,
            };
        }

        /// <summary>
        /// Converts <see cref="Prediction.Result"/> to <see cref="Prediction.AoeResult"/> with given hit count and collision result
        /// </summary>
        /// <param name="pResult">Prediction result as <see cref="Prediction.Result"/></param>
        /// <param name="hitCount">Aoe hits</param>
        /// <param name="colResult">Aoe collision result</param>
        /// <returns>Converted prediction result as <see cref="Prediction.AoeResult"/></returns>
        internal static Prediction.AoeResult ToAoeResult(this Prediction.Result pResult, int hitCount, Collision.Result colResult)
        {
            return new Prediction.AoeResult
            {
                CastPosition = pResult.CastPosition,
                HitCount = hitCount,
                CollisionResult = colResult
            };
        }
    }
}
