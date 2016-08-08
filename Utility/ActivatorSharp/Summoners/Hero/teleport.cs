using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace Activator.Summoners
{
    class teleport : CoreSum
    {
        internal override string Name => "summonerteleport";
        internal override string DisplayName => "Teleport";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 3500;

        private static int _lastPing;
        private static Random _rand => new Random();
        private static Vector3 _lastPingLocation;

        // ping credits to Honda :^)
        private static void Ping(Vector3 pos, bool sound = false)
        {
            if (Utils.GameTimeTickCount - _lastPing < 5000)
            {
                return;
            }

            _lastPing = Utils.GameTimeTickCount;
            _lastPingLocation = pos;

            SimplePing(sound);
            LeagueSharp.Common.Utility.DelayAction.Add(109 + _rand.Next(90, 300), () => SimplePing(sound));
        }

        private static void SimplePing(bool sound = false)
        {   
            TacticalMap.ShowPing(PingCategory.Fallback, _lastPingLocation, sound);
        }

        public override void OnTick(EventArgs args)
        {
            if (!IsReady())
            {
                return;
            }

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.IsDead || !hero.Player.IsValid || hero.Player.IsZombie)
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) > 3000 && hero.Player.LSDistance(Game.CursorPos) > 3000)
                {
                    if (hero.HitTypes.Contains(HitType.Ultimate) && Menu.Item("teleulthp2").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0)
                        {
                            Ping(hero.Player.ServerPosition, Menu.Item("telesound").GetValue<bool>());
                        }
                    }

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <= 35 && Menu.Item("telelowhp2").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0)
                        {
                            Ping(hero.Player.ServerPosition, Menu.Item("telesound").GetValue<bool>());
                        }
                    }
                }
            }
        }
    }
}
