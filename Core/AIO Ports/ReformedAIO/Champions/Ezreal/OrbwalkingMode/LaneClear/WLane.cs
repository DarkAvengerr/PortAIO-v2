using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.OrbwalkingMode.LaneClear
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ezreal.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WLane(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent || !ObjectManager.Player.UnderTurret(true))
            {
                return;
            }

            var allies = ObjectManager.Player.GetAlliesInRange(wSpell.Spell.Range)
                    .Where(x => !x.IsMe)
                    .FirstOrDefault(x => x.Distance(ObjectManager.Player.Position) <= wSpell.Spell.Range);

            var prediction = wSpell.Spell.GetPrediction(allies);

            if (allies != null && prediction.Hitchance >= HitChance.Medium)
            {
                wSpell.Spell.Cast(prediction.CastPosition);
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

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
