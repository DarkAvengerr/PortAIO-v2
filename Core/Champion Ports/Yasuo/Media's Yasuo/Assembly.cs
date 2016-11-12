using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp.Common;

    #endregion

    class Assembly
    {
        #region Fields

        /// <summary>
        ///     The features of the assembly
        /// </summary>
        public List<IFeatureChild> Features = new List<IFeatureChild>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Assembly" /> class.
        /// </summary>
        public Assembly(IChampion champion)
        {
            if (champion == null || GlobalVariables.RootMenu != null)
            {
                return;
            }

            GlobalVariables.RootMenu = new Menu($"[{GlobalVariables.Author}]: " + champion.Name, string.Format("Root"), true);

            champion.Load();

            Events.Initialize();

            var orbWalkingMenu = new Menu("Orbwalking", "Orbwalking");

            GlobalVariables.RootMenu.AddSubMenu(orbWalkingMenu);

            GlobalVariables.Orbwalker = new Orbwalking.Orbwalker(GlobalVariables.RootMenu.SubMenu("Orbwalking"));

            GlobalVariables.RootMenu.AddToMainMenu();
        }

        #endregion
    }
}