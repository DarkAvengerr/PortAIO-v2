using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.LaneClear
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SharpDX;

    internal sealed class QLaneClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QLaneClear(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        private void OnUpdate(EventArgs args)
        {
              if ((Menu.Item("EnemiesCheck").GetValue<bool>()
                && ObjectManager.Player.CountEnemiesInRange(1750) >= 1)
                || (ObjectManager.Player.ManaPercent <= Menu.Item("QMana").GetValue<Slider>().Value)
                || !CheckGuardians())
            {
                return;
            }
           
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, qSpell.Spell.Range).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            var prediction = qSpell.Spell.GetPrediction(minions, true);

            var collision = qSpell.Spell.GetCollision(ObjectManager.Player.Position.To2D(), new List<Vector2> { prediction.UnitPosition.To2D() });

            if (collision.Count >= Menu.Item("MinHit").GetValue<Slider>().Value)
            {
                qSpell.Spell.Cast(collision[0]);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("EnemiesCheck", "Check Enemies First").SetValue(true).SetTooltip("Wont Q If Nearby Enemies"));
            Menu.AddItem(new MenuItem("MinHit", "Min Hit By Q").SetValue(new Slider(3, 0, 6)));
            Menu.AddItem(new MenuItem("QMana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
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
    }
}
