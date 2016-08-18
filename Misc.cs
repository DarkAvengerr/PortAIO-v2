using EloBuddy;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortAIO.Dual_Port
{
    class Misc
    {
        public static Menu menu;

        public static void Load()
        {
            (menu = new Menu("PortAIO Misc", "PAIOMisc", true)).AddToMainMenu();

            var dualPort = new Menu("Dual-Port", "DualPAIOPort");
            menu.AddSubMenu(dualPort);

            var hasDualPort = true;

            string[] champ = new string[] { };
            switch(ObjectManager.Player.Hero)
            {
                case Champion.Aatrox:
                    champ = new string[] { "BrianSharp", "KappaSeries", "SAutoCarry" };
                    break;
                case Champion.Ahri:
                    champ = new string[] { "OKTW", "DZAhri", "EloFactory Ahri", "KappaSeries", "xSalice", "BadaoSeries", "DZAIO" };
                    break;
                case Champion.Akali:
                    champ = new string[] { "xQx Akali", "Kappa Series", "Korean Akali", "Trookali", "xSalice", "StonedSeriesAIO" };
                    break;
                case Champion.Alistar:
                    champ = new string[] { "ElAlistar", "Support Is Easy", "FreshBooster", "vSeries" };
                    break;
                case Champion.Amumu:
                    champ = new string[] { "Amumu#", "BrianSharp", "StonedSeriesAIO", "ShineAIO" };
                    break;
                case Champion.Anivia:
                    champ = new string[] { "OKTW", "Anivia#", "xSalice" };
                    break;
                case Champion.Annie:
                    champ = new string[] { "OKTW", "Korean Annie", "SharpyAIO", "Support is Easy" };
                    break;
                case Champion.Ashe:
                    champ = new string[] { "OKTW", "ProSeries", "ReformedAIO", "SharpShooter", "SurvivorSeries", "xSalice" };
                    break;
                case Champion.AurelionSol:
                    champ = new string[] { "ElAurelionSol", "SkyLv_Aurelion", "vAurelionSol" };
                    break;
                case Champion.Azir:
                    champ = new string[] { "HeavenStrike Azir", "Creator of Elo", "SAutoCarry", "xSalice" };
                    break;
                case Champion.Bard:
                    champ = new string[] { "DZBard", "DZAIO", "FreshBooster", "xBard" };
                    break;
                case Champion.Blitzcrank:
                    champ = new string[] { "OKTW", "FreshBooster", "KurisuBlitz", "SAutoCarry", "SharpShooter", "ShineAIO", "Support is Easy", "vSeries" };
                    break;
                case Champion.Brand:
                    champ = new string[] { "The Brand", "Hikicarry Brand", "OKTW", "Survivor Series", "yol0 Brand" };
                    break;
                case Champion.Braum:
                    champ = new string[] { "OKTW", "FreshBooster", "Support is Easy" };
                    break;
                case Champion.Caitlyn:
                    champ = new string[] { "OKTW", "SharpShooter" };
                    break;
                case Champion.Cassiopeia:
                    champ = new string[] { "SAutoCarry", "SFXChallenger", "SharpyAIO", "xSalice" };
                    break;
                case Champion.Chogath:
                    champ = new string[] { "UnderratedAIO", "Windwalker Cho'Gath", "xSalice" };
                    break;
                case Champion.Corki:
                    champ = new string[] { "El Corki", "ADCPackage", "D-Corki", "hikiMarksman", "OKTW", "ProSeries", "SAutoCarry", "SharpShooter", "xSalice" };
                    break;
                case Champion.Darius:
                    champ = new string[] { "OKTW", "ElEasy", "SAutoCarry" };
                    break;
                case Champion.Diana:
                    champ = new string[] { "ElDiana", "D-Diana", "ReformedAIO" };
                    break;
                case Champion.DrMundo:
                    champ = new string[] { "Hestia's Mundo", "BrianSharp", "KappaSeries", "SAutoCarry", "SharpyAIO", "StonedSeriesAIO" };
                    break;
                case Champion.Draven:
                    champ = new string[] { "OKTW", "hikiMarksman", "SharpShooter" };
                    break;
                case Champion.Ekko:
                    champ = new string[] { "OKTW", "EloFactor Ekko", "xSalice" };
                    break;
                case Champion.Elise:
                    champ = new string[] { "GFUEL Elise", "D-Elise", "EliseGod", "Hikigaya Elise" };
                    break;
                case Champion.Evelynn:
                    champ = new string[] { "Evelynn#", "OKTW", "UnderratedAIO" };
                    break;
                case Champion.Ezreal:
                    champ = new string[] { "OKTW", "ADCPackage", "D-Ezreal", "DZAIO", "hikiMarksman", "iSeriesReborn", "ProSeries", "SFXChallenger", "SharpShooter", "ShineAIO", "UnderratedAIO", "xSalice" };
                    break;
                case Champion.FiddleSticks:
                    champ = new string[] { "Feedlesticks", "Support is Easy", "vSeries" };
                    break;
                case Champion.Fiora:
                    champ = new string[] { "Project Fiora", "UnderratedAIO", "xSalice" };
                    break;
                case Champion.Fizz:
                    champ = new string[] { "Math Fizz", "ElFizz", "UnderratedAIO" };
                    break;
                // Galio - Underrated
                // Gangplank - Underrated
                // Garen - Underrated
                case Champion.Gnar:
                    champ = new string[] { "Hellsing's Gnar", "SluttyGnar" };
                    break;
                case Champion.Gragas:
                    champ = new string[] { "The Drunk Carry", "ReformedAIO", "UnderratedAIO" };
                    break;
                case Champion.Graves:
                    champ = new string[] { "OKTW", "D-Graves", "hikiMarksman", "Kurisu Graves", "SFXChallenger", "SharpShooter" };
                    break;
                case Champion.Hecarim:
                    champ = new string[] { "JustHecarim", "SharpyAIO", "UnderratedAIO" };
                    break;
                // Heimerdinger - 2Girls1Donger
                case Champion.Illaoi:
                    champ = new string[] { "Tentacle Kitty", "SharpShooter" };
                    break;
                case Champion.Irelia:
                    champ = new string[] { "Irelia II", "Irelia to the Challenger", "xSalice" };
                    break;
                case Champion.Janna:
                    champ = new string[] { "LCS Janna", "FreshBooster", "Support is Easy", "vSeries" };
                    break;
                case Champion.JarvanIV:
                    champ = new string[] { "BrianSharp", "D-Jarvan", "StonedSeries AIO" };
                    break;
                case Champion.Jax:
                    champ = new string[] { "xQx Jax", "BrianSharp", "NoobJaxReloaded", "SAutoCarry", "UnderratedAIO" };
                    break;
                case Champion.Jayce:
                    champ = new string[] { "OKTW", "Hikicarry Jayce", "xSalice" };
                    break;
                case Champion.Jhin:
                    champ = new string[] { "OKTW", "Hikigaya's Jhin", "SAutoCarry" };
                    break;
                case Champion.Jinx:
                    champ = new string[] { "OKTW", "ADCPackage", "GENESIS Jinx", "iSeriesReborn", "ProSeries", "SharpShooter", "xSalice" };
                    break;
                case Champion.Kalista:
                    champ = new string[] { "S+ Class Kalista", "DZAIO", "HERMES Kalista", "Hikicarry Kalista", "iSeriesReborn", "OKTW", "SAutoCarry", "SFXChallenger", "SharpShooter" };
                    break;
                case Champion.Karma:
                    champ = new string[] { "Kortatu's Karma", "KarmaXD", "Support is Easy", "vSeries" };
                    break;
                case Champion.Karthus:
                    champ = new string[] { "OKTW", "SharpShooter", "xSalice" };
                    break;
                case Champion.Kassadin:
                    champ = new string[] { "PainInMyKass", "SharpyAIO", "Slutty Kassadin" };
                    break;
                case Champion.Katarina:
                    champ = new string[] { "Staberina", "ElEasy", "ElSmartKatarina", "xSalice" };
                    break;
                case Champion.Kayle:
                    champ = new string[] { "SephKayle", "BrianSharp", "D-Kayle", "OKTW" };
                    break;
                case Champion.Kennen:
                    champ = new string[] { "UnderratedAIO", "BrianSharp", "Hestia's Kennen" };
                    break;
                // Kha'Zix - Seph Kha'Zix
                case Champion.Kindred:
                    champ = new string[] { "Yin & Yang", "OKTW", "SharpShooter" };
                    break;
                case Champion.KogMaw:
                    champ = new string[] { "OKTW", "D-Kog'Maw", "iSeriesReborn", "ProSeries", "SFXChallenger", "SharpShooter", "xSalice" };
                    break;
                case Champion.Leblanc:
                    champ = new string[] { "Leblanc II", "FreshBooster", "LCS Leblanc" };
                    break;
                case Champion.LeeSin:
                    champ = new string[] { "ElLeeSin", "BrianSharp", "FreshBooster" };
                    break;
                case Champion.Leona:
                    champ = new string[] { "ElEasy", "Support is Easy", "vSeries" };
                    break;
                case Champion.Lissandra:
                    champ = new string[] { "SephLissandra", "xSalice" };
                    break;
                case Champion.Lucian:
                    champ = new string[] { "LCS Lucian", "BrianSharp", "hikiMarksman", "Hoola Lucian", "iLucian", "iSeriesReborn", "KoreanLucian", "OKTW", "SAutoCarry", "SharpShooter", "xSalice" };
                    break;
                case Champion.Lulu:
                    champ = new string[] { "Lululicious", "HeavenStrikeLulu", "SharpShooter", "Support is Easy" };
                    break;
                case Champion.Lux:
                    champ = new string[] { "OKTW", "vSeries" };
                    break;
                // Malphite - ElEasy
                case Champion.Malzahar:
                    champ = new string[] { "OKTW", "SurvivorSeries" };
                    break;
                case Champion.Maokai:
                    champ = new string[] { "UnderratedAIO", "BrianSharp" };
                    break;
                case Champion.MasterYi:
                    champ = new string[] { "MasterSharp", "Hoola Yi", "SAutoCarry" };
                    break;
                case Champion.MissFortune:
                    champ = new string[] { "OKTW", "Alex's MissFortune", "D-MissFortune", "SAutoCarry", "SFXChallenger", "SharpShooter" };
                    break;
                case Champion.Mordekaiser:
                    champ = new string[] { "xQx Mordekaiser", "UnderratedAIO" };
                    break;
                case Champion.Morgana:
                    champ = new string[] { "Kurisu Morgana", "FreshBooster", "OKTW", "ShineAIO", "Support is Easy", "vSeries" };
                    break;
                case Champion.Nami:
                    champ = new string[] { "ElNami", "FreshBooster", "Support is Easy", "vSeries" };
                    break;
                case Champion.Nasus:
                    champ = new string[] { "ElEasy", "BrianSharp", "UnderratedAIO" };
                    break;
                case Champion.Nautilus:
                    champ = new string[] { "Nautilus - Danz", "PlebNautilus", "vSeries" };
                    break;
                case Champion.Nidalee:
                    champ = new string[] { "KurisuNidalee", "HeavenStrikeNidalee", "NechritoNidalee", "D-Nidalee" };
                    break;
                // Nocturne - Underrated
                case Champion.Nunu:
                    champ = new string[] { "Nunu by Alqohol", "Support Is Easy" };
                    break;
                case Champion.Olaf:
                    champ = new string[] { "Olaf is Back II", "UnderratedAIO" };
                    break;
                case Champion.Orianna:
                    champ = new string[] { "Kortatu Orianna", "DZAIO", "OKTW", "SAutoCarry", "SFXChallenger", "xSalice" };
                    break;
                case Champion.Pantheon:
                    champ = new string[] { "xQx Pantheon", "SAutoCarry" };
                    break;
                case Champion.Poppy:
                    champ = new string[] { "UnderratedAIO", "FreshBooster", "vSeries" };
                    break;
                case Champion.Quinn:
                    champ = new string[] { "OKTW", "GFUEL Quinn" };
                    break;
                // Rammus - BrianSharp
                case Champion.RekSai:
                    champ = new string[] { "D-Reksai", "HeavenStrike Rek'Sai", "Rek'Sai Winner of Fights" };
                    break;
                case Champion.Renekton:
                    champ = new string[] { "UnderratedAIO", "SharpyAIO" };
                    break;
                case Champion.Rengar:
                    champ = new string[] { "ElRengar", "D-Rengar", "SAutoCarry" };
                    break;
                case Champion.Riven:
                    champ = new string[] { "KurisuRiven", "Hoola Riven", "SAutoCarry" };
                    break;
                case Champion.Rumble:
                    champ = new string[] { "UnderratedAIO", "xSalice" };
                    break;
                case Champion.Ryze:
                    champ = new string[] { "Survivor Ryze", "BrianSharp", "FreshBooster", "ReformedAIO", "Sharpshooter", "StonedSeries AIO" };
                    break;
                case Champion.Sejuani:
                    champ = new string[] { "ElSejuani", "UnderratedAIO" };
                    break;
                // Shaco - Underrated
                case Champion.Shen:
                    champ = new string[] { "UnderratedAIO", "BrianSharp", "Kimbaeng Shen" };
                    break;
                case Champion.Shyvana:
                    champ = new string[] { "D-Shyvana", "HeavenStrike Shyvana", "JustShyvana", "SAutoCarry" };
                    break;
                case Champion.Singed:
                    champ = new string[] { "UnderratedAIO", "ElSinged" };
                    break;
                // Sion - Underrated
                case Champion.Sivir:
                    champ = new string[] { "OKTW", "DZAIO", "hikiMarksman", "ProSeries", "SFXChallenger", "SharpShooter", "ShineAIO" };
                    break;
                // Skarner - Underrated
                case Champion.Sona:
                    champ = new string[] { "ElEasy", "Royal Song of Sona", "Support is Easy", "Vodka Sona", "vSeries" };
                    break;
                case Champion.Soraka:
                    champ = new string[] { "SephSoraka", "FreshBooster", "Heal-Bot", "MLG Soraka", "Support is Easy", "vSeries" };
                    break;
                case Champion.Swain:
                    champ = new string[] { "OKTW", "SluttySwain", "The Mocking Swain" };
                    break;
                case Champion.Syndra:
                    champ = new string[] { "Syndra by Kortatu", "BadaoSeries", "ElEasy", "Hikigaya Syndra", "OKTW", "Syndra by L33T", "vSeries", "xSalice" };
                    break;
                case Champion.TahmKench:
                    champ = new string[] { "UnderratedAIO", "FreshBooster", "STahmKench", "vSeries" };
                    break;
                // Taliyah - Toph#
                case Champion.Talon:
                    champ = new string[] { "GFUEL Talon", "Hoola Talon" };
                    break;
                case Champion.Taric:
                    champ = new string[] { "SkyLv_Taric", "ElEasy", "PippyTaric", "Support is Easy", "vSeries" };
                    break;
                case Champion.Teemo:
                    champ = new string[] { "PandaTeemo", "SharpShooter" };
                    break;
                case Champion.Thresh:
                    champ = new string[] { "Chain Warden", "FreshBooster", "OKTW", "Support is Easy", "vSeries" };
                    break;
                case Champion.Tristana:
                    champ = new string[] { "ElTristana", "ADCPackage", "D-Tristana", "iSeriesReborn", "OKTW", "PewPewTristana", "ProSeries", "SharpShooter" };
                    break;
                case Champion.Trundle:
                    champ = new string[] { "ElTrundle", "DZAIO", "UnderratedAIO", "vSeries" };
                    break;
                case Champion.Tryndamere:
                    champ = new string[] { "BrianSharp", "The Lich King", "UnderratedAIO" };
                    break;
                case Champion.TwistedFate:
                    champ = new string[] { "TwistedFate by Kortatu", "SharpShooter", "BadaoSeries", "EloFactory TF", "OKTW", "SAutoCarry", "SFXChallenger", "Twisted Fate - Danz" };
                    break;
                case Champion.Twitch:
                    champ = new string[] { "OKTW", "iSeriesReborn", "iTwitch 2.0", "SAutoCarry", "SharpShooter" };
                    break;
                case Champion.Udyr:
                    champ = new string[] { "BrianSharp", "D-Udyr", "EloFactory Udyr", "LCS Udyr", "UnderratedAIO" };
                    break;
                case Champion.Urgot:
                    champ = new string[] { "OKTW", "xSalice" };
                    break;
                case Champion.Varus:
                    champ = new string[] { "ElVarus", "OKTW", "SFXChallenger", "SharpShooter" };
                    break;
                case Champion.Vayne:
                    champ = new string[] { "VayneHunterReborn", "hikiMarksman", "iSeriesReborn", "OKTW", "SAutoCarry", "SharpShooter", "xSalice" };
                    break;
                case Champion.Veigar:
                    champ = new string[] { "UnderratedAIO", "DZAIO", "FreshBooster", "SAutoCarry" };
                    break;
                case Champion.Velkoz:
                    champ = new string[] { "Vel'Koz by Kortatu", "OKTW" };
                    break;
                case Champion.Vi:
                    champ = new string[] { "ElVi", "xQx Vi" };
                    break;
                case Champion.Viktor:
                    champ = new string[] { "TRUSt in my Viktor", "Hikicarry Viktor", "Perplexed Viktor", "SAutoCarry", "SFXChallenger", "Badao's Viktor", "xSalice" };
                    break;
                case Champion.Vladimir:
                    champ = new string[] { "ElVladimir", "DZAIO", "SFXChallenger", "xSalice" };
                    break;
                case Champion.Volibear:
                    champ = new string[] { "UnderratedAIO", "KappaSeries", "NoobVolibear", "StonedSeries AIO" };
                    break;
                case Champion.Warwick:
                    champ = new string[] { "The Blood Hunter", "BrianSharp", "D-Warwick", "DZAIO" };
                    break;
                // Wukong - Underrated
                case Champion.Xerath:
                    champ = new string[] { "Kortatu's Xerath", "OKTW", "SluttyXerath" };
                    break;
                case Champion.XinZhao:
                    champ = new string[] { "xQx XinZhao", "BrianSharp", "XinZhao God" };
                    break;
                case Champion.Yasuo:
                    champ = new string[] { "YasuoPro", "BrianSharp", "GosuMechanics" };
                    break;
                // Yorick - Underrated
                // Zac - Underrated
                case Champion.Zed:
                    champ = new string[] { "Korean Zed", "SharpyAIO" };
                    break;
                // Ziggs - Kortatu
                case Champion.Zilean:
                    champ = new string[] { "ElZilean", "Support is Easy" };
                    break;
                case Champion.Zyra:
                    champ = new string[] { "D-Zyra", "Support is Easy", "xSalice" };
                    break;
                default:
                    hasDualPort = false;
                    dualPort.AddItem(new MenuItem("info1", "There are no dual-port for this champion."));
                    dualPort.AddItem(new MenuItem("info2", "Feel free to request one."));
                    break;
            }

            if (hasDualPort)
            {
                dualPort.AddItem(new MenuItem(ObjectManager.Player.Hero.ToString(), "Which dual-port?").SetValue(new StringList(champ)));
            }

            var autoPlay = new Menu("Auto Play", "PortAIOAUTOPLAY");
            autoPlay.AddItem(new MenuItem("AutoPlay", "Enable AutoPlay?").SetValue(false));
            autoPlay.AddItem(new MenuItem("selectAutoPlay", "Which AutoPlay?").SetValue(new StringList(new[] { "AramDETFull", "AutoJungle" })));
            menu.AddSubMenu(autoPlay);

            var utility = new Menu("Utilities", "Utilitiesports");
            utility.AddItem(new MenuItem("enableActivator", "Enable Activator?").SetValue(false));
            utility.AddItem(new MenuItem("Activator", "Which Activator?").SetValue(new StringList(new[] { "ElUtilitySuite", "Activator#" })));

            utility.AddItem(new MenuItem("enableTracker", "Enable Tracker?").SetValue(false));
            utility.AddItem(new MenuItem("Tracker", "Which Tracker?").SetValue(new StringList(new[] { "SFXUtility", "ShadowTracker" })));

            utility.AddItem(new MenuItem("enableEvade", "Enable Evade?").SetValue(false));
            utility.AddItem(new MenuItem("Evade", "Which Evade?").SetValue(new StringList(new[] { "EzEvade", "Evade" })));

            utility.AddItem(new MenuItem("enableHuman", "Enable Humanizer?").SetValue(false));
            utility.AddItem(new MenuItem("Humanizer", "Which Humanizer?").SetValue(new StringList(new[] { "Humanizer#", "Sebby Ban Wars" })));
            menu.AddSubMenu(utility);

            menu.AddItem(new MenuItem("UtilityOnly", "Utility Only?").SetValue(false));
            menu.AddItem(new MenuItem("ChampsOnly", "Champs Only?").SetValue(false));
        }
    }
}
