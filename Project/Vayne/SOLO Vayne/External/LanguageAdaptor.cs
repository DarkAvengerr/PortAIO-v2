using System;
using System.Collections.Generic;
using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.External.Language;
using SoloVayne.External.Language.Languages;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.External
{
    class LanguageAdaptor
    {
        public List<ILanguage> Languages = new List<ILanguage>() { new English(), new Italian() };

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageAdaptor"/> class.
        /// </summary>
        public LanguageAdaptor()
        {
            var builtStringArray = Languages.Select(language => language.GetLanguage()).ToArray();

            var LanguageMenu = new Menu("[SOLO] Language", "solo.vayne.language");
            {
                LanguageMenu.AddStringList("solo.vayne.language.current", "Language:", builtStringArray);
                LanguageMenu.AddBool("solo.vayne.language.select", "Select!").ValueChanged += delegate
                {
                    var language = GetCurrentlySelectedLanguage();
                    if (language != null)
                    {
                        ChangeLanguage(language);
                    }
                    
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                    {
                        LanguageMenu.Item("solo.vayne.language.select").SetValue(false);
                        Chat.Print(
                            $"<b>[<font color='#009aff'>SOLO</font>] Changed language to <font color='#009aff'>{GetCurrentlySelectedLanguage().GetLanguage()}</font></b>");
                    });
                };
            }
            Variables.Menu.AddSubMenu(LanguageMenu);
        }

        /// <summary>
        /// Gets the currently selected language.
        /// </summary>
        /// <returns></returns>
        private ILanguage GetCurrentlySelectedLanguage()
        {
            var currentValue = Variables.Menu.Item("solo.vayne.language.current").GetValue<StringList>().SelectedValue;

            var language = Languages.FirstOrDefault(m => m.GetLanguage().ToLower().Equals(currentValue.ToLower()));

            return language;
        }

        /// <summary>
        /// Changes the language.
        /// </summary>
        /// <param name="Language">The language.</param>
        public void ChangeLanguage(ILanguage Language)
        {
            foreach (var item in Language.GetTranslationDictionary())
            {
                var itemKey = item.Key;
                var itemValue = item.Value;
                var menuItem = Variables.Menu.Item(itemKey);
                try
                {
                    if (menuItem != null)
                    {
                        menuItem.DisplayName = itemValue;
                    }
                    else
                    {
                        LogHelper.AddToLog(new LogItem("Language_Module", "Could Not Translate: "+ itemKey + " with " + itemValue, LogSeverity.Warning));
                    }
                }
                catch (Exception e)
                {
                    LogHelper.AddToLog(new LogItem("Language_Module", e, LogSeverity.Warning));
                }
                
            }
        }
    }
}
