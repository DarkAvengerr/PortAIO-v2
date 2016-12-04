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
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    #endregion

    public class PlayerMustHaveBuff : GuardianBase
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuff" /> class.
        /// </summary>
        /// <param name="buff">The buff.</param>
        public PlayerMustHaveBuff(string buff)
        {
            this.Func = () => ObjectManager.Player.HasBuff(buff);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuff" /> class.
        /// </summary>
        /// <param name="buffs">The buffs.</param>
        public PlayerMustHaveBuff(IEnumerable<string> buffs)
        {
            this.Func = () => buffs.All(buff => ObjectManager.Player.HasBuff(buff));
        }

        #endregion
    }
}