#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;

namespace TreeSharp
{
    public class Repeater : Decorator
    {
        private int execCount;

        public Repeater(Composite child, int numberOfExecutions = -1)
            : base(child)
        {
            this.execCount = numberOfExecutions;
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            bool endless = execCount == -1;
            int i = 0;
            while(true)
            {
                
                if (DecoratedChild.LastStatus == null || DecoratedChild.LastStatus != RunStatus.Running)
                {
                    DecoratedChild.Stop(context);
                    DecoratedChild.Start(context);
                }
                DecoratedChild.Tick(context);
                i++;
                if (!endless && i == execCount - 1) //break out if iterations are done
                {
                    yield return RunStatus.Success;
                    yield break;
                }
                yield return RunStatus.Running;
            }
        }
    }
}