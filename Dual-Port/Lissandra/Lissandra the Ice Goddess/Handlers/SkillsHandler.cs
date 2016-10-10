using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess.Handlers
{
    public class SkillsHandler
    {
        public static SpellSlot IgniteSlot
        {
            get
            {
                return ObjectManager.Player.GetSpellSlot("summonerdot");
            }
        }

        public static Spell QShard { get; set; }

        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 700) },
            { SpellSlot.W, new Spell(SpellSlot.W, 440) },
            { SpellSlot.E, new Spell(SpellSlot.E, 1050) },
            { SpellSlot.R, new Spell(SpellSlot.R, 550) }
        };

        public static void Load()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.25f, 75, 2250, true, SkillshotType.SkillshotLine);
            Spells[SpellSlot.W].Delay = 0.25f;
            Spells[SpellSlot.E].SetSkillshot(0.25f, 110, 850, false, SkillshotType.SkillshotLine);
            Spells[SpellSlot.R].SetSkillshot(0.25f, 690, 800, false, SkillshotType.SkillshotCircle);


            //Q Shard
            QShard = new Spell(SpellSlot.Q, 850);
            QShard.SetSkillshot(0.25f, 90, 2250, false, SkillshotType.SkillshotLine);
        }

        public static Vector3? GetQPrediction(AIHeroClient unit)
        {
            var normalPrediction = Spells[SpellSlot.Q].GetPrediction(unit);
            if (normalPrediction.Hitchance == HitChance.Collision || normalPrediction.Hitchance == HitChance.OutOfRange)
            {
                //There's collision or is out of range. We use Q shard prediction
                //Get Shard prediction
                var shardPrediction = QShard.GetPrediction(unit);
                if (shardPrediction.Hitchance >= HitChance.High) //TODO Might need to change to >= High in case it doesn't cast too often
                {
                    //Make a list of positions including the Cast Position for the Shard Prediction
                    var positionsList = new List<Vector2>();
                    {
                        positionsList.Add(shardPrediction.CastPosition.To2D());
                    }
                    //Check the collision Objects between my position and the cast position
                    var collisionObjects = QShard.GetCollision(ObjectManager.Player.ServerPosition.To2D(), positionsList);
                    //If there's a minion/Any collisionable object in between
                    if (collisionObjects.Any())
                    {
                        return shardPrediction.CastPosition;
                    }
                }
            }
            if (normalPrediction.Hitchance >= HitChance.High && unit.IsValidTarget(Spells[SpellSlot.Q].Range))
            {
                //The Hitchance is not Collision or Out of Range, Normal prediction.
                return normalPrediction.CastPosition;
            }
            return null;
        }

        public static int GetQCollisionObjects(Obj_AI_Base fromUnit)
        {
           var positionsList = new List<Vector2>();
           {
                 positionsList.Add(fromUnit.ServerPosition.To2D());
           }
           var collisionObjects = QShard.GetCollision(ObjectManager.Player.ServerPosition.To2D(), positionsList);
           return collisionObjects.Count();
        }
    }
}
