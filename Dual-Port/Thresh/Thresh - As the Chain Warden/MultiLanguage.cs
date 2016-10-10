using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshAsurvil
{
	public class MultiLanguage {
		public static Dictionary<string, string> Chinese { get; private set; } = new Dictionary<string, string> {
			{ "锤石As", "锤石 - 魂锁典狱"},
			{ "走砍设置", "走砍设置"},
			{ "技能设置", "技能设置"},
			{ "位移Q","敌人位移自动Q" },
			{ "不用Q2","不自动Q2飞过去" },
			{ "人数比", "敌人数>队友数?个不自动Q2"},
			{ "Q名单","Q 名单"},
			{ "提示","如果对面有人开L#，请设置不Q他"},
			{ "逃跑设置", "逃跑设置"},
			{ "逃跑", "逃跑" },
			{ "E推人", "自动E推人" },
			{ "Q野怪", "自动Q野怪 [测试]"},
			{ "预判设置", "预判设置"},
			{ "预判模式", "预判模式"},
			{ "命中率", "命中率"},
			{ "大招设置", "大招设置"},
			{ "大招人数", "大招人数"},
			{ "自动大招模式", "自动大招模式"},
			{ "辅助模式", "辅助模式"},
			{ "辅助模式距离", "辅助模式距离"},
			{ "辅助目标", "攻击ADC的目标 [测试]"},
			{ "显示设置", "显示设置"},
			{"技能可用才显示", "技能可用才显示" },
			{ "显示Q", "显示 Q 范围"},
			{ "显示W", "显示 W 范围"},
			{ "显示E", "显示 E 范围"},
			{ "显示R", "显示 R 范围"},
			{ "智能施法", "智能施法"},
			{"智能施法标签", "启用这个功能，按下QWE会自动施法技能" },
			{ "智能Q", "智能 Q"},
			{ "智能W", "智能 W"},
			{ "智能E", "智能 E"},
			{ "防御塔设置", "防御塔设置"},
			{"控制塔攻击的敌人", "自动Q/E防御塔攻击的敌人"},
			{ "拉敌人进塔", "自动Q/E敌人进塔"},
			{ "Q不进敌塔", "不自动Q2进敌方防御塔"},
			{ "语言选择", "MultiLanguage Settings"},
			{ "选择语言", "Selecte Language"},
			{ "标识目标", "标识目标"},
			{ "调试","调试"}

		};

		public static Dictionary<string, string> English { get; private set; } = new Dictionary<string, string> {
			{ "锤石As", "Thresh As the Chain Warden"},
			{ "走砍设置", "Orbwalker"},
			{ "技能设置", "Spell Settings"},
			{ "位移Q", "Auto Q Dash Enemy"},
			{ "不用Q2", "Don't Auto Q2"},
			{ "人数比", "Don't Q2 if Enemies > allies"},
			{ "Q名单","Q List"},
			{ "提示","if you find some gays seems use L# too,Dont Q him"},
			{ "逃跑设置", "Flee Settings"},
			{"逃跑", "Flee" },
			{"E推人", "Auto E push" },
			{ "Q野怪", "Auto Q Jungle [TEST]"},
			{ "预判设置", "Predict Settings"},
			{ "预判模式", "Prediction Mode"},
			{ "命中率", "HitChance"},
			{ "大招设置", "Box Settings"},
			{ "大招人数", "Box Count"},
			{ "自动大招模式", "Box Mode"},
			{ "辅助模式", "Support Mode"},
			{ "辅助模式距离", "Support Mode Range"},
			{ "辅助目标", "Attack ADC's Target [TEST]"},
			{ "显示设置", "Drawing Settings"},
			{"技能可用才显示", "Draw when skill is ready" },
			{ "显示Q", "Draw Q Range"},
			{ "显示W", "Draw W Range"},
			{ "显示E", "Draw E Range"},
			{ "显示R", "Draw R Range"},
			{ "智能施法", "Smart Cast"},
			{"智能施法标签", "Enable Follow Options,Prss ALT+Q/W/E Auto Cast Spell" },
			{ "智能Q", "Smart Cast Q"},
			{ "智能W", "Smart Cast W"},
			{ "智能E", "Smart Cast E"},
			{ "防御塔设置", "Turret Settings"},
			{"控制塔攻击的敌人", "Q/E ally Turret’s target"},
			{ "拉敌人进塔", "Q/E target into ally turret"},
			{ "Q不进敌塔", "Don't Q2 in enemy turret"},
			{ "语言选择", "多语言设置"},
			{ "选择语言", "选择语言"},
			{ "标识目标", "Draw Target"},
			{ "调试","Debug Mod"}
		};

	}
}
