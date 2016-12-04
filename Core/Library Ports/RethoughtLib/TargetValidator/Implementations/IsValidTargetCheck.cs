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
namespace RethoughtLib.TargetValidator.Implementations
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.TargetValidator.Interfaces;

    #endregion

    public class IsValidTargetCheck : ICheckable
    {
        #region Public Methods and Operators

        #region ICheckable Members

        #region Public Methods and Operators

        public bool Check(Obj_AI_Base target)
        {
            return target.IsValidTarget();
        }

        #endregion

        #endregion

        #endregion
    }
}