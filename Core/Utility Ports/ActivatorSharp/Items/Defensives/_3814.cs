using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Items.Defensives
{
    class _3814 : CoreItem 
    {
        internal override int Id => 3814;
        internal override int Priority => 6;
        internal override string Name => "Edge";
        internal override string DisplayName => "Edge of Night";
        internal override int Duration => 250;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.SelfMuchHP, MenuType.SelfCount, MenuType.ActiveCheck, MenuType.Gapcloser };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 85;
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
                    if (ShouldUseOnMany(hero))
                        UseItem();
                }

                if (Player.CountEnemiesInRange(Range) >= Menu.Item("selfcount" + Name).GetValue<Slider>().Value)
                {
                    UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            AIHeroClient attacker = gapcloser.Sender;

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
                        UseItem();
                    }
                }

                if (!Menu.Item("enemygapdanger" + Name).GetValue<bool>())
                {
                    if (attacker.Distance(hero.Player) <= Range / 2f)
                    {
                        UseItem();
                    }
                }
            }
        }
    }
}
