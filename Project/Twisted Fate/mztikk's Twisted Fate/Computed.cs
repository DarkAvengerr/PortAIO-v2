using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Computed
    {
        #region Public Methods and Operators

        public static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is AIHeroClient)
            {
                args.Process = CardSelector.Status != SelectStatus.Selecting
                               && Environment.TickCount - CardSelector.LastWSent > 300;
            }

            if (CardSelector.Status == SelectStatus.Selecting
                && ((Config.IsChecked("disableAAselectingC")
                     && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    || (Config.IsChecked("disableAAselectingH")
                        && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    || (Config.IsChecked("disableAAselectingLC")
                        && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    || (Config.IsChecked("disableAAselectingJC")
                        && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)))
            {
                args.Process = false;
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Gate" && Config.IsChecked("AutoYAG"))
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }

            if (!sender.IsMe)
            {
            }
        }

        public static void SafeCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
        }

        public static void YellowIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name.ToLower() != "goldcardpreattack" || !Spells.Q.IsReady())
            {
                return;
            }

            var qTarget = args.Target as Obj_AI_Base;
            if (qTarget == null || !qTarget.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            if (Config.IsChecked("yellowIntoQ")
                && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && Config.IsChecked("useQCombo"))
            {
                var qPred = Spells.Q.GetPrediction(qTarget);
                Spells.Q.Cast(qPred.CastPosition);
            }

            if (Config.IsChecked("autoYellowIntoQ"))
            {
                var qPred = Spells.Q.GetPrediction(qTarget);
                Spells.Q.Cast(qPred.CastPosition);
            }
        }

        #endregion
    }
}