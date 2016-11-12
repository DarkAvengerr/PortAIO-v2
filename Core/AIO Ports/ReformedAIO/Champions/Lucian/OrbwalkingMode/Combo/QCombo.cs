using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

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

            if (!Menu.Item("ExtendedQ").GetValue<bool>() || target.Distance(ObjectManager.Player) <= q2Spell.Spell.Range)
            {
                return;
            }

            var minions = MinionManager.GetMinions(q2Spell.Spell.Range);

            foreach (var m in minions)
            {
                if (q2Spell.QMinionExtend(m))
                {
                    qSpell.Spell.Cast(m);
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                ||Menu.Item("QMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("ExtendedQ", "Extended Q").SetValue(true));
            Menu.AddItem(new MenuItem("QMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }
    }
}
