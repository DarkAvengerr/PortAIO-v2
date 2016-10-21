using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.Translations
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    internal interface ITranslation
    {
        #region Public Properties

        string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        Dictionary<string, string> Strings();

        #endregion
    }
}