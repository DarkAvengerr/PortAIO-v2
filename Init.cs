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
                    BrianSharp.Program.Main();
                    break;
                case Champion.Ahri:
                    //DZAhri.Program.Game_OnGameLoad();
                    //EloFactory_Ahri.Program.Game_OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Akali:
                    //KoreanAkali.Program.Game_OnGameLoad();
                    //AkaliTroop.Program.Game_OnGameLoad();
                    Akali.Program.Game_OnGameLoad();
                    break;
                case Champion.Alistar:
                    ElAlistarReborn.Alistar.OnGameLoad();
                    break;
                case Champion.Amumu:
                    AmumuSharp.Program.Game_OnGameLoad();
                    break;
                case Champion.Anivia:
                    AniviaSharp.Program.Main();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Annie:
                    // KoreanAnnie.Program.Game_OnGameLoad(); - Korean Annie
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Ashe:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.AurelionSol:
                    //vAurelionSol.AurelionSol.Game_OnGameLoad();
                    //SkyLv_AurelionSol.Initialiser.Game_OnGameLoad();
                    ElAurelion_Sol.AurelionSol.OnGameLoad();
                    break;
                case Champion.Azir:
                    //Azir_Creator_of_Elo.Program.Main();
                    HeavenStrikeAzir.Program.Game_OnGameLoad();
                    break;
                case Champion.Bard:
                    //xBard.Program.Game_OnGameLoad();
                    DZBard.Program.Game_OnGameLoad();
                    break;
                case Champion.Blitzcrank:
                    //KurisuRiven.Program.Game_OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Brand:
                    //HikiCarry_Brand.Program.Game_OnGameLoad();
                    //yol0Brand.Program.Game_OnGameLoad();
                    TheBrand.Program.Main();
                    break;
                case Champion.Braum:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Caitlyn:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Cassiopeia:
                    SAutoCarry.Program.Game_OnGameLoad();
                    break;
                case Champion.Chogath:
                    //WindWalker_Cho._._.gath.Program.Game_OnGameLoad();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Corki:
                    ElCorki.Corki.Game_OnGameLoad();
                    break;
                case Champion.Darius:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Diana:
                    ElDiana.Diana.OnLoad();
                    break;
                case Champion.DrMundo:
                    Mundo.Program.Main();
                    break;
                case Champion.Draven:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Ekko:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    //EloFactory_Ekko.Program.Game_OnGameLoad();
                    break;
                case Champion.Elise:
                    //EliseGod.Program.OnGameLoad();
                    //HikiCarry_Elise.Program.Game_OnGameLoad();
                    GFUELElise.Elise.OnGameLoad();
                    break;
                case Champion.Evelynn:
                    Evelynn.Program.Game_OnGameLoad();
                    break;
                case Champion.Ezreal:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.FiddleSticks:
                    Feedlesticks.Program.Game_OnGameLoad();
                    break;
                case Champion.Fiora:
                    FioraProject.Program.Game_OnGameLoad();
                    break;
                case Champion.Fizz:
                    //ElFizz.Fizz.OnGameLoad();
                    MathFizz.Program.Game_OnGameLoad();
                    break;
                case Champion.Galio:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Gangplank:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Garen:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Gnar:
                    //Slutty_Gnar_Reworked.Gnar.OnLoad();
                    Gnar.Program.Game_OnGameLoad();
                    break;
                case Champion.Gragas:
                    GragasTheDrunkCarry.Program.Main();
                    break;
                case Champion.Graves:
                    //KurisuGraves.Program.Game_OnLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Hecarim:
                    JustHecarim.Program.OnLoad();
                    break;
                case Champion.Heimerdinger:
                    Two_Girls_One_Donger.Program.Game_OnGameLoad();
                    break;
                case Champion.Illaoi:
                    Illaoi___Tentacle_Kitty.Program.Game_OnGameLoad();
                    break;
                case Champion.Irelia:
                    //IreliaToTheChallenger.Program.Load();
                    Irelia.Irelia.Game_OnGameLoad();
                    break;
                case Champion.Janna:
                    LCS_Janna.Program.OnGameLoad();
                    break;
                case Champion.JarvanIV:
                    BrianSharp.Program.Main();
                    break;
                case Champion.Jax:
                    //NoobJaxReloaded.Program.Game_OnGameLoad();
                    JaxQx.Program.Game_OnGameLoad();
                    break;
                case Champion.Jayce:
                    //HikiCarry_Jayce___Hammer_of_Justice.Program.OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Jhin:
                    //Jhin___The_Virtuoso.Program.JhinOnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Jinx:
                    //Jinx_Genesis.Program.Game_OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Kalista:
                    //HERMES_Kalista.Program.Main();
                    //HikiCarry_Kalista.Program.Game_OnGameLoad();
                    S_Plus_Class_Kalista.Program.OnLoad();
                    break;
                case Champion.Karma:
                    //KarmaXD.Program.Game_OnGameLoad();
                    break;
                case Champion.Karthus:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Kassadin:
                    //Kassawin.Kassadin.OnLoad();
                    PainInMyKass.Program.Game_OnGameLoad();
                    break;
                case Champion.Katarina:
                    Staberina.Program.Main();
                    //ElKatarina.Program.OnLoad();
                    break;
                case Champion.Kayle:
                    SephKayle.Program.OnGameLoad();
                    break;
                case Champion.Kennen:
                    //Kennen.Program.Main();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Khazix:
                    SephKhazix.Khazix.Main();
                    break;
                case Champion.Kindred:
                    Kindred___YinYang.Program.Game_OnGameLoad();
                    break;
                case Champion.KogMaw:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Leblanc:
                    //LCS_LeBlanc.Program.OnLoad();
                    Leblanc.Leblanc.Game_OnGameLoad();
                    break;
                case Champion.LeeSin:
                    ElLeeSin.Program.Game_OnGameLoad();
                    break;
                case Champion.Leona:
                    ElEasy.Entry.OnLoad();
                    break;
                case Champion.Lissandra:
                    SephLissandra.Program.Main();
                    break;
                case Champion.Lucian:
                    //iLucian.LucianBootstrap.OnGameLoad();
                    //KoreanLucian.Program.Game_OnGameLoad();
                    //HoolaLucian.Program.OnGameLoad();
                    LCS_Lucian.Program.OnLoad();
                    break;
                case Champion.Lulu:
                    //HeavenStrikeLuLu.Program.Game_OnGameLoad();
                    LuluLicious.Program.Main();
                    break;
                case Champion.Malphite:
                    ElEasy.Entry.OnLoad();
                    break;
                case Champion.Malzahar:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Maokai:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.MasterYi:
                    //HoolaMasterYi.Program.OnGameLoad();
                    MasterSharp.Program.Main();
                    break;
                case Champion.MissFortune:
                    //Miss_Fortune.Program.OnLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Mordekaiser:
                    Mordekaiser.Program.Game_OnGameLoad();
                    break;
                case Champion.Morgana:
                    KurisuMorgana.Program.Game_OnGameLoad();
                    break;
                case Champion.Nami:
                    ElNamiBurrito.Nami.Game_OnGameLoad();
                    break;
                case Champion.Nasus:
                    ElEasy.Entry.OnLoad();
                    break;
                case Champion.Nautilus:
                    //PlebNautilus.Program.Game_OnGameLoad();
                    Nautilus_AnchorTheChallenger.program.Game_OnGameLoad();
                    break;
                case Champion.Nidalee:
                    //HeavenStrikeNidalee.Program.Game_OnGameLoad();
                    //Nechrito_Nidalee.Program.OnLoad();
                    KurisuNidalee.Program.Main();
                    break;
                case Champion.Nocturne:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Nunu:
                    LSharpNunu.Nunu.Game_OnGameLoad();
                    break;
                case Champion.Olaf:
                    Olaf.Olaf.Game_OnGameLoad();
                    break;
                case Champion.Orianna:
                    Orianna.Program.Game_OnGameLoad();
                    break;
                case Champion.Pantheon:
                    Pantheon.Program.Game_OnGameLoad();
                    break;
                case Champion.Poppy:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Quinn:
                    //GFUELQuinn.Quinn.OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Rammus:
                    BrianSharp.Program.Main();
                    break;
                case Champion.RekSai:
                    //RekSai.Program.Game_OnGameLoad();
                    //HeavenStrikeReksaj.Program.Game_OnGameLoad();
                    D_RekSai.Program.Game_OnGameLoad();
                    break;
                case Champion.Renekton:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Rengar:
                    ElRengarRevamped.Rengar.OnLoad();
                    //D_Rengar.Program.Game_OnGameLoad();
                    break;
                case Champion.Riven:
                    switch (Misc.menu.Item(Champion.Riven.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            KurisuRiven.Program.Game_OnGameLoad();
                            break;
                        case 1:
                            HoolaRiven.Program.OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Rumble:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Ryze:
                    SurvivorSeries.SurviorSeries.Main();
                    break;
                case Champion.Sejuani:
                    ElSejuani.Sejuani.OnLoad();
                    break;
                case Champion.Shaco:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Shen:
                    //Kimbaeng_Shen.Program.Game_OnGameLoad();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Shyvana:
                    //JustShyvana.Program.OnLoad();
                    //HeavenStrikeShyvana.Program.Game_OnGameLoad();
                    D_Shyvana.Program.Game_OnGameLoad();
                    break;
                case Champion.Singed:
                    //ElSinged.Singed.Game_OnGameLoad();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Sion:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Sivir:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Skarner:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Sona:
                    //VodkaSona.Program.Game_OnLoad();
                    //RoyalSona.Program.Game_OnGameLoad();
                    ElEasy.Entry.OnLoad();
                    break;
                case Champion.Soraka:
                    //Soraka_HealBot.Program.OnGameLoad();
                    //MLGSORAKA.Program.OnLoad();
                    SephSoraka.Soraka.SorakaMain();
                    break;
                case Champion.Swain:
                    //Slutty_Swain.Swain.OnLoad();
                    //The_Mocking_Swain.Program.Game_OnGameLoad();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Syndra:
                    //Hikigaya_Syndra.Program.OnLoad();
                    //SyndraL33T.Bootstrap.Main();
                    Syndra.Program.Game_OnGameLoad();
                    break;
                case Champion.TahmKench:
                    //STahmKench.Program.Main();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Taliyah:
                    TophSharp.Taliyah.OnLoad();
                    break;
                case Champion.Talon:
                    //HoolaTalon.Program.OnGameLoad();
                    GFUELTalon.Talon.OnGameLoad();
                    break;
                case Champion.Taric:
                    //PippyTaric.Program.LoadStuff();
                    SkyLv_Taric.Initialiser.Game_OnGameLoad();
                    break;
                case Champion.Teemo:
                    PandaTeemo.Program.Game_OnGameLoad();
                    break;
                case Champion.Thresh: // DONE.
                    Thresh___The_Chain_Warden.Program.Game_OnGameLoad();
                    break;
                case Champion.Tristana: // DONE.
                    //PewPewTristana.Program.OnLoad();
                    ElTristana.Tristana.OnLoad();
                    break;
                case Champion.Trundle:
                    ElTrundle.Trundle.OnLoad();
                    break;
                case Champion.Tryndamere:
                    //TheLichKing.Program.Game_OnGameLoad();
                    BrianSharp.Program.Main();
                    break;
                case Champion.TwistedFate:
                    switch (Misc.menu.Item(Champion.TwistedFate.ToString()).GetValue<StringList>().SelectedIndex)
                    {
                        case 0: // Kortatu
                            TwistedFate.Program.Game_OnGameLoad();
                            break;
                        case 1: // SharpShooter
                            SharpShooter.Program.Game_OnGameLoad();
                            break;
                        case 2:
                            //EloFactory_TwistedFate.Program.Game_OnGameLoad();
                            break;
                        case 3:
                            //Twisted_Fate___Its_all_in_the_cards.Program.Game_OnGameLoad();
                            break;
                    }
                    break;
                case Champion.Twitch:
                    //iTwitch.Program.Main();
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Udyr:
                    BrianSharp.Program.Main();
                    // LCS_Udyr.Program.OnGameLoad();
                    //EloFactory_Udyr.Program.Game_OnGameLoad();
                    break;
                case Champion.Urgot:
                    OneKeyToWin_AIO_Sebby.Program.GameOnOnGameLoad();
                    break;
                case Champion.Varus:
                    Elvarus.Varus.Game_OnGameLoad();
                    break;
                case Champion.Vayne:
                    VayneHunter_Reborn.Program.Game_OnGameLoad();
                    break;
                case Champion.Veigar:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Velkoz:
                    Velkoz.Program.Game_OnGameLoad();
                    break;
                case Champion.Vi:
                    //Vi.Vi.Game_OnGameLoad();
                    ElVi.Vi.OnLoad();
                    break;
                case Champion.Viktor:
                    // HikiCarry_Viktor.Program.Game_OnGameLoad();
                    // ViktorBadao.Program.Game_OnGameLoad();
                    PerplexedViktor.Program.Game_OnGameLoad();
                    Viktor.Program.Game_OnGameLoad();
                    break;
                case Champion.Vladimir:
                    ElVladimirReborn.Vladimir.OnLoad();
                    break;
                case Champion.Volibear:
                    //NoobVolibear.Program.Game_OnGameLoad();
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Warwick:
                    Warwick.Program.Game_OnGameLoad();
                    break;
                case Champion.MonkeyKing:
                    break;
                case Champion.Xerath:
                    //The_Slutty_Xerath.Xerath.OnLoad();
                    Xerath.Program.Game_OnGameLoad();
                    break;
                case Champion.XinZhao:
                    //Xin.Program.GameOnOnGameLoad();
                    XinZhao.Program.Game_OnGameLoad();
                    break;
                case Champion.Yasuo:
                    YasuoPro.Initalization.Main();
                    //GosuMechanicsYasuo.Program.Game_OnGameLoad();
                    break;
                case Champion.Yorick:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Zac:
                    UnderratedAIO.Program.OnGameLoad();
                    break;
                case Champion.Zed:
                    KoreanZed.Program.Game_OnGameLoad();
                    break;
                case Champion.Ziggs:
                    Ziggs.Program.Game_OnGameLoad();
                    break;
                case Champion.Zilean:
                    ElZilean.Zilean.OnGameLoad();
                    break;
                case Champion.Zyra:
                    D_Zyra.Program.Game_OnGameLoad();
                    break;
            }
        }
    }
}