using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static Program;
    using static FioraPassive;
    using EloBuddy;

    public static class GetTargets
    {
        #region GetTarget
        public static bool FocusUlted { get { return Menu.Item("Focus Ulted Target").GetValue<bool>(); } }
        public static TargetMode TargetingMode
        {
            get
            {
                var menuindex = Menu.Item("Targeting Mode").GetValue<StringList>().SelectedIndex;
                switch (menuindex)
                {
                    case 0:
                        return TargetMode.Optional;
                    case 1:
                        return TargetMode.Selected;
                    case 2:
                        return TargetMode.Priority;
                    default:
                        return TargetMode.Normal;
                }

            }
        }
        public enum TargetMode
        {
            Normal = 0,
            Optional = 1,
            Selected = 2,
            Priority = 3
        }
        public static AIHeroClient GetTarget(float range = 500)
        {
            if (TargetingMode == TargetMode.Normal)
                return GetStandarTarget(range);
            if (TargetingMode == TargetMode.Optional)
                return GetOptionalTarget();
            if (TargetingMode == TargetMode.Priority)
                return GetPriorityTarget();
            if (TargetingMode == TargetMode.Selected)
                return GetSelectedTarget();
            return null;
        }
        public static AIHeroClient GetUltedTarget()
        {
            if (!FocusUlted)
                return null;
            return HeroManager.Enemies.FirstOrDefault(x => x != null && x.IsValid && FioraUltiPassiveObjects
                                .Any(i => i != null && i.IsValid && i.Position.To2D().Distance(x.Position.To2D()) <= 50));
        }
        #region Normal
        public static AIHeroClient GetStandarTarget(float range)
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(500))
                return ulted;
            return TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
        }
        #endregion Normal

        #region Priority
        public static float PriorityRange { get { return Menu.Item("Priority Range").GetValue<Slider>().Value; } }
        public static int PriorityValue(AIHeroClient target)
        {
            return Menu.Item("Priority" + target.ChampionName).GetValue<Slider>().Value;
        }
        public static AIHeroClient GetPriorityTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(PriorityRange))
                return ulted;
            return HeroManager.Enemies.Where(x => x.IsValidTarget(PriorityRange) && !x.IsZombie && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead)
                                    .OrderByDescending(x => PriorityValue(x))
                                    .ThenBy(x => x.Health)
                                    .FirstOrDefault();
        }
        #endregion Priority

        #region Selected
        public static float SelectedRange { get { return Menu.Item("Selected Range").GetValue<Slider>().Value; } }
        public static bool SwitchIfNoTargeted { get { return Menu.Item("Selected Switch If No Selected").GetValue<bool>(); } }
        public static AIHeroClient GetSelectedTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(SelectedRange))
                return ulted;
            var tar = TargetSelector.GetSelectedTarget();
            var tarD = tar.IsValidTarget(SelectedRange) && !tar.IsZombie ? tar : null;
            if (tarD != null && tarD.IsHPBarRendered && tarD.IsHPBarRendered && tarD.IsTargetable && !tarD.IsDead)
                return tarD;
            else
            {
                if (SwitchIfNoTargeted)
                    return GetOptionalTarget();
                return null;
            }
        }
        #endregion Selected

        #region Optional
        public static AIHeroClient SuperOldOptionalTarget = null;
        public static AIHeroClient OldOptionalTarget = null;
        public static AIHeroClient PreOptionalTarget = null;
        public static AIHeroClient OptionalTarget = null;
        public static float OptionalRange { get { return Menu.Item("Optional Range").GetValue<Slider>().Value; } }
        public static uint OptionalKey { get { return Menu.Item("Optional Switch Target Key").GetValue<KeyBind>().Key; } }
        public static AIHeroClient GetOptionalTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(OptionalRange))
            {
                OptionalTarget = ulted;
                return OptionalTarget;
            }
            if (OptionalTarget.IsValidTarget(OptionalRange) && !OptionalTarget.IsZombie)
                return OptionalTarget;
            OptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsZombie && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead)
                                .OrderBy(x => ObjectManager.Player.Distance(x.Position)).FirstOrDefault();
            return OptionalTarget;
        }
        public static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN)
            {
                if (args.WParam == (uint)OptionalKey)
                {
                    OptionalTarget = GetOptionalTarget();
                    if (OptionalTarget == null)
                    {
                        PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsZombie && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead)
                                                       .OrderBy(x => OldOptionalTarget != null ? x.NetworkId == OldOptionalTarget.NetworkId : x.IsEnemy)
                                                       .ThenBy(x => ObjectManager.Player.Distance(x.Position)).FirstOrDefault();
                        if (PreOptionalTarget != null)
                        {
                            OptionalTarget = PreOptionalTarget;
                        }
                        return;
                    }
                    PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsZombie && x.NetworkId != OptionalTarget.NetworkId && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead)
                                                   .OrderBy(x => OldOptionalTarget != null ? x.NetworkId == OldOptionalTarget.NetworkId : x.IsEnemy)
                                                   .ThenBy(x => ObjectManager.Player.Distance(x.Position)).FirstOrDefault();
                    if (PreOptionalTarget != null)
                    {
                        OldOptionalTarget = OptionalTarget;
                        OptionalTarget = PreOptionalTarget;
                    }
                    return;
                }
            }
            if (args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                OptionalTarget = GetOptionalTarget();
                if (OptionalTarget == null)
                {
                    PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead
                                                    && x.IsValidTarget(400, true, Game.CursorPos) && !x.IsZombie)
                                                   .OrderBy(x => Game.CursorPos.To2D().Distance(x.Position.To2D())).FirstOrDefault();
                    if (PreOptionalTarget != null)
                    {
                        OptionalTarget = PreOptionalTarget;
                    }
                    return;
                }
                PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && x.IsHPBarRendered && x.IsHPBarRendered && x.IsTargetable && !x.IsDead
                                                && x.IsValidTarget(400, true, Game.CursorPos) && !x.IsZombie)
                                               .OrderBy(x => Game.CursorPos.To2D().Distance(x.Position.To2D())).FirstOrDefault();
                if (PreOptionalTarget != null)
                {
                    OldOptionalTarget = OptionalTarget;
                    OptionalTarget = PreOptionalTarget;
                }
                return;
            }
        }
        #endregion Optional

        #endregion GetTarget
    }
}
