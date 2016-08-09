using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Veigar.Modules
{
    class VeigarAutoQ : IModule
    {
        public void OnLoad()
        {
            
        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.extra.farmQ") &&
                   ObjectManager.Player.ManaPercent > 35 && Variables.Spells[SpellSlot.Q].IsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var positions = MinionManager.GetMinions(Variables.Spells[SpellSlot.Q].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Where(m => Variables.Spells[SpellSlot.Q].GetDamage(m) >= m.Health + 5)
                    .Select(m => m.ServerPosition.To2D()).ToList();

            var lineFarmLocation = Variables.Spells[SpellSlot.Q].GetLineFarmLocation(positions);

            if (lineFarmLocation.MinionsHit > 0)
            {
                Variables.Spells[SpellSlot.Q].Cast(lineFarmLocation.Position);
            }
        }
    }
}
