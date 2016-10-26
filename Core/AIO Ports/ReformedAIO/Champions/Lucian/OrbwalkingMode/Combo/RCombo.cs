namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Damage;
    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly LucDamage damage;

        private readonly RSpell rSpell;

        public RCombo(RSpell rSpell, LucDamage damage)
        {
            this.rSpell = rSpell;
            this.damage = damage;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            var target = TargetSelector.GetTarget(Menu.Item("Range").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);

            if (target == null
                || !target.IsValidTarget(rSpell.Spell.Range)
                || target.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange
                || (Menu.Item("RKillable").GetValue<bool>() && damage.GetComboDamage(target) < target.Health)
                || (Menu.Item("SafetyCheck").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) > ObjectManager.Player.CountAlliesInRange(1400)))
            {
                return;
            }

            var pred = rSpell.Spell.GetPrediction(target);

            if (pred.Hitchance >= HitChance.High)
            {
                rSpell.Spell.Cast(pred.CastPosition);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("RKillable", "Only If Killable").SetValue(true));
            Menu.AddItem(new MenuItem("SafetyCheck", "Safety Check").SetValue(true));
            Menu.AddItem(new MenuItem("Range", "R Range").SetValue(new Slider(1400, 150, 1400)));
            Menu.AddItem(new MenuItem("RMana", "Min Mana %").SetValue(new Slider(25, 0, 100)));

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
    }
}
