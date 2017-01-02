using System.Collections.Generic;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Common;
    using System.Linq;
    using Spells;
    using SharpDX;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Flee : Logic // WallJump Credit By tulisan69
    {
        private static int lastCastTime;

        private static readonly Vector2 spot1 = new Vector2(7274, 5908);
        private static readonly Vector2 spot2 = new Vector2(8222, 3158);
        private static readonly Vector2 spot3 = new Vector2(3674, 7058);
        private static readonly Vector2 spot5 = new Vector2(8372, 9606);
        private static readonly Vector2 spot6 = new Vector2(6650, 11766);
        private static readonly Vector2 spot7 = new Vector2(1678, 8428);
        private static readonly Vector2 spot10 = new Vector2(6424, 5208);
        private static readonly Vector2 spot11 = new Vector2(13172, 6508);
        private static readonly Vector2 spot12 = new Vector2(11222, 7856);
        private static readonly Vector2 spot13 = new Vector2(10372, 8456);
        private static readonly Vector2 spot14 = new Vector2(4324, 6258);
        private static readonly Vector2 spot16 = new Vector2(7672, 8906);
        private static readonly Vector2 spotC = new Vector2(2232, 8412);
        private static readonly Vector2 spotD = new Vector2(7046, 5426);
        private static readonly Vector2 spotE = new Vector2(8322, 2658);
        private static readonly Vector2 spotG = new Vector2(3892, 6466);
        private static readonly Vector2 spotH = new Vector2(12582, 6402);
        private static readonly Vector2 spotI = new Vector2(11072, 8306);
        private static readonly Vector2 spotJ = new Vector2(10882, 8416);
        private static readonly Vector2 spotL = new Vector2(6574, 12256);
        private static readonly Vector2 spotN = new Vector2(7784, 9494);

        internal static void Init()
        {
            if (Menu.Item("FleeWallJump", true).GetValue<bool>() && WallJumpPos.Any(x => x.DistanceToPlayer() <= E.Range + 50))
            {
                WallVisibleGetLogic();
                WallJumpLogic();
            }
            else
            {
                if (IsDashing)
                {
                    if (Menu.Item("FleeQ", true).GetValue<bool>() && Q.IsReady() && !SpellManager.HaveQ3)
                    {
                        var qMinion = MinionManager.GetMinions(lastEPos, 220, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();

                        if (qMinion.IsValidTarget(220))
                        {
                            Q.Cast(Me.Position, true);
                        }
                    }
                }
                else
                {
                    if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
                    {
                        var dashList = new List<Obj_AI_Base>();
                        dashList.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(E.Range)));
                        dashList.AddRange(MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly));

                        var dash = dashList.Where(
                                i => SpellManager.CanCastE(i) &&
                                     PosAfterE(i).Distance(Game.CursorPos) < Game.CursorPos.DistanceToPlayer()
                                     && Evade.EvadeManager.IsSafe(PosAfterE(i).To2D()).IsSafe)
                            .MinOrDefault(i => Game.CursorPos.Distance(PosAfterE(i)));

                        if (dash.IsValidTarget(E.Range) && SpellManager.CanCastE(dash))
                        {
                            E.CastOnUnit(dash, true);
                            return;
                        }
                    }

                    if (Menu.Item("FleeQ3", true).GetValue<bool>() && SpellManager.HaveQ3 && Q3.IsReady() &&
                        HeroManager.Enemies.Any(x => x.IsValidTarget(Q3.Range)))
                    {
                        SpellManager.CastQ3();
                    }
                }
            }
        }

        private static void WallVisibleGetLogic()
        {
            if (Me.Distance(spot1) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(7110, 5612).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(7110, 5612).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot2) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(8372, 2908).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(8372, 2908).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot3) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(3674, 6708).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(3674, 6708).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }


            if (Me.Distance(spot5) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(7923, 9351).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(7923, 9351).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot6) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(6426, 12138).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(6426, 12138).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot7) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(2050, 8416).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(2050, 8416).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot10) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(6824, 5308).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(6824, 5308).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot11) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(12772, 6458).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(12772, 6458).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot12) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(11072, 8156).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(11072, 8156).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot13) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(10772, 8456).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(10772, 8456).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot14) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(4024, 6358).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(4024, 6358).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }

            if (Me.Distance(spot16) <= 150)
            {
                if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 &&
                    Utils.TickCount - lastCastTime > 5000)
                {
                    if (Me.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, new Vector2(7822, 9306).To3D()))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
                else if (W.IsReady() && Utils.TickCount - lastCastTime > 5000)
                {
                    if (W.Cast(new Vector2(7822, 9306).To3D(), true))
                    {
                        lastCastTime = Utils.TickCount;
                    }
                }
            }
        }

        private static void WallJumpLogic()
        {
            var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

            if (jminions.Any())
            {
                foreach (var jungleMobs in jminions)
                {
                    if (Me.Distance(spot1) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot2) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot3) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot5) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot6) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot7) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot10) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot11) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot12) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot13) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot14) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spot16) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotC) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotD) <= 150)
                    {
                        if (jungleMobs.BaseSkinName != "SRU_Razorbreak"
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini3.1.2"
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini3.1.4"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotE) <= 150)
                    {
                        if (jungleMobs.BaseSkinName == "SRU_KrugMini"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotG) <= 150)
                    {
                        if (jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini2.1.3" && jungleMobs.IsVisible
                            && E.IsReady() && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotH) <= 150)
                    {
                        if (jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotI) <= 120)
                    {
                        if (jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini8.1.3"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotJ) <= 120)
                    {
                        if (jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini8.1.2"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotL) <= 150)
                    {
                        if (jungleMobs.BaseSkinName == "SRU_KrugMini"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }

                    if (Me.Distance(spotN) <= 150)
                    {
                        if (jungleMobs.BaseSkinName != "SRU_RazorbreakMini9.1.2"
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini9.1.4"
                            && jungleMobs.BaseSkinName != "SRU_Razorbreak" && jungleMobs.IsVisible
                            && E.IsReady() && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }
        }
    }
}
