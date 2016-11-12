using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.Translations
{
    using System.Collections.Generic;

    class English : ITranslation
    {
        public string Name { get; set; } = "English (Default)";

        public Dictionary<string, string> Strings()
        {
            var strings = new Dictionary<string, string> { };

            return strings;
        }
    }
}
