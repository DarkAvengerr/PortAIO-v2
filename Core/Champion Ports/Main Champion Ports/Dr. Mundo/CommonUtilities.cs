using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Mundo
{
    internal class CommonUtilities
    {
        public static HitChance GetHitChance(string name)
        {
            var hitChance = ConfigMenu.config.Item(name).GetValue<StringList>();

            switch (hitChance.SList[hitChance.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }

        public static bool CheckItem()
        {
            return ItemData.Tiamat_Melee_Only.GetItem().IsReady() ||
                   ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady() ||
                   ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady();
        }

        public static void UseItem()
        {
            if (!CheckItem())
                return;

            if (ItemData.Tiamat_Melee_Only.GetItem().IsOwned(ObjectManager.Player))
            {
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            }
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsOwned(ObjectManager.Player))
            {
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            }
            if (ItemData.Titanic_Hydra_Melee_Only.GetItem().IsOwned(ObjectManager.Player))
            {
                ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();
            }
        }
    }
}
