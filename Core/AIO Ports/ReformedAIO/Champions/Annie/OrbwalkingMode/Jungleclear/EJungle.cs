using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.OrbwalkingMode.Jungleclear
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EJungle(ESpell spell)
        {
            this.spell = spell;
        }

        private List<Obj_AI_Base> Mob =>
           MinionManager.GetMinions(ObjectManager.Player.Position,
               spell.Spell.Range,
               MinionTypes.All,
               MinionTeam.Neutral);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent))
            {
                return;
            }

            spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("Range", "Enemy Search Range").SetValue(new Slider(625, 0, 800)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }
    }
}
