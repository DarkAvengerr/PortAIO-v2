using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Consumables
{
    class _2137 : CoreItem
    {
        internal override int Id => 2137;
        internal override int Priority => 3;
        internal override string Name => "Elixir of Ruin";
        internal override string DisplayName => "Elixir of Ruin";
        internal override int Duration => 101;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP };
        internal override MapType[] Maps => new[] { MapType.SummonersRift, MapType.TwistedTreeline, MapType.HowlingAbyss };
        internal override int DefaultHP => 10;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Player.HasBuff("ElixirOfRuin"))
                        return;

                    if (hero.Player.IsRecalling() || hero.Player.InFountain())
                        return;

                    if (hero.Player.Health/hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value && hero.IncomeDamage > 0)
                        UseItem();

                    if (ShouldUseOnMany(hero))
                        UseItem();
                }
            }
        }
    }
}
