using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Katarina
{
    static class Logic
    {
        
        static public Spell Q, W, E, R;
        static public AIHeroClient Player => HeroManager.Player; //Beautiful Lambda

        private static readonly int[] DAMAGEONLEVEL = {75, 78, 87, 94, 102, 111, 120, 131, 143, 155, 168, 183, 198, 214, 231, 248, 267, 287}; //Copied from Katarina's Wikia

        public static void startLogic()
        {
            InitializeFields();
            //Subscriptions
            Game.OnUpdate += Game_OnUpdate;            
        }


        private static void Game_OnUpdate(EventArgs args)
        {
            
            if (Player.HasBuff("katarinarsound"))
            {
                if(Config.GetBoolValue("ks.r"))
                {
                    Killsteal();
                }
                Config.Orbwalker.SetAttack(false);
                Config.Orbwalker.SetAttack(false);
                return;
            }
            Config.Orbwalker.SetAttack(true);
            Config.Orbwalker.SetAttack(true);
            Killsteal();
            //Select current Mode
            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Lasthit();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        private static void InitializeFields()
        {
            Q = new Spell(SpellSlot.Q,625);
            W = new Spell(SpellSlot.W,200);
            E = new Spell(SpellSlot.E,725);
            R = new Spell(SpellSlot.R,550);
        }

        private static void Combo() //Still need to apply permutation - Probably Finished
        {
            if (Config.GetBoolValue("combo.q"))
            {
                QLogic();
            }
            if (Config.GetBoolValue("combo.w"))
            {
                WLogic();
            }
            if (Config.GetBoolValue("combo.e"))
            {
                ELogic();
            }
            if (Config.GetBoolValue("combo.r"))
            {
                RLogic();
            }
        }

        private static void Killsteal()
        {
            if (!Config.GetBoolValue("ks.use")) return;
            foreach(AIHeroClient enemy in HeroManager.Enemies)
            {
                if (enemy.IsDead || enemy.IsZombie) return;
                if(enemy.Distance(Player) < Q.Range && GetDamage(enemy,false) >= enemy.Health)
                {
                    QLogic(true);
                    WLogic();
                    ELogic(enemy);

                }
                if(enemy.Distance(Player) < E.Range && E.GetDamage(enemy) >= enemy.Health)
                {
                    E.Cast(enemy.Position);
                }
            }
        }
        
        private static void QLogic(bool ksMode = false)
        {
            if (!Q.IsReady())
            {
                return;
            }
            AIHeroClient target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Magical);
            if(target == null)
            {
                return;
            }
            if (!ksMode && Config.GetBoolValue("combo.q.minion") && target.Distance(Player) > 400)
            {
                //Optimal Position to attack Target
                Vector3 optimalQPosition = target.ServerPosition.Extend(Player.Position, 350);
                Obj_AI_Base min = ObjectManager.Get<Obj_AI_Base>().Aggregate((x, y) => x.Distance(optimalQPosition) < y.Distance(optimalQPosition) ? x : y);
                if(min.Distance(optimalQPosition) < 130)
                {
                    Q.Cast(min);
                }
            }
            target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target.Distance(Player) < Q.Range && (Config.GetBoolValue("combo.q.direct") && (!Config.GetBoolValue("combo.q.onlyrunaway") || !target.IsFacing(Player) || target.Distance(Player) < 300) || ksMode))
            {
                Q.Cast(target);
            }
            

        }

        private static void WLogic()
        {
            //Maybe needs some adjustment for Passive Resets
            if (HeroManager.Enemies.Any(e => !e.IsDead && e.Distance(Player) <= 275))
            {
                W.Cast();
            }
        }

        private static void ELogic(AIHeroClient target = null)
        {
            if (target == null)
            {
                target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            }
            if (target == null) return;
            Dagger eDagger = DaggerManager.GetDaggers().FirstOrDefault(d => d.GetPosition().Distance(target.Position) <= 330 && d.GetPosition().Distance(Player.Position) >= 140); //Finetuning needed
            if(eDagger != null)
            {
                Vector3 ePositon = eDagger.GetPosition().Extend(target.Position, 140);
                if(Player.Distance(ePositon) <= E.Range)
                {
                    E.Cast(ePositon);
                    return;
                }
                ePositon = Player.Position.Extend(ePositon, E.Range);
                if(ePositon.Distance(eDagger.GetPosition()) <= 140 && ePositon.Distance(target.Position) < 200)
                {
                    E.Cast(ePositon);
                    return;
                }
            }
            if(Config.GetBoolValue("combo.ealways"))
            {
                E.Cast(target.Position.Extend(Player.Position,-50));
            }
        }

        private static void RLogic()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target != null && R.IsReady() && (target.HealthPercent < Config.GetSliderValue("combo.rhealth") || target.Health < R.GetDamage(target,1) * .7) && target.Distance(Player) < (E.IsReady() && Config.GetBoolValue("combo.e") ? E.Range : R.Range - 200))
            {
                if (E.IsReady() && Config.GetBoolValue("combo.e"))
                {
                    E.Cast(target.Position);
                }
                if (W.IsReady() && Config.GetBoolValue("combo.w"))
                {
                    W.Cast();
                }
                if ((!E.IsReady() || !Config.GetBoolValue("combo.e")) && (!W.IsReady() || !Config.GetBoolValue("combo.w")) && target.Distance(Player) <= R.Range)
                {
                    R.Cast();
                }
            }
        }

        private static void LaneClear()
        {
            List<Obj_AI_Base> sourroundingMinions;
            if (Config.GetBoolValue("laneclear.q") && Q.IsReady())
            {
                Obj_AI_Base m = MinionManager.GetMinions(Player.Position, Q.Range).FirstOrDefault();
                if (m != null)
                {
                    Q.Cast(m);
                }
            }

            if (Config.GetBoolValue("laneclear.w") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(W.Range);
                if (sourroundingMinions.Count() > 2)
                {
                    W.Cast();
                }
            }

            if (Config.GetBoolValue("laneclear.e") && E.IsReady())
            {
                Obj_AI_Base m = MinionManager.GetMinions(Player.Position, E.Range).FirstOrDefault(minion => Q.GetDamage(minion) >= minion.Health);
                if (m != null)
                {
                    E.Cast(m);
                }
            }

        }

        private static void Lasthit()
        {
            if (Config.GetBoolValue("lasthit.q") && Q.IsReady())
            {
                Obj_AI_Base m = MinionManager.GetMinions(Player.Position, Q.Range).FirstOrDefault(minion => Q.GetDamage(minion) >= minion.Health);
                if (m != null)
                {
                    Q.Cast(m);
                }
            }

            if (Config.GetBoolValue("lasthit.e") && E.IsReady())
            {
                Obj_AI_Base m = MinionManager.GetMinions(Player.Position, E.Range).FirstOrDefault(minion => Q.GetDamage(minion) >= minion.Health);
                if (m != null)
                {
                    E.Cast(m);
                }
            }

        }

        private static void JungleClear()
        {
            if (Config.GetBoolValue("jungleclear.q") && Q.IsReady())
            {
                Obj_AI_Base defaultMinion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();
                if (defaultMinion != null)
                {
                    Q.Cast(defaultMinion);
                }
            }
            if (Config.GetBoolValue("jungleclear.w") && W.IsReady())
            {
                if (MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Neutral).Any())
                {
                    W.Cast();
                }
            }
            if (Config.GetBoolValue("jungleclear.e") && E.IsReady())
            {
                Obj_AI_Base defaultMinion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();
                if (defaultMinion != null)
                {
                    E.Cast(defaultMinion);
                }
            }
        }

        private static void Harass()
        {
            if (Config.GetBoolValue("harass.q"))
            {
                QLogic(true);
            }
            if (Config.GetBoolValue("harass.w"))
            {
                WLogic();
            }
            if (Config.GetBoolValue("harass.e"))
            {
                ELogic();
            }
        }

        private static double RawPassiveDamage()
        {
            double toReturn = 0;
            if(Player.Level >= 16)
            {
                toReturn += Player.TotalMagicalDamage();
            }
            else if (Player.Level >= 11)
            {
                toReturn += Player.TotalMagicalDamage() * 0.85;
            }
            else if (Player.Level >= 6)
            {
                toReturn += Player.TotalMagicalDamage() * 0.7;
            }
            else
            {
                toReturn += Player.TotalMagicalDamage * 0.55;
            }
            toReturn += (Player.TotalAttackDamage - Player.BaseAttackDamage);
            return toReturn + DAMAGEONLEVEL[Player.Level - 1];
        }

        public static float GetDamage(AIHeroClient target)
        {
            return GetDamage(target, true);
        }

        public static float GetDamage(AIHeroClient target, bool includeUltimateDamage)
        {
            if(target == null)
            {
                return 0;
            }
            float toReturn = 0;
            
            if(DaggerManager.GetDaggers().Any(d => d.GetPosition().Distance(target.Position) < 340))
            {
                toReturn += (float)Player.CalcDamage(target, Damage.DamageType.Magical, RawPassiveDamage());
            }
            if (Q.IsReady())
            {
                toReturn += Q.GetDamage(target);
            }
            if (E.IsReady())
            {
                toReturn += E.GetDamage(target);
            }
            if(includeUltimateDamage && R.IsReady())
            {
                toReturn += R.GetDamage(target, 1);
            }
            return (float)toReturn;
        }
    }
}
