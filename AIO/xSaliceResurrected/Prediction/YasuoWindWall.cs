namespace xSaliceResurrected_Rework.Prediction
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System.Linq;
    using EloBuddy;
    public class YasuoWindWall
    {
        private static readonly bool Enable;
        private static readonly YasuoWall YasuoWall = new YasuoWall();

        static YasuoWindWall()
        {
            foreach (var Yasuo in HeroManager.Enemies.Where(x => x.IsValidTarget()).Select(x => x.ChampionName == "Yasuo"))
            {
                if (Yasuo)
                {
                    Enable = true;
                }
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            if (!Enable)
            {
                return;
            }

            if (!sender.IsEnemy || sender.IsMinion || Args.SData.IsAutoAttack() || sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            if (Args.SData.Name == "YasuoWMovingWall")
            {
                YasuoWall.CastTime = Game.Time;
                YasuoWall.CastPosition = sender.Position.Extend(Args.End, 400);
                YasuoWall.YasuoPosition = sender.Position;
                YasuoWall.WallLvl = sender.Spellbook.Spells[1].Level;
            }
        }

        public static bool CollisionYasuo(Vector3 from, Vector3 to)
        {
            if (!Enable)
            {
                return false;
            }

            if (Game.Time - YasuoWall.CastTime > 4)
            {
                return false;
            }

            var level = YasuoWall.WallLvl;
            var wallWidth = (350 + 50 * level);
            var wallDirection = (YasuoWall.CastPosition.To2D() - YasuoWall.YasuoPosition.To2D()).Normalized().Perpendicular();
            var wallStart = YasuoWall.CastPosition.To2D() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            return wallStart.Intersection(wallEnd, to.To2D(), @from.To2D()).Intersects;
        }
    }

    public class YasuoWall
    {
        public Vector3 YasuoPosition { get; set; }

        public float CastTime { get; set; }

        public Vector3 CastPosition { get; set; }

        public float WallLvl { get; set; }

        public YasuoWall()
        {
            CastTime = 0;
        }
    }
}
