#region copyrights

//  Copyright 2016 Marvin Piekarek
//  CardDrawing.cs is part of RARETwistedFate.
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

using LeagueSharp.SDK.Utils;
using PortAIO.Properties;
using SharpDX;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RARETwistedFate.TwistedFate
{
    internal class CardDrawing
    {
        internal Cards ActiveCard;
        internal Vector2 PosCard;

        public CardDrawing(CardsFrame cF, Vector2 extrapos)
        {
            PosCard = cF.PosDraw + extrapos;
            ActiveCard = Cards.BlueCard;

            BlueCard = new Render.Sprite(Resources.BlaueKarte_jpg, PosCard)
            {
                Scale = new Vector2(0.90f, 0.90f)
            };
            BlueCard.Add();

            RedCard = new Render.Sprite(Resources.RoteKarte_jpg, PosCard)
            {
                Scale = new Vector2(0.90f, 0.90f)
            };

            YellowCard = new Render.Sprite(Resources.GoldeneKarte_jpg, PosCard)
            {
                Scale = new Vector2(0.90f, 0.90f)
            };

            OffCard = new Render.Sprite(Resources.offKarte, PosCard)
            {
                Scale = new Vector2(0.90f, 0.90f)
            };
        }

        internal Render.Sprite BlueCard { get; set; }
        internal Render.Sprite RedCard { get; set; }
        internal Render.Sprite YellowCard { get; set; }
        internal Render.Sprite OffCard { get; set; }

        internal bool ShowButton()
        {
            return MenuTwisted.MainMenu["W"]["ShowButton"];
        }

        internal void ChangeDrawCard()
        {
            switch (ActiveCard)
            {
                case Cards.BlueCard:
                    BlueCard.Remove();
                    RedCard.Add();
                    ActiveCard = Cards.RedCard;
                    break;
                case Cards.RedCard:
                    RedCard.Remove();
                    YellowCard.Add();
                    ActiveCard = Cards.GoldCard;
                    break;
                case Cards.GoldCard:
                    YellowCard.Remove();
                    OffCard.Add();
                    ActiveCard = Cards.OffCard;
                    break;
                case Cards.OffCard:
                    OffCard.Remove();
                    BlueCard.Add();
                    ActiveCard = Cards.BlueCard;
                    break;
            }
        }

        internal void MoveButtons(Vector2 newPosition, Vector2 extra)
        {
            PosCard = newPosition + extra;

            BlueCard.Position = PosCard;
            BlueCard.PositionUpdate = () => PosCard;
            BlueCard.PositionUpdate = null;

            RedCard.Position = PosCard;
            RedCard.PositionUpdate = () => PosCard;
            RedCard.PositionUpdate = null;

            YellowCard.Position = PosCard;
            YellowCard.PositionUpdate = () => PosCard;
            YellowCard.PositionUpdate = null;

            OffCard.Position = PosCard;
            OffCard.PositionUpdate = () => PosCard;
            OffCard.PositionUpdate = null;
        }

        internal void ActivateMe()
        {
            ActiveCard = Cards.BlueCard;
            BlueCard.Add();
        }

        internal void DisableMe()
        {
            YellowCard.Remove();
            BlueCard.Remove();
            RedCard.Remove();
            OffCard.Remove();
        }
    }
}