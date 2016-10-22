using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility.MenuUtility;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Modules.ModuleList.Misc
{
    internal class BuyBlueTrinket : IModule
    {
        private static float LastCheckTick = 0f;

        public void OnLoad() {}

        public bool ShouldGetExecuted()
        {
            return (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.general.blueTrinket"));
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (Environment.TickCount - LastCheckTick < 300f)
            {
                return;
            }

            LastCheckTick = Environment.TickCount;

            if (ObjectManager.Player.IsDead || ObjectManager.Player.InShop())
            {
                if (!ItemData.Farsight_Alteration.GetItem().IsOwned())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        250, () => Shop.BuyItem(ItemId.Farsight_Alteration));
                }

            }
        }
    }
}

