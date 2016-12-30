using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.Core.Spells
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SPrediction;

    using Prediction = SPrediction.Prediction;

    using SharpDX;

    internal class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Hexplosive Minefield";

        public override Spell Spell { get; set; }

       // public GameObject GameObject;

        public List<ObjList> GameobjectLists = new List<ObjList>();

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public Prediction.Result SPredictionOutput(AIHeroClient target)
        {
            return Spell.GetSPrediction(target);
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            return Spell.GetPrediction(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.E, 800);
            Spell.SetSkillshot(.35f, 300f, 1600, false, SkillshotType.SkillshotCircle);

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null || sender.Type != GameObjectType.obj_GeneralParticleEmitter)
            {
                return;
            }

            var particle = sender as Obj_GeneralParticleEmitter;

            if (particle != null && sender.Name.Contains("Ziggs_Base_E_placedMine.troy"))
            {
                Console.WriteLine("add");
                GameobjectLists.Add(new ObjList(particle, particle.Position));
            }
        }

        private void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.Name.Contains("Ziggs"))
            {
                return;
            }

            var particle = sender as Obj_GeneralParticleEmitter;

            if (particle == null || !particle.Name.Contains("Ziggs_Base_E_placedMine.troy"))
            {
                return;
            }

            GameobjectLists.RemoveAll(x => x.ObjecEmitter.NetworkId == particle.NetworkId);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }

    public class ObjList
    {
        public Obj_GeneralParticleEmitter ObjecEmitter;

        public Vector3 Position;

        public ObjList(Obj_GeneralParticleEmitter obj, Vector3 position)
        {
            this.ObjecEmitter = obj;
            this.Position = position;
        }
    }
}
