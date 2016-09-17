using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoGP;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankAuto
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - BadaoGangplankCombo.LastCondition >= 100 + Game.Ping)
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.Distance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalking.Attack = false;
                                Orbwalking.Move = false;
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalking.Attack = true;
                                    Orbwalking.Move = true;
                                });
                                if (BadaoMainVariables.Q.Cast(barrel.Bottle) == Spell.CastStates.SuccessfullyCasted)
                                {
                                    BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }

                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (Orbwalking.CanAttack())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.Distance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalking.Attack = false;
                                Orbwalking.Move = false;
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalking.Attack = true;
                                    Orbwalking.Move = true;
                                });
                                if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                                {
                                    BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (BadaoMainVariables.W.IsReady() &&
                BadaoGangplankVariables.AutoWLowHealth.GetValue<bool>() &&
                BadaoGangplankVariables.AutoWLowHealthValue.GetValue<Slider>().Value >= Player.Health*100/Player.MaxHealth)
            {
                BadaoMainVariables.W.Cast();
            }
            if (BadaoMainVariables.W.IsReady()
                && BadaoGangplankVariables.AutoWCC.GetValue<bool>())
            {
                foreach (var bufftype in new BuffType[] {BuffType.Stun, BuffType.Snare, BuffType.Suppression,
                BuffType.Silence,BuffType.Taunt,BuffType.Charm,BuffType.Blind,BuffType.Fear,BuffType.Polymorph})
                {
                    if (Player.HasBuffOfType(bufftype))
                        BadaoMainVariables.W.Cast();
                }
            }
            if (BadaoMainVariables.Q.IsReady())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.Q.Range) 
                && BadaoMainVariables.Q.GetDamage(x) >= x.Health))
                {
                    BadaoMainVariables.Q.Cast(hero);
                }
            }
        }
    }
}
