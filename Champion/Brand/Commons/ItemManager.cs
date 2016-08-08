using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.Commons.ComboSystem;
using TheBrand.Commons.Items;
using EloBuddy;

namespace TheBrand.Commons
{
    public static class ItemManager
    {
        private static Dictionary<IActivateableItem, bool> _items;
        private static bool _combo, _harass;

        public static void Initialize(Menu menu, ComboProvider combo)
        {
            _items = new Dictionary<IActivateableItem, bool>();
            var items = new IActivateableItem[] { new BilgewaterCutlass(), new Botrk(), new YoumusBlade(), new RavenousHydra() };

            foreach (var activateableItem in items)
            {
                IActivateableItem item = activateableItem;

                var itemMenu = new Menu(item.GetDisplayName(), item.GetDisplayName());
                item.Initialize(itemMenu);
                _items.Add(item, true);
                itemMenu.AddMItem("Enabled", true, (sender, agrs) => _items[item] = agrs.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                menu.AddSubMenu(itemMenu);
            }
            menu.AddMItem("Use in combo", true, (sender, args) => _combo = args.GetNewValue<bool>());
            menu.AddMItem("Use in harass", false, (sender, args) => _harass = args.GetNewValue<bool>());
            menu.ProcStoredValueChanged<bool>();
            Game.OnUpdate += _ => Update(combo);
        }

        private static void Update(ComboProvider combo)
        {
            var target = combo.Target;
            if (!target.LSIsValidTarget()) return;
            if (combo.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !_combo || combo.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && !_harass || (combo.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && combo.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)) return;
            foreach (var item in _items)
                if (item.Value)
                    item.Key.Update(target);
        }

        public static T GetItem<T>() where T : IActivateableItem
        {
            foreach (var item in _items.Keys)
                if (item is T)
                    return (T)item;
            return default(T);
        }
    }
}
