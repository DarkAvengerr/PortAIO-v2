using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons.ComboSystem;
using TheTwitch.Commons.Summoners;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons
{
    public class SummonerManager : IManager
    {
        private Dictionary<ISummonerSpell, bool> _summs;
        private bool _combo;
        private bool Enabled;

        public void Attach(Menu menu, ComboProvider provider)
        {
            _summs = new Dictionary<ISummonerSpell, bool>();
            var summs = new ISummonerSpell[] { new Heal(), new Cleanse() };

            foreach (var activateableItem in summs)
            {
                ISummonerSpell item = activateableItem;

                if (!activateableItem.IsAvailable()) continue;

                var itemMenu = new Menu(item.GetDisplayName(), item.GetDisplayName());
                item.Initialize(itemMenu);
                _summs.Add(item, true);
                itemMenu.AddMItem("Enabled", true, (sender, agrs) => _summs[item] = agrs.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                menu.AddSubMenu(itemMenu);
            }

            if (summs.All(sum => !sum.IsAvailable()))
            {
                menu.AddMItem("- No supported summoner spell available -");
                menu.AddMItem("Supported spells:");
                foreach (var summonerSpell in summs)
                {
                    menu.AddMItem("* " + summonerSpell.GetDisplayName());
                }
            }
            else
            {
                menu.AddMItem("Only in combo", true, (sender, args) => _combo = args.GetNewValue<bool>());
                menu.AddMItem("Enabled", false, (sender, args) => Enabled = args.GetNewValue<bool>());
            }
            menu.ProcStoredValueChanged<bool>();
            Game.OnUpdate += _ => Update(provider);
        }

        private void Update(ComboProvider provider)
        {
            if (provider.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && _combo || !Enabled) return;
            foreach (var summ in _summs)
                if (summ.Value)
                    summ.Key.Update();
        }

        public T GetItem<T>() where T : ISummonerSpell
        {
            foreach (var item in _summs.Keys)
                if (item is T)
                    return (T)item;
            return default(T);
        }
    }
}
