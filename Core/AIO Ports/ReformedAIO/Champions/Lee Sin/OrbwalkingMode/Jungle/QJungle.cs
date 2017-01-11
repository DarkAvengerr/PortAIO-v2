using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QJungle(QSpell spell)
        {
            this.spell = spell;
        }

        private static Obj_AI_Base Mob =>
              MinionManager.GetMinions(ObjectManager.Player.Position,
                                       700,
                                       MinionTypes.All,
                                       MinionTeam.Neutral).FirstOrDefault();

        private readonly string[] bigMobs = { "SRU_Razorbeak", "SRU_Red", "SRU_Blue", "sru_krugmini", "SRU_Krug", "SRU_Gromp", "Sru_Crab", "SRU_Golem", "SRU_Murkwolf", "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        private static bool IsLegendary(GameObject mob)
        {
            return mob.Name.ToLower().Contains("baron")
                || mob.Name.ToLower().Contains("dragon")
                || mob.Name.ToLower().Contains("herald");
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() 
                || Mob == null
                || (Menu.Item("LeeSin.Jungle.Q.Big").GetValue<bool>() && !bigMobs.Contains(Mob.BaseSkinName) && !IsLegendary(Mob)))
            {
                return;
            }

            if (spell.IsQ1)
            {
                if (IsLegendary(Mob) && Mob.HealthPercent < 35 && Mob.HealthPercent > 20)
                {
                    return;
                }
                spell.Spell.Cast(Mob.Position);
            }
            else
            {
                if ((Mob.BaseSkinName == "Sru_Crab" && Mob.Distance(ObjectManager.Player) > 400) || spell.ShouldQ2(Mob) || Mob.Health < spell.GetDamage(Mob))
                {
                    spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("LeeSin.Jungle.Q.Big", "Only Q Big Creeps").SetValue(false));
        }
    }
}
