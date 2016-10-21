using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons.Summoners
{
    class Heal : ISummonerSpell
    {
        private readonly Dictionary<string, bool> _saveAllies = new Dictionary<string, bool>();
        private Slider _minHealth, _burstHealth, _allyHealth, _enemyDistance;
        private readonly float[] _healthStates = new float[10];
        private float _lastHealthState;

        public void Initialize(Menu menu)
        {
            for (int i = 0; i < _healthStates.Length; i++)
                _healthStates[i] = ObjectManager.Player.HealthPercent;

            menu.AddMItem("Use when:");
            menu.AddMItem("Health below %", new Slider(15, 1, 35), (sender, args) => _minHealth = args.GetNewValue<Slider>());
            menu.AddMItem("Health % gone in < 1 sec", new Slider(60), (sender, args) => _burstHealth = args.GetNewValue<Slider>());
            if (HeroManager.Allies.Any(ally => ally.NetworkId != ObjectManager.Player.NetworkId))
            {
                var allies = menu.CreateSubmenu("Save allies");
                allies.AddMItem("When health below %", new Slider(15, 1, 35), (sender, args) => _allyHealth = args.GetNewValue<Slider>());
                allies.AddMItem("And enemy distance <", new Slider(1500, 500, 2500), (sender, args) => _enemyDistance = args.GetNewValue<Slider>());
                HeroManager.Allies.Where(ally => ally.NetworkId != ObjectManager.Player.NetworkId).ToList().ForEach(ally => allies.AddMItem("Save " + ally.ChampionName, true, (sender, args) => _saveAllies[ally.ChampionName] = args.GetNewValue<bool>()));
                allies.ProcStoredValueChanged<bool>();
                allies.ProcStoredValueChanged<Slider>();
            }
            menu.ProcStoredValueChanged<Slider>();

            Game.OnUpdate += (eArgs) =>
            {
                if (_lastHealthState + 0.1f < Game.Time)
                {
                    _lastHealthState = Game.Time;
                    _healthStates[(int)((Game.Time * 10) % 10)] = ObjectManager.Player.HealthPercent;
                }
            };

            //Drawing.OnDraw += (args) =>
            //{
            //    for (int i = 0; i < _healthStates.Length; i++)
            //    {
            //        Drawing.DrawText(800, 200 + i * 20, Color.Blue, _healthStates[i].ToString());
            //    }
            //};
        }

        public void Update()
        {
            var saveAlly = HeroManager.Allies.FirstOrDefault(ally =>  ally.IsValid && !ally.IsDead && ally.HealthPercent < _allyHealth.Value && ally.Distance(ObjectManager.Player) < 850 && HeroManager.Enemies.Any(enemy => enemy.Distance(ally) < _enemyDistance.Value));
            if (saveAlly != null)
            {
                ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerheal").Slot, saveAlly);
            }

            if (_healthStates[(int)((Game.Time * 10 + 1) % 10)] > ObjectManager.Player.HealthPercent + _burstHealth.Value)
            {
                ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerheal").Slot);
            }

            if(_minHealth.Value > ObjectManager.Player.HealthPercent)
                ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerheal").Slot);
        }

        public string GetDisplayName()
        {
            return "Heal";
        }

        public bool IsAvailable()
        {
            return ObjectManager.Player.Spellbook.Spells.Any(spell => spell.Name == "summonerheal");
        }
    }
}
