using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Lee_Sin.Core
{
    class WardJump
    {
        private static float _lastWardJumpTime;
        public static float LastWardCreated;
        public static Vector3 WardCastPosition;

        public static bool WardCastable 
        {
            get { return Game.Time - _lastWardJumpTime > 0.50 && Items.GetWardSlot().SpellSlot.IsReady(); }
        }

        public static void HikiJump(Vector3 position)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (WardCastable && Spells.W.Instance.Name == "BlindMonkWOne" && Spells.W.IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, position);
                LastWardCreated = Game.Time;
                _lastWardJumpTime = Game.Time;
                WardCastPosition = position;
            }
            var ward = ObjectManager.Get<Obj_AI_Base>()
                    .OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition))
                    .FirstOrDefault(
                        obj =>
                            obj.IsAlly && !obj.IsMe && obj.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                            (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                             Vector3.DistanceSquared(Game.CursorPos, obj.ServerPosition) <= 150 * 150));
            if (ward != null)
            {
                Spells.W.CastOnUnit(ward);
            }
        }
    }
}
