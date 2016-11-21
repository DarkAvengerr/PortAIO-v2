using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _3030 : CoreItem
    {
        internal override int Id => 3030;
        internal override int Priority => 5;
        internal override string Name => "GPL-800";
        internal override string DisplayName => "Hextech GPL-800";
        internal override int Duration => 100;
        internal override float Range => 375f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Tar != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                    return;

                // todo: check collision and/or cone prediction (may not be needed)
                // todo: this is just basic usage for now

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                     UseItem(Tar.Player.ServerPosition, true);
                }

                if (Player.Health / Player.MaxHealth * 100 <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player.ServerPosition, true);
                }
            }
        }
    }
}
