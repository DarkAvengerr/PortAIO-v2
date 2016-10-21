using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Handlers
{
    class ITEMS : Core
    {
        public static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };
        public static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };

        public static void CastHydra()
        {
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            else if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
        }
        public static void CastYoumoo()
        {
            if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }
        public static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Champion.Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }
            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Champion.Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }
            Champion.Smite = Player.GetSpellSlot("summonersmite");
        }
        public static void SmiteJungle()
        {
            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var damage = Player.Spellbook.GetSpell(Champion.Smite).State == SpellState.Ready
                    ? (float)Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.Distance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (damage >= minion.Health)
                            Player.Spellbook.CastSpell(Champion.Smite, minion);
                    }
                }

            }
        }
    }
}
