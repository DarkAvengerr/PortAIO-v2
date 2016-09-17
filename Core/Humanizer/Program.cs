#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

using EloBuddy;
namespace PortAIOHuman
{
    public class Program
    {
        public static Menu Menu;
        public static int LastMove;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Dictionary<SpellSlot, int> LastCast = new Dictionary<SpellSlot, int>();
        public static int BlockedSpellCount;
        public static int BlockedMoveCount;
        public static int NextMovementDelay;
        public static Vector3 LastMovementPosition = Vector3.Zero;

        public static List<SpellSlot> Items = new List<SpellSlot>
        {
            SpellSlot.Item1,
            SpellSlot.Item2,
            SpellSlot.Item3,
            SpellSlot.Item4,
            SpellSlot.Item5,
            SpellSlot.Item6,
            SpellSlot.Trinket
        };

        public static void Game_OnGameLoad()
        {
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var spell = args.Slot;
            var senderValid = sender != null && sender.Owner != null && sender.Owner.IsMe;

            if (!senderValid || !Items.Contains(spell))
            {
                return;
            }

            var min = 50;
            var max = 200;
            var delay = min >= max ? min : WeightedRandom.Next(min, max);

            if (LastCast[spell].TimeSince() < delay)
            {
                BlockedSpellCount++;
                args.Process = false;
                return;
            }

            LastCast[spell] = Utils.TickCount;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            var senderValid = sender != null && sender.IsValid && sender.IsMe;

            if (!senderValid || args.Order != GameObjectOrder.MoveTo)
            {
                return;
            }

            if (LastMovementPosition != Vector3.Zero && args.TargetPosition.Distance(LastMovementPosition) < 300)
            {
                if (NextMovementDelay == 0)
                {
                    var min = 50;
                    var max = 200;
                    NextMovementDelay = min > max ? min : WeightedRandom.Next(min, max);
                }

                if (LastMove.TimeSince() < NextMovementDelay)
                {
                    NextMovementDelay = 0;
                    BlockedMoveCount++;
                    args.Process = false;
                    return;
                }

                var wp = ObjectManager.Player.GetWaypoints();

                if (args.TargetPosition.Distance(Player.ServerPosition) < 50)
                {
                    BlockedMoveCount++;
                    args.Process = false;
                    return;
                }
            }

            LastMovementPosition = args.TargetPosition;
            LastMove = Utils.TickCount;
        }
    }
}