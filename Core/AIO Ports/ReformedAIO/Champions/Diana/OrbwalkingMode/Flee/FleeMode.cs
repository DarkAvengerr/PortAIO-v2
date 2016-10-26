namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Flee
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

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
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    fleeLogic = new FleeLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(
                new MenuItem(Name + "FleeKey", "Flee Key").SetValue(new KeyBind('A', KeyBindType.Press)));

            Menu.AddItem(new MenuItem(Name + "FleeMinion", "Flee To Minions").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "FleeMob", "Flee To Mobs").SetValue(true));

            Menu.AddItem(
                new MenuItem(Name + "FleeVector", "Flee To Vector").SetValue(true)
                    .SetTooltip("Flee's To Jungle Camps"));

            fleeLogic = new FleeLogic();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Menu.Item(Menu.Name + "FleeKey").GetValue<KeyBind>().Active) return;

            var jump =
                fleeLogic.JumpPos.FirstOrDefault(
                    x =>
                    x.Value.Distance(ObjectManager.Player.Position) < 300f && x.Value.Distance(Game.CursorPos) < 700f);

            var monster =
                MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral)
                    .FirstOrDefault();

            var mobs = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly);

            if (jump.Value.IsValid() && Menu.Item(Menu.Name + "FleeVector").GetValue<bool>())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);

                foreach (var junglepos in
                    fleeLogic.JunglePos.Where(
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

            foreach (var junglepos in fleeLogic.JunglePos)
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