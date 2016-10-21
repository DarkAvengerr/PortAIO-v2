using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Linq;

    internal class GravesOnTick : Graves
    {
        internal static void Init(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Menu["Key"]["Combo"].GetValue<MenuKeyBind>().Active)
            {
                ComboLogic();
            }

            if (Menu["Key"]["Harass"].GetValue<MenuKeyBind>().Active)
            {
                HarassLogic();
            }

            if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
            {
                LaneLogic();
                JungleLogic();
            }

            AutoLogic();
        }

        private static void ComboLogic()
        {
            var t = Variables.TargetSelector.GetTarget(R);

            if (t != null && t.IsValidTarget() && !t.IsZombie)
            {
                if (Menu["Q"]["Combo"].GetValue<MenuBool>() && Q.IsReady() && Q.IsInRange(t))
                {
                    var QPred = Q.GetPrediction(t, true);

                    if (QPred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(QPred.CastPosition);
                    }
                }

                if (Menu["E"]["Combo"].GetValue<MenuBool>() && E.IsReady())
                {
                    if (CanCaseE(t, Game.CursorPos))
                    {
                        E.Cast(Game.CursorPos);
                        Variables.Orbwalker.ResetSwingTimer();
                    }
                }

                if (Menu["W"]["Combo"].GetValue<MenuBool>() && W.IsReady() && W.IsInRange(t))
                {
                    var WPred = W.GetPrediction(t);

                    if (WPred.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(WPred.CastPosition);
                    }
                }

                if (Menu["R"]["Combo"].GetValue<MenuBool>() && R.IsReady() && !Q.IsReady() && R.IsInRange(t))
                {
                    R.CastIfWillHit(t, Menu["R"]["ComboHit"].GetValue<MenuSlider>().Value);
                }
            }
        }

        private static void HarassLogic()
        {
            var t = Variables.TargetSelector.GetTarget(Q);

            if (Menu["Q"]["Harass"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["Q"]["HarassMana"].GetValue<MenuSlider>().Value && Q.IsReady() && Q.IsInRange(t))
            {
                var QPred = Q.GetPrediction(t, true);

                if (QPred.Hitchance >= HitChance.High)
                {
                    Q.Cast(QPred.CastPosition);
                }
            }
        }

        private static void LaneLogic()
        {
            var Minions = GameObjects.EnemyMinions.Where(m => !m.IsDead && !m.IsZombie && m.IsValidTarget(Q.Range) && m.IsMinion()).ToList();

            if (Menu["Q"]["Lane"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["Q"]["LaneMana"].GetValue<MenuSlider>().Value && Q.IsReady() && Minions.Count() > 0)
            {
                var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                if (QFarm.MinionsHit >= Menu["Q"]["LaneHit"].GetValue<MenuSlider>().Value)
                {
                    Q.Cast(QFarm.Position);
                }
            }
        }

        private static void JungleLogic()
        {
            //var Mobs = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(Q.Range)).ToList();
            var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

            if (Mobs.Count() > 0)
            {
                if (Menu["Q"]["Jungle"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["Q"]["JungleMana"].GetValue<MenuSlider>().Value && Q.IsReady())
                {
                    Q.Cast(Mobs[0].Position);
                }
            }
        }

        private static void AutoLogic()
        {
            if (Menu["Q"]["Auto"].GetValue<MenuBool>() && Q.IsReady())
            {
                foreach (var t in GameObjects.EnemyHeroes.Where(t => !t.IsDead && !t.IsZombie && t.IsValidTarget(Q.Range) && Q.IsInRange(t)))
                {
                    if (t != null)
                    {
                        if(!CanMove(t))
                        {
                            Q.Cast(t);
                        }

                        if (t.Health < Q.GetDamage(t))
                        {
                            Q.Cast(t);
                        }
                    }
                }
            }

            if (Menu["R"]["Auto"].GetValue<MenuBool>() && R.IsReady())
            {
                foreach (var t in GameObjects.EnemyHeroes.Where(t => !t.IsDead && !t.IsZombie && t.IsValidTarget(R.Range) && R.IsInRange(t)))
                {
                    if (t != null)
                    {
                        if (t.Health < R.GetDamage(t) && t.DistanceToPlayer() > Me.GetRealAutoAttackRange() + 100 && Menu["R"][t.ChampionName.ToLower()].GetValue<MenuBool>())
                        {
                            R.Cast(t);
                        }
                    }
                }
            }
        }
    }
}