using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.General
{
    class ActivatorModule : ISOLOModule
    {
        private Items.Item BOTRK = new Items.Item((int) ItemId.Blade_of_the_Ruined_King, 450f);
        private Items.Item Youmuu = new Items.Item((int)ItemId.Youmuus_Ghostblade);
        private Items.Item Cutlass = new Items.Item((int)ItemId.Bilgewater_Cutlass, 450f);


        /// <summary>
        /// Called when the module is loaded
        /// </summary>
        public void OnLoad()
        {

        }

        /// <summary>
        /// Shoulds the module get executed.
        /// </summary>
        /// <returns></returns>
        public bool ShouldGetExecuted()
        {
            return ((BOTRK.IsOwned() && BOTRK.IsReady()) 
                || (Youmuu.IsOwned() && Youmuu.IsReady()) 
                || (Cutlass.IsOwned() && Cutlass.IsReady()))
                && (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || ObjectManager.Player.HealthPercent < 10);
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        /// <summary>
        /// Called when the module is executed.
        /// </summary>
        public void OnExecute()
        {
            var target = Variables.Orbwalker.GetTarget();

            if (target is AIHeroClient && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(target) + 125f))
            {
                if (target.IsValidTarget(450f))
                {
                    var targetHealth = target.HealthPercent;
                    var myHealth = ObjectManager.Player.HealthPercent;

                    if (myHealth < 50 && targetHealth > 20 && (BOTRK.IsOwned() && BOTRK.IsReady()))
                    {
                        BOTRK.Cast(target as AIHeroClient);
                    }

                    if (targetHealth < 65 && (Cutlass.IsOwned() && Cutlass.IsReady()))
                    {
                        Cutlass.Cast(target as AIHeroClient);
                    }
                }

                if (Youmuu.IsOwned() && Youmuu.IsReady())
                {
                    Youmuu.Cast();
                }
            }
        }
    }

    enum ItemType
    {
        OnAfterAA, OnUpdate
    }
}
