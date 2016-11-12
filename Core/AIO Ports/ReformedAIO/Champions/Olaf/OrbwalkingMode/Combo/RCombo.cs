using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Champions.Olaf.Core.Damage;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private HeroInfo heroInfo;

        private readonly OlafDamage damage;

        private readonly RSpell spell;

        public RCombo(RSpell spell, OlafDamage damage)
        {
            this.spell = spell;
            this.damage = damage;
        }

        private static AIHeroClient Target => TargetSelector.GetTarget(550, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent))
            {
                return;
            }

            if ((heroInfo.Immobilized(ObjectManager.Player) && Menu.Item("Immobilized").GetValue<bool>())
                || damage.GetComboDamage(Target) * 1.15 >= Target.Health && Menu.Item("Killable").GetValue<bool>())
            {
                spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("Killable", "Use When Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Immobilized", "Use When Immobilized").SetValue(true));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));

            heroInfo = new HeroInfo();
        }
    }
}
