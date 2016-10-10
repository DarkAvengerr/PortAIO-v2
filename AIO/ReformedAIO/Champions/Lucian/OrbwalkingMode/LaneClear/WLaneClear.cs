using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.LaneClear
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WLaneClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WLaneClear(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Menu.Item("EnemiesCheck").GetValue<bool>()
                && ObjectManager.Player.CountEnemiesInRange(1350) >= 1
                || (ObjectManager.Player.ManaPercent <= Menu.Item("WMana").GetValue<Slider>().Value)
                || ObjectManager.Player.HasBuff("LucianPassiveBuff")
                || !CheckGuardians())
            {
                return;
            }

            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, wSpell.Spell.Range);

            var pred = wSpell.Spell.GetCircularFarmLocation(minion);

            if (pred.MinionsHit >= Menu.Item("MinHit").GetValue<Slider>().Value)
            {
                wSpell.Spell.Cast(pred.Position);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EnemiesCheck", "Check Enemies First").SetValue(true).SetTooltip("Wont W If Nearby Enemies"));
            Menu.AddItem(new MenuItem("MinHit", "Min Hit By W").SetValue(new Slider(3, 0, 6)));
            Menu.AddItem(new MenuItem("WMana", "Min Mana %").SetValue(new Slider(80, 0, 100)));
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
