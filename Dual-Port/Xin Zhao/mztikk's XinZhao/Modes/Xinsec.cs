using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Config = mztikkXinZhao.Config;

    internal static class Xinsec
    {
        #region Properties

        internal static bool RecalcFlashPosAfterE { get; } = true;

        #endregion

        #region Methods

        internal static void Execute()
        {
            AIHeroClient xinsecTarget = null;
            Obj_AI_Base eTarget = null;
            switch (Config.GetStringListValue("xinsecTargetting"))
            {
                case 0:
                    xinsecTarget = TargetSelector.SelectedTarget;
                    break;
                case 1:
                    xinsecTarget = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);
                    break;
                case 2:
                    xinsecTarget =
                        HeroManager.Enemies.Where(en => en.Distance(ObjectManager.Player.Position) <= 2000)
                            .OrderBy(en => en.MaxHealth)
                            .FirstOrDefault();
                    break;
            }

            if (xinsecTarget == null || !Spells.E.IsReady() || !Spells.R.IsReady()
                || xinsecTarget.HasBuff("XinZhaoIntimidate") || xinsecTarget.IsInvulnerable)
            {
                return;
            }

            var extendToPos = ObjectManager.Player.Position.GetBestAllyPlace(1750);
            if (extendToPos == Vector3.Zero)
            {
                return;
            }

            var xinsecTargetExtend = xinsecTarget.Position.Extend(extendToPos, -200);
            var eTargetMinion =
                MinionManager.GetMinions(ObjectManager.Player.Position, 2500)
                    .Where(
                        m =>
                        m.Distance(ObjectManager.Player.Position) <= Spells.E.Range
                        && m.Distance(xinsecTargetExtend) < Spells.FlashRange - 25)
                    .OrderBy(m => m.Distance(xinsecTargetExtend))
                    .FirstOrDefault();
            if (eTargetMinion != null)
            {
                eTarget = eTargetMinion;
            }
            else
            {
                var eTargetHero =
                    HeroManager.Enemies.Where(
                        m =>
                        m.Distance(ObjectManager.Player.Position) <= Spells.E.Range
                        && m.Distance(xinsecTargetExtend) < Spells.FlashRange - 25 && m != xinsecTarget
                        && m.NetworkId != xinsecTarget.NetworkId)
                        .OrderBy(m => m.Distance(xinsecTargetExtend))
                        .FirstOrDefault();
                if (eTargetHero != null)
                {
                    eTarget = eTargetHero;
                }
            }

            if (eTarget != null)
            {
                if (eTarget.Distance(xinsecTargetExtend) <= 40 && eTarget.Distance(xinsecTarget) > 7)
                {
                    Spells.E.Cast(eTarget);
                    LeagueSharp.Common.Utility.DelayAction.Add(300, () => Spells.R.Cast());
                }

                if (eTarget.Distance(xinsecTargetExtend) > 40 && Spells.Flash.IsReady()
                    && Config.IsChecked("xinsecFlash"))
                {
                    var castDelay = Mainframe.RDelay.Next(325, 375);
                    Spells.E.Cast(eTarget);
                    if (RecalcFlashPosAfterE)
                    {
                        extendToPos = ObjectManager.Player.Position.GetBestAllyPlace(1750);
                        xinsecTargetExtend = xinsecTarget.Position.Extend(extendToPos, -200);
                    }

                    LeagueSharp.Common.Utility.DelayAction.Add(
                        castDelay, 
                        () => ObjectManager.Player.Spellbook.CastSpell(Spells.Flash.Slot, xinsecTargetExtend));
                    LeagueSharp.Common.Utility.DelayAction.Add(castDelay + 40, () => Spells.R.Cast());
                }
            }
        }

        #endregion
    }
}