using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Lane
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QLane(QSpell qSpell)
        {
            this.qSpell = qSpell;
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

            Menu.AddItem(new MenuItem("LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneQMana", "Mana %").SetValue(new Slider(65, 0, 100)));

            Menu.AddItem(new MenuItem("MinQHit", "Minimum Hit By Q").SetValue(new Slider(4, 0, 6)));
        }

      
        private void OnUpdate(EventArgs args)
        {
            if (Menu.Item("LaneQMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var minions = MinionManager.GetMinions(qSpell.Spell.Range);

            if (minions == null) return;

            if (Menu.Item("LaneQEnemy").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(2000) >= 1)
            {
                return;
            }

            var pos = qSpell.Spell.GetLineFarmLocation(minions);

            if(pos.MinionsHit >= Menu.Item("MinQHit").GetValue<Slider>().Value)

            qSpell.Spell.Cast(pos.Position);
        }
    }
}
