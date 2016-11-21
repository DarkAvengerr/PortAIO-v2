using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Defensives
{
    class _3401 : CoreItem
    {
        internal override int Id => 3401;
        internal override int Priority => 5;
        internal override string Name => "Mountain";
        internal override string DisplayName => "Face of the Mountain";
        internal override int Duration => 250;
        internal override float Range => 700f;
        internal override int DefaultHP => 35;
        internal override int DefaultMP => 0;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.Zhonyas, MenuType.SelfMuchHP };
        internal override MapType[] Maps => new[] { MapType.Common };

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseItem(hero.Player);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseItem(hero.Player);

                    if (hero.Player.Health/hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.TowerDamage > 0  || hero.IncomeDamage > 0 || 
                            hero.MinionDamage > hero.Player.Health)
                            UseItem(hero.Player);
                    }

                    if (ShouldUseOnMany(hero))
                        UseItem(hero.Player);
                }
            }
        }
    }
}
