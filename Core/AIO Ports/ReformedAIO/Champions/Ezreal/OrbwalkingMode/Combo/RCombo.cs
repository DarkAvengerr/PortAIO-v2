using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell rSpell;

        public RCombo(RSpell rSpell)
        {
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(Menu.Item("Range").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || !CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && ObjectManager.Player.UnderTurret(true))
                || Target.Distance(ObjectManager.Player) < 900)
            {
                return;
            }

            rSpell.Spell.CastIfWillHit(Target, Menu.Item("Hit").GetValue<Slider>().Value);

            if (Target.HealthPercent > Menu.Item("Health").GetValue<Slider>().Value)
            {
                return;
            }

            var prediction = rSpell.Spell.GetPrediction(Target);

            switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (prediction.Hitchance >= HitChance.Medium)
                    {
                        rSpell.Spell.Cast(prediction.CastPosition);
                    }
                    break;
                case 1:
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        rSpell.Spell.Cast(prediction.CastPosition);
                    }
                    break;
                case 2:
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        rSpell.Spell.Cast(prediction.CastPosition);
                    }
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));

            Menu.AddItem(new MenuItem("Turret", "Enemy Turret Check").SetValue(true));

            Menu.AddItem(new MenuItem("Health", "Use When Target Health %").SetValue(new Slider(30, 0, 100)));

            Menu.AddItem(new MenuItem("Hit", "Auto if X Hit").SetValue(new Slider(3, 0, 5)));

            Menu.AddItem(new MenuItem("Range", "Range").SetValue(new Slider(3000, 500, 4000)));
        }
    }
}
