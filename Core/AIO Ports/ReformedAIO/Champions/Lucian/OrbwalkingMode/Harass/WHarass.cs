using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Harass
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WHarass(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (!CheckGuardians()
                || Menu.Item("WMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                if (Menu.Item("WPred").GetValue<bool>())
                {
                    wSpell.Spell.Cast(target.Position);
                }
                else
                {
                    var wPred = wSpell.Spell.GetPrediction(target, true);

                    if (wPred.Hitchance > HitChance.Medium)
                    {
                        wSpell.Spell.Cast(wPred.CastPosition);
                    }
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("WPred", "Disable Prediction").SetValue(true));
            Menu.AddItem(new MenuItem("WMana", "Min Mana %").SetValue(new Slider(20, 0, 100)));
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
