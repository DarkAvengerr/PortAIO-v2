using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Core
{
    using System;
    using System.Linq;

    using Dark_Star_Thresh.Update;

    using LeagueSharp;
    using LeagueSharp.Common;

    using CollisionableObjects = SebbyLib.Prediction.CollisionableObjects;
    using HitChance = SebbyLib.Prediction.HitChance;
    using PredictionInput = SebbyLib.Prediction.PredictionInput;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    internal class Core
    {
        public static Orbwalking.Orbwalker Orbwalker;

        public static AIHeroClient Player => ObjectManager.Player;

        public static int GetStunDuration(Obj_AI_Base target)
        {
            return (int)(target.Buffs.Where(b => b.IsActive && Game.Time < b.EndTime
            && (b.Type == BuffType.Charm 
            || b.Type == BuffType.Knockback 
            || b.Type == BuffType.Stun 
            || b.Type == BuffType.Suppression 
            || b.Type == BuffType.Snare)).Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time) * 1000;
        }

        public static void CastQ(AIHeroClient target)
        {
            if (target == null || MenuConfig.Config.Item("blacklist" + target.CharData.BaseSkinName).GetValue<bool>() || (!MenuConfig.Q2 && Mode.ThreshQ(target)))
            {
                return;
            }

            switch (MenuConfig.PredMode.SelectedIndex)
            {
                case 0:

                    var predictionInput = new PredictionInput
                                   {
                        Aoe = false,
                        Collision = true,
                        Speed = MenuConfig.Speed,
                        Delay = .5f,
                        Range = MenuConfig.Range,
                        From = Player.ServerPosition,
                        Radius = MenuConfig.Width,
                        Unit = target,
                        Type = SkillshotType.SkillshotLine,
                        CollisionObjects = new [] {CollisionableObjects.YasuoWall, CollisionableObjects.Minions }
                                   };

                    var prediction = SebbyLib.Prediction.Prediction.GetPrediction(predictionInput);

                    if (MenuConfig.Hitchance.SelectedIndex == 0 && prediction.Hitchance < HitChance.High)
                    {
                        return;
                    }

                    if (MenuConfig.Hitchance.SelectedIndex == 1 && prediction.Hitchance < HitChance.VeryHigh)
                    {
                        return;
                    }

                    Spells.Q.Cast(prediction.CastPosition);

                    break;

                case 1:

                    var commonPred = Spells.Q.GetPrediction(target);

                    if (MenuConfig.Hitchance.SelectedIndex == 0 && commonPred.Hitchance < LeagueSharp.Common.HitChance.High)
                    {
                        return;
                    }

                    if (MenuConfig.Hitchance.SelectedIndex == 1 && commonPred.Hitchance < LeagueSharp.Common.HitChance.VeryHigh)
                    {
                        return;
                    }

                    Spells.Q.Cast(commonPred.CastPosition);
                    break;
            }
        }
    }
}
