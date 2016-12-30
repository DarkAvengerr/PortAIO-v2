using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EHarass(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Caitlyn.Harass.E.Mana", "Mana %").SetValue(new Slider(0, 0, 100)));

            Menu.AddItem(new MenuItem("Caitlyn.Harass.E.AntiGapcloser", "Anti Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Harass.E.AntiMelee", "E Anti-Melee").SetValue(true));
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("Caitlyn.Harass.E.AntiGapcloser").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null || !target.IsEnemy || !CheckGuardians())
            {
                return;
            }

            spell.Spell.Cast(gapcloser.End);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Menu.Item("Caitlyn.Harass.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            var ePrediction = spell.Spell.GetPrediction(Target);

            if (ObjectManager.Player.Distance(Target) < ObjectManager.Player.AttackRange / 2 && Menu.Item("Caitlyn.Harass.E.AntiMelee").GetValue<bool>() && ePrediction.Hitchance >= HitChance.High)
            {
                spell.Spell.Cast(Target.Position);
            }

            if (ePrediction.Hitchance < HitChance.VeryHigh)
            {
                return;
            }

            spell.Spell.Cast(ePrediction.CastPosition);
        }
    }
}
