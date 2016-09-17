using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Diana
{
    class Logic
    {
        public static AIHeroClient Player => ObjectManager.Player;
        private static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };

        private static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };
        public static SpellSlot Smite;
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
        public static bool HasItem() => ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady();

        // Thanks jQuery for letting me use this! Great guy.
        public static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }
            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }
            Smite = Player.GetSpellSlot("summonersmite");
        }

        public static void SmiteJungle()
        {
            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var StealDmg = Player.Spellbook.GetSpell(Smite).State == SpellState.Ready
                    ? (float)Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.Distance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (StealDmg >= minion.Health)
                            Player.Spellbook.CastSpell(Smite, minion);
                    }
                }
                else if (minion.Distance(Player.ServerPosition) <= 850)
                    {
                        if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                        {
                        var QRDmg = Spells.Q.IsReady() && Spells.R.IsReady()
                   ? Spells.Q.GetDamage(minion) + Spells.R.GetDamage(minion)
                   : 0;
                        if(QRDmg >= minion.Health)
                        {
                            Spells.Q.Cast(minion);
                            Spells.R.Cast(minion);
                        }
                        if (QRDmg + StealDmg >= minion.Health)
                        {
                            Spells.Q.Cast(minion);
                            Spells.R.Cast(minion);
                            Player.Spellbook.CastSpell(Smite, minion);
                        }       
                      }
               }
            }
        }
    }
}
