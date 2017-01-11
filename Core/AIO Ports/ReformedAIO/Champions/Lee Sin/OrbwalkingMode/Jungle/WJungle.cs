using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Jungle
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WJungle(WSpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Mob =>
              MinionManager.GetMinions(ObjectManager.Player.Position,
                  500,
                  MinionTypes.All,
                  MinionTeam.Neutral).FirstOrDefault();

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians()
                || Mob == null
                || !sender.IsMe
                || !args.SData.IsAutoAttack())
            {
                return;
            }

            if (spell.W1)
            {
                spell.Spell.Cast();
            }
            else if(!spell.W1 && spell.PassiveStacks <= 1)
            {
                spell.Spell.CastOnUnit(ObjectManager.Player);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
        }
    }
}
