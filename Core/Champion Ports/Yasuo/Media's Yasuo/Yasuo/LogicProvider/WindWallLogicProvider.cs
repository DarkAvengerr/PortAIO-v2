using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    using System.Collections.Generic;

    using CommonEx.Extensions;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK;

    using SharpDX;

    using Prediction = SebbyLib.Prediction.Prediction;
    using PredictionInput = SebbyLib.Prediction.PredictionInput;
    using PredictionOutput = SebbyLib.Prediction.PredictionOutput;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;
    using TargetSelector = LeagueSharp.Common.TargetSelector;

    // TODO: Maybe Block spells that aiming an enemy and are blockable i.e: Lux W
    // TODO: E when W not needed (ie. Ally wont get hit)
    // TODO: E behind W when skillshot is targeted (ie. cait ult) will hit you or next AA will kill you or do much dmg
    // TODO: Anti Gragas Insec (more of a fun thing actually.)
    // TODO: Crit in AA
    // TODO: 1v1 SafeZone logic
    // TODO: Clean up code
    // TODO: Annie Stun, Katarina Ult

    public class WindWallLogicProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindWallLogicProvider"/> class.
        /// </summary>
        public WindWallLogicProvider()
        {
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Determines the "usemode"
        /// </summary>
        public enum WindWallMode
        {
            Protecting,

            SelfProtecting
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the optimal cast position for multiple skillshots
        /// </summary>
        /// <param name="skillshots"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Vector3 GetCastPosition(Skillshot[] skillshots, WindWallMode mode)
        {
            var skillshotDict = new Dictionary<Skillshot, Vector3>();
            var result = Vector3.Zero;

            foreach (var skillshot in skillshots)
            {
                skillshotDict.Add(skillshot, this.GetCastPosition(skillshot.MissilePosition(), skillshot.Direction));
            }

            switch (mode)
            {
                case WindWallMode.Protecting:
                    // TODO
                    break;
                case WindWallMode.SelfProtecting:
                    // TODO
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Returns the optimal cast position for one missile
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCastPosition(Vector2 missilePosition, Vector2 direction)
        {
            return Vector3.Zero;
        }

        /// <summary>
        ///     Returns a precautionary position that is supposed to soak the most dmg before it happens
        /// </summary>
        /// <param name="units"></param>
        /// <param name="prediction"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public PredictionOutput GetPrecautionaryPosition(
            List<AIHeroClient> units,
            bool prediction = true,
            float range = 1000)
        {
            var predInput = new PredictionInput
                                {
                                    From = GlobalVariables.Player.ServerPosition,
                                    Aoe = true,
                                    Collision = GlobalVariables.Spells[SpellSlot.W].Collision,
                                    Speed = 4000,
                                    Delay = float.MaxValue,
                                    Radius = range,
                                    Unit =
                                        EnumerableExtensions.MaxOrDefault(
                                            units,
                                            TargetSelector.GetPriority),
                                    Type = SkillshotType.SkillshotCone
                                };

            return Prediction.GetPrediction(predInput);
        }

        #endregion
    }
}