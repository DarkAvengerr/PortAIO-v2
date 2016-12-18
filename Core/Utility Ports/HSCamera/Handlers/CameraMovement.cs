// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraMovement.cs" company="hsCamera">
//      Copyright (c) hsCamera. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hsCamera.Handlers
{
    internal class CameraMovement : Program
    {
        public static void SemiDynamic(Vector3 position)
        {
            var distance = Camera.ScreenPosition.Distance(position);


            if (distance <= 1)
                return;

            var speed = Math.Max(0.2f, Math.Min(20, distance*0.0007f*20));
            switch (_config.Item("dynamicmode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                {
                    var direction = (position.To2D() - Camera.ScreenPosition).Normalized()*speed;
                    Camera.ScreenPosition = direction + Camera.ScreenPosition;
                }
                    break;
                case 1:
                {
                    var direction = (position.To2D() - Camera.ScreenPosition).Normalized()*
                                    _config.Item("followcurspeed").GetValue<Slider>().Value;
                    Camera.ScreenPosition = direction + Camera.ScreenPosition;
                }
                    break;
                case 2:
                {
                    var direction = (position.To2D() - Camera.ScreenPosition).Normalized()*
                                    _config.Item("followtfspeed").GetValue<Slider>().Value;
                    Camera.ScreenPosition = direction + Camera.ScreenPosition;
                }
                    break;
            }
        }
    }
}