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
    public enum Policy { ONE_MET, ALL_MET }
    /// <summary>
    ///   This class is not yet implemented!
    /// </summary>
    public class Parallel : GroupComposite
    {
        private Policy failurePolicy, successPolicy;

        public Parallel(Policy failurePolicy, Policy successPolicy, params Composite[] children)
            : base(children)
        {
            this.failurePolicy = failurePolicy;
            this.successPolicy = successPolicy;
        }

        public Parallel(Policy failurePolicy, Policy successPolicy, ContextChangeHandler contextChange, params Composite[] children)
            : this(failurePolicy, successPolicy, children)
        {
            ContextChanger = contextChange;
        }

        public override void Start(object context)
        {
            base.Start(context);
            foreach (Composite node in Children)
                node.Start(context);
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            if (ContextChanger != null)
                context = ContextChanger(context);

            bool anySuccess, anyFailure, allFailure, allSuccess;
            while (true)
            {
                allSuccess = true;
                allFailure = true;
                anySuccess = anyFailure = false;
              
                foreach (Composite node in Children)
                {
                    if(node.LastStatus == null || node.LastStatus == RunStatus.Running)
                    {
                        Selection = node;
                        node.Tick(context);
                        Selection = null;
                    }

                    if (node.LastStatus == RunStatus.Success)
                    {
                        anySuccess = true;
                        allFailure = false;
                    }
                    else if (node.LastStatus == RunStatus.Failure)
                    {
                        anyFailure = true;
                        allSuccess = false;
                    }
                    else
                    {
                        //running
                        allSuccess = allFailure = false;
                    }
                }
                if((anySuccess && successPolicy == Policy.ONE_MET) || allSuccess)
                {
                    yield return RunStatus.Success;
                    yield break;
                }
                if ((anyFailure && failurePolicy == Policy.ONE_MET) || allFailure)
                {
                    yield return RunStatus.Failure;
                    yield break;
                }
                yield return RunStatus.Running;
            }
        }

        public override void Stop(object context)
        {
            base.Stop(context);
            foreach (Composite node in Children)
                node.Stop(context);
        }
    }
}