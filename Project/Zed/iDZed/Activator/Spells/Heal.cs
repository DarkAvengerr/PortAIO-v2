using System.Linq;
using iDzed.Activator.Spells;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZed.Activator.Spells
{
    class Heal : ISummonerSpell
    {
        public void OnLoad() { }
        public string GetDisplayName()
        {
            return "Heal";
        }

        public void AddToMenu(Menu menu)
        {
            menu.AddItem(
                new MenuItem("com.idz.zed.activator.summonerspells." + GetName() + ".hpercent", "Health %").SetValue(
                    new Slider(25, 1)));
        }

        public bool RunCondition()
        {
            return GetSummonerSpell().IsReady() &&
                   MenuHelper.IsMenuEnabled("com.idz.zed.activator.summonerspells." + GetName() + ".enabled") &&
                   ObjectManager.Player.HealthPercent <=
                   MenuHelper.GetSliderValue("com.idz.zed.activator.summonerspells." + GetName() + ".hpercent") &&
                   ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >= 1;
        }

        public void Execute()
        {
            GetSummonerSpell().Cast();
        }

        public SummonerSpell GetSummonerSpell()
        {
            return SummonerSpells.Heal;
        }

        public string GetName()
        {
            return GetSummonerSpell().Names.First().ToLowerInvariant();
        }
    }
}
