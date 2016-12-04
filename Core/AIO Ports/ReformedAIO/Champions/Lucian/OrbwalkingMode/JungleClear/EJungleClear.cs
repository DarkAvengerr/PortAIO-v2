using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.JungleClear
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungleClear : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        public EJungleClear(ESpell eSpell)
        {
            this.eSpell = eSpell;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians() || !sender.IsMe || Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var mob =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position,
                    ObjectManager.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault();

            if (mob == null)
            {
                return;
            }

            eSpell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 70));
        }


        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Range", "Range").SetValue(new Slider(65, 0, 425)));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
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
    }
}
