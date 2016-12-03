using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Items.Defensives
{
    class _3157 : CoreItem
    {
        internal override int Id => 3157;
        internal override int Priority => 7;
        internal override string Name => "Zhonyas";
        internal override string DisplayName => "Zhonya's Hourglass";
        internal override int Duration => 2500;
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.Zhonyas };
        internal override MapType[] Maps => new[] { MapType.SummonersRift, MapType.HowlingAbyss };
        internal override int DefaultHP => 35;  
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value && 
                        hero.IncomeDamage > 0)
                        UseItem();

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>() &&
                        hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseItem();

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>() &&
                        hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseItem();

                    if (ShouldUseOnMany(hero))
                        UseItem();
                }
            }
        }
    }
}
