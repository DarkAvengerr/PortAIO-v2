
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        #region Static Fields

        /// <summary>
        ///     A list of the names of the champions who have a different healthbar type.
        /// </summary>
        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

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
                                                                                                 BaseSkinName = "Sru_Crab",
                                                                                                 Width = 61, Height = 2,
                                                                                                 XOffset = 1, YOffset = 5
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

        #region Public Methods and Operators

        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            var globalChampions = new List<string> { "Lux", "Jhin", "Ryze", "Taliyah", "Caitlyn" };
            Drawing.OnDraw += delegate
                {
                    if (!GameObjects.Player.Position.IsOnScreen())
                    {
                        return;
                    }

                    /// <summary>
                    ///     Loads the Q drawing,
                    ///     Loads the Extended Q drawing.
                    /// </summary>
                    if (Vars.Q != null && Vars.Q.IsReady())
                    {
                        if (Vars.Menu["drawings"]["q"] != null && Vars.Menu["drawings"]["q"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(
                                GameObjects.Player.Position,
                                GameObjects.Player.ChampionName.Equals("Jinx") && GameObjects.Player.HasBuff("JinxQ")
                                    ? Vars.PowPow.Range
                                    : Vars.Q.Range,
                                Color.LightGreen,
                                2);
                        }
                        if (Vars.Menu["drawings"]["qe"] != null
                            && Vars.Menu["drawings"]["qe"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Q2.Range, Color.Yellow, 2);
                        }
                    }

                    /// <summary>
                    ///     Loads the W drawing.
                    /// </summary>
                    if (Vars.W != null && Vars.W.IsReady())
                    {
                        if (Vars.Menu["drawings"]["w"] != null && Vars.Menu["drawings"]["w"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.W.Range, Color.Purple, 2);
                        }
                    }

                    /// <summary>
                    ///     Loads the E drawing.
                    /// </summary>
                    if (Vars.E != null && Vars.E.IsReady())
                    {
                        if (Vars.Menu["drawings"]["e"] != null && Vars.Menu["drawings"]["e"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.E.Range, Color.Cyan, 2);
                        }
                    }

                    /// <summary>
                    ///     Loads the R drawing.
                    /// </summary>
                    if (Vars.R != null && Vars.R.IsReady())
                    {
                        if (Vars.Menu["drawings"]["r"] != null && Vars.Menu["drawings"]["r"].GetValue<MenuBool>().Value)
                        {
                            if (globalChampions.Contains(GameObjects.Player.ChampionName))
                            {
                                Geometry.DrawCircleOnMinimap(GameObjects.Player.Position, Vars.R.Range, Color.White);
                            }
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.R.Range, Color.Red, 2);
                        }
                        if (Vars.Menu["drawings"]["r2"] != null
                            && Vars.Menu["drawings"]["r2"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.R2.Range, Color.Blue, 2);
                        }
                    }
                };
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