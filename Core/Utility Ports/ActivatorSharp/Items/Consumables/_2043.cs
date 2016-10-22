using System;
using Activator.Base;
using Activator.Handlers;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Items.Consumables
{
    class _2043 : CoreItem
    {
        internal override int Id => 2043;
        internal override string Name => "Vision Ward";
        internal override string DisplayName => "Vision Ward";
        internal override int Duration => 101;
        internal override int Priority => 5;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.SummonersRift };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.Stealth))
                {
                    UseItem(hero.Player.ServerPosition, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }
    }
}
