using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCS_LeBlanc.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Modes.Combo
{
    internal static class WRQE
    {
        public static void WRQECombo()
        {
            if (ObjectManager.Player.HasBuff("LeblancSlide") && !Utilities.Enabled("w.combo.back"))
            {
                return;
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.W.Range)))
                {
                    var hit = Spells.W.GetPrediction(enemy);
                    if (hit.Hitchance >= Utilities.HikiChance("w.hit.chance"))
                    {
                        Spells.W.Cast(enemy);
                    }
                }
            }

            if (!Spells.W.IsReady() && Spells.R.IsReady() && Utilities.Enabled("r.combo") && Utilities.UltimateKey() == "W")
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range)))
                {
                    var hit = Spells.R.GetPrediction(enemy);
                    if (hit.Hitchance >= HitChance.Medium)
                    {
                        Spells.R.Cast(enemy);
                    }
                }
            }

            if (!Spells.W.IsReady() && !Spells.R.IsReady() && Spells.Q.IsReady() && Utilities.Enabled("q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.CastOnUnit(enemy);
                }
            }

            if (!Spells.W.IsReady() && !Spells.R.IsReady() && !Spells.Q.IsReady() && 
                Spells.E.IsReady() && Utilities.Enabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.E.Range)))
                {
                    var hit = Spells.E.GetPrediction(enemy);
                    if (hit.Hitchance >= Utilities.HikiChance("e.hit.chance"))
                    {
                        Spells.E.Cast(enemy);
                    }
                }
            }
        }

        public static void WRQESelected(AIHeroClient enemy)
        {
            if (ObjectManager.Player.HasBuff("LeblancSlide") && !Utilities.Enabled("w.combo.back"))
            {
                return;
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && enemy.IsValidTarget(Spells.W.Range))
            {
                var hit = Spells.W.GetPrediction(enemy);
                if (hit.Hitchance >= HitChance.Medium)
                {
                    Spells.W.Cast(enemy);
                }
            }

            if (!Spells.W.IsReady() && Spells.R.IsReady() && Utilities.Enabled("r.combo") && Utilities.UltimateKey() == "W"
                && enemy.IsValidTarget(Spells.R.Range))
            {
                var hit = Spells.R.GetPrediction(enemy);
                if (hit.Hitchance >= HitChance.Medium)
                {
                    Spells.R.Cast(enemy);
                }
            }

            if (!Spells.W.IsReady() && !Spells.R.IsReady() && Spells.Q.IsReady() && Utilities.Enabled("q.combo")
                && enemy.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastOnUnit(enemy);
            }

            if (!Spells.W.IsReady() && !Spells.R.IsReady() && !Spells.Q.IsReady() &&
                Spells.E.IsReady() && Utilities.Enabled("e.combo") && enemy.IsValidTarget(Spells.E.Range))
            {
                var hit = Spells.E.GetPrediction(enemy);
                if (hit.Hitchance >= HitChance.Medium)
                {
                    Spells.E.Cast(enemy);
                }
            }
        }

        public static void Init()
        {
            switch (Menus.Config.Item("combo.style").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    WRQESelected(TargetSelector.SelectedTarget);
                    break;
                case 1:
                    WRQECombo();
                    break;
            }
        }
    }
}
