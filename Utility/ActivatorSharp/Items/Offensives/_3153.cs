using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Items.Offensives
{
    class _3153 : CoreItem
    {
        internal override int Id => 3153;
        internal override int Priority => 5;
        internal override string Name => "Botrk";
        internal override string DisplayName => "Blade of the Ruined King";
        internal override int Duration => 100;
        internal override float Range => 550f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP, MenuType.Gapcloser };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 90;
        internal override int DefaultMP => 0;


        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Tar != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                    return;

                if ((Tar.Player.Health / Tar.Player.MaxHealth * 100) <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);
                }

                if ((Player.Health / Player.MaxHealth * 100) <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);
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
                    if (attacker.LSDistance(hero.Player) <= Range / 2f)
                    {
                        UseItem(Tar.Player, true);
                    }
                }

                if (!Menu.Item("enemygapdanger" + Name).GetValue<bool>())
                {
                    if (attacker.LSDistance(hero.Player) <= Range / 2f)
                    {
                        UseItem(Tar.Player, true);
                    }
                }
            }
        }
    }
}
