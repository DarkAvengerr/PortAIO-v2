using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using System.Threading;

namespace CNLib
{
    public static class MultiLanguage
    {
        public static bool IsCN { get; private set; }

        private static Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
        private static Dictionary<string, Dictionary<string, string>> DictionaryList = new Dictionary<string, Dictionary<string, string>>();

        static MultiLanguage()
        {
            IsCN = IsChinese();
            Translations = IsCN ? ChineseDictionary : EnglishDictionary;
            //DeBug.WriteConsole("[MultiLanguage]", $"语言设置完毕 是否为中文{IsCN}");
        }

        public static string TranslatTo(this string textToTranslate, string language)
        {

            if (string.IsNullOrEmpty(textToTranslate))
            {
                return "";
            }
            if (!DictionaryList.Keys.Contains(language))
            {
                return textToTranslate;
            }

            Dictionary<string, string> Translations = null;
            if (language == "Chinese")
            {
                Translations = ChineseDictionary;
            }
            else if (language == "English")
            {
                Translations = EnglishDictionary;
            }
            else
            {
                Translations = DictionaryList[language];
            }

            var show = string.Empty;
            var textToTranslateToLower = textToTranslate.ToLower();
            if (Translations.ContainsKey(textToTranslateToLower))
            {
                show = Translations[textToTranslateToLower];
            }
            else if (Translations.ContainsKey(textToTranslate))
            {
                show = Translations[textToTranslate];
            }
            else
            {
                show = textToTranslate;
            }
            return show;
        }

        public static string _(string textToTranslate)
        {
            var show = string.Empty;
            if (string.IsNullOrEmpty(textToTranslate))
            {
                return "";
            }

            //DeBug.WriteConsole("[翻译]",$"原文{textToTranslate}");
            var textToTranslateToLower = textToTranslate.ToLower();
            if (Translations.ContainsKey(textToTranslateToLower))
            {
                show = Translations[textToTranslateToLower];
                //DeBug.WriteConsole("[翻译]", $"小写{show}");
            }
            else if (Translations.ContainsKey(textToTranslate))
            {
                show = Translations[textToTranslate];
                //DeBug.WriteConsole("[翻译]", $"不变{show}");
            }
            else
            {
                show = textToTranslate;
                //DeBug.WriteConsole("[翻译]", $"直接{show}");
            }
            return show;
        }

        public static void AddLanguage(Dictionary<string, Dictionary<string, string>> LanguageDictionary)
        {
            DictionaryList = LanguageDictionary;

            if (LanguageDictionary.Keys.Contains("English"))
            {
                foreach (var item in LanguageDictionary["English"])
                {
                    EnglishDictionary.Add(item.Key, item.Value);
                }
            }
            if (LanguageDictionary.Keys.Contains("Chinese"))
            {
                foreach (var item in LanguageDictionary["Chinese"])
                {
                    ChineseDictionary.Add(item.Key, item.Value);
                }
            }

        }

        private static bool IsChinese()
        {
            return false;
        }

        private static Dictionary<string, string> EnglishDictionary { get; set; } = new Dictionary<string, string>()
        {
            { "脚本信息","Assembly Info"},
            { "如果你喜欢这个脚本，记得在脚本库中点赞！","Don't forget to Upvote in the Assembly Database if you like this"},
            { "作者：晴依","Author: Asuvril"},
            { "[版本检查]","[Version Check]"},
            { "有新版本","New version available"},
            { "的版本检查发生了异常","Version check exception"},
            { "[新闻]","[News]"},
            { "提示新闻","Show News"},
            { "检查版本","Version Check"},
            { "[自动加点]","[Auto Level]"},
            { "自动加点","Auto Level Up"},
            { "启用","Enable"},
            { "最主","Most Important"},
            { "优先","Important"},
            { "其次","General"},
            { "最后","Least"},
            { "升级等级","Start Level"},
            { "加点延迟","Delay befor level up"},
            { "你开启了自动加点，但没有设置加点方案.","Level sequence has worng!"},
        };

        private static Dictionary<string, string> ChineseDictionary { get; set; } = new Dictionary<string, string>
        {
            { "Drawings","显示设置"},
            { "AACircle","平A范围"},
            { "Enemy AA circle","敌人平A范围"},
            { "HoldZone","待命区域"},
            { "Line Width","线圈宽度"},
            { "Last Hit Helper","补刀标识"},
            { "Misc","其它设置"},
            { "Hold Position Radius","待命区域半径"},
            { "Priorize farm over harass","优先补刀，其次消耗"},
            { "Auto attack wards","平A眼"},
            { "Auto attack pets & traps","平A宠物/分身"},
            { "Auto attack gangplank barrel","平A船长的桶"},
            {"Jungle clear small first","清野先清小" },
            {"Don't kite if Attack Speed > 2.5","攻速>2.5禁止走砍" },
            {"Focus minions over objectives","优先小兵，其次其它物品(眼/花/桶等)" },
            {"Use Missile Check","检查发射物" },
            {"Extra windup time","额外收手时间" },
            {"Farm delay","补刀延迟" },
            {"Last hit","补刀" },
            {"Mixed","消耗" },
            {"Freeze","保持兵线" },
            {"LaneClear","清线" },
            {"Combo","连招" },
            {"Combo without moving","连招但不跟随鼠标移动" },
            {"","" },
        };
    }
}
