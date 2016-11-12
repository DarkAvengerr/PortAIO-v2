using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Items.Defensives
{
    class _3180 : CoreItem
    {
        internal override int Id => 3180;
        internal override int Priority => 4;
        internal override string Name => "Odyns";
        internal override string DisplayName => "Odyn's Veil";
        internal override int Duration => 250;
        internal override float Range => 525f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfCount };
        internal override MapType[] Maps => new[] { MapType.CrystalScar };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (!Parent.Item(Parent.Name + "useon" + Player.NetworkId).GetValue<bool>())
                return;

            if (Player.Health/Player.MaxHealth * 100 <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
            {
                UseItem();
            }

            if (Player.CountEnemiesInRange(Range) >= Menu.Item("selfcount" + Name).GetValue<Slider>().Value)
            {
                UseItem();
            }
        }
    }
}
