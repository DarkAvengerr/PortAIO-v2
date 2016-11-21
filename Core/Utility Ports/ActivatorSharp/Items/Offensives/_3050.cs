using System;
using System.Linq;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _3050 : CoreItem
    {
        internal override int Id => 3050;
        internal override string Name => "Zekes";
        internal override string DisplayName => "Zeke's Harbringer";
        internal override int Duration => 100;
        internal override int Priority => 5;
        internal override float Range => 1000f;
        internal override MenuType[] Category => new[] { MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 99;
        internal override int DefaultMP => 99;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            var highadhero =
                Activator.Heroes.Where(x => x.Player.IsAlly && !x.Player.IsDead && !x.Player.IsMelee)
                    .OrderByDescending(x => x.Player.FlatPhysicalDamageMod + x.Player.BaseAttackDamage)
                    .FirstOrDefault();

            if (!highadhero.Player.IsMe && highadhero.Player.Distance(Player.ServerPosition) <= Range)
            {
                if (!highadhero.Player.HasBuff("rallyingbanneraurafriend"))
                {
                    UseItem(highadhero.Player, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }
    }
}
