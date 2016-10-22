using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class CastOnDash
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_AurelionSol.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_AurelionSol.Q;
            }
        }
        #endregion

        static CastOnDash()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.AutoQOnDashPosition", "Auto Use Q On Target Dash End Position").SetValue(true));
            
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.AutoQOnDashPosition").GetValue<bool>() || !sender.IsEnemy || sender == null) return;

            if (sender.NetworkId == target.NetworkId)
            {

                if (Q.IsReady() && Player.Distance(sender) < Q.Range && Player.Mana >= Q.ManaCost)
                {
                    var delay = (int)(args.EndTick - Game.Time - Q.Delay - 0.1f);
                    if (delay > 0)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                    }
                    else
                    {
                        Q.Cast(args.EndPos);
                    }
                }
            }
        }
        #endregion
    }
}
