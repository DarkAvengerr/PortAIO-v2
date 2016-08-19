using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheGaren;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons.SummonerSpells
{
    class Cleanse : ISummonerSpell
    {
        private bool _blind, _stun, _fear, _snare, _polymorph, _silence, _charm, _exhaust, _ignite, _sleep, _taunt, _noAliW;
        private Slider _minDuration;

        public void Initialize(Menu menu)
        {
            menu.AddMItem("Min duration in ms", new Slider(1000, 0, 3000), (sender, args) => _minDuration = args.GetNewValue<Slider>());
            menu.AddMItem("1000 ms = 1 sec");
            var typeMenu = menu.CreateSubmenu("Use on");
            typeMenu.AddMItem("Blind", false, (sender, args) => _blind = args.GetNewValue<bool>());
            typeMenu.AddMItem("Silence", false, (sender, args) => _silence = args.GetNewValue<bool>());

            typeMenu.AddMItem("Stun", true, (sender, args) => _stun = args.GetNewValue<bool>());
            typeMenu.AddMItem("Fear", true, (sender, args) => _fear = args.GetNewValue<bool>());
            typeMenu.AddMItem("Snare", true, (sender, args) => _snare = args.GetNewValue<bool>());
            typeMenu.AddMItem("Polymorph", true, (sender, args) => _polymorph = args.GetNewValue<bool>());
            typeMenu.AddMItem("Charm", true, (sender, args) => _charm = args.GetNewValue<bool>());
            typeMenu.AddMItem("Sleep", true, (sender, args) => _sleep = args.GetNewValue<bool>());
            typeMenu.AddMItem("Taunt", true, (sender, args) => _taunt = args.GetNewValue<bool>());
            var miscMenu = menu.CreateSubmenu("Misc");
            miscMenu.AddMItem("Don't use on Alistar W (is a stun)", true, (sender, args) => _noAliW = args.GetNewValue<bool>());
            miscMenu.AddMItem("Use on killable Ignite", true, (sender, args) => _ignite = args.GetNewValue<bool>());
            miscMenu.AddMItem("Use on Exhaust", true, (sender, args) => _exhaust = args.GetNewValue<bool>());

        }

        public void Update()
        {
            foreach (var buff in ObjectManager.Player.Buffs)
            {
                if (buff.Type == BuffType.Blind && _blind || buff.Type == BuffType.Stun && _stun || buff.Type == BuffType.Fear && _fear || buff.Type == BuffType.Snare && _snare || buff.Type == BuffType.Polymorph && _polymorph || buff.Type == BuffType.Silence && _silence || buff.Type == BuffType.Charm && _charm ||
                     buff.Type == BuffType.Taunt && _taunt)
                {
                    //Console.WriteLine((buff.EndTime - Game.Time) + "buff.EndTime - Game.Time > _minDuration.Value / 1000f" + _minDuration.Value / 1000f + " spell:" + buff.Type + " caster: " + buff.Caster.Name);

                    if (buff.Caster.Type == GameObjectType.AIHeroClient && ((AIHeroClient)buff.Caster).ChampionName == "Alistar" && _noAliW) continue;

                    if (buff.EndTime - Game.Time > _minDuration.Value / 1000f)
                        ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerboost").Slot);
                }

                if (_ignite && buff.Name == "summonerdot" && GetRemainingIgniteDamage(ObjectManager.Player) > ObjectManager.Player.Health)
                    ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerboost").Slot);

                if (_exhaust && buff.Name == "summonerexhaust")
                    ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "summonerboost").Slot);
            }
        }


        public string GetDisplayName()
        {
            return "Cleanse (WIP)";
        }

        public bool IsAvailable()
        {
            return ObjectManager.Player.Spellbook.Spells.Any(spell => spell.Name == "summonerboost");
        }

        public static float GetRemainingIgniteDamage(Obj_AI_Base target)
        {
            var ignitebuff = target.GetBuff("summonerdot");
            if (ignitebuff == null) return 0;
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.True, ((int)(ignitebuff.EndTime - Game.Time) + 1) * (50 + ((AIHeroClient)ignitebuff.Caster).Level * 20) / 5);
        }
    }
}
