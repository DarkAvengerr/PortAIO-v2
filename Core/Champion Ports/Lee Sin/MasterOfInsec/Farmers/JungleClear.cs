using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
    static class JungleClear
    {
    public static void  Do()
        {
            var MinionN = MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useQ = Program.menu.Item("QJ").GetValue<bool>();
            var useW = Program.menu.Item("WJ").GetValue<bool>();
            var useE = Program.menu.Item("EJ").GetValue<bool>();
            if (MinionN == null) return;
            if (Program.Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo")
            {
                if (Program.menu.Item("jpassive").GetValue<bool>())
                    MasterOfInsec.Data.castSpell(Program.Q, "jpassive", Program.Player);
            }
            else if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonkwtwo")
            {
                if (Program.menu.Item("jpassive").GetValue<bool>())
                    MasterOfInsec.Data.castSpell(Program.W, "jpassive", Program.Player);
            }
            else if (Program.E.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonketwo")
            {
                if (Program.menu.Item("jpassive").GetValue<bool>())
                    MasterOfInsec.Data.castSpell(Program.W, "jpassive", Program.Player);
            }
            if (Program.Q.IsReady() && useQ)
            {
                if (MinionN.Distance(ObjectManager.Player.Position) <= Program.Q.Range)
                {
                    if (Program.menu.Item("jpassive").GetValue<bool>())
                     LeagueSharp.Common.Utility.DelayAction.Add(100, () =>   MasterOfInsec.Data.castSpell(Program.Q, "jpassive", MinionN));
                    else
                        MasterOfInsec.Data.castSpell(Program.Q, "jpassive", MinionN);
                }
            }
            if (Program.E.IsInRange(MinionN)&&Program.W.IsReady() && useW)
            {
                if (Program.menu.Item("jpassive").GetValue<bool>())
                  LeagueSharp.Common.Utility.DelayAction.Add(100, () =>   MasterOfInsec.Data.castSpell(Program.W, "jpassive", Program.Player));
                else
                MasterOfInsec.Data.castSpell(Program.W, "jpassive", Program.Player);
            }
            var MinionNe = MinionManager.GetMinions(Program.E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Program.E.IsReady() && useE)
            {
                if (MinionNe.Count >= Program.menu.Item("JMinE").GetValue<Slider>().Value)
                {
                    if (Program.menu.Item("jpassive").GetValue<bool>())
                        MasterOfInsec.Data.castSpell(Program.E, "jpassive");
                    else
                   LeagueSharp.Common.Utility.DelayAction.Add(100, () =>    MasterOfInsec.Data.castSpell(Program.E, "jpassive"));
                    if (Items.CanUseItem(3077) && Program.Player.Distance(MinionN.Position) < 350)
                        Items.UseItem(3077);
                    if (Items.CanUseItem(3074) && Program.Player.Distance(MinionN.Position) < 350)
                        Items.UseItem(3074);
                }
                
            }

        
        }

        }

    }

