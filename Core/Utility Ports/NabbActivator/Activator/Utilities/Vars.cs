using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The Vars class.
    /// </summary>
    internal class Vars
    {
        #region Static Fields

        /// <summary>
        ///     A list of the names of the champions who cast Invalid Snares.
        /// </summary>
        public static readonly List<string> InvalidSnareCasters = new List<string>
                                                                      { "Leona", "Zyra", "Lissandra", "Cassiopeia" };

        /// <summary>
        ///     A list of the names of the champions who cast Invalid Stuns.
        /// </summary>
        public static readonly List<string> InvalidStunCasters = new List<string>
                                                                     {
                                                                         "Amumu", "LeeSin", "Alistar", "Hecarim",
                                                                         "Blitzcrank"
                                                                     };

        /// <summary>
        ///     States if the champion has any autoattack resets.
        /// </summary>
        public static bool HasAnyReset = false;

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
                                                                                                 BaseSkinName = "SRU_Red",
                                                                                                 Width = 139, Height = 4,
                                                                                                 XOffset = 12,
                                                                                                 YOffset = 24
                                                                                             },
                                                                                         new JungleHpBarOffset
                                                                                             {
                                                                                                 BaseSkinName = "SRU_Blue",
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
                                                                                                 BaseSkinName = "SRU_Krug",
                                                                                                 Width = 79, Height = 2,
                                                                                                 XOffset = 1, YOffset = 7
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

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the drawings menu.
        /// </summary>
        public static Menu DrawingsMenu { internal get; set; }

        /// <summary>
        ///     Gets the challenging smite's damage.
        /// </summary>
        public static int GetChallengingSmiteDamage => 54 + 6 * GameObjects.Player.Level;

        /// <summary>
        ///     Gets the chilling smite's damage.
        /// </summary>
        public static int GetChillingSmiteDamage => 20 + 8 * GameObjects.Player.Level;

        /// <summary>
        ///     Gets the ignite damage.
        /// </summary>
        public static int GetIgniteDamage => 50 + 20 * GameObjects.Player.Level;

        /// <summary>
        ///     Gets the normal smite's damage.
        /// </summary>
        public static int GetSmiteDamage
            =>
                GameObjects.Player.GetBuffCount(
                    GameObjects.Player.Buffs.First(s => s.Name.ToLower().Contains("smitedamagetracker")).Name);

        /// <summary>
        ///     Gets or sets the keybinds menu.
        /// </summary>
        public static Menu KeysMenu { internal get; set; }

        /// <summary>
        ///     Gets or sets the assembly menu.
        /// </summary>
        public static Menu Menu { internal get; set; }

        /// <summary>
        ///     Gets or sets the slider menu.
        /// </summary>
        public static Menu SliderMenu { internal get; set; }

        /// <summary>
        ///     Gets or sets the Smite Spell.
        /// </summary>
        public static Spell Smite { internal get; set; }

        /// <summary>
        ///     Gets or sets the smite menu.
        /// </summary>
        public static Menu SmiteMenu { internal get; set; }

        /// <summary>
        ///     Gets or sets the smite miscellaneous menu.
        /// </summary>
        public static Menu SmiteMiscMenu { internal get; set; }

        /// <summary>
        ///     Gets or sets the smite whitelist menu.
        /// </summary>
        public static Menu SmiteWhiteListMenu { internal get; set; }

        /// <summary>
        ///     Gets or sets the W Spell.
        /// </summary>
        public static Spell W { internal get; set; }

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