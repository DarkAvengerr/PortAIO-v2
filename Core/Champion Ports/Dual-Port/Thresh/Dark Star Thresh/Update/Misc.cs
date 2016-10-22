using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Update
{
    using System;
    using System.Linq;

    using Dark_Star_Thresh.Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Misc : Core
    {
        public static void Skinchanger(EventArgs args)
        {
            //Player.SetSkin(Player.CharData.BaseSkinName, MenuConfig.UseSkin ? MenuConfig.Config.Item("Skin").GetValue<StringList>().SelectedIndex : Player.SkinId);
        }

        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MenuConfig.Interrupt || !sender.IsEnemy)
            {
                return;
            }

            if (Spells.E.IsReady() && sender.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(sender);
            }

            else if (sender.IsValidTarget(Spells.Q.Range)
                && Spells.Q.IsReady()
                && args.DangerLevel == Interrupter2.DangerLevel.High 
                && args.EndTime > Utils.TickCount + Spells.Q.Delay + Player.Distance(sender.ServerPosition / Spells.Q.Speed))
            {
                CastQ(sender);
            }
        }

        public static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuConfig.Gapcloser || gapcloser.Sender.IsEnemy) return;

            var sender = gapcloser.Sender;

            var wAlly =
               Player.GetAlliesInRange(Spells.W.Range)
                   .Where(x => !x.IsMe)
                   .FirstOrDefault(x => x.Distance(gapcloser.End) <= Spells.W.Range + 375);

            if (Spells.W.IsReady() && wAlly != null)
            {
                Spells.W.Cast(wAlly.Position);
            }

            if (Spells.E.IsReady() && sender.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(sender);
            }

            else if (Spells.Q.IsReady() && sender.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(gapcloser.End);
            }
        }
    }
}
