using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbTracker
{
    using LeagueSharp.SDK.UI;

    using SharpDX;

    /// <summary>
    ///     The Color convert class,
    ///     Converts a SharpDX.Color or a System.Drawing.Color type into the same color as seen by a colorblind person
    ///     for each type of colorblindness, using the 'FromBgra' function.
    ///     https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Color.Palette.cs
    /// </summary>
    internal class Colors
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts a SharpDX.Color type into the same color as seen by a colorblind person
        ///     for each type of colorblindness, using the 'FromBgra' function.
        /// </summary>
        public static Color Convert(Color color)
        {
            var sdxConvert = new[,]
                                 {
                                     { 0xFF808080, 0xFF7F7F81, 0xFF807E82, 0xFF808080, 0xFF7F7F7F }, // gray
                                     { 0xFFFFFF00, 0xFFFFE802, 0xFFFFF205, 0xFFFFD6D0, 0xFFE1E1E1 }, // yellow
                                     { 0xFFFF0000, 0xFF6D5600, 0xFF332802, 0xFFF70500, 0xFF4C4C4C }, // red
                                     { 0xFF00FFFF, 0xFF91A8FF, 0xFFCCD3FF, 0xFF07F9FF, 0xFFB2B2B2 }, // cyan
                                     { 0xFF90EE90, 0xFFD3C594, 0xFFEDD893, 0xFF9ADCE2, 0xFFC7C7C7 }, // lightgreen
                                     { 0xFF800080, 0xFF23377D, 0xFF011981, 0xFF71170F, 0xFF343434 } // purple
                                 };
            Vars.SdxColor =
                Color.FromBgra(
                    sdxConvert[
                        SdxColor(color),
                        Vars.Menu["miscellaneous"]["colorblind"]["mode"].GetValue<MenuList>().Index]);
            return Vars.SdxColor;
        }

        /// <summary>
        ///     Converts a System.Drawing.Color type into the same color as seen by a colorblind person
        ///     for each type of colorblindness, using the 'FromArgb' function.
        /// </summary>
        public static System.Drawing.Color Convert(System.Drawing.Color color)
        {
            var sdConvert = new[,,]
                                {
                                    {
                                        { 128, 128, 128 }, { 127, 127, 129 }, { 128, 126, 130 }, { 128, 128, 128 },
                                        { 127, 127, 127 }
                                    }, // gray
                                    {
                                        { 255, 255, 0 }, { 255, 232, 2 }, { 255, 242, 5 }, { 255, 214, 208 },
                                        { 225, 225, 225 }
                                    }, // yellow
                                    { { 255, 0, 0 }, { 109, 86, 0 }, { 51, 40, 2 }, { 247, 5, 0 }, { 76, 76, 76 } },

                                    // red
                                    {
                                        { 0, 255, 255 }, { 145, 168, 255 }, { 204, 211, 255 }, { 7, 249, 255 },
                                        { 178, 178, 178 }
                                    }, // cyan
                                    {
                                        { 102, 255, 0 }, { 211, 197, 148 }, { 237, 216, 147 }, { 154, 220, 226 },
                                        { 199, 199, 199 }
                                    },

                                    // lightgreen
                                    { { 143, 0, 255 }, { 35, 55, 125 }, { 1, 19, 81 }, { 71, 17, 15 }, { 34, 34, 34 } }

                                    // purple
                                };
            Vars.SdColor =
                System.Drawing.Color.FromArgb(
                    sdConvert[
                        SdColor(color),
                        Vars.Menu["miscellaneous"]["colorblind"]["mode"].GetValue<MenuList>().Index,
                        0],
                    sdConvert[
                        SdColor(color),
                        Vars.Menu["miscellaneous"]["colorblind"]["mode"].GetValue<MenuList>().Index,
                        1],
                    sdConvert[
                        SdColor(color),
                        Vars.Menu["miscellaneous"]["colorblind"]["mode"].GetValue<MenuList>().Index,
                        2]);
            return Vars.SdColor;
        }

        /// <summary>
        ///     The System.Drawing.Color type table.
        /// </summary>
        public static int SdColor(System.Drawing.Color color)
        {
            if (color == System.Drawing.Color.Gray) return 0;
            if (color == System.Drawing.Color.Yellow) return 1;
            if (color == System.Drawing.Color.Red) return 2;
            if (color == System.Drawing.Color.Cyan) return 3;
            if (color == System.Drawing.Color.LightGreen) return 4;
            if (color == System.Drawing.Color.Purple) return 5;

            return -1;
        }

        /// <summary>
        ///     The SharpDX.Color type table.
        /// </summary>
        public static int SdxColor(Color color)
        {
            if (color == Color.Gray) return 0;
            if (color == Color.Yellow) return 1;
            if (color == Color.Red) return 2;
            if (color == Color.Cyan) return 3;
            if (color == Color.LightGreen) return 4;
            if (color == Color.Purple) return 5;

            return -1;
        }

        #endregion
    }
}