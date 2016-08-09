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
    class Barrier : ISRSpell
    {
        private SpellSlot barrierSlot
        {
            get { return ObjectManager.Player.GetSpellSlot("summonerbarrier"); }
        }

        public void OnLoad()
        {
            DamagePrediction.OnSpellWillKill += DamagePrediction_OnSpellWillKill;
        }

        public void BuildMenu(Menu RootMenu)
        {
            var spellMenu = new Menu("Barrier", "iseriesr.activator.spells.barrier");
            {
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.barrier.onhealth", "On health < %")).SetValue(new Slider(10, 1));
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.barrier.ls", "Evade/Damage Prediction integration")).SetValue(true);

                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.barrier.sep11", ">>>         On Mode        <<<"));

                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.barrier.always", "Always")).SetValue(true);
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.barrier.combo", "In Combo")).SetValue(true);
            }
            RootMenu.AddSubMenu(spellMenu);
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.barrier.always")
                || (MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.barrier.combo") && Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo);
        }

        public void Run()
        {
            if (ShouldRun())
            {
                var rSkills = EvadeHelper.EvadeDetectedSkillshots.Where(
                skillshot => skillshot.IsAboutToHit(250, ObjectManager.Player.ServerPosition)).ToList();

                if (MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.barrier.ls"))
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

                if (ObjectManager.Player.HealthPercent <
                    MenuExtensions.GetItemValue<Slider>("iseriesr.activator.spells.barrier.onhealth").Value)
                {
                    CastSpell();
                }
            }
        }

        private void DamagePrediction_OnSpellWillKill(AIHeroClient sender, AIHeroClient target, SpellData sData)
        {
            if (target.IsMe && sender.IsEnemy)
            {
                if (ShouldRun() && MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.barrier.ls"))
                {
                    CastSpell();
                }
            }
        }

        private void CastSpell()
        {
            if (barrierSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.GetSpell(barrierSlot).State == SpellState.Ready)
            {
                ObjectManager.Player.Spellbook.CastSpell(barrierSlot);
            }
        }
    }
}
