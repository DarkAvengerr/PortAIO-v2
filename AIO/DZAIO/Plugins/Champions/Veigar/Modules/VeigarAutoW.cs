using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Veigar.Modules
{
    class VeigarAutoW : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.extra.autoW") 
                && Variables.Spells[SpellSlot.W].IsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = EntityHelper.GetStunnedTarget(Variables.Spells[SpellSlot.W].Range);
            if (target.IsValidTarget())
            {
                    var stunBuff = target.Buffs.Where(b => b.Type == BuffType.Stun)
                        .OrderByDescending(m => m.EndTime - Game.Time)
                        .FirstOrDefault();
                    if (stunBuff != null)
                    {
                        var actualStunDuration = stunBuff.EndTime - Game.Time;
                        if (actualStunDuration > 1.300f)
                        {
                            Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                        }
                    }
            }
        }
    }
}
