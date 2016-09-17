using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Font = SharpDX.Direct3D9.Font;
using Color = System.Drawing.Color;
using XDColor = SharpDX.Color;
using LeagueSharp;
using LeagueSharp.Common;
using Rectangle = SharpDX.Rectangle;
using EloBuddy;

namespace CNLib {
	public static class FontExtensions {
		private static readonly Dictionary<string, Rectangle> Measured = new Dictionary<string, Rectangle>();

		private static Rectangle GetMeasured(Font font, string text) {
			Rectangle rec;
			var key = font.Description.FaceName + font.Description.Width + font.Description.Height +
					  font.Description.Weight + text;
			if (!Measured.TryGetValue(key, out rec))
			{
				rec = font.MeasureText(null, text, FontDrawFlags.Center);
				Measured.Add(key, rec);
			}
			return rec;
		}

		public static XDColor ColorToXD(Color color) {
			return new XDColor(color.R,color.G,color.B,color.A);
		}

		public static void DrawTextCentered(this Font font,
			string text,
			Vector2 position,
			Color color,
			bool outline = false) {
			DrawTextCentered(font,text,position, ColorToXD(color),outline);
		}

		public static void DrawTextCentered(this Font font,
			string text,
			Obj_AI_Base target,
			Color color,
			bool outline = false) 
		{
			var measure = GetMeasured(font, text);
			var position = Drawing.WorldToScreen(target.Position);
			if (outline)
			{
				font.DrawText(
					null, text, (int)(position.X + 1 - measure.Width * 0.5f),
					(int)(position.Y + 1 - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X - 1 - measure.Width * 0.5f),
					(int)(position.Y - 1 - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X + 1 - measure.Width * 0.5f),
					(int)(position.Y - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X - 1 - measure.Width * 0.5f),
					(int)(position.Y - measure.Height * 0.5f), XDColor.Black);
			}
			font.DrawText(
				null, text, (int)(position.X - measure.Width * 0.5f), (int)(position.Y - measure.Height * 0.5f), new XDColor(color.R, color.G, color.B, color.A));

		}

		public static void DrawTextCentered(this Font font,
			string text,
			Vector2 position,
			XDColor color,
			bool outline = false) 
		{
			
			var measure = GetMeasured(font, text);
			if (outline)
			{
				font.DrawText(
					null, text, (int)(position.X + 1 - measure.Width * 0.5f),
					(int)(position.Y + 1 - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X - 1 - measure.Width * 0.5f),
					(int)(position.Y - 1 - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X + 1 - measure.Width * 0.5f),
					(int)(position.Y - measure.Height * 0.5f), XDColor.Black);
				font.DrawText(
					null, text, (int)(position.X - 1 - measure.Width * 0.5f),
					(int)(position.Y - measure.Height * 0.5f), XDColor.Black);
			}
			font.DrawText(
				null, text, (int)(position.X - measure.Width * 0.5f), (int)(position.Y - measure.Height * 0.5f), color);
		}

		public static void DrawTextCentered(this Font font, string text, int x, int y, XDColor color) {
			DrawTextCentered(font, MultiLanguage._(text), new Vector2(x, y), color);
		}

		public static void DrawTextCentered(this Font font, string text, int x, int y, Color color) {
			DrawTextCentered(font, MultiLanguage._(text), new Vector2(x, y), new XDColor(color.R,color.G,color.B,color.A));
		}

		/// <summary>
		/// 显示在屏幕上指定位置文字
		/// </summary>
		/// <param name="font">字体</param>
		/// <param name="text">要显示的内容</param>
		/// <param name="color">显示颜色</param>
		/// <param name="x">显示在屏幕X轴%</param>
		/// <param name="y">显示在屏幕Y轴%</param>
		public static void DrawScreenPercent(this Font font, string text, Color color, int x,int y) {
			DrawTextCentered(font, MultiLanguage._(text), new Vector2((float)(Drawing.Width) * x / 100, (float)(Drawing.Height) * y / 100), new XDColor(color.R, color.G, color.B, color.A));
		}
	}
}
