using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoShen;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenCombo
    {
        public static void BadaoActive()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!args.SData.IsAutoAttack())
                return;
            if (!(sender is AIHeroClient) || !(args.Target is AIHeroClient))
                return;
            var target = args.Target as AIHeroClient;
            var unit = sender as AIHeroClient;
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (!target.BadaoIsValidTarget(float.MaxValue, false))
                return;
            if (!(target.IsAlly))
                return;
            if (unit is AIHeroClient && unit.IsEnemy && target is AIHeroClient
                && target.Position.To2D().Distance(BadaoShenVariables.SwordPos) <= BadaoMainVariables.W.Range
                && BadaoShenHelper.UseWCombo(target as AIHeroClient))
            {
                BadaoMainVariables.W.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (BadaoMainVariables.E.IsReady() && Orbwalking.CanMove(80) && BadaoShenVariables.ComboEIfHit.GetValue<bool>())
            {
                foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget() 
                && x.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.E.Range))
                {
                    List<AIHeroClient> a = new List<AIHeroClient>();
                    var EPred = BadaoMainVariables.E.GetPrediction(hero);
                    var EndPos = Geometry.Extend(ObjectManager.Player.Position.To2D(), EPred.CastPosition.To2D(), 650);
                    foreach (AIHeroClient hero2 in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget() &&
                    x.NetworkId != hero.NetworkId &&
                    x.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.E.Range))
                    {
                        var PredHero2 = Prediction.GetPrediction(hero2, (hero2.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) /
                            BadaoMainVariables.E.Range) * 0.3f);
                        if (hero2.Position.To2D().Distance(ObjectManager.Player.Position.To2D(), EndPos, true)
                            <= 50 + hero.BoundingRadius &&
                            PredHero2.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D(), EndPos, true)
                            <= 50 + hero.BoundingRadius)
                        {
                            a.Add(hero2);
                        }
                    }
                    if (a.Count() >= BadaoShenVariables.ComboEIfWillHit.GetValue<Slider>().Value + 1)
                    {
                        BadaoMainVariables.E.Cast(EndPos);
                    }
                }
            }
            foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget() &&
                     x.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.E.Range))
            {
                if (BadaoShenHelper.UseEComboToTarget(hero) && Orbwalking.CanMove(80))
                {
                    var PredPos = BadaoMainVariables.E.GetPrediction(hero).CastPosition.To2D();
                    Vector2 CastPos = new Vector2();
                    if (PredPos.Distance(ObjectManager.Player.Position.To2D()) <= 450)
                    {
                        CastPos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredPos, PredPos.Distance(ObjectManager.Player.Position.To2D()) + 200);
                    }
                    else
                    {
                        CastPos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredPos, 650);
                    }
                    if (PredPos.Distance(ObjectManager.Player.Position.To2D()) <= 650)
                    {
                        BadaoMainVariables.E.Cast(CastPos);
                        break;
                    }
                }
            }
            if (BadaoShenHelper.UseQCombo())
            {
                foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget()))
                {
                    var Castpos = BadaoMainVariables.Q.GetPrediction(hero).CastPosition.To2D();
                    if (Castpos.Distance(ObjectManager.Player.Position.To2D(), BadaoShenVariables.SwordPos, true)
                        <= 50 + hero.BoundingRadius)
                    {
                        BadaoMainVariables.Q.Cast();
                    }
                }
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (BadaoShenHelper.UseQCombo())
            {
                BadaoMainVariables.Q.Cast();
            }
        }
    }
}
