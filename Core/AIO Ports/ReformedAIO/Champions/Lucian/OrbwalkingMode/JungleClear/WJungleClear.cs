using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.JungleClear
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WJungleClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WJungleClear(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!CheckGuardians() 
                || Menu.Item("Jungle.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.Position,
                                               wSpell.Spell.Range,
                                               MinionTypes.All,
                                               MinionTeam.Neutral);

            if (mob == null)
            {
                return;
            }

            foreach (var m in mob)
            {
                wSpell.Spell.Cast(m.Position);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Jungle.W.Mana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Orbwalking.AfterAttack -= AfterAttack;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Orbwalking.AfterAttack += AfterAttack;
        }
    }
}
