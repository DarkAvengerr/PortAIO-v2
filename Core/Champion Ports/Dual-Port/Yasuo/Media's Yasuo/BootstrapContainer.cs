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
                Console.WriteLine("1");
                if (GlobalVariables.Assembly != null)
                {
                    Console.WriteLine("2");
                    return;
                }

                Console.WriteLine("3");

                if (GlobalVariables.ChampionDependent)
                {
                    Console.WriteLine("4");
                    if (GlobalVariables.SupportedChampions.Contains(GlobalVariables.Player.ChampionName.ToLower()))
                    {
                        Console.WriteLine("5");
                        switch (GlobalVariables.Player.ChampionName)
                        {
                            case "Yasuo":
                                Console.WriteLine("6");
                                GlobalVariables.Assembly = new Assembly(new ChampionYasuo());
                                Console.WriteLine("7");
                                break;
                        }
                    }
                    Console.WriteLine("8");
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