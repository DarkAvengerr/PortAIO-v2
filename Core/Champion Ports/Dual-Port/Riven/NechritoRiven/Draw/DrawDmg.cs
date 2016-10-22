using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    #region

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core;
    using Menus;

    using SharpDX;

    #endregion

    internal class DrawDmg
    {
        #region Static Fields

        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        #endregion

        #region Public Methods and Operators

        public static void DmgDraw(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                if (!MenuConfig.Dind) return;

                Indicator.Unit = enemy;

                Indicator.DrawDmg(Dmg.GetComboDamage(enemy), enemy.Health <= Dmg.GetComboDamage(enemy) * .7 ? Color.LawnGreen : Color.Yellow);
            }
        }

        #endregion
    }
}