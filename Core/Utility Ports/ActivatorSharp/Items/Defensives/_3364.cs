using System;
using Activator.Base;
using Activator.Handlers;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Items.Defensives
{
    class _3364 : CoreItem
    {
        internal override int Id => 3364;
        internal override int Priority => 4;
        internal override string Name => "Oracles";
        internal override string DisplayName => "Oracle's Lens";
        internal override int Duration => 250;
        internal override float Range => 600f;
        internal override int DefaultHP => 99;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.SummonersRift };

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.Stealth))
                {
                    UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }
    }
}
