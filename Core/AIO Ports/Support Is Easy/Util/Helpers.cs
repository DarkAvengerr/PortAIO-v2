using EloBuddy; namespace Support.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Version = System.Version;

    internal static class Helpers
    {
        public static AIHeroClient AllyBelowHp(int percentHp, float range)
        {
            foreach (var ally in HeroManager.Allies)
            {
                if (ally.IsMe)
                {
                    if (((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
                else if (ally.IsAlly)
                {
                    if (Vector3.Distance(ObjectManager.Player.Position, ally.Position) < range
                        && ((ally.Health / ally.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
            }

            return null;
        }

        public static List<AIHeroClient> AllyInRange(float range)
        {
            return
                HeroManager.Allies.Where(
                    h =>
                    ObjectManager.Player.Distance(h.Position) < range && h.IsAlly && !h.IsMe && h.IsValid && !h.IsDead)
                           .OrderBy(h => ObjectManager.Player.Distance(h.Position))
                           .ToList();
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return ObjectManager.Player.CountEnemiesInRange((int)range) >= numOfEnemy;
        }

        public static void PrintMessage(string message)
        {
            Chat.Print("<font color='#15C3AC'>Support:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        /// <summary>
        ///     ReversePosition
        /// </summary>
        /// <param name="positionMe"></param>
        /// <param name="positionEnemy"></param>
        /// <remarks>Credit to LXMedia1</remarks>
        /// <returns>Vector3</returns>
        public static Vector3 ReversePosition(Vector3 positionMe, Vector3 positionEnemy)
        {
            var x = positionMe.X - positionEnemy.X;
            var y = positionMe.Y - positionEnemy.Y;
            return new Vector3(positionMe.X + x, positionMe.Y + y, positionMe.Z);
        }

        public static void UpdateCheck()
        {
        }
    }
}