using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Items.Defensives
{
    class _3107 : CoreItem 
    {
        internal override int Id => 3107;
        internal override int Priority => 5;
        internal override string Name => "Redemption";
        internal override string DisplayName => "Redemption";
        internal override int Duration => 250;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP };
        internal override MapType[] Maps => new[] { MapType.SummonersRift, MapType.HowlingAbyss };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

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
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.TowerDamage > 0 || hero.IncomeDamage > 0 ||
                            hero.MinionDamage > hero.Player.Health)
                            UseItem(Prediction.GetPrediction(hero.Player, 2500f).UnitPosition);
                    }

                    if (ShouldUseOnMany(hero))
                        UseItem(Prediction.GetPrediction(hero.Player, 2500f).UnitPosition);
                }
            }
        }
    }
}
