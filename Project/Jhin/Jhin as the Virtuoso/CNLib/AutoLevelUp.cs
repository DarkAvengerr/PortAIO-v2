using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using EloBuddy;

namespace CNLib {
	public static class AutoLevelUp {

		private static Menu Config{ get; set; }
		private static int lv1, lv2, lv3, lv4;

		public static void Initialize(Menu config, Spell[] initLvSq = default(Spell[])) {
			Config = config;

			var menu = config.AddMenu("自动加点", "自动加点");
			menu.AddBool("启用", "启用", true);
			menu.AddStringList("最主", "最主", new[] { "Q", "W", "E", "R" }, initLvSq == default(Spell[]) ? 3: GetIndexFormSpell(initLvSq[0]));
			menu.AddStringList("优先", "优先", new[] { "Q", "W", "E", "R" }, initLvSq == default(Spell[]) ? 1 : GetIndexFormSpell(initLvSq[1]));
			menu.AddStringList("其次", "其次", new[] { "Q", "W", "E", "R" }, initLvSq == default(Spell[]) ? 1 : GetIndexFormSpell(initLvSq[2]));
			menu.AddStringList("最后", "最后", new[] { "Q", "W", "E", "R" }, initLvSq == default(Spell[]) ? 1 : GetIndexFormSpell(initLvSq[3]));
			menu.AddSlider("升级等级", "升级等级",2,6,1);
			menu.AddSlider("加点延迟", "加点延迟", 700,0,1000);

			Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
		}

		private static int GetIndexFormSpell(Spell s) {
			switch (s.Slot)
			{
				case SpellSlot.Q:
					return 0;
					break;
				case SpellSlot.W:
					return 1;
					break;
				case SpellSlot.E:
					return 2;
					break;
				case SpellSlot.R:
					return 3;
					break;
			}
			return -1;
		}

		private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args) {
			if (Config.GetBool("启用"))
			{
				lv1 = Config.GetStringIndex("最主");
				lv2 = Config.GetStringIndex("优先");
				lv3 = Config.GetStringIndex("其次");
				lv4 = Config.GetStringIndex("最后");

				if (lv2 == lv3 || lv2 == lv4 || lv3 == lv4 || lv1 == lv2 || lv1 == lv3 || lv1 == lv4)
				{
					Chat.Print("[自动加点]".ToHtml(Color.Gold, FontStlye.Bold)
						+ " "
						+ Config.DisplayName.ToHtml(Color.SkyBlue)
						+ $"你开启了自动加点，但没有设置加点方案.".ToHtml(Color.SkyBlue));
					return;
				}

				int delay = Config.GetSliderValue("加点延迟");
				LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Up(lv1));
                LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => Up(lv2));
                LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => Up(lv3));
                LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => Up(lv4));
			}
		}

		private static void Up(int indx) {
			if (ObjectManager.Player.Level < 4)
			{
				if (indx == 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
				if (indx == 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
				if (indx == 2 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
			}
			else
			{
				if (indx == 0)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
				if (indx == 1)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
				if (indx == 2)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
				if (indx == 3)
					ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
			}
		}
	}
}
