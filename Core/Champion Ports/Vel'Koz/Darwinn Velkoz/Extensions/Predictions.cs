using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SPrediction;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s_velkoz.Extensions
{
    internal static class Predictions
    {
        public static void CastWithSebbyPred(Spell spell, Obj_AI_Base unit, HitChance hit)
        {
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            bool aoe2 = false;

            if (spell.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if (spell.Width > 80 && !spell.Collision)
                aoe2 = true;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = aoe2,
                Collision = spell.Collision,
                Speed = spell.Speed,
                Delay = spell.Delay,
                Range = spell.Range,
                From = ObjectManager.Player.ServerPosition,
                Radius = spell.Width,
                Unit = unit,
                Type = CoreType2
            };
            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            //var poutput2 = QWER.GetPrediction(target);

            if (spell.Speed != float.MaxValue && OktwCommon.CollisionYasuo(ObjectManager.Player.ServerPosition,
                poutput2.CastPosition))
                return;

            switch (hit)
            {
                case HitChance.Low:
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Low)
                        spell.Cast(poutput2.CastPosition);
                    break;
                case HitChance.Medium:
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                        spell.Cast(poutput2.CastPosition);
                    break;
                case HitChance.High:
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        spell.Cast(poutput2.CastPosition);
                    break;
                case HitChance.VeryHigh:
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        spell.Cast(poutput2.CastPosition);
                    break;
                case HitChance.Immobile:
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Immobile)
                        spell.Cast(poutput2.CastPosition);
                    break;
            }
        }
        /// <summary>
        /// skill cast
        /// </summary>
        /// <param name="spell">spell</param>
        /// <param name="unit">enemy</param>
        /// <param name="hit">hitchance</param>
        /// <param name="menuitemname"> menu item name </param>
        /// <param name="menuname"> and menu name like a "Config" </param>
        public static void HikiCast(this Spell spell, Obj_AI_Base unit, HitChance hit, string menuitemname, Menu menuname)
        {
            switch (menuname.Item(menuitemname).GetValue<StringList>().SelectedIndex)
            {
                case 0: // Common Prediction
                    var hitchance = spell.GetPrediction(unit);
                    if (hitchance.Hitchance >= hit)
                    {
                        spell.Cast(hitchance.CastPosition);
                    }
                    break;
                case 1: // Sebby Prediction
                    CastWithSebbyPred(spell, unit, hit);
                    break;
                case 2: // Sprediction
                    spell.SPredictionCast((AIHeroClient)unit, hit);
                    break;
            }
        }

    }
}