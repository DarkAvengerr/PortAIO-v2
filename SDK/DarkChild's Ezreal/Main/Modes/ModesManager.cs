using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;

    internal class ModesManager
    {
        public static void GameOnOnUpdate(EventArgs args)
        {
            if (Config.Player.IsDead)
            {
                return;
            }

            var target = SpellsManager.R.GetTarget();
            if (target != null && Config.RMenu.GetKeyBind("Rkey") && SpellsManager.R.IsReady() && target.IsValidTarget(SpellsManager.R.Range))
            {
                SpellsManager.R.Cast(SpellsManager.R.GetPrediction(target).CastPosition);
            }

            if (Config.MiscMenu.GetKeyBind("EW") && SpellsManager.E.IsReady() && SpellsManager.W.IsReady() && SpellsManager.E.Instance.SData.Mana + SpellsManager.W.Instance.SData.Mana < Config.Player.Mana)
            {
                SpellsManager.W.Cast(Game.CursorPos);
            }

            Active.Extcute();

            if (Misc.ComboMode)
            {
                Combo.Execute();
            }

            if (Misc.HybridMode)
            {
                Hybrid.Execute();
            }

            if (Misc.LastHitMode)
            {
                LastHit.Execute();
            }

            if (Misc.LaneClearMode)
            {
                LaneClear.Execute();
                JungleClear.Execute();
            }
        }
    }
}
