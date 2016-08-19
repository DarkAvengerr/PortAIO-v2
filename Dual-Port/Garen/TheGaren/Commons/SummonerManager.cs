using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheKalista.Commons.ComboSystem;
using TheGaren;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons
{
    public class SummonerManager
    {
        private Dictionary<ISummonerSpell, bool> _summs;
        private bool _combo;
        private bool _enabled;

        public void Attach(Menu menu, ComboProvider provider, params ISummonerSpell[] summs)
        {
            _summs = new Dictionary<ISummonerSpell, bool>();
            
            foreach (var activateableItem in summs)
            {
                ISummonerSpell item = activateableItem;

                if (!activateableItem.IsAvailable()) continue;

                var itemMenu = new Menu(item.GetDisplayName(), item.GetDisplayName());
                item.Initialize(itemMenu);
                _summs.Add(item, true);
                itemMenu.AddMItem("Enabled", true, (sender, agrs) => _summs[item] = agrs.GetNewValue<bool>());
                menu.AddSubMenu(itemMenu);
            }

            if (summs.All(sum => !sum.IsAvailable()))
            {
                menu.AddMItem("- No Supported Summoner Spell Available -");
                menu.AddMItem("Supported Spells:");
                foreach (var summonerSpell in summs)
                {
                    menu.AddMItem("* " + summonerSpell.GetDisplayName());
                }
            }
            else
            {
                menu.AddMItem("Only in Combo", true, (sender, args) => _combo = args.GetNewValue<bool>());
                menu.AddMItem("Enabled", false, (sender, args) => _enabled = args.GetNewValue<bool>());
            }
            Game.OnUpdate += _ => Update(provider);
        }

        private void Update(ComboProvider provider)
        {
            if (provider.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && _combo || !_enabled) return;
            foreach (var summ in _summs)
                if (summ.Value)
                    summ.Key.Update();
        }

        public T GetFirstItem<T>() where T : ISummonerSpell
        {
            return (T)_summs.Keys.FirstOrDefault(item => item is T);
        }
    }
}
