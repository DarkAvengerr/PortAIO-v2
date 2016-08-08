using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class LowLifePeel : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.lowlifepeel") 
                && Variables.spells[SpellSlot.E].LSIsReady()
                && !Variables.spells[SpellSlot.Q].LSIsReady()
                && (ObjectManager.Player.HealthPercent <= 25);
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var meleeEnemies = ObjectManager.Player.LSGetEnemiesInRange(400f).FindAll(m => m.IsMelee);

            if (meleeEnemies.Any())
            {
                var mostDangerous = meleeEnemies.OrderByDescending(m => m.LSGetAutoAttackDamage(ObjectManager.Player)).First();
                //Variables.spells[SpellSlot.E].CastOnUnit(mostDangerous);
            }
        }
    }
}
