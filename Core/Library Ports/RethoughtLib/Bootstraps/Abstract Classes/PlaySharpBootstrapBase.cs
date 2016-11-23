//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:43 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    using RethoughtLib.Bootstraps.Interfaces;

    #endregion

    public abstract class PlaySharpBootstrapBase : IBootstrap
    {
        #region Fields

        /// <summary>
        ///     The modules
        /// </summary>
        public List<LoadableBase> Modules = new List<LoadableBase>();

        /// <summary>
        ///     Gets or sets the string that gets checked for check for.
        /// </summary>
        /// <value>
        ///     The check for.
        /// </value>
        protected internal List<string> Strings = new List<string>();

        protected List<LoadableBase> LoadedModules = new List<LoadableBase>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <exception cref="ArgumentException">There can't be multiple similar modules in the PlaySharpBootstrap.</exception>
        public virtual void AddModule(LoadableBase module)
        {
            if (this.Modules.Contains(module)) throw new ArgumentException("There can't be multiple similar modules in the PlaySharpBootstrap.");

            this.Modules.Add(module);
        }

        /// <summary>
        ///     Adds the module.
        /// </summary>
        /// <param name="modules">the modules</param>
        /// <exception cref="ArgumentException">There can't be multiple similar modules in the PlaySharpBootstrap.</exception>
        public virtual void AddModules(IEnumerable<LoadableBase> modules)
        {
            var loadables = modules as IList<LoadableBase> ?? modules.ToList();

            if ((from moduleToAdd in loadables
                 from existingModule in this.Modules
                 where moduleToAdd.Equals(existingModule)
                 select moduleToAdd).Any()) throw new ArgumentException("There can't be multiple similar modules in the PlaySharpBootstrap.");

            this.Modules.AddRange(loadables);
        }

        /// <summary>
        ///     Adds a string with witch the bootstrap is checking for modules.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void AddString(string value)
        {
            if (string.IsNullOrEmpty(value) || (this.Strings == null) || this.Strings.Contains(value)) return;

            this.Strings.Add(value);
        }

        /// <summary>
        ///     Adds strings with witch the bootstrap is checking for modules.
        /// </summary>
        /// <param name="values">the values</param>
        public virtual void AddStrings(IEnumerable<string> values)
        {
            var validValues =
                this.Strings.SelectMany(s => values, (s, value) => new { s, value })
                    .Where(@t => !@t.s.Equals(@t.value))
                    .Select(@t => @t.value)
                    .ToList();

            this.Strings.AddRange(validValues);
        }

        /// <summary>
        ///     Removes the module.
        /// </summary>
        /// <param name="module">The module.</param>
        public virtual void RemoveModule(LoadableBase module)
        {
            if (!this.Modules.Contains(module)) return;

            this.Modules.Remove(module);
        }

        /// <summary>
        ///     Removes a string with witch the bootstrap was checking for modules.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void RemoveString(string value)
        {
            if (!this.Strings.Contains(value)) return;

            this.Strings.Remove(value);
        }

        /// <summary>
        ///     Removes strings with witch the bootstrap was checking for modules.
        /// </summary>
        /// <param name="values">the values</param>
        public virtual void RemoveStrings(IEnumerable<string> values)
        {
            var strings = values as IList<string> ?? values.ToList();

            foreach (var @string in this.Strings) foreach (var value in strings) if (@string.Equals(value)) this.Strings.Remove(value);
        }

        /// <summary>
        ///     Compares module names with entries in the strings list. If they match it will load the module.
        /// </summary>
        public virtual void Run()
        {
            CustomEvents.Game.OnGameLoad += delegate(EventArgs args)
                {
                    if (!this.Modules.Any()) throw new InvalidOperationException("There are no modules in the Bootstrap to load.");

                    if (!this.Strings.Any())
                        throw new InvalidOperationException(
                                  "There are no strings in the Bootstrap to make a check with modules.");

                    var unknownModulesCount = 0;

                    foreach (var module in this.Modules)
                    {
                        if (string.IsNullOrWhiteSpace(module.DisplayName) || !module.Tags.Any())
                        {
                            unknownModulesCount++;
                            continue;
                        }

                        foreach (var @string in this.Strings)
                        {
                            Console.WriteLine(@string);
                            foreach (var tag in module.Tags)
                            {
                                Console.WriteLine(tag);
                                if (!tag.Equals(@string)) continue;

                                module.Load();
                                this.LoadedModules.Add(module);
                            }
                        }
                    }

                    Console.WriteLine(
                        $"[{this}] {unknownModulesCount} unknown Modules, {this.LoadedModules.Count} loaded Modules");

                    if (unknownModulesCount > 0)
                        Console.WriteLine(
                            $"[{this}] Please consider tagging and naming your unknown modules. The name must not be null or whitespace.");
                };
        }

        #endregion
    }
}