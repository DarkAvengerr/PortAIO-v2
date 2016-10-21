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
 namespace RivenToTheChallenger.Spells
{
    static class BrokenWings
    {
        private static bool waitingQCast = false;
        public static event EventHandler<GameObjectPlayAnimationEventArgs> OnPlayAnimation;
        public static int Stacks { get; private set; }
        public static Spell Q { get; }

        public static int TotalRange
        {
            get
            {
                if (!Q.IsReady(475))
                    return 0;
                var range = -100 + (int) (Q.Range*(3 - Stacks));
                return range < 0 ? 0 : range;
            }
        }
        static BrokenWings()
        {
            Q = new Spell(SpellSlot.Q);
            Q.SetSkillshot(250, 112.5f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Q.Range = 260;
            Stacks = ObjectManager.Player.GetBuffCount("RivenTriCleave"); //First Access
            if (Stacks == -1)
            {
                Stacks = 0;
            }
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnBuffUpdate += OnBuffUpdateCount;
            Obj_AI_Base.OnPlayAnimation += BaseOnOnPlayAnimation;
        }

        public static void Cast(Obj_AI_Base target)
        {
            Q.Cast(target.ServerPosition); // this way it always casts to enemy I think rather than cursosr
            /*  if (waitingQCast) return;

            if (Game.CursorPos.Distance(target.ServerPosition, true) <= target.BoundingRadius * target.BoundingRadius || ObjectManager.Player.IsFacing(target))
            {
                Q.Cast(target);
                return;
            }
            waitingQCast = true;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
            LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping, () =>
            {
                Q.Cast(target);
                waitingQCast = false;
            });*/

        }

        private static void BaseOnOnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (OnPlayAnimation == null || !sender.IsMe)
            {
                return;
            }
            if (args.Animation != "Spell1a" && args.Animation != "Spell1b" && args.Animation != "Spell1c")
            {
                return;
            }
            OnPlayAnimation(sender, args);
        }

        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name != "RivenTriCleave") return;
            Stacks = 1;
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name != "RivenTriCleave") return;
            Stacks = 0;
            Q.Width = WindSlash.HasRBuff ? 162.5f : 112.5f;
        }

        private static void OnBuffUpdateCount(Obj_AI_Base sender, Obj_AI_BaseBuffUpdateEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name != "RivenTriCleave") return;
            ++Stacks;
            if (Stacks == 2)
            {
                Q.Width = WindSlash.HasRBuff ? 200 : 150;
            }
        }
    }
}
