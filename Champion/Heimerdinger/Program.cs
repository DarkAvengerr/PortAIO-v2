/*
 

                                                    __                    .-'''-.                                                
       .-''-.                    .---.         ...-'  |`. _______        '   _    \                                              
     .' .-.  )         .--.      |   |         |      |  |\  ___ `'.   /   /` '.   \   _..._               __.....__             
    / .'  / /    .--./)|__|      |   |         ....   |  | ' |--.\  \ .   |     \  ' .'     '.  .--./) .-''         '.           
   (_/   / /    /.''\\ .--.-,.--.|   |           -|   |  | | |    \  '|   '      |  .   .-.   ./.''\\ /     .-''"'-.  `..-,.--.  
        / /    | |  | ||  |  .-. |   |            |   |  | | |     |  \    \     / /|  '   '  | |  | /     /________\   |  .-. | 
       / /      \`-' / |  | |  | |   |      _  ...'   `--' | |     |  |`.   ` ..' / |  |   |  |\`-' /|                  | |  | | 
      . '       /("'`  |  | |  | |   |    .' | |         |`| |     ' .'   '-...-'`  |  |   |  |/("'` \    .-------------| |  | | 
     / /    _.-'\ '---.|  | |  '-|   |   .   | ` --------\ | |___.' /'              |  |   |  |\ '---.\    '-.____...---| |  '-  
   .' '  _.'.-'' /'""'.|__| |    |   | .'.'| |//`---------/_______.'/               |  |   |  | /'""'.\`.             .'| |      
  /  /.-'_.'    ||     || | |    '---.'.'.-'  /           \_______|/                |  |   |  |||     || `''-...... -'  | |      
 /    _.'       \'. __//  |_|        .'   \_.'                                      |  |   |  |\'. __//                 |_|      
( _.-'           `'---'                                                             '--'   '--' `'---'                           

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Threading;
using EloBuddy;

namespace Two_Girls_One_Donger
{
    internal class Program
    {
        private const string Champion = "Heimerdinger";
        private static Orbwalking.Orbwalker Orbwalker;
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell Q1;
        private static Spell W1;
        private static Spell E1;
        public static Spell E2;
        public static Spell E3;
        private static Spell R;
        private static Menu Config;
        private static List<Vector3> TurretSpots;
        private static Items.Item ZHO;
        public static SpellSlot Ignite;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static void Game_OnGameLoad()
        {
            Notifications.AddNotification("2Girls1Donger Loaded", 3000);
            if (Player.CharData.BaseSkinName != Champion) return;

            #region spells            
            ZHO = new Items.Item(3157, 1f);
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);

            W1 = new Spell(SpellSlot.W, 1100);
            E1 = new Spell(SpellSlot.E, 925);
            E2 = new Spell(SpellSlot.E, 1125);
            E3 = new Spell(SpellSlot.E, 1325);

            Q.SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            W1.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E1.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E2.SetSkillshot(0.25f + E1.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);
            E3.SetSkillshot(0.3f + E2.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);
            #endregion
        
            #region Menu
            //Menu
            Config = new Menu(Champion, "2Girls1Donger", true);

            //Targetselector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //LaneClear
            Config.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("LaneclearW", "Use W")).SetValue(true);
            Config.SubMenu("Laneclear").AddItem(new MenuItem("LaneclearE", "Use E")).SetValue(false);
            Config.SubMenu("Laneclear").AddItem(new MenuItem("LaneMana", "Minimum Mana for clear")).SetValue(new Slider(30, 0, 100));

            //C-C-C-Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQRCombo", "Use Q Upgrade")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("QRcount", "Minimum Enemies for Q upgrade")).SetValue(new Slider(2, 1, 5));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWRCombo", "Use W Upgrade")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseERCombo", "Use E Upgrade")).SetValue(true);
            //Config.SubMenu("Combo").AddItem(new MenuItem("UseEAOECombo", "ER AOE")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ERcount", "Minimum Enemies to Stun")).SetValue(new Slider(3, 1, 5));
            Config.SubMenu("Combo").AddItem(new MenuItem("KS", "Killsteal")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ZhoUlt", "Ult + Q > Zhonyas")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            
            //MISCMENU
            Config.SubMenu("Misc").AddItem(new MenuItem("AntiGap", "Anti Gapcloser - E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Interrupt Spells - E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoHarras", "Auto Harass W").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle, true)));
            Config.SubMenu("Misc").AddItem(new MenuItem("ManaW", "Auto Harass if % MP >").SetValue(new Slider(30, 1, 100)));
            Config.AddToMainMenu();

            // Interruption
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            Game.OnUpdate += OnGameUpdate;

        }
            #endregion

            #region wip
        private static void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var lanemana = Config.Item("LaneMana").GetValue<Slider>().Value;
            var laneclear = (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear);
            var MinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width);
            var Wfarmpos = W.GetLineFarmLocation(MinionsW, W.Width);
            var Efarmpos = E.GetCircularFarmLocation(MinionsE, E.Width);
            
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Wfarmpos.MinionsHit >= 3 && Config.Item("LaneclearW").GetValue<bool>()
                && Player.ManaPercent >= lanemana)
            {
                W.Cast(Wfarmpos.Position);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Efarmpos.MinionsHit >= 3 && MinionsE.Count >= 1 && Config.Item("LaneclearE").GetValue<bool>()
                && Player.ManaPercent >= lanemana)
            {
                E.Cast(Efarmpos.Position);
            }

            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Config.Item("KS").GetValue<bool>())
            {
                KS();
            }
            if (Config.Item("ZhoUlt").GetValue<bool>())
            {
                ZhoUlt();
            }
            if (Config.Item("AutoHarras").GetValue<KeyBind>().Active)
            {
                AutoHarras();
            }

        }


        private static void CastER(Obj_AI_Base target) // copied from ScienceARK
        {

            PredictionOutput prediction;

            if (ObjectManager.Player.Distance(target) < E1.Range)
            {
                var oldrange = E1.Range;
                E1.Range = E2.Range;
                prediction = E1.GetPrediction(target, true);
                E1.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E2.Range)
            {
                var oldrange = E2.Range;
                E2.Range = E3.Range;
                prediction = E2.GetPrediction(target, true);
                E2.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E3.Range)
            {
                prediction = E3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <= E1.Range + E1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100 *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }
                    R.Cast();
                    E1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <=
                         ((E1.Range + E1.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), E1.Range - 100);
                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            E1.Range *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
                                ();

                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
            }
        }
        
        private static void ZhoUlt()
        {
            var fullHP = Player.MaxHealth;
            var HP = Player.Health;
            var critHP = fullHP / 4;
            if (HP <= critHP)
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
                if (target == null) return;
                R.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add(1010, () => Q.Cast(Player.Position));
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Q.Cast(Player.Position));
                LeagueSharp.Common.Utility.DelayAction.Add(1200, () => ZHO.Cast());
            }

        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            var qtarget = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (qtarget == null)
                return;
            var wpred = W.GetPrediction(target);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Q.IsReady() && R.IsReady() && Config.Item("UseQRCombo").GetValue<bool>() &&
                    Config.Item("UseQCombo").GetValue<bool>() && qtarget.IsValidTarget(650) &&
                    Player.Position.CountEnemiesInRange(650) >=
                    Config.Item("QRcount").GetValue<Slider>().Value)
                {
                    R.Cast();
                    Q.Cast(Player.Position.Extend(target.Position, +300));
                }
                else
                {
                    if (Q.IsReady() && Config.Item("UseQCombo").GetValue<bool>() && qtarget.IsValidTarget(650) &&

                        Player.Position.CountEnemiesInRange(650) >= 1)
                    {
                        Q.Cast(Player.Position.Extend(target.Position, +300));
                    }
                }
                if (E3.IsReady() && R.IsReady() && Config.Item("UseERCombo").GetValue<bool>() &&
                    Config.Item("UseRCombo").GetValue<bool>() &&
                    target.Position.CountEnemiesInRange(450 - 250) >=
                    Config.Item("ERcount").GetValue<Slider>().Value)
                {
                    CastER(target);
                }
                else
                {
                    if (E.IsReady() && Config.Item("UseECombo").GetValue<bool>() && target.IsValidTarget(E.Range))
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                    if (W.IsReady() && Config.Item("UseWRCombo").GetValue<bool>() && Config.Item("UseRCombo").GetValue<bool>() &&
                        R.IsReady() && target.IsValidTarget(W.Range) &&
                        wpred.Hitchance >= HitChance.High && CalcDamage(target) >= target.Health)
                    {
                        R.Cast();

                        LeagueSharp.Common.Utility.DelayAction.Add(1010,
                            () => W.CastIfHitchanceEquals(target, HitChance.High, true));
                    }
                    else
                    {
                        if (W.IsReady() && Config.Item("UseWCombo").GetValue<bool>() && target.IsValidTarget(W.Range))
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                        }
                    }
                }
            }
        }
        private static float GetDistance(AttackableUnit target)
        {
            return Vector3.Distance(Player.Position, target.Position);
        }
        private static void AutoHarras()
        {
          var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
          if (target == null || !target.IsValid)
              return;
          var harassmana = Config.Item("ManaW").GetValue<Slider>().Value;
          var useW = Config.Item("AutoHarras").GetValue<KeyBind>().Active;
          if (W.IsReady() && target.IsValidTarget() && useW && (Player.Mana / Player.MaxMana) * 100 > harassmana)
            {
              W.CastIfHitchanceEquals(target,HitChance.High,true);
            }
          }

        private static int CalcDamage(Obj_AI_Base target)
        {



            //Calculate Combo Damage

            var aa = Player.GetAutoAttackDamage(target, true);
            var damage = aa;

            if (Ignite != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(Ignite) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);


            if (Config.Item("UseE").GetValue<bool>()) // edamage
            {
                if (E.IsReady())
                {
                    damage += E.GetDamage(target);
                }
            }

            if (E.IsReady() && Config.Item("UseE").GetValue<bool>()) // rdamage
            {

                damage += E.GetDamage(target);
            }

            if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
            {
                damage += W.GetDamage(target);
            }
            if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
            {
                if (R.IsReady() && Config.Item("UseW").GetValue<bool>() && Config.Item("UseR").GetValue<bool>())
                    damage += W.GetDamage(target) * 2.2;
            }
            return (int)damage;

        }

        private static void KS()
        {
            var target = TargetSelector.GetTarget(E.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetEDamage(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
                E.CastIfHitchanceEquals(target, HitChance.High, true);
                return;
            }


            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetWDamage(target))
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {

                    W.Cast(prediction.CastPosition);
                    return;
                }
            }

            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetW1Damage(target) && R.IsReady())
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    R.Cast();
                    W.Cast(prediction.CastPosition);
                    W.Cast(prediction.CastPosition);
                    return;
                }
            }

        }

        private static float GetWDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        private static float GetW1Damage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W1.IsReady() && R.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W, 1);

            return (float)damage * 2;
        }

        private static float GetEDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (E.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            return (float)damage * 2;
        }
        #endregion

            #region Misc
        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) && Config.Item("AntiGap").GetValue<bool>())
                E.Cast(gapcloser.End);
        }


        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && Config.Item("Interrupt").GetValue<bool>())
                E.Cast(sender.Position);
        }
        #endregion
    }
}