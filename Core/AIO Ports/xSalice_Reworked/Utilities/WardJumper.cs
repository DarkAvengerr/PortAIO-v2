using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Utilities
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Base;

    public class WardJumper : SpellBase
    {
        public static int LastPlaced;
        public static Vector3 LastWardPos;
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static void JumpKs(AIHeroClient target)
        {
            foreach (var ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                E.IsReady() && Q.IsReady() && ward.Name.ToLower().Contains("ward") &&
                ward.Distance(target.ServerPosition) < Q.Range && ward.Distance(Player.Position) < E.Range))
            {
                E.Cast(ward);
                return;
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero =>
                E.IsReady() && Q.IsReady() && hero.Distance(target.ServerPosition) < Q.Range &&
                hero.Distance(Player.Position) < E.Range && hero.IsValidTarget(E.Range)))
            {
                E.Cast(hero);
                return;
            }

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                E.IsReady() && Q.IsReady() && minion.Distance(target.ServerPosition) < Q.Range &&
                minion.Distance(Player.Position) < E.Range && minion.IsValidTarget(E.Range)))
            {
                E.Cast(minion);
                return;
            }

            if (Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(target);
                return;
            }

            if (Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(target);
            }
        }

        public static void WardJump()
        {
            foreach (
                var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (E.IsReady())
                {
                    E.Cast(hero);
                    return;
                }
            }

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (E.IsReady())
                {
                    E.Cast(minion);
                    return;
                }
            }
        }
    }
}
