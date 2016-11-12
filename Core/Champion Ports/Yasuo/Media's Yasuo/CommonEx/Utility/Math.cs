using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Utility
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SpellDatabase = LeagueSharp.SDK.SpellDatabase;

    class Math
    {
        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector2.Zero;
            }
            float x = 0, y = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
            }

            return new Vector2(x / units.Count, y / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Vector2> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector2.Zero;
            }

            float x = 0, y = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
            }

            return new Vector2(x / vectors.Count, y / vectors.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector3</returns>
        public static Vector3 GetMeanVector3(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector3.Zero;
            }
            float x = 0, y = 0, z = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
                z += unit.ServerPosition.Z;
            }

            return new Vector3(x / units.Count, y / units.Count, z / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector3 GetMeanVector3(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector3.Zero;
            }

            float x = 0, y = 0, z = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
                z += vector.Z;
            }

            return new Vector3(x / vectors.Count, y / vectors.Count, z / vectors.Count);
        }

        public static float GetPathLenght(Vector3[] path)
        {
            var result = 0f;

            for (var i = 0; i < path.Count(); i++)
            {
                if (i + 1 != path.Count())
                {
                    result += path[i].Distance(path[i + 1]);
                }
            }
            return result;
        }

        internal static float DistanceToTarget(Obj_AI_Base unit)
        {
            return unit.Distance(TargetSelector.GetSelectedTarget());
        }

        /// <summary>
        ///     Returns Missile Costs based on Spell Name
        /// </summary>
        /// <param name="spellName"></param>
        /// <returns>float</returns>
        internal static float GetMissileSpeed(string spellName)
        {
            switch (spellName)
            {
                case "YasuoDash":
                    return 1000 + (ObjectManager.Player.MoveSpeed - 345);
                case "YasuoQ":
                    return Single.MaxValue;
                case "YasuoQ2":
                    return 1400;
                default:
                    return SpellDatabase.GetByName(spellName).MissileSpeed;
            }
        }

        public static float GetMeanDistance(List<Vector3> vectors, Vector3 vector)
        {
            var result = vectors.Sum(v => v.Distance(vector));

            return result / vectors.Count;
        }
    }
}
