using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly Q1Spell qSpell;

        private readonly Q3Spell q3Spell;

        public QLane(Q1Spell qSpell, Q3Spell q3Spell)
        {
            this.qSpell = qSpell;
            this.q3Spell = q3Spell;
        }

        private float Range => ObjectManager.Player.HasBuff("YasuoQ3W") ? q3Spell.Spell.Range : qSpell.Spell.Range;

        private List<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, Range);

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Minion)
            {
                if (ObjectManager.Player.IsDashing() && ObjectManager.Player.Distance(m) > 475)
                {
                    return;
                }

                if (q3Spell.Active)
                {
                    switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            q3Spell.Spell.CastIfHitchanceEquals(m, HitChance.Medium);
                            break;
                        case 1:
                            q3Spell.Spell.CastIfHitchanceEquals(m, HitChance.High);
                            break;
                        case 2:
                            q3Spell.Spell.CastIfHitchanceEquals(m, HitChance.VeryHigh);
                            break;
                    }
                }
                else
                {
                    switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            qSpell.Spell.CastIfHitchanceEquals(m, HitChance.Medium);
                            break;
                        case 1:
                            qSpell.Spell.CastIfHitchanceEquals(m, HitChance.High);
                            break;
                        case 2:
                            qSpell.Spell.CastIfHitchanceEquals(m, HitChance.VeryHigh);
                            break;
                    }
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

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}