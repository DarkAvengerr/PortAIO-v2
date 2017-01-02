using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    internal class AsheDrawings
    {
        private Ashe _parentAssembly;
        private string _menuName = "Drawings";

        public AsheDrawings(Ashe parentAssembly)
        {
            _parentAssembly = parentAssembly;
        }

        public void Initialize()
        {
            var drawOffMenu = _parentAssembly.Menu.GetItemEndKey("Off",_menuName);
            var drawFillColorMenu = _parentAssembly.Menu.GetItemEndKey("FillColor",_menuName);
            //DrawDamage.DamageToUnit = _parentAssembly.GetComboDamage;

            //DrawDamage.Enabled = drawOffMenu.GetValue<bool>();
            //DrawDamage.Fill = drawFillColorMenu.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFillColorMenu.GetValue<Circle>().Color;

            drawOffMenu.ValueChanged += (sender, eventArgs) =>
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFillColorMenu.ValueChanged += ( sender, eventArgs) =>
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
        }

        public void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = _parentAssembly.Menu.GetItemEndKey("Off", _menuName).GetValue<bool>();
            var drawAutoR = _parentAssembly.Menu.GetItemEndKey("AutoR", _menuName).GetValue<Circle>();
            var autoRRange = _parentAssembly.Menu.GetItemEndKey("Range", "Misc.AutoR").GetValue<Slider>().Value;
            var drawW = _parentAssembly.Menu.GetItemEndKey("W", _menuName).GetValue<Circle>();

            if (!drawOff)
                return;

            if (drawAutoR.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, autoRRange, Color.White);
            }
            if (drawW.Active)
            {
                if (_parentAssembly.GetSpell(Spells.W).Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _parentAssembly.GetSpell(Spells.W).Range, Color.White);
                }

            }

        }
    }
}