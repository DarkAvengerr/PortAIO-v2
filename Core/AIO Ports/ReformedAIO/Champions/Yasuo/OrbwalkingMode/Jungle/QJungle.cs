using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly Q1Spell qSpell;

        private readonly Q3Spell q3Spell;

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
                    if (ObjectManager.Player.IsDashing())
                    {
                        return;
                    }

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