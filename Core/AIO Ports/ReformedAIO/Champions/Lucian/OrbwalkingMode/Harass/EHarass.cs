namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Harass
{
    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Damage;
    using ReformedAIO.Champions.Lucian.Spells;
    using ReformedAIO.Core.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly LucDamage damage;

        private readonly ESpell eSpell;

        private readonly DashSmart dashSmart;

        public EHarass(ESpell eSpell, LucDamage damage, DashSmart dashSmart)
        {
            this.eSpell = eSpell;
            this.damage = damage;
            this.dashSmart = dashSmart;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (!CheckGuardians()
                || Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                if (target.Health < damage.GetComboDamage(target) && ObjectManager.Player.HealthPercent > target.HealthPercent)
                {
                    eSpell.Spell.Cast(target.Position);
                }
                else
                {
                    if (target.UnderTurret(true))
                    {
                        return;
                    }

                    switch (Menu.Item("EMode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            eSpell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, Menu.Item("EDistance").GetValue<Slider>().Value));
                            break;
                        case 1:
                            eSpell.Spell.Cast(dashSmart.Deviation(ObjectManager.Player.Position.To2D(), target.Position.To2D(), Menu.Item("EDistance").GetValue<Slider>().Value).To3D());
                            break;
                        case 2:
                            eSpell.Spell.Cast(dashSmart.ToSafePosition(target, target.Position, Menu.Item("EDistance").GetValue<Slider>().Value));
                            break;
                    }
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EMode", "Mode").SetValue(new StringList(new[] { "Cursor", "Side", "Automatic" })));
            Menu.AddItem(new MenuItem("EDistance", "E Distance").SetValue(new Slider(65, 1, 425)).SetTooltip("Less = Faster"));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Orbwalking.AfterAttack -= AfterAttack;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Orbwalking.AfterAttack += AfterAttack;
        }
    }
}
