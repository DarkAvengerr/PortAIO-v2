// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllModes.cs" company="hsCamera">
//      Copyright (c) hsCamera. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using hsCamera.Handlers;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hsCamera.AllModes
{
    internal class AllModes : Program
    {
        public static void CameraMode()
        {
            if (_config.Item("CLH").GetValue<KeyBind>().Active)
                Modes.FarmTracker();
            if (_config.Item("CLC").GetValue<KeyBind>().Active)
                Modes.FarmTracker();
            if (_config.Item("CCombo").GetValue<KeyBind>().Active)
                switch (_config.Item("dynamicmode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        Modes.Normal();
                        break;
                    case 1:
                        Modes.FollowCursor();
                        break;
                    case 2:
                        Modes.EnemyTracker();
                        break;
                }
        }
    }
}