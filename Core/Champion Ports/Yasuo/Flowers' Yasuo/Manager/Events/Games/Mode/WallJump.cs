using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Spells;

    internal class WallJump : Logic
    {
        private static int lastCastTime;
        private static float LastMoveC;

        private static Vector2 spot1 = new Vector2(7274, 5908);
        private static Vector2 spot2 = new Vector2(8222, 3158);
        private static Vector2 spot3 = new Vector2(3674, 7058);
        private static Vector2 spot5 = new Vector2(8372, 9606);
        private static Vector2 spot6 = new Vector2(6650, 11766);
        private static Vector2 spot7 = new Vector2(1678, 8428);
        private static Vector2 spot10 = new Vector2(6424, 5208);
        private static Vector2 spot11 = new Vector2(13172, 6508);
        private static Vector2 spot12 = new Vector2(11222, 7856);
        private static Vector2 spot13 = new Vector2(10372, 8456);
        private static Vector2 spot14 = new Vector2(4324, 6258);
        private static Vector2 spot16 = new Vector2(7672, 8906);
        private static Vector2 spotC = new Vector2(2232, 8412);
        private static Vector2 spotD = new Vector2(7046, 5426);
        private static Vector2 spotE = new Vector2(8322, 2658);
        private static Vector2 spotG = new Vector2(3892, 6466);
        private static Vector2 spotH = new Vector2(12582, 6402);
        private static Vector2 spotI = new Vector2(11072, 8306);
        private static Vector2 spotJ = new Vector2(10882, 8416);
        private static Vector2 spotK = new Vector2(3730, 8080);
        private static Vector2 spotL = new Vector2(6574, 12256);
        private static Vector2 spotN = new Vector2(7784, 9494);

        public static List<Vector2> WallJumpPos = new List<Vector2>();

        internal static void InitPos()
        {
            WallJumpPos.Add(new Vector2(7274, 5908));
            WallJumpPos.Add(new Vector2(8222, 3158));
            WallJumpPos.Add(new Vector2(7784, 9494));
            WallJumpPos.Add(new Vector2(6574, 12256));
            WallJumpPos.Add(new Vector2(3730, 8080));
            WallJumpPos.Add(new Vector2(10882, 8416));
            WallJumpPos.Add(new Vector2(11072, 8306));
            WallJumpPos.Add(new Vector2(12582, 6402));
            WallJumpPos.Add(new Vector2(3892, 6466));
            WallJumpPos.Add(new Vector2(8322, 2658));
            WallJumpPos.Add(new Vector2(7046, 5426));
            WallJumpPos.Add(new Vector2(2232, 8412));
            WallJumpPos.Add(new Vector2(7672, 8906));
            WallJumpPos.Add(new Vector2(4324, 6258));
            WallJumpPos.Add(new Vector2(3674, 7058));
            WallJumpPos.Add(new Vector2(8372, 9606));
            WallJumpPos.Add(new Vector2(6650, 11766));
            WallJumpPos.Add(new Vector2(1678, 8428));
            WallJumpPos.Add(new Vector2(6424, 5208));
            WallJumpPos.Add(new Vector2(13172, 6508));
            WallJumpPos.Add(new Vector2(11222, 7856));
            WallJumpPos.Add(new Vector2(10372, 8456));
        }

        internal static void Init()
        {
            Orbwalker.SetMovement(!WallJumpPos.Any(x => x.DistanceToPlayer() <= 130));

            if (Me.Distance(spot1) <= 150)
            {
                MoveToLimited(spot1.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot1.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot1.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot2) <= 150)
            {
                MoveToLimited(spot2.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot2.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot2.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot3) <= 150)
            {
                MoveToLimited(spot3.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot3.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot3.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot5) <= 150)
            {
                MoveToLimited(spot5.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot5.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot5.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot6) <= 150)
            {
                MoveToLimited(spot6.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot6.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot6.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot7) <= 150)
            {
                MoveToLimited(spot7.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot7.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot7.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot10) <= 150)
            {
                MoveToLimited(spot10.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot10.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot10.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot11) <= 150)
            {
                MoveToLimited(spot11.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot11.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot11.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot12) <= 150)
            {
                MoveToLimited(spot12.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot12.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot12.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot13) <= 150)
            {
                MoveToLimited(spot13.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot13.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot13.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot14) <= 150)
            {
                MoveToLimited(spot14.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot14.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot14.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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
            }

            if (Me.Distance(spot16) <= 150)
            {
                MoveToLimited(spot16.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Q3.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spot16.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }

                if (Me.ServerPosition.Equals(spot16.To3D()) && W.IsReady())
                {
                    if (Items.GetWardSlot() != null && Items.GetWardSlot().Stacks > 0 && Utils.TickCount - lastCastTime > 5000)
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

            if (Me.Distance(spotC) <= 600)
            {
                MoveToLimited(spotC.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotC.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotD) <= 600)
            {
                MoveToLimited(spotD.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(100)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotD.To3D()) && jungleMobs.BaseSkinName != "SRU_Razorbreak"
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini3.1.2"
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini3.1.4"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotE) <= 600)
            {
                MoveToLimited(spotE.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotE.To3D()) && jungleMobs.BaseSkinName == "SRU_KrugMini"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotG) <= 600)
            {
                MoveToLimited(spotG.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Me.AttackRange)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotG.To3D()) && jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini2.1.3" && jungleMobs.IsVisible
                            && E.IsReady() && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotH) <= 600)
            {
                MoveToLimited(spotH.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotH.To3D()) && jungleMobs.IsVisible && E.IsReady()
                            && jungleMobs.IsValidTarget(E.Range) && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotI) <= 120)
            {
                MoveToLimited(spotI.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(100)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotI.To3D()) && jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini8.1.3"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotJ) <= 120)
            {
                MoveToLimited(spotJ.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotJ.To3D()) && jungleMobs.BaseSkinName != "SRU_Murkwolf"
                            && jungleMobs.BaseSkinName != "SRU_MurkwolfMini8.1.2"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotL) <= 600)
            {
                MoveToLimited(spotL.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotL.To3D()) && jungleMobs.BaseSkinName == "SRU_KrugMini"
                            && jungleMobs.IsVisible && E.IsReady() && jungleMobs.IsValidTarget(E.Range)
                            && SpellManager.CanCastE(jungleMobs))
                        {
                            E.CastOnUnit(jungleMobs);
                        }
                    }
                }
            }

            if (Me.Distance(spotN) <= 600)
            {
                MoveToLimited(spotN.To3D());

                var jminions = MinionManager.GetMinions(Me.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral);

                foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(100)))
                {
                    if (jungleMobs != null)
                    {
                        if (Me.ServerPosition.Equals(spotN.To3D())
                            && jungleMobs.BaseSkinName != "SRU_RazorbreakMini9.1.2"
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

        private static void MoveToLimited(Vector3 where)
        {
            if (Utils.TickCount - LastMoveC < 500)
            {
                return;
            }

            LastMoveC = Utils.TickCount;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
    }
}
