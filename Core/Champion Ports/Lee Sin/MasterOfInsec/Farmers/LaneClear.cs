using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
    internal static class LaneClear
    {
        public static void Do()
        {
            try
            {


                var MinionN =
                    MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault();
                var useQ = Program.menu.Item("QL").GetValue<bool>();
                var useW = Program.menu.Item("WL").GetValue<bool>();
                var useE = Program.menu.Item("EL").GetValue<bool>();

                if (Program.Q.IsReady() &&
                    ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo")
                {
                    if (Program.menu.Item("lpassive").GetValue<bool>())
                        Data.castSpell(Program.Q, "lpassive", Program.Player);
                }
                else if (Program.W.IsReady() &&
                         ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonkwtwo")
                {
                    if (Program.menu.Item("lpassive").GetValue<bool>())
                        Data.castSpell(Program.W, "lpassive", Program.Player);
                }
                else if (Program.E.IsReady() &&
                         ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonketwo")
                {
                    if (Program.menu.Item("lpassive").GetValue<bool>())
                        Data.castSpell(Program.W, "lpassive", Program.Player);
                }
                if (Program.Q.IsReady() && useQ)
                {
                    if (MinionN.Distance(ObjectManager.Player.Position) <= Program.Q.Range)
                    {
                        if (Program.menu.Item("lpassive").GetValue<bool>())
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () => Data.castSpell(Program.Q, "lpassive", MinionN));
                        else
                            Data.castSpell(Program.Q, "lpassive", MinionN);
                    }
                }
                if (Program.E.IsInRange(MinionN) && Program.W.IsReady() && useW)
                {
                    if (Program.menu.Item("lpassive").GetValue<bool>())
                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => Data.castSpell(Program.W, "lpassive", Program.Player));
                    else
                        Data.castSpell(Program.W, "lpassive", Program.Player);
                }
                var MinionNe = MinionManager.GetMinions(Program.E.Range, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth);
                if (Program.E.IsReady() && useE)
                {
                    if (MinionNe.Count >= Program.menu.Item("LMinE").GetValue<Slider>().Value)
                    {
                        if (Program.menu.Item("lpassive").GetValue<bool>())
                            Data.castSpell(Program.E, "lpassive");
                        else
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () => Data.castSpell(Program.E, "lpassive"));
                        if (Items.CanUseItem(3077) && Program.Player.Distance(MinionN.Position) < 350)
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074) && Program.Player.Distance(MinionN.Position) < 350)
                            Items.UseItem(3074);
                    }
                }
            }
            catch (Exception e ){ }
        }

    }
}