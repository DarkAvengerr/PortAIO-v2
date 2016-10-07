using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger.Spells
{
    static class WindSlash
    {
        public static Action OnR1Casted;
        public static Action OnR2Casted;
        public static Spell R1 { get; }
        public static Spell R2 { get; }
        public static bool HasRBuff { get; }

        static WindSlash()
        {
            R1 = new Spell(SpellSlot.R);
            R2 = new Spell(SpellSlot.R, true);
            R2.SetSkillshot(R2.Delay, R2.Width, 1600, false, SkillshotType.SkillshotCone);
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.Slot != SpellSlot.R)
            {
                return;
            }
            bool isR1 = args.Target?.IsMe ?? false;
            if (isR1)
            {
                if (OnR1Casted != null)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((int)R1.Delay - Game.Ping, () => { OnR1Casted(); });
                }
            }
            else
            {
                if (OnR2Casted != null)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((int)R2.Delay - Game.Ping, () => { OnR2Casted(); });
                }
            }
        }


        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name != "RivenFengShuiEngine")    return;
            BrokenWings.Q.Width = BrokenWings.Stacks == 2 ? 200 : 162.5f;
            KiBurst.W.Width = 135;
        }
        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name != "RivenFengShuiEngine")    return;
            BrokenWings.Q.Width = BrokenWings.Stacks == 2 ? 150 : 112.5f;
            KiBurst.W.Width = 125;
        }

    }
}
