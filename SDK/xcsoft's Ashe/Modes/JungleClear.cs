using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;

using Settings = xcAshe.Config.Modes.JungleClear;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcAshe.Modes
{
    internal sealed class JungleClear : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.JungleClearActive;
        }

        internal override void Execute()
        {
            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            //if (Settings.UseW && W.IsReady() && GameObjects.Player.ManaPercent > Settings.MinMana)
            //{//큰 정글 몹 부터 W 로 먹기
            //    var target = GameObjects.Jungle.OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(800));
            //    if (target != null)
            //    {
            //        W.Cast(target);
            //    }
            //}
        }
    }
}
