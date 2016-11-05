using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class Collision
    {
        public static List<CollisionArgs> CollisionObjects = new List<CollisionArgs>();

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                if (sender.IsValid && sender.IsEnemy && args.SData.Name == "YasuoWMovingWall" &&
                    sender.Distance(EntryPoint.Player) < 1500)
                {
                    CollisionObjects.Add(
                        new CollisionArgs
                        {
                            Sender = hero,
                            WallCastTick = (int) (Game.Time * 0x3E8),
                            WallCastedPos = hero.ServerPosition.To2D()
                        });
                }
            }
        }

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            if (EntryPoint.Player.Distance(sender.Position) < 1500 && sender.IsValid &&
                Regex.IsMatch(sender.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase))
            {
                var closestObject =
                    CollisionObjects.Where(o => o.WallGameObject == null)
                        .OrderBy(o => o.WallCastedPos.Distance(sender.Position))
                        .FirstOrDefault();
                if (closestObject != null && closestObject.Sender.IsValid && closestObject.WallCastedPos.IsValid())
                {
                    closestObject.WallGameObject = sender;
                }
            }
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            if (EntryPoint.Player.Distance(sender.Position) < 1500 && sender.IsValid &&
                Regex.IsMatch(sender.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase))
            {
                var closestObject =
                    CollisionObjects.FirstOrDefault(o => o.WallGameObject != null && o.WallGameObject == sender);
                if (closestObject != null && closestObject.WallGameObject != null)
                {
                    CollisionObjects.Remove(closestObject);
                }
            }
        }

        public static bool DetectCollision(GameObject gameObject)
        {
            return (from collisionArgs in CollisionObjects
                where collisionArgs != null
                let collisionObject = collisionArgs.WallGameObject
                where collisionObject != null
                let level = Convert.ToInt32(collisionObject.Name.Substring(collisionObject.Name.Length - 6, 1))
                let wallWidth = (300 + 50 * level)
                let wallDirection =
                    (collisionObject.Position.To2D() - collisionArgs.WallCastedPos).Normalized().Perpendicular()
                let wallStart = collisionObject.Position.To2D() + (wallWidth / 2f) * wallDirection
                let wallEnd = wallStart - wallWidth * wallDirection
                select
                    !wallStart.Intersection(wallEnd, EntryPoint.Player.Position.To2D(), gameObject.Position.To2D())
                        .Point.IsValid() ||
                    !((int) (Game.Time * 0x3E8) + Game.Ping + Mechanics.Spells[SpellSlot.R].Delay -
                      collisionArgs.WallCastTick < 4000)).Any(flag => flag);
        }

        public class CollisionArgs : IDisposable
        {
            public AIHeroClient Sender;
            public Vector2 WallCastedPos;
            public int WallCastTick;
            public GameObject WallGameObject;

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool preGc)
            {
                if (preGc)
                {
                    Sender = null;
                    WallCastedPos = Vector2.Zero;
                    WallCastTick = 0;
                    WallGameObject = null;
                    GC.SuppressFinalize(this);
                }
            }

            ~CollisionArgs()
            {
                Dispose(false);
            }
        }
    }
}