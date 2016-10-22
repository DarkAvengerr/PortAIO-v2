using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Common
{
    using System;
    using System.Collections.Generic;
    using System.Resources;
    using System.Web.Script.Serialization;
    using PortAIO.Properties;

    public static class Language
    {
        public static Dictionary<string, string> Translations = new Dictionary<string, string>();

        static Language()
        {
            LoadLanguage("Chinese");
        }

        public static string Translation(string textToTranslate)
        {
            var textToTranslateToLower = textToTranslate.ToLower();

            return Translations.ContainsKey(textToTranslateToLower)
                ? Translations[textToTranslateToLower]
                : textToTranslate;
        }

        public static bool LoadLanguage(string language)
        {
            try
            {
                var languageStrings =
                    new ResourceManager("Flowers_ADC_Series.Properties.Resources", typeof(Resources).Assembly).GetString
                        (language + "Json");

                if (string.IsNullOrEmpty(languageStrings))
                {
                    return false;
                }

                Translations = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(languageStrings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}