using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Threading;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Original_Gragas
{
    class R
    {
        public static Spell ult = new Spell(SpellSlot.R, 1150, TargetSelector.DamageType.Magical);
        public static int ExplosionRadius = 400;
        public static int KnockbackRange = 600;
        public static GameObject InsecObject; 
    }

    class Q
    {
        public static Spell barrel = new Spell(SpellSlot.Q, 800, TargetSelector.DamageType.Magical);
        public static Vector3 qPosition = new Vector3(0,0,0);
    }

    class E
    {
        public static Spell bellyslam = new Spell(SpellSlot.E, 600, TargetSelector.DamageType.Magical);
        public static int hitradius = 200;
    }

    class W
    {
        public static Spell drinkstuff = new Spell(SpellSlot.W, HeroManager.Player.AttackRange + HeroManager.Player.BBox.Minimum.Length(), TargetSelector.DamageType.Magical);
        public static int Wradius = 100;
    }

    class GragasSkills
    {
        public static void Flabslap()
        {
            
            if (Program.E_Menu.Item("useE").GetValue<bool>() == false)
            {
                return;
            }
            
            if (Program.orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                AIHeroClient target = null;
                if (Program.E_Menu.Item("focus_unit").GetValue<StringList>().SelectedValue == "Target Selector")
                {
                    target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                }
                else
                {
                    foreach (AIHeroClient unit in HeroManager.Enemies)
                    {
                        if (unit.ChampionName == Program.E_Menu.Item("focus_unit").GetValue<StringList>().SelectedValue)
                        {
                            target = unit;
                        }
                    }
                    if (target == null)
                    {
                        target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                    }
                }

                if (E.bellyslam.IsReady() && E.bellyslam.IsInRange(target, E.bellyslam.Range))
                {
                    PredictionOutput epred = E.bellyslam.GetPrediction(target);
                    if (epred.Hitchance >= HitChance.High)
                    {
                        E.bellyslam.Cast(epred.CastPosition);
                    }
                }
            }
            else if (Program.orbwalker.ActiveMode.ToString().ToLower() == "laneclear")
            {
                List<Obj_AI_Base> jMinions = MinionManager.GetMinions(HeroManager.Player.Position, E.bellyslam.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                var bigminion = (jMinions.Count >= 1) ? jMinions[jMinions.Count - 1] : null;
                if (bigminion == null)
                {
                    return;
                }
                if (bigminion.IsValid && E.bellyslam.IsReady() && E.bellyslam.GetPrediction(bigminion).Hitchance == HitChance.Medium && bigminion.Health > HeroManager.Player.TotalAttackDamage)
                {
                    // Only use E if it will hit the large minion, and dont use E if an auutoattacking will kill the big minion. ;^)
                    E.bellyslam.Cast(E.bellyslam.GetPrediction(bigminion).CastPosition);
                }
                else if (jMinions.Count > 1) //sometimes lists return 1 when empty. 
                {
                    var endpos = E.bellyslam.GetCircularFarmLocation(jMinions, 100).Position;
                    E.bellyslam.Cast(endpos);
                }
            }
        }

        public static void UseW()
        {
            if (!Program.W_Menu.Item("useW").GetValue<bool>())
            {
                return;
            }
            if (Program.orbwalker.ActiveMode.ToString().ToLower() == "laneclear")
            {
                if (Program.W_Menu.Item("laneclearW").GetValue<bool>())
                {
                    List<Obj_AI_Base> lMinions = MinionManager.GetMinions(HeroManager.Player.Position, 300);
                    if (lMinions.Count > Program.W_Menu.Item("num_w").GetValue<Slider>().Value)
                    {
                        W.drinkstuff.Cast();
                    }
                }
                if (Program.W_Menu.Item("jungleW").GetValue<bool>())
                {
                    List<Obj_AI_Base> jMinions = MinionManager.GetMinions(HeroManager.Player.Position, 300, MinionTypes.All, MinionTeam.Neutral);
                    var bigminion = (jMinions.Count >= 1) ? jMinions[jMinions.Count - 1] : null;
                    if (bigminion == null)
                    {
                        return;
                    }
                    if (bigminion.IsValid && W.drinkstuff.IsReady())
                    {
                        W.drinkstuff.Cast();
                    }
                    else if (jMinions.Count > 1)
                    {
                        W.drinkstuff.Cast();
                    }
                }
            }
            else if (Program.orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                if (Program.W_Menu.Item("comboW").GetValue<bool>())
                {
                    foreach (AIHeroClient hero in HeroManager.Enemies)
                    {
                        if (hero.Distance(HeroManager.Player) <= 300)
                        {
                            W.drinkstuff.Cast();
                        }
                    }
                }
            }
        }

        public static void DetonateQ()
        {

            if (Q.qPosition == new Vector3(0,0,0) || Q.barrel.ManaCost != 0 || !Program.Q_Auto.Item("useQ").GetValue<bool>() || !Q.barrel.IsReady())
            {
                return;
            }

            List<AIHeroClient> units = new List<AIHeroClient> { };
            List<Obj_AI_Base> minions = MinionManager.GetMinions(Q.qPosition, 300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
 
            foreach (AIHeroClient unit in HeroManager.Enemies)
            {
                if (unit.Distance(Q.qPosition) <= 300)
                {
                    units.Add(unit);
                }
            }

            foreach (AIHeroClient unit in units)
            {
                if (unit.Distance(Q.qPosition) > 200 && Program.Q_Auto.Item("combo").GetValue<bool>())
                {
                    Q.barrel.Cast();
                }
            }

            int numKillMinions = 0;
            foreach (Obj_AI_Base minion in minions)
            {
                if (minion.Health < Q.barrel.GetDamage(minion))
                {
                    numKillMinions += 1;
                }
            }

            if (Program.Q_Auto.Item("farm").GetValue<bool>() && numKillMinions >= Program.Q_Auto.Item("num_farm").GetValue<Slider>().Value || minions.Count >= Program.Q_Auto.Item("num_clear").GetValue<Slider>().Value)
            {
                Q.barrel.Cast();
            }
        }

        public static void ThrowQ()
        {
            if (Q.barrel.ManaCost == 0)
            {
                return;
            }
            AIHeroClient target = null;
            if (Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue == "Target Selector")
            {
                target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
            }
            else
            {
                foreach (AIHeroClient unit in HeroManager.Enemies)
                {
                    if (unit.ChampionName == Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue)
                    {
                        target = unit;
                    }
                }
                if (target == null)
                {
                    target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                }
            }
            if (Program.orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                if (Q.barrel.IsReady() && target.IsValid && target.Distance(HeroManager.Player) <= Q.barrel.Range)
                {
                    Q.barrel.Cast(Q.barrel.GetPrediction(target).CastPosition);
                }
            }
            else if (Program.orbwalker.ActiveMode.ToString().ToLower() == "laneclear")
            {
                if (Q.barrel.IsReady() && Program.Q_LANECLEAR.Item("useQ").GetValue<bool>())
                {
                    List<Obj_AI_Base> Minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.barrel.Range);
                    MinionManager.FarmLocation farmlocation = Q.barrel.GetCircularFarmLocation(Minions);
                    int hits = farmlocation.MinionsHit;
                    
                    if (hits >= Program.Q_LANECLEAR.Item("min_num").GetValue<Slider>().Value)
                    {
                        Q.barrel.Cast(farmlocation.Position);
                    }
                    List<Obj_AI_Base> jMinions = MinionManager.GetMinions(HeroManager.Player.Position, Q.barrel.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                    farmlocation = Q.barrel.GetCircularFarmLocation(jMinions);
                    if (farmlocation.MinionsHit >= 1)
                    {
                        Q.barrel.Cast(farmlocation.Position);
                    }
                }
            }
        }

        public static void WindyUlt()
        {
            if (Program.orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                AIHeroClient target = null;
                if (Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue == "Target Selector")
                {
                    target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                }
                else
                {
                    foreach (AIHeroClient unit in HeroManager.Enemies)
                    {
                        if (unit.ChampionName == Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue)
                        {
                            target = unit;
                        }
                    }
                    if (target == null)
                    {
                        target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                    }
                }
                if (Q.qPosition != new Vector3(0,0,0) && target.IsValid && target.Distance(Q.qPosition) <= (R.KnockbackRange + 250) && Q.barrel.IsReady())
                {
                    if (E.bellyslam.IsReady() && E.bellyslam.IsInRange(target, E.bellyslam.Range))
                    {
                        PredictionOutput epred = E.bellyslam.GetPrediction(target);
                        if (epred.Hitchance >= HitChance.High)
                        {
                            E.bellyslam.Cast(epred.CastPosition);
                        }
                    }

                    Vector3 qpos = Q.qPosition;
                    Vector3 ultonpos = target.Position + (target.Position - qpos).Normalized() * R.ExplosionRadius;

                    if (R.ult.IsReady() && R.ult.IsInRange(ultonpos))
                    {
                        R.ult.Cast(ultonpos);
                    }
                }
            }
        }

        public static void InsecUlt()
        {

            if (Program.R_Insec.Item("force_key").GetValue<KeyBind>().Active)
            {
                AIHeroClient target = null;
                if (Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue == "Target Selector")
                {
                    target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                }
                else
                {
                    foreach (AIHeroClient unit in HeroManager.Enemies)
                    {
                        if (unit.ChampionName == Program.R_Insec.Item("focus_unit").GetValue<StringList>().SelectedValue)
                        {
                            target = unit;
                        }
                    }
                    if (target == null)
                    {
                        target = TargetSelector.GetTarget(1150, new[] { TargetSelector.DamageType.Physical, TargetSelector.DamageType.Magical }[Program.menu.SubMenu("ts").Item("DamageType").GetValue<StringList>().SelectedIndex]);
                    }
                }
                if (target.IsValid && R.ult.IsReady() && R.ult.IsInRange(target, R.ult.Range))
                {
                    PredictionOutput ultpred = R.ult.GetPrediction(target, true);
                    if (ultpred.AoeTargetsHitCount + 1 >= Program.R_Insec.Item("min_num").GetValue<Slider>().Value)
                    {
                        if (E.bellyslam.IsReady() && E.bellyslam.IsInRange(target, E.bellyslam.Range))
                        {
                            PredictionOutput epred = E.bellyslam.GetPrediction(target);
                            if (epred.Hitchance >= HitChance.High)
                            {
                                E.bellyslam.Cast(epred.CastPosition);
                            }
                        }

                        Vector3 endpos = target.Position;
                        Vector3 ultonpos = endpos + (endpos - HeroManager.Player.Position).Normalized() * (R.ExplosionRadius);
                        Vector3 ultendpos = target.Position + (target.Position - ultonpos).Normalized() * (R.KnockbackRange);

                        if (R.ult.IsInRange(endpos, R.ult.Range))
                        {
                            R.ult.Cast(ultonpos);
                        }
                        
                        if (Q.barrel.IsReady() && Q.barrel.IsInRange(ultendpos, Q.barrel.Range) && Program.R_Insec.Item("barrel_use").GetValue<bool>())
                        {
                            Q.barrel.Cast(ultendpos);
                        }
                        
                    } 
                }
            }
        }
    }
}
