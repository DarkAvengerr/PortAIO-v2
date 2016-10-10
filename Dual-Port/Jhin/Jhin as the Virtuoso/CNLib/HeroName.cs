using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using EloBuddy;

namespace CNLib {
	public class HeroNameEntity {
		public string ChampionName { get; set; }
		public string CNName { get; set; }
		public string[] Names { get; set; }

		public HeroNameEntity(string ChampionName, string CNName, string[] Names) {
			this.ChampionName = ChampionName;
			this.CNName = CNName;
			this.Names = Names;
		}
	}

	public static class HeroName {
		
		#region 初始化 HeroNameList
		public static List<HeroNameEntity> HeroNameList => new List<HeroNameEntity>
		{
			new HeroNameEntity("Aatrox", "剑魔", new[] { "亚托克斯" }),
			new HeroNameEntity("Ahri", "阿狸", new[] { "九尾妖狐" }),
			new HeroNameEntity("Akali", "阿卡丽", new[] { "暗影之拳", "AKL" }),
			new HeroNameEntity("Alistar", "牛头酋长", new[] { "阿利斯塔" }),
			new HeroNameEntity("Anivia", "冰霜凤凰", new[] { "冰鸟", "冰凤", "艾尼维亚" }),
			new HeroNameEntity("Annie", "安妮", new[] { "火女", "黑暗火女" }),
			new HeroNameEntity("Ashe", "寒冰射手", new[] { "艾希" }),
			new HeroNameEntity("Azir", "沙漠皇帝", new[] { "沙皇", "阿兹尔", "脆皮鸡" }),
			new HeroNameEntity("Amumu", "阿木木", new[] { "觞之木乃伊" }),
			new HeroNameEntity("Blitzcrank", "蒸汽机器人", new[] { "布里茨" }),
			new HeroNameEntity("Bard", "巴德", new[] { "星界游神" }),
			new HeroNameEntity("Brand", "火男", new[] { "布兰德", "复仇烈焰" }),
			new HeroNameEntity("Braum", "布隆", new[] { "弗雷尔卓德之心" }),
			new HeroNameEntity("Caitlyn", "皮城女警", new[] { "凯瑟琳" }),
			new HeroNameEntity("Cassiopeia", "蛇女", new[] { "蛇魔之拥", "卡西奥佩娅" }),
			new HeroNameEntity("Cho'Gath", "虚空恐惧", new[] { "科加斯", "大虫子" }),
			new HeroNameEntity("Corki", "飞行投弹手", new[] { "库奇", "飞机" }),
			new HeroNameEntity("Darius", "诺克萨斯之手", new[] { "德莱厄斯", "诺手", "小学生之手" }),
			new HeroNameEntity("Diana", "黛安娜", new[] { "皎月女神" }),
			new HeroNameEntity("Dr.Mundo", "蒙多", new[] { "祖安狂人" }),
			new HeroNameEntity("Draven", "德莱文", new[] { "荣耀行刑官" }),
			new HeroNameEntity("Ekko", "艾克", new[] { "时间刺客" }),
			new HeroNameEntity("Elise", "蜘蛛女皇", new[] { "伊莉丝" }),
			new HeroNameEntity("Evelynn", "寡妇制造者", new[] { "伊芙琳" }),
			new HeroNameEntity("Ezreal", "伊泽瑞尔", new[] { "EZ", "探险家" }),
			new HeroNameEntity("Fiddlesticks", "末日使者", new[] { "费德提克" }),
			new HeroNameEntity("Fiora", "无双剑姬", new[] { "菲奥娜" }),
			new HeroNameEntity("Fizz", "菲兹", new[] { "小鱼人", "潮汐海灵" }),
			new HeroNameEntity("Galio", "加里奥", new[] { "哨兵之殇" }),
			new HeroNameEntity("Gangplank", "海盗船长", new[] { "普郎克" }),
			new HeroNameEntity("Garen", "盖伦", new[] { "德玛西亚正义" }),
			new HeroNameEntity("Gnar", "纳尔", new[] { "迷失之牙" }),
			new HeroNameEntity("Gragas", "酒桶", new[] { "古拉加斯" }),
			new HeroNameEntity("Graves", "男枪", new[] { "格雷福斯", "亡命之徒" }),
			new HeroNameEntity("Hecarim", "人马", new[] { "战争之影", "赫卡里姆" }),
			new HeroNameEntity("Heimerdinger", "大发明家", new[] { "大头", "黑默丁格" }),
			new HeroNameEntity("Irelia", "刀妹", new[] { "刀锋意志", "艾瑞莉娅" }),
			new HeroNameEntity("Janna", "风女", new[] { "风暴女神", "迦娜" }),
			new HeroNameEntity("JarvanIV", "德玛西亚皇子", new[] { "嘉文四世" }),
			new HeroNameEntity("Jax", "武器大师", new[] { "贾克斯" }),
			new HeroNameEntity("Jayce", "杰斯", new[] { "未来守护者" }),
			new HeroNameEntity("Jinx", "金克丝", new[] { "暴走萝莉" }),
			new HeroNameEntity("Kalista", "卡莉丝塔", new[] { "复仇之矛", "klst" }),
			new HeroNameEntity("Karma", "卡尔玛", new[] { "天启者", "扇子妈" }),
			new HeroNameEntity("Karthus", "死亡歌颂者", new[] { "卡尔萨斯", "死歌" }),
			new HeroNameEntity("Kassadin", "卡萨丁", new[] { "虚空行者" }),
			new HeroNameEntity("Katarina", "卡特琳娜", new[] { "不祥之刃" }),
			new HeroNameEntity("Kayle", "审判天使", new[] { "凯尔" }),
			new HeroNameEntity("Kennen", "凯南", new[] { "狂暴之心", "电耗子" }),
			new HeroNameEntity("Kha'Zix", "卡兹克", new[] { "虚空掠夺者", "螳螂" }),
			new HeroNameEntity("Kindred", "千珏", new[] { "永猎双子" }),
			new HeroNameEntity("Kog'Maw", "深渊巨口", new[] { "大嘴", "克格莫" }),
			new HeroNameEntity("LeBlanc", "诡术妖姬", new[] { "乐芙兰" }),
			new HeroNameEntity("LeeSin", "盲僧", new[] { "李青", "瞎子" }),
			new HeroNameEntity("Leona", "曙光女神", new[] { "蕾欧娜", "日女" }),
			new HeroNameEntity("Lissandra", "丽桑卓", new[] { "冰霜女巫", "冰女" }),
			new HeroNameEntity("Lucian", "卢锡安", new[] { "奥巴马", "圣枪游侠" }),
			new HeroNameEntity("Lulu", "璐璐", new[] { "仙灵女巫" }),
			new HeroNameEntity("Lux", "光辉女郎", new[] { "拉克丝" }),
			new HeroNameEntity("Malphite", "熔岩巨兽", new[] { "墨菲特", "石头人" }),
			new HeroNameEntity("Malzahar", "玛尔扎哈", new[] { "虚空先知" }),
			new HeroNameEntity("Maokai", "扭曲树精", new[] { "茂凯", "大树" }),
			new HeroNameEntity("MasterYi", "无极剑圣", new[] { "易大师" }),
			new HeroNameEntity("MissFortune", "女枪", new[] { "赏金猎人", "厄运小姐" }),
			new HeroNameEntity("Mordekaiser", "金属大师", new[] { "莫德凯撒" }),
			new HeroNameEntity("Morgana", "莫甘娜", new[] { "堕落天使" }),
			new HeroNameEntity("Nami", "娜美", new[] { "唤潮鲛姬", "美人鱼" }),
			new HeroNameEntity("Nasus", "狗头", new[] { "沙漠死神", "内瑟斯" }),
			new HeroNameEntity("Nautilus", "深海泰坦", new[] { "诺提勒斯" }),
			new HeroNameEntity("Nidalee", "豹女", new[] { "狂野女猎手", "奈德丽", "奶大力" }),
			new HeroNameEntity("Nocturne", "永恒夜魇", new[] { "魔腾", "夜曲" }),
			new HeroNameEntity("Nunu", "努努", new[] { "雪人骑士" }),
			new HeroNameEntity("Olaf", "狂战士", new[] { "奥拉夫" }),
			new HeroNameEntity("Orianna", "发条魔灵", new[] { "奥莉安娜" }),
			new HeroNameEntity("Pantheon", "潘森", new[] { "战争之王" }),
			new HeroNameEntity("Poppy", "波比", new[] { "钢铁大使" }),
			new HeroNameEntity("Quinn", "奎因", new[] { "德玛西亚之翼" }),
			new HeroNameEntity("Rammus", "披甲龙龟", new[] { "拉莫斯" }),
			new HeroNameEntity("Rek'Sai", "虚空遁地兽", new[] { "雷克塞", "挖掘机" }),
			new HeroNameEntity("Renekton", "荒漠屠夫", new[] { "雷克顿", "鳄鱼" }),
			new HeroNameEntity("Rengar", "狮子狗", new[] { "傲之追猎者", "雷恩加尔" }),
			new HeroNameEntity("Riven", "瑞文", new[] { "放逐之刃", "锐雯" }),
			new HeroNameEntity("Rumble", "兰博", new[] { "机械公敌" }),
			new HeroNameEntity("Ryze", "瑞兹", new[] { "流浪法师" }),
			new HeroNameEntity("Sejuani", "瑟庄妮", new[] { "凛冬之怒", "猪妹" }),
			new HeroNameEntity("Shaco", "恶魔小丑", new[] { "萨科" }),
			new HeroNameEntity("Shen", "慎", new[] { "暮光之眼" }),
			new HeroNameEntity("Shyvana", "龙血武姬", new[] { "希瓦娜" }),
			new HeroNameEntity("Singed", "炼金术士", new[] { "辛吉德" }),
			new HeroNameEntity("Zyra", "婕拉", new[] { "荆棘之兴" }),
			new HeroNameEntity("Zilean", "基兰", new[] { "时光守护者" }),
			new HeroNameEntity("Ziggs", "炸弹人", new[] { "爆破鬼才", "吉格斯" }),
			new HeroNameEntity("Zed", "劫", new[] { "影流之主" }),
			new HeroNameEntity("Zac", "扎克", new[] { "生化魔人", "大大超人" }),
			new HeroNameEntity("Yorick", "掘墓者", new[] { "约里克" }),
			new HeroNameEntity("Yasuo", " 亚索", new[] { "疾风剑豪" }),
			new HeroNameEntity("XinZhao", "赵信", new[] { "德邦总管" }),
			new HeroNameEntity("Xerath", "泽拉斯", new[] { "远古巫灵" }),
			new HeroNameEntity("Wukong", "猴子", new[] { "齐天大圣", "孙悟空" }),
			new HeroNameEntity("Warwick", "狼人", new[] { "嗜血猎手", "沃里克" }),
			new HeroNameEntity("Volibear", "狗熊", new[] { "雷霆咆哮", "沃利贝尔" }),
			new HeroNameEntity("Vladimir", "吸血鬼", new[] { "弗拉基米尔", "猩红收割者" }),
			new HeroNameEntity("Viktor", "维克托", new[] { "机械先驱", "三只手" }),
			new HeroNameEntity("Vi", "蔚", new[] { "皮城执法官" }),
			new HeroNameEntity("Vel'Koz", "虚空之眼", new[] { "维克兹", "大眼怪" }),
			new HeroNameEntity("Veigar", "邪恶小法师", new[] { "维迦" }),
			new HeroNameEntity("Vayne", "薇恩", new[] { "VN", "暗夜猎手" }),
			new HeroNameEntity("Varus", "维鲁斯", new[] { "惩戒之箭", "反向Q", "高德伟", "GodV" }),
			new HeroNameEntity("Urgot", "厄加特", new[] { "首领之傲", "螃蟹" }),
			new HeroNameEntity("Udyr", "乌迪尔", new[] { "兽灵行者" }),
			new HeroNameEntity("Twitch", "图奇", new[] { "老鼠", "瘟疫之源" }),
			new HeroNameEntity("Twisted Fate", "卡牌大师", new[] { "崔斯特" }),
			new HeroNameEntity("Tryndamere", "蛮族之王", new[] { "蛮王", "泰达米尔" }),
			new HeroNameEntity("Trundle", "巨魔之王", new[] { "特朗德尔" }),
			new HeroNameEntity("Tristana", "小炮", new[] { "崔斯塔娜", "麦林炮手" }),
			new HeroNameEntity("Thresh", "锤石", new[] { "魂锁典狱长" }),
			new HeroNameEntity("Teemo", "提莫", new[] { "迅捷斥候", "郭敬明" }),
			new HeroNameEntity("Taric", "宝石骑士", new[] { "塔里克" }),
			new HeroNameEntity("TahmKench", "塔姆", new[] { "河流之王" }),
			new HeroNameEntity("Talon", "男刀", new[] { "泰隆", "刀锋之影" }),
			new HeroNameEntity("Syndra", "辛德拉", new[] { "暗黑元首", "球女" }),
			new HeroNameEntity("Swain", "策士统领", new[] { "斯维因", "乌鸦" }),
			new HeroNameEntity("Soraka", "索拉卡", new[] { "奶妈", "众星之子", "星娘" }),
			new HeroNameEntity("Sona", "琴女", new[] { "琴瑟仙女", "娑娜" }),
			new HeroNameEntity("Skarner", "蝎子", new[] { "斯卡纳", "水晶先锋" }),
			new HeroNameEntity("Sivir", "轮子妈", new[] { "希维尔", "战争女神" }),
			new HeroNameEntity("Sion", "塞恩", new[] { "亡灵战神", "老司机" }),
			new HeroNameEntity("Singed ", "炼金术士", new[] { "辛吉德" }),
			new HeroNameEntity("Illaoi", "海兽祭司", new[] { "俄洛伊", "触手怪" }),
			new HeroNameEntity("Jhin", "烬", new[] { "戏命师" })
		};
		#endregion

		public static string GetHeroName(string ChampionName) {
			var name = HeroNameList.Find(h => h.ChampionName == ChampionName)?.CNName.Trim();
			return string.IsNullOrEmpty(name)? ChampionName: name;
		}

		public static string CnName(this AIHeroClient hero) {
			return MultiLanguage.IsCN ? GetHeroName(hero.ChampionName) : hero.ChampionName;
		}

        public static string ToCN(this string ChampionName, bool enable = true)
        {
            if (enable)
            {
                return GetHeroName(ChampionName);
            }
            else
            {
                return ChampionName;
            }

        }
    }
}
