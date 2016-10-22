using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using _Project_Geass.Functions.Objects;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions
{

    internal class Prediction
    {
        #region Public Fields

        public static int PredictionMethod => StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}.PredictionMethod").GetValue<StringList>().SelectedIndex;

        #endregion Public Fields

        #region Private Methods

        private static bool ValidChampion(AIHeroClient target) {return !target.HasBuffOfType(BuffType.Invulnerability)&&!target.HasBuffOfType(BuffType.SpellImmunity)&&!target.HasBuffOfType(BuffType.SpellShield);}

        #endregion Private Methods

        #region Public Methods

        public static bool CheckColision(PredictionOutput prediction) //Returns if a colision is meet
        {
            var colision=prediction.CollisionObjects.Any(obj => !obj.IsChampion()&&obj.IsEnemy);
            //if(colision) StaticObjects.ProjectLogger.WriteLog($"Colision");
            return colision;
        }

        public static bool CheckColision(SPrediction.Prediction.Result prediction) //Returns if a colision is meet
        {
            var commonOutput=new PredictionOutput {Hitchance=prediction.HitChance, CollisionObjects=prediction.CollisionResult.Units, CastPosition=(Vector3)prediction.CastPosition, UnitPosition=(Vector3)prediction.UnitPosition};
            StaticObjects.ProjectLogger.WriteLog("SPrediction=>Common Colision Check");
            return CheckColision(commonOutput);
        }

        public static bool DoCast(Spell spell, AIHeroClient target, HitChance minHitChance, bool colisionCheck=false)
        {
            //  StaticObjects.ProjectLogger.WriteLog("DoCast Call");
            if ((PredictionMethod==0)||((PredictionMethod==1)&&colisionCheck)) //Sebby Colision is broken...lol
            {
                var output=spell.GetPrediction(target);
                if (PredictionMethod==1)
                    StaticObjects.ProjectLogger.WriteLog("SebbyPrediction=>Common (Sebby Colision is broken)");

                if (colisionCheck)
                    if (CheckColision(output))
                        return false;

                if (minHitChance>output.Hitchance)
                    return false;

                spell.Cast(output.CastPosition);
                return true;
            }

            if (PredictionMethod==1)
            {
                var output=SebbyLib.Prediction.Prediction.GetPrediction(target, spell.Delay);

                if (minHitChance>(HitChance)output.Hitchance)
                    return false;

                spell.Cast(output.CastPosition);
                return true;
            }

            if (PredictionMethod==2)
            {
                var output=spell.GetSPrediction(target);

                if (colisionCheck)
                    if (CheckColision(output))
                        return false;

                if (minHitChance>output.HitChance)
                    return false;

                spell.Cast(output.CastPosition);
                return true;
            }

            return false;
        }

        public static HitChance GetHitChance(string value) => (HitChance)Enum.Parse(typeof(HitChance), value);

        public static string[] GetHitChanceNames()
        {
            var names=Enum.GetNames(typeof(HitChance));
            return new[] {names[(int)HitChance.High], names[(int)HitChance.VeryHigh], names[(int)HitChance.Medium], names[(int)HitChance.Low], names[(int)HitChance.Immobile], names[(int)HitChance.Dashing]};
        }

        public static IOrderedEnumerable<AIHeroClient> OrderTargets(Spell spell)
        {
            var damageType=spell.DamageType==TargetSelector.DamageType.Physical;
            return damageType? Heroes.GetEnemies(spell.Range).Where(ValidChampion).OrderBy(hp => hp.Health/hp.PercentArmorMod) : Heroes.GetEnemies(spell.Range).Where(ValidChampion).OrderBy(hp => hp.Health/hp.PercentMagicReduction);
        }

        #endregion Public Methods
    }

}