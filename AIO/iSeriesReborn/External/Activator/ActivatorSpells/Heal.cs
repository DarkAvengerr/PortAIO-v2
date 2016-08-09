using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Evade;
using LeagueSharp;
using LeagueSharp.Common;
using SpellData = EloBuddy.SpellData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.ActivatorSpells
{
    class Heal : ISRSpell
    {
        private SpellSlot HealSlot
        {
            get { return ObjectManager.Player.GetSpellSlot("summonerheal"); }
        }

        public void OnLoad()
        {
            DamagePrediction.OnSpellWillKill += DamagePrediction_OnSpellWillKill;
        }

        public void BuildMenu(Menu RootMenu)
        {
            var spellMenu = new Menu("Heal", "iseriesr.activator.spells.heal");
            {
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.heal.onhealth", "On Health < %")).SetValue(new Slider(10,1));
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.heal.ls", "Evade/Damage Prediction integration")).SetValue(true);
                
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.heal.sep11", ">>>         On Mode        <<<"));
                
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.heal.always", "Always")).SetValue(true);
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.heal.combo", "In Combo")).SetValue(true);
            }
            RootMenu.AddSubMenu(spellMenu);
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.heal.always")
                || (MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.heal.combo") && Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo);
        }

        public void Run()
        {
            if (ShouldRun())
            {
                var rSkills = EvadeHelper.EvadeDetectedSkillshots.Where(
                skillshot =>skillshot.IsAboutToHit(250, ObjectManager.Player.ServerPosition)).ToList();

                if (MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.heal.ls"))
                {
                    if (
                        rSkills.Any(
                            skillshot =>
                                skillshot.Caster.GetSpellDamage(ObjectManager.Player, skillshot.SpellData.SpellName) >=
                                ObjectManager.Player.Health + 15))
                    {
                        CastSpell();
                    }
                }

                if (ObjectManager.Player.HealthPercent <=
                    MenuExtensions.GetItemValue<Slider>("iseriesr.activator.spells.heal.onhealth").Value)
                {
                    CastSpell();
                }
            }
        }

        private void DamagePrediction_OnSpellWillKill(AIHeroClient sender, AIHeroClient target, SpellData sData)
        {
            if (target.IsMe && sender.IsEnemy)
            {
                if (ShouldRun() && MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.heal.ls"))
                {
                    CastSpell();
                }
            }
        }

        private void CastSpell()
        {
            if (HealSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.GetSpell(HealSlot).State == SpellState.Ready)
            {
                ObjectManager.Player.Spellbook.CastSpell(HealSlot);
            }
        }
    }
}
