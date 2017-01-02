using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Damage;
    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly LuxDamage damage;

        private readonly RSpell spell;

        private readonly ESpell eSpell;

        public RCombo(RSpell spell, ESpell eSpell, LuxDamage damage)
        {
            this.spell = spell;
            this.eSpell = eSpell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Target.Distance(ObjectManager.Player) > 1100
                || (Menu.Item("Lux.Combo.R.Safe").GetValue<bool>() && Target.Distance(ObjectManager.Player) < 125)
                || Menu.Item("Lux.Combo.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            spell.Spell.CastIfWillHit(Target, Menu.Item("Lux.Combo.R.Hit").GetValue<Slider>().Value);

           
            if (Menu.Item("Lux.Combo.R.E").GetValue<bool>() && eSpell.IsActive && eSpell.InRange(Target))
            {
                eSpell.Spell.Cast();
            }

            if (HealthPrediction.GetHealthPrediction(Target, (int)spell.Spell.Delay + Game.Ping / 2) < spell.GetDamage(Target) && Target.Distance(ObjectManager.Player) <= 1000)
            {
                Cast();
            }
        }

        private void Cast()
        {
            switch (Menu.Item("Lux.Combo.R.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
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

            Menu.AddItem(new MenuItem("Lux.Combo.R.E", "Use If E Active (Burst)").SetValue(true));

            Menu.AddItem(new MenuItem("Lux.Combo.R.Safe", "Don't Use If Close").SetValue(true));

            Menu.AddItem(new MenuItem("Lux.Combo.R.Hit", "Use If X Hit").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("Lux.Combo.R.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Lux.Combo.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
