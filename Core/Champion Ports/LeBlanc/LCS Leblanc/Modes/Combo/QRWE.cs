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
    internal static class QRWE
    {
        public static void QRWECombo()
        {
            if (ObjectManager.Player.HasBuff("LeblancSlide") && !Utilities.Enabled("w.combo.back"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") )
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.CastOnUnit(enemy);
                }
            }
            else if (Spells.R.IsReady() && Utilities.Enabled("r.combo") && !Spells.Q.IsReady() && 
                Utilities.UltimateKey() == "Q")
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.R.Range)))
                {
                    Spells.R.CastOnUnit(enemy);
                }
            }
            else if (!Spells.R.IsReady() && !Spells.Q.IsReady() && Spells.W.IsReady() && Utilities.Enabled("w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.W.Range)))
                {
                    var hit = Spells.W.GetPrediction(enemy);
                    if (hit.Hitchance >= Utilities.HikiChance("w.hit.chance"))
                    {
                        Spells.W.Cast(hit.CastPosition);
                    }
                }
            }
            else if (!Spells.R.IsReady() && !Spells.Q.IsReady() && !Spells.W.IsReady() && Spells.E.IsReady() && Utilities.Enabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range)))
                {
                    var hit = Spells.E.GetPrediction(enemy);
                    if (hit.Hitchance >= Utilities.HikiChance("e.hit.chance"))
                    {
                        Spells.E.Cast(hit.CastPosition);
                    }
                }
            }
        }

        public static void QRWESelected(AIHeroClient enemy)
        {
            if (ObjectManager.Player.HasBuff("LeblancSlide") && !Utilities.Enabled("w.combo.back"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && enemy.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastOnUnit(enemy);
            }
            else if (Spells.R.IsReady() && Utilities.Enabled("r.combo") && !Spells.Q.IsReady() &&
                Utilities.UltimateKey() == "Q" && enemy.IsValidTarget(Spells.R.Range))
            {
                Spells.R.CastOnUnit(enemy);
            }
            else if (!Spells.R.IsReady() && !Spells.Q.IsReady() && Spells.W.IsReady() && Utilities.Enabled("w.combo")
                && enemy.IsValidTarget(Spells.W.Range))
            {
                var hit = Spells.W.GetPrediction(enemy);
                if (hit.Hitchance >= HitChance.Medium)
                {
                    Spells.W.Cast(hit.CastPosition);
                }
                
            }
            else if (!Spells.R.IsReady() && !Spells.Q.IsReady() && !Spells.W.IsReady() && Spells.E.IsReady() && 
                Utilities.Enabled("e.combo") && enemy.IsValidTarget(Spells.E.Range))
            {
                var hit = Spells.E.GetPrediction(enemy);
                if (hit.Hitchance >= HitChance.Medium)
                {
                    Spells.E.Cast(hit.CastPosition);
                }
            }
        }

        public static void Init()
        {
            switch (Menus.Config.Item("combo.style").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    QRWESelected(TargetSelector.SelectedTarget);
                    break;
                case 1:
                    QRWECombo();
                    break;
            }
        }
    }
}
