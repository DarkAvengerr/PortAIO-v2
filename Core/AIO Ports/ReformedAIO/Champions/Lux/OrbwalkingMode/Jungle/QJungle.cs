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

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QJungle(QSpell spell)
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
                || Mob.HasBuff("luxilluminatingfraulein")
                || Menu.Item("Lux.Jungle.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            spell.Spell.Cast(Mob);
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


            Menu.AddItem(new MenuItem("Lux.Jungle.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
