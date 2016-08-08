using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = Soraka_HealBot.Config;

    public class AutoESpell
    {
        #region Fields

        public string ChampionName;

        public string SDataName;

        #endregion

        #region Constructors and Destructors

        public AutoESpell(string championName, string sDataName)
        {
            this.ChampionName = championName;
            this.SDataName = sDataName;
        }

        #endregion
    }

    internal static class NerfEverything
    {
        #region Static Fields

        internal static List<AutoESpell> NonMoving = new List<AutoESpell>()
                                                         {
                                                             new AutoESpell("Katarina", "KatarinaR"), 
                                                             new AutoESpell("Nunu", "AbsoluteZero"), 
                                                             new AutoESpell("Fiddlesticks", "Drain"), 
                                                             new AutoESpell("Lucian", "LucianR"), 
                                                             new AutoESpell("Varus", "VarusQ"), 
                                                             new AutoESpell("MissFortune", "MissFortuneBulletTime")
                                                         };

        internal static List<AutoESpell> NonTargetMoving = new List<AutoESpell>() { };

        internal static List<AutoESpell> TargettedMoving = new List<AutoESpell>()
                                                               {
                                                                   new AutoESpell("Katarina", "KatarinaE"), 
                                                                   new AutoESpell("Warwick", "InfiniteDuress"), 
                                                                   new AutoESpell("Pantheon", "PantheonW")
                                                               };

        internal static List<GameObject> ThreshLanterns = new List<GameObject>();

        #endregion

        #region Public Methods and Operators

        public static void Interrupts(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || !Spells.E.LSIsReady())
            {
                return;
            }

            if (NonMoving.Any(x => x.SDataName == args.SData.Name) && Config.IsChecked("autoe." + args.SData.Name))
            {
                if (sender.LSDistance(ObjectManager.Player) <= Spells.E.Range)
                {
                    if (Config.IsChecked("autoe.humanize"))
                    {
                        var delay = OtherUtils.RDelay.Next(
                            Config.GetSliderValue("autoe.lowerhuman"), 
                            Config.GetSliderValue("autoe.upperhuman"));
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(sender));
                        return;
                    }

                    Spells.E.Cast(sender);
                }
            }

            if (TargettedMoving.Any(x => x.SDataName == args.SData.Name) && Config.IsChecked("autoe." + args.SData.Name))
            {
                if (ObjectManager.Player.LSDistance(args.Target.Position) <= Spells.E.Range)
                {
                    if (Config.IsChecked("autoe.humanize"))
                    {
                        var delay = OtherUtils.RDelay.Next(
                            Config.GetSliderValue("autoe.lowerhuman"), 
                            Config.GetSliderValue("autoe.upperhuman"));
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(args.Target.Position));
                        return;
                    }

                    Spells.E.Cast(args.Target.Position);
                }
            }

            if (NonTargetMoving.Any(x => x.SDataName == args.SData.Name) && Config.IsChecked("autoe." + args.SData.Name))
            {
                if (ObjectManager.Player.LSDistance(args.End) <= Spells.E.Range)
                {
                    if (Config.IsChecked("autoe.humanize"))
                    {
                        var delay = OtherUtils.RDelay.Next(
                            Config.GetSliderValue("autoe.lowerhuman"), 
                            Config.GetSliderValue("autoe.upperhuman"));
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(args.End));
                        return;
                    }

                    Spells.E.Cast(args.End);
                }
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (!Spells.E.LSIsReady())
            {
                return;
            }

            if (Enumerable.Any(ThreshLanterns))
            {
                var lantern = Enumerable.FirstOrDefault(
                    ThreshLanterns, 
                    x =>
                    x.Position.LSDistance(ObjectManager.Player.Position) < Spells.E.Range
                    && x.Position.LSCountEnemiesInRange(Spells.E.Width) > 0);
                if (lantern != null)
                {
                    Spells.E.Cast(lantern.Position);
                }
            }
        }

        #endregion
    }
}