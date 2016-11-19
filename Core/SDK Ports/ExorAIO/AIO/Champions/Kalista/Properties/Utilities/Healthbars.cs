
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Kalista
{
    using System.Drawing;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Healthbars
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
                {
                    if (!Vars.E.IsReady() || !Vars.Menu["drawings"]["edmg"].GetValue<MenuBool>().Value)
                    {
                        return;
                    }

                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            h =>
                            h.IsValidTarget() && Bools.IsPerfectRendTarget(h)
                            && (h is AIHeroClient || Vars.JungleList.Contains(h.CharData.BaseSkinName)))
                        .ToList()
                        .ForEach(
                            unit =>
                                {
                                    /// <summary>
                                    ///     Defines what HPBar Offsets it should display.
                                    /// </summary>
                                    var mobOffset =
                                        Vars.JungleHpBarOffsetList.FirstOrDefault(
                                            x => x.BaseSkinName.Equals(unit.CharData.BaseSkinName));

                                    var width = Vars.JungleList.Contains(unit.CharData.BaseSkinName)
                                                    ? mobOffset?.Width ?? Vars.SWidth
                                                    : Vars.SWidth;
                                    var height = Vars.JungleList.Contains(unit.CharData.BaseSkinName)
                                                     ? mobOffset?.Height ?? Vars.SHeight
                                                     : Vars.SHeight;
                                    var xOffset = Vars.JungleList.Contains(unit.CharData.BaseSkinName)
                                                      ? mobOffset?.XOffset ?? Vars.SxOffset((AIHeroClient)unit)
                                                      : Vars.SxOffset((AIHeroClient)unit);
                                    var yOffset = Vars.JungleList.Contains(unit.CharData.BaseSkinName)
                                                      ? mobOffset?.YOffset ?? Vars.SyOffset((AIHeroClient)unit)
                                                      : Vars.SyOffset((AIHeroClient)unit);
                                    var barPos = unit.HPBarPosition;
                                    {
                                        barPos.X += xOffset;
                                        barPos.Y += yOffset;
                                    }
                                    var drawEndXPos = barPos.X + width * (unit.HealthPercent / 100);
                                    var drawStartXPos = barPos.X
                                                        + (Vars.GetRealHealth(unit)
                                                           > (float)GameObjects.Player.GetSpellDamage(unit, SpellSlot.E)
                                                           + (float)
                                                             GameObjects.Player.GetSpellDamage(
                                                                 unit,
                                                                 SpellSlot.E,
                                                                 DamageStage.Buff)
                                                               ? width
                                                                 * ((Vars.GetRealHealth(unit)
                                                                     - ((float)
                                                                        GameObjects.Player.GetSpellDamage(
                                                                            unit,
                                                                            SpellSlot.E)
                                                                        + (float)
                                                                          GameObjects.Player.GetSpellDamage(
                                                                              unit,
                                                                              SpellSlot.E,
                                                                              DamageStage.Buff))) / unit.MaxHealth * 100
                                                                    / 100)
                                                               : 0);
                                    Drawing.DrawLine(
                                        drawStartXPos,
                                        barPos.Y,
                                        drawEndXPos,
                                        barPos.Y,
                                        height,
                                        Vars.GetRealHealth(unit)
                                        < (float)GameObjects.Player.GetSpellDamage(unit, SpellSlot.E)
                                        + (float)GameObjects.Player.GetSpellDamage(unit, SpellSlot.E, DamageStage.Buff)
                                            ? Color.Blue
                                            : Color.Orange);
                                    Drawing.DrawLine(
                                        drawStartXPos,
                                        barPos.Y,
                                        drawStartXPos,
                                        barPos.Y + height + 1,
                                        1,
                                        Color.Lime);
                                });
                };
        }

        #endregion
    }
}