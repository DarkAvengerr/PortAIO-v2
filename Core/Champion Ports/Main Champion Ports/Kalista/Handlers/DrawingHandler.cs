using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Drawing;
using Damage = S_Plus_Class_Kalista.Libaries.Damage;

namespace S_Plus_Class_Kalista.Handlers
{
    class DrawingHandler : Core
    {
        private const string _MenuNameBase = ".Drawing Menu";
        private const string _MenuItemBase = ".Drawing.";
        public static void Load()
        {
            var _Menu = new Menu(_MenuNameBase,"drawingMenu");
            _Menu.AddSubMenu(DrawingOnMonsters.DrawingMonsterMenu());
            _Menu.AddSubMenu(DrawingOnChamps.DrawingOnChampionsMenu());
            _Menu.AddSubMenu(DrawingOnMinions.DrawingOnMinionsMenu());
            SMenu.AddSubMenu(_Menu);

            DrawingOnMonsters.DamageToMonster = Damage.DamageCalc.CalculateRendDamage;
            DrawingOnChamps.DamageToEnemy = Damage.DamageCalc.CalculateRendDamage;

            EloBuddy.Drawing.OnDraw += DrawingOnMonsters.OnDrawMonster;
            EloBuddy.Drawing.OnDraw += DrawingOnChamps.OnDrawEnemy;
            EloBuddy.Drawing.OnDraw += DrawingOnChamps.OnDrawSelf;
            EloBuddy.Drawing.OnDraw += DrawingOnMinions.OnMinionDraw;

        }
    }
}
