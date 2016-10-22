using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LadyGragas
{
    internal class Program
    {
        public const string ChampName = "Gragas";
        public static HpBarIndicator Hpi = new HpBarIndicator();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Vector3 insecpos;
        public static Vector3 eqpos;
        public static Vector3 movingawaypos;
        public static GameObject Barrel;
        public static SpellSlot Ignite;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != ChampName)
                return;
            Notifications.AddNotification("LadyGragas - [V2.1]", 8000);

            Q = new Spell(SpellSlot.Q, 775);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 675);
            R = new Spell(SpellSlot.R, 1100);

            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.0f, 50, 1000, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);


            Config = new Menu("Lady Gragas", "Gragas", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Gragas: Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Gragas: Target Selector", "Target Selector")));

            //COMBOMENU

            var combo = Config.AddSubMenu(new Menu("Gragas: Combo Settings", "Combo Settings"));
            combo.AddItem(new MenuItem("UseQ", "Use Q - Barell Roll").SetValue(true));
            combo.AddItem(new MenuItem("autoQ", "Auto Detonate Q").SetValue(true));
            combo.AddItem(new MenuItem("UseW", "Use W - Drunken Rage ").SetValue(true));
            combo.AddItem(new MenuItem("UseE", "Use E - Body Slam").SetValue(true));
            combo.AddItem(new MenuItem("UseR", "Use R - Explosive Cask | FINISHER").SetValue(true));
            combo.AddItem(new MenuItem("UseRprotector", "Use R - Explosive Cask | PROTECTOR").SetValue(false));
            combo.AddItem(new MenuItem("Rhp", "Own HP%").SetValue(new Slider(35, 100, 0)));
            combo.AddItem(new MenuItem("UseRdmg", "Use R - Explosive Cask | AOE DMG").SetValue(false));
            combo.AddItem(new MenuItem("rdmgslider", "Enemy Count").SetValue(new Slider(3, 5, 1)));
            combo.AddItem(new MenuItem("InsecMode", "Insec Mode - Leftclick on InsecTarget").SetValue(new KeyBind('K', KeyBindType.Press)));



            //HARASSMENU

            Config.SubMenu("Gragas: Harass Settings")
                .AddItem(new MenuItem("harassQ", "Use Q - Barell Roll").SetValue(true));
            Config.SubMenu("Gragas: Harass Settings")
                .AddItem(new MenuItem("harassE", "Use E - Bodyslam").SetValue(true));
            Config.SubMenu("Gragas: Harass Settings")
                .AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            //LANECLEARMENU
            Config.SubMenu("Gragas: Laneclear Settings")
                .AddItem(new MenuItem("laneQ", "Use Q - Barell Roll").SetValue(true));
            Config.SubMenu("Gragas: Laneclear Settings")
                .AddItem(new MenuItem("jungleW", "Use W - Drunken Rage").SetValue(true));
            Config.SubMenu("Gragas: Laneclear Settings")
                .AddItem(new MenuItem("laneE", "Use E - Bodyslam").SetValue(true));
            Config.SubMenu("Gragas: Laneclear Settings")
                .AddItem(new MenuItem("laneclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            //JUNGLEFARMMENU
            Config.SubMenu("Gragas: Jungle Settings")
                .AddItem(new MenuItem("jungleQ", "Use Q - Barell Roll").SetValue(true));
            Config.SubMenu("Gragas: Jungle Settings")
                .AddItem(new MenuItem("jungleW", "Use W - Drunken Rage").SetValue(true));
            Config.SubMenu("Gragas: Jungle Settings")
                .AddItem(new MenuItem("jungleE", "Use E - Bodyslam").SetValue(true));
            Config.SubMenu("Gragas: Jungle Settings")
                .AddItem(new MenuItem("jungleclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            //DRAWINGMENU
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Draw Insec Position", "Draw Insec Position").SetValue(true));
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));         
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(new Circle(true, Color.Orange)));
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(new Circle(true, Color.DarkOrange)));
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
            Config.SubMenu("Gragas: Draw Settings")
                .AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(new Circle(true, Color.LawnGreen)));
            Config.SubMenu("Gragas: Draw Settings").AddItem(new MenuItem("Rrdy", "Draw R - Status").SetValue(true));

            //MISCMENU
            var killsteal = Config.AddSubMenu(new Menu("Gragas: Killsteal Settings", "Killsteal Settings"));
            killsteal.AddItem(new MenuItem("SmartKS", "Use SmartKS").SetValue(true));
            killsteal.AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));
            killsteal.AddItem(new MenuItem("KSQ", "Use Q").SetValue(true));
            killsteal.AddItem(new MenuItem("KSE", "Use E").SetValue(true));
            killsteal.AddItem(new MenuItem("RKS", "Use R").SetValue(true));

            Config.SubMenu("Gragas: Misc Settings").AddItem(new MenuItem("DrawD", "Damage Indicator").SetValue(true));
            Config.SubMenu("Gragas: Misc Settings")
                .AddItem(new MenuItem("AntiGapE", "Use E on Gapclosers").SetValue(true));
            Config.SubMenu("Gragas: Misc Settings")
                .AddItem(new MenuItem("AntiGapR", "Use R on Gapclosers").SetValue(false));
            Config.SubMenu("Gragas: Misc Settings")
                .AddItem(new MenuItem("EInterrupt", "Use E to Interrupt Spells").SetValue(true));
            Config.SubMenu("Gragas: Misc Settings")
                .AddItem(new MenuItem("RInterrupt", "Use R to Interrupt Spells").SetValue(false));

            Config.AddToMainMenu();

            //Idk what this is called but it's something <3

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            GameObject.OnCreate += GragasObject;
            GameObject.OnDelete += GragasBarrelNull;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            Drawing.OnDraw += etcdraw;
        }
        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }


        private static void etcdraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var starget = TargetSelector.GetSelectedTarget();
            var epos = Drawing.WorldToScreen(starget.Position);

            if (Config.Item("Draw Insec Position").GetValue<bool>() && R.IsReady() && starget.IsValidTarget(R.Range) && R.Level > 0)

            Drawing.DrawText(epos.X, epos.Y, Color.DarkSeaGreen, "Insec Target");
            if (Config.Item("Draw Insec Position").GetValue<bool>() && R.IsReady() && starget.IsValidTarget(R.Range) && R.Level > 0)
            Render.Circle.DrawCircle(target.Position, 150, Color.LightSeaGreen);
            if (Config.Item("Draw Insec Position").GetValue<bool>() && R.IsReady() && starget.IsValidTarget(R.Range) && R.Level > 0)
            insecpos = player.Position.Extend(target.Position, player.Distance(target) + 150);
            Render.Circle.DrawCircle(insecpos, 100, Color.GreenYellow);
        }

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Config.Item("AntiGapE").GetValue<bool>() && E.GetPrediction(gapcloser.Sender).Hitchance >= HitChance.High)
                E.Cast(gapcloser.Sender);
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && Config.Item("AntiGapR").GetValue<bool>())
                R.Cast(gapcloser.Sender);

        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && sender.IsValidTarget(R.Range) && Config.Item("interrupt").GetValue<bool>())
                R.CastIfHitchanceEquals(sender, HitChance.High);
            if (E.IsReady() && sender.IsValidTarget(E.Range) && Config.Item("interrupt").GetValue<bool>())
                E.CastIfHitchanceEquals(sender, HitChance.High);       
        }

        private static void GragasBarrelNull(GameObject sender, EventArgs args) //BARREL LOCATION - GONE
        {
            {
            }

            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Barrel = null;
            }

        }

        private static void Killsteal()
        {
            {

                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.IsValidTarget(R.Range))
                        .Where(x => !x.IsZombie)
                        .Where(x => !x.IsDead))
                {
                    Ignite = player.GetSpellSlot("summonerdot");
                    var edmg = E.GetDamage(enemy);
                    var qdmg = Q.GetDamage(enemy);
                    var rdmg = R.GetDamage(enemy);
                    var rpred = R.GetPrediction(enemy);
                    var qpred = Q.GetPrediction(enemy);
                    var epred = E.GetPrediction(enemy);




                    if (enemy.Health < edmg && E.IsReady() && epred.Hitchance >= HitChance.VeryHigh)
                        E.Cast(epred.CastPosition, Config.Item("packetcast").GetValue<bool>());

                    if (enemy.Health < qdmg && qpred.Hitchance >= HitChance.VeryHigh &&
                        Q.IsReady() &&
                        Config.Item("KSQ").GetValue<bool>())

                        Q.Cast(enemy, Config.Item("packetcast").GetValue<bool>());

                    if (enemy.Health < rdmg && rpred.Hitchance >= HitChance.VeryHigh &&
                        !Q.IsReady() &&
                        Config.Item("KSR").GetValue<bool>())

                        R.Cast(enemy, Config.Item("packetcast").GetValue<bool>());

                    if (player.Distance(enemy.Position) <= 600 && IgniteDamage(enemy) >= enemy.Health &&
                        Config.Item("UseIgnite").GetValue<bool>() && R.IsReady() && Ignite.IsReady())
                        player.Spellbook.CastSpell(Ignite, enemy);
                }
            }
        }

        public static bool Exploded { get; set; }

        public static void InsecCombo()
        {
            var target = TargetSelector.GetSelectedTarget();
            Orbwalking.Orbwalk(null, Game.CursorPos);

            eqpos = player.Position.Extend(target.Position, player.Distance(target));
            insecpos = player.Position.Extend(target.Position, player.Distance(target) + 200);
            movingawaypos = player.Position.Extend(target.Position, player.Distance(target) + 300);
            eqpos = player.Position.Extend(target.Position, player.Distance(target) + 100);

            if (target.IsFacing(player) == false &&
                target.IsMoving & (R.IsInRange(insecpos) && target.Distance(insecpos) < 300))
                R.Cast(movingawaypos);

            if (R.IsInRange(insecpos) && target.Distance(insecpos) < 300 && target.IsFacing(player) && target.IsMoving)
                R.Cast(eqpos);

            else if (R.IsInRange(insecpos) && target.Distance(insecpos) < 300)
                R.Cast(insecpos);

            if (!Exploded) return;

            var prediction = E.GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                E.Cast(target.ServerPosition);
                Q.Cast(target.ServerPosition);
            
                    
                
            }
        }


        private static
            void GragasObject(GameObject sender, EventArgs args) //BARREL LOCATION
        {
            if (sender.Name == "Gragas_Base_R_End.troy")
            {
                Exploded = true;
                LeagueSharp.Common.Utility.DelayAction.Add(3000, () => { Exploded = false; });
            }
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Barrel = sender;



            }
        }
    
    

            private static bool IsWall(Vector3 pos)
        {
            CollisionFlags cFlags = NavMesh.GetCollisionFlags(pos);
            return (cFlags == CollisionFlags.Wall);
        }





            private static
            void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                LaneClear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                eLogic();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Overkill();

            if (Config.Item("SmartKS").GetValue<bool>())
            {
                Killsteal();
            }

            if (Config.Item("InsecMode").GetValue<KeyBind>().Active)
                InsecCombo();

            if (Barrel.Position.CountEnemiesInRange(275) >= 1)
                Q.Cast();
 

        }

        private static void OnDraw(EventArgs args)
        {
            //Draw Skill Cooldown on Champ
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (R.IsReady() && Config.Item("Rrdy").GetValue<bool>())
            {
                Drawing.DrawText(pos.X, pos.Y, Color.Gold, "R is Ready!");
            }

            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            foreach (var tar in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(2000)))
            {
            }

            if (Config.Item("Qdraw").GetValue<Circle>().Active)
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Config.Item("Qdraw").GetValue<Circle>().Color : Color.Red);


            if (Config.Item("Wdraw").GetValue<Circle>().Active)
                if (W.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? Config.Item("Wdraw").GetValue<Circle>().Color : Color.Red);

            if (Config.Item("Edraw").GetValue<Circle>().Active)
                if (E.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range - 1,
                        E.IsReady() ? Config.Item("Edraw").GetValue<Circle>().Color : Color.Red);

            if (Config.Item("Rdraw").GetValue<Circle>().Active)
                if (R.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range - 2,
                        R.IsReady() ? Config.Item("Rdraw").GetValue<Circle>().Color : Color.Red);
        }

        private static void OnEndScene(EventArgs args)
        {
            {
                //Damage Indicator
                if (Config.SubMenu("Gragas: Misc Settings").Item("DrawD").GetValue<bool>())
                {
                    foreach (var enemy in
                        ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                    {
                        Hpi.unit = enemy;
                        Hpi.drawDmg(CalcDamage(enemy), Color.Gold);
                    }
                }
            }
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            //Calculate Combo Damage

            var aa = player.GetAutoAttackDamage(target, true);
            var damage = aa;

            if (Config.Item("UseE").GetValue<bool>()) // edamage
            {
                if (E.IsReady())
                {
                    damage += E.GetDamage(target);
                }

                if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
                {
                    damage += W.GetDamage(target);
                }
            }

            if (R.IsReady() && Config.Item("UseR").GetValue<bool>()) // rdamage
            {
                damage += R.GetDamage(target);
            }

            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>())
            {
                damage += Q.GetDamage(target);
            }
            return (int) damage;
        }


        private static int overkill(Obj_AI_Base target)
        {
            //overkill protection m8;
            var aa = player.GetAutoAttackDamage(target, true);
            var damage = aa;

            if (Ignite != SpellSlot.Unknown &&
                player.Spellbook.CanUseSpell(Ignite) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (Config.Item("UseE").GetValue<bool>()) // edamage
            {
                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    damage += E.GetDamage(target);
                }
                if (W.IsReady() && target.IsValidTarget(E.Range))
                {
                    damage += W.GetDamage(target) + aa;
                }

                if (Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(E.Range + 100) && Q.IsReady())
                {
                    damage += Q.GetDamage(target)*1;
                }
                return (int) damage;
            }
            return 0;
        }

        private static void Qcast()
        {
            
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);



            var prediction = Q.GetPrediction(target);

            if (prediction.Hitchance >= HitChance.High 
                && Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && Barrel == null)
                Q.Cast(target);



            if (R.IsReady() && Config.Item("UseRprotector").GetValue<bool>() && player.HealthPercentage() <= Config.Item("Rhp").GetValue<Slider>().Value)
                R.Cast(player.Position);

            if (R.IsReady() && Config.Item("UseRdmg").GetValue<bool>() &&
                target.Position.CountEnemiesInRange(250) >= Config.Item("rdmgslider").GetValue<Slider>().Value)

                R.Cast(target);

        }

        private static void eLogic()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            var prediction = E.GetPrediction(target);
            if (E.IsReady() && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(player)))
                E.Cast(target);
            else if (W.IsReady() && target.IsValidTarget(E.Range) && Config.Item("UseW").GetValue<bool>())
                W.Cast();
            if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
                return;
            if (player.HasBuff("GragasWAttackBuff"))
                E.Cast(target);
            else if (E.IsReady() && target.IsValidTarget(E.Range))
                E.Cast(target);


            if (player.HasBuff("GragasWAttackBuff") && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(player)))
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
        }

        private static void Overkill()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
  
            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && target.IsValidTarget(R.Range) &&
                     R.GetDamage(target) >= target.Health && overkill(target) <= target.Health) 

                R.Cast(target.ServerPosition);
        }

        private static void Harass()
        {

            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qpred = Q.GetPrediction(t);
            var epred = E.GetPrediction(t);
            if (E.IsReady() && Config.Item("harassE").GetValue<bool>() && t.IsValidTarget(E.Range) &&
                player.ManaPercentage() >= harassmana && epred.Hitchance >= HitChance.High)
                E.Cast(t);
            {
                if (Q.IsReady() && Config.Item("harassQ").GetValue<bool>() && t.IsValidTarget(Q.Range) &&
                    player.ManaPercentage() >= harassmana && qpred.Hitchance >= HitChance.High)
                    Q.Cast(t);
            }
        }

        private static void LaneClear()
        {
            var lanemana = Config.Item("laneclearmana").GetValue<Slider>().Value;
            var junglemana = Config.Item("jungleclearmana").GetValue<Slider>().Value;
            var jungleQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var jungleE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var laneE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width);
            var laneQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width);

            var Qjunglepos = Q.GetCircularFarmLocation(jungleQ, Q.Width);
            var Ejunglepos = E.GetLineFarmLocation(jungleE, E.Width);

            var Qfarmpos = Q.GetCircularFarmLocation(laneQ, Q.Width);
            var Efarmpos = E.GetLineFarmLocation(laneE, E.Width);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Qjunglepos.MinionsHit >= 1 &&
                Config.Item("jungleQ").GetValue<bool>()
                && player.ManaPercentage() >= junglemana)
            {
                Q.Cast(Qjunglepos.Position);
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Q.Cast(Qjunglepos.Position));
            }
            foreach (var minion in jungleE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range) &&
                    Ejunglepos.MinionsHit >= 1 && jungleE.Count >= 1 && Config.Item("jungleE").GetValue<bool>()
                    && player.ManaPercentage() >= junglemana)
                {
                    E.Cast(minion.Position);
                }
                foreach (var minion in jungleE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range) &&
                    Ejunglepos.MinionsHit >= 1 && jungleE.Count >= 1 && Config.Item("jungleE").GetValue<bool>()
                    && player.ManaPercentage() >= junglemana)
                {
                    W.Cast(player);
                }
            {
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Qfarmpos.MinionsHit >= 3 &&
                Config.Item("laneQ").GetValue<bool>()
                && player.ManaPercentage() >= lanemana)
            {
                Q.Cast(Qfarmpos.Position);
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Q.Cast(Qfarmpos.Position));
            }

            foreach (var minion in laneE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range) &&
                    Efarmpos.MinionsHit >= 2 && laneE.Count >= 2 && Config.Item("laneE").GetValue<bool>()
                    && player.ManaPercentage() >= lanemana)
                {
                    E.Cast(minion.Position);
                }
            foreach (var minion in laneE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range) &&
                    Efarmpos.MinionsHit >= 1 && laneE.Count >= 1 && Config.Item("laneE").GetValue<bool>()
                    && player.ManaPercentage() >= lanemana)
                {
                    W.Cast(player);
                }
        }
    }
}