using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _3092 : CoreItem
    {
        internal override int Id => 3092;
        internal override int Priority => 5;
        internal override string Name => "Queens";
        internal override string DisplayName => "Frost Queen's Claim";
        internal override float Range => 1550f;
        internal override int Duration => 2000;
        internal override int DefaultHP => 55;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
           {
               if (hero.Attacker != null && hero.Attacker.IsValidTarget(900))
               {
                   if (!Parent.Item(Parent.Name + "useon" + hero.Attacker.NetworkId).GetValue<bool>())
                       return;

                   if (hero.Player.Distance(Player.ServerPosition) <= Range)
                   {
                       if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                           Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                       {
                            UseItem();
                       }
                   }
               }
           }

            if (Tar != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                    return;

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }
    }
}
