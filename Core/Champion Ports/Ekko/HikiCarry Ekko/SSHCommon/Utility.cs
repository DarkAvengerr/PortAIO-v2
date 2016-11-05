/*
 Copyright 2015 - 2015 ShineCommon
 Utility.cs is part of ShineCommon
 
 ShineCommon is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 ShineCommon is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with ShineCommon. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SSHCommon;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SSHCommon
{
    public static class Utility
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
    }
}
