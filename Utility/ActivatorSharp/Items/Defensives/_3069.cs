using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Items.Defensives
{
    class _3069 : CoreItem
    {
        internal override int Id => 3069;
        internal override int Priority => 4;
        internal override string Name => "Talisman";
        internal override string DisplayName => "Talisman of Ascension";
        internal override int Duration => 1000;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.Zhonyas };
        internal override MapType[] Maps => new[] { MapType.Common };
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
                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseItem();

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseItem();

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && hero.Attacker != null &&
                            hero.Attacker.Distance(hero.Player.ServerPosition) <= 600)
                            UseItem();    
                    }
                }
            }

            if (Tar != null)
            {
                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem();
                }
            }
        }
    }
}
