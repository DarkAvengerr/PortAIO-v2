using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Utils
{
    using System.Linq;

    using ElRengarDecentralized.Components;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal static class ItemManager
    {
        /// <summary>
        ///     Gets the Blade of the Ruined king item.
        /// </summary>
        public static Items.Item Botrk => ItemData.Blade_of_the_Ruined_King.GetItem();

        /// <summary>
        ///     Gets the Bilgewater Cutlass item.
        /// </summary>
        public static Items.Item Cutlass => ItemData.Bilgewater_Cutlass.GetItem();

        /// <summary>
        ///     Gets the Youmuu Ghostblade item.
        /// </summary>
        public static Items.Item Youmuus => ItemData.Youmuus_Ghostblade.GetItem();

        /// <summary>
        ///     Gets the Tiamat item.
        /// </summary>
        public static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the Ravenous Hydra item.
        /// </summary>
        public static Items.Item RavenousHydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the Titanic Hydra item.
        /// </summary>
        public static Items.Item TitanicHydra => new Items.Item(3748, 385);

        /// <summary>
        ///     Gets if the item is ready.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        ///     If the item is valid and ready.
        /// </returns>
        public static bool IsValidAndReady(this Items.Item item) => item != null && item.IsReady();

        /// <summary>
        ///     Checks the menu item.
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            var name = "Items" + Program.Orbwalker.ActiveMode.ToString().ToLower();
            var item = MyMenu.RootMenu.Item(name);
            return item != null && item.IsActive();
        }

        /// <summary>
        ///     Item casting handler.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CastItems(Obj_AI_Base target)
        {
            if (ObjectManager.Player.IsDashing() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return false;
            }

            var botrk = Botrk;
            if (botrk.IsValidAndReady() && botrk.Cast(target))
            {
                return true;
            }

            var cutlass = Cutlass;
            if (cutlass.IsValidAndReady() && cutlass.Cast(target))
            {
                return true;
            }

            if (!Program.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LaneClear))
            {
                var youmuus = Youmuus;
                if (youmuus.IsValidAndReady() && youmuus.Cast())
                {
                    return true;
                }
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var heroes = ObjectManager.Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = Tiamat;
            if (tiamat.IsValidAndReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = RavenousHydra;
            if (hydra.IsValidAndReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var titanic = TitanicHydra;
            return titanic.IsValidAndReady() && count > 0 && titanic.Cast();
        }
    }
}
