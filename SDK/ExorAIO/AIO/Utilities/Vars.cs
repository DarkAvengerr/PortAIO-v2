
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     The Vars class.
    /// </summary>
    internal class Vars
    {
        #region Static Fields

        /// <summary>
        ///     A list of the names of the champions who cast Invalid Snares.
        /// </summary>
        public static readonly List<string> InvalidSnareCasters = new List<string> { "Leona", "Zyra", "Lissandra" };

        /// <summary>
        ///     A list of the names of the champions who cast Invalid Stuns.
        /// </summary>
        public static readonly List<string> InvalidStunCasters = new List<string>
                                                                     {
                                                                         "Amumu", "LeeSin", "Alistar", "Hecarim",
                                                                         "Blitzcrank"
                                                                     };

        /// <summary>
        ///     A list of the names of the champions who have a different healthbar type.
        /// </summary>
        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

        /// <summary>
        ///     The last tick.
        /// </summary>
        public static int LastTick = 0;

        /// <summary>
        ///     The default enemy HP bar height offset.
        /// </summary>
        public static int SHeight = 8;

        /// <summary>
        ///     The default enemy HP bar width offset.
        /// </summary>
        public static int SWidth = 103;

        /// <summary>
        ///     The jungle HP bar offset list.
        /// </summary>
        internal static readonly List<JungleHpBarOffset> JungleHpBarOffsetList = new List<JungleHpBarOffset>
                                                                                     {
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Dragon_Air",
                                                                                                 Width = 140, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Dragon_Fire",
                                                                                                 Width = 140, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Dragon_Water",
                                                                                                 Width = 140, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Dragon_Earth",
                                                                                                 Width = 140, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Dragon_Elder",
                                                                                                 Width = 140, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Baron",
                                                                                                 Width = 190,
                                                                                                 Height = 10,
                                                                                                 XOffset = 16,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_RiftHerald",
                                                                                                 Width = 139, Height = 6,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 22
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Red",
                                                                                                 Width = 139, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Blue",
                                                                                                 Width = 139, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Gromp",
                                                                                                 Width = 86, Height = 2,
                                                                                                 XOffset = 1,
                                                                                                 YOffset = 7
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "Sru_Crab",
                                                                                                 Width = 61, Height = 2,
                                                                                                 XOffset = 1,
                                                                                                 YOffset = 5
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Krug",
                                                                                                 Width = 79, Height = 2,
                                                                                                 XOffset = 1,
                                                                                                 YOffset = 7
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Razorbeak",
                                                                                                 Width = 74, Height = 2,
                                                                                                 XOffset = 1,
                                                                                                 YOffset = 7
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName =
                                                                                                     "SRU_Murkwolf",
                                                                                                 Width = 74, Height = 2,
                                                                                                 XOffset = 1,
                                                                                                 YOffset = 7
                                                                                             }
                                                                                     };

        /// <summary>
        ///     The jungle HP bar offset list.
        /// </summary>
        internal static readonly string[] JungleList =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water",
                "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
                "SRU_RiftHerald", "SRU_Red", "SRU_Blue", "SRU_Gromp",
                "Sru_Crab", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
            };

        /// <summary>
        ///     Gets all the important jungle locations.
        /// </summary>
        internal static readonly List<Vector3> Locations = new List<Vector3>
                                                               {
                                                                   new Vector3(9827.56f, 4426.136f, -71.2406f),
                                                                   new Vector3(4951.126f, 10394.05f, -71.2406f),
                                                                   new Vector3(10998.14f, 6954.169f, 51.72351f),
                                                                   new Vector3(7082.083f, 10838.25f, 56.2041f),
                                                                   new Vector3(3804.958f, 7875.456f, 52.11121f),
                                                                   new Vector3(7811.249f, 4034.486f, 53.81299f)
                                                               };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Player's real AutoAttack-Range.
        /// </summary>
        public static float AaRange => GameObjects.Player.GetRealAutoAttackRange();

        /// <summary>
        ///     Gets or sets the Drawings menu.
        /// </summary>
        public static Menu DrawingsMenu { get; set; }

        /// <summary>
        ///     Gets or sets the E Spell.
        /// </summary>
        public static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the E2 Spell.
        /// </summary>
        public static Spell E2 { get; set; }

        /// <summary>
        ///     Gets or sets the E Spell menu.
        /// </summary>
        public static Menu EMenu { get; set; }

        /// <summary>
        ///     Gets or sets the loaded champion.
        /// </summary>
        public static bool IsLoaded { get; set; } = true;

        /// <summary>
        ///     Gets or sets the assembly menu.
        /// </summary>
        public static Menu Menu { get; set; } = new Menu(
            $"aio.{GameObjects.Player.ChampionName.ToLower()}",
            $"[ExorAIO]: {GameObjects.Player.ChampionName}",
            true);

        /// <summary>
        ///     Gets or sets the Miscellaneous menu.
        /// </summary>
        public static Menu MiscMenu { get; set; }

        /// <summary>
        ///     Gets or sets the PowPow Range.
        /// </summary>
        public static Spell PowPow { get; set; }

        /// <summary>
        ///     Gets or sets the Q Spell.
        /// </summary>
        public static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the 2nd stage of the Q Spell.
        /// </summary>
        public static Spell Q2 { get; set; }

        /// <summary>
        ///     Gets or sets the Q2 Spell menu.
        /// </summary>
        public static Menu Q2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the Q Spell menu.
        /// </summary>
        public static Menu QMenu { get; set; }

        /// <summary>
        ///     Gets or sets the R Spell.
        /// </summary>
        public static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the R2 Spell.
        /// </summary>
        public static Spell R2 { get; set; }

        /// <summary>
        ///     Gets or sets the R Spell menu.
        /// </summary>
        public static Menu RMenu { get; set; }

        /// <summary>
        ///     Gets or sets the Soulbound.
        /// </summary>
        public static AIHeroClient SoulBound { get; set; }

        /// <summary>
        ///     Gets or sets the settings menu.
        /// </summary>
        public static Menu SpellsMenu { get; set; }

        /// <summary>
        ///     Gets or sets the W Spell.
        /// </summary>
        public static Spell W { get; set; }

        /// <summary>
        ///     Gets or sets the W2 Spell menu.
        /// </summary>
        public static Menu W2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the second Whitelist menu.
        /// </summary>
        public static Menu WhiteList2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the first Whitelist menu.
        /// </summary>
        public static Menu WhiteListMenu { get; set; }

        /// <summary>
        ///     Gets or sets the W Spell menu.
        /// </summary>
        public static Menu WMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the health with Blitzcrank's Shield support.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The target Health with Blitzcrank's Shield support.
        /// </returns>
        public static float GetRealHealth(Obj_AI_Base target)
        {
            var debuffer = 0f;

            /// <summary>
            ///     Gets the predicted reduction from Blitzcrank Shield.
            /// </summary>
            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.ChampionName.Equals("Blitzcrank") && !hero.HasBuff("BlitzcrankManaBarrierCD"))
                {
                    debuffer += hero.Mana / 2;
                }
            }
            return target.Health + target.AttackShield + target.HPRegenRate + debuffer;
        }

        /// <summary>
        ///     The default enemy HP bar x offset.
        /// </summary>
        public static int SxOffset(AIHeroClient target)
        {
            return SpecialChampions.Contains(target.ChampionName) ? 1 : 10;
        }

        /// <summary>
        ///     The default enemy HP bar y offset.
        /// </summary>
        public static int SyOffset(AIHeroClient target)
        {
            return SpecialChampions.Contains(target.ChampionName) ? 3 : 20;
        }

        #endregion

        /// <summary>
        ///     The jungle HP bar offset.
        /// </summary>
        internal class JungleHpBarOffset
        {
            #region Fields

            internal string BaseSkinName;

            internal int Height;

            internal int Width;

            internal int XOffset;

            internal int YOffset;

            #endregion
        }
    }
}