using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Modules
{
    /// <summary>
    /// The Twitch Mob Stealer Module
    /// </summary>
    class TwitchESteal : IModule
    {
        private float LastCastTick;

        public string GetName()
        {
            return "TwitchESteal";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].IsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.twitch.misc.steale");
        }

        public void Run()
        {
            var baron =
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition,
                        Variables.spells[SpellSlot.E].Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth)
                        .FirstOrDefault(
                            x => x.IsValidTarget() && HealthPrediction.GetHealthPrediction(x, 250) + 5 < this.GetBaronReduction(x) && x.Name.Contains("Baron"));

            var dragon =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    Variables.spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x => x.IsValidTarget() && HealthPrediction.GetHealthPrediction(x, 250) + 5 < this.GetDragonReduction(x) && x.Name.Contains("Dragon"));

            if (((dragon != null && Variables.spells[SpellSlot.E].CanCast(dragon))
                || (baron != null && Variables.spells[SpellSlot.E].CanCast(baron)))
                && (Environment.TickCount - LastCastTick >= 500))
            {
                Variables.spells[SpellSlot.E].Cast();
                LastCastTick = Environment.TickCount;
            }
        }


        /// <summary>
        /// Gets the baron reduction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private float GetBaronReduction(Obj_AI_Base target)
        {
            return ObjectManager.Player.HasBuff("barontarget")
                       ? Variables.spells[SpellSlot.E].GetDamage(target) * 0.5f
                       : Variables.spells[SpellSlot.E].GetDamage(target);
        }


        /// <summary>
        /// Gets the dragon reduction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private float GetDragonReduction(Obj_AI_Base target)
        {
            return ObjectManager.Player.HasBuff("s5test_dragonslayerbuff")
                       ? Variables.spells[SpellSlot.E].GetDamage(target)
                         * (1 - (.07f * ObjectManager.Player.GetBuffCount("s5test_dragonslayerbuff")))
                       : Variables.spells[SpellSlot.E].GetDamage(target);
        }
    }
}
