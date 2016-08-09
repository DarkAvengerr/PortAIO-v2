using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Soraka_HealBot.Modes;

    internal static class Events
    {
        #region Static Fields

        

        public static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var tar = args.Target as Obj_AI_Minion;

            if (tar != null && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && Config.IsChecked("disableAAH"))
            {
                var air = ObjectManager.Player.CountAlliesInRange(Config.GetSliderValue("allyRangeH"));
                if (air > 0)
                {
                    Chat.Print("disable");
                    args.Process = false;
                }
            }

            var hTar = args.Target as AIHeroClient;
            if (hTar == null)
            {
                return;
            }

            if ((Config.IsChecked("comboDisableAA")
                 || (Config.IsChecked("bLvlDisableAA")
                     && ObjectManager.Player.Level >= Config.GetSliderValue("lvlDisableAA")))
                && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }
        }

        public static void OnObjectCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.Name == "Thresh_Base_Lantern")
            {
                NerfEverything.ThreshLanterns.Add(sender);
            }
        }

        public static void OnObjectDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.Name == "Thresh_Base_Lantern")
            {
                NerfEverything.ThreshLanterns.Remove(sender);
            }
        }

        public static void SavingGrace(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args.Target == null || sender.IsMe)
            {
                return;
            }

            var enemy = sender as AIHeroClient;
            var ally = args.Target as AIHeroClient;
            if (enemy == null || !enemy.IsEnemy || ally == null || !ally.IsAlly)
            {
                return;
            }

            var enemyDmg = enemy.GetSpellDamage(ally, args.Slot);
            if (ally.Health > enemyDmg || ally.Health.Equals(ally.MaxHealth))
            {
                return;
            }

            if (Config.IsChecked("autoW") && Spells.W.IsReady() && Spells.W.Level > 0
                && ally.Distance(ObjectManager.Player) <= Spells.W.Range
                && Config.IsChecked("autoW_" + ally.ChampionName))
            {
                if (ally.Health + Spells.GetWHeal() > enemyDmg && ally.Health <= enemyDmg)
                {
                    Spells.W.Cast(ally);
                }
            }

            if (Config.IsChecked("autoR") && Spells.R.IsReady() && Spells.R.Level > 0
                && Config.IsChecked("autoR_" + ally.ChampionName))
            {
                if (ally.Health + Spells.GetUltHeal(ally) > enemyDmg && ally.Health <= enemyDmg)
                {
                    Spells.R.Cast();
                }
            }
        }

        #endregion
    }
}