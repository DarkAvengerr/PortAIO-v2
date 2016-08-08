using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Trundle.Modules
{
    class TrundleQKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.trundle.extra.qks") &&
                   Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var ksAbleEnemy =
                HeroManager.Enemies.FirstOrDefault(m => m.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range) &&
                        Variables.Spells[SpellSlot.Q].IsKillable(m));

            if (ksAbleEnemy != null)
            {
                Variables.Spells[SpellSlot.Q].Cast(ksAbleEnemy);
            }
        }
    }
}
