#region copyrights

//  Copyright 2016 Marvin Piekarek
//  TwistedFate.cs is part of RARETwistedFate.
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

using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RARETwistedFate.TwistedFate
{
    internal class TwistedFate
    {
        internal CardManager CardManager; // W skill
        internal CardShot CardShot; // Q spell
        internal AIHeroClient Player;


        public TwistedFate()
        {
            Player = GameObjects.Player;

            if (Player.CharData.BaseSkinName == "TwistedFate")
            {
                CardManager = new CardManager(this);
                CardShot = new CardShot();
                MenuTwisted.Init(this);

                Chat.Print("RARETwistedFate => has been loaded.");
                Game.OnUpdate += Game_OnUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            }
            else
            {
                Chat.Print("RARETwistedFate => not loaded wrong champion detected.");
            }
        }

        private void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.Equals("Gate", StringComparison.InvariantCultureIgnoreCase) &&
                MenuTwisted.MainMenu["R"]["Pick"])
            {
                CardManager.HandleCards(OrbwalkingMode.Combo, true);
            }
            
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (MenuTwisted.MainMenu["Draw"]["Q"] && CardShot.SpellQ.Level >= 1)
            {
                Render.Circle.DrawCircle(Player.Position, CardShot.SpellQ.Range, Color.Aqua, 2);
            }

            if (MenuTwisted.MainMenu["Draw"]["W"] && CardManager.SpellW.Level >= 1)
            {
                Render.Circle.DrawCircle(Player.Position, CardManager.SpellW.Range, Color.Brown, 2);
            }
            if (MenuTwisted.MainMenu["Draw"]["R"])
            {
                Render.Circle.DrawCircle(Player.Position, 5500f, Color.Brown, 2);
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    CardManager.HandleCards(Variables.Orbwalker.ActiveMode, false);
                    if (!MenuTwisted.MainMenu["Q"]["OnlyImmoQ"])
                    {
                        CardShot.HandleQ(Variables.Orbwalker.ActiveMode);
                    }
                    break;

                case OrbwalkingMode.Hybrid:
                    CardManager.HandleCards(Variables.Orbwalker.ActiveMode, false);
                    CardShot.HandleQ(Variables.Orbwalker.ActiveMode);
                    break;

                case OrbwalkingMode.LaneClear:
                    CardManager.HandleCards(Variables.Orbwalker.ActiveMode, false);
                    CardShot.HandleQ(Variables.Orbwalker.ActiveMode);
                    break;

                case OrbwalkingMode.LastHit:
                    CardManager.HandleCards(Variables.Orbwalker.ActiveMode, false);
                    CardShot.HandleQ(Variables.Orbwalker.ActiveMode);
                    break;
            }
            if (CardManager.NeedToCastW)
            {
                CardManager.PickCard(CardManager.GetCardfromString(MenuTwisted.MainMenu["R"]["ActiveCard"].GetValue<MenuList>().SelectedValueAsObject.ToString()));
                
            }
        }
    }
}