namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo  : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Spell[SpellSlot.E].Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EMana", "Mana %").SetValue(new Slider(0, 0, 100)));

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

            Spells.Spell[SpellSlot.E].Cast(gapcloser.End);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Menu.Item("EMana").GetValue<Slider>().Value > Vars.Player.ManaPercent || !CheckGuardians())
            {
                return;
            }

            if (Vars.Player.Distance(Target) < 270 && Menu.Item("AntiMelee").GetValue<bool>())
            {
                Spells.Spell[SpellSlot.E].Cast(Target.Position);
            }

            var ePrediction = Spells.Spell[SpellSlot.E].GetPrediction(this.Target);

            if (ePrediction.Hitchance < HitChance.High)
            {
                return;
            }

            Spells.Spell[SpellSlot.E].Cast(ePrediction.CastPosition);
        }
    }
}
