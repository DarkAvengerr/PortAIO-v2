using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Items.Cleansers
{
    class _3222 : CoreItem
    {
        internal override int Id => 3222;
        internal override string Name => "Mikaels";
        internal override string DisplayName => "Mikael's Crucible";
        internal override int Priority => 7;
        internal override int Duration => 1000;
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.Cleanse, MenuType.ActiveCheck  };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 5;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.ForceQSS)
                {
                    UseItem(hero.Player);
                    hero.MikaelsBuffCount = 0;
                    hero.MikaelsHighestBuffTime = 0;
                }

                Helpers.CheckMikaels(hero.Player);

                if (hero.MikaelsBuffCount >= Menu.Item("use" + Name + "number").GetValue<Slider>().Value &&
                    hero.MikaelsHighestBuffTime >= Menu.Item("use" + Name + "time").GetValue<Slider>().Value)
                {
                    if (!Menu.Item("use" + Name + "od").GetValue<bool>())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + Menu.Item("use" + Name + "delay").GetValue<Slider>().Value, delegate
                            {
                                UseItem(hero.Player, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                                hero.MikaelsBuffCount = 0;
                                hero.MikaelsHighestBuffTime = 0;
                            });
                    }
                }

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0)
                    {
                        UseItem(hero.Player, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                        hero.MikaelsBuffCount = 0;
                        hero.MikaelsHighestBuffTime = 0;
                    }
                }
            }        
        }
    }
}
