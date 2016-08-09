using DZLib.Logging;
using iSeriesReborn.Champions.Vayne.Skills;
using iSeriesReborn.Champions.Vayne.Utility;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Geometry;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Modules
{
    class VayneQKS : IModule
    {
        public string GetName()
        {
            return "Vayne_QKS";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.qinrange");
        }

        public void Run()
        {
            var currentTarget = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 240f, TargetSelector.DamageType.Physical);
            if (!currentTarget.IsValidTarget())
            {
                return;
            }

            if (currentTarget.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <=
                Orbwalking.GetRealAutoAttackRange(null))
            {
                return;
            }

            if (HealthPrediction.GetHealthPrediction(currentTarget, (int)(250 + Game.Ping / 2f)) <
                ObjectManager.Player.GetAutoAttackDamage(currentTarget) +
                Variables.spells[SpellSlot.Q].GetDamage(currentTarget)
                && HealthPrediction.GetHealthPrediction(currentTarget, (int)(250 + Game.Ping / 2f)) > 0)
            {
                var extendedPosition = ObjectManager.Player.ServerPosition.Extend(
                    currentTarget.ServerPosition, 300f);
                if (VayneQ.IsSafe(extendedPosition))
                {
                    Variables.spells[SpellSlot.Q].Cast(extendedPosition);
                    Orbwalking.ResetAutoAttackTimer();
                    TargetSelector.SetTarget(currentTarget);
                }
            }
        }
    }
}
