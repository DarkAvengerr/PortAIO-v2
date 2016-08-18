using EloBuddy;
using LeagueSharp.Common;
namespace YasuoMedia
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::YasuoMedia.Base;
    using CommonEx.Classes;
    using global::YasuoMedia.Yasuo;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    /// <summary>
    ///     Class that loads every component of the assembly. Should only get called once.
    /// </summary>
    internal class BootstrapContainer
    {
        #region Methods

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            try
            {
                if (GlobalVariables.Assembly != null)
                {
                    return;
                }

                if (GlobalVariables.ChampionDependent)
                {
                    if (GlobalVariables.SupportedChampions.Contains(
                            GlobalVariables.Player.ChampionName.ToLower()))
                    {
                        switch (GlobalVariables.Player.ChampionName)
                        {
                            case "Yasuo":
                                GlobalVariables.Assembly = new Assembly(new ChampionYasuo());
                                break;
                        }
                    }

                    GlobalVariables.Assembly = new Assembly(new BaseChampion());
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(
                    string.Format(
                        "[{0}]: BootstrapContainer.Initialize() Failed loading the assembly. Exception: " + ex,
                        GlobalVariables.Name));
            }
        }

        #endregion
    }
}