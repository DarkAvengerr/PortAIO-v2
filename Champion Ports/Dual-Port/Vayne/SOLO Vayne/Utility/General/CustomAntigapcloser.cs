using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SOLOVayne.Utility.General
{

    /// <summary>
    /// The delegate for the <see cref="AntiGapcloser.OnEnemyGapcloser"/> event.
    /// </summary>
    /// <param name="gapcloser">The gapcloser.</param>
    public delegate void OnGapcloseH(ActiveGapcloser gapcloser);

    /// <summary>
    /// The type of gapcloser.
    /// </summary>
    public enum GapcloserType
    {
        /// <summary>
        /// The gapcloser used a skillshot ability.
        /// </summary>
        Skillshot,

        /// <summary>
        /// The gapcloser used a targeted ability.
        /// </summary>
        Targeted
    }

    /// <summary>
    /// Represents a gapcloser.
    /// </summary>
    public struct Gapcloser
    {
        /// <summary>
        /// The champion name
        /// </summary>
        public string ChampionName;

        /// <summary>
        /// The skill type
        /// </summary>
        public GapcloserType SkillType;

        /// <summary>
        /// The slot
        /// </summary>
        public SpellSlot Slot;

        /// <summary>
        /// The spell name
        /// </summary>
        public string SpellName;

        public bool ShouldBeRepelledOnSmartMode;
    }

    /// <summary>
    /// Represents an active gapcloser.
    /// </summary>
    public struct ActiveGapcloser
    {
        /// <summary>
        /// The end
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// The sender
        /// </summary>
        public AIHeroClient Sender;

        /// <summary>
        /// The skill type
        /// </summary>
        public GapcloserType SkillType;

        /// <summary>
        /// The start
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// The tick count
        /// </summary>
        public int TickCount;

        /// <summary>
        /// The slot
        /// </summary>
        public SpellSlot Slot;

        public SpellData SData;
    }

    /// <summary>
    /// Provides events and information about gapclosers.
    /// </summary>
    public static class CustomAntiGapcloser
    {
        /// <summary>
        /// The gapcloser spells
        /// </summary>
        public static List<Gapcloser> Spells = new List<Gapcloser>();

        /// <summary>
        /// The active gapclosers
        /// </summary>
        public static List<ActiveGapcloser> ActiveGapclosers = new List<ActiveGapcloser>();

        /// <summary>
        /// Initializes static members of the <see cref="AntiGapcloser"/> class. 
        /// </summary>
        static CustomAntiGapcloser()
        {
            #region Aatrox

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Aatrox",
                    Slot = SpellSlot.Q,
                    SpellName = "aatroxq",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Akali

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Akali",
                    Slot = SpellSlot.R,
                    SpellName = "akalishadowdance",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Alistar

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Alistar",
                    Slot = SpellSlot.W,
                    SpellName = "headbutt",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Corki

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Corki",
                    Slot = SpellSlot.W,
                    SpellName = "carpetbomb",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Diana

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Diana",
                    Slot = SpellSlot.R,
                    SpellName = "dianateleport",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion
            
            #region Ekko
            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Ekko",
                    Slot = SpellSlot.E,
                    SpellName = "ekkoe",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });
	
            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Ekko",
                    Slot = SpellSlot.E,
                    SpellName = "ekkoeattack",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });
            #endregion

            #region Elise

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Elise",
                    Slot = SpellSlot.Q,
                    SpellName = "elisespiderqcast",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });
                
            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Elise",
                    Slot = SpellSlot.E,
                    SpellName = "elisespideredescent",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Fiora

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Fiora",
                    Slot = SpellSlot.Q,
                    SpellName = "fioraq",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Fizz

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Fizz",
                    Slot = SpellSlot.Q,
                    SpellName = "fizzpiercingstrike",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Gnar

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Gnar",
                    Slot = SpellSlot.E,
                    SpellName = "gnarbige",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Gnar",
                    Slot = SpellSlot.E,
                    SpellName = "gnare",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Gragas

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Gragas",
                    Slot = SpellSlot.E,
                    SpellName = "gragase",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Graves

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Graves",
                    Slot = SpellSlot.E,
                    SpellName = "gravesmove",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Hecarim

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Hecarim",
                    Slot = SpellSlot.R,
                    SpellName = "hecarimult",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion
            
            #region Illaoi
            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Illaoi",
                    Slot = SpellSlot.W,
                    SpellName = "illaoiwattack",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });
            #endregion
            
            
            #region Irelia

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Irelia",
                    Slot = SpellSlot.Q,
                    SpellName = "ireliagatotsu",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region JarvanIV

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "JarvanIV",
                    Slot = SpellSlot.Q,
                    SpellName = "jarvanivdragonstrike",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Jax

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Jax",
                    Slot = SpellSlot.Q,
                    SpellName = "jaxleapstrike",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Jayce

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Jayce",
                    Slot = SpellSlot.Q,
                    SpellName = "jaycetotheskies",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Kassadin

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Kassadin",
                    Slot = SpellSlot.R,
                    SpellName = "riftwalk",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Khazix

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixe",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixelong",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region LeBlanc

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.W,
                    SpellName = "leblancslide",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.R,
                    SpellName = "leblancslidem",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region LeeSin

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeeSin",
                    Slot = SpellSlot.Q,
                    SpellName = "blindmonkqtwo",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Leona

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Leona",
                    Slot = SpellSlot.E,
                    SpellName = "leonazenithblade",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Lucian

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Lucian",
                    Slot = SpellSlot.E,
                    SpellName = "luciane",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Malphite

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Malphite",
                    Slot = SpellSlot.R,
                    SpellName = "ufslash",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region MasterYi

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "MasterYi",
                    Slot = SpellSlot.Q,
                    SpellName = "alphastrike",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region MonkeyKing

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "MonkeyKing",
                    Slot = SpellSlot.E,
                    SpellName = "monkeykingnimbus",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Pantheon

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.W,
                    SpellName = "pantheon_leapbash",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.R,
                    SpellName = "pantheonrjump",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.R,
                    SpellName = "pantheonrfall",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Poppy

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Poppy",
                    Slot = SpellSlot.E,
                    SpellName = "poppyheroiccharge",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Renekton

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Renekton",
                    Slot = SpellSlot.E,
                    SpellName = "renektonsliceanddice",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Riven

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.Q,
                    SpellName = "riventricleave",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.E,
                    SpellName = "rivenfeint",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Sejuani

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Sejuani",
                    Slot = SpellSlot.Q,
                    SpellName = "sejuaniarcticassault",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Shen

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Shen",
                    Slot = SpellSlot.E,
                    SpellName = "shenshadowdash",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Shyvana

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Shyvana",
                    Slot = SpellSlot.R,
                    SpellName = "shyvanatransformcast",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Talon

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Talon",
                    Slot = SpellSlot.E,
                    SpellName = "taloncutthroat",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Tristana

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Tristana",
                    Slot = SpellSlot.W,
                    SpellName = "rocketjump",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Tryndamere

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Tryndamere",
                    Slot = SpellSlot.E,
                    SpellName = "slashcast",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            #region Vi

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Vi",
                    Slot = SpellSlot.Q,
                    SpellName = "viq",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region XinZhao

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "XinZhao",
                    Slot = SpellSlot.E,
                    SpellName = "xenzhaosweep",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Yasuo

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Yasuo",
                    Slot = SpellSlot.E,
                    SpellName = "yasuodashwrapper",
                    SkillType = GapcloserType.Targeted,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Zac

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Zac",
                    Slot = SpellSlot.E,
                    SpellName = "zace",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = true
                });

            #endregion

            #region Ziggs

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Ziggs",
                    Slot = SpellSlot.W,
                    SpellName = "ziggswtoggle",
                    SkillType = GapcloserType.Skillshot,
                    ShouldBeRepelledOnSmartMode = false
                });

            #endregion

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        /// <summary>
        /// Occurs on an incoming enemy gapcloser.
        /// </summary>
        public static event OnGapcloseH OnEnemyGapcloser;

        /// <summary>
        /// Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveGapclosers.RemoveAll(entry => Utils.TickCount > entry.TickCount + 900);
            if (OnEnemyGapcloser == null)
            {
                return;
            }

            foreach (
                var gapcloser in
                    ActiveGapclosers.Where(gapcloser => gapcloser.Sender.IsValidTarget())
                        .Where(
                            gapcloser =>
                                gapcloser.SkillType == GapcloserType.Targeted ||
                                (gapcloser.SkillType == GapcloserType.Skillshot &&
                                 ObjectManager.Player.Distance(gapcloser.Sender, true) < 250000))) // 500 * 500
            {
                OnEnemyGapcloser(gapcloser);
            }
        }

        /// <summary>
        /// Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!SpellIsGapcloser(args))
            {
                return;
            }

            ActiveGapclosers.Add(
                new ActiveGapcloser
                {
                    Start = args.Start,
                    End = args.End,
                    Sender = (AIHeroClient)sender,
                    TickCount = Utils.TickCount,
                    SkillType = (args.Target != null && args.Target.IsMe) ? GapcloserType.Targeted : GapcloserType.Skillshot,
                    Slot = ((AIHeroClient)sender).GetSpellSlot(args.SData.Name),
                    SData = args.SData
                });
        }

        /// <summary>
        /// Checks if a spell is a gapcloser.
        /// </summary>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private static bool SpellIsGapcloser(GameObjectProcessSpellCastEventArgs args)
        {
            return Spells.Any(spell => spell.SpellName == args.SData.Name.ToLower());
        }

        public static bool SpellShouldBeRepelledOnSmartMode(string name)
        {
            return Spells.Any(spell => spell.SpellName == name.ToLower() && spell.ShouldBeRepelledOnSmartMode);
        }
    }
}