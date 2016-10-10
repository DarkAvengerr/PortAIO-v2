using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons.ComboSystem;
using TheTwitch.Commons.Items;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons
{
    public class ItemManager : IManager
    {
        private Dictionary<IActivateableItem, bool> _items;
        private bool _combo;
        private ComboProvider _provider;
        private bool Enabled;

        public void Attach(Menu menu, ComboProvider provider)
        {
            _provider = provider;
            _items = new Dictionary<IActivateableItem, bool>();
            var items = new IActivateableItem[] { new BilgewaterCutlass(), new Botrk(), new YoumusBlade() };

            foreach (var activateableItem in items)
            {
                IActivateableItem item = activateableItem;
                if (item.GetRange() != int.MaxValue && item.GetRange() != 0 && item.GetRange() + 100 < ObjectManager.Player.AttackRange) continue;

                var itemMenu = new Menu(item.GetDisplayName(), item.GetDisplayName());
                item.Initialize(itemMenu, this);
                _items.Add(item, true);
                itemMenu.AddMItem("Enabled", true, (sender, agrs) => _items[item] = agrs.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                menu.AddSubMenu(itemMenu);
            }
            menu.AddMItem("Only in combo", true, (sender, args) => _combo = args.GetNewValue<bool>());
            menu.AddMItem("Enabled", true, (sender, args) => Enabled = args.GetNewValue<bool>());
            menu.ProcStoredValueChanged<bool>();
            Game.OnUpdate += _ => Update();
        }

        private void Update()
        {
            if (_provider.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && _combo || !Enabled) return;
            foreach (var item in _items)
                if (item.Value && _provider.Target.IsValidTarget())
                    item.Key.Update(_provider.Target);
        }

        public bool IsTickingNow(IActivateableItem item)
        {
            return _items.ContainsKey(item) && _items[item] && (!_combo || _provider.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo);
        }

        public T GetItem<T>() where T : IActivateableItem
        {
            foreach (var item in _items.Keys)
                if (item is T)
                    return (T)item;
            return default(T);
        }
    }
}
