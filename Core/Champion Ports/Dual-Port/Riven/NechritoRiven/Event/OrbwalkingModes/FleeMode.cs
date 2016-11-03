using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using System.Linq;

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class FleeMode : Core
    {
        #region Public Methods and Operators

        public static void Flee()
        {
            if (MenuConfig.WallFlee && ObjectManager.Player.CountEnemiesInRange(1500) == 0)
            {
                var end = Player.ServerPosition.Extend(Game.CursorPos, 350);
                var isWallDash = FleeLogic.IsWallDash(end, 350);

                var eend = Player.ServerPosition.Extend(Game.CursorPos, 350);
                var wallE = FleeLogic.GetFirstWallPoint(Player.ServerPosition, eend);
                var wallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

                Player.GetPath(wallPoint);

                if (Spells.Q.IsReady() && Qstack < 3)
                {
                    Spells.Q.Cast(Game.CursorPos);
                }

                if (Qstack != 3 || !isWallDash) return;

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);

                if (Spells.E.IsReady() && wallPoint.Distance(Player.ServerPosition) <= Spells.E.Range)
                {
                    Spells.E.Cast(wallE);

                    LeagueSharp.Common.Utility.DelayAction.Add(190, () => Spells.Q.Cast(wallPoint));
                }

                if (wallPoint.Distance(Player.ServerPosition) <= 65)
                {
                    Spells.Q.Cast(wallPoint);
                }
            }
            else
            {
                var enemy = HeroManager.Enemies.Where(target => BackgroundData.InRange(target) && Spells.W.IsReady());

                var x = Player.Position.Extend(Game.CursorPos, 300);

                var targets = enemy as AIHeroClient[] ?? enemy.ToArray();

                if (Spells.W.IsReady() && targets.Any())
                {
                    foreach (var target in targets)
                    {
                        if (BackgroundData.InRange(target))
                        {
                            Spells.W.Cast();
                        }
                    }
                }

                if (Spells.Q.IsReady() && !Player.IsDashing())
                {
                    Spells.Q.Cast(Game.CursorPos);
                }

                if (MenuConfig.FleeYomuu)
                {
                    Usables.CastYoumoo();
                }

                if (Spells.E.IsReady() && !Player.IsDashing())
                {
                    Spells.E.Cast(x);
                }
            }
        }

        #endregion
    }
}
