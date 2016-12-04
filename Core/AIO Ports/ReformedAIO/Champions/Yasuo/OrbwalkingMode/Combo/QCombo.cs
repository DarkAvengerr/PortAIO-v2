using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.Get_Information.HeroInfo;
    using ReformedAIO.Library.Spell_Information;

    using Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly Q1Spell qSpell;

        private readonly Q3Spell q3Spell;

        private DashPosition dashPos;

        public QCombo(Q1Spell qSpell, Q3Spell q3Spell)
        {
            this.qSpell = qSpell;
            this.q3Spell = q3Spell;
        }

        private float Range => ObjectManager.Player.HasBuff("YasuoQ3W") ? q3Spell.Spell.Range : qSpell.Spell.Range;

        private AIHeroClient Target => TargetSelector.GetTarget(Range, TargetSelector.DamageType.Physical);

       // private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, Range).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians() || (ObjectManager.Player.IsDashing() && !qSpell.EqRange(Target.Position)))
            {
                return;
            }

            if (q3Spell.Active)
            {
                var pred = q3Spell.Spell.GetPrediction(Target);

                switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            q3Spell.Spell.Cast(pred.CastPosition);
                        }
                        break;
                    case 1:
                        if (pred.Hitchance >= HitChance.High)
                        {
                            q3Spell.Spell.Cast(pred.CastPosition);
                        }
                        break;
                    case 2:
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            q3Spell.Spell.Cast(pred.CastPosition);
                        }
                        break;
                }
            }
            else
            {
                LeagueSharp.Common.Utility.DelayAction.Add(2, ()=> qSpell.Spell.CastOnUnit(Target));
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            dashPos = new DashPosition();

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}