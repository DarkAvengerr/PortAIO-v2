using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.PermaActive
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;
    using ReformedAIO.Champions.Gnar.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class Flee : ChildBase
    {
        private FleeLogic fleeLogic;

        private GnarState gnarState;

        public override string Name { get; set; }

        public Flee(string name)
        {
            Name = name;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Menu.Item("FleeKey").GetValue<KeyBind>().Active) return;
           
            var jumpPos = fleeLogic.JumpPos.FirstOrDefault(x => x.Value.Distance(ObjectManager.Player.Position) < 425f && x.Value.Distance(Game.CursorPos) < 700f);

            var m = MinionManager.GetMinions(425f, MinionTypes.All, MinionTeam.All).FirstOrDefault();

            if (jumpPos.Value.IsValid() && Menu.Item("FleeVector").GetValue<bool>())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jumpPos.Value);

                if (gnarState.Mini)
                {
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(jumpPos.Value);
                    }
                }

                if (gnarState.Mega)
                {
                    if (Spells.E2.IsReady())
                    {
                        Spells.E2.Cast(jumpPos.Value);
                    }
                }
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (gnarState.Mega  
                || !Spells.E.IsReady()
                || !Menu.Item("FleeMinion").GetValue<bool>())
            {
                return;
            }

            if (m.Distance(Game.CursorPos) <= 425f)
            {
                Spells.E.Cast(m);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem("FleeKey", "Flee Key").SetValue(new KeyBind('A', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("FleeMinion", "Jump On Minions").SetValue(true));

            Menu.AddItem(new MenuItem("FleeVector", "Jump To Jungle Camps").SetValue(true));

            gnarState = new GnarState();
           
            fleeLogic = new FleeLogic();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }
    }
}
