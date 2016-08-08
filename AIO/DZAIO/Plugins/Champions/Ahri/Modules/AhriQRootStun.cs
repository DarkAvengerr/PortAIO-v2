using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Ahri.Modules
{
    class AhriQRootStun : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.extra.autoQ") &&
                   Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(m => m.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range) && m.IsHeavilyImpaired());

            if (target != null)
            {
                Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
            }
        }
    }
}
