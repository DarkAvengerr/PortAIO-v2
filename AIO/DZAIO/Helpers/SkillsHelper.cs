using System;
using DZAIO_Reborn.Core;
using DZLib.Logging;
using DZLib.Menu;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers
{
    static class SkillsHelper
    {
        public static bool IsEnabledAndReady(this Spell spell, ModesMenuExtensions.Mode mode)
        {
            if (ObjectManager.Player.IsDead)
                return false;
            try
            {
                var enabledCondition = Variables.AssemblyMenu.Item("dzaio.champion." + ObjectManager.Player.ChampionName.ToLowerInvariant() + ".use" + DZLib.Menu.ModesMenuExtensions.GetStringFromSpellSlot(spell.Slot).ToLowerInvariant() + GetStringFromMode(mode)).GetValue<bool>();
                return (spell.LSIsReady() && enabledCondition);
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("IsEnabledAndReady", e.ToString(), LogSeverity.Severe));
            }
            return false;
        }

        static string GetStringFromMode(ModesMenuExtensions.Mode mode)
        {
            switch (mode)
            {
                case ModesMenuExtensions.Mode.Combo:
                    return "C";
                case ModesMenuExtensions.Mode.Harrass:
                    return "H";
                case ModesMenuExtensions.Mode.Lasthit:
                    return "LH";
                case ModesMenuExtensions.Mode.Laneclear:
                    return "LC";
                case ModesMenuExtensions.Mode.Farm:
                    return "F";
                default:
                    return "unk";
            }
        }

        /// <summary>
        /// Gets the baron reduction.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static float GetBaronReduction(this Spell spell, Obj_AI_Base target)
        {
            return ObjectManager.Player.LSHasBuff("barontarget")
                       ? spell.GetDamage(target) * 0.5f
                       : spell.GetDamage(target);
        }


        /// <summary>
        /// Gets the dragon reduction.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static float GetDragonReduction(this Spell spell, Obj_AI_Base target)
        {
            return ObjectManager.Player.LSHasBuff("s5test_dragonslayerbuff")
                       ? spell.GetDamage(target)
                         * (1 - (.07f * ObjectManager.Player.GetBuffCount("s5test_dragonslayerbuff")))
                       : spell.GetDamage(target);
        }
    }
}
