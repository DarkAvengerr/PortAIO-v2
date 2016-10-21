using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon.PluginBase;
using SharpDX;
using EloBuddy;

namespace SAutoCarry.Champions.Helpers
{
    public static class Tumble
    {
        private static SCommon.PluginBase.Champion s_Champion;
        private static Vector2 s_NextWallTumbleWalkPos;
        private static int s_LastWallTumbleTick;
        private static Random s_Rnd;
        public static void Initialize(SCommon.PluginBase.Champion champ)
        {
            s_Champion = champ;
            Menu tumble = new Menu("Tumble Settings", "SAutoCarry.Helpers.Tumble.Root");
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.Mode", "Mode").SetValue(new StringList(new[] { "Auto Pos", "Cursor Pos" }, 1)));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.Wall", "Always Tumble to wall if possible").SetTooltip("Tumbles to wall when possible (fastest Q->AA burst)").SetValue(false));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.Only2W", "Tumble only when enemy has 2 w stacks").SetValue(false));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.Only2WHarass", "Tumble only when enemy has 2 w stacks in harass mode").SetValue(true));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.DontQIntoEnemy", "Dont Q Into Enemies").SetTooltip("if this option enabled, assembly wont go in AA range with Tumble").SetValue(false));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.DontSafeCheck", "Dont check tumble position is safe").SetTooltip("if this option enabled, assembly never checks if the tumble position is safe").SetValue(false));
            tumble.AddItem(new MenuItem("SAutoCarry.Helpers.Tumble.Root.WallTumble", "Wall Tumble").SetValue(new KeyBind('Y', KeyBindType.Press)));
            s_Champion.ConfigMenu.AddSubMenu(tumble);
            s_Rnd = new Random();
            Game.OnUpdate += Game_OnUpdate;
        }
        
        public static Vector3 FindTumblePosition(AIHeroClient target)
        {
            if ((Only2W || (s_Champion.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed && Only2WHarass)) && target.GetBuffCount("vaynesilvereddebuff") == 1) // == 1 cuz calling this after attack which is aa missile still flying
                return Vector3.Zero;

            if(WallIfPossible)
            {
                var outRadius = ObjectManager.Player.BoundingRadius / (float)Math.Cos(2 * Math.PI / 8);

                for (var i = 1; i <= 8; i++)
                {
                    var angle = i * 2 * Math.PI / 8;
                    float x = ObjectManager.Player.Position.X + outRadius * (float)Math.Cos(angle);
                    float y = ObjectManager.Player.Position.Y + outRadius * (float)Math.Sin(angle);
                    var colFlags = NavMesh.GetCollisionFlags(x, y);
                    if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                        return new Vector3(x, y, 0);
                }
            }

            if (Mode == 0)
            {
                Vector3 vec = target.ServerPosition;

                if (target.Path.Length > 0)
                {
                    if (ObjectManager.Player.Distance(vec) < ObjectManager.Player.Distance(target.Path.Last()))
                        return IsSafe(target, Game.CursorPos);
                    else
                        return IsSafe(target, Game.CursorPos.To2D().Rotated(Geometry.DegreeToRadian((vec - ObjectManager.Player.ServerPosition).To2D().AngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()) % 90)).To3D());
                }
                else
                {
                    if (target.IsMelee)
                        return IsSafe(target, Game.CursorPos);
                }

                return IsSafe(target, ObjectManager.Player.ServerPosition + (target.ServerPosition - ObjectManager.Player.ServerPosition).Normalized().To2D().Rotated(Geometry.DegreeToRadian(90 - (vec - ObjectManager.Player.ServerPosition).To2D().AngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()))).To3D() * 300f);
            }
            else if(Mode == 1)
            {
                return Game.CursorPos;
            }

            return Vector3.Zero;
        }

        public static Vector3 IsSafe(AIHeroClient target, Vector3 vec, bool checkTarget = true)
        {
            if (DontSafeCheck)
                return vec;

            if (checkTarget)
            {
                if (target.ServerPosition.To2D().Distance(vec) <= target.AttackRange)
                {
                    if (vec.CountEnemiesInRange(1000) > 1)
                        return Vector3.Zero;
                    else if (target.ServerPosition.To2D().Distance(vec) <= target.AttackRange / 2f)
                        return SCommon.Maths.Geometry.Deviation(ObjectManager.Player.ServerPosition.To2D(), target.ServerPosition.To2D(), 60).To3D();
                }

                if (((DontQIntoEnemies || target.IsMelee) && HeroManager.Enemies.Any(p => p.ServerPosition.To2D().Distance(vec) <= p.AttackRange + ObjectManager.Player.BoundingRadius + (p.IsMelee ? 100 : 0))) || vec.UnderTurret(true))
                    return Vector3.Zero;
            }
            if (HeroManager.Enemies.Any(p => p.NetworkId != target.NetworkId && p.ServerPosition.To2D().Distance(vec) <= p.AttackRange + (p.IsMelee ? 50 : 0)) || vec.UnderTurret(true))
                return Vector3.Zero;

            return vec;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (args == null || ObjectManager.Player.IsDead)
                return;

            if (WallTumble)
            {
                if (Game.MapId == GameMapId.SummonersRift)
                {
                    //asuna's walltumble positions
                    Vector2 drakeWallQPos = new Vector2(11514, 4462);
                    Vector2 drakeWallMovePos = new Vector2(12050, 4827);
                    Vector2 midWallQPos = new Vector2(6667, 8794);
                    Vector2 midWallMovePos = new Vector2(6962, 8952);

                    if (ObjectManager.Player.Distance(midWallQPos) >= ObjectManager.Player.Distance(drakeWallQPos))
                        MoveAndWallTumble(drakeWallMovePos, drakeWallQPos);
                    else
                        MoveAndWallTumble(midWallMovePos, midWallQPos);
                }
            }
        }

        public static void MoveAndWallTumble(Vector2 pos, Vector2 pos2)
        {
            if (Utils.TickCount - s_LastWallTumbleTick < 50)
                return;
            s_LastWallTumbleTick = Utils.TickCount;
            if (ObjectManager.Player.ServerPosition.To2D().Distance(pos) > 1500 || !s_Champion.Spells[SCommon.PluginBase.Champion.Q].IsReady())
                return;

            //if near at tumble pos
            if(ObjectManager.Player.ServerPosition.To2D().Distance(pos) <= 10f)
            {
                if (s_NextWallTumbleWalkPos != pos)
                {
                    s_NextWallTumbleWalkPos = pos;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos.To3D());
                }
                //asuna's delay (too lazy to check myself :( ) 
                LeagueSharp.Common.Utility.DelayAction.Add((int)(200f + Game.Ping / 2f), () =>
                {
                    s_Champion.Spells[0].Cast(pos2 - new Vector2(s_Rnd.NextFloat(0, s_Rnd.NextFloat(1, 2.01f)), s_Rnd.NextFloat(0, s_Rnd.NextFloat(1, 2.01f)))); //do wall tumble with randomized pos
                });
                return;
            }
            if(!ObjectManager.Player.IsMoving)
            {
                s_NextWallTumbleWalkPos = ObjectManager.Player.ServerPosition.To2D() + (pos - ObjectManager.Player.ServerPosition.To2D()).Normalized() * 200;
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, s_NextWallTumbleWalkPos.To3D());
            }
            else
            {
                if(ObjectManager.Player.ServerPosition.To2D().Distance(s_NextWallTumbleWalkPos) <= 50)
                {
                    s_NextWallTumbleWalkPos = ObjectManager.Player.ServerPosition.To2D() + (pos - ObjectManager.Player.ServerPosition.To2D()).Normalized() * 200;
                    Vector2 a = ObjectManager.Player.ServerPosition.To2D();
                    Vector2 b = s_NextWallTumbleWalkPos;
                    Vector2 c = pos;
                    float d = a.Distance(c) + c.Distance(b) - a.Distance(b);
                    if (-float.Epsilon < d && float.Epsilon > d)
                        s_NextWallTumbleWalkPos = pos;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, s_NextWallTumbleWalkPos.To3D());
                }
            }
        }

        public static int Mode
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.Mode").GetValue<StringList>().SelectedIndex; }
        }

        public static bool WallIfPossible
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.Wall").GetValue<bool>(); }
        }

        public static bool Only2W
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.Only2W").GetValue<bool>(); }
        }

        public static bool Only2WHarass
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.Only2WHarass").GetValue<bool>(); }
        }

        public static bool DontQIntoEnemies
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.DontQIntoEnemy").GetValue<bool>(); }
        }

        public static bool DontSafeCheck
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.DontSafeCheck").GetValue<bool>(); }
        }

        public static bool WallTumble
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Tumble.Root.WallTumble").GetValue<KeyBind>().Active; }
        }
    }
}
