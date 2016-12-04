using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.Stack
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Champions.Annie.Core;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class PassiveStack : OrbwalkingChild
    {
        public override string Name { get; set; } = "Stack";

        private PassiveCount count;

        private readonly ESpell eSpell;

        private readonly WSpell wSpell;

        public PassiveStack(ESpell eSpell, WSpell wSpell)
        {
            this.eSpell = eSpell;
            this.wSpell = wSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(wSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("Recall")
                || (ObjectManager.Player.InFountain() && !Menu.Item("Fountain").GetValue<bool>())
                || (ObjectManager.Player.InShop() && Menu.Item("Shop").GetValue<bool>())
                || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Tear").GetValue<bool>()
                && Utils.TickCount - wSpell.Spell.LastCastAttemptT > 4000
                && (Items.HasItem(3070) || Items.HasItem(3004))
                || (count.StunCount >= Menu.Item("Passive").GetValue<Slider>().Value))
            {
                if (eSpell.Spell.IsReady())
                {
                    eSpell.Spell.Cast();
                }
                else if(wSpell.Spell.IsReady())
                {
                    if (Target != null)
                    {
                        wSpell.Spell.Cast(Target);
                    }

                    var minions =
                        MinionManager.GetMinions(ObjectManager.Player.Position, eSpell.Spell.Range).FirstOrDefault();

                    if (minions != null)
                    {
                        wSpell.Spell.Cast(minions);
                    }

                    var mobs =
                        MinionManager.GetMinions(
                            ObjectManager.Player.Position,
                            wSpell.Spell.Range,
                            MinionTypes.All,
                            MinionTeam.Neutral).FirstOrDefault();

                    if (mobs != null)
                    {
                        wSpell.Spell.Cast(mobs);
                    }
                    else
                    {
                        eSpell.Spell.Cast(Game.CursorPos);
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

            Menu.AddItem(new MenuItem("Tear", "Stack Tear").SetValue(true));

            Menu.AddItem(new MenuItem("Fountain", "Stack In Fountain").SetValue(true));

            Menu.AddItem(new MenuItem("Passive", "Stack Passive To: ").SetValue(new Slider(4, 1, 4)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana").SetValue(new Slider(50, 0, 100)));
            
            count = new PassiveCount();
        }
    }
}
