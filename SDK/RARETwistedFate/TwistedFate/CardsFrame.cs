#region copyrights

//  Copyright 2016 Marvin Piekarek
//  CardsFrame.cs is part of RARETwistedFate.
//  RARETwistedFate is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  RARETwistedFate is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with RARETwistedFate. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region usages

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK.Utils;
using PortAIO.Properties;
using SharpDX;
using Render = LeagueSharp.SDK.Utils.Render;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RARETwistedFate.TwistedFate
{
    internal class CardsFrame
    {
        private bool _isActive;
        public CardDrawing CardCombo;

        public CardDrawing CardFarm;
        public Vector2 PosDraw;

        public CardsFrame()
        {
            _isActive = true;
            PosDraw = new Vector2(50, 50) + new Vector2(-10, -10);

            Frame = new Render.Sprite(Resources.frame, PosDraw) {Color = new ColorBGRA(255, 255, 255, 170)};
            Frame.Add();

            CardFarm = new CardDrawing(this, new Vector2(8, 10));
            CardCombo = new CardDrawing(this, new Vector2(Frame.Width/2 + 8, 10));

            Game.OnWndProc += Game_OnWndProc;
        }

        private Render.Sprite Frame { get; }

        internal bool ShowButton()
        {
            return MenuTwisted.MainMenu["W"]["ShowButton"];
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (ShowButton())
            {
                if (!_isActive)
                {
                    _isActive = true;
                    Activate();
                }
                else if ((args.Msg == (uint) WindowsMessages.WM_LBUTTONUP) &&
                         MouseOnButton(CardCombo.PosCard, CardCombo.YellowCard) && args.WParam != 5)
                {
                    CardCombo.ChangeDrawCard();
                }
                else if ((args.Msg == (uint) WindowsMessages.WM_LBUTTONUP) &&
                         MouseOnButton(CardFarm.PosCard, CardCombo.YellowCard) && args.WParam != 5)
                {
                    CardFarm.ChangeDrawCard();
                }
                else if ((args.Msg == (uint) WindowsMessages.WM_MOUSEMOVE) && (args.WParam == 5)
                         && MouseOnButton(PosDraw, Frame))
                {
                    MoveButtons(new Vector2(Utils.GetCursorPos().X - Frame.Width/2f,
                        Utils.GetCursorPos().Y - Frame.Height/2f));
                }
            }
            else
            {
                CardCombo.ActiveCard = Cards.Disabled;
                CardFarm.ActiveCard = Cards.Disabled;
                Disable();
            }
        }

        internal bool MouseOnButton(Vector2 posButton, Render.Sprite button)
        {
            var pos = Utils.GetCursorPos();

            return (pos.X >= posButton.X) && pos.X <= posButton.X + button.Width
                   && pos.Y >= posButton.Y && pos.Y <= posButton.Y + button.Height;
        }

        internal void MoveButtons(Vector2 newPosition)
        {
            PosDraw = newPosition;

            Frame.Position = PosDraw;
            Frame.PositionUpdate = () => PosDraw;
            Frame.PositionUpdate = null;

            CardFarm.MoveButtons(Frame.Position, new Vector2(8, 10));
            CardCombo.MoveButtons(Frame.Position, new Vector2(Frame.Width/2 + 8, 10));
        }

        public void Activate()
        {
            Frame.Add();
            CardCombo.ActivateMe();
            CardFarm.ActivateMe();
        }

        public void Disable()
        {
            _isActive = false;
            Frame.Remove();
            CardCombo.DisableMe();
            CardFarm.DisableMe();
        }
    }
}