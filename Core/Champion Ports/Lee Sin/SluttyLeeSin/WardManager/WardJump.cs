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
 namespace Lee_Sin.WardManager
{
    class WardJump : LeeSin
    {
        private static float _lastWardJumpTime;
        public static float LastWardCreated;
        public static Vector3 WardCastPosition;

        public static bool WardCastable
        {
            get { return Game.Time - _lastWardJumpTime > 0.50 && Items.GetWardSlot().SpellSlot.IsReady(); }
        }

        public static void WardJumped(Vector3 position, bool objectuse, bool use = true)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (WardCastable && Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" && Player.Spellbook.GetSpell(SpellSlot.W).IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, position);
                LastWardCreated = Game.Time;
                _lastWardJumpTime = Game.Time;
                WardCastPosition = position;
            }
            var ward =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(o => o.IsAlly && o.Name.ToLower().Contains("ward") && (o.Distance(position) < 200))
                    .ToList()
                    .FirstOrDefault();

            if (ward != null)
            {
                W.Cast(ward);
                LeeSin.Lastcastedw = Environment.TickCount;
            }
        }

        /*
        public static void WardJumped(Vector3 position, bool objectuse, bool use = true)
        {
            var objects =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x =>
                            x.IsValid && x.Distance(position) < 200 && x.IsAlly && !x.IsDead &&
                            !x.Name.ToLower().Contains("turret"));

            var ward = WardSorter.Wards();

            if (objectuse)
            {
                if (objects == null)
                {
                    if (W.IsReady() && ward != null && Environment.TickCount - Lastwcasted > 200 && W1() && !use)
                    {
                        Items.UseItem(ward.Id, position);
                    }
                    if (W.IsReady() && ward != null && W1() && use && Environment.TickCount - Lastcastedw > 200)
                    {
                        Items.UseItem(ward.Id, position);
                    }
                }
            }
            else
            {
                if (W.IsReady() && ward != null && Environment.TickCount - Lastwcasted > 200 && W1() && !use)
                {
                    Items.UseItem(ward.Id, position);
                }
                if (W.IsReady() && ward != null && W1() && use && Environment.TickCount - Lastcastedw > 200)
                {
                    Items.UseItem(ward.Id, position);
                }
            }

            if (!objectuse) return;
            foreach ( var wards in ObjectManager.Get<Obj_AI_Base>().Where(wards => W.IsReady() && W1() && !W2() && (objects != null)))
            {
                W.Cast(objects);
                LeeSin.Lastcastedw = Environment.TickCount;
            }
        }
        */
    }
}