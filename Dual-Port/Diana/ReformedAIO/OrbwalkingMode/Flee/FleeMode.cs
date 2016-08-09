using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Flee
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class FleeMode : ChildBase
    {
        #region Fields

        private FleeLogic fleeLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "Flee";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.fleeLogic = new FleeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(
                new MenuItem(this.Name + "FleeKey", "Flee Key").SetValue(new KeyBind('A', KeyBindType.Press)));

            this.Menu.AddItem(new MenuItem(this.Name + "FleeMinion", "Flee To Minions").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "FleeMob", "Flee To Mobs").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "FleeVector", "Flee To Vector").SetValue(true)
                    .SetTooltip("Flee's To Jungle Camps"));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!this.Menu.Item(this.Menu.Name + "FleeKey").GetValue<KeyBind>().Active) return;

            var jump =
                this.fleeLogic.JumpPos.FirstOrDefault(
                    x =>
                    x.Value.Distance(ObjectManager.Player.Position) < 300f && x.Value.Distance(Game.CursorPos) < 700f);

            var monster =
                MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health)
                    .FirstOrDefault();

            var mobs = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly);

            if (jump.Value.IsValid() && this.Menu.Item(this.Menu.Name + "FleeVector").GetValue<bool>())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);

                foreach (var junglepos in
                    this.fleeLogic.JunglePos.Where(
                        junglepos =>
                        Game.CursorPos.Distance(junglepos) <= 350
                        && ObjectManager.Player.Position.Distance(junglepos) <= 825
                        && Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.R].IsReady()))
                {
                    Variables.Spells[SpellSlot.Q].Cast(junglepos);

                    if (monster != null)
                    {
                        Variables.Spells[SpellSlot.R].Cast(monster);
                    }
                }
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            foreach (var junglepos in this.fleeLogic.JunglePos)
            {
                if (Game.CursorPos.Distance(junglepos) <= 350
                    && ObjectManager.Player.Position.Distance(junglepos) <= 900
                    && Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.R].IsReady())
                {
                    Variables.Spells[SpellSlot.Q].Cast(junglepos);
                    Variables.Spells[SpellSlot.R].Cast(monster);
                }
                else if (Variables.Spells[SpellSlot.R].IsReady() && !Variables.Spells[SpellSlot.Q].IsReady()
                         && monster.Distance(ObjectManager.Player.Position) > 600f
                         && monster.Distance(Game.CursorPos) <= 350f)
                {
                    Variables.Spells[SpellSlot.R].Cast(monster);
                }
            }

            if (!mobs.Any()) return;

            var mob = mobs.MaxOrDefault(x => x.MaxHealth);

            if (!(mob.Distance(Game.CursorPos) <= 750) || !(mob.Distance(ObjectManager.Player) >= 475)) return;

            if (Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.R].IsReady()
                && ObjectManager.Player.Mana
                > Variables.Spells[SpellSlot.R].ManaCost + Variables.Spells[SpellSlot.Q].ManaCost
                && mob.Health > Variables.Spells[SpellSlot.Q].GetDamage(mob))
            {
                Variables.Spells[SpellSlot.Q].Cast(mob);
                Variables.Spells[SpellSlot.R].Cast(mob);
            }
            else
            {
                if (Variables.Spells[SpellSlot.R].IsReady())
                {
                    Variables.Spells[SpellSlot.R].Cast(mob);
                }
            }
        }

        #endregion
    }
}