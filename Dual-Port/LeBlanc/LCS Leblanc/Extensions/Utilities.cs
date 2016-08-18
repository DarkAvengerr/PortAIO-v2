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
    internal static class Utilities
    {
        public static string UltimateKey()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
            {
                return "Q";
            }
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
            {
                return "W";
            }
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
            {
                return "E";
            }
            return "none";
        }

        public static void UpdateUltimateVariable()
        {
            if (UltimateKey() == "Q")
            {
                Spells.R = new Spell(SpellSlot.R, Spells.Q.Range);
            }
            else if (UltimateKey() == "W")
            {
                Spells.R = new Spell(SpellSlot.R, Spells.W.Range);
                Spells.R.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);
            }
            else if (UltimateKey() == "E")
            {
                Spells.R = new Spell(SpellSlot.R, Spells.E.Range);
                Spells.R.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            }
            else if (UltimateKey() == "none")
            {
                Spells.R = new Spell(SpellSlot.R);
            }
        }

        public static bool HasMaliceBuff(this AIHeroClient enemy) // Q DEBUFF
        {
            return enemy.HasBuff("leblancchaosorb");
        }

        public static bool HasChainBuff(this AIHeroClient enemy) // E DEBUFF
        {
            return enemy.HasBuff("leblancsoulshackle");
        }

        public static bool HasSecondMaliceBuff(this AIHeroClient enemy)
        {
            return enemy.HasBuff("leblancchaosorbm");
        }

        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred","Jhin"
            };

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

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

    }
}
