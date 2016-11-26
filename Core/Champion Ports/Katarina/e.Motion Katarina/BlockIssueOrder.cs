using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace e.Motion_Katarina
{
    class BlockIssueOrder
    {
        private static int whenToCancelR = 0;
        public static void InitializeBlockIssueOrder()
        {
            EloBuddy.Player.OnIssueOrder += OnIssueOrder;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if(sender.IsMe && args.Buff.Name == "katarinarsound")
            {
                whenToCancelR = Utils.TickCount + 400;
            }
        }

        private static void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && Logic.Player.HasBuff("katarinarsound") && Utils.TickCount <= whenToCancelR && Config.GetBoolValue("misc.noRCancel"))
                args.Process = false;
        }
    }
}
