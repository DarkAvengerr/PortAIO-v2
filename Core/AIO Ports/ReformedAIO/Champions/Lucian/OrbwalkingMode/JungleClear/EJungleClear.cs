using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.JungleClear
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungleClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        public EJungleClear(ESpell eSpell)
        {
            this.eSpell = eSpell;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (!CheckGuardians())
            {
                return;
            }

            if (!eSpell.Spell.IsReady()
                || Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }


            var mob =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position,
                    ObjectManager.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault();

            if (mob == null)
            {
                return;
            }

            eSpell.Spell.Cast(eSpell.Deviation(ObjectManager.Player.Position.To2D(), mob.Position.To2D(), Menu.Item("Range").GetValue<Slider>().Value));
        }


        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Range", "Range").SetValue(new Slider(65, 0, 425)));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
             Orbwalking.AfterAttack -= AfterAttack;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Orbwalking.AfterAttack += AfterAttack;
        }
    }
}
