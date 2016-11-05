using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace OriannaTheruleroftheBall
{
    class Initiator
    {
        public struct Initiatorinfo
        {
            public string Hero;
            public string Spell;
            public string Spelldata;
        }

        public static List<Initiatorinfo> InitiatorList = new List<Initiatorinfo>();

        static Initiator()
        {
            #region Amumu

            InitiatorList.Add(new Initiatorinfo
                {
                    Hero = "Amumu",
                    Spell = "Amumu-Q",
                    Spelldata = "bandagetoss"
                });

            #endregion

            #region FiddleSticks

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "FiddleSticks",
                Spell = "FiddleSticks-R",
                Spelldata = "crowstorm"
            });

            #endregion

            #region Gragas

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Gragas",
                Spell = "Gragas-E",
                Spelldata = "gragase"
            });

            #endregion

            #region Hecarim

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Hecarim",
                Spell = "Hecarim-R",
                Spelldata = "hecarimult"
            });

            #endregion

            #region Irelia

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Irelia",
                Spell = "Irelia-Q",
                Spelldata = "ireliagatotsu"
            });

            #endregion

            #region JarvanIV

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "JarvanIV",
                Spell = "JarvanIV-EQ",
                Spelldata = "JarvanIVDragonStrike"
            });

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "JarvanIV",
                Spell = "JarvanIV-R",
                Spelldata = "JarvanIVCataclysm"
            });

            #endregion

            #region LeeSin

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "LeeSin",
                Spell = "LeeSin-Q2",
                Spelldata = "blindmonkqtwo"
            });

            #endregion

            #region Leona

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Leona",
                Spell = "Leona-E",
                Spelldata = "LeonaZenithBladeMissle"
            });

            #endregion

            #region Lissandra

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Lissandra",
                Spell = "Lissandra-E",
                Spelldata = "LissandraE"
            });

            #endregion

            #region Malphite

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Malphite",
                Spell = "Malphite-R",
                Spelldata = "UFSlash"
            });

            #endregion

            #region Maokai

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Maokai",
                Spell = "Maokai-W",
                Spelldata = "MaokaiUnstableGrowth"
            });

            #endregion

            #region Rengar

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Rengar",
                Spell = "Rengar-R",
                Spelldata = "RengarR"
            });

            #endregion

            #region Sejuani

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Sejuani",
                Spell = "Sejuani-Q",
                Spelldata = "SejuaniArcticAssault"
            });

            #endregion

            #region Shen

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Shen",
                Spell = "Shen-E",
                Spelldata = "ShenShadowDash"
            });

            #endregion

            #region Shyvana

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Shyvana",
                Spell = "Shyvana-R",
                Spelldata = "ShyvanaTransformCast"
            });

            #endregion

            #region Thresh

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Thresh",
                Spell = "Thresh-Q",
                Spelldata = "threshqleap"
            });

            #endregion

            #region Vi

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Vi",
                Spell = "Vi-Q",
                Spelldata = "ViQ"
            });

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Vi",
                Spell = "Vi-R",
                Spelldata = "ViR"
            });

            #endregion

            #region Zac

            InitiatorList.Add(new Initiatorinfo
            {
                Hero = "Zac",
                Spell = "Zac-E",
                Spelldata = "ZacE"
            });

            #endregion
        }
    }
}
