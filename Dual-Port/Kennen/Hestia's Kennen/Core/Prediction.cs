using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
namespace Kennen.Core
{
    internal static class Prediction
    {

        private static void SebbyCast(Spell spell, Obj_AI_Base target, HitChance hitChance)
        {
            var aoe = false;
            var SpellType = SebbyLib.Prediction.SkillshotType.SkillshotLine;

            if (spell.Type == SkillshotType.SkillshotCircle)
            {
                SpellType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe = true;
            }

            if (spell.Type == SkillshotType.SkillshotCone)
            {
                SpellType = SebbyLib.Prediction.SkillshotType.SkillshotCone;
                aoe = true;
            }

            if (spell.Width > 80 && !spell.Collision)
            {
                aoe = true;
            }

            var predInput = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = aoe,
                Collision = spell.Collision,
                Speed = spell.Speed,
                Delay = spell.Delay,
                Range = spell.Range,
                From = ObjectManager.Player.ServerPosition,
                Radius = spell.Width,
                Unit = target,
                Type = SpellType
            };

            var predOutput = SebbyLib.Prediction.Prediction.GetPrediction(predInput);
            
            switch (hitChance)
            {
                case HitChance.Low:
                    if (predOutput.Hitchance >= SebbyLib.Prediction.HitChance.Low)
                    {
                        spell.Cast(predOutput.CastPosition);
                    }
                    break;
                case HitChance.Medium:
                    if (predOutput.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                    {
                        spell.Cast(predOutput.CastPosition);
                    }
                    break;
                case HitChance.High:
                    if (predOutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        spell.Cast(predOutput.CastPosition);
                    }
                    break;
                case HitChance.VeryHigh:
                    if (predOutput.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                    {
                        spell.Cast(predOutput.CastPosition);
                    }
                    break;
                default:
                    if (predOutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        spell.Cast(predOutput.CastPosition);
                    }
                    break;
            }
        }
        
        private static void CommonCast(Spell spell, Obj_AI_Base target, HitChance hitChance)
        {
            var spellHitChance = spell.GetPrediction(target);
            switch (hitChance)
            {
                case HitChance.Low:
                    if (spellHitChance.Hitchance >= HitChance.Low)
                    {
                        spell.Cast(target);
                    }
                    break;
                case HitChance.Medium:
                    if (spellHitChance.Hitchance >= HitChance.Medium)
                    {
                        spell.Cast(target);
                    }
                    break;
                case HitChance.High:
                    if (spellHitChance.Hitchance >= HitChance.High)
                    {
                        spell.Cast(target);
                    }
                    break;
                case HitChance.VeryHigh:
                    if (spellHitChance.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Cast(target);
                    }
                    break;
                default:
                    if (spellHitChance.Hitchance >= HitChance.High)
                    {
                        spell.Cast(target);
                    }
                    break;
            }
        }

        public static void CastSpell(this Spell spell, Obj_AI_Base target, string predMenu, string hitMenu)
        {
            switch (Configs.config.Item(predMenu).GetValue<StringList>().SelectedIndex)
            {
                case 0: //Common
                    CommonCast(spell, target, GetHitchance(hitMenu));
                    break;
                case 1: //OKTW
                    SebbyCast(spell, target, GetHitchance(hitMenu));
                    break;
                default:
                    CommonCast(spell, target, GetHitchance(hitMenu));
                    break;
            }
        }

        private static HitChance GetHitchance(string menuName)
        {
            var hitChance = Configs.config.Item(menuName).GetValue<StringList>().SelectedIndex;

            switch (hitChance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.VeryHigh;
            }

        }
    }
}
