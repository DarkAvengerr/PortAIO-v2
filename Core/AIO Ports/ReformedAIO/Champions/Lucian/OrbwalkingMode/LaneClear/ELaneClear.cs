namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.LaneClear
{
    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;
    using ReformedAIO.Core.Dash_Handler;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELaneClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        private readonly DashSmart dashSmart;

        public ELaneClear(ESpell eSpell, DashSmart dashSmart)
        {
            this.eSpell = eSpell;
            this.dashSmart = dashSmart;
        }

        private void OnUpdate(EventArgs args)
        {
            if ((Menu.Item("EnemiesCheck").GetValue<bool>()
                && ObjectManager.Player.CountEnemiesInRange(1500) >= 1)
                || (ObjectManager.Player.ManaPercent <= Menu.Item("EMana").GetValue<Slider>().Value)
                || !CheckGuardians())
            {
                return;
            }

            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, eSpell.Spell.Range).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            eSpell.Spell.Cast(dashSmart.Deviation(ObjectManager.Player.Position.To2D(), minion.Position.To2D(), Menu.Item("Range").GetValue<Slider>().Value));
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EnemiesCheck", "Check Enemies First").SetValue(true).SetTooltip("Wont E If Nearby Enemies"));
            Menu.AddItem(new MenuItem("Range", "E Range").SetValue(new Slider(65, 1, 425)));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(60, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }
    }
}
