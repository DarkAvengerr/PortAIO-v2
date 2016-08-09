#region

using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using System.Linq;
using PortAIO.Dual_Port;
using LeagueSharp.Common;
// ReSharper disable ObjectCreationAsStatement

#endregion

namespace PortAIO
{
    internal static class Init
    {
        private static void Main()
        {
            Loading.OnLoadingComplete += Initialize;
        }

        private static void Initialize(EventArgs args)
        {
            //LeagueSharp.SDK.Bootstrap.Init(); - SDK is not yet added so it is not yet needed.
            Misc.Load();

            LoadChampion();
            LoadUtility();
        }

        public static void LoadUtility()
        {
            //ProFlash.Program.Main();
            //TiltSharp.Program.Main();
            //new SFXHumanizer_Pro.SFXHumanizerPro().OnGameLoad();

            //imAsharpHuman.Program.Main();
            //imAsharpHumanPro.Program.Main();

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
                }
            }

            if (Misc.menu.Item("enableTracker").GetValue<bool>())
            {
                switch (Misc.menu.Item("Tracker").GetValue<StringList>().SelectedIndex)
                {
                    case 0: // SFXUtility
                        SFXUtility.Program.Main();
                        break;
                    case 1: // ShadowTracker
                        ShadowTracker.Program.Game_OnGameLoad();
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
                }
            }
        }

        public static void LoadChampion()
        {
            // Support.Program.Main(); - Support is Easy Champions
            // ReformedAIO.Program.Main(); - ReformedAIO Champs
            // KappaSeries.Program.OnGameLoad(); - KappaSeries Champions
            // xSaliceResurrected.Program.LoadReligion(); - xSalice Champs
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
                            xSaliceResurrected.Program.LoadReligion();
                            break;
                        case 5: // BadaoSeries
                            BadaoSeries.Program.OnLoad();
                            break;
                        case 6: // DZAIO
                            DZAIO_Reborn.Program.Main();
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
                            xSaliceResurrected.Program.LoadReligion();
                            break;
                        case 5: // StonedSeriesAIO
                            StonedSeriesAIO.Program.Main();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            SurvivorSeries.SurviorSeries.Main();
                            break;
                        case 5: // xSalice
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            SurvivorSeries.SurviorSeries.Main();
                            break;
                        case 4: // yol0 Brand
                            yol0Brand.Program.Game_OnGameLoad();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                        case 2: // xSalice
                            xSaliceResurrected.Program.LoadReligion();
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
                    }
                    break;
                // Galio
                // Gangplank
                // Garen
                case Champion.Gnar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Hellsing's Gnar
                            Gnar.Program.Game_OnGameLoad();
                            break;
                        case 1: // SluttyGnar
                            Slutty_Gnar_Reworked.Gnar.OnLoad();
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
                    }
                    break;
                // Heimerdinger
                case Champion.Illaoi:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Tentacle Kitty
                            Illaoi___Tentacle_Kitty.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
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
                            xSaliceResurrected.Program.LoadReligion();
                            break;
                    }
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                            Kennen.Program.Main();
                            break;
                    }
                    break;
                // Kha'Zix
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
                    }
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
                            xSaliceResurrected.Program.LoadReligion();
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
                    }
                    break;
                case Champion.Lissandra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // SephLissandra
                            SephLissandra.Program.Main();
                            break;
                        case 1: //
                            xSaliceResurrected.Program.LoadReligion();
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
                            xSaliceResurrected.Program.LoadReligion();
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
                    }
                    break;
                // Malphite
                case Champion.Malzahar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // OKTW
                            OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                            break;
                        case 1: // SurvivorSeries
                            SurvivorSeries.SurviorSeries.Main();
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
                    }
                    break;
                case Champion.Nidalee:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {

                    }
                    break;
                // Nocturne
                case Champion.Nunu:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Olaf:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Orianna:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Pantheon:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Poppy:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Quinn:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Rammus
                case Champion.RekSai:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Renekton:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Rengar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Riven:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Rumble:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Ryze:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Sejuani:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Shaco
                case Champion.Shen:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Shyvana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Singed:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Sion
                case Champion.Sivir:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Skarner
                case Champion.Sona:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Soraka:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Swain:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Syndra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.TahmKench:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Taliyah
                case Champion.Talon:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Taric:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Teemo:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Thresh:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Tristana:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Trundle:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Tryndamere:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.TwistedFate:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Twitch:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Udyr:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Urgot:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Varus:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Vayne:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Veigar:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Velkoz:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Vi:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Viktor:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Vladimir:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Volibear:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Warwick:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Wukong
                case Champion.Xerath:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.XinZhao:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Yasuo:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Yorick
                // Zac
                case Champion.Zed:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                // Ziggs
                case Champion.Zilean:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
                case Champion.Zyra:
                    switch (Misc.menu.Item(ObjectManager.Player.Hero.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                    }
                    break;
            }
        }
    }
}