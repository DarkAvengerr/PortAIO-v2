using Geass_Tristana.Misc;
using LeagueSharp.Common;
using System;
using GeassLib.Drawing.Champions;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Drawing
{
    internal class Champs : Core, GeassLib.Interfaces.Drawing.Champion
    {
        public const string MenuItemBase = ".Champions.";
        public const string MenuNameBase = ".Champions Menu";

        private readonly DamageIndicator _damageIndicator = new DamageIndicator(DamageLib.CalcDamage, 1000,true);

        public void OnDrawEnemy(EventArgs args)
        {
            if (!SMenu.Item(MenuItemBase + "Boolean.DrawOnEnemy").GetValue<bool>())
            {
                _damageIndicator.SetFillEnabled(false);
                _damageIndicator.SetKillableEnabled(false);
                return;
            }

            _damageIndicator.SetFillEnabled(SMenu.Item(MenuItemBase + "Boolean.DrawOnEnemy.FillColor").GetValue<Circle>().Active);
            _damageIndicator.SetFill(SMenu.Item(MenuItemBase + "Boolean.DrawOnEnemy.FillColor").GetValue<Circle>().Color);


            _damageIndicator.SetKillableEnabled(SMenu.Item(MenuItemBase + "Boolean.DrawOnEnemy.KillableColor").GetValue<Circle>().Active);
            _damageIndicator.SetKillable(SMenu.Item(MenuItemBase + "Boolean.DrawOnEnemy.KillableColor").GetValue<Circle>().Color);
        }

        public void OnDrawSelf(EventArgs args)
        {
            if (!SMenu.Item(MenuItemBase + "Boolean.DrawOnSelf").GetValue<bool>())
                return;

            if (!Champion.Player.Position.IsOnScreen())
                return;

            if (SMenu.Item(MenuItemBase + "Boolean.DrawOnSelf.ComboColor").GetValue<Circle>().Active && Champion.GetSpellR.Level > 0)
                Render.Circle.DrawCircle(Champion.Player.Position, Champion.GetSpellR.Range, SMenu.Item(MenuItemBase + "Boolean.DrawOnSelf.ComboColor").GetValue<Circle>().Color, 2);

            if (SMenu.Item(MenuItemBase + "Boolean.DrawOnSelf.WColor").GetValue<Circle>().Active && Champion.GetSpellW.Level > 0)
                Render.Circle.DrawCircle(Champion.Player.Position, Champion.GetSpellW.Range, SMenu.Item(MenuItemBase + "Boolean.DrawOnSelf.WColor").GetValue<Circle>().Color, 2);
        }

    }
}