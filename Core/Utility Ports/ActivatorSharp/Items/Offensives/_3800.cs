using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _3800 : CoreItem
    {
        internal override int Id => 3800;
        internal override int Priority => 5;
        internal override string Name => "Righteous";
        internal override string DisplayName => "Righteous Glory";
        internal override int Duration => 1000;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Health / Player.MaxHealth * 100 <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp >= 3000 || Player.CountEnemiesInRange(Range) >= 1)
                {
                    UseItem();
                }
            }

            if (Tar != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                {
                    return;
                }

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp >= 3000)
                    {
                        UseItem();
                    }
                    else if (Player.CountEnemiesInRange(Range) >= 1)
                    {
                        UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    }
                }
            }
        }
    }
}
