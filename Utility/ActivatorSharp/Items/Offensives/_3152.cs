using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Items.Offensives
{
    class _3152 : CoreItem
    {
        internal override int Id => 3152;
        internal override int Priority => 5;
        internal override string Name => "Protobelt-01";
        internal override string DisplayName => "Hextech Protobelt-01";
        internal override int Duration => 100;
        internal override float Range => 300f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP };
        internal override MapType[] Maps => new[] { MapType.Common };
        internal override int DefaultHP => 55;
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
                    if (Tar.Player.Distance(Player.ServerPosition) > Range - 100)
                    {
                        if (!Tar.Player.IsFacing(Player) && Player.IsFacing(Tar.Player))
                        {
                            var endpos = Player.ServerPosition.To2D() + Player.Direction.To2D().Perpendicular() * Range;
                            if (endpos.To3D().CountEnemiesInRange(Range + (1 + Player.AttackRange + Player.Distance(Player.BBox.Minimum))) > 0)
                            {
                                UseItem(Tar.Player.ServerPosition, true);
                            }
                        }
                    }
                }

                if ((Player.Health / Player.MaxHealth * 100) <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (!Player.IsFacing(Tar.Player))
                    {
                        var endpos = Player.ServerPosition.To2D() + Player.Direction.To2D().Perpendicular() * Range;
                        if (endpos.To3D().CountEnemiesInRange(Range + (1 + Player.AttackRange + Player.Distance(Player.BBox.Minimum))) <= 1)
                        {
                            UseItem(Game.CursorPos, true);
                        }
                    }
                }
            }
        }
    }
}
