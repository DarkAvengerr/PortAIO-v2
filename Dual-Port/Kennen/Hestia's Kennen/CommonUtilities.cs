using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen
{
    internal class CommonUtilities : Spells
    {
        public static HitChance GetHitChance(string name)
        {
            var hitChance = ConfigMenu.config.Item(name).GetValue<StringList>();

            switch (hitChance.SList[hitChance.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }

        public static bool CheckZhonya()
        {
            return ItemData.Wooglets_Witchcap.GetItem().IsReady() ||
                   ItemData.Zhonyas_Hourglass.GetItem().IsReady();
        }

        public static void UseZhonya()
        {
            if (!CheckZhonya())
                return;

            if (ItemData.Wooglets_Witchcap.GetItem().IsOwned(ObjectManager.Player))
            {
                ItemData.Wooglets_Witchcap.GetItem().Cast();
            }
            if (ItemData.Zhonyas_Hourglass.GetItem().IsOwned(ObjectManager.Player))
            {
                ItemData.Zhonyas_Hourglass.GetItem().Cast();
            }
        }

        public static float GetComboDamage(Obj_AI_Base target)
        {
            var comboDamage = 0d;

            if (q.LSIsReady())
            {
                comboDamage += q.GetDamage(target);
            }

            if (w.LSIsReady())
            {
                comboDamage += w.GetDamage(target);
            }

            if (ignite.LSIsReady())
            {
                comboDamage += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }
                
            return (float) comboDamage;
        }
    }
}
