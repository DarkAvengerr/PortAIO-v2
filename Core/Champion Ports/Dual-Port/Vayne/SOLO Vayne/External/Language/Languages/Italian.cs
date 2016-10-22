using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.External.Language.Languages
{
    class Italian : ILanguage
    {
        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <returns></returns>
        public string GetLanguage()
        {
            return "Italian (Italiano - IT)";
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
                {"solo.vayne.mixed.mode", "Modalità di Harass"},
                {"solo.vayne.laneclear.condemn.jungle", "Usa Condanna sui mob della giungla"},
                {"solo.vayne.misc.tumble.noqintoenemies", "Non usare la Q nei nemici"},
                {"solo.vayne.misc.tumble.qks", "Usa Q per Killsteal"},
                {"solo.vayne.misc.tumble.smartQ", "Usa logica Q: SOLO Vayne"},
                {"solo.vayne.misc.condemn.autoe", "Usa Condanna (E) Automaticamente"},
                {"solo.vayne.misc.condemn.current", "Usa Condanna (E) solo sul target corrente"},
                {"solo.vayne.misc.condemn.save", "SOLO: Salvami!"},
                {"solo.vayne.misc.miscellaneous.antigapcloser", "Respingi i Gapclosers"},
                {"solo.vayne.misc.miscellaneous.interrupter", "Interrompi con Condanna (E)"},
            };

            return Translations;
        }
    }
}
