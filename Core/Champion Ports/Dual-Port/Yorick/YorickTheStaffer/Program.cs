using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using YorickTheStafferHpBarIndicator;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yorick
{
    class Program
    {
        //Declaration of Variables
        private static String championName = "Yorick";
        public static AIHeroClient Player;
        private static Menu Menu;
        private static Orbwalking.Orbwalker orbwalker;
        private static Spell Q, W, E, R;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private static bool MuramanaOn;
        private static bool allowMuramana;
        // Declaration Ends here XD
        //Skillshot List 
        private static string[] skillshots= new string []{"AhriSeduce",
            "BandageToss",
            "RocketGrab",
            "BrandBlaze",
            "BraumQ",
            "InfectedCleaverMissileCast",
            "EliseHumanE",
            "JayceQAccel",
            "JinxW",
            "KalistaMysticShot",
            "KarmaQ",
            "KarmaQMantra",
            "LeblancSoulShackle",
            "LeblancSoulShackleM",
            "BlindMonkQOne",
            "LuxLightBinding",
            "DarkBindingMissile",
            "NautilusAnchorDrag",
            "JavelinToss",
            "RengarE",
            "RyzeQ",
            "TahmKenchQ",
            "ThreshQ",
            "XerathMageSpear"
            };

        public static List<YorickTheStaffer.Evade.Skillshot> DetectedSkillshots = new List<YorickTheStaffer.Evade.Skillshot>();


        public static void Main()
        {
           
            Game_OnGameLoad();
            Orbwalking.AfterAttack += AfterAttack;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnCastSpell += OnSpell;
            Obj_AI_Base.OnSpellCast += DoCast;

        }

        static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != championName)
            {
                return;
            }
            Menu = new Menu ("YorickTheStaffer", "yorick", true);
            TargetSelector.AddToMenu(Menu);
            orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));          
           
            //Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            YorickTheStaffer.Evade.SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;



            //I create very good menu shit that greatly increase script performance XD
            var comboMenu = new Menu("Combo", "yorick.combo");
            {
                comboMenu.AddItem(new MenuItem("yorick.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("yorick.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("yorick.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("yorick.combo.user", "Use R").SetValue(true));
            }
            Menu.AddSubMenu(comboMenu);
            var harassMenu = new Menu("Harass", "yorick.harass");
            {
                harassMenu.AddItem(new MenuItem("yorick.harass.useq", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("yorick.harass.usee", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("yorick.harass.usew", "Use W").SetValue(true));
                harassMenu.AddItem(new MenuItem("yorick.harass.dontberetardpls", "This has good harass logic dont stop it from using spell lol pls"));
                harassMenu.AddItem(new MenuItem("yorick.harass.manalimit", "Mana Limit for Harass").SetValue(new Slider(30, 0, 100)));
            }
            Menu.AddSubMenu(harassMenu);
            var laneClearMenu = new Menu("Lane Shove", "yorick.laneclear");
            {
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.useq", "Use Q")).SetValue(true);
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.usew", "Use W")).SetValue(true);
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.usee", "Use E")).SetValue(true);
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.lasthit", "Attempt to Lasthit with spells prior to clearing")).SetValue(true);
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.shove", "Shove the lane like a professional?")).SetValue(true);
                laneClearMenu.AddItem(new MenuItem("yorick.laneclear.manalimit","Mana Limit for Laneclear").SetValue(new Slider(80,0,100)));
                
            }
            Menu.AddSubMenu(laneClearMenu);
            var lasthitMenu = new Menu("Lasthit", "yorick.lasthit");
            {
                lasthitMenu.AddItem(new MenuItem("yorick.lasthit.lasthitw", "Use W to lasthit").SetValue(true));
                lasthitMenu.AddItem(new MenuItem("yorick.lasthit.lasthite", "Use E to lasthit").SetValue(true));
                lasthitMenu.AddItem(new MenuItem("yorick.lasthit.manalimit", "Mana Limit for Lasthit").SetValue(new Slider(0, 0, 100)));
                //Soon TM
                // lasthitMenu.AddItem(new MenuItem("yorick.lasthit.priority", "Use E where possilbe").SetValue(true));
            }
            Menu.AddSubMenu(lasthitMenu);
            var aacancelMenu = new Menu("Auto Attack Cancel Method", "yorick.aacancel");
            { 
                aacancelMenu.AddItem(new MenuItem("yorick.aacancel.logic", "Chose your AA cancel Logic for Q XD").SetValue(new Slider(1, 1, 2)));
                aacancelMenu.AddItem(new MenuItem("yorick.aacancel.explain", "1 is On Attack, 2 is Hoola, Hoola is recommended"));
            }
            Menu.AddSubMenu(aacancelMenu);
            
            var ultimateMenu = new Menu("Ultimate", "yorick.ultimate");
            {
                ultimateMenu.AddItem(new MenuItem("yorick.ultimate.useonself", "Use R to safe you").SetValue(true));
                ultimateMenu.AddItem(new MenuItem("yorick.ultimate.notrust", "Dont let the Assembly use R").SetValue(true));
                ultimateMenu.AddItem(new MenuItem("yorick.ultimate.adprio", "Prioritize ADC").SetValue(true));
            }
            Menu.AddSubMenu(ultimateMenu);
            var goldmanMenu = new Menu("Goldman Features", "yorick.goldman");
            {
           
                goldmanMenu.AddItem(new MenuItem("yorick.goldman.manamune", "Use Manamune like professional XD "));
                goldmanMenu.AddItem(new MenuItem("yorick.goldman.cancerxd", "Use W to make enemy consider suicide XD").SetValue(new KeyBind("g".ToCharArray()[0], KeyBindType.Press)));
                goldmanMenu.AddItem(new MenuItem("yorick.goldman.flee", "Never get caught again").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                goldmanMenu.AddItem(new MenuItem("yorick.goldman.dive", "Dive with W XD").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                
            }
            Menu.AddSubMenu(goldmanMenu);
            Menu.AddToMainMenu();
            //Luckily this is end of cancer menu declaration XD

            //I declare Spells now cuz Spells do damage
            Q = new Spell(SpellSlot.Q, 125, TargetSelector.DamageType.Physical);
            W = new Spell(SpellSlot.W, 600, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 550, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 900);
            
            W.SetSkillshot(W.Instance.SData.SpellCastTime, W.Instance.SData.LineWidth, W.Speed, false, SkillshotType.SkillshotCircle);
            //I am done declaring spells and it was surprisingly easy XD

            // subscribe to Drawing event
            Drawing.OnDraw += Drawing_OnDraw;

            // subscribe to Update event gets called every game update around 10ms
            Game.OnUpdate += Game_OnGameUpdate;

            // Creates a notification in the right-upper corner of the game (String message, int Milliseconds)
            Notifications.AddNotification("Lets have fun with Yorick the Staffer", 5000);
            // A metric fuckton of things have happened so far XD
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (R.IsReady() && Player.Mana >= R.ManaCost)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red, 10);
            }
        }

     

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo();
            }

            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if ((Player.ManaPercent > Menu.Item("yorick.harass.manalimit").GetValue<Slider>().Value))
                {
                    harass();
                }
                
            }

            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                if (Player.ManaPercent > Menu.Item("yorick.lasthit.manalimit").GetValue<Slider>().Value)
                {
                    lasthit();
                }
                
            }
          
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Player.ManaPercent > Menu.Item("yorick.laneclear.manalimit").GetValue<Slider>().Value)
                { 
                    laneclear();
                }
            }

            if (Menu.Item("yorick.goldman.flee").GetValue<KeyBind>().Active)
            {
                flee();
                
            }

            if (Menu.Item("yorick.goldman.dive").GetValue<KeyBind>().Active)
            {
                
               
                 castWForDive();
            }
            if (Menu.Item("yorick.goldman.cancerxd").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (R.IsReady() == true  && Player.Mana>= R.ManaCost && Player.CountEnemiesInRange(1200) > 0 && Menu.Item("yorick.ultimate.notrust").GetValue<bool>()== false)
            {
                UltTarget();
            }
            checkMuramana();
            
        }

        private static void flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var closestEnemy = Player.GetEnemiesInRange(1200).FirstOrDefault();
            SharpDX.Vector3 enemypos = closestEnemy.Position;
            switch (fleeCaseAnalysis(closestEnemy))
            {
                case"slownleech":
                    if (W.IsReady()) W.Cast(Player.Position.Extend(enemypos,400));
                    if (E.IsReady()) E.Cast(closestEnemy);
                    
                    break;
                case "slowonly":
                    if (W.IsReady()) W.Cast(Player.Position.Extend(enemypos, 400));
                    
                    break;
                case "qforspeedup":
                    if (Q.IsReady()) Q.Cast();
                    
                    break;
                case "drowning":
                    
                    break;
            }
        }

        private static string fleeCaseAnalysis(AIHeroClient closestEnemy)
        {
            if (Player.Position.Distance(closestEnemy.Position)-(Player.BoundingRadius+closestEnemy.AttackRange)>300)
            {
                if (E.CanCast(closestEnemy)) return "slownleech"; 
                return "slowonly";
            }
            if (Player.Position.Distance(closestEnemy.Position) < Player.AttackRange + closestEnemy.BoundingRadius)
            {
                return "qforspeedup";
            }
            else return "drowning";
           
        
        }

        private static void laneclear()
        {

            jungle();
         
          
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 500, MinionTypes.All, MinionTeam.Enemy);
            var rangedMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 500, MinionTypes.Ranged,MinionTeam.Enemy);
            if (allMinions != null)
            {   if (Menu.Item("yorick.laneclear.lasthit").GetValue<bool>()== true){
                lasthit();
            }
                if (Menu.Item("yorick.laneclear.shove").GetValue<bool>() == true)
                {
                   
                    if (rangedMinions.ToArray().Length >= 1 && W.IsReady() && Menu.Item("yorick.laneclear.usew").GetValue<bool>() == true)
                    {
                        var rangedPos = W.GetCircularFarmLocation(rangedMinions);
                        W.Cast(rangedPos.Position);
                    }
                    if (rangedMinions.ToArray().Length == 1 && E.IsReady() && Menu.Item("yorick.laneclear.usee").GetValue<bool>() == true)
                    {

                        E.Cast(rangedMinions[0]);
                    }
                    if (rangedMinions.ToArray().Length == 0 && W.IsReady() && allMinions.ToArray().Length >= 1 && Menu.Item("yorick.laneclear.usew").GetValue<bool>() == true)
                    {
                        var meleePos = W.GetCircularFarmLocation(allMinions);
                        W.Cast(meleePos.Position);
                    }
                    if (allMinions.ToArray().Length == 1)
                    {
                        E.Cast(allMinions[0]);
                    }
                    if (allMinions.ToArray().Length == 0)
                    {
                        return;
                    }

                }
                if (Menu.Item("yorick.laneclear.shove").GetValue<bool>() == false)
                {
                    if (Menu.Item("yorick.laneclear.lasthit").GetValue<bool>() == true)
                    {
                        lasthit();
                    }
                    if (Menu.Item("yorick.laneclear.usew").GetValue<bool>() == true && W.IsReady())
                    {
                        var retardpushpos = W.GetCircularFarmLocation(allMinions);
                        W.Cast(retardpushpos.Position);
                    }


                }
            }
            

        }
        //adding jungleclear 
        private static void jungle()
        {
            var jungleminion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (jungleminion == null) return;
            var wjungleminions = MinionManager.GetMinions(Player.ServerPosition,500, MinionTypes.All, MinionTeam.Neutral);
            var wcastposition = W.GetCircularFarmLocation(wjungleminions);
            if (W.IsReady() && Player.Mana> E.ManaCost+Q.ManaCost+W.ManaCost)          
            W.Cast(jungleminion.Position);
            if (E.IsReady()&& E.ManaCost < Player.Mana)
            E.Cast(jungleminion);
        }
        //start of divehelper logic
        private static float minionTankCheck (Obj_AI_Turret enemytower)
        {
            float timetanked = 0;
            if (enemytower == null) {return timetanked; }
            var allyminionsundertower = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Distance(enemytower)<900 && !x.IsDead && x.IsAlly);
            if (allyminionsundertower == null) return timetanked;
            
            foreach (var minion in allyminionsundertower)
            {
                float shotssurvived=(float) (minion.Health / enemytower.GetAutoAttackDamage(minion));
                if (shotssurvived < 1) shotssurvived = 1;
                timetanked +=shotssurvived*enemytower.AttackSpeedMod;
            }
            float timetankedXD = timetanked;
            timetanked = 0;
            return timetankedXD;
            
        }
        //Used for fleemode
        private static bool minionsUnderTower(Obj_AI_Turret enemytower)
        {
            if (enemytower.Spellbook.IsAutoAttacking && !enemytower.IsAttackingPlayer) return true;
            else return false;
        }

        private static void castWForDive()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var enemytower = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.Distance(Player.Position) < 1100 && x.IsEnemy && x.Health> 0).FirstOrDefault();
            if (!W.IsReady() || W.ManaCost > Player.Mana)   return;
            if (minionTankCheck(enemytower) < 2.5)
            {
                W.Cast(Player.Position.Extend(enemytower.Position, 500));
            }
            else return;            
        }
        //end of divehelper logic

        //start of muramana logic 
        //Useless atm but will be obscenely useful soon TM
        private static double dmgPerMana()
        {
            var targetm = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical, true, HeroManager.Enemies.Where<AIHeroClient>(x=> x.Distance(Player)<1000));
            var targetp = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where<AIHeroClient>(x => x.Distance(Player) < 1000));
            double dmgCombo = Q.GetDamage(targetp)+W.GetDamage(targetm)+E.GetDamage(targetm);
            double manafloor = Q.ManaCost * 3 + W.ManaCost * 2 + E.ManaCost * 2;
            var muramanadmgpermana = MuramanaDmg(0, Player.Mana, ref manafloor) / Player.Mana - manafloor;
            return 0;
        }


       //Recursive method make script 300% more performant and ensure smoothest insec and wardjump around                                                                                                                                                      I am fully aware that this baby here could have been solved by just taking the difference between manaretain and player.Mana and multiplying by 0.06 but recursive methods increase awesomeness of the script and I can put it in the topic to meme/use for marketing. Also this will be used in a secret SoonTm Feature XD
        static double MuramanaDmg(double dmg, double mana, ref double manafloor)
        {
            mana -= mana * 0.03;
            dmg += mana * 0.06;
            if (mana <= manafloor)
            {
                return mana;
            }
            return MuramanaDmg(dmg, mana, ref manafloor);
        }
        //end of muramana logic XD
        private static void Game_OnGameDraw(EventArgs args)
        {
           
        }

        private static void combo()
        {
            
          
          
           omenOfPestilence();
           omenOfFamine();
            
           
        }

        private static void harass()
        {
            //Full damage Harass slows the player before applying the ls ghoul
            if (Player.ManaPercent>= 70) { 
            if(Menu.Item("yorick.harass.usew").GetValue<bool>().Equals(true) && W.IsReady() == true)
            {
                omenOfPestilence();
            }
                if (Menu.Item("yorick.harass.usee").GetValue<bool>().Equals(true) && E.IsReady() == true)
                {
                    omenOfFamine();
                }

                //Sustain oriented harass ensures you use mana to heal back up instead of blowing it on dmg u cant use cuz low 
                //Later I will add Logic Checks to see if you can all in soon tm kappa 
                else 
                omenOfFamine();
        }



    }
        //I can add freeze option to this but it will not work  well also yoricks ghouls dont really allow for freezing lmao XD 
          
        private static void lasthit()
        {
            var enemyminions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(ObjectManager.Player) < 600 && x.IsMinion);
            var lasthitbye = enemyminions.Where(x => x.Health <= E.GetDamage(x) && x.Distance(ObjectManager.Player) < 550 && x.Distance(ObjectManager.Player) > Player.AttackRange + x.BoundingRadius);
            var lasthitbyw = enemyminions.Where(x => x.Health <= W.GetDamage(x) && x.Distance(ObjectManager.Player) < 600 && x.Distance(ObjectManager.Player) > Player.AttackRange + x.BoundingRadius);
            if (E.IsReady()== true && Menu.Item("yorick.lasthit.lasthite").GetValue<bool>()== true && lasthitbye.ToArray().Length>0)
            {
                foreach(var minion in lasthitbye)
                {
                    E.CastOnUnit(minion);
                    break;                 
                             
                }
            }


            if (W.IsReady() == true && Menu.Item("yorick.lasthit.lasthitw").GetValue<bool>() == true && lasthitbyw.ToArray().Length > 0)
            {
                foreach (var minion in lasthitbyw)
                {
                    W.Cast(minion);
                    break;

                }
            }
            else return;
        }
        //This make nice q aa reset for insane dmg kappa
        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
        if (Menu.Item("yorick.aacancel.logic").GetValue<Slider>().Value == 1) { 
         if (Player.Mana > Q.ManaCost)
            {
               if (unit.IsMe && Q.IsReady() && ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target is AIHeroClient)) )
                {
                  //  ToggleMuramana();
                    Q.Cast();
                  //  ToggleMuramana();
                    
                }
                if (unit.IsMe && Q.IsReady() && ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))&& target is AIHeroClient && Menu.Item("yorick.harass.useq").GetValue<bool>()==true)
                {
                  //  ToggleMuramana();
                    Q.Cast();
                  //  ToggleMuramana();

                }
                if (unit.IsMe && Q.IsReady()&& ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)) && Menu.Item("yorick.laneclear.useq").GetValue<bool>() == true)
                    {
                   // ToggleMuramana();
                    Q.Cast();
                    //ToggleMuramana();
                }
            }
                 if(Q.IsReady() == false || Player.Mana<Q.ManaCost && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                 {
                //ToggleMuramana();
                CastTitanicHydra();
                   
                //ToggleMuramana();
                 }
            }
        }

        private static void OnDetectSkillshot(YorickTheStaffer.Evade.Skillshot skillshot)
        {
            if (skillshot.Unit.Team == Player.Team) return;
            if (!W.IsReady() && !E.IsReady() ||!Menu.Item("yorick.goldman.cancerxd").GetValue<KeyBind>().Active) return;
            var Blockchamps = HeroManager.Allies.Where<AIHeroClient>(x => x.Distance(Player) < 600 - x.BoundingRadius -20);
            foreach (var champ in Blockchamps)
            {

                if (skillshots.Contains(skillshot.SpellData.SpellName)&& skillshot.IsDanger(champ.Position.To2D()))
                {
                    castEForSS(champ, skillshot);
                    break;
                }                               
            }  
        }

        private static void castWForSS(AIHeroClient champ, YorickTheStaffer.Evade.Skillshot skillshot)
        {
                W.Cast(champ.Position.Extend(skillshot.Start.To3D(), champ.BoundingRadius)+20);      
        }

        private static void castEForSS(AIHeroClient champ, YorickTheStaffer.Evade.Skillshot skillshot)
        {
            var enemyminions = ObjectManager.Get<Obj_AI_Minion>().Where(x => !x.IsAlly && x.Health > 40 && x.IsValidTarget(550, false, Player.Position));
            foreach (var minion in enemyminions)
            {
                if (skillshot.IsDanger(Player.Position.To2D().Extend(minion.Position.To2D(), Player.Distance(minion.Position)+ 50)))
                {
                    E.Cast(minion);
                    return;
                }
            }
            castWForSS(champ, skillshot);
        }


        private static void DoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Menu.Item("yorick.aacancel.logic").GetValue<Slider>().Value == 2)
            {
                if (!sender.IsMe || !args.SData.IsAutoAttack()) return;
                if (args.Target is Obj_AI_Base)
                {
                    var target = (Obj_AI_Base)args.Target;
                    if (!target.IsValidTarget(300 + Player.BoundingRadius) || target == null) return;
                    if (target is AIHeroClient && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (HasTitan()) CastTitan();
                        else if (!HasTitan())
                            if (Q.IsReady()) Q.Cast();
                            
                        
                    }
                    if (target is AIHeroClient && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (HasTitan()) CastTitan();
                        else if (!HasTitan()) if (Q.IsReady()) Q.Cast();

                    }
                    if (target is Obj_AI_Base && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Menu.Item("yorick.laneclear.useq").GetValue<bool>() == true)
                    {
                        if (HasTitan()) CastTitan();
                        else if (!HasTitan()) if (Q.IsReady()) Q.Cast();

                    }
                }

            }
        }
        //writing this gave me tons of cancer

            private static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                Orbwalking.LastAATick = 0;
            }
        }

        private static void CastTitanicHydra()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
            }
        }
        
        private static void checkMuramana()
        {
                if (Player.Mana > 400 && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    allowMuramana = true;
                ToggleMuramana();
                }
                else allowMuramana = false;
                ToggleMuramana();
        }

        private static void ToggleMuramana()
        {
            if (Items.HasItem(3042)&& Items.CanUseItem(3042))
            {
                
                if (MuramanaOn == false && allowMuramana == true)
                {
                    Items.UseItem(3042);
                    MuramanaOn = true;
                    return;
                }

                if (allowMuramana == false && MuramanaOn == true)
                {
                    Items.UseItem(3042);
                    MuramanaOn = false;
                    return;
                }
            }
            
        }
       
        //Titanic Hydra AA Reset
        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "ItemTitanicHydraCleave") Orbwalking.LastAATick = 0;
        }

        //W casting
        private static void omenOfPestilence()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (W.IsReady() == true)
            {
                W.CastOnUnit(target);
            }
        }
        //E casting
        private static void omenOfFamine()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range,TargetSelector.DamageType.Magical);
            if (E.IsReady() == true)
            {
                E.Cast(target);
            }
            
        }
        //R casting not use lmao
        private static void omenOfDeath()
        {

        }

        private static bool HasTitan() => (Items.HasItem(3748) && Items.CanUseItem(3748));

        private static void CastTitan()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
            }
        }

        // approximate max damage over ult lifetime aylmao not used atm and highly inaccurate XD
        private static double ultComboCheck()
        {
            var dmg = Q.GetDamage(TargetSelector.GetSelectedTarget())+ 4* Player.GetAutoAttackDamage(TargetSelector.GetSelectedTarget())+W.GetDamage(TargetSelector.GetSelectedTarget())+E.GetDamage(TargetSelector.GetSelectedTarget());
            
            return dmg;
        }
        //Thx BestTuks and Hoola
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>()
                .Where(ene => ene.IsValidTarget()))
            {
                Indicator.unit = enemy;
                Indicator.drawDmg(getComboDamage(enemy), SharpDX.Color.Aqua);
            }
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (Items.HasItem(3078))
                {
                    damage +=(float) (Player.GetAutoAttackDamage(enemy)+ 2f* Player.BaseAttackDamage*enemy.PercentPhysicalReduction)*1.10f*2f;
                }
                if (Items.HasItem(3057))
                {
                    damage += (float) (Player.GetAutoAttackDamage(enemy) + Player.BaseAttackDamage * enemy.PercentPhysicalReduction)*1.10f*2f;
                }
                damage +=(float) Player.GetAutoAttackDamage(enemy)*1.10f;
                if (Q.IsReady()) damage += (Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy, true))*1.15f;
                if (E.IsReady()) damage += E.GetDamage(enemy);
                if (W.IsReady()) damage += W.GetDamage(enemy);
                if (Items.HasItem(3748) && Items.CanUseItem(3748))
                    damage += (float)Player.GetAutoAttackDamage(enemy)+(Player.MaxHealth * 0.1f + 40)*1.15f*enemy.PercentPhysicalReduction;

                return damage;
            }
            return 0;
        }

        private static void UltTarget()
        {
            var nearbyfriends = ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && x.IsChampion() && !x.IsDead);
            string[] heronames = new string[5];
            double[] AD = new double[5];
            double[] AP = new double[5];
            foreach (var hero in nearbyfriends)
            {
                int i = 0;
                
                AD[i] = hero.AttackSpeedMod * hero.TotalAttackDamage * (hero.PercentCritDamageMod * hero.FlatCritChanceMod);
                AP[i] = hero.AbilityPower();
            }
         
            foreach (var hero in nearbyfriends.Where(x => x.Distance(Player) < 900))
            {
                if (hero.AttackSpeedMod * hero.TotalAttackDamage * (hero.PercentCritDamageMod * hero.FlatCritChanceMod) == AD.Max()&& Menu.Item("yorick.ultimate.adprio").GetValue<bool>()== true && hero.CountEnemiesInRange(1200) > 0 )
                {
                    if (hero.Health < hero.MaxHealth * 0.2)
                    {
                        R.Cast(hero);
                        break;
                    }
                   
                }
                if (hero.AbilityPower() == AP.Max() && hero.CountEnemiesInRange(1200)>0)
                {
                    if (hero.Health < hero.MaxHealth * 0.2)
                    {
                        R.Cast(hero);
                    }
                }
            }
        }
    }


}
