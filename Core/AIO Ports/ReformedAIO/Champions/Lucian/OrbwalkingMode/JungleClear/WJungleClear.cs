using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.JungleClear
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WJungleClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WJungleClear(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (Menu.Item("WMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            var mob =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position,
                    wSpell.Spell.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault();

            if (mob == null)
            {
                return;
            }

            wSpell.Spell.Cast(mob);
        }


        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

         //   Menu.AddItem(new MenuItem("MinHit", "Min Hit").SetValue(new Slider(2, 0, 3)));
            Menu.AddItem(new MenuItem("WMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
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
