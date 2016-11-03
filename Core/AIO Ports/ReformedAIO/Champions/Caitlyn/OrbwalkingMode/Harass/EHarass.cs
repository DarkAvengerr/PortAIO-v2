using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        public EHarass(ESpell eSpell)
        {
            this.eSpell = eSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(eSpell.Spell.Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Mana", "Mana %").SetValue(new Slider(0, 0, 100)));

            Menu.AddItem(new MenuItem("AntiGapcloser", "Anti Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("AntiMelee", "E Anti-Melee").SetValue(true));
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("AntiGapcloser").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null || !target.IsEnemy || !CheckGuardians())
            {
                return;
            }

            eSpell.Spell.Cast(gapcloser.End);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent || !CheckGuardians())
            {
                return;
            }

            if (ObjectManager.Player.Distance(Target) < ObjectManager.Player.AttackRange / 2 && Menu.Item("AntiMelee").GetValue<bool>())
            {
                eSpell.Spell.Cast(Target.Position);
            }

            var ePrediction = eSpell.Spell.GetPrediction(Target);

            if (ePrediction.Hitchance < HitChance.High)
            {
                return;
            }

            eSpell.Spell.Cast(ePrediction.CastPosition);
        }
    }
}
