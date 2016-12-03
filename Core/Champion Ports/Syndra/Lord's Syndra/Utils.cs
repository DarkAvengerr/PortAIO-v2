using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LordsSyndra
{

    public static class Utils
    {
        public static Vector2 _yasuoWallCastedPos;
        public static GameObject _yasuoWall;
        public static int _wallCastT;

        public static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (onlyOrbs)
                return OrbManager.GetOrbToGrab((int)Spells.W.Range);
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(Spells.W.Range)))
                return minion.ServerPosition;
            return OrbManager.GetOrbToGrab((int)Spells.W.Range);
        }

        public static bool BuffCheck(Obj_AI_Base enemy)
        {
            var buff = 0;
            if (enemy.HasBuff("UndyingRage") && Program.Menu.Item("DontRbuffUndying").GetValue<bool>()) buff++;
            if (enemy.HasBuff("JudicatorIntervention") && Program.Menu.Item("DontRbuffJudicator").GetValue<bool>()) buff++;
            if (enemy.HasBuff("ZacRebirthReady") && Program.Menu.Item("DontRbuffZac").GetValue<bool>()) buff++;
            if (enemy.HasBuff("AttroxPassiveReady") && Program.Menu.Item("DontRbuffAttrox").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Spell Shield") && Program.Menu.Item("DontRbuffSivir").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Black Shield") && Program.Menu.Item("DontRbuffMorgana").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Chrono Shift") && Program.Menu.Item("DontRbuffZilean").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Ferocious Howl") && Program.Menu.Item("DontRbuffAlistar").GetValue<bool>()) buff++;

            return buff <= 0;
        }

        public static bool DetectCollision(GameObject target)
        {
            if (_yasuoWall == null || !Program.Menu.Item("YasuoWall").GetValue<bool>() ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return true;

            var level = _yasuoWall.Name.Substring(_yasuoWall.Name.Length - 6, 1);
            var wallWidth = (300 + 50 * Convert.ToInt32(level));
            var wallDirection = (_yasuoWall.Position.To2D() - _yasuoWallCastedPos).Normalized().Perpendicular();
            var wallStart = _yasuoWall.Position.To2D() + ((int)(wallWidth / 2)) * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            var intersection = wallStart.Intersection(wallEnd, ObjectManager.Player.Position.To2D(), target.Position.To2D());

            return !intersection.Point.IsValid() || !(Environment.TickCount + Game.Ping + Spells.R.Delay - _wallCastT < 4000);

        }
    }
}
