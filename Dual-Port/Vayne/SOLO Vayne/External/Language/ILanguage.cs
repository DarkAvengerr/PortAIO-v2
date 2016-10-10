using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.External.Language
{
    interface ILanguage
    {
        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <returns></returns>
        string GetLanguage();

        /// <summary>
        /// Gets the localized string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetLocalizedString(string name);

        /// <summary>
        /// Gets the translation dictionary.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetTranslationDictionary();
    }
}
