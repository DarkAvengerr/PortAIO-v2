using System;
using System.Collections.Generic;
using System.Linq;
using SebbyLib;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jinx_Genesis
{
    class Program
    {
        private static string ChampionName = "Jinx";

        public static SebbyLib.Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Spell Q, W, E, R;
        private static float QMANA, WMANA, EMANA ,RMANA;
        private static bool FishBoneActive= false, Combo = false, Farm = false;
        private static AIHeroClient blitz = null;
        private static float WCastTime = Game.Time;

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        private static List<AIHeroClient> Enemies = new List<AIHeroClient>();

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName) return;

            LoadMenu();
            Q = new Spell(SpellSlot.Q, Player.AttackRange);
            W = new Spell(SpellSlot.W, 1490f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f);

            W.SetSkillshot(0.6f, 75f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsEnemy)
                {
                    Enemies.Add(hero);
                }
                else if(hero.ChampionName.Equals("Blitzcrank"))
                {
                    blitz = hero;
                }
            }

            Game.OnUpdate += Game_OnGameUpdate;
            SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Chat.Print("<font color=\"#00BFFF\">GENESIS </font>Jinx<font color=\"#000000\"> by Sebby </font> - <font color=\"#FFFFFF\">Loaded</font>");
        }
       
        private static void LoadMenu()
        {
            Config = new Menu(ChampionName + " GENESIS", ChampionName + " GENESIS", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new SebbyLib.Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddToMainMenu();

            Config.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells").SetValue(true));

            Config.SubMenu("Q Config").AddItem(new MenuItem("Qcombo", "Combo Q").SetValue(true));
            Config.SubMenu("Q Config").AddItem(new MenuItem("Qharass", "Harass Q").SetValue(true));
            Config.SubMenu("Q Config").AddItem(new MenuItem("farmQout", "Farm Q out range AA minion").SetValue(true));
            Config.SubMenu("Q Config").AddItem(new MenuItem("Qlaneclear", "Lane clear x minions").SetValue(new Slider(4, 10, 2)));
            Config.SubMenu("Q Config").AddItem(new MenuItem("Qchange", "Q change mode FishBone -> MiniGun").SetValue(new StringList(new[] { "Real Time", "Before AA"}, 1)));
            Config.SubMenu("Q Config").AddItem(new MenuItem("Qaoe", "Force FishBone if can hit x target").SetValue(new Slider(3, 5, 0)));
            Config.SubMenu("Q Config").AddItem(new MenuItem("QmanaIgnore", "Ignore mana if can kill in x AA").SetValue(new Slider(4, 10, 0)));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                Config.SubMenu("Q Config").SubMenu("Harass Q enemy:").AddItem(new MenuItem("harasQ" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
             
            Config.SubMenu("W Config").AddItem(new MenuItem("Wcombo", "Combo W").SetValue(true));
            Config.SubMenu("W Config").AddItem(new MenuItem("Wharass", "W harass").SetValue(true));
            Config.SubMenu("W Config").AddItem(new MenuItem("Wks", "W KS").SetValue(true));
            Config.SubMenu("W Config").AddItem(new MenuItem("Wts", "Harass mode").SetValue(new StringList(new[] { "Target selector", "All in range" }, 0)));
            Config.SubMenu("W Config").AddItem(new MenuItem("Wmode", "W mode").SetValue(new StringList(new[] { "Out range MiniGun", "Out range FishBone", "Custome range" }, 0)));
            Config.SubMenu("W Config").AddItem(new MenuItem("Wcustome", "Custome minimum range").SetValue(new Slider(600, 1500, 0)));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                Config.SubMenu("W Config").SubMenu("Harass W enemy:").AddItem(new MenuItem("haras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu("E Config").AddItem(new MenuItem("Ecombo", "Combo E").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Etel", "E on enemy teleport").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Ecc", "E on CC").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Eslow", "E on slow").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Edash", "E on dash").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Espell", "E on special spell detection").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("Eaoe", "E if can catch x enemies").SetValue(new Slider(3, 5, 0)));
            Config.SubMenu("E Config").SubMenu("E Gap Closer").AddItem(new MenuItem("EmodeGC", "Gap Closer position mode").SetValue(new StringList(new[] { "Dash end position", "Jinx position"}, 0)));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                Config.SubMenu("E Config").SubMenu("E Gap Closer").SubMenu("Cast on enemy:").AddItem(new MenuItem("EGCchampion" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu("R Config").AddItem(new MenuItem("Rks", "R KS").SetValue(true));
            Config.SubMenu("R Config").SubMenu("Semi-manual cast R").AddItem(new MenuItem("useR", "Semi-manual cast R key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu("R Config").SubMenu("Semi-manual cast R").AddItem(new MenuItem("semiMode", "Semi-manual cast mode").SetValue(new StringList(new[] { "Low hp target", "AOE"}, 0)));
            Config.SubMenu("R Config").AddItem(new MenuItem("Rmode", "R mode").SetValue(new StringList(new[] { "Out range MiniGun ", "Out range FishBone ", "Custome range " }, 0)));
            Config.SubMenu("R Config").AddItem(new MenuItem("Rcustome", "Custome minimum range").SetValue(new Slider(1000, 1600, 0)));
            Config.SubMenu("R Config").AddItem(new MenuItem("RcustomeMax", "Max range").SetValue(new Slider(3000, 10000, 0)));
            Config.SubMenu("R Config").AddItem(new MenuItem("Raoe", "R if can hit x target and can kill").SetValue(new Slider(2, 5, 0)));
            Config.SubMenu("R Config").SubMenu("OverKill protection").AddItem(new MenuItem("Rover", "Don't R if allies near target in x range ").SetValue(new Slider(500, 1000, 0)));
            Config.SubMenu("R Config").SubMenu("OverKill protection").AddItem(new MenuItem("RoverAA", "Don't R if Jinx winding up").SetValue(true));
            Config.SubMenu("R Config").SubMenu("OverKill protection").AddItem(new MenuItem("RoverW", "Don't R if can W KS").SetValue(true));

            //Config.SubMenu("MISC").SubMenu("Use harass mode").AddItem(new MenuItem("LaneClearmode", "LaneClear").SetValue(true));
            //Config.SubMenu("MISC").SubMenu("Use harass mode").AddItem(new MenuItem("Mixedmode", "Mixed").SetValue(true));
            //Config.SubMenu("MISC").SubMenu("Use harass mode").AddItem(new MenuItem("LastHitmode", "LastHit").SetValue(true));

            //Config.SubMenu("Mana Manager").AddItem(new MenuItem("ManaKs", "always safe mana to KS R or W").SetValue(true));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("QmanaCombo", "Q combo mana").SetValue(new Slider(20, 100, 0)));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("QmanaHarass", "Q harass mana").SetValue(new Slider(40, 100, 0)));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("QmanaLC", "Q lane clear mana").SetValue(new Slider(80, 100, 0)));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("WmanaCombo", "W combo mana").SetValue(new Slider(20, 100, 0)));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("WmanaHarass", "W harass mana").SetValue(new Slider(40, 100, 0)));
            Config.SubMenu("Mana Manager").AddItem(new MenuItem("EmanaCombo", "E mana").SetValue(new Slider(20, 100, 0)));

            Config.SubMenu("Prediction Config").AddItem(new MenuItem("PredictionMODE", "Prediction MODE").SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION"}, 1)));
            Config.SubMenu("Prediction Config").AddItem(new MenuItem("Wpred", "W Hit Chance").SetValue(new StringList(new[] {"VeryHigh W", "High W"}, 0)));
            Config.SubMenu("Prediction Config").AddItem(new MenuItem("Epred", "E Hit Chance").SetValue(new StringList(new[] { "VeryHigh E", "High E" }, 0)));
            Config.SubMenu("Prediction Config").AddItem(new MenuItem("Rpred", "R Hit Chance").SetValue(new StringList(new[] { "VeryHigh R", "High R" }, 0)));

            Config.SubMenu("Harass key config").AddItem(new MenuItem("LaneClearHarass", "LaneClear Harass").SetValue(true));
            Config.SubMenu("Harass key config").AddItem(new MenuItem("LastHitHarass", "LastHit Harass").SetValue(true));
            Config.SubMenu("Harass key config").AddItem(new MenuItem("MixedHarass", "Mixed Harass").SetValue(true));

            //Config.Item("Qchange").GetValue<StringList>().SelectedIndex == 1
            //Config.Item("haras" + enemy.ChampionName).GetValue<bool>()
            //Config.Item("QmanaCombo").GetValue<Slider>().Value
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.ManaPercent < Config.Item("EmanaCombo").GetValue<Slider>().Value)
                return;

            if (E.LSIsReady())
            {
                var t = gapcloser.Sender;
                if (t.LSIsValidTarget(E.Range) && Config.Item("EGCchampion" + t.ChampionName).GetValue<bool>())
                {
                    if(Config.Item("EmodeGC").GetValue<StringList>().SelectedIndex == 0)
                        E.Cast(gapcloser.End);
                    else
                        E.Cast(Player.ServerPosition);
                }
            }
        }

        private static void BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {
            if (!FishBoneActive)
                return;

            if (Q.LSIsReady() && args.Target is AIHeroClient && Config.Item("Qchange").GetValue<StringList>().SelectedIndex == 1)
            {
                var t = (AIHeroClient)args.Target;
                if ( t.LSIsValidTarget())
                {
                    FishBoneToMiniGun(t);
                }
            }

            if (!Combo && args.Target is Obj_AI_Minion)
            {
                var t = (Obj_AI_Minion)args.Target;
                if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear && Player.ManaPercent > Config.Item("QmanaLC").GetValue<Slider>().Value && CountMinionsInRange(250, t.Position) >= Config.Item("Qlaneclear").GetValue<Slider>().Value)
                {
                    
                }
                else if (GetRealDistance(t) < GetRealPowPowRange(t))
                {
                    args.Process = false;
                    if (Q.LSIsReady())
                        Q.Cast();
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (sender.IsMinion)
                return;

            if (sender.IsMe)
            {
                if (args.SData.Name == "JinxWMissile")
                    WCastTime = Game.Time;
            }

            if (!E.LSIsReady() || !sender.IsEnemy || !Config.Item("Espell").GetValue<bool>() || Player.ManaPercent < Config.Item("EmanaCombo").GetValue<Slider>().Value || !sender.IsValid<AIHeroClient>() || !sender.LSIsValidTarget(E.Range) )
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                E.Cast(sender.Position);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            SetValues();

            if (Q.LSIsReady())
                Qlogic();
            if (W.LSIsReady())
                Wlogic();
            if (E.LSIsReady())
                Elogic();
            if (R.LSIsReady())
                Rlogic();
        }

        private static void Rlogic()
        {
            R.Range = Config.Item("RcustomeMax").GetValue<Slider>().Value;

            if (Config.Item("useR").GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    if(Config.Item("semiMode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        R.Cast(t);
                    }
                    else
                    {
                        R.CastIfWillHit(t, 2);
                        R.Cast(t, true, true);
                    }
                }   
            }

            if (Config.Item("Rks").GetValue<bool>())
            {
                bool cast = false;
                

                if (Config.Item("RoverAA").GetValue<bool>() && (!SebbyLib.Orbwalking.CanAttack() || Player.Spellbook.IsAutoAttacking))
                    return;

                foreach (var target in Enemies.Where(target => target.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(target) ))
                {
                    
                    float predictedHealth = target.Health + target.HPRegenRate * 2;

                    var Rdmg = R.GetDamage(target, 1);
                    if(Player.LSDistance(target.Position) < 1500)
                    {

                        Rdmg = Rdmg * (Player.LSDistance(target.Position) / 1500);
                       
                    }

                    if (Rdmg > predictedHealth)
                    {
                        cast = true;
                        PredictionOutput output = R.GetPrediction(target);
                        Vector2 direction = output.CastPosition.LSTo2D() - Player.Position.LSTo2D();
                        direction.Normalize();

                        foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget()))
                        {
                            if (enemy.NetworkId == target.NetworkId || !cast)
                                continue;
                            PredictionOutput prediction = R.GetPrediction(enemy);
                            Vector3 predictedPosition = prediction.CastPosition;
                            Vector3 v = output.CastPosition - Player.ServerPosition;
                            Vector3 w = predictedPosition - Player.ServerPosition;
                            double c1 = Vector3.Dot(w, v);
                            double c2 = Vector3.Dot(v, v);
                            double b = c1 / c2;
                            Vector3 pb = Player.ServerPosition + ((float)b * v);
                            float length = Vector3.Distance(predictedPosition, pb);
                            if (length < (R.Width + 150 + enemy.BoundingRadius / 2) && Player.LSDistance(predictedPosition) < Player.LSDistance(target.ServerPosition))
                                cast = false;
                        }

                        if (cast)
                        {
                            if (Config.Item("RoverW").GetValue<bool>() && target.LSIsValidTarget(W.Range) && W.GetDamage(target) > target.Health && W.Instance.Cooldown - (W.Instance.CooldownExpires - Game.Time) < 1.1)
                                return;

                            if (target.LSCountEnemiesInRange(400) > Config.Item("Raoe").GetValue<Slider>().Value)
                                CastSpell(R, target);

                            if (RValidRange(target) && target.LSCountAlliesInRange(Config.Item("Rover").GetValue<Slider>().Value) == 0)
                                CastSpell(R, target);
                        }
                    }
                }
            }
        }

        private static bool RValidRange(Obj_AI_Base t)
        {
            var range = GetRealDistance(t);

            if (Config.Item("Rmode").GetValue<StringList>().SelectedIndex == 0)
            {
                if (range > GetRealPowPowRange(t))
                    return true;
                else
                    return false;

            }
            else if (Config.Item("Rmode").GetValue<StringList>().SelectedIndex == 1)
            {
                if (range > Q.Range)
                    return true;
                else
                    return false;
            }
            else if (Config.Item("Rmode").GetValue<StringList>().SelectedIndex == 2)
            {
                if (range > Config.Item("Rcustome").GetValue<Slider>().Value && !SebbyLib.Orbwalking.InAutoAttackRange(t))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static void Elogic()
        {
            if (Player.ManaPercent < Config.Item("EmanaCombo").GetValue<Slider>().Value)
                return;

            if (blitz != null && blitz.LSDistance(Player.Position) < E.Range)
            {
                foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(2000) && enemy.LSHasBuff("RocketGrab")))
                {
                    E.Cast(blitz.Position.LSExtend(enemy.Position, 30));
                    return;
                }
            }

            foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) ))
            {

                E.CastIfWillHit(enemy, Config.Item("Eaoe").GetValue<Slider>().Value);

                if(Config.Item("Ecc").GetValue<bool>())
                {
                    if (!OktwCommon.CanMove(enemy))
                        E.Cast(enemy.Position);
                    E.CastIfHitchanceEquals(enemy, HitChance.Immobile);
                }

                if(enemy.MoveSpeed < 250 && Config.Item("Eslow").GetValue<bool>())
                    E.Cast(enemy);
                if (Config.Item("Edash").GetValue<bool>())
                    E.CastIfHitchanceEquals(enemy, HitChance.Dashing);
            }
            

            if (Config.Item("Etel").GetValue<bool>())
            {
                foreach (var Object in ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.IsEnemy && Obj.LSDistance(Player.ServerPosition) < E.Range && (Obj.LSHasBuff("teleport_target", true) || Obj.LSHasBuff("Pantheon_GrandSkyfall_Jump", true))))
                {
                    E.Cast(Object.Position);
                }
            }

            if (Combo && Player.IsMoving && Config.Item("Ecombo").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (t.LSIsValidTarget(E.Range) && E.GetPrediction(t).CastPosition.LSDistance(t.Position) > 200)
                {
                    if (Player.Position.LSDistance(t.ServerPosition) > Player.Position.LSDistance(t.Position))
                    {
                        if (t.Position.LSDistance(Player.ServerPosition) < t.Position.LSDistance(Player.Position))
                            CastSpell(E, t);
                    }
                    else
                    {
                        if (t.Position.LSDistance(Player.ServerPosition) > t.Position.LSDistance(Player.Position))
                            CastSpell(E, t);
                    }
                }
            }
        }

        private static bool WValidRange(Obj_AI_Base t)
        {
            var range = GetRealDistance(t);

            if (Config.Item("Wmode").GetValue<StringList>().SelectedIndex == 0)
            {
                if (range > GetRealPowPowRange(t) && Player.LSCountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                    return true;
                else
                    return false;

            }
            else if (Config.Item("Wmode").GetValue<StringList>().SelectedIndex == 1)
            {
                if (range > Q.Range + 50 && Player.LSCountEnemiesInRange(Q.Range + 50) == 0)
                    return true;
                else
                    return false;
            }
            else if (Config.Item("Wmode").GetValue<StringList>().SelectedIndex == 2)
            {
                if(range > Config.Item("Wcustome").GetValue<Slider>().Value && Player.LSCountEnemiesInRange(Config.Item("Wcustome").GetValue<Slider>().Value) == 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static void Wlogic()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.LSIsValidTarget() && WValidRange(t))
            {
                if (Config.Item("Wks").GetValue<bool>() && GetKsDamage(t, W) > t.Health && OktwCommon.ValidUlt(t))
                {
                    CastSpell(W, t);
                }

                if (Combo && Config.Item("Wcombo").GetValue<bool>() && Player.ManaPercent > Config.Item("WmanaCombo").GetValue<Slider>().Value)
                {
                    CastSpell(W, t);
                }
                else if (Farm && SebbyLib.Orbwalking.CanAttack() && !Player.Spellbook.IsAutoAttacking && Config.Item("Wharass").GetValue<bool>() && Player.ManaPercent > Config.Item("WmanaHarass").GetValue<Slider>().Value)
                {
                    if (Config.Item("Wts").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Config.Item("haras" + t.ChampionName).GetValue<bool>())
                            CastSpell(W, t);
                    }
                    else
                    {
                        foreach (var enemy in Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && WValidRange(t) && Config.Item("haras" + enemy.ChampionName).GetValue<bool>()))
                            CastSpell(W, enemy);
                    }
                }
                
            }
        }

        private static void Qlogic()
        {
            if (FishBoneActive)
            {
                var orbT = Orbwalker.GetTarget();
                if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear && Player.ManaPercent > Config.Item("QmanaLC").GetValue<Slider>().Value && orbT.IsValid<Obj_AI_Minion>())
                {
                    
                }
                else if (Config.Item("Qchange").GetValue<StringList>().SelectedIndex == 0 && orbT.IsValid<AIHeroClient>())
                {
                    var t = (AIHeroClient)Orbwalker.GetTarget();
                    FishBoneToMiniGun(t);
                }  
                else
                {
                    if (!Combo && Orbwalker.ActiveMode != SebbyLib.Orbwalking.OrbwalkingMode.None)
                        Q.Cast();
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(Q.Range + 40, TargetSelector.DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    if ((!SebbyLib.Orbwalking.InAutoAttackRange(t) || t.LSCountEnemiesInRange(250) >= Config.Item("Qaoe").GetValue<Slider>().Value))
                    {
                        if (Combo && Config.Item("Qcombo").GetValue<bool>() && (Player.ManaPercent > Config.Item("QmanaCombo").GetValue<Slider>().Value || Player.LSGetAutoAttackDamage(t) * Config.Item("QmanaIgnore").GetValue<Slider>().Value > t.Health))
                        {
                            Q.Cast();
                        }
                        if (Farm && SebbyLib.Orbwalking.CanAttack() && !Player.Spellbook.IsAutoAttacking && Config.Item("harasQ" + t.ChampionName).GetValue<bool>() && Config.Item("Qharass").GetValue<bool>() && (Player.ManaPercent > Config.Item("QmanaHarass").GetValue<Slider>().Value || Player.LSGetAutoAttackDamage(t) * Config.Item("QmanaIgnore").GetValue<Slider>().Value > t.Health))
                        {
                            Q.Cast();
                        }
                    }
                }
                else
                {
                    if (Combo && Player.ManaPercent > Config.Item("QmanaCombo").GetValue<Slider>().Value)
                    {
                        Q.Cast();
                    }
                    else if (Farm && !Player.Spellbook.IsAutoAttacking && Config.Item("farmQout").GetValue<bool>() && SebbyLib.Orbwalking.CanAttack())
                    {
                        foreach (var minion in MinionManager.GetMinions(Q.Range + 30).Where(
                        minion => !SebbyLib.Orbwalking.InAutoAttackRange(minion) && minion.Health < Player.LSGetAutoAttackDamage(minion) * 1.2 && GetRealPowPowRange(minion) < GetRealDistance(minion) && Q.Range < GetRealDistance(minion)))
                        {
                            Orbwalker.ForceTarget(minion);
                            Q.Cast();
                            return;
                        }
                    }
                    if(Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear && Player.ManaPercent > Config.Item("QmanaLC").GetValue<Slider>().Value)
                    {
                        var orbT = Orbwalker.GetTarget();
                        if (orbT.IsValid<Obj_AI_Minion>() && CountMinionsInRange(250, orbT.Position) >= Config.Item("Qlaneclear").GetValue<Slider>().Value)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private static int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = MinionManager.GetMinions(pos, range);
            int count = 0;
            foreach (var minion in minions)
            {
                count++;
            }
            return count;
        }

        public static float GetKsDamage(Obj_AI_Base t, Spell QWER)
        {
            var totalDmg = QWER.GetDamage(t);

            if (Player.LSHasBuff("summonerexhaust"))
                totalDmg = totalDmg * 0.6f;

            if (t.LSHasBuff("ferocioushowl"))
                totalDmg = totalDmg * 0.7f;

            if (t is AIHeroClient)
            {
                var champion = (AIHeroClient)t;
                if (champion.ChampionName == "Blitzcrank" && !champion.LSHasBuff("BlitzcrankManaBarrierCD") && !champion.LSHasBuff("ManaBarrier"))
                {
                    totalDmg -= champion.Mana / 2f;
                }
            }

            var extraHP = t.Health - SebbyLib.HealthPrediction.GetHealthPrediction(t, 500);

            totalDmg += extraHP;
            totalDmg -= t.HPRegenRate;
            totalDmg -= t.PercentLifeStealMod * 0.005f * t.FlatPhysicalDamageMod;

            return totalDmg;
        }

        private static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            if (Config.Item("PredictionMODE").GetValue<StringList>().SelectedIndex == 0)
            {
                if (QWER.Slot == SpellSlot.W)
                {
                    if (Config.Item("Wpred").GetValue<StringList>().SelectedIndex == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
                if (QWER.Slot == SpellSlot.R)
                {
                    if (Config.Item("Rpred").GetValue<StringList>().SelectedIndex == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
                if (QWER.Slot == SpellSlot.E)
                {
                    if (Config.Item("Epred").GetValue<StringList>().SelectedIndex == 0)
                        QWER.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    else
                        QWER.Cast(target);
                }
            }
            else
            {
                SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                var predInput2 = new SebbyLib.Prediction.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };

                var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if (QWER.Slot == SpellSlot.W)
                {
                    if (Config.Item("Wpred").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
                if (QWER.Slot == SpellSlot.R)
                {
                    if (Config.Item("Rpred").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
                if (QWER.Slot == SpellSlot.E)
                {
                    if (Config.Item("Epred").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                            QWER.Cast(poutput2.CastPosition);
                    }
                    else
                    {
                        if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                            QWER.Cast(poutput2.CastPosition);
                    }
                }
            }
        }

        private static void FishBoneToMiniGun(Obj_AI_Base t)
        {
            var realDistance = GetRealDistance(t);

            if(realDistance < GetRealPowPowRange(t) && t.LSCountEnemiesInRange(250) < Config.Item("Qaoe").GetValue<Slider>().Value)
            {
                if (Player.ManaPercent < Config.Item("QmanaCombo").GetValue<Slider>().Value || Player.LSGetAutoAttackDamage(t) * Config.Item("QmanaIgnore").GetValue<Slider>().Value < t.Health)
                    Q.Cast();

            }
        }

        private static float GetRealDistance(Obj_AI_Base target) { return Player.ServerPosition.LSDistance(target.ServerPosition) + Player.BoundingRadius + target.BoundingRadius; }

        private static float GetRealPowPowRange(GameObject target) { return 650f + Player.BoundingRadius + target.BoundingRadius; }

        private static void SetValues()
        {
            if (Config.Item("Wmode").GetValue<StringList>().SelectedIndex == 2)
                Config.Item("Wcustome").Show(true);
            else
                Config.Item("Wcustome").Show(false);

            if (Config.Item("Rmode").GetValue<StringList>().SelectedIndex == 2)
                Config.Item("Rcustome").Show(true);
            else
                Config.Item("Rcustome").Show(false);


            if (Player.LSHasBuff("JinxQ"))
                FishBoneActive = true;
            else
                FishBoneActive = false;

            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo)
                Combo = true;
            else
                Combo = false;

            if (
                (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear && Config.Item("LaneClearHarass").GetValue<bool>()) ||
                (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LastHit && Config.Item("LaneClearHarass").GetValue<bool>()) || 
                (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed && Config.Item("MixedHarass").GetValue<bool>())
               )
                Farm = true;
            else
                Farm = false;

            Q.Range = 685f + Player.BoundingRadius + 25f * Player.Spellbook.GetSpell(SpellSlot.Q).Level;

            QMANA = 10f;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;
            RMANA = R.Instance.SData.Mana;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange").GetValue<bool>())
            {
                if (FishBoneActive)
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, 590f + Player.BoundingRadius, System.Drawing.Color.Gray, 1, 1);
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range - 40, System.Drawing.Color.Gray, 1, 1);
            }
            if (Config.Item("wRange").GetValue<bool>())
            {
                if (Config.Item("onlyRdy").GetValue<bool>())
                {
                    if (W.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (Config.Item("eRange").GetValue<bool>())
            {
                if (Config.Item("onlyRdy").GetValue<bool>())
                {
                    if (E.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
