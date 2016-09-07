#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Draw 
{
    internal class DrawDmg : Core.Core
    {
        private static readonly HpBarDraw DrawHpBar = new HpBarDraw();

        public static void OnDrawEnemy(EventArgs args)
        {
            if (Player.IsDead || !MenuConfig.UseDrawings || !MenuConfig.Dind)
            {
                return;
            }
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget() && !x.IsZombie))
            {
                var EasyKill = Spells.Q.IsReady() && Dmg.IsLethal(enemy)
                      ? new ColorBGRA(0, 255, 0, 120)
                      : new ColorBGRA(255, 255, 0, 120);
                DrawHpBar.unit = enemy;
                DrawHpBar.drawDmg(Dmg.ComboDmg(enemy), EasyKill);
            }
        }
    }
}
