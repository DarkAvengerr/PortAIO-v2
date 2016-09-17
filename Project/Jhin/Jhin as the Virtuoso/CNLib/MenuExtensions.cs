using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace CNLib
{
    public static class MenuExtensions
	{

		public static void Translat(this Menu config) {
			foreach (var menu in config.Children)
			{
				menu.DisplayName = MultiLanguage._(menu.DisplayName);
				foreach (var item in menu.Items)
				{
					item.DisplayName = MultiLanguage._(item.DisplayName);
				}
			}

			foreach (var item in config.Items)
			{
				item.DisplayName = MultiLanguage._(item.DisplayName);
			}
		}

		public static Orbwalking.Orbwalker AddOrbwalker(this Menu config, string name, string displayName) {
			var OrbMenu = config.AddMenu("走砍设置", "走砍设置");
			var Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

			OrbMenu.Translat();
			return Orbwalker;
		}

		#region 菜单类方法
		public static int ItemIndex { get; private set; } = 0;

		public static Menu CreatMainMenu(string name, string displayName) {
			var config = new Menu(MultiLanguage._(displayName), name, true);
			config.AddToMainMenu();

			var Menu = config.AddMenu("Credits", "脚本信息");
			Menu.AddLabel("作者：晴依");
			Menu.AddLabel("如果你喜欢这个脚本，记得在脚本库中点赞！");
			return config;
		}

		/// <summary>
		/// 添加子菜单
		/// </summary>
		/// <param name="config"></param>
		/// <param name="name"></param>
		/// <param name="displayName"></param>
		/// <returns></returns>
		public static Menu AddMenu(this Menu config, string name, string displayName) {
			if (config.Children.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误",$"已经包含了名为{name}的子菜单!",DebugLevel.Warning);
			}

			return config.AddSubMenu(new Menu(MultiLanguage._(displayName), name));
		}

		public static MenuItem AddLabel(this Menu config, string name, string displayName) {
			if (config.Items.Any(m => m.Name == name))
			{
				config.Item(name).DisplayName = displayName;
				return config.Item(name);
			}
			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)));
		}

		public static MenuItem AddLabel(this Menu config, string displayName) {
			ItemIndex++;
			return config.AddItem(new MenuItem(ItemIndex.ToString(), MultiLanguage._(displayName)));
		}

		public static string GetLabel(this Menu config, string name) {
			return config.Item(name).DisplayName;
		}

		public static MenuItem AddSeparator(this Menu config, string name) {
			return config.AddItem(new MenuItem(name, ""));
		}

		public static MenuItem AddSeparator(this Menu config) {
			ItemIndex++;
			return config.AddItem(new MenuItem(ItemIndex.ToString(), ""));
		}

		public static MenuItem AddBool(this Menu config, string name, string displayName, bool Defaults = false) {
			if (config.Items.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误", $"已经包含了名为{name}的菜单!", DebugLevel.Warning);
			}
			////DeBug.WriteConsole("AddBool", $"displayName{displayName} --- {MultiLanguage._(displayName)}");

			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)).SetValue(Defaults));
		}

		public static bool GetBool(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<bool>();
			}
			catch (Exception ex)
			{
				//DeBug.WriteConsole("[菜单取值]",$"名为{name}的菜单项的值类型不是bool",DebugLevel.Wrang);
			}
			return false;

		}

		public static MenuItem AddStringList(this Menu config, string name, string displayName, string[] stringList = null, int Index = 0) {
			if (config.Items.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误", $"已经包含了名为{name}的菜单!", DebugLevel.Warning);
			}
			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)).SetValue(new StringList(stringList.Select(s => MultiLanguage._(s)).ToArray(), Index)));
		}

		public static StringList GetStringList(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<StringList>();
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是StringList", DebugLevel.Wrang);
			}
			return new StringList();
		}

		public static int GetStringIndex(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<StringList>().SelectedIndex;
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是StringIndex", DebugLevel.Wrang);
			}
			return 0;
		}

		public static MenuItem AddSlider(this Menu config, string name, string displayName, int Defaults = 0, int min = 0, int max = 100) {
			if (config.Items.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误", $"已经包含了名为{name}的菜单!", DebugLevel.Warning);
			}
			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)).SetValue(new Slider(Defaults, min, max)));
		}

		public static Slider GetSlider(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<Slider>();
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是Slider", DebugLevel.Wrang);
			}
			return new Slider();
		}

		public static int GetSliderValue(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<Slider>().Value;
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是SliderValue", DebugLevel.Wrang);
			}
			return 0;

		}

		public static MenuItem AddCircle(this Menu config, string name, string displayName, bool active = false, Color color = new Color()) {
			if (config.Items.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误", $"已经包含了名为{name}的菜单!", DebugLevel.Warning);
			}
			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)).SetValue(new Circle(active, color)));
		}

		public static Circle GetCircle(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<Circle>();
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是Circle", DebugLevel.Wrang);
			}
			return new Circle();
		}

		public static bool GetCircleActive(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<Circle>().Active;
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是CircleActive", DebugLevel.Wrang);
			}
			return false;
		}

		public static Color GetCircleColor(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<Circle>().Color;
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是CircleColor", DebugLevel.Wrang);
			}
			return Color.White;
		}

		public static MenuItem AddKeyBind(this Menu config, string name, string displayName,uint key, KeyBindType type,bool active = false) {
			if (config.Items.Any(m => m.Name == name))
			{
				//DeBug.Write("创建菜单错误", $"已经包含了名为{name}的菜单!", DebugLevel.Warning);
			}
			return config.AddItem(new MenuItem(name, MultiLanguage._(displayName)).SetValue(new KeyBind(key, type, active)));
		}

		public static bool GetKeyActive(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<KeyBind>().Active;
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是KeyActive", DebugLevel.Wrang);
			}
			return false;
		}

		public static KeyBind GetKeyBind(this Menu config, string name) {
			try
			{
				return config.Item(name).GetValue<KeyBind>();
			}
			catch (Exception)
			{
				//DeBug.WriteConsole("[菜单取值]", $"名为{name}的菜单项的值类型不是KeyBind", DebugLevel.Wrang);
			}
			return new KeyBind();
		}
		#endregion

	}
}
