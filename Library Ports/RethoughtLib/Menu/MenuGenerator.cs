using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Menu
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using RethoughtLib.Menu.Interfaces;

    #endregion

    #region Using Directives

    #endregion

    // REWORK TODO
    /// <summary>
    ///     Generates a Preset Menu to the given Menu
    /// </summary>
    public class MenuGenerator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerator" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="generator">The menu preset.</param>
        public MenuGenerator(Menu menu, IGenerator generator)
        {
            this.Generator = generator;
            this.Menu = menu;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The menu set
        /// </summary>
        public IGenerator Generator { get; set; }

        /// <summary>
        ///     The menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Generates the menu.
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        ///     Get sure that you declared a valid menuPreset and a valid menu in the
        ///     constructor before generating.
        /// </exception>
        public void Generate()
        {
            if (this.Generator == null || this.Menu == null)
            {
                throw new NullReferenceException(
                    "Get sure that you declared a valid menuPreset and a valid menu in the constructor before generating.");
            }

            this.Generator.Generate(this.Menu);
        }

        #endregion
    }
}