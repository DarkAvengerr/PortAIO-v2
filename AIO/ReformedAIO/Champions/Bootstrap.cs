using EloBuddy; namespace ReformedAIO.Champions
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.Bootstraps.Implementations;

    #endregion

    internal class Bootstrap : LeagueSharpMultiBootstrap
    {
        #region Constructors and Destructors

        public Bootstrap(List<LoadableBase> modules, List<string> additionalStrings = null)
            : base(modules, additionalStrings)
        {
            Console.WriteLine("Reformed AIO - Loaded!");
            Chat.Print(
                "<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded</font></b>");
            Chat.Print(
                "<b><font color=\"FFFFFF\">Don't forget to</font></b><b><font color=\"#00ff00\"> Upvote</font></b><b><font color=\"FFFFFF\"> In DB If You Liked It</font></b>");
        }

        #endregion
    }
}