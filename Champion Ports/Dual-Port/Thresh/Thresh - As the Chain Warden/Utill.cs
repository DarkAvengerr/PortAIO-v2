using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshAsurvil {
	public static class Utill {

		#region 一组调试工具
		public static void Print(string format, params object[] param) {
			string s = string.Format(format, param);
			Chat.Print(s.ToHtml("#AAAAFF", FontStlye.Cite));
		}
		public static void Debug(string content, DebugLevel level = DebugLevel.Info, Output way = Output.Console) {
			if (way == Output.Console)
			{
				ConsoleColor color = ConsoleColor.White;
				if (level == DebugLevel.Info)
				{
					color = ConsoleColor.Green;
				}
				else if (level == DebugLevel.Warning)
				{
					color = ConsoleColor.Yellow;
				}
				else if (level == DebugLevel.Wrang)
				{
					color = ConsoleColor.Red;
				}
				Console.ForegroundColor = color;
				Console.WriteLine("AS锤石：" + content);
				Console.ForegroundColor = ConsoleColor.White;
			}
			else if(Thresh.Config.Item("调试").GetValue<bool>())
			{
				System.Drawing.Color color = System.Drawing.Color.White;
				if (level == DebugLevel.Info)
				{
					color = System.Drawing.ColorTranslator.FromHtml("#AAAAFF");
				}
				else if (level == DebugLevel.Warning)
				{
					color = System.Drawing.Color.Orange;
				}
				else if (level == DebugLevel.Wrang)
				{
					color = System.Drawing.Color.Red;
				}
			}
		}
		public static void DebugChat(string format, params object[] param) {
			string s = string.Format(format, param);
			Chat.Print(s.ToHtml("#AAAAFF", FontStlye.Cite));
		}
		public static void DebugConsole(string format, params object[] param) {
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("AS锤石：" + format, param);
			Console.ForegroundColor = ConsoleColor.White;
		}
		#endregion
	}
}
