using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EJungle(ESpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Mob =>
              MinionManager.GetMinions(ObjectManager.Player.Position,
                  spell.Spell.Range,
                  MinionTypes.All,
                  MinionTeam.Neutral).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Mob == null
                || Menu.Item("Lux.Jungle.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (spell.IsActive && spell.InRange(Mob))
            {
                spell.Spell.Cast();
            }
            else
            {
                spell.Spell.Cast(spell.Prediction(Mob).CastPosition);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);


            Menu.AddItem(new MenuItem("Lux.Jungle.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
