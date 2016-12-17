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

    using SharpDX;

    #endregion

    internal class DrawDmg : Core
    {
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        public static void DmgDraw(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1750)))
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }

                Indicator.Unit = enemy;

                if (MenuConfig.Dind)
                {
                    Indicator.DrawDmg(Dmg.GetComboDamage(enemy),
                       enemy.Health <= Dmg.GetComboDamage(enemy) * .85
                       ? Color.LawnGreen
                       : Color.Yellow);
                }

                if (MenuConfig.R2Draw && Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
                {
                    Indicator.DrawDmg(Dmg.RDmg(enemy), Color.DarkSlateGray);   
                }
            }
        }
    }
}