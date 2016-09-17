using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.External.Language.Languages
{
    class English : ILanguage
    {
        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <returns></returns>
        public string GetLanguage()
        {
            return "English (English - US)";
        }

        /// <summary>
        /// Gets the localized string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetLocalizedString(string name)
        {
            var dictionary = GetTranslationDictionary();
            if (dictionary.ContainsKey(name) && dictionary[name] != null)
            {
                return dictionary[name];
            }

            return "";
        }

        /// <summary>
        /// Gets the translation dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTranslationDictionary()
        {
            var Translations = new Dictionary<string, string>()
            {
                {"solo.vayne.mixed.mode", "Harass Mode"},
                {"solo.vayne.laneclear.condemn.jungle", "Condemn Jungle Mobs"},
                {"solo.vayne.misc.tumble.noqintoenemies", "Don't Q into enemies"},
                {"solo.vayne.misc.tumble.qks", "Q for Killsteal"},
                {"solo.vayne.misc.tumble.smartQ", "Use SOLO Vayne Q Logic"},
                {"solo.vayne.misc.condemn.autoe", "Auto E"},
                {"solo.vayne.misc.condemn.current", "Only E Current Target"},
                {"solo.vayne.misc.condemn.save", "SOLO: Save Me"},
                {"solo.vayne.misc.miscellaneous.antigapcloser", "Antigapcloser"},
                {"solo.vayne.misc.miscellaneous.interrupter", "Interrupter"},
            };

            return Translations;
        }
    }
}
