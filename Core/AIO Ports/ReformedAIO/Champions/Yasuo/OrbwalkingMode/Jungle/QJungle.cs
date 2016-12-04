using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly Q1Spell qSpell;

        private readonly Q3Spell q3Spell;

        private DashPosition dashPos;

        public QJungle(Q1Spell qSpell, Q3Spell q3Spell)
        {
            this.qSpell = qSpell;
            this.q3Spell = q3Spell;
        }

        private float Range => ObjectManager.Player.HasBuff("YasuoQ3W") ? q3Spell.Spell.Range : qSpell.Spell.Range;

        private IOrderedEnumerable<Obj_AI_Base> Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 Range,
                 MinionTypes.All,
                 MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Mob)
            {
                if (ObjectManager.Player.IsDashing() && !qSpell.EqRange(m.Position))
                {
                    return;
                }

                if (q3Spell.Active)
                {
                    var pred = q3Spell.Spell.GetPrediction(m, true);

                    switch (Menu.Item("JHitchance").GetValue<StringList>().SelectedIndex)
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
                    qSpell.Spell.Cast(m);
                }
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

            Menu.AddItem(new MenuItem("JHitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}