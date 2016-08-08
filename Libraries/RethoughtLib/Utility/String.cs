using EloBuddy; namespace RethoughtLib.Utility
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Threading;

    using global::RethoughtLib.Design;

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    /// <summary>
    ///     Class that offers string utilities
    /// </summary>
    public class String
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Capitalizes the first char of the specified string.
        ///     IE: word > Word, i'am a sentece > I'am a sentence
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string Capitalize(string str)
        {
            switch (str.Length)
            {
                case 0:
                    return "";
                case 1:
                    return str.ToUpper();
                default:
                    return str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
        }

        /// <summary>
        ///     Shortens the specified string.
        ///     IE: I'm a long sentence > I'm a long sen...
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Shorten(string str, int length)
        {
            if (str.Length < length + 2)
            {
                return str;
            }

            if (str.Length <= 3)
            {
                return "...";
            }

            var removedResult = str.Remove(length - 3, str.Length - length);

            return removedResult + "...";
        }

        /// <summary>
        ///     Formats the string into all first letters uppercase.
        ///     Won't work if title is all uppercase.
        /// </summary>
        /// <param name="str">the string</param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str);
        }

        public static List<string> WrapIntoRectangle(
            string str,
            Sprite sprite,
            Rectangle rectangle,
            Offset<int> offset,
            Font font = default(Font))
        {
            //var sb = new StringBuilder();

            //var availableWidth = rectangle.Width - (offset.Left + offset.Right);
            //var availableHeight = rectangle.Height - (offset.Top + offset.Bottom);

            //if (font == null) return new List<string>() { str };

            //var expectedRawRec = font.MeasureText(sprite, str, rectangle, FontDrawFlags.WordBreak);
            ////font.DrawText(sprite, str, rectangle, FontDrawFlags.WordBreak, ColorBGRA.FromRgba(5));

            //if (expectedRawRec.Height > rectangle.Height) return new List<string>() {str};

            //int next;

            //// Parse each line of text
            //for (var pos = 0; pos < str.Length; pos = next)
            //{
            //    // Find end of line
            //    var eol = str.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal);
            //    if (eol == -1) next = eol = text.Length;
            //    else next = eol + Environment.NewLine.Length;

            //    // Copy this line of text, breaking into smaller lines as needed
            //    if (eol > pos)
            //    {
            //        do
            //        {
            //            var len = eol - pos;
            //            if (len > width) len = BreakLine(str, pos, width);
            //            sb.Append(str, pos, len);
            //            sb.Append(Environment.NewLine);

            //            // Trim whitespace following break
            //            pos += len;
            //            while (pos < eol && char.IsWhiteSpace(str[pos])) pos++;
            //        }
            //        while (eol > pos);
            //    }
            //    else sb.Append(Environment.NewLine); // Empty line
            //}
            //return sb.ToString();

            return null;
        }

        /// <summary>
        ///     Formats the given text into lines.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="font">
        ///     The font.
        /// </param>
        /// <param name="width">
        ///     The width.
        /// </param>
        /// <param name="height">
        ///     The height.
        /// </param>
        /// <param name="sprite">
        ///     The sprite.
        /// </param>
        /// <param name="htmlSupport">
        ///     Indicates whether to support HTML tags.
        /// </param>
        /// <returns>
        ///     The formatted list.
        /// </returns>
        /// <remarks>L33T</remarks>
        public static List<string> FormatText(string text, Font font, int width, int height, Sprite sprite, bool htmlSupport)
        {
            var lineAmount = font.MeasureText(sprite, text, 0).Width / width + 1;

            var lastIndex = 0;
            var format = false;
            var linesList = new List<string>();

            if (text.Contains("</br>") && htmlSupport)
            {
                lineAmount = 0;
                var formatLines = new List<string>();
                var valueCopy = text;
                while (valueCopy.Contains("</br>"))
                {
                    var breakLine = valueCopy.Substring(0, valueCopy.IndexOf("</br>", StringComparison.Ordinal));
                    ++lineAmount;
                    formatLines.Add(!string.IsNullOrEmpty(breakLine) ? breakLine : string.Empty);

                    valueCopy = valueCopy.Substring(
                        valueCopy.IndexOf("</br>", StringComparison.Ordinal) + 5,
                        valueCopy.Length - valueCopy.IndexOf("</br>", StringComparison.Ordinal) - 5);
                }

                formatLines.Add(string.IsNullOrEmpty(valueCopy) ? " " : valueCopy);
                height += font.Description.Height;

                foreach (var line in formatLines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        linesList.Add(" ");
                        height += font.Description.Height;
                        continue;
                    }

                    for (var j = line.Length; j > -1; --j)
                    {
                        if (j - 1 > -1 && line.Length - lastIndex - j >= 0
                            && font.MeasureText(sprite, line.Substring(lastIndex, line.Length - lastIndex - j), 0).Width
                            < width)
                        {
                            continue;
                        }

                        var original = line.Substring(lastIndex, line.Length - lastIndex - j);

                        if (!string.IsNullOrEmpty(original))
                        {
                            linesList.Add(original);
                        }

                        lastIndex = line.Length - j;
                    }

                    lastIndex = 0;
                }

                format = true;
            }

            if (!format)
            {
                for (var j = text.Length; j > -1; --j)
                {
                    if (j - 1 > -1 && text.Length - lastIndex - j >= 0
                        && font.MeasureText(sprite, text.Substring(lastIndex, text.Length - lastIndex - j), 0).Width
                        < width)
                    {
                        continue;
                    }

                    var original = text.Substring(lastIndex, text.Length - lastIndex - j);

                    if (!string.IsNullOrEmpty(original))
                    {
                        linesList.Add(original);
                    }

                    lastIndex = text.Length - j;
                }
            }

            if (lineAmount > 4)
            {
                height += font.Description.Height * (lineAmount - 4);
            }

            return linesList;
        }

        #endregion
    }
}