using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    internal class GravesGapCloser : Graves
    {
        internal static void Init(object sender, Events.GapCloserEventArgs args)
        {
            if (args.IsDirectedToPlayer)
            {
                if (Menu["W"]["Gap"].GetValue<MenuBool>() && W.IsReady() && args.Sender.IsValidTarget(W.Range) && args.End.DistanceToPlayer() <= 200)
                {
                    W.Cast(args.End);
                }
            }
        }
    }
}