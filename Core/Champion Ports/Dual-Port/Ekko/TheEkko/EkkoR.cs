using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko
{
    class EkkoR : Skill
    {
        private MenuItem _ultMin, _ultMinHealth, _ultSave;

        public EkkoR(Spell spell)
            : base(spell)
        {

        }


        public override void Initialize(IMainContext context, ComboProvider combo)
        {
            _ultMin = context.GetRootMenu().SubMenu("Combo").Item("Combo.MinUltEnemies");
            _ultMinHealth = context.GetRootMenu().SubMenu("Combo").Item("Combo.MinUltHealth%");
            _ultSave = context.GetRootMenu().SubMenu("Combo").Item("Combo.UltforSave");
            base.Initialize(context, combo);
        }



        public override void Cast(AIHeroClient target, bool force = false, HitChance minChance = HitChance.Low)
        {
            if (HasBeenSafeCast()) return;
            var ekko = ObjectManager.Get<GameObject>().FirstOrDefault(item => item.Name == "Ekko_Base_R_TrailEnd.troy");
            if (ekko == null) return;
            var enemyCount = HeroManager.Enemies.Count(enemy => enemy.Distance(ekko.Position) < 400 && enemy.IsValidTarget());
            if (enemyCount >= _ultMin.GetValue<Slider>().Value && ObjectManager.Player.HealthPercent >= _ultMinHealth.GetValue<Slider>().Value)
            {
                SafeCast(() => Spell.Cast());
            }
            if (_ultSave.GetValue<bool>() && (HealthPrediction.GetHealthPrediction(ObjectManager.Player, 1) < 0 && enemyCount == 0 || ObjectManager.Player.HealthPercent < 10 && (target == null || target.HealthPercent > 20)))
            {
                SafeCast(() => Spell.Cast());
            }

        }

        public override int GetPriority()
        {
            return 4;
        }
    }
}
