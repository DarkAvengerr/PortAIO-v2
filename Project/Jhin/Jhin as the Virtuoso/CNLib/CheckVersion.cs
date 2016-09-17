using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CNLib {
	public static class CheckVersion {

		public static string CheckUrl { get; private set; }
		public static string NewsUrl { get; private set; }

		private static Menu MenuConfig { get; set; }

		public static void Initialize(Menu config,string checkUrl,string newsUrl = "") {
			MenuConfig = config;
			CheckUrl = checkUrl;
			NewsUrl = newsUrl;

			if (config.SubMenu("Credits")!=null)
			{
				var Menu = config.SubMenu("Credits");
				Menu.AddSeparator();
				Menu.AddBool("检查版本", "检查版本",true);
				Menu.AddBool("新闻", "提示新闻", true);
			}
			else
			{
				var Menu = config.AddMenu("Credits", "脚本信息");
				Menu.AddLabel("作者：晴依");
				Menu.AddLabel("如果你喜欢这个脚本，记得在脚本库中点赞！");
				Menu.AddSeparator();
				Menu.AddBool("检查版本", "检查版本", true);
				Menu.AddBool("新闻", "提示新闻", true);
			}

			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
		}

		private static void Game_OnGameLoad(EventArgs args) {
			if (MenuConfig.GetBool("检查版本"))
			{
				UpdateCheck();
			}
			if (MenuConfig.GetBool("新闻") && !string.IsNullOrEmpty(NewsUrl))
			{
				News();
			}
		}

		private static async void News() {
			try
			{
				using (var web = new WebClient())
				{
					var rawFile = await web.DownloadStringTaskAsync(NewsUrl);
					if (!string.IsNullOrEmpty(rawFile))
					{
						Chat.Print("[新闻]".ToHtml(Color.Gold, FontStlye.Bold)
							+ " "
							+ MenuConfig.DisplayName.ToHtml(Color.SkyBlue,FontStlye.Bold)
							+ rawFile);
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private static async void UpdateCheck() {
			try
			{
				using (var web = new WebClient())
				{
					var rawFile = await web.DownloadStringTaskAsync(CheckUrl);
					var checkFile =
						new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match
							(rawFile);
					if (!checkFile.Success)
					{
						return;
					}
					var gitVersion =
						new System.Version(
							$"{checkFile.Groups[1]}.{checkFile.Groups[2]}.{checkFile.Groups[3]}.{checkFile.Groups[4]}");
					if (gitVersion > Assembly.GetExecutingAssembly().GetName().Version)
					{
                        Chat.Print("[版本检查]".ToHtml(Color.Gold, FontStlye.Bold)
							+ " "
							+ MenuConfig.DisplayName.ToHtml(Color.SkyBlue, FontStlye.Bold)
							+ " "
							+ "有新版本".ToHtml(Color.SkyBlue) + gitVersion.ToString());
					}
				}
			}
			catch (Exception ex)
			{
                Chat.Print("[版本检查]".ToHtml(Color.Gold, FontStlye.Bold)
							+ " "
							+ MenuConfig.DisplayName.ToHtml(Color.Red,FontStlye.Bold)
							+ " "
							+ "的版本检查发生了异常".ToHtml(Color.Red));
			}
		}

	}
}
