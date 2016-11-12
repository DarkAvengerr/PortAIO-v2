using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _2051 : CoreItem
    {
        internal override int Id => 2051;
        internal override int Priority => 5;
        internal override string Name => "Guardians";
        internal override string DisplayName => "Guardian's Horn";
        internal override MapType[] Maps => new[] { MapType.HowlingAbyss };
        internal override int Duration => 100;
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP, MenuType.SelfCount };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.CountEnemiesInRange(Range) > Menu.Item("selfcount" + Name).GetValue<Slider>().Value)
            {
                UseItem();
            }

            if (Tar != null)
            {
                if ((Player.Health / Player.MaxHealth * 100) <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player);
                }

                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                    return;

                if ((Tar.Player.Health / Tar.Player.MaxHealth * 100) <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);  
                }

            }
        }
    }
}
