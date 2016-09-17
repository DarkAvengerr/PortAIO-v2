using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Core
{
    class Helper
    {
        public static GameObject LuxE;
        public static bool Enabled(string menuName)
        {
            return LuxMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int Slider(string menuName)
        {
            return LuxMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[LuxMenu.Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        public static bool IsEnemyImmobile(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void OnCreate(GameObject objects, EventArgs args)
        {
            if (objects.Name == "Lux_Base_E_mis.troy")
            {
                LuxE = objects;
            }
        }

        public static void OnDelete(GameObject objects , EventArgs args)
        {
            if (objects.Name == "Lux_Base_E_tar_nova.troy")
            {
                LuxE = null;
            }
        }
        public static int EInsCheck()
        {
            if (Spells.E.Instance.Name == "LuxLightStrikeKugel") // Normal
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

    }
}
