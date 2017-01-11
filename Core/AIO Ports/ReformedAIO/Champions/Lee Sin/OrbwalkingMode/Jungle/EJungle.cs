using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Jungle
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Damage;
    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        private readonly WSpell wSpell;

        public EJungle(ESpell spell, WSpell wSpell)
        {
            this.spell = spell;
            this.wSpell = wSpell;
        }

        private List<Obj_AI_Base> Mob =>
              MinionManager.GetMinions(ObjectManager.Player.Position,
                  spell.Spell.Range,
                  MinionTypes.All,
                  MinionTeam.Neutral);

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians() 
                || Mob == null 
                || !sender.IsMe 
                || !args.SData.IsAutoAttack())
            {
                return;
            }

            foreach (var m in Mob)
            {
                if ((Menu.Item("LeeSin.Jungle.E.Killable").GetValue<bool>() && m.Health < spell.GetDamage(m))
                    || !wSpell.Spell.IsReady()
                    || !spell.E1
                    && (spell.ShouldE2(m) || spell.PassiveStacks <= 1))
                {
                    spell.CastItem();
                    spell.Spell.Cast(m);
                }
              
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

            Menu.AddItem(new MenuItem("LeeSin.Jungle.E.Killable", "Use If Killable").SetValue(true).SetTooltip("Will use when W is down too"));
        }
    }
}
