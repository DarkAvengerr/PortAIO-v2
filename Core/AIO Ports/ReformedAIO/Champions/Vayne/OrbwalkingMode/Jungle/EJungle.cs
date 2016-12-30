using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Jungle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Condemn_Logic;
    using ReformedAIO.Champions.Vayne.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        private readonly CondemnTypes condemnTypes;

        public EJungle(ESpell spell, CondemnTypes condemnTypes)
        {
            this.spell = spell;
            this.condemnTypes = condemnTypes;
        }

        private List<Obj_AI_Base> Mob =>
            MinionManager.GetMinions(ObjectManager.Player.Position,
                spell.Spell.Range,
                MinionTypes.All,
                MinionTeam.Neutral);

        private readonly string[] mobStrings = { "SRU_Razorbeak", "SRU_Red", "SRU_Blue", "SRU_Krug", "SRU_Gromp", "Sru_Crab", "SRU_Murkwolf" };

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null
                || Menu.Item("Vayne.Jungle.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Mob)
            {
                if (!mobStrings.Contains(m.BaseSkinName))
                {
                    return;
                }

                switch (Menu.Item("Vayne.Jungle.E.Mode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (condemnTypes.Reformed(m, Menu.Item("Vayne.Jungle.E.Push").GetValue<Slider>().Value, spell.Spell))
                        {
                            spell.Spell.CastOnUnit(m);
                        }
                        break;

                    case 1:
                        if (condemnTypes.Marksman(m, spell.Spell))
                        {
                            spell.Spell.CastOnUnit(m);
                        }
                        break;

                    case 2:
                        if (condemnTypes.SharpShooter(
                            m,
                            spell.Spell,
                            Menu.Item("Vayne.Jungle.E.Push").GetValue<Slider>().Value))
                        {
                            spell.Spell.CastOnUnit(m);
                        }
                        break;
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

            Menu.AddItem(new MenuItem("Vayne.Jungle.E.Mode", "Mode: ").SetValue(new StringList(new[] { "Reformed", "Marksman", "Sharpshooter", "VHR" })));

            Menu.AddItem(new MenuItem("Vayne.Jungle.E.Push", "Push Distance").SetValue(new Slider(470, 0, 470)));

            Menu.AddItem(new MenuItem("Vayne.Jungle.E.Mana", "Min Mana %").SetValue(new Slider(10, 0, 100)));
        }
    }
}
