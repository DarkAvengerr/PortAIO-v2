using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        private readonly Q2Spell q2Spell;

        public QCombo(QSpell qSpell, Q2Spell q2Spell)
        {
            this.qSpell = qSpell;
            this.q2Spell = q2Spell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            var target = TargetSelector.GetTarget(q2Spell.Spell.Range, TargetSelector.DamageType.Physical);

            if (target == null)
            {
                return;
            }

            if (!Menu.Item("ExtendedQ").GetValue<bool>() || target.Distance(ObjectManager.Player) <= qSpell.Spell.Range || !q2Spell.QMinionExtend())
            {
                return;
            }

            var m = MinionManager.GetMinions(qSpell.Spell.Range).FirstOrDefault();

            qSpell.Spell.CastOnUnit(m);
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (Menu.Item("QMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(qSpell.Spell.Range + 30));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                qSpell.Spell.CastOnUnit(target);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("ExtendedQ", "Extended Q").SetValue(true));
            Menu.AddItem(new MenuItem("QMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
            Orbwalking.AfterAttack -= AfterAttack;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAttack;
        }
    }
}
