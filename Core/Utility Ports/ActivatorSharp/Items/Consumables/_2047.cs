using System;
using Activator.Base;
using Activator.Handlers;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Items.Consumables
{
    class _2047 : CoreItem
    {
        internal override int Id => 2047;
        internal override string Name => "Oracle's Extract";
        internal override string DisplayName => "Oracle's Extract";
        internal override int Duration => 101;
        internal override int Priority => 5;
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.Stealth, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.TwistedTreeline, MapType.CrystalScar, MapType.HowlingAbyss };
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
