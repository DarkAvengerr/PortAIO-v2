using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Extensions
{
    internal static class Utilities
    {
        /// <summary>
        /// High Priority Champions
        /// </summary>
        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred","Jhin"
            };
       
        /// <summary>
        /// Hitchance Name Array
        /// </summary>
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        
        /// <summary>
        /// Hitchance Array
        /// </summary>
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };
        
        /// <summary>
        /// Gives me true if unit has yasuo e buff
        /// </summary>
        /// <param name="unit">target</param>
        /// <returns></returns>
        public static bool HasYasuoEBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("YasuoDashWrapper");
        }

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Menus.Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        public static bool Enabled(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<bool>();
        }

        public static int Slider(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool Empowered(this Spell spell)
        {
            return spell.Instance.Name == "YasuoQ3W";
        }

        public static bool IsKnockedup(this AIHeroClient unit, bool onlyyasuoq)
        {
            return onlyyasuoq ? unit.HasBuff("yasuoq3mis") : unit.HasBuffOfType(BuffType.Knockup) 
                || unit.HasBuffOfType(BuffType.Knockback);
        }

    }
}
