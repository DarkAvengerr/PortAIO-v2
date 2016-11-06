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
            switch (ObjectManager.Player.Hero)
            {
                case EloBuddy.Champion.Aatrox:
                    champ = new string[] { "BrianSharp", "KappaSeries", "SAutoCarry", "NoobAatrox" };
                    break;
                case EloBuddy.Champion.Ahri:
                    champ = new string[] { "OKTW", "DZAhri", "EloFactory Ahri", "KappaSeries", "xSalice", "BadaoSeries", "DZAIO", "M1D 0R F33D", "AhriSharp", "[SDK] Flowers' Series", "Babehri", "EasyAhri", "SenseAhri" };
                    break;
                case EloBuddy.Champion.Akali:
                    champ = new string[] { "xQx Akali", "Kappa Series", "Korean Akali", "Trookali", "xSalice", "StonedSeriesAIO", "M1D 0R F33D", "[SDK] ExorAIO", "[SDK] Flowers' Akali", "[SDK] Flowers' Series", "[SDK] TroopAIO", "Bloodmoon Akali", "Royal Rapist Akali", "sAIO" };
                    break;
                case EloBuddy.Champion.Alistar:
                    champ = new string[] { "ElAlistar", "Support Is Easy", "FreshBooster", "vSeries", "SkyAlistar" };
                    break;
                case EloBuddy.Champion.Amumu:
                    champ = new string[] { "Amumu#", "BrianSharp", "StonedSeriesAIO", "ShineAIO", "[SDK] ExorAIO", "DJ Amumu", "MasterOfSadness" };
                    break;
                case EloBuddy.Champion.Anivia:
                    champ = new string[] { "OKTW", "Anivia#", "xSalice", "[SDK] ExorAIO" };
                    break;
                case EloBuddy.Champion.Annie:
                    champ = new string[] { "OKTW", "Korean Annie", "SharpyAIO", "Support is Easy", "EloFactory Annie", "Flowers' Annie", "OAnnie" };
                    break;
                case EloBuddy.Champion.Ashe:
                    champ = new string[] { "OKTW", "ProSeries", "ReformedAIO", "SharpShooter", "[SA] SurvivorSeries", "xSalice", "Marksman#", "[SDK] The Queen of the Ice", "[SDK] ChallengerSeriesAIO", "[SDK] Dicaste's Ashe", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] xcsoft's Ashe", "Ashe#", "CarryAshe", "SNAshe", "ProjectGeass", "Hikicarry ADC", "Flowers' ADC Series", "SurvivorSeriesAIO" };
                    break;
                case EloBuddy.Champion.AurelionSol:
                    champ = new string[] { "ElAurelionSol", "SkyLv_Aurelion", "vAurelionSol", "Aurelion Sol As The Star Forger", "Flowers' AurelionSol", "Badao AurelionSol" };
                    break;
                case EloBuddy.Champion.Azir:
                    champ = new string[] { "HeavenStrike Azir", "Creator of Elo", "SAutoCarry", "xSalice", "Azir by Kortatu", "AzirSharp", "Night Stalker Azir" };
                    break;
                case EloBuddy.Champion.Bard:
                    champ = new string[] { "DZBard", "DZAIO", "FreshBooster", "xBard", "[SDK] ChallengerSeriesAIO", "BreakingBard", "DesomondBard" };
                    break;
                case EloBuddy.Champion.Blitzcrank:
                    champ = new string[] { "OKTW", "FreshBooster", "KurisuBlitz", "SAutoCarry", "SharpShooter", "ShineAIO", "Support is Easy", "vSeries", "[SDK] Flowers' Series", "[SDK] xcsoft's Blitzcrank", "JustBlitzcrank", "MoonBlitz", "SluttyBlitz", "sAIO" };
                    break;
                case EloBuddy.Champion.Brand:
                    champ = new string[] { "The Brand", "Hikicarry Brand", "OKTW", "[SA] SurvivorSeries", "yol0 Brand", "DevBrand", "Flowers' Brand", "Kimbaeng Brand", "sBrand", "SN Brand", "SurvivorSeriesAIO" };
                    break;
                case EloBuddy.Champion.Braum:
                    champ = new string[] { "OKTW", "FreshBooster", "Support is Easy" };
                    break;
                case EloBuddy.Champion.Caitlyn:
                    champ = new string[] { "OKTW", "SharpShooter", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] ExorAIO", "SluttyCaitlyn", "Hikicarry ADC", "Flowers' ADC Series", "ReformedAIO" };
                    break;
                case EloBuddy.Champion.Cassiopeia:
                    champ = new string[] { "SAutoCarry", "SFXChallenger", "SharpyAIO", "xSalice", "TheCassiopeia", "[SDK] ExorAIO", "Eat My Cass", "mztikk's Cass", "RiseOfThePython", "sAIO" };
                    break;
                case EloBuddy.Champion.Chogath:
                    champ = new string[] { "UnderratedAIO", "Windwalker Cho'Gath", "xSalice", "Troop'Gath" };
                    break;
                case EloBuddy.Champion.Corki:
                    champ = new string[] { "El Corki", "ADCPackage", "D-Corki", "hikiMarksman", "OKTW", "ProSeries", "SAutoCarry", "SharpShooter", "xSalice", "Marksman#", "[SDK] ExorAIO", "EasyCorki", "jhkCorki", "LeCorki", "PewPewCorki", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Darius:
                    champ = new string[] { "OKTW", "ElEasy", "SAutoCarry", "[SDK] ExorAIO", "[SDK] Flowers' Series", "Darius#", "KurisuDarius", "ODarius", "PerfectDarius", "sAIO" };
                    break;
                case EloBuddy.Champion.Diana:
                    champ = new string[] { "ElDiana", "D-Diana", "ReformedAIO", "[SDK] ExorAIO", "Diana MasterRace", "MoonDiana", "NechritoDiana", "[SDK] Tc_SDKexAIO", "Flowers' Diana", "ElDiana Revamped" };
                    break;
                case EloBuddy.Champion.DrMundo:
                    champ = new string[] { "Hestia's Mundo", "BrianSharp", "KappaSeries", "SAutoCarry", "SharpyAIO", "StonedSeriesAIO", "[SDK] ExorAIO", "[SDK] Valvrave#", "MundoSharpy" };
                    break;
                case EloBuddy.Champion.Draven:
                    champ = new string[] { "OKTW", "hikiMarksman", "SharpShooter", "Marksman#", "M00N Draven", "[SDK] ExorAIO", "[SDK] Tyler1.exe", "badaoDraven", "myWorld AIO", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Ekko:
                    champ = new string[] { "OKTW", "EloFactory Ekko", "xSalice", "Ekko Master of Time", "Ekko The Boy Who Shattered Time", "EkkoGod", "ElEkko", "HikiCarry Ekko", "TheEkko" };
                    break;
                case EloBuddy.Champion.Elise:
                    champ = new string[] { "GFUEL Elise", "D-Elise", "EliseGod", "Hikigaya Elise", "Sense Elise", "SephElise", "BadaoElise" };
                    break;
                case EloBuddy.Champion.Evelynn:
                    champ = new string[] { "Evelynn#", "OKTW", "UnderratedAIO", "[SDK] ExorAIO", "[SDK] TroopAIO", "JustEvelynn", "SkyLv Evelynn" };
                    break;
                case EloBuddy.Champion.Ezreal:
                    champ = new string[] { "OKTW", "ADCPackage", "D-Ezreal", "DZAIO", "hikiMarksman", "iSeriesReborn", "ProSeries", "SFXChallenger", "SharpShooter", "ShineAIO", "UnderratedAIO", "xSalice", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] DarkChild's Ezreal", "[SDK] ExorAIO", "[SDK] Flowers' Series", "EasyEzreal", "Ezreal - Prodigal Explorer", "Ezreal - the Dream Chaser", "iDzEzreal", "iEzrealReworked", "PerplexedEzreal", "myWorld AIO", "[SDK] Tc_SDKexAIO", "ProjectGeass", "Hikicarry ADC", "Flowers' ADC Series", "ReformedAIO", "HandicAPEzreal" };
                    break;
                case EloBuddy.Champion.FiddleSticks:
                    champ = new string[] { "Feedlesticks", "Support is Easy", "vSeries" };
                    break;
                case EloBuddy.Champion.Fiora:
                    champ = new string[] { "Project Fiora", "UnderratedAIO", "jesuisFiora" };
                    break;
                case EloBuddy.Champion.Fizz:
                    champ = new string[] { "Math Fizz", "ElFizz", "UnderratedAIO", "HeavenStrikeFizz", "NoobFizz", "OneKeyToFish" };
                    break;
                case EloBuddy.Champion.Galio:
                    champ = new string[] { "UnderratedAIO", "Desomond Galio", "Galio#" };
                    break;
                case EloBuddy.Champion.Gangplank:
                    champ = new string[] { "UnderratedAIO", "Badao Gangplank", "Bangplank", "BePlank", "e.Motion Gangplank" };
                    break;
                case EloBuddy.Champion.Garen:
                    champ = new string[] { "UnderratedAIO", "TheGaren", "TroopGaren", "yol0 Garen" };
                    break;
                case EloBuddy.Champion.Gnar:
                    champ = new string[] { "Hellsing's Gnar", "SluttyGnar", "Marksman#", "hGnar", "ReformedAIO" };
                    break;
                case EloBuddy.Champion.Gragas:
                    champ = new string[] { "The Drunk Carry", "ReformedAIO", "UnderratedAIO", "LadyGragas", "NechritoGragas", "OriginalGragas" };
                    break;
                case EloBuddy.Champion.Graves:
                    champ = new string[] { "OKTW", "D-Graves", "hikiMarksman", "Kurisu Graves", "SFXChallenger", "SharpShooter", "Marksman#", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] VSTGraves", "EasyGraves", "BadaoGraves", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Hecarim:
                    champ = new string[] { "JustHecarim", "SharpyAIO", "UnderratedAIO", "[SDK] [SBTW] Hecarim", "[SDK] Flowers' Series", "Herrari 477 GTB", "Ponycopter" };
                    break;
                case EloBuddy.Champion.Heimerdinger:
                    champ = new string[] { "2Girls1Donger", "TheDonger" };
                    break;
                case EloBuddy.Champion.Illaoi:
                    champ = new string[] { "Tentacle Kitty", "SharpShooter", "[SDK] Flowers' Series", "[SDK] Kraken Priestess", "IllaoiSOH", "TentacleBabeIllaoi" };
                    break;
                case EloBuddy.Champion.Irelia:
                    champ = new string[] { "Irelia II", "Irelia to the Challenger", "xSalice", "[SDK] ChallengerSeriesAIO", "Irelia God", "Irelia Reloaded", "Rethought Irelia", "SluttyIrelia", "[SA] SurvivorSeries Irelia", "SurvivorSeriesAIO" };
                    break;
                case EloBuddy.Champion.Ivern:
                    champ = new string[] { "UnderratedAIO", "SSIvern" };
                    break;
                case EloBuddy.Champion.Janna:
                    champ = new string[] { "LCS Janna", "FreshBooster", "Support is Easy", "vSeries" };
                    break;
                case EloBuddy.Champion.JarvanIV:
                    champ = new string[] { "BrianSharp", "D-Jarvan", "StonedSeries AIO", "J4 Helper" };
                    break;
                case EloBuddy.Champion.Jax:
                    champ = new string[] { "xQx Jax", "BrianSharp", "NoobJaxReloaded", "SAutoCarry", "UnderratedAIO", "[SDK] ExorAIO", "SkyLv Jax" };
                    break;
                case EloBuddy.Champion.Jayce:
                    champ = new string[] { "OKTW", "Hikicarry Jayce", "xSalice", "AJayce", "JayceSharpV2" };
                    break;
                case EloBuddy.Champion.Jhin:
                    champ = new string[] { "OKTW", "Hikigaya's Jhin", "SAutoCarry", "Marksman#", "[SDK] ExorAIO", "[SDK] hJhin", "Jhin as the Virtuoso", "BadaoJhin", "[SDK] Tc_SDKexAIO", "Flowers' Jhin", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Jinx:
                    champ = new string[] { "OKTW", "ADCPackage", "GENESIS Jinx", "iSeriesReborn", "ProSeries", "SharpShooter", "xSalice", "Marksman#", "[SDK] ExorAIO", "CjShu Jinx", "EasyJinx", "EloFactory Jinx", "GenerationJinx", "PennyJinxReborn", "myWorld AIO", "[SDK] Tc_SDKexAIO", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Kalista:
                    champ = new string[] { "S+ Class Kalista", "DZAIO", "HERMES Kalista", "Hikicarry Kalista", "iSeriesReborn", "OKTW", "SAutoCarry", "SFXChallenger", "SharpShooter", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] ExorAIO", "[SDK] xcsoft's Kalista", "DonguKalista", "EasyKalista", "ElKalista", "iKalista", "iKalista:Reborn", "Kalima", "KAPPALISTAXD", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Karma:
                    champ = new string[] { "Kortatu's Karma", "KarmaXD", "Support is Easy", "vSeries", "[SDK] Karma Never Falter", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] SpiritKarma", "Karma - the Enlightened One" };
                    break;
                case EloBuddy.Champion.Karthus:
                    champ = new string[] { "OKTW", "SharpShooter", "xSalice", "[SDK] RAREKarthus", "Karthus#", "KimbaengKarthus", "SNKarthus", "XDSharpAIO", "[SDK] ExorAIO" };
                    break;
                case EloBuddy.Champion.Kassadin:
                    champ = new string[] { "PainInMyKass", "SharpyAIO", "Slutty Kassadin", "[SDK] PreservedKassadin", "Kassadin the Harbinger", "KicKassadin" };
                    break;
                case EloBuddy.Champion.Katarina:
                    champ = new string[] { "Staberina", "ElEasy", "ElSmartKatarina", "xSalice", "e.Motion Katarina", "EasyCarry Katarina", "JustKatarina", "sKatarina", "SluttyKatarina", "sAIO" };
                    break;
                case EloBuddy.Champion.Kayle:
                    champ = new string[] { "SephKayle", "BrianSharp", "D-Kayle", "OKTW", "[SDK] ChallengerSeriesAIO", "Hikicarry Kayle", "LeKayle", "Roach's Kayle" };
                    break;
                case EloBuddy.Champion.Kennen:
                    champ = new string[] { "UnderratedAIO", "BrianSharp", "Hestia's Kennen", "[SDK] Valvrave#" };
                    break;
                case EloBuddy.Champion.Khazix:
                    champ = new string[] { "Seph Kha'Zix", "KhaZix#", "SurvivorSeries Kha'Zix" };
                    break;
                case EloBuddy.Champion.Kindred:
                    champ = new string[] { "Yin & Yang", "OKTW", "SharpShooter", "Marksman#", "KindredSpirits", "SluttyKindred", "Flowers' ADC Series" };
                    break;
                // Kled - HikiKled
                case EloBuddy.Champion.KogMaw:
                    champ = new string[] { "OKTW", "D-Kog'Maw", "iSeriesReborn", "ProSeries", "SFXChallenger", "SharpShooter", "xSalice", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] ExorAIO", "EasyKogMaw", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Leblanc:
                    champ = new string[] { "Leblanc II", "FreshBooster", "LCS Leblanc", "M1D 0R F33D", "BadaoLeBlanc", "PopBlanc" };
                    break;
                case EloBuddy.Champion.LeeSin:
                    champ = new string[] { "ElLeeSin", "BrianSharp", "FreshBooster", "Hikicarry LeeSin", "Lee is Back", "Slutty LeeSin", "[SDK] Valvrave#", "yol0LeeSin", "[SDK] Tc_SDKexAIO" };
                    break;
                case EloBuddy.Champion.Leona:
                    champ = new string[] { "ElEasy", "Support is Easy", "vSeries", "SethLeona", "Troopeona", "sAIO" };
                    break;
                case EloBuddy.Champion.Lissandra:
                    champ = new string[] { "SephLissandra", "xSalice", "Lissandra the Ice Goddess" };
                    break;
                case EloBuddy.Champion.Lucian:
                    champ = new string[] { "LCS Lucian", "BrianSharp", "hikiMarksman", "Hoola Lucian", "iLucian", "iSeriesReborn", "KoreanLucian", "OKTW", "SAutoCarry", "SharpShooter", "xSalice", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] ExorAIO", "D_Lucian", "FuckingLucianReborn", "SluttyLucian", "Hikicarry ADC", "Flowers' ADC Series", "ReformedAIO" };
                    break;
                case EloBuddy.Champion.Lulu:
                    champ = new string[] { "Lululicious", "HeavenStrikeLulu", "SharpShooter", "Support is Easy", "Lulu & Pix", "Lulu#", "SethLulu" };
                    break;
                case EloBuddy.Champion.Lux:
                    champ = new string[] { "OKTW", "vSeries", "M1D 0R F33D", "M00N Lux", "[SDK] ExorAIO", "CheerleaderLux", "ElLux", "Hikigaya Lux", "SephLux" };
                    break;
                case EloBuddy.Champion.Malphite:
                    champ = new string[] { "ElEasy", "JustMalphite", "SephMalphite" };
                    break;
                case EloBuddy.Champion.Malzahar:
                    champ = new string[] { "OKTW", "[SA] SurvivorSeries", "M1D 0R F33D", "NoobMalzahar", "SurvivorSeriesAIO" };
                    break;
                case EloBuddy.Champion.Maokai:
                    champ = new string[] { "UnderratedAIO", "BrianSharp", "JustMaokai" };
                    break;
                case EloBuddy.Champion.MasterYi:
                    champ = new string[] { "MasterSharp", "Hoola Yi", "SAutoCarry", "MasterYi by Prunes", "xQx Yi", "Yi by Crisdmc", "[SDK] ChallengerYi" };
                    break;
                case EloBuddy.Champion.MissFortune:
                    champ = new string[] { "OKTW", "Alex's MissFortune", "D-MissFortune", "SAutoCarry", "SFXChallenger", "SharpShooter", "Marksman#", "[SDK] ExorAIO", "BadaoMissfortune", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Mordekaiser:
                    champ = new string[] { "xQx Mordekaiser", "UnderratedAIO" };
                    break;
                case EloBuddy.Champion.Morgana:
                    champ = new string[] { "Kurisu Morgana", "FreshBooster", "OKTW", "ShineAIO", "Support is Easy", "vSeries", "[SDK] Flowers' Series" };
                    break;
                case EloBuddy.Champion.Nami:
                    champ = new string[] { "ElNami", "FreshBooster", "Support is Easy", "vSeries", "ElNamiRevamped" };
                    break;
                case EloBuddy.Champion.Nasus:
                    champ = new string[] { "ElEasy", "BrianSharp", "UnderratedAIO", "Nasus the Crazy Dog", "Nasus the Lumber Jack", "sAIO" };
                    break;
                case EloBuddy.Champion.Nautilus:
                    champ = new string[] { "Nautilus - Danz", "PlebNautilus", "vSeries", "[SDK] ExorAIO", "Hestia's Nautlius", "JustNautilus", "Nautilus - the Freelo Titan" };
                    break;
                case EloBuddy.Champion.Nidalee:
                    champ = new string[] { "KurisuNidalee", "HeavenStrikeNidalee", "NechritoNidalee", "D-Nidalee", "Flowers' Nidalee", "Nidalee - the Beastial Huntress" };
                    break;
                case EloBuddy.Champion.Nocturne:
                    champ = new string[] { "UnderratedAIO", "xQx Nocturne", "[SDK] ExorAIO" };
                    break;
                case EloBuddy.Champion.Nunu:
                    champ = new string[] { "Nunu by Alqohol", "Support Is Easy", "[SDK] ExorAIO" };
                    break;
                case EloBuddy.Champion.Olaf:
                    champ = new string[] { "Olaf is Back II", "UnderratedAIO", "[SDK] ExorAIO", "JustOlaf - Reborn", "Korean Olaf" };
                    break;
                case EloBuddy.Champion.Orianna:
                    champ = new string[] { "Kortatu Orianna", "DZAIO", "OKTW", "SAutoCarry", "SFXChallenger", "xSalice", "[SDK] ExorAIO", "Orianna - the Ruler of Ball", "Orianna by Trelli", "Orianna Grande", "Midlane#" };
                    break;
                case EloBuddy.Champion.Pantheon:
                    champ = new string[] { "xQx Pantheon", "SAutoCarry", "[SDK] ExorAIO", "NoobPantheon", "Pantheon mztikk's", "Roach's Pantheon" };
                    break;
                case EloBuddy.Champion.Poppy:
                    champ = new string[] { "UnderratedAIO", "FreshBooster", "vSeries", "BadaoPoppy" };
                    break;
                case EloBuddy.Champion.Quinn:
                    champ = new string[] { "OKTW", "GFUEL Quinn", "Marksman#", "[SDK] ExorAIO", "[SDK] Tc_SDKexAIO", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Rammus:
                    champ = new string[] { "BrianSharp", "Rammus is OK" };
                    break;
                case EloBuddy.Champion.RekSai:
                    champ = new string[] { "D-Reksai", "HeavenStrike Rek'Sai", "Rek'Sai Winner of Fights" };
                    break;
                case EloBuddy.Champion.Renekton:
                    champ = new string[] { "UnderratedAIO", "SharpyAIO", "[SDK] ExorAIO", "NoobRenekton", "sAIO" };
                    break;
                case EloBuddy.Champion.Rengar:
                    champ = new string[] { "ElRengar : Revamped", "D-Rengar", "SAutoCarry", "[SDK] Pridestalker Rengar", "Badao Rengar", "Experimental ElRengar", "HoolaRengar", "NechritoRengar" };
                    break;
                case EloBuddy.Champion.Riven:
                    champ = new string[] { "KurisuRiven", "Hoola Riven", "SAutoCarry", "Nechrito Riven", "[SDK] Flowers' Series", "[SDK] ReforgedRiven", "EasyPeasyRivenSqueezy", "EloFactory Riven", "Flowers' Riven", "HeavenStrikeRiven", "RivenSharpV2", "yol0Riven", "RivenToTheChallenger" };
                    break;
                case EloBuddy.Champion.Rumble:
                    champ = new string[] { "UnderratedAIO", "xSalice", "ElRumble" };
                    break;
                case EloBuddy.Champion.Ryze:
                    champ = new string[] { "[SA] SurvivorSeries", "BrianSharp", "FreshBooster", "Sharpshooter", "StonedSeries AIO", "[SDK] ArcaneRyze", "[SDK] EvictRyze", "[SDK] ExorAIO", "[SDK] Flowers' Series", "BurstRyze", "HeavenStrikeRyze", "JustRyze", "Ryze#", "SluttyRyze", "TRUSt in my Ryze", "sAIO", "SurvivorSeriesAIO" };
                    break;
                case EloBuddy.Champion.Sejuani:
                    champ = new string[] { "ElSejuani", "UnderratedAIO" };
                    break;
                case EloBuddy.Champion.Shaco:
                    champ = new string[] { "UnderratedAIO", "Ch3wyM00N Shaco" };
                    break;
                case EloBuddy.Champion.Shen:
                    champ = new string[] { "UnderratedAIO", "BrianSharp", "Kimbaeng Shen", "Badao Shen" };
                    break;
                case EloBuddy.Champion.Shyvana:
                    champ = new string[] { "D-Shyvana", "HeavenStrike Shyvana", "JustShyvana", "SAutoCarry" };
                    break;
                case EloBuddy.Champion.Singed:
                    champ = new string[] { "UnderratedAIO", "ElSinged" };
                    break;
                case EloBuddy.Champion.Sion:
                    champ = new string[] { "UnderratedAIO", "SimpleSion" };
                    break;
                case EloBuddy.Champion.Sivir:
                    champ = new string[] { "OKTW", "DZAIO", "hikiMarksman", "ProSeries", "SFXChallenger", "SharpShooter", "ShineAIO", "Marksman#", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] xcsoft's Sivir", "HeavenStrikeSivir", "iSivir", "JustSivir", "KurisuSivir", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Skarner:
                    champ = new string[] { "UnderratedAIO", "kSkarner", "SneakySkarner" };
                    break;
                case EloBuddy.Champion.Sona:
                    champ = new string[] { "ElEasy", "Royal Song of Sona", "Support is Easy", "Vodka Sona", "vSeries", "[SDK] ExorAIO", "mztikk's Sona" };
                    break;
                case EloBuddy.Champion.Soraka:
                    champ = new string[] { "SephSoraka", "FreshBooster", "Heal-Bot", "MLG Soraka", "Support is Easy", "vSeries", "[SDK] ChallengerSeriesAIO", "Sophie's Soraka", "Soraka#", "SorakaToTheChallenger" };
                    break;
                case EloBuddy.Champion.Swain:
                    champ = new string[] { "OKTW", "SluttySwain", "The Mocking Swain", "xQx Swain" };
                    break;
                case EloBuddy.Champion.Syndra:
                    champ = new string[] { "Syndra by Kortatu", "BadaoSeries", "Hikigaya Syndra", "OKTW", "Syndra by L33T", "vSeries", "xSalice", "SephSyndra", "Syndra - The Dark Sovereign" };
                    break;
                case EloBuddy.Champion.TahmKench:
                    champ = new string[] { "UnderratedAIO", "FreshBooster", "STahmKench", "vSeries", "Hahaha's Tahm Kench", "ElTahmKench" };
                    break;
                case EloBuddy.Champion.Taliyah:
                    champ = new string[] { "Toph#", "[SDK] ExorAIO", "[SDK] StoneWeaver" };
                    break;
                case EloBuddy.Champion.Talon:
                    champ = new string[] { "GFUEL Talon", "Hoola Talon", "Badao Talon", "ElTalon", "HeavenStrikeTalon", "MistakenTalon", "TrooplonRewritten", "sAIO", "Flowers' Talon" };
                    break;
                case EloBuddy.Champion.Taric:
                    champ = new string[] { "SkyLv_Taric", "ElEasy", "PippyTaric", "Support is Easy", "vSeries" };
                    break;
                case EloBuddy.Champion.Teemo:
                    champ = new string[] { "PandaTeemo", "SharpShooter", "Marksman#", "[SDK] ChallengerSeriesAIO", "[SDK] SwiftlyTeemo", "NoobTeemo", "TSM_Teemo", "[SDK] Tc_SDKexAIO" };
                    break;
                case EloBuddy.Champion.Thresh:
                    champ = new string[] { "Danz - Chain Warden", "FreshBooster", "OKTW", "Support is Easy", "vSeries", "Dark Star Thresh", "SluttyThresh", "Thresh as The Chain Warden", "Thresh - Catch Fish", "Thresh - the Ruler of the Soul", "Thresh the Flay Maker", "yol0 Thresh" };
                    break;
                case EloBuddy.Champion.Tristana:
                    champ = new string[] { "ElTristana", "ADCPackage", "D-Tristana", "iSeriesReborn", "OKTW", "PewPewTristana", "ProSeries", "SharpShooter", "Marksman#", "[SDK] ExorAIO", "[SDK] Flowers' Series", "Flowers' Tristana", "Geass Tristana", "SkyLV Tristana", "Tristana#", "TrooperTristana", "ProjectGeass", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Trundle:
                    champ = new string[] { "ElTrundle", "DZAIO", "UnderratedAIO", "vSeries", "[SDK] xD Trundle", "FastTrundle", "JustTrundle" };
                    break;
                case EloBuddy.Champion.Tryndamere:
                    champ = new string[] { "BrianSharp", "The Lich King", "UnderratedAIO", "[SDK] ExorAIO", "HaxDeTryndamere" };
                    break;
                case EloBuddy.Champion.TwistedFate:
                    champ = new string[] { "TwistedFate by Kortatu", "SharpShooter", "BadaoSeries", "EloFactory TF", "OKTW", "SAutoCarry", "SFXChallenger", "Twisted Fate - Danz", "[SDK] Flowers' Series", "[SDK] RARETwistedFate", "Diabath's TwistedFate", "Flowers' TwistedFate", "mztikk's TwistedFate" };
                    break;
                case EloBuddy.Champion.Twitch:
                    champ = new string[] { "OKTW", "iSeriesReborn", "iTwitch 2.0", "SAutoCarry", "SharpShooter", "Marksman#", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] InfectedTwitch", "Flowers' Twitch", "NechritoTwitch", "SNTwitch", "theobjops's Twitch", "TheTwitch", "Twitch#", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Udyr:
                    champ = new string[] { "BrianSharp", "D-Udyr", "EloFactory Udyr", "LCS Udyr", "UnderratedAIO", "[SDK] ExorAIO", "NoobUdyr" };
                    break;
                case EloBuddy.Champion.Urgot:
                    champ = new string[] { "OKTW", "xSalice", "Marksman#", "[SDK] Discaste's Urgot", "[SDK] TroopAIO", "TUrgot", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Varus:
                    champ = new string[] { "ElVarus", "OKTW", "SFXChallenger", "SharpShooter", "Marksman#", "Varus God", "Hikicarry ADC", "Flowers' ADC Series", "ElVarus Revamped" };
                    break;
                case EloBuddy.Champion.Vayne:
                    champ = new string[] { "VayneHunterReborn", "hikiMarksman", "iSeriesReborn", "OKTW", "SAutoCarry", "SharpShooter", "xSalice", "Marksman#", "hi im gosu", "[SDK] ChallengerSeriesAIO", "[SDK] ExorAIO", "[SDK] Flowers' Series", "[SDK] hVayne", "HikiCarry Vayne Masterrace", "PRADA Vayne", "SOLO Vayne", "VayneGodMode", "Hikicarry ADC", "Flowers' ADC Series" };
                    break;
                case EloBuddy.Champion.Veigar:
                    champ = new string[] { "UnderratedAIO", "DZAIO", "FreshBooster", "SAutoCarry", "[SDK] ExorAIO", "ElVeigar", "Placebo Veigar", "SluttyVeigar", "BadaoVeigar" };
                    break;
                case EloBuddy.Champion.Velkoz:
                    champ = new string[] { "Vel'Koz by Kortatu", "OKTW" };
                    break;
                case EloBuddy.Champion.Vi:
                    champ = new string[] { "ElVi", "xQx Vi" };
                    break;
                case EloBuddy.Champion.Viktor:
                    champ = new string[] { "TRUSt in my Viktor", "Hikicarry Viktor", "Perplexed Viktor", "SAutoCarry", "SFXChallenger", "Badao's Viktor", "xSalice", "[SDK] Flowers' Series", "[SDK] Flowers' Viktor", "[SDK] TRUSt in my Viktor" };
                    break;
                case EloBuddy.Champion.Vladimir:
                    champ = new string[] { "ElVladimir", "DZAIO", "SFXChallenger", "xSalice", "[SDK] The Rivers will Run Red", "[SDK] Flowers' Series", "[SDK] TroopAIO", "[SDK] Valvrave#" };
                    break;
                case EloBuddy.Champion.Volibear:
                    champ = new string[] { "UnderratedAIO", "KappaSeries", "NoobVolibear", "StonedSeries AIO" };
                    break;
                case EloBuddy.Champion.Warwick:
                    champ = new string[] { "The Blood Hunter", "BrianSharp", "D-Warwick", "DZAIO", "[SDK] ExorAIO", "Warwick II" };
                    break;
                case EloBuddy.Champion.MonkeyKing:
                    champ = new string[] { "UnderratedAIO", "2Girls1Monkey", "Hoola Wukong", "JustWukong", "mztikk's Wukong", "xQx Wukong", "NoobWukong" };
                    break;
                case EloBuddy.Champion.Xerath:
                    champ = new string[] { "Kortatu's Xerath", "OKTW", "SluttyXerath", "M1D 0R F33D", "ElXerath", "Xerath - the Magnus Ascendant" };
                    break;
                case EloBuddy.Champion.XinZhao:
                    champ = new string[] { "xQx XinZhao", "BrianSharp", "XinZhao God", "mztikk's Xin Zhao", "NoobXinZhao" };
                    break;
                case EloBuddy.Champion.Yasuo:
                    champ = new string[] { "YasuoPro", "BrianSharp", "GosuMechanics", "YasuoSharpv2", "[Yasuo] Master of Wind", "M1D 0R F33D", "YasuoMemeBender", "Media's Yasuo", "[SDK] Valvrave#", "BadaoYasuo", "hYasuo" };
                    break;
                case EloBuddy.Champion.Yorick:
                    champ = new string[] { "UnderratedAIO", "The Staffer" };
                    break;
                case EloBuddy.Champion.Zac:
                    champ = new string[] { "UnderratedAIO", "The Secret Flubber" };
                    break;
                case EloBuddy.Champion.Zed:
                    champ = new string[] { "Korean Zed", "SharpyAIO", "[SDK] Valvrave#", "iDZed", "Ze-D is Back" };
                    break;
                case EloBuddy.Champion.Ziggs:
                    champ = new string[] { "Ziggs#", "Royal Ziggy" };
                    break;
                case EloBuddy.Champion.Zilean:
                    champ = new string[] { "ElZilean", "Support is Easy", "BlackZilean" };
                    break;
                case EloBuddy.Champion.Zyra:
                    champ = new string[] { "D-Zyra", "Support is Easy", "xSalice", "[SDK] RAREZyra" };
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

            var dutility = new Menu("Dual-Utilities", "Utilitiesports");
            dutility.AddItem(new MenuItem("enableActivator", "Enable Activator?").SetValue(false));
            dutility.AddItem(new MenuItem("Activator", "Which Activator?").SetValue(new StringList(new[] { "ElUtilitySuite", "Activator#", "NabbActivator" })));

            dutility.AddItem(new MenuItem("enableTracker", "Enable Tracker?").SetValue(false));
            dutility.AddItem(new MenuItem("Tracker", "Which Tracker?").SetValue(new StringList(new[] { "SFXUtility", "Tracker#", "NabbTracker" })));

            dutility.AddItem(new MenuItem("enableEvade", "Enable Evade?").SetValue(false));
            dutility.AddItem(new MenuItem("Evade", "Which Evade?").SetValue(new StringList(new[] { "EzEvade", "Evade" })));

            dutility.AddItem(new MenuItem("enableHuman", "Enable Humanizer?").SetValue(false));
            dutility.AddItem(new MenuItem("Humanizer", "Which Humanizer?").SetValue(new StringList(new[] { "Humanizer#", "Sebby Ban Wars" })));

            dutility.AddItem(new MenuItem("enablePredictioner", "Enable Predictioner?").SetValue(false));
            dutility.AddItem(new MenuItem("Predictioner", "Which Predictioner?").SetValue(new StringList(new[] { "SPredictioner", "OKTWPredictioner" })));
            menu.AddSubMenu(dutility);

            var utility = new Menu("Standalone Utilities", "PortAIOuTILITIESS");
            utility.AddItem(new MenuItem("ShadowTracker", "Enable ShadowTracker?").SetValue(false));
            utility.AddItem(new MenuItem("UniversalPings", "Enable UniversalPings?").SetValue(false));
            menu.AddSubMenu(utility);

            var autoPlay = new Menu("Auto Play", "PortAIOAUTOPLAY");
            autoPlay.AddItem(new MenuItem("AutoPlay", "Enable AutoPlay?").SetValue(false));
            autoPlay.AddItem(new MenuItem("selectAutoPlay", "Which AutoPlay?").SetValue(new StringList(new[] { "AramDETFull", "AutoJungle", "SharpAI" })));
            menu.AddSubMenu(autoPlay);

            menu.AddItem(new MenuItem("UtilityOnly", "Utility Only?").SetValue(false));
            menu.AddItem(new MenuItem("ChampsOnly", "Champs Only?").SetValue(false));
        }
    }
}
