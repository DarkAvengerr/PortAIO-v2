#region


//Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\"

using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using System.Linq;
using PortAIO.Dual_Port;
using LeagueSharp.Common;
using System.Net;
using System.Text.RegularExpressions;
// ReSharper disable ObjectCreationAsStatement

#endregion

namespace PortAIO
{
    public static class Init
    {
        public static bool loaded = false;
        public static int moduleNum = 1;

        public static void Initialize()
        {
            Console.WriteLine("[PortAIO] Core loading : Module " + moduleNum + " - Common Loaded");
            moduleNum++;

            Misc.Load();
            Console.WriteLine("[PortAIO] Core loading : Module " + moduleNum + " - Misc Loaded");
            moduleNum++;
            if (!Misc.menu.Item("UtilityOnly").GetValue<bool>())
            {
                LoadChampion();
                Console.WriteLine("[PortAIO] Core loading : Module " + moduleNum + " - Champion Script Loaded");
                moduleNum++;
                Game.OnUpdate += Game_OnUpdate;
                Console.WriteLine("[PortAIO] Core loading : Module " + moduleNum + " - Champion Load Detected, Disabling EB Orbwalker");
                moduleNum++;
            }
            if (!Misc.menu.Item("ChampsOnly").GetValue<bool>())
            {
                LoadUtility();
                Console.WriteLine("[PortAIO] Core loading : Module " + moduleNum + " - Utilities Loaded");
                moduleNum++;
            }

            Console.WriteLine("[PortAIO] Core loaded.");
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            EloBuddy.SDK.Orbwalker.DisableAttacking = true;
            EloBuddy.SDK.Orbwalker.DisableMovement = true;
        }

        public static void PortAIOMsg(string msg)
        {
            Chat.Print("<font color=\"#43ddaa\">[PortAIO] </font><font color=\"#ff9999\">" + msg + "</font>");
        }

        public static void LoadUtility()
        {
            if (Misc.menu.Item("enableEvade").GetValue<bool>())
            {
                switch (Misc.menu.Item("Evade").GetValue<StringList>().SelectedIndex)
                {

                    case 0: // EzEvade - Done
                        ezEvade.Program.Main();
                        break;
                    case 1: // Evade# - Done
                        Evade.Program.Game_OnGameStart();
                        break;
                }
            }

            if (Misc.menu.Item("AutoPlay").GetValue<bool>())
            {
                switch (Misc.menu.Item("selectAutoPlay").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // AramDetFull
                        ARAMDetFull.Program.Main();
                        break;
                    case 1: // AutoJungle
                        AutoJungle.Program.OnGameLoad();
                        break;
                    case 2: // SharpAI
                        SharpAI.Program.Main();
                        break;
                }
            }

            if (Misc.menu.Item("enableTracker").GetValue<bool>())
            {
                switch (Misc.menu.Item("Tracker").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // SFXUtility
                        SFXUtility.Program.Main();
                        break;
                    case 1: // Tracker#
                        Tracker.Program.Main();
                        break;
                    case 2: // NabbTracker
                        NabbTracker.Program.Main();
                        PortAIOMsg("You're running an SDK utility. If you're using a champion port that uses the LeagueSharp-Common Orbwalker, then please disable the L# SDK Orbwalker by going into the menu and unchecking 'Enabled'.");
                        break;
                }
            }

            if (Misc.menu.Item("enableHuman").GetValue<bool>())
            {
                switch (Misc.menu.Item("Humanizer").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // Humanizer#
                        HumanizerSharp.Program.Game_OnGameLoad();
                        break;
                    case 1: // SebbyBanWars
                        Sebby_Ban_War.Program.Game_OnGameLoad();
                        break;
                }
            }

            if (Misc.menu.Item("enablePredictioner").GetValue<bool>())
            {
                switch (Misc.menu.Item("Predictioner").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // SPredictioner
                        SPredictioner.Program.Main();
                        break;
                    case 1: // OKTW Predictioner
                        OKTWPredictioner.Program.Main();
                        break;
                }
            }

            if (Misc.menu.Item("enableActivator").GetValue<bool>())
            {
                switch (Misc.menu.Item("Activator").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // ElUtilitySuite
                        ElUtilitySuite.Entry.OnLoad();
                        break;
                    case 1: // Activator#
                        Activator.Activator.Game_OnGameLoad();
                        break;
                    case 2: // NabbActivator
                        NabbActivator.Program.Main();
                        PortAIOMsg("You're running an SDK utility. If you're using a champion port that uses the LeagueSharp-Common Orbwalker, then please disable the L# SDK Orbwalker by going into the menu and unchecking 'Enabled'.");
                        break;
                }
            }

            if (Misc.menu.Item("ShadowTracker").GetValue<bool>())
            {
                ShadowTracker.Program.Game_OnGameLoad();
            }

            if (Misc.menu.Item("UniversalPings").GetValue<bool>())
            {
                UniversalPings.Program.Main();
            }
        }

        public static void LoadChampion()
        {
            // Support.Program.Main(); - Support is Easy Champions
            // ReformedAIO.Program.Main(); - ReformedAIO Champs
            // KappaSeries.Program.OnGameLoad(); - KappaSeries Champions
            // xSaliceResurrected_Rework.Program.LoadReligion(); - xSalice Champs
            // SurvivorSeries.SurviorSeries.Main(); - SurvivorSeries Champions
            // ADCPackage.Program.Game_OnGameLoad(); - ADCPackage
            // StonedSeriesAIO.Program.Main(); - StonedSeriesAIO - TheKushStyle
            // Sharpy_AIO.Program.Game_OnGameLoad(); - SharpyAIO
            // ProSeries.Program.GameOnOnGameLoad();
            // iSeriesReborn.Program.OnGameLoad();
            // ShineSharp.Program.Game_OnGameLoad();
            // vSupport_Series.Program.Game_OnGameLoad();
            // BadaoSeries.Program.OnLoad();
            // DZAIO_Reborn.Program.Main();
            // FreshBooster.Program.Game_OnGameLoad();
            // hikiMarksmanRework.Program.Game_OnGameLoad();

            switch (ObjectManager.Player.Hero)
            {
                case Champion.Aatrox:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 1: // Kappa Series
                            KappaSeries.Program.OnGameLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // NoobAatrox
                            NoobAatrox.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Ahri:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // DZAhri
                            DZAhri.Program.Game_OnGameLoad();
                            break;
                        case 2: // EloFactory
                            EloFactory_Ahri.Program.Game_OnGameLoad();
                            break;
                        case 3: // Kappa Series
                            KappaSeries.Program.OnGameLoad();
                            break;
                        case 4: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 5: // BadaoSeries
                            BadaoSeries.Program.OnLoad();
                            break;
                        case 6: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 7: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 8: // AhriSharp
                            AhriSharp.Program.Game_OnGameLoad();
                            break;
                        case 9: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 10: // Babehri
                            Babehri.Program.Game_OnGameLoad();
                            break;
                        case 11: // EasyAhri
                            EasyAhri.Program.Main();
                            break;
                        case 12: // SenseAhri
                            Sense_Ahri.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Akali:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // xQx Akali
                            Akali.Program.Game_OnGameLoad();
                            break;
                        case 1: // Kappa Series
                            KappaSeries.Program.OnGameLoad();
                            break;
                        case 2: // Korean Akali
                            KoreanAkali.Program.Game_OnGameLoad();
                            break;
                        case 3: // Troopkali
                            AkaliTroop.Program.Game_OnGameLoad();
                            break;
                        case 4: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 5: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
                            break;
                        case 6: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 7: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 8: // Flowers' Akali
                            Flowers_Akali.Program.Main();
                            break;
                        case 9: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 10: // TroopAIO
                            _SDK_TroopAIO.Program.Main();
                            break;
                        case 11: // Bloodmoon Akali
                            BloodMoonAkali.Program.Game_OnGameLoad();
                            break;
                        case 12: // Royal Rapist Akali
                            RoyalAkali.Program.Game_OnGameLoad();
                            break;
                        case 13: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Alistar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElAlistar
                            ElAlistarReborn.Alistar.OnGameLoad();
                            break;
                        case 1: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 2: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 4: // SkyAlstar
                            AlistarBySky97.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Amumu:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Amumu#
                            AmumuSharp.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
                            break;
                        case 3: // ShineAIO
                            ShineSharp.Program.Game_OnGameLoad();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // DJ Amumu
                            DJAmumu.Program.Game_OnGameLoad();
                            break;
                        case 6: // MasterofSadness
                            MasterOfSadness.Program.Main();
                            break;
                    }
                    break;
                case Champion.Anivia:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // Anivia#
                            AniviaSharp.Program.Main();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Annie:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // Korean Annie
                            KoreanAnnie.Program.Game_OnGameLoad();
                            break;
                        case 2: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 3: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 4: // EloFactory Annie
                            EloFactory_Annie.Program.Game_OnGameLoad();
                            break;
                        case 5: // Flower's Annie
                            Flowers__Annie.Program.Game_OnGameLoad();
                            break;
                        case 6: // OAnnie
                            OAnnie.Program.Main();
                            break;
                    }
                    break;
                case Champion.Ashe:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 2: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                        case 3: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 4: // SurvivorSeries
                            SurvivorAshe.Program.Game_OnGameLoad();
                            break;
                        case 5: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 6: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 7: // [SBTW] Ashe
                            Flowers_Ashe.Program.Main();
                            break;
                        case 8: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 9: // Dicaste's Ashe
                            DicasteAshe.Program.Main();
                            break;
                        case 10: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 11: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 12: // xcsoft's Ashe
                            xcAshe.Program.Main();
                            break;
                        case 13: // Ashe#
                            AsheSharp.Source.Program.Game_OnGameLoad();
                            break;
                        case 14: // CarryAshe
                            CarryAshe.Program.Game_OnGameLoad();
                            break;
                        case 15: // SNAshe
                            SNAshe.Program.Game_OnGameLoad();
                            break;
                        case 16: // ProjectGeass
                            _Project_Geass.Program.Main();
                            break;
                        case 17: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 18: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                        case 19: //SurvivorSeries AIO
                            SurvivorSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.AurelionSol:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElAurelionSol
                            ElAurelion_Sol.AurelionSol.OnGameLoad();
                            break;
                        case 1: // SkyLv_Aurelion
                            SkyLv_AurelionSol.Initialiser.Game_OnGameLoad();
                            break;
                        case 2: // vAurelionSol
                            vAurelionSol.AurelionSol.Game_OnGameLoad();
                            break;
                        case 3: // Aurelion Sol As The Star Forger
                            Aurelion_Sol_As_the_Star_Forger.Program.Main();
                            break;
                        case 4: // Flowers' AurelionSol
                            Flowers__AurelionSol.Program.Game_OnGameLoad();
                            break;
                        case 5: // Badao Aurelion
                            BadaoKingdom.Program.Main();
                            break;
                    }
                    break;
                case Champion.Azir:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // HeavenStrike Azir
                            HeavenStrikeAzir.Program.Game_OnGameLoad();
                            break;
                        case 1: // Creator of Elo
                            Azir_Creator_of_Elo.Program.Main();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 4: // Azir by Kortatu
                            AzirKortatu.Program.Main();
                            break;
                        case 5: // AzirSharp
                            AzirSharp.Program.Main();
                            break;
                        case 6: // Night Stalker Azir
                            Night_Stalker_Azir.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Bard:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // DZBard
                            DZBard.Program.Game_OnGameLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 3: // xBard
                            xBard.Program.Game_OnGameLoad();
                            break;
                        case 4: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 5: // BreakingBard
                            BreakingBard.Program.Main();
                            break;
                        case 6: // DesomondBard
                            DesomondBard.Program.Main();
                            break;
                    }
                    break;
                case Champion.Blitzcrank:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // KurisuBlitz
                            Blitzcrank.Program.Game_OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 5: // ShineAIO
                            ShineSharp.Program.Game_OnGameLoad();
                            break;
                        case 6: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 7: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 8: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 9: // xcsoft's Blitzcrank
                            xcBlitzcrank.Program.Main();
                            break;
                        case 10: // JustBlitzcrank
                            JustBlitz.Program.Main();
                            break;
                        case 11: // MoonBlitz
                            MoonBlitz.Program.Main();
                            break;
                        case 12: // SluttyBlitz
                            Slutty_Blitz.Program.Main();
                            break;
                        case 13: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Brand:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // The Brand
                            TheBrand.Program.Main();
                            break;
                        case 1: // Hikicarry Brand
                            HikiCarry_Brand.Program.Game_OnGameLoad();
                            break;
                        case 2: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 3: // SurvivorSeries
                            SurvivorBrand.Program.Game_OnGameLoad();
                            break;
                        case 4: // yol0 Brand
                            yol0Brand.Program.Game_OnGameLoad();
                            break;
                        case 5: // DevBrand
                            DevBrand.Program.Main();
                            break;
                        case 6: // Flower's Brand
                            Flowers_Brand.Program.Main();
                            break;
                        case 7: // Kimbaeng Brand
                            Kimbaeng_Brand.Program.Main();
                            break;
                        case 8: // sBrand
                            sBrand.Program.Main();
                            break;
                        case 9: // SNBrand
                            SNBrand.Program.Main();
                            break;
                        case 10: //SurvivorSeries AIO
                            SurvivorSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Braum:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // Support Is Easy
                            Support.Program.Main();
                            break;
                    }
                    break;
                case Champion.Caitlyn:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 3: // ChallengerSeriesAIO
                            Challenger_Series.Program.Main();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // Slutty Caitlyn
                            Slutty_Caitlyn.Program.Main();
                            break;
                        case 6: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 7: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                        case 8: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Cassiopeia:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 1: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 2: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 3: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 4: // TheCassiopeia
                            TheCassiopeia.Program.Main();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // Eat my Cass
                            Eat_My_Cass.Program.Main();
                            break;
                        case 7: // mztikks Cass
                            mztikksCassiopeia.Program.Main();
                            break;
                        case 8: // RiseofThePython
                            riseofthepython.Program.Main();
                            break;
                        case 9: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Chogath:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // Windwalker Cho'Gath
                            WindWalker_Cho._._.gath.Program.Game_OnGameLoad();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // Troop Gath
                            TroopChogath.Program.Main();
                            break;
                    }
                    break;
                case Champion.Corki:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElCorki
                            ElCorki.Corki.Game_OnGameLoad();
                            break;
                        case 1: // ADCPackage
                            ADCPackage.Program.Game_OnGameLoad();
                            break;
                        case 2: // D-Corki
                            D_Corki.Program.Game_OnGameLoad();
                            break;
                        case 3: // hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 4: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 5: // ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 6: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 7: // SharpShooter 
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 8: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 9: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 10: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 11: // EasyCorki
                            EasyCorki.EasyCorki.Main();
                            break;
                        case 12: // jhkCorki
                            jhkCorki.Program.Game_OnGameLoad();
                            break;
                        case 13: // LeCorki
                            LeCorki.Program.Main();
                            break;
                        case 14: // PewPewCorki
                            PewPewCorki.Program.Main();
                            break;
                        case 15: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Darius:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 4: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 5: // Darius#
                            DariusSharp.Program.Main();
                            break;
                        case 6: // KurisuDarius
                            KurisuDarius.Program.Main();
                            break;
                        case 7: // ODarius
                            ODarius.Program.Main();
                            break;
                        case 8: // PerfectDarius
                            PerfectDarius.Program.Main();
                            break;
                        case 9: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Diana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElDiana
                            ElDiana.Diana.OnLoad();
                            break;
                        case 1: // D-Diana
                            D_Diana.Program.Game_OnGameLoad();
                            break;
                        case 2: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 4: // Diana Masterrace
                            Diana_Masterrace.Program.Main();
                            break;
                        case 5: // MoonDiana
                            MoonDiana.Program.Main();
                            break;
                        case 6: // NechritoDiana
                            Nechrito_Diana.Program.Main();
                            break;
                        case 7: // TC_SDKExAIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                        case 8: // Flowers' Diana
                            Flowers_Diana.Program.Main();
                            break;
                        case 9: // ElDiana Revamped
                            ElDianaRevamped.Program.Bootstrap();
                            break;
                    }
                    break;
                case Champion.DrMundo:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Hestia's Mundo
                            Mundo.Program.Main();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // KappaSeries
                            KappaSeries.Program.OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 5: // StonedSeries
                            StonedSeriesAIO.Program.Main();
                            break;
                        case 6: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 7: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                        case 8: // MundoSharpy
                            Mundo_Sharpy.Program.Main();
                            break;
                    }
                    break;
                case Champion.Draven:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 2: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 3: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 4: // M00N Draven
                            MoonDraven.Program.GameOnOnGameLoad();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // Tyler1
                            Tyler1.Program.Main();
                            break;
                        case 7: // BadaoDraven
                            BadaoDraven.Program.Main();
                            break;
                        case 8: // myWorld AIO
                            myWorld.Program.Main();
                            break;
                        case 9: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 10: // Flowers' ADC_Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Ekko:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // EloFactory Ekko
                            EloFactory_Ekko.Program.Game_OnGameLoad();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // Ekko Master of Time
                            Ekko_master_of_time.Program.Main();
                            break;
                        case 4: // Ekko The Boy Who Shattered Time
                            Ekko_the_Boy_Who_Shattered_Time.Bootstrap.Main();
                            break;
                        case 5: // EkkoGod
                            EkkoGod.Program.Main();
                            break;
                        case 6: // ElEkko
                            ElEkko.Program.Main();
                            break;
                        case 7: // Hikicarry Ekko
                            HikiCarry_Ekko.Program.Main();
                            break;
                        case 8: // TheEkko
                            TheEkko.Program.Main();
                            break;
                    }
                    break;
                case Champion.Elise:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // GFUEL Elise
                            GFUELElise.Elise.OnGameLoad();
                            break;
                        case 1: // D-Elise
                            D_Elise.Program.Game_OnGameLoad();
                            break;
                        case 2: // EliseGod
                            EliseGod.Program.OnGameLoad();
                            break;
                        case 3: // Hikigaya Elise
                            HikiCarry_Elise.Program.Game_OnGameLoad();
                            break;
                        case 4: // Sense Elise
                            Sense_Elise.Program.Main();
                            break;
                        case 5: // SephElise
                            SephElise.Program.Main();
                            break;
                        case 6: // BadaoElise
                            BadaoKingdom.Program.Main();
                            break;

                    }
                    break;
                case Champion.Evelynn:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Evelynn#
                            Evelynn.Program.Game_OnGameLoad();
                            break;
                        case 1: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 4: // TroopAIO
                            _SDK_TroopAIO.Program.Main();
                            break;
                        case 5: // JustEvelynn
                            JustEvelynn.Program.Main();
                            break;
                        case 6: // SkyLv Evelynn
                            SkyLv_Evelynn.Initialiser.Main();
                            break;
                    }
                    break;
                case Champion.Ezreal:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // ADCPackage
                            ADCPackage.Program.Game_OnGameLoad();
                            break;
                        case 2: //D-Ezreal
                            D_Ezreal.Program.Game_OnGameLoad();
                            break;
                        case 3: //DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 4: //hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 5: //iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 6: //ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 7: //SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 8: //SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 9: //ShineAIO
                            ShineSharp.Program.Game_OnGameLoad();
                            break;
                        case 10: //UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 11: //xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 12: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 13: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 14: // DarkChild's Ezreal
                            DarkEzreal.Program.Main();
                            break;
                        case 15: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 16: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 17: // EasyEzreal
                            EasyEzreal.EazyEzreal.Main();
                            break;
                        case 18: // Ezreal - The Prodigal Explorer
                            Ezreal___The_prodigal_explorer.Program.Main();
                            break;
                        case 19: // Ezreal - The Dream Chaser
                            EzrealDreamCatcher.Program.Main();
                            break;
                        case 20: // IDzEzreal
                            iDZEzreal.Program.Main();
                            break;
                        case 21: // iEzrealReworked
                            iEzrealReworked.Program.Main();
                            break;
                        case 22: // Perplexed Ezreal
                            PerplexedEzreal.Program.Main();
                            break;
                        case 23: // myWorldAIO
                            myWorld.Program.Main();
                            break;
                        case 24: // TCSDexAIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                        case 25: // ProjectGeass
                            _Project_Geass.Program.Main();
                            break;
                        case 26: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 27: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                        case 28: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                        case 29: // HandicAPEzreal
                            HandicapEzreal.Program.Main();
                            break;
                    }
                    break;
                case Champion.FiddleSticks:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Feedlesticks
                            Feedlesticks.Program.Game_OnGameLoad();
                            break;
                        case 1: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 2: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Fiora:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Project Fiora
                            FioraProject.Program.Game_OnGameLoad();
                            break;
                        case 1: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 2: // jesuisFiora
                            jesuisFiora.Program.Main();
                            break;
                    }
                    break;
                case Champion.Fizz:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Math Fizz
                            MathFizz.Program.Game_OnGameLoad();
                            break;
                        case 1: // ElFizz
                            ElFizz.Fizz.OnGameLoad();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // HeavenStrikeFizz
                            HeavenStrikeFizz.Program.Main();
                            break;
                        case 4: // NoobFizz
                            NoobFizz.Program.Main();
                            break;
                        case 5: // OneKeyToFish
                            OneKeyToFish.Program.Main();
                            break;
                    }
                    break;
                case Champion.Galio:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // Desomond Galio
                            DesomondGalio.Program.Main();
                            break;
                        case 2: // Galio#
                            GalioSharp.Program.Main();
                            break;
                    }
                    break;
                case Champion.Gangplank:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // Badao Gangplank
                            BadaoGP.Program.Main();
                            break;
                        case 2: // BangPlank
                            Bangplank.Program.Main();
                            break;
                        case 3: // BePlank
                            BePlank.Program.Main();
                            break;
                        case 4: // e.Motion Gangplank
                            e.Motion_Gangplank.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Garen:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // TheGaren
                            TheGaren.Program.Main();
                            break;
                        case 2: // TroopGaren
                            TroopGaren.Program.Main();
                            break;
                        case 3: // yol0 Garen
                            yol0Garen.Program.Main();
                            break;
                    }
                    break;
                case Champion.Gnar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Hellsing's Gnar
                            Gnar.Program.Game_OnGameLoad();
                            break;
                        case 1: // SluttyGnar
                            Slutty_Gnar_Reworked.Gnar.OnLoad();
                            break;
                        case 2: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 3: // hGnar
                            hGnar.Program.Main();
                            break;
                        case 4: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Gragas:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // The Drunk Carry
                            GragasTheDrunkCarry.Program.Main();
                            break;
                        case 1: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // LadyGragas
                            LadyGragas.Program.Main();
                            break;
                        case 4: // NechritoGragas
                            Nechrito_Gragas.Program.Main();
                            break;
                        case 5: // OriginalGragas
                            Original_Gragas.Program.Main();
                            break;
                    }
                    break;
                case Champion.Graves:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // D-Graves
                            D_Graves.Program.Game_OnGameLoad();
                            break;
                        case 2: // Hikimarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 3: // KurisuGraves
                            KurisuGraves.Program.Game_OnLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 7: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 8: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 9: // VSTGraves
                            VST_Auto_Carry_Standalone_Graves.Program.Main();
                            break;
                        case 10: // EasyGraves
                            EasyGraves.EasyGraves.Main();
                            break;
                        case 11: // BadaoGraves
                            BadaoKingdom.Program.Main();
                            break;
                        case 12: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Hecarim:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // JustHecarim
                            JustHecarim.Program.OnLoad();
                            break;
                        case 1: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // [SBTW] Hecarim
                            Flowers_Hecarim.Program.Main();
                            break;
                        case 4: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 5: // Herrari 477 GTB
                            Herrari_488_GTB.Program.Main();
                            break;
                        case 6: // Ponycopter
                            Ponycopter.Ponycopter.Main();
                            break;
                    }
                    break;
                case Champion.Heimerdinger:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // 2Girls1Donger
                            Two_Girls_One_Donger.Program.Game_OnGameLoad();
                            break;
                        case 1: // TheDonger
                            The_Donger.Donger.Main();
                            break;
                    }
                    break;
                case Champion.Illaoi:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Tentacle Kitty
                            Illaoi___Tentacle_Kitty.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 3: // Kraken Priestess
                            Flowers__Illaoi.Program.Main();
                            break;
                        case 4: // IllaoiSOH
                            IllaoiSOH.Program.Main();
                            break;
                        case 5: // TentacleBabeIllaoi
                            TentacleBabeIllaoi.Program.Main();
                            break;
                    }
                    break;
                case Champion.Irelia:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Irelia II
                            Irelia.Irelia.Game_OnGameLoad();
                            break;
                        case 1: // Irelia to the Challenger
                            IreliaToTheChallenger.Program.Load();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 4: // IreliaGod
                            IreliaGod.Program.Main();
                            break;
                        case 5: // Irelia Reloaded
                            Irelia_Reloaded.Program.Main();
                            break;
                        case 6: // Rethought Irelia
                            Rethought_Irelia.Program.Main();
                            break;
                        case 7: // SluttyIrelia
                            Slutty_Irelia.Program.Main();
                            break;
                        case 8: // SurvivorSeries
                            SVIrelia.Program.Main();
                            break;
                        case 9: //SurvivorSeries AIO
                            SurvivorSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Ivern:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Janna:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // LCS Janna
                            LCS_Janna.Program.OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.JarvanIV:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 1: // D-Jarvan
                            D_Jarvan.Program.Game_OnGameLoad();
                            break;
                        case 2: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
                            break;
                        case 3: // J4Helper
                            J4Helper.Program.Main();
                            break;
                    }
                    break;
                case Champion.Jax:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // xQx Jax
                            JaxQx.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // NoobJaxReloaded
                            NoobJaxReloaded.Program.Game_OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // SkyLv JAx
                            SkyLv_Jax.Initialiser.Main();
                            break;
                    }
                    break;
                case Champion.Jayce:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // Hikicarry Jayce
                            HikiCarry_Jayce___Hammer_of_Justice.Program.OnGameLoad();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // AJayce
                            AJayce.Program.Main();
                            break;
                        case 4: // JayceSharpV2
                            JayceSharpV2.Program.Main();
                            break;
                    }
                    break;
                case Champion.Jhin:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // Hikigaya's Jhin
                            Jhin___The_Virtuoso.Program.JhinOnGameLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // hJhin
                            hJhin.Program.Main();
                            break;
                        case 6: // Jhin As The Virtuoso
                            Jhin_As_The_Virtuoso.Program.Main();
                            break;
                        case 7: // BadaoJhin
                            BadaoKingdom.Program.Main();
                            break;
                        case 8: // Tc_SDKexAIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                        case 9: // Flowers' Jhin
                            Flowers_Jhin.Program.Main();
                            break;
                        case 10: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 11: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Jinx:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // ADCPackage
                            ADCPackage.Program.Game_OnGameLoad();
                            break;
                        case 2: // GenesisJinx
                            Jinx_Genesis.Program.Game_OnGameLoad();
                            break;
                        case 3: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 4: // ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 7: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 8: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 9: // CJShu Jinx
                            CjShuJinx.Program.Main();
                            break;
                        case 10: // EasyJinx
                            EasyJinx.EasyJinx.Main();
                            break;
                        case 11: // EloFactory Jinx
                            EloFactory_Jinx.Program.Main();
                            break;
                        case 12: // GenerationJinx
                            GenerationJinx.Program.Main();
                            break;
                        case 13: // PennyJinx Reborn
                            PennyJinxReborn.Program.Main();
                            break;
                        case 14: // myWorld AIO
                            myWorld.Program.Main();
                            break;
                        case 15: // Tc_SDKex AIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                        case 16: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 17: // Flowers' ADC_Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Kalista:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // S+Class
                            S_Plus_Class_Kalista.Program.OnLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // HERMES Kalista
                            HERMES_Kalista.Program.Main();
                            break;
                        case 3: // Hikicarry Kalista
                            HikiCarry_Kalista.Program.Game_OnGameLoad();
                            break;
                        case 4: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 5: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 6: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 7: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 8: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 9: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 10: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 11: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 12: // xcsoft's Kalista
                            xcKalista.Program.Main();
                            break;
                        case 13: // DonghuKalista
                            DonguKalista.Program.Main();
                            break;
                        case 14: // EasyKalista
                            break;
                        case 15: // ElKalista
                            ElKalista.Program.Main();
                            break;
                        case 16: // iKalista
                            IKalista.Program.Main();
                            break;
                        case 17: // iKalista:Reborn
                            iKalistaReborn.Program.Main();
                            break;
                        case 18: // Kalima
                            Kalima.Kalista.Main();
                            break;
                        case 19: // KAPPALISTAXD
                            KAPPALISTAXD.Program.Main();
                            break;
                        case 20: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 21: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Karma:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu's Karma
                            Karma.Program.Game_OnGameLoad();
                            break;
                        case 1: // KarmaXD
                            KarmaXD.Program.Game_OnGameLoad();
                            break;
                        case 2: // SupportIsEasy
                            Support.Program.Main();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 4: // [SBTW] Karma
                            Flowers_Karma.Program.Main();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 7: // SpiritKarma
                            Spirit_Karma.Program.Main();
                            break;
                        case 8: // Karma - The Enlightened One
                            Karma______the_Enlightened_One.Program.Main();
                            break;
                    }
                    break;
                case Champion.Karthus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // RAREKarthus
                            RAREKarthus.Program.Main();
                            break;
                        case 4: // Karthus#
                            KarthusSharp.Program.Main();
                            break;
                        case 5: // KimbaengKarthus
                            Kimbaeng_KarThus.Program.Main();
                            break;
                        case 6: // SNKarthus
                            SNKarthus.Program.Main();
                            break;
                        case 7: // XDSharpAIO
                            XDSharp.Program.Main();
                            break;
                        case 8: // ExorAIO
                            ExorAIO.Program.Main();
                            break;

                    }
                    break;
                case Champion.Kassadin:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // PainInMyKass
                            PainInMyKass.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 2: // SluttyKassadin
                            Kassawin.Kassadin.OnLoad();
                            break;
                        case 3: // PreservedKassadin
                            Preserved_Kassadin.Program.Main();
                            break;
                        case 4: // Kassadin The Harbringer
                            Kassadin_the_Harbinger.Program.Main();
                            break;
                        case 5: // KicKassadin
                            KicKassadin.KicKassadin.Main();
                            break;
                    }
                    break;
                case Champion.Katarina:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Staberina
                            Staberina.Program.Main();
                            break;
                        case 1: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 2: // ElSmartKatarina
                            ElKatarina.Program.OnLoad();
                            break;
                        case 3: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 4: // e.Motion Katarina
                            e.Motion_Katarina.Program.Game_OnGameLoad();
                            break;
                        case 5: // EasyCarry Katarina
                            EasyCarryKatarina.Program.Main();
                            break;
                        case 6: // JustKatarina
                            JustKatarina.Program.Main();
                            break;
                        case 7: // sKatarina
                            sKatarina.Program.Main();
                            break;
                        case 8: // SluttyKatarina
                            Slutty_Katarina.Program.Main();
                            break;
                        case 9: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Kayle:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SephKayle
                            SephKayle.Program.OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // D-Kayle
                            D_Kayle.Program.Game_OnGameLoad();
                            break;
                        case 3: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 4: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 5: // Hikicarry Kayle
                            HikiCarry_Kayle.Program.Main();
                            break;
                        case 6: // LeKayle
                            LeKayle.Program.Main();
                            break;
                        case 7: // Roach's Kayle
                            RoachKayle.Program.Main();
                            break;
                    }
                    break;
                case Champion.Kennen:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // Hestia's Kennen
                            Kennen.Champion.Kennen.Kennen_OnLoad();
                            break;
                        case 3: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                    }
                    break;
                case Champion.Khazix:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SephKhaZix
                            SephKhazix.Khazix.Main();
                            break;
                        case 1: // KhaZix#
                            KhazixSharp.Program.Main();
                            break;
                        case 2: // SSKha'Zix
                            SSKhaZix.SSKhaXiz.Main();
                            break;
                    }
                    break;
                case Champion.Kindred:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Yin & Yang
                            Kindred___YinYang.Program.Game_OnGameLoad();
                            break;
                        case 1: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 2: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 3: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 4: // KindredSpirits
                            KindredSpirits.Program.Main();
                            break;
                        case 5: // Slutty Kindred
                            Slutty_Kindred.Program.Main();
                            break;
                        case 6: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Kled:
                    Hiki.Kled.Program.Main();
                    break;
                case Champion.KogMaw:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // D-Kog
                            D_Kogmaw.Program.Game_OnGameLoad();
                            break;
                        case 2: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 3: // ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 7: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 8: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 9: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 10: // EasyKogmaw
                            EasyKogMaw.EasyKogMaw.Main();
                            break;
                        case 18: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Leblanc:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // LeBlanc II
                            Leblanc.Leblanc.Game_OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // LCS Leblanc
                            LCS_LeBlanc.Program.OnLoad();
                            break;
                        case 3: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 4: // BadaoLeblanc
                            break;
                        case 5: // PopBlanc
                            PopBlanc.Program.Main();
                            break;
                    }
                    break;
                case Champion.LeeSin:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElLeeSin
                            ElLeeSin.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 3: // Hikicarry Lee Sin
                            HikiCarry_Lee_Sin.Program.Main();
                            break;
                        case 4: // Lee is Back
                            LeeSin.Program.Main();
                            break;
                        case 5: // Slutty Lee Sin
                            Lee_Sin.Program.Main();
                            break;
                        case 6: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                        case 7: // yol0LeeSin
                            yol0LeeSin.Program.Main();
                            break;
                        case 8: // TCSDKexAIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                    }
                    break;
                case Champion.Leona:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 1: // Support Is Easy
                            Support.Program.Main();
                            break;
                        case 2: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 3: // SethLeona
                            SethLeona.Program.Main();
                            break;
                        case 4: // Troopeona
                            Troopeona.Program.Main();
                            break;
                        case 5: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Lissandra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SephLissandra
                            SephLissandra.Program.Main();
                            break;
                        case 1: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 2: // Lissandra The Ice Goddess
                            Lissandra_the_Ice_Goddess.Program.Main();
                            break;
                    }
                    break;
                case Champion.Lucian:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // LCS Lucian
                            LCS_Lucian.Program.OnLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 3: // HoolaLucian
                            HoolaLucian.Program.OnGameLoad();
                            break;
                        case 4: // iLucian
                            iLucian.LucianBootstrap.OnGameLoad();
                            break;
                        case 5: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 6: // Korean Lucian
                            KoreanLucian.Program.Game_OnGameLoad();
                            break;
                        case 7: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 8: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 9: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 10: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 11: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 12: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 13: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 14: // D_Lucian
                            D_Lucian.Program.Main();
                            break;
                        case 15: // FuckingLucianReborn
                            FuckingLucianReborn.Program.Main();
                            break;
                        case 16: // Slutty Lucian
                            Slutty_Lucian.Program.Main();
                            break;
                        case 17: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 18: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                        case 19: // ReformedAIO
                            ReformedAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Lulu:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Lululicious
                            LuluLicious.Program.Main();
                            break;
                        case 1: // HeavenstrikeLulu
                            HeavenStrikeLuLu.Program.Game_OnGameLoad();
                            break;
                        case 2: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 3: // Support Is Easy
                            Support.Program.Main();
                            break;
                        case 4: // Lulu & Pix
                            Lulu_and_Pix.Program.Main();
                            break;
                        case 5: // Lulu#
                            LuluSharp.Program.Main();
                            break;
                        case 6: // SethLulu
                            SethLulu.Program.Main();
                            break;
                    }
                    break;
                case Champion.Lux:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 2: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 3: // M00N Lux
                            MoonLux.Program.Main();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // Cheerleader Lux
                            CheerleaderLux.Loader.Main();
                            break;
                        case 6: // ElLux
                            ElLux.Program.Main();
                            break;
                        case 7: // Hikigaya Lux
                            Hikigaya_Lux.Program.Main();
                            break;
                        case 8: // SephLux
                            SephLux.Program.Main();
                            break;
                    }
                    break;
                case Champion.Malphite:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 1: //JustMalphite
                            JustMalphite.Program.Main();
                            break;
                        case 2: // SephMalphite
                            Malphite.Program.Main();
                            break;
                    }
                    break;
                case Champion.Malzahar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // SurvivorSeries
                            SurvivorMalzahar.Program.Game_OnGameLoad();
                            break;
                        case 2: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 3: // NoobMalzahar
                            NoobMalzahar.Program.Main();
                            break;
                        case 4: //SurvivorSeries AIO
                            SurvivorSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Maokai:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // JustMaokai
                            JustMaokai.Program.Main();
                            break;
                    }
                    break;
                case Champion.MasterYi:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // MasterSharp
                            MasterSharp.Program.Main();
                            break;
                        case 1: // Hoola Yi
                            HoolaMasterYi.Program.OnGameLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // MasterYi by Prunes
                            MasterYiByPrunes.Program.Main();
                            break;
                        case 4: // xQx Yi
                            MasterYiQx.Program.Main();
                            break;
                        case 5: // Yi by Crisdmc
                            crisMasterYi.Program.Main();
                            break;
                        case 6: // ChallengerYi
                            ChallengerYi.Program.Main();
                            break;
                    }
                    break;
                case Champion.MissFortune:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // Alex's MF
                            Miss_Fortune.Program.OnLoad();
                            break;
                        case 2: // D-MF
                            D_MissF.Program.Game_OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 7: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 8: // BadaoMissFortune
                            BadaoMissFortune.Program.Main();
                            break;
                        case 9: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Mordekaiser:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // xQx Mordekaiser
                            Mordekaiser.Program.Game_OnGameLoad();
                            break;
                        case 1: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Morgana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // KurisuMorgana
                            KurisuMorgana.Program.Game_OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 3: // ShineAIO
                            ShineSharp.Program.Game_OnGameLoad();
                            break;
                        case 4: // Support Is Easy
                            Support.Program.Main();
                            break;
                        case 5: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 6: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nami:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElNami
                            ElNamiBurrito.Nami.Game_OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // Support Is Easy
                            Support.Program.Main();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 4: // ElNamiRevamped
                            ElNamiDecentralized.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nasus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // Nasus The Crazy Dog
                            break;
                        case 4: // Nasus the Lumberjack
                            NasusTheLumberJack.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nautilus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Nautilus - Danz
                            Nautilus_AnchorTheChallenger.program.Game_OnGameLoad();
                            break;
                        case 1: // PlebNautilus
                            PlebNautilus.Program.Game_OnGameLoad();
                            break;
                        case 2: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 4: // Hestia's Nautilus
                            HestiaNautilus.Program.Main();
                            break;
                        case 5: // JustNautilus
                            JustNautilus.Program.Main();
                            break;
                        case 6: // Nautilus - The Freelo Titan
                            Nautilus_Is_Meme.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nidalee:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: //Kurisu Nidalee
                            KurisuNidalee.Program.Main();
                            break;
                        case 1: // HeavenStrikeNidalee
                            HeavenStrikeNidalee.Program.Game_OnGameLoad();
                            break;
                        case 2: //Nechrito Nidalee
                            Nechrito_Nidalee.Program.OnLoad();
                            break;
                        case 3: // D-Nidalee
                            D_Nidalee.Program.Game_OnGameLoad();
                            break;
                        case 4: // Flowers' Nidalee
                            Flowers_Nidalee.Program.OnGameLoad();
                            break;
                        case 5: // Nidalee the Beastial Huntress
                            NidaleeTheBestialHuntress.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nocturne:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // xQx Nocturne
                            Nocturne.Nocturne.Init();
                            break;
                        case 2: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Nunu:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Nunu by Aqlohol
                            LSharpNunu.Nunu.Game_OnGameLoad();
                            break;
                        case 1: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 2: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Olaf:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Olaf is Back II
                            Olaf.Olaf.Game_OnGameLoad();
                            break;
                        case 1: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 2: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 3: // JustOlaf Reborn
                            JustOlaf___Reborn.Program.Main();
                            break;
                        case 4: // Korean Olaf
                            KoreanOlaf.Program.Main();
                            break;
                    }
                    break;
                case Champion.Orianna:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu Orianna
                            Orianna.Program.Game_OnGameLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 6: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 7: // Orianna - the Ruler of Ball
                            OriannaTheruleroftheBall.Program.Main();
                            break;
                        case 8: // Orianna by Trelli
                            Orianna_by_trelli.Program.Main();
                            break;
                        case 9: // Orianna Grande
                            OriannaGrande.Program.Main();
                            break;
                        case 10: // MidLane#
                            MidlaneSharp.Program.Main();
                            break;
                    }
                    break;
                case Champion.Pantheon:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // xQx Pantheon
                            Pantheon.Program.Game_OnGameLoad();
                            break;
                        case 1: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 2: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 3: // NoobPantheon
                            NoobPantheon.Program.Main();
                            break;
                        case 4: // Pantheon mztikks
                            mztikkPantheon.Program.Main();
                            break;
                        case 5: // Roach's Pantheon
                            RoachPantheon.Program.Main();
                            break;
                    }
                    break;
                case Champion.Poppy:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 3: // BadaoPoppy
                            BadaoKingdom.Program.Main();
                            break;
                    }
                    break;
                case Champion.Quinn:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // GFUEL Quinn
                            GFUELQuinn.Quinn.OnGameLoad();
                            break;
                        case 2: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 4: // Tc_SDKex AIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                        case 5: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 6: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Rammus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 1: // Rammus is OK
                            Rammus.Program.Main();
                            break;
                    }
                    break;
                case Champion.RekSai:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // D-Rek'Sai
                            D_RekSai.Program.Game_OnGameLoad();
                            break;
                        case 1: // HeavenStrike Rek'Sai
                            HeavenStrikeReksaj.Program.Game_OnGameLoad();
                            break;
                        case 2: // Rek'Sai Winner of Fights
                            RekSai.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Renekton:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 2: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 3: // NoobRenekton
                            NoobRenekton.Program.Main();
                            break;
                        case 4: // sAIO
                            sAIO.Program.Main();
                            break;
                    }
                    break;

                case Champion.Rengar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElRengar
                            ElRengarRevamped.Rengar.OnLoad();
                            break;
                        case 1: // D-Rengar
                            D_Rengar.Program.Game_OnGameLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // Pridestalker Rengar
                            PrideStalker_Rengar.Program.Main();
                            break;
                        case 4: // Badao Rengar
                            BadaoRengar.Program.Main();
                            break;
                        case 5: // Experimental ElRengar
                            ElRengar.Program.Main();
                            break;
                        case 6: // Hoola Rengar
                            HoolaRengar.Program.Main();
                            break;
                        case 7: // NechritoRengar
                            Nechrito_Rengar.MAIN.Main();
                            break;
                    }
                    break;
                case Champion.Riven:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // KurisuRiven
                            KurisuRiven.Program.Game_OnGameLoad();
                            break;
                        case 1: // HoolaRiven
                            HoolaRiven.Program.OnGameLoad();
                            break;
                        case 2: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 3: // NechritoRiven
                            NechritoRiven.Program.Main();
                            break;
                        case 4: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 5: // ReforgedRiven
                            Reforged_Riven.Program.Main();
                            break;
                        case 6: // EasyPeasyRivenSqueezy
                            EasyPeasyRivenSqueezy.Program.Main();
                            break;
                        case 7: // EloFactory Riven
                            EloFactory_Riven.Program.Main();
                            break;
                        case 8: // Flower's Riven
                            Flowers_Riven.Program.Main();
                            break;
                        case 9: // HeavenStrikeRiven
                            HeavenStrikeRiven.Program.Main();
                            break;
                        case 10: // RivenSharpV2
                            RivenSharp.Program.Main();
                            break;
                        case 11: // yol0Riven
                            yol0Riven.Program.Main();
                            break;
                        case 12: // RivenToTheChallenger  
                            RivenToTheChallenger.Program.Main();
                            break;
                    }
                    break;
                case Champion.Rumble:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 2: // ElRumble
                            ElRumble.Program.Main();
                            break;
                    }
                    break;
                case Champion.Ryze:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Survivor Ryze
                            SurvivorRyze.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 3: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 4: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
                            break;
                        case 5: // ArcaneRyze
                            Arcane_Ryze.Program.Main();
                            break;
                        case 6: // EvictRyze
                            EvictRyze.Program.Main();
                            break;
                        case 7: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 8: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 9: // BurstRyze
                            RyzeAssembly.Program.Main();
                            break;
                        case 10: // HeavenStrikeRyze
                            HeavenStrikeRyze.Program.Main();
                            break;
                        case 11: // JustRyze
                            JustRyze.Program.Main();
                            break;
                        case 12: // Ryze#
                            RyzeSharp.Program.Main();
                            break;
                        case 13: // SluttyRyze
                            Slutty_ryze.Program.Main();
                            break;
                        case 14: // TRUSt in my Ryze
                            TrusRyze.Program.Main();
                            break;
                        case 15: // sAIO
                            sAIO.Program.Main();
                            break;
                        case 16: //SurvivorSeries AIO
                            SurvivorSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Sejuani:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElSejuani
                            ElSejuani.Sejuani.OnLoad();
                            break;
                        case 1: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Shaco:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // Ch3wyM00N Shaco
                            ChewyMoonsShaco.Program.Main();
                            break;
                    }
                    break;
                case Champion.Shen:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // Kimbaeng Shen
                            Kimbaeng_Shen.Program.Game_OnGameLoad();
                            break;
                        case 3: // BadaoShen
                            BadaoShen.Program.Main();
                            break;
                    }
                    break;
                case Champion.Shyvana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // D-Shyvana
                            D_Shyvana.Program.Game_OnGameLoad();
                            break;
                        case 1: // HeavenStrike Shyvana
                            HeavenStrikeShyvana.Program.Game_OnGameLoad();
                            break;
                        case 2: //  JustShyvana
                            JustShyvana.Program.OnLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Singed:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // ElSinged
                            ElSinged.Singed.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Sion:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // SimpleSion
                            Sion.Program.Main();
                            break;
                    }
                    break;
                case Champion.Sivir:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 3: // Proseries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // ShineAIO
                            ShineSharp.Program.Game_OnGameLoad();
                            break;
                        case 7: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 8: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 9: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 10: // xcsoft's Sivir
                            xcSivir.Program.Main();
                            break;
                        case 11: // HeavenStrikeSivir
                            HeavenStrikeSivir.Program.Main();
                            break;
                        case 12: // iSivir
                            iSivir.Program.Main();
                            break;
                        case 13: // JustSivir
                            JustSivir.Program.Main();
                            break;
                        case 14: // KurisuSivir
                            KurisuSivir.Program.Main();
                            break;
                        case 15: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 16: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Skarner:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // kSkarner
                            kSkarner.Program.Main();
                            break;
                        case 2: // SneakySkarner
                            SneakySkarner.Program.Main();
                            break;
                    }
                    break;
                case Champion.Sona:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 1: // Royal Song of Son
                            RoyalSona.Program.Game_OnGameLoad();
                            break;
                        case 2: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 3: // Vodka Sona
                            VodkaSona.Program.Game_OnLoad();
                            break;
                        case 4: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // mztikks Sona
                            mztikkSona.Program.Main();
                            break;
                    }
                    break;
                case Champion.Soraka:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Seph Soraka
                            SephSoraka.Soraka.SorakaMain();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // Heal-Bot
                            Soraka_HealBot.Program.OnGameLoad();
                            break;
                        case 3: // MLG Soraka
                            MLGSORAKA.Program.OnLoad();
                            break;
                        case 4: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 5: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 6: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 7: // Sophie's Soraka
                            Sophies_Soraka.Program.Main();
                            break;
                        case 8: // Soraka#
                            SorakaSharp.Source.Program.Main();
                            break;
                        case 9: // SorakaToTheChallenger
                            SorakaToTheChallenger.Program.Load();
                            break;
                    }
                    break;
                case Champion.Swain:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // SluttySwain
                            Slutty_Swain.Swain.OnLoad();
                            break;
                        case 2: // The Mocking Swain
                            The_Mocking_Swain.Program.Game_OnGameLoad();
                            break;
                        case 3: // xQx Swain
                            Swain.Program.Main();
                            break;
                    }
                    break;
                case Champion.Syndra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu's Syndra
                            Syndra.Program.Game_OnGameLoad();
                            break;
                        case 1: // BadaoSeries
                            BadaoSeries.Program.OnLoad();
                            break;
                        case 2: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 3: // Hikigaya Syndra
                            Hikigaya_Syndra.Program.OnLoad();
                            break;
                        case 4: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 5: // Syndra by L33T
                            SyndraL33T.Bootstrap.Main();
                            break;
                        case 6: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 7: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 8: // SephSyndra
                            SephSyndra.Syndra.Main();
                            break;
                        case 9: // Syndra - The Dark Sovereign
                            Syndra______The_Dark_Sovereign.Program.Main();
                            break;
                    }
                    break;
                case Champion.TahmKench:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // STahmKench
                            STahmKench.Program.Main();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 4: // Hahaha's Tahm Kench
                            Hahaha_s_Tahm_Kench.Program.Main();
                            break;
                        case 5:
                            ElTahmKench.Program.Bootstrap();
                            break;
                    }
                    break;
                case Champion.Taliyah:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // TophSharp
                            TophSharp.Taliyah.OnLoad();
                            break;
                        case 1: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 2: // Stoneweaver
                            Taliyah.Program.Main();
                            break;
                    }
                    break;
                case Champion.Talon:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // GFUEL Talon
                            GFUELTalon.Talon.OnGameLoad();
                            break;
                        case 1: // Hoola Talon
                            HoolaTalon.Program.OnGameLoad();
                            break;
                        case 2: // Badao Talon
                            badaoTalon.Program.Main();
                            break;
                        case 3: // ElTalon
                            ElTalon.Program.Main();
                            break;
                        case 4: // HeavenStrikeTalon
                            HeavenStrikeTalon.Program.Main();
                            break;
                        case 5: // MistakenTalon
                            MistakenTalon.Program.Main();
                            break;
                        case 6: // TrooplonRewritten
                            TrooplonRewritten.Program.Main();
                            break;
                        case 7: // sAIO
                            sAIO.Program.Main();
                            break;
                        case 8: // Flowers' Talon
                            Flowers_Talon.Program.Main();
                            break;
                    }
                    break;
                case Champion.Taric:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SkyLv_Taric
                            SkyLv_Taric.Initialiser.Game_OnGameLoad();
                            break;
                        case 1: // ElEasy
                            ElEasy.Entry.OnLoad();
                            break;
                        case 2: // PippyTaric
                            PippyTaric.Program.LoadStuff();
                            break;
                        case 3: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 4: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Teemo:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // KarmaPanda
                            Chat.Print("Credits : KarmaPanda");
                            PandaTeemo.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 3: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 4: // SwiftlyTeemo
                            Swiftly_Teemo.Program.Main();
                            break;
                        case 5: // NoobTeemo
                            NoobTeemo.Program.Main();
                            break;
                        case 6: // TSM_Teemo
                            TSM_Teemo.Program.Main();
                            break;
                        case 7: // Tc_SDKEx AIO
                            Tc_SDKexAIO.PlaySharp.Main();
                            break;
                    }
                    break;
                case Champion.Thresh:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Chain Warden
                            Thresh___The_Chain_Warden.Program.Game_OnGameLoad();
                            break;
                        case 1: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 2: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 3: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 4: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 5: // Dark Star Thresh
                            Dark_Star_Thresh.Program.OnLoad();
                            break;
                        case 6: // Slutty Thresh
                            Slutty_Thresh.Program.Main();
                            break;
                        case 7: // Thresh as the Chain Warden
                            ThreshAsTheChainWarden.Program.Main();
                            break;
                        case 8: // Thresh - Catch Fish
                            ThreshCatchFish.Program.Main();
                            break;
                        case 9: // Thresh Ruler of the Soul
                            ThreshTherulerofthesoul.Program.Main();
                            break;
                        case 10: // Thresh the Flay Maker
                            ThreshFlayMaker.Program.Main();
                            break;
                        case 11: // yol0 Thresh
                            yol0Thresh.Program.Main();
                            break;
                    }
                    break;
                case Champion.Tristana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElTristana
                            ElTristana.Tristana.OnLoad();
                            break;
                        case 1: // ADCPackage
                            ADCPackage.Program.Game_OnGameLoad();
                            break;
                        case 2: // D-Tristana
                            D_Tristana.program.Game_OnGameLoad();
                            break;
                        case 3: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 4: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 5: // PewPewTristana
                            PewPewTristana.Program.OnLoad();
                            break;
                        case 6: // ProSeries
                            ProSeries.Program.GameOnOnGameLoad();
                            break;
                        case 7: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 8: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 9: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 10: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 11: // Flower's Tristana
                            Flowers_Tristana.Program.Main();
                            break;
                        case 12: // Geass Tristana
                            Geass_Tristana.Program.Main();
                            break;
                        case 13: // SkyLv Tristana
                            SkyLv_Tristana.Initialiser.Main();
                            break;
                        case 14: // Tristana#
                            TristanaSharp.Program.Main();
                            break;
                        case 15: // TrooperTristana
                            TrooperTristana.Program.Main();
                            break;
                        case 16: // ProjectGeass
                            _Project_Geass.Program.Main();
                            break;
                        case 17: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 18: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Trundle:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElTrundle
                            ElTrundle.Trundle.OnLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // vSeries
                            vSupport_Series.Program.Game_OnGameLoad();
                            break;
                        case 4: // xDTrundle
                            xDTrundle.Program.Main();
                            break;
                        case 5: // FastTrundle
                            FastTrundle.Program.Main();
                            break;
                        case 6: // JustTrundle
                            JustTrundle.Program.Main();
                            break;
                    }
                    break;
                case Champion.Tryndamere:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 1: // The Lich King
                            TheLichKing.Program.Game_OnGameLoad();
                            break;
                        case 2: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 3: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.TwistedFate:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu Twistedfate
                            TwistedFate.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2: // BadaoSeries
                            BadaoSeries.Program.OnLoad();
                            break;
                        case 3: // EloFactor TF
                            EloFactory_TwistedFate.Program.Game_OnGameLoad();
                            break;
                        case 4: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 5: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 6: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 7: // TwistedFate-Danz
                            Twisted_Fate___Its_all_in_the_cards.Program.Game_OnGameLoad();
                            break;
                        case 8: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 9: // RARETwistedFate
                            RARETwistedFate.Program.Main();
                            break;
                        case 10: // Diabath's TwistedFate
                            D_TwistedFate.Program.Main();
                            break;
                        case 11: // Flower's Twisted Fate
                            FlowersTwistedFate.Program.Main();
                            break;
                        case 12: // mztikks Twisted Fate
                            mztikksTwistedFate.Program.Main();
                            break;
                    }
                    break;
                case Champion.Twitch:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 2: // iTwitch2.0
                            iTwitch.Program.Main();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 5: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 6: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 7: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 8: // InfectedTwitch
                            Infected_Twitch.Program.Main();
                            break;
                        case 9: // Flower's Twitch
                            Flowers_Twitch.Program.Main();
                            break;
                        case 10: // NechritoTwitch
                            Nechrito_Twitch.Program.Main();
                            break;
                        case 11: // SNTwitch
                            SNTwitch.Program.Main();
                            break;
                        case 12: // theobjops's Twitch
                            Twiitch.Twitch.Main();
                            break;
                        case 13: // TheTwitch
                            TheTwitch.Program.Main();
                            break;
                        case 14: // Twitch#
                            TwitchSharp.Program.Main();
                            break;
                        case 15: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Udyr:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 1: // D-Udyr
                            D_Udyr.Program.Game_OnGameLoad();
                            break;
                        case 2: // EloFactory Udyr
                            EloFactory_Udyr.Program.Game_OnGameLoad();
                            break;
                        case 3: // LCS Udyr
                            LCS_Udyr.Program.OnGameLoad();
                            break;
                        case 4: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 5: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 6: // NoodUdyr
                            NoobUdyr.Program.Main();
                            break;
                    }
                    break;
                case Champion.Urgot:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 2: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 3: // Dicaste's Urgot
                            DicasteUrgot.Program.Main();
                            break;
                        case 4: // TroopAIO
                            _SDK_TroopAIO.Program.Main();
                            break;
                        case 5: // TUrgot
                            TUrgot.Program.Main();
                            break;
                        case 6: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Varus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElVarus
                            Elvarus.Varus.Game_OnGameLoad();
                            break;
                        case 1: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 2: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 3: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 4: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 5: // VarusGod
                            Varus_God.Program.Main();
                            break;
                        case 6: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 7: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                        case 8: // ElVarusRevamped
                            ElVarusRevamped.Program.Main();
                            break;
                    }
                    break;
                case Champion.Vayne:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // VayneHunterReborn
                            VayneHunter_Reborn.Program.Main();
                            break;
                        case 1: // hikiMarksman
                            hikiMarksmanRework.Program.Game_OnGameLoad();
                            break;
                        case 2: // iSeriesReborn
                            iSeriesReborn.Program.OnGameLoad();
                            break;
                        case 3: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 4: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 5: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 6: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 7: // Marksman#
                            Marksman.Program.Game_OnGameLoad();
                            break;
                        case 8: // hi im gosu
                            hi_im_gosu.Vayne.Game_OnGameLoad();
                            break;
                        case 9: // ChallengerSeries
                            Challenger_Series.Program.Main();
                            break;
                        case 10: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 11: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 12: // hVayne
                            hVayne.Program.Main();
                            break;
                        case 13: // Hikicarry Vayne Masterrace
                            HikiCarry_Vayne_Masterrace.Program.Main();
                            break;
                        case 14: // PRADA Vayne
                            PRADA_Vayne.Program.Main();
                            break;
                        case 15: // SOLO Vayne
                            SoloVayne.Program.Main();
                            break;
                        case 16: // VayneGodMode
                            GodModeOn_Vayne.Program.Main();
                            break;
                        case 17: // Hikicarry ADC
                            HikiCarry.Program.Main();
                            break;
                        case 18: // Flowers' ADC Series
                            Flowers_ADC_Series.Program.Main();
                            break;
                    }
                    break;
                case Champion.Veigar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // FreshBooster
                            FreshBooster.Program.Game_OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // ElVeigar
                            ElVeigar.ElVeigar.Main();
                            break;
                        case 6: // Placebo Veigar
                            PlaceboVeigar.Program.Main();
                            break;
                        case 7: // Slutty Veigar
                            Slutty_Veigar.Program.Main();
                            break;
                        case 8: // BadaoVeigar
                            BadaoKingdom.Program.Main();
                            break;
                    }
                    break;
                case Champion.Velkoz:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu Vel'Koz
                            Velkoz.Program.Game_OnGameLoad();
                            break;
                        case 1: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                    }
                    break;
                case Champion.Vi:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElVi
                            ElVi.Vi.OnLoad();
                            break;
                        case 1: // xQx Vi
                            Vi.Vi.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Viktor:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // TRUSt in my Viktor
                            Viktor.Program.Game_OnGameLoad();
                            break;
                        case 1: // Hikicarry Viktor
                            HikiCarry_Viktor.Program.Game_OnGameLoad();
                            break;
                        case 2: // Perplexed Viktor
                            PerplexedViktor.Program.Game_OnGameLoad();
                            break;
                        case 3: // SAutoCarry
                            SAutoCarry.Program.Game_OnGameLoad();
                            break;
                        case 4: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 5: // Badao's Viktor
                            ViktorBadao.Program.Game_OnGameLoad();
                            break;
                        case 6: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 7: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 8: // Flowers' Viktor
                            Flowers_Viktor.Program.Main();
                            break;
                        case 9:
                            TRUStInMyViktor.Program.Main();
                            break;
                    }
                    break;
                case Champion.Vladimir:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElVladimir
                            ElVladimirReborn.Vladimir.OnLoad();
                            break;
                        case 1: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 2: // SFXChallenger
                            SFXChallenger.Program.Main();
                            break;
                        case 3: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 4: // [SBTW] Vladimir
                            Flowers_Vladimir.Program.Main();
                            break;
                        case 5: // Flowers' Series
                            Flowers_Series.Program.Main();
                            break;
                        case 6: // TroopAIO
                            _SDK_TroopAIO.Program.Main();
                            break;
                        case 7: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                    }
                    break;
                case Champion.Volibear:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // KappaSeries
                            KappaSeries.Program.OnGameLoad();
                            break;
                        case 2: // NoobVolibear
                            NoobVolibear.Program.Game_OnGameLoad();
                            break;
                        case 3: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
                            break;
                    }
                    break;
                case Champion.Warwick:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // The Blood Hunter
                            Warwick.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // D-Warwick
                            D_Warwick.Program.Game_OnGameLoad();
                            break;
                        case 3: // DZAIO
                            DZAIO_Reborn.Program.Main();
                            break;
                        case 4: // ExorAIO
                            ExorAIO.Program.Main();
                            break;
                        case 5: // Warwick II
                            WarwickII.Program.Main();
                            break;
                    }
                    break;
                case Champion.MonkeyKing:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // 2Girls1Monkey
                            Two_Girls_One_Monkey.Program.Main();
                            break;
                        case 2: // HoolaWukong
                            HoolaWukong.Program.Main();
                            break;
                        case 3: // JustWukong
                            JustWukong.Program.Main();
                            break;
                        case 4: // mztikk's Wukong
                            Wukong.Program.Main();
                            break;
                        case 5: // xQx Wukong
                            WukongxQx.Program.Main();
                            break;
                        case 6: // NoobWukong
                            NoobWukong.Program.Main();
                            break;

                    }
                    break;
                case Champion.Xerath:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu's Xerath
                            Xerath.Program.Game_OnGameLoad();
                            break;
                        case 1: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 2: // SluttyXerath
                            The_Slutty_Xerath.Xerath.OnLoad();
                            break;
                        case 3: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 4: // ElXerath
                            ElXerath.Program.Main();
                            break;
                        case 5: // Xerath - Magnus Ascendant
                            Xerath___The_Magus_Ascendant.Program.Main();
                            break;
                    }
                    break;
                case Champion.XinZhao:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // xQx XinZhao
                            XinZhao.Program.Game_OnGameLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // XinZhao God
                            Xin.Program.GameOnOnGameLoad();
                            break;
                        case 3: // mztikks Xin Zhao
                            mztikkXinZhao.Program.Main();
                            break;
                        case 4: // NoobXinZhao
                            NoobXin_Zhao.Program.Main();
                            break;
                    }
                    break;
                case Champion.Yasuo:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // YasuoPro
                            new YasuoPro.Yasuo().OnLoad();
                            break;
                        case 1: // BrianSharp
                            BrianSharp.Program.Main();
                            break;
                        case 2: // GosuMechanics
                            GosuMechanicsYasuo.Program.Game_OnGameLoad();
                            break;
                        case 3: // YasuoSharpv2
                            YasuoSharpV2.Program.Main();
                            break;
                        case 4: // MasterOfWinds
                            MasterOfWind.Program.Main();
                            break;
                        case 5: // M1D 0R F33D
                            Mid_or_Feed.Program.Main();
                            break;
                        case 6: //YasuoMemeBender
                            YasuoTheLastMemebender.Program.Game_OnGameLoad();
                            break;
                        case 7: // Media's Yasuo
                            YasuoMedia.Program.Main();
                            break;
                        case 8: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                        case 9: // Badaos Yasuo
                            BadaoYasuo.Program.Main();
                            break;
                        case 10: // hYasuo
                            hYasuo.Program.Main();
                            break;
                    }
                    break;
                case Champion.Yorick:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // The Staffer
                            yorick.Program.Main();
                            break;
                    }
                    break;
                case Champion.Zac:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // UnderratedAIO
                            UnderratedAIO.Program.OnGameLoad();
                            break;
                        case 1: // The Secret Flubber
                            Zac_The_Secret_Flubber.Program.Main();
                            break;
                    }
                    break;
                case Champion.Zed:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Korean Zed
                            KoreanZed.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpyAIO
                            Sharpy_AIO.Program.Game_OnGameLoad();
                            break;
                        case 2: // Valvrave#
                            Valvrave_Sharp.Program.Main();
                            break;
                        case 3: // iDZed
                            iDZed.Program.Main();
                            break;
                        case 4: // Ze-D Is Back
                            zedisback.Program.Main();
                            break;
                    }
                    break;
                case Champion.Ziggs:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Ziggs#
                            ZiggsKortatu.Program.Game_OnGameLoad();
                            break;
                        case 1: // Royal Ziggy
                            ZiggsRoyal.Program.Main();
                            break;
                    }
                    break;
                case Champion.Zilean:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // ElZilean
                            ElZilean.Zilean.OnGameLoad();
                            break;
                        case 1: // Support is Easy
                            Support.Program.Main();
                            break;
                    }
                    break;
                case Champion.Zyra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // D-Zyra
                            D_Zyra.Program.Game_OnGameLoad();
                            break;
                        case 1: // Support is Easy
                            Support.Program.Main();
                            break;
                        case 2: // xSalice
                            xSaliceResurrected_Rework.Program.LoadReligion();
                            break;
                        case 3: // RAREZyra
                            RAREZyra.Program.Main();
                            break;
                    }
                    break;
            }
        }
    }
}
