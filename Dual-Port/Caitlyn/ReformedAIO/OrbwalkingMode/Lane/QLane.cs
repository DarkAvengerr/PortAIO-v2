using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Lane
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QLane : ChildBase
    {
        private readonly Orbwalking.Orbwalker orbwalker;

        public QLane(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }


        public override string Name { get; set; }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem(Name + "LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneQMana", "Mana %").SetValue(new Slider(65, 0, 100)));

            Menu.AddItem(new MenuItem(Name + "MinQHit", "Minimum Hit By Q").SetValue(new Slider(4, 0, 6)));
        }

      
        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Spells.Spell[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneQMana").GetValue<Slider>().Value > Vars.Player.ManaPercent) return;

            var minions = MinionManager.GetMinions(Spells.Spell[SpellSlot.Q].Range);

            if (minions == null) return;

            if (Menu.Item(Menu.Name + "LaneQEnemy").GetValue<bool>()
                && minions.Any(m => m.CountEnemiesInRange(2500) > 0))
            {
                return;
            }

            var pos = Spells.Spell[SpellSlot.Q].GetLineFarmLocation(minions);

            if(pos.MinionsHit >= Menu.Item(Menu.Name + "MinQHit").GetValue<Slider>().Value)

            Spells.Spell[SpellSlot.Q].Cast(pos.Position);
        }
    }
}
