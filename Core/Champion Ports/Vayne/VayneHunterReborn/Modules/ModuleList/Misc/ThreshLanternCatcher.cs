using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy;
using LeagueSharp.Common;
namespace VayneHunter_Reborn.Modules.ModuleList.Misc
{
    class ThreshLanternCatcher : IModule
    {
        private GameObject LanternObject;


        public void OnLoad()
        {
            GameObject.OnCreate += OnObjCreate;
            GameObject.OnDelete += OnObjDelete;
        }

        private void OnObjDelete(GameObject sender, System.EventArgs args)
        {
            if (LanternObject != null && sender != null && sender.NetworkId == LanternObject.NetworkId)
            {
                LanternObject = null;
            }
        }

        private void OnObjCreate(GameObject sender, System.EventArgs args)
        {
            if (sender.IsValid<Obj_AI_Minion>()
                && sender.IsAlly
                && sender.Name.Equals("ThreshLantern", StringComparison.OrdinalIgnoreCase))
            {
                LanternObject = sender;
            }
        }

        public bool ShouldGetExecuted()
        {
            return (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.general.threshCatch")
                && ObjectManager.Player.HealthPercent < MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.general.hpThresh").Value);
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (LanternObject == null)
                return;

            if (ObjectManager.Player.ServerPosition.Distance(LanternObject.Position) <= 500f)
            {
                //Cast Interact spell
                ObjectManager.Player.Spellbook.CastSpell((SpellSlot)62, LanternObject);
            }
        }
    }
}
