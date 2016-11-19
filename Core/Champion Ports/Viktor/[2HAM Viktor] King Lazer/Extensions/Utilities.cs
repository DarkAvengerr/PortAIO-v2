using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Viktor.Extensions
{
   internal static class Utilities
    {
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitChanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static HitChance TheHitChance(string var_Menu)
        {
            return HitChanceArray[Menus.menuCfg.Item(var_Menu).GetValue<StringList>().SelectedIndex];
        }

        public static bool IsEnabled(string var_Menu)
        {
            return Menus.menuCfg.Item(var_Menu).GetValue<bool>();
        }

        public static int SliderValue(string var_Menu)
        {
            return Menus.menuCfg.Item(var_Menu).GetValue<Slider>().Value;
        }

        public static bool IsEnemyImmobile(this AIHeroClient unit)
        {
            return unit.HasBuffOfType(BuffType.Stun)        || unit.HasBuffOfType(BuffType.Snare)           ||
                   unit.HasBuffOfType(BuffType.Knockup)     || unit.HasBuffOfType(BuffType.Polymorph)       ||
                   unit.HasBuffOfType(BuffType.Charm)       || unit.HasBuffOfType(BuffType.Fear)            ||
                   unit.HasBuffOfType(BuffType.Knockback)   || unit.HasBuffOfType(BuffType.Silence)         ||
                   unit.HasBuffOfType(BuffType.Taunt)       || unit.HasBuffOfType(BuffType.Suppression)     ||
                   unit.HasBuffOfType(BuffType.Slow)        || unit.IsStunned                               ||
                   unit.IsChannelingImportantSpell()        || !unit.CanMove                                || 
                   unit.IsRecalling();
        }
    }
}
