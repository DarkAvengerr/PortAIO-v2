using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SharpDX;

    internal sealed class SmiteHandler : OrbwalkingChild
    {
        public override string Name { get; set; } = "Smite";

        private readonly QSpell spell;

        private readonly WSpell wSpell;

        public SmiteHandler(QSpell spell, WSpell wSpell)
        {
            this.spell = spell;
            this.wSpell = wSpell;
        }

        private static Obj_AI_Base Mob =>
              MinionManager.GetMinions(ObjectManager.Player.Position,
                                       1100,
                                       MinionTypes.All,
                                       MinionTeam.Neutral).FirstOrDefault();

       
        private static AIHeroClient Target => TargetSelector.GetTarget(ObjectManager.Player, 500, TargetSelector.DamageType.True);

        private static bool IsLegendary(Obj_AI_Base mob)
        {
            return mob.Name.ToLower().Contains("baron")
                || mob.Name.ToLower().Contains("dragon")
                || mob.Name.ToLower().Contains("herald");
        }

        private double Damage => spell.HasQ2(Mob) 
            ? spell.Q2Damage(Mob) + spell.SmiteMonsters()
            : spell.SmiteMonsters();

        private readonly IOrderedEnumerable<Vector3> JumpPositions = new List<Vector3>()
                                                  {
                                                      new Vector3(5772, 10660, 56),
                                                      new Vector3(5373, 11180, 57),
                                                      new Vector3(9107, 4506, 52),
                                                      new Vector3(9220, 3900, 55),
                                                      new Vector3(9493, 3569, 64)
                                                  }.OrderBy(x => x.Distance(ObjectManager.Player.Position));

      
        private void OnUpdate(EventArgs args)
        {
            if (Mob != null)
            {
                if (Menu.Item("LeeSin.Smite.Early").GetValue<bool>() && ObjectManager.Player.Level == 1)
                {
                    return;
                }

                if (IsLegendary(Mob)
                    && Menu.Item("LeeSin.Smite.Steal").GetValue<bool>()
                    && ObjectManager.Player.CountAlliesInRange(300) == 0
                    && Mob.Health < Damage)
                {

                    spell.Spell.Cast(Mob);

                    if (ObjectManager.Player.IsDashing())
                    {
                        wSpell.Jump(JumpPositions.FirstOrDefault(), false, false, true);
                        ObjectManager.Player.Spellbook.CastSpell(spell.Smite, Mob);
                    }
                }

                if (Mob.Health > Damage 
                    || Mob.Distance(ObjectManager.Player) > 500
                    || !Menu.Item("LeeSin.Jungle.Smite.Blue").GetValue<bool>() && Mob.BaseSkinName.Contains("Blue")
                    || (Menu.Item("LeeSin.Jungle.Smite.Ammo").GetValue<bool>() && ObjectManager.Player.Spellbook.GetSpell(spell.Smite).Ammo == 1 && !IsLegendary(Mob)))
                {
                    return;
                }

                if (spell.HasQ2(Mob))
                {
                    spell.Spell.Cast(Mob);
                }

                ObjectManager.Player.Spellbook.CastSpell(spell.Smite, Mob);
            }

            if (Target?.Health < spell.SmiteTargetableDamage()
                && Menu.Item("LeeSin.Killsteal.Smite").GetValue<bool>()
                && Target.Distance(ObjectManager.Player) <= 500)
            {
                ObjectManager.Player.Spellbook.CastSpell(spell.Smite, Target);
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

            Menu.AddItem(new MenuItem("LeeSin.Smite.Early", "Don't Smite Lvl 1").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Smite.Steal", "Steal From Enemy (OP)").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Killsteal.Smite", "Smite Killsteal").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Jungle.Q.Smite", "Use Smite In Jungle").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Jungle.Smite.Ammo", "Save 1 Charge (Smart)").SetValue(true).SetTooltip("Will still smite Baron/Dragon"));

            Menu.AddItem(new MenuItem("LeeSin.Jungle.Smite.Blue", "Smite Blue").SetValue(false));
        }
    }
}
