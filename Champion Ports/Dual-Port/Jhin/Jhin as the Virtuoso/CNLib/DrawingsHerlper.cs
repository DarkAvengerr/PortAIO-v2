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
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CNLib {
	public static class DrawingsHerlper {
		public static void DrawText(this Font font, string text,Color color,Vector2 position) {
			font.DrawText(null,MultiLanguage._(text),(int)position.X, (int)position.Y, new ColorBGRA(color.B, color.G, color.R, color.A));
		}

		/// <summary>
		/// 显示在屏幕上指定位置文字
		/// </summary>
		/// <param name="font">字体</param>
		/// <param name="text">要显示的内容</param>
		/// <param name="color">显示颜色</param>
		/// <param name="x">显示在屏幕X轴%</param>
		/// <param name="y">显示在屏幕Y轴%</param>
		public static void DrawText(this Font font, string text, Color color, int x,int y) {
			font.DrawText(null, MultiLanguage._(text), (int)((float)(Drawing.Width) * x / 100), (int)((float)(Drawing.Height) * y / 100), new ColorBGRA(color.B, color.G, color.R, color.A));
		}
	}
}
