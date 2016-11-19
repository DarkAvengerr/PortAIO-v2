using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.OrbwalkingMode.Stack
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ezreal.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class StackTear : OrbwalkingChild
    {
        public override string Name { get; set; } = "Stack";

       // private readonly PlayerInfo playerInfo = new PlayerInfo();

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        public StackTear(QSpell qSpell, WSpell wSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(wSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || ObjectManager.Player.HasBuff("Recall")
                || (ObjectManager.Player.InFountain() && Menu.Item("Fountain").GetValue<bool>())
                || (ObjectManager.Player.InShop() && Menu.Item("Shop").GetValue<bool>())
                || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Tear").GetValue<bool>() && Utils.TickCount - qSpell.Spell.LastCastAttemptT > 4000 && (Items.HasItem(3070) || Items.HasItem(3004)))
            {
                if (Target != null)
                {
                    var prediction = qSpell.Spell.GetPrediction(Target);

                    qSpell.Spell.Cast(prediction.CastPosition);
                }

                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, qSpell.Spell.Range).FirstOrDefault();

                if (minions != null)
                {
                    var prediction = qSpell.Spell.GetPrediction(minions);

                    qSpell.Spell.Cast(prediction.CastPosition);
                }

                var mobs = MinionManager.GetMinions(ObjectManager.Player.Position, qSpell.Spell.Range, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();

                if (mobs != null)
                {
                    var prediction = qSpell.Spell.GetPrediction(mobs);

                    qSpell.Spell.Cast(prediction.CastPosition);
                }
                else
                {
                    qSpell.Spell.Cast(Game.CursorPos);
                }
            }

            //if (Menu.Item("WAlly").GetValue<bool>() && ObjectManager.Player.HasBuff("ezrealrisingspellforce"))
            //{
            //    if (Game.Time - playerInfo.GetBuffEndTime(ObjectManager.Player, "ezrealrisingspellforce") > 300)
            //    {
            //        var allies = ObjectManager.Player.GetAlliesInRange(wSpell.Spell.Range)
            //                .Where(x => !x.IsMe)
            //                .FirstOrDefault(x => x.Distance(ObjectManager.Player.Position) <= wSpell.Spell.Range);

            //        var prediction = wSpell.Spell.GetPrediction(allies);

            //        if (allies != null && prediction.Hitchance >= HitChance.Medium && ObjectManager.Player.UnderTurret(true))
            //        {
            //            wSpell.Spell.Cast(prediction.CastPosition);
            //        }
            //    }
            //}
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

          //  Menu.AddItem(new MenuItem("WAlly", "Smart W Ally").SetValue(true));

            Menu.AddItem(new MenuItem("Tear", "Stack Tear").SetValue(true));

            Menu.AddItem(new MenuItem("Fountain", "Don't Stack In Fountain").SetValue(true));

            Menu.AddItem(new MenuItem("Shop", " Don't Stack In Shop").SetValue(true));

            Menu.AddItem(new MenuItem("Mana", "Min Mana").SetValue(new Slider(50, 0, 100)));
        }
    }
}
