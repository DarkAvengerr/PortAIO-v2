using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Items.Defensives
{
    class _3143 : CoreItem
    {
        internal override int Id => 3143;
        internal override int Priority => 4;
        internal override string Name => "Randuins";
        internal override string DisplayName => "Randuin's Omen";
        internal override int Duration => 250;
        internal override float Range => 500f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfCount, MenuType.Gapcloser  };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.CountEnemiesInRange(Range) >= Menu.Item("selfcount" + Name).GetValue<Slider>().Value)
            {
                UseItem();
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var attacker = gapcloser.Sender;

            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                !Menu.Item("enemygap" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!hero.Player.IsMe)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + attacker.NetworkId).GetValue<bool>())
                    continue;

                if (Menu.Item("enemygapmelee" + Name).GetValue<bool>() && !attacker.IsMelee())
                    continue;

                if (hero.HitTypes.Contains(HitType.Ultimate) || hero.HitTypes.Contains(HitType.Danger))
                {
                    if (attacker.Distance(hero.Player) <= Range / 2f)
                    {
                        UseItem(Tar.Player, true);
                    }
                }

                if (!Menu.Item("enemygapdanger" + Name).GetValue<bool>())
                {
                    if (attacker.Distance(hero.Player) <= Range / 2f)
                    {
                        UseItem(Tar.Player, true);
                    }
                }
            }
        }
    }
}
