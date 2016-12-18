// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Modes.cs" company="hsCamera">
//      Copyright (c) hsCamera. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hsCamera.Handlers
{
    internal class Modes : Program
    {
        public static float IsRanged()
        {
            if (ObjectManager.Player.IsRanged)
                return ObjectManager.Player.AttackRange;
            return 330f;
        }

        public static void EnemyTracker()
        {
            foreach (var enemy in HeroManager.Enemies.OrderBy(x => x.Distance(ObjectManager.Player.Position))
                .Where(x => x.IsValidTarget(IsRanged())))
                CameraMovement.SemiDynamic(ObjectManager.Player.Position.Extend(enemy.Position, IsRanged()));
            CameraMovement.SemiDynamic(ObjectManager.Player.Position.Extend(Game.CursorPos, IsRanged()));
        }

        public static void FarmTracker()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position,
                    IsRanged(),
                    MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health)
                .OrderBy(x => x.Distance(ObjectManager.Player.Position));

            foreach (var minion in minions)
                CameraMovement.SemiDynamic(minion.Position);
            CameraMovement.SemiDynamic(ObjectManager.Player.Position.Extend(Game.CursorPos, IsRanged()));
        }

        public static void FollowCursor()
        {
            if (_config.Item("dynamicmode").GetValue<StringList>().SelectedIndex == 1)
                CameraMovement.SemiDynamic(ObjectManager.Player.Position.Extend(Game.CursorPos,
                    _config.Item("followoffset").GetValue<Slider>().Value));
        }

        public static void Normal()
        {
            CameraMovement.SemiDynamic(ObjectManager.Player.Position);
        }
    }
}