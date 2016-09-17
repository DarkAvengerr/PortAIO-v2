using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;

namespace LittleHumanizer
{
    public static class Program
    {
        public static Menu _menu, _setting;
        public static Random _random;
        public static Dictionary<string, int> _lastCommandT;
        public static int BlockedCount = 0;
        public static bool _thisMovementCommandHasBeenTamperedWith = false;
        public static LastSpellCast LastSpell = new LastSpellCast();
        public static List<LastSpellCast> LastSpellsCast = new List<LastSpellCast>();

        public static int GameTimeTickCount
        {
            get { return (int)(Game.Time * 1000); }
        }

        public static double Randomize(int min, int max)
        {
            var x = _random.Next(min, max) + 1 + 1 - 1 - 1;
            var y = _random.Next(min, max);
            if (_random.Next(0, 1) > 0)
            {
                return x;
            }
            if (1 == 1)
            {
                return (x + y) / 2d;
            }
            return y;
        }

        public static void Main()
        {
            _random = new Random(DateTime.Now.Millisecond);
            _lastCommandT = new Dictionary<string, int>();
            foreach (var order in Enum.GetValues(typeof(GameObjectOrder)))
            {
                _lastCommandT.Add(order.ToString(), 0);
            }
            foreach (var spellslot in Enum.GetValues(typeof(SpellSlot)))
            {
                _lastCommandT.Add("spellcast" + spellslot, 0);
            }

            Player.OnIssueOrder += Player_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs issueOrderEventArgs)
        {
            if (sender.IsMe && !issueOrderEventArgs.IsAttackMove)
            {
                if (issueOrderEventArgs.Order == GameObjectOrder.AttackUnit || issueOrderEventArgs.Order == GameObjectOrder.AttackTo && false)
                    return;
                if (issueOrderEventArgs.Order == GameObjectOrder.MoveTo && false)
                    return;
            }

            var orderName = issueOrderEventArgs.Order.ToString();
            var order = _lastCommandT.FirstOrDefault(e => e.Key == orderName);
            if (Environment.TickCount - order.Value <
                Randomize(
                    1000 / 9,
                    1000 / 6) + _random.Next(-10, 10))
            {
                BlockedCount += 1;
                issueOrderEventArgs.Process = false;
                return;
            }
            if (issueOrderEventArgs.Order == GameObjectOrder.MoveTo &&
                        issueOrderEventArgs.TargetPosition.IsValid() && !_thisMovementCommandHasBeenTamperedWith)
            {
                _thisMovementCommandHasBeenTamperedWith = true;
                issueOrderEventArgs.Process = false;
                Player.IssueOrder(GameObjectOrder.MoveTo,
                    Randomize(issueOrderEventArgs.TargetPosition, -10, 10));
            }
            _thisMovementCommandHasBeenTamperedWith = false;
            _lastCommandT.Remove(orderName);
            _lastCommandT.Add(orderName, Environment.TickCount);
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;
            if (!(new SpellSlot[] {SpellSlot.Q,SpellSlot.W,SpellSlot.E,SpellSlot.R,SpellSlot.Summoner1,SpellSlot.Summoner2
                ,SpellSlot.Item1,SpellSlot.Item2,SpellSlot.Item3,SpellSlot.Item4,SpellSlot.Item5,SpellSlot.Item6,SpellSlot.Trinket})
                .Contains(args.Slot))
                return;
            if (Environment.TickCount - LastSpell.CastTick < 50)
            {
                args.Process = false;
                BlockedCount += 1;
            }
            else
            {
                LastSpell = new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount };
            }
            if (LastSpellsCast.Any(x => x.Slot == args.Slot))
            {
                LastSpellCast spell = LastSpellsCast.FirstOrDefault(x => x.Slot == args.Slot);
                if (spell != null)
                {
                    if (Environment.TickCount - spell.CastTick <= 250 + Game.Ping)
                    {
                        args.Process = false;
                        BlockedCount += 1;
                    }
                    else
                    {
                        LastSpellsCast.RemoveAll(x => x.Slot == args.Slot);
                        LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
                    }
                }
                else
                {
                    LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
                }
            }
            else
            {
                LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
            }
        }

        public static bool IsWall(Vector3 vector)
        {
            return NavMesh.GetCollisionFlags(vector.X, vector.Y).HasFlag(CollisionFlags.Wall);
        }

        public class LastSpellCast
        {
            public int CastTick = 0;
            public SpellSlot Slot = SpellSlot.Unknown;
        }

        public static Vector3 Randomize(Vector3 position, int min, int max)
        {
            var ran = new Random(Environment.TickCount);
            return position + new Vector2(ran.Next(min, max), ran.Next(min, max)).To3D();
        }
    }
}