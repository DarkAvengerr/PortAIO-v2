using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Events
    {
        #region Public Methods and Operators

        public static void OnInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!sender.IsEnemy || ObjectManager.Player.IsRecalling() || !Config.IsChecked("useInterrupt"))
            {
                return;
            }

            var wanteDangerLevel = Interrupter2.DangerLevel.High;
            switch (Config.GetStringListValue("dangerL"))
            {
                case 0:
                    wanteDangerLevel = Interrupter2.DangerLevel.Low;
                    break;
                case 1:
                    wanteDangerLevel = Interrupter2.DangerLevel.Medium;
                    break;
                case 2:
                    wanteDangerLevel = Interrupter2.DangerLevel.High;
                    break;
                default:
                    wanteDangerLevel = Interrupter2.DangerLevel.High;
                    break;
            }

            if (Spells.R.IsReady() && sender.IsValidTarget(Spells.R.Range) && args.DangerLevel == wanteDangerLevel)
            {
                Spells.R.Cast();
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "XenZhaoComboTarget")
            {
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        #endregion
    }
}