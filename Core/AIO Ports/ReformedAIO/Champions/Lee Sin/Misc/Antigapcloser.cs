using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Misc
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class LeeSinAntiGapcloser : OrbwalkingChild
    {
        public override string Name { get; set; } = "Anti-Gapcloser";

        private readonly RSpell rSpell;

        private readonly WSpell wSpell;

        public LeeSinAntiGapcloser(RSpell rSpell, WSpell wSpell)
        {
            this.rSpell = rSpell;
            this.wSpell = wSpell;
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!CheckGuardians() || gapcloser.Sender == null)
            {
                return;
            }

            if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 375
                && rSpell.Spell.IsReady() 
                && Menu.Item("LeeSin.Antigapcloser.R").GetValue<bool>())
            {
                rSpell.Spell.CastOnUnit(gapcloser.Sender); // TODO: Add flash insec!
            }
            else if (wSpell.Spell.IsReady() && Menu.Item("LeeSin.Antigapcloser.W").GetValue<bool>() && wSpell.W1)
            {
                 wSpell.Jump(Game.CursorPos, true, true, true);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Antigapcloser.R", "Use R").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Antigapcloser.W", "Use W").SetValue(true).SetTooltip("Wardjump if R not possible"));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
        }
    }
}
