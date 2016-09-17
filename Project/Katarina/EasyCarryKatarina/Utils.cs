#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EasyCarryKatarina
{
    public static class Utils
    {
        private static int _wallCastT;
        private static Vector2 _yasuoWallCastedPos;

        public static void Log(string m)
        {
            var r = string.Format("[ECS] - {0}", m);
            Console.WriteLine(r);
        }

        public static bool CanKill(this Spell spell, Obj_AI_Base target)
        {
            return spell.CanCast(target) && spell.IsKillable(target);
        }

        public static bool ProcQ(Obj_AI_Base target)
        {
            return target.HasBuff("KatarinaQMark") || (!QinAir() && !Program.spells[Program.Spells.Q].IsReady());
        }

        public static List<string> Jumpobjects = new List<string>
        {
            "ward", "minion", "champion"
        };

        public static bool CanJumpTo(GameObject obj)
        {
            return Jumpobjects.Any(y => y.Contains(obj.Name.ToLower()));
        }
        
        public static bool RHeroBlock()
        {
            
            return HeroManager.Enemies.Any(y => y.Distance(Program.Player) <= 550 && y.IsValidTarget());
        }

        //Credits to Mantas :)
        public static bool QinAir()
        {
            return ObjectManager.Get<MissileClient>().Any(missile =>missile.SData.Name == "KatarinaQ" && missile.SpellCaster.IsMe);
        }

        //#endregion

        //Credits to Kortatu for Evade Yasuo WW detection
        #region Windwall 
        public static bool CheckLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return a.Intersection(b, c, d).Intersects;
        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid || sender.Team == Program.Player.Team || args.SData.Name != "YasuoWMovingWall") return;
            _wallCastT = Environment.TickCount;
            _yasuoWallCastedPos = sender.ServerPosition.To2D();
        }

        public static bool GetQCollision(AIHeroClient target)
        {
            var from = Program.Player.ServerPosition.To2D();
            var to = target.ServerPosition.To2D();
            if (
                !ObjectManager.Get<AIHeroClient>()
                    .Any(
                        hero =>
                            hero.IsValidTarget(float.MaxValue, false) &&
                            hero.Team == ObjectManager.Player.Team && hero.ChampionName == "Yasuo"))
            {
                return false;
            }

            GameObject wall = null;

            foreach (var gameObject in ObjectManager.Get<GameObject>().Where(gameObject => gameObject.IsValid && Regex.IsMatch(gameObject.Name, "_w_windwall.\\.troy",RegexOptions.IgnoreCase)))
            {
                wall = gameObject;
            }

            if (wall == null) return false;

            var level = wall.Name.Substring(wall.Name.Length - 6, 1);
            var wallWidth = (300 + 50*Convert.ToInt32(level));


            var wallDirection = (wall.Position.To2D() - _yasuoWallCastedPos).Normalized().Perpendicular();
            var wallStart = wall.Position.To2D() + wallWidth/2*wallDirection;
            var wallEnd = wallStart - wallWidth*wallDirection;

            Drawing.DrawLine(wallStart, wallEnd, 50, Color.Blue);

            return (Environment.TickCount - _wallCastT < 4000 && CheckLineIntersection(wallStart, wallEnd, from, to));
        }

        #endregion
    }
}