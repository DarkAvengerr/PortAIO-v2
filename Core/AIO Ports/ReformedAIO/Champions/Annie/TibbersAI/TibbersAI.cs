using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.TibbersAI
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class TibbersAI : OrbwalkingChild
    {
        public override string Name { get; set; } = "TibbersAI";

        private readonly Orbwalking.Orbwalker orbwalker;

        public TibbersAI(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private Obj_AI_Base tibbers;

        private AIHeroClient Target => TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

        private Obj_AI_Base Minion => MinionManager.GetMinions(tibbers.Position, 700).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || tibbers == null)
            {
                return;
            }

            var gameObjectStructure = orbwalker.GetTarget() as Obj_AI_Base;

            if (Target != null && !tibbers.UnderTurret(true))
            {
                EloBuddy.Player.IssueOrder(tibbers.Distance(Target) > tibbers.AttackRange
                ? GameObjectOrder.MovePet
                : GameObjectOrder.AttackUnit,
                Target);
            }
            else if(gameObjectStructure != null)
            {
                EloBuddy.Player.IssueOrder(tibbers.Distance(gameObjectStructure) > tibbers.AttackRange
                ? GameObjectOrder.MovePet
                : GameObjectOrder.AttackUnit,
                gameObjectStructure);
            }
            else if(Minion != null)
            {
                EloBuddy.Player.IssueOrder(tibbers.Distance(Minion) > tibbers.AttackRange
              ? GameObjectOrder.MovePet
              : GameObjectOrder.AttackUnit,
                Minion);
            }
        }

        private void GameObjectOnOnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj is Obj_AI_Minion && obj.Name.ToLower() == "tibbers")
            {
                tibbers = (Obj_AI_Base)obj;
            }
        }
        private void GameObjectOnOnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsAlly && obj is Obj_AI_Minion && obj.Name.ToLower() == "tibbers")
            {
                tibbers = null;
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            GameObject.OnDelete -= GameObjectOnOnDelete;
            GameObject.OnCreate -= GameObjectOnOnCreate;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            GameObject.OnDelete += GameObjectOnOnDelete;
            GameObject.OnCreate += GameObjectOnOnCreate;
            Game.OnUpdate += OnUpdate;
        }
    }
}
