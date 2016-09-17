using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CNLib {
	public static class DeBug {
		//string Prefix = ""
		//public static void Debug(string content, DebugLevel level = DebugLevel.Info,
		//	Output way = Output.Console, MenuItem DebugMenu = null) 
		//{
		//	if (DebugMenu != null && !DebugMenu.GetValue<bool>()) return;

		//	if (way == Output.Console)
		//	{
		//		ConsoleColor color = ConsoleColor.White;
		//		if (level == DebugLevel.Info)
		//		{
		//			color = ConsoleColor.Green;
		//		}
		//		else if (level == DebugLevel.Warning)
		//		{
		//			color = ConsoleColor.Yellow;
		//		}
		//		else if (level == DebugLevel.Wrang)
		//		{
		//			color = ConsoleColor.Red;
		//		}
		//		Console.ForegroundColor = color;
		//		Console.WriteLine(content);
		//		Console.ForegroundColor = ConsoleColor.White;
		//	}
		//	else if(way == Output.ChatBox)
		//	{
		//		System.Drawing.Color color = System.Drawing.Color.White;
		//		if (level == DebugLevel.Info)
		//		{
		//			color = System.Drawing.ColorTranslator.FromHtml("#AAAAFF");
		//		}
		//		else if (level == DebugLevel.Warning)
		//		{
		//			color = System.Drawing.Color.Orange;
		//		}
		//		else if (level == DebugLevel.Wrang)
		//		{
		//			color = System.Drawing.Color.Red;
		//		}

		//		Chat.Print(content.ToHtml(color, FontStlye.Cite));
		//	}
		//}

		public static void Debug(string content, DebugLevel level = DebugLevel.Info,
			Output way = Output.Console, bool enable = true) {
			if (!enable) return;

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
				Console.WriteLine(content);
				Console.ForegroundColor = ConsoleColor.White;
			}
			else if (way == Output.ChatBox)
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

				Chat.Print(content.ToHtml(color, FontStlye.Cite));
			}
		}

		//public static void Debug(string Prefix,string content, DebugLevel level = DebugLevel.Info,
		//	Output way = Output.Console,MenuItem DebugMenu = null) 
		//{
		//	if (DebugMenu!=null && !DebugMenu.GetValue<bool>()) return;

		//	if (way == Output.Console)
		//	{
		//		ConsoleColor color = ConsoleColor.White;
		//		if (level == DebugLevel.Info)
		//		{
		//			color = ConsoleColor.Green;
		//		}
		//		else if (level == DebugLevel.Warning)
		//		{
		//			color = ConsoleColor.Yellow;
		//		}
		//		else if (level == DebugLevel.Wrang)
		//		{
		//			color = ConsoleColor.Red;
		//		}
		//		Console.ForegroundColor = color;
		//		Console.WriteLine(string.IsNullOrEmpty(Prefix) ? (Prefix + "：") : "" + content);
		//		Console.ForegroundColor = ConsoleColor.White;
		//	}
		//	else
		//	{
		//		System.Drawing.Color color = System.Drawing.Color.White;
		//		if (level == DebugLevel.Info)
		//		{
		//			color = System.Drawing.ColorTranslator.FromHtml("#AAAAFF");
		//		}
		//		else if (level == DebugLevel.Warning)
		//		{
		//			color = System.Drawing.Color.Orange;
		//		}
		//		else if (level == DebugLevel.Wrang)
		//		{
		//			color = System.Drawing.Color.Red;
		//		}
		//		content = string.IsNullOrEmpty(Prefix) ? (Prefix + "：") : "" + content;
		//		Chat.Print(content.ToHtml(color, FontStlye.Cite));
		//	}
		//}

		public static void Debug(string Prefix, string content, DebugLevel level = DebugLevel.Info,
			Output way = Output.Console, bool enable = true) {
			if (!enable) return;

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
				Console.WriteLine(!string.IsNullOrEmpty(Prefix) ? (Prefix + "："+ content) : "" + content);
				Console.ForegroundColor = ConsoleColor.White;
			}
			else
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
				content = string.IsNullOrEmpty(Prefix) ? (Prefix + "：") : "" + content;
				Chat.Print(content.ToHtml(color, FontStlye.Cite));
			}
		}
		public static void DebugChat(MenuItem config, string format, params object[] param) {
			if (config.GetValue<bool>())
			{
				string s = string.Format(format, param);
				Chat.Print(s.ToHtml("#AAAAFF", FontStlye.Cite));
			}
		}

		public static void DebugChat(string format, params object[] param) {
			string s = string.Format(format, param);
			Chat.Print(s.ToHtml("#AAAAFF", FontStlye.Cite));
		}

		public static void DebugConsole(MenuItem config, string Prefix, string format, params object[] param) {
			if (!config.GetValue<bool>()) return;
			if (!string.IsNullOrEmpty(Prefix))
			{
				Prefix += "：";
			}
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine(Prefix + format, param);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void DebugConsole(string Prefix, string format, params object[] param) {
			if (!string.IsNullOrEmpty(Prefix))
			{
				Prefix += "：";
			}
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine(Prefix + format, param);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void DebugConsole(string format, params object[] param) {
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine(format, param);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
