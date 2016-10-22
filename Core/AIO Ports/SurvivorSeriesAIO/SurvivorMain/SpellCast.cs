using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SPrediction;
using SurvivorSeriesAIO.Core;
using HitChance = SebbyLib.Prediction.HitChance;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using SkillshotType = SebbyLib.Prediction.SkillshotType;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.SurvivorMain
{
    internal class SpellCast
    {
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static IRootMenu RootConfig { get; set; }

        public static MenuItem SelectedHitChance { get; set; }
        public static MenuItem SelectedPrediction { get; set; }

        /// <summary>
        ///     General Usage of Spells
        /// </summary>
        /// <param name="QWER"></param>
        /// <param name="target"></param>

        #region SebbySpell
        public static void SebbySpellMain(Spell QWER, Obj_AI_Base target)
        {
            if (RootConfig.SelectedPrediction.GetValue<StringList>().SelectedIndex == 1)
            {
                var CoreType2 = SkillshotType.SkillshotLine;
                var aoe2 = false;

                if (QWER.Type == LeagueSharp.Common.SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if ((QWER.Width > 80) && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = Prediction.GetPrediction(predInput2);

                if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((QWER.Speed != float.MaxValue) &&
                    OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;
                // LastPoint - Save | Make sure to replace the HitChances with VeryHigh/Medium - Invert them
                if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 0)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && (poutput2.AoeTargetsHitCount > 1) && (poutput2.Hitchance >= HitChance.Medium))
                        QWER.Cast(poutput2.CastPosition);
                }
                else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 1)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        QWER.Cast(poutput2.CastPosition);
                }
                else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 2)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                }
            }
            else if (RootConfig.SelectedPrediction.GetValue<StringList>().SelectedIndex == 0)
            {
                if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 0)
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.Medium);
                else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 1)
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 2)
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.VeryHigh);
            }
            else if (RootConfig.SelectedPrediction.GetValue<StringList>().SelectedIndex == 2)
            {
                if (target is AIHeroClient && target.IsValid)
                {
                    var t = target as AIHeroClient;
                    if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 0)
                        QWER.SPredictionCast(t, LeagueSharp.Common.HitChance.Medium);
                    else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 1)
                        QWER.SPredictionCast(t, LeagueSharp.Common.HitChance.High);
                    else if (RootConfig.SelectedHitChance.GetValue<StringList>().SelectedIndex == 2)
                        QWER.SPredictionCast(t, LeagueSharp.Common.HitChance.VeryHigh);
                }
                else
                {
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                }
            }
        }

        #endregion
    }
}