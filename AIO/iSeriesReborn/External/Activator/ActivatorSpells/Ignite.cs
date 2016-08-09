using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.ActivatorSpells
{
    class Ignite : ISRSpell
    {
        private SpellSlot IgniteSlot => ObjectManager.Player.GetSpellSlot("summonerdot");

        public void OnLoad()
        {
        }

        public void BuildMenu(Menu RootMenu)
        {
            var spellMenu = new Menu("Ignite", "iseriesr.activator.spells.ignite");
            {
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.ignite.sep11", ">>>         On Mode        <<<"));

                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.ignite.always", "Always")).SetValue(true);
                spellMenu.AddItem(new MenuItem("iseriesr.activator.spells.ignite.combo", "In Combo")).SetValue(true);
            }
            RootMenu.AddSubMenu(spellMenu);
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.ignite.always")
                || (MenuExtensions.GetItemValue<bool>("iseriesr.activator.spells.ignite.combo") && Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo);
        }

        public void Run()
        {
            if (ShouldRun())
            {
                var hero =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            h => h.IsValidTarget(465f)).OrderBy(m => m.Health);
                    var myHero = hero.FirstOrDefault();
                    if (myHero != null)
                    {
                        var healSlot = myHero.GetSpellSlot("summonerheal");
                        var health = myHero.Health + 10;

                        if (healSlot != SpellSlot.Unknown && myHero.Spellbook.GetSpell(healSlot).State == SpellState.Ready)
                        {
                            //Let's assume he'll use heal. 0.5f because of grevious wounds.
                            health += GetHealAmount(myHero) * 0.5f;
                        }

                        if (health <= ObjectManager.Player.GetSummonerSpellDamage(myHero, Damage.SummonerSpell.Ignite))
                        {
                            CastSpell(myHero);
                        }
                    }
            }
        }

        private int GetHealAmount(AIHeroClient Hero)
        {
            return 90 + (Hero.Level - 1) * 15;
        }

        private void CastSpell(AIHeroClient target)
        {
            if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.GetSpell(IgniteSlot).State == SpellState.Ready)
            {
                ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, target);
            }
        }
    }
}
