using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using SharpDX;
using SMouse = SmoothMouse.SmoothMouse;

using Color = System.Drawing.Color;

//// Half of this stolen from Hellsing's code :* \\
/* 
 * TODO:
 * Objective steal
 * Проверка всех интерраптов и гепклозеров ///ПРОВЕРИТЬ - Проверено, соу фар :D
 * Игнайт
*/
 
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ZiggsRoyal
{
    class Program
    {
        static readonly AIHeroClient player = ObjectManager.Player;  //Player object
        static Spell Q1, Q2, Q3, W, E, R;                           //Spells
        static bool DOTReady, igniteCheck = false;  //Ignite, has ignite, does W exist
        enum beingFocusedModes { NONE, TURRET, CHAMPION };        //Being focused by
        enum escapeModes { TOMOUSE, FROMTURRET }       //Escape to mouse, escape away from turret
        enum WModes { NONE, INTERRUPTOR, ANTIGAPCLOSER, ESCAPE, COMBAT }            //Modes of second W cast
        static beingFocusedModes beingFocusedBy;
        static escapeModes escapeMode;
        static WModes Wmode;
        static Vector3 escapePos,  wPos;                      //Position to escape from focus, TurretUnitPosition, Explosive( W ) position
        static TUnit TUnit_obj = new TUnit();
        static string wObj = "ZiggsW_mis_ground.troy";              //Well, W object, as is
        public static Menu menu;                                           //Menu! (@_@ )
        static Orbwalking.Orbwalker SOW;                            //SOW! (^_^ )
        static List<Spell> SpellList = new List<Spell>();
        static Items.Item DFG = new Items.Item(3128, 750);          //DFG!!!!!!
        static List<FEnemy> lastTimePinged = new List<FEnemy>();
		static SpellSlot IgniteSlot = player.GetSpellSlot("SummonerDot");
        static float lastQ = 0f;
        static bool CRB = false;
        //(?)List of damage sources to calc
        private static readonly List<Tuple<SpellSlot, int>> mainCombo = new List<Tuple<SpellSlot, int>>();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;                // OnGameLoad callback
        }
        /// <summary>
        /// OnGameLoad callback. Executes on loading game
        /// </summary>
        /// <param name="args"></param>
        static void Game_OnGameLoad(EventArgs args)
        {
            if (player.ChampionName != "Ziggs") return;                   //Champion validation
            //Spell init
            Q1 = new Spell(SpellSlot.Q, 850);
            Q2 = new Spell(SpellSlot.Q, 1125);
            Q3 = new Spell(SpellSlot.Q, 1400);
            W  = new Spell(SpellSlot.W, 970);
            E  = new Spell(SpellSlot.E, 870);
            R  = new Spell(SpellSlot.R, 5300);
            //SetSkillshot(float delay, float width, float speed, bool collision, SkillshotType type, Vector3 from = null, Vector3 rangeCheckFrom = null);
            Q1.SetSkillshot(0.3f,            130, 1700, false , SkillshotType.SkillshotCircle);
            Q2.SetSkillshot(0.25f + Q1.Delay,130, 1700, false, SkillshotType.SkillshotCircle);
            Q3.SetSkillshot(0.30f + Q2.Delay,130, 1700, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.250f,           275, 1800, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1.000f,           235, 2700, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1.014f,           525, 1750, false, SkillshotType.SkillshotCircle);
            SpellList.AddRange(new[] { Q3, W, E });
            //Ignite
            var ignite = player.Spellbook.GetSpell(player.GetSpellSlot("SummonerDot"));
            
            //Combo settings
            mainCombo.Add(Tuple.Create(SpellSlot.Q, 0));
            mainCombo.Add(Tuple.Create(SpellSlot.W, 0));
            mainCombo.Add(Tuple.Create(SpellSlot.E, 0));
            mainCombo.Add(Tuple.Create(SpellSlot.R, 0));
            //Menu loading
            LoadMenu();
            //Presets
            Wmode = WModes.NONE;
            try
            {
                foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
                    if (hero.IsEnemy) lastTimePinged.Add(new FEnemy(hero));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Additional callbacks
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter_OnPossibleToInterrupt;
            GameObject.OnCreate += GO_OnCreate;
            GameObject.OnDelete += GO_OnRemove;
            Game.OnProcessPacket += OnRecievePacket;
            Orbwalking.BeforeAttack += (a) =>
                {
                    if (menu.Item("streamMouseOrb").GetValue<bool>())
                        SMouse.addMouseEvent(a.Target.Position, false, CRB);
                };
            SMouse.start();
            Chat.Print("Royal Ziggs loaded!");
        }

        static void OnRecievePacket(GamePacketEventArgs args)
        {
            if (PacketChannel.S2C == args.Channel && args.PacketData[0] == Packet.S2C.TowerAggro.Header)//Header checks
            {
                Packet.S2C.TowerAggro.Struct temp = Packet.S2C.TowerAggro.Decoded(args.PacketData);//Decode packet
                if (temp.TargetNetworkId != player.NetworkId) return;
                TUnit_obj.Position = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)temp.TurretNetworkId).Position;//Get turret position
                TUnit_obj.LastAggroTime = Game.Time;
                TUnit_obj.isAggred = true;
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            // Combo
            if (menu.SubMenu("combo").Item("Active").GetValue<KeyBind>().Active)
                Combo();

            // Harass
            if (menu.SubMenu("harass").Item("Active").GetValue<KeyBind>().Active)
                Harass();

            // Wave clear
            if (menu.SubMenu("waveClear").Item("Active").GetValue<KeyBind>().Active)
                LaneClear();

            if (menu.SubMenu("misc").Item("MAKEMETHEHELLOUTTAHEREMAN").GetValue<KeyBind>().Active)
                if (TUnit_obj.isAggred)
                    Escape(escapeModes.FROMTURRET);
                else
                    Escape(escapeModes.TOMOUSE);
            if (Game.Time - 8 > TUnit_obj.LastAggroTime) TUnit_obj.isAggred = false;//Ye, my awful english.
            WExploder();
            CRB = menu.Item("streamMouseCR").GetValue<bool>();
        }
        static void GO_OnCreate(EloBuddy.GameObject GO, EventArgs args)
        {
            if (GO.Name == wObj)
            {
                wPos = GO.Position;
            }
        }
        static void GO_OnRemove(EloBuddy.GameObject GO, EventArgs args)
        {
            if (GO.Name == wObj)
            {
                wPos = default(Vector3);
                Wmode = WModes.NONE;
            }
        }
        static void Drawing_OnDraw(EventArgs args)
        {
            //Draw the ranges of the spells. Ty Honda, stolen from your code :P
            foreach (var spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "range").GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(player.Position, spell.Range, menuItem.Color);
            }
			string additional = "";
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsEnemy && hero.IsVisible && hero.IsTargetable && player.GetSpellDamage(hero, SpellSlot.R) > hero.Health && !hero.IsDead && R.IsReady())
                {
				additional += hero.BaseSkinName + " ";
                    foreach(FEnemy enemy in lastTimePinged)
                        if (enemy.NetworkId == hero.NetworkId)
                        {
                            Drawing.DrawText(Drawing.Width * 0.7f, Drawing.Height * 0.5f, System.Drawing.Color.GreenYellow, "Ult can kill - "+additional);
                            if (enemy.LastAggroTime < Game.Time - 7)
                            {
                                Console.WriteLine("KS ping executed");
                                //Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(hero.Position.X, hero.Position.Y, hero.NetworkId, 0, Packet.PingType.Fallback)).Process();
                                enemy.LastAggroTime = Game.Time;
                            }
                        }
                }
            }
        }
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && menu.SubMenu("misc").Item("antigapcloser").GetValue<bool>())
            {
                //if (gapcloser.SkillType == GapcloserType.Skillshot)
                    W.Cast(gapcloser.End, true); //TODO: разные интеррапты для лисинов\джарванов\леон
                    SMouse.addMouseEvent(gapcloser.End, false, CRB);
                //else//Проверить работоспособность на разных гепклозерах
                   // W.Cast(gapcloser.End);//СДЕЛАТЬ, БЛЯДЬ!
                Wmode = WModes.ANTIGAPCLOSER;
            }
        }
        /// <summary>
        /// Interruptor
        /// </summary>
        /// <param name="unit">Unit that causing interruptable spell</param>
        /// <param name="spell">Spell that can be interrupted</param>
        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (unit.IsAlly) return;
            if (W.IsReady() &&
                Vector3.Distance(player.Position, unit.Position) <= -1 + W.Range + W.Width / 2 &&
                menu.SubMenu("misc").Item("interrupt").GetValue<bool>() &&
                spell.DangerLevel >= Interrupter2.DangerLevel.Medium)
            {
                Vector3 pos = V3E(player.Position, unit.Position, -1 + Vector3.Distance(player.Position, unit.Position) - W.Width/2);
                W.Cast(pos, true);
                SMouse.addMouseEvent(pos, false, CRB);
                Wmode = WModes.INTERRUPTOR;
            }
        }
        /// <summary>
        /// Comboing
        /// </summary>
        static void Combo()
        {
            Console.WriteLine("Combo");
            bool useQ = Q1.IsReady() && menu.SubMenu("combo").Item("UseQ").GetValue<bool>();
            bool useW =  W.IsReady() && menu.SubMenu("combo").Item("UseW").GetValue<bool>();
            bool useE =  E.IsReady() && menu.SubMenu("combo").Item("UseE").GetValue<bool>();
            bool useR =  R.IsReady() && (menu.SubMenu("combo").Item("UseR").GetValue<bool>() || menu.SubMenu("ulti").Item("forceR").GetValue<KeyBind>().Active);
            AIHeroClient targetQ = TargetSelector.GetTarget(Q3.Range, TargetSelector.DamageType.Magical);
            AIHeroClient targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            AIHeroClient targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            AIHeroClient targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (targetW.IsValid && CalculateDamage(targetW) > targetW.Health)
            {
                if (IgniteSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && menu.Item("UseIgnite").GetValue<bool>())
                    player.Spellbook.CastSpell(IgniteSlot, targetW);
                SMouse.addMouseEvent(targetW.Position, false, CRB);
                /*
				if(DFG.IsReady())
                    DFG.Cast(targetW);*/
            }

            if (useW)
            {
                PredictionOutput prediction = W.GetPrediction(targetW);
                if (menu.SubMenu("misc").Item("GETOVERHERE").GetValue<bool>() && (prediction.Hitchance >= HitChance.Medium))
                {
                    Vector3 pos = V3E(player.Position, prediction.CastPosition, Vector3.Distance(player.Position, prediction.CastPosition) + 30);
                    if (Vector3.Distance(player.Position, prediction.CastPosition) >=
                        Vector3.Distance(player.Position, targetW.Position))
                    {
                        W.Cast(pos);
                        SMouse.addMouseEvent(pos, false, CRB);
                        Wmode = WModes.COMBAT;
                    }
                    else
                    {
                        //pos = V3E(player.Position, prediction.CastPosition, Vector3.Distance(player.Position, prediction.CastPosition));
                        W.Cast(targetW.Position);
                        SMouse.addMouseEvent(targetW.Position, false, CRB);
                        Wmode = WModes.COMBAT;
                    }
                }
                else
                {
                    Vector3 pos = V3E(player.Position, targetW.Position, Vector3.Distance(player.Position, prediction.CastPosition) - 10);
                    W.Cast(pos);
                    SMouse.addMouseEvent(pos, false, CRB);
                    Wmode = WModes.COMBAT;
                }
            }
            if (useQ)
            {
                CastQ(targetQ);
                SMouse.addMouseEvent(V3E(player.Position, targetQ.Position, Vector3.Distance(player.Position, targetQ.Position) * 0.5f), false, CRB);
            }
            if (useE)
            {
                PredictionOutput prediction = E.GetPrediction(targetE);
                Vector3 pos = V3E(player.Position, prediction.CastPosition,
                        Vector3.Distance(player.Position, prediction.CastPosition) + 30);
                if (prediction.Hitchance >= HitChance.Medium)
                {
                    E.Cast(pos);
                    SMouse.addMouseEvent(pos, false, CRB);
                }
            }
            if (useR)//TEST IT!
            {
                //Honda's way to predict ulti
                R.Delay = 1900 + 1500 * targetR.Distance(player.Position) / 5300;
                PredictionOutput prediction = R.GetPrediction(targetR);
                if ((menu.SubMenu("ulti").Item("ultiOnKillable").GetValue<bool>() && (player.GetSpellDamage(targetR, SpellSlot.R, 0) > targetR.Health && !(CalculateDamage(targetR, true) > targetR.Health) && Vector3.Distance(player.Position, targetR.Position) < W.Range && lastQ + 3 < Game.Time) || menu.SubMenu("ulti").Item("forceR").GetValue<KeyBind>().Active))
                    if (prediction.Hitchance >= HitChance.Medium || menu.SubMenu("ulti").Item("forceRPrediction").GetValue<bool>())
                    {
                        R.Cast(prediction.CastPosition);
                        SMouse.addMouseEvent(prediction.CastPosition, false, CRB);
                    }

                if (!menu.SubMenu("ulti").Item("AOE").GetValue<bool>()) return;
                {
                    if (R.CastIfWillHit(targetR, menu.SubMenu("ulti").Item("enemiesToHit").GetValue<Slider>().Value))
                        SMouse.addMouseEvent(targetR.Position, false, CRB);
                }
            }
        }
        /// <summary>
        /// Harass
        /// </summary>
        static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q3.Range, TargetSelector.DamageType.Magical);
            if (menu.SubMenu("harass").Item("UseQ").GetValue<bool>() && Q1.IsReady())
            {
                CastQ(target);
                SMouse.addMouseEvent(target.Position, false, CRB);
            }
            target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (menu.SubMenu("harass").Item("UseE").GetValue<bool>() && E.IsReady())
            {
                E.Cast(target);
                SMouse.addMouseEvent(target.Position, false, CRB);
            }
        }
        /// <summary>
        /// Farming function
        /// </summary>
        static void LaneClear()
        {
            //Minions
            List<Obj_AI_Base> minions = MinionManager.GetMinions(player.Position, Q3.Range);
            //Farm locations for spells
            MinionManager.FarmLocation QPos = Q3.GetCircularFarmLocation(minions);
            //MinionManager.FarmLocation WPos = W.GetCircularFarmLocation(minions);
            MinionManager.FarmLocation EPos = E.GetCircularFarmLocation(minions);
            //Minons count
            int numToHit = menu.SubMenu("waveClear").Item("waveNum").GetValue<Slider>().Value;
            //Using of spells
            bool useQ = menu.SubMenu("waveClear").Item("UseQ").GetValue<bool>() && Q1.IsReady();
            //bool useW = menu.SubMenu("waveClear").Item("UseW").GetValue<bool>();
            bool useE = menu.SubMenu("waveClear").Item("UseE").GetValue<bool>() && E.IsReady();
            //Casts
            if (useQ && QPos.MinionsHit >= numToHit)
            {
                Q1.Cast(QPos.Position, true);
                SMouse.addMouseEvent(QPos.Position.To3D(), false, CRB);
            }
            //if (false && WPos.MinionsHit >= numToHit) W.Cast(WPos.Position, true);
            if (useE && EPos.MinionsHit >= numToHit)
            {
                E.Cast(EPos.Position, true);
                SMouse.addMouseEvent(EPos.Position.To3D(), false, CRB);
            }
        }
        static void Escape(escapeModes mode)
        {
            switch (mode)
            {
                case escapeModes.TOMOUSE: //Escaping to mouse
                    {
                        Vector3 cursorPos = Game.CursorPos;
                        Vector3 pos = V3E(player.Position, cursorPos, (float)-15);
                        if (!IsPassWall(player.Position, cursorPos))//Escaping to mouse pos
                        {
                            Vector3 pass = V3E(player.Position, cursorPos, 100);//Point to move closer to the wall (could be better, i know, i'll improve it)
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(pass.X, pass.Y, 0));
                            SMouse.addMouseEvent(new Vector3(pass.X, pass.Y, 0), true, false);
                        }
                        else //Escape through the wall (Flash fail)
                        {
                            Vector3 jumpPred = V3E(pos, cursorPos, 700);
                            if (IsWall(jumpPred.To2D()) && IsPassWall(player.Position, jumpPred))//Can't we jump over?
                            {
                                Vector3 pass = V3E(player.Position, jumpPred, 100);//Point to move closer to the wall (could be better, i know, i'll improve it)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(pass.X, pass.Y, 0));//Move closer to the wall
                                SMouse.addMouseEvent(new Vector3(pass.X, pass.Y, 0), true, false);
                                return;
                            }
                            else //Yes! We can!
                            { //Stand still
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(player.Position.X, player.Position.Y, 0));
                            }
                        }
                        if (!W.IsReady()) return;//Simple!
                        W.Cast(pos);//Poof! W cast!
                        SMouse.addMouseEvent(pos, false, CRB);
                        Wmode = WModes.ESCAPE;//WMode
                    }
                    break;
                case escapeModes.FROMTURRET:
                    {
                        Vector3 WPos = V3E(player.Position, TUnit_obj.Position, 40);//Positions
                        Vector3 escapePos = V3E(player.Position, TUnit_obj.Position, -450);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(escapePos.X, escapePos.Y, 0));//Move away
                        if (!W.IsReady()) return;//Simple!
                        W.Cast(WPos);//Cast W to move away
                        SMouse.addMouseEvent(WPos, false, CRB);
                        //TUnit_obj.isAggred = false;//Turrent isn't focusing us anymore
                        Wmode = WModes.ESCAPE;//???PROFIT
                    }
                    break;
            }
        }
        /// <summary>
        /// Menu creation
        /// </summary>
        static void LoadMenu()
        {
            // Initialize the menu
            menu = new Menu("Royal Ziggy", "Ziggs", true);

            // Target selector
            Menu targetSelector = new Menu("Target Selector", "ts");
            TargetSelector.AddToMenu(targetSelector);
            menu.AddSubMenu(targetSelector);

            // Orbwalker
            Menu orbwalker = new Menu("Orbwalker", "orbwalker");
            SOW = new Orbwalking.Orbwalker(orbwalker);
            menu.AddSubMenu(orbwalker);

            // Combo
            Menu combo = new Menu("Combo", "combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("UseIgnite", "Ignite in combo").SetValue(true));
            combo.AddItem(new MenuItem("Active", "Combo key").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press)));

            // Harass
            Menu harass = new Menu("Harass", "harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("UseE", "Use E").SetValue(false));
            harass.AddItem(new MenuItem("Active", "Harass key").SetValue<KeyBind>(new KeyBind('C', KeyBindType.Press)));

            // Wave clear
            Menu waveClear = new Menu("Lane clear", "waveClear");
            menu.AddSubMenu(waveClear);
            waveClear.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            //waveClear.AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            waveClear.AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            waveClear.AddItem(new MenuItem("waveNum", "Minions to hit").SetValue<Slider>(new Slider(3, 1, 10)));
            waveClear.AddItem(new MenuItem("Active", "Lane clear key").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press)));

            // Drawings
            Menu misc = new Menu("Misc", "misc");
            menu.AddSubMenu(misc);
            misc.AddItem(new MenuItem("interrupt", "Interrupt spells").SetValue(true));
            //misc.AddItem(new MenuItem("interruptLevel", "Interrupt only with danger level").SetValue<InterruptableDangerLevel>(InterruptableDangerLevel.Medium));
            misc.AddItem(new MenuItem("antigapcloser", "Anti-Gapscloser").SetValue(true));
            misc.AddItem(new MenuItem("GETOVERHERE", "Try to throw enemy closer in combo").SetValue(true));
            misc.AddItem(new MenuItem("streamMouse", "Stream: smooth mouse").SetValue(false));
            misc.AddItem(new MenuItem("streamMouseCR", "Stream: put cursor back").SetValue(false));
            misc.AddItem(new MenuItem("streamMouseOrb", "Stream: orbwalk").SetValue(true));
            misc.AddItem(new MenuItem("MAKEMETHEHELLOUTTAHEREMAN", "Escape: Smart W").SetValue<KeyBind>(new KeyBind('G', KeyBindType.Press)));

            //Ultimate settings
            Menu ulti = new Menu("Ultimate", "ulti");
            menu.AddSubMenu(ulti);
            ulti.AddItem(new MenuItem("123", "            ----There is no auto ulti----"));
            ulti.AddItem(new MenuItem("forceR", "Force ulti cast(combo mode)").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Press)));
            ulti.AddItem(new MenuItem("forceRPrediction", "Cast with any hitchance").SetValue(true));
            ulti.AddItem(new MenuItem("ultiOnKillable", "Ulti on killable(may KS)").SetValue(true));
            ulti.AddItem(new MenuItem("AOE", "Cast into crowd").SetValue(true));
            ulti.AddItem(new MenuItem("enemiesToHit", "Enemies to hit").SetValue<Slider>(new Slider(3, 1, 5)));

            //Stolen from Honda7's code, cause i'm lazy fuck ( -_-)
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after a rotation").SetValue<Circle>(new Circle(true, Color.Aqua));
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit += hero => (float)CalculateDamage(hero);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<Circle>().Active;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Color = dmgAfterComboItem.GetValue<Circle>().Color;
            dmgAfterComboItem.ValueChanged += (object sender, OnValueChangeEventArgs eventArgs) =>
                {
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<Circle>().Active;
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Color = eventArgs.GetNewValue<Circle>().Color;
                };
            // Drawings
            Menu drawings = new Menu("Drawings", "drawings");
            menu.AddSubMenu(drawings);
            drawings.AddItem(new MenuItem("Qrange", "Q Range").SetValue<Circle>(new Circle(true, Color.FromArgb(150, Color.IndianRed))));
            drawings.AddItem(new MenuItem("Wrange", "W Range").SetValue<Circle>(new Circle(true, Color.FromArgb(150, Color.IndianRed))));
            drawings.AddItem(new MenuItem("Erange", "E Range").SetValue<Circle>(new Circle(false, Color.FromArgb(150, Color.DarkRed))));
            drawings.AddItem(dmgAfterComboItem);

            // Finalize menu
            menu.AddToMainMenu();
            Console.WriteLine("Menu finalized");
        }
        /*
        /// <summary>
        /// Is spells on cooldown
        /// </summary>
        /// <param name="spells"></param>
        /// <returns></returns>
        private static bool isCooldown(List<Spell> spells)
        {
            foreach(Spell spell in spells)
            {
                if (!spell.IsReady()) return true;
            }
            return false;
        }
        */
        /// <summary>
        /// Get Vector3 position in direction by distance
        /// </summary>
        /// <param name="from">Start point</param>
        /// <param name="direction">Direction of vector(End point)</param>
        /// <param name="distance">Distance</param>
        /// <returns>Vector3</returns>
        static Vector3 V3E(Vector3 from, Vector3 direction, float distance)
        {
            return (from.To2D() + distance * Vector2.Normalize(direction.To2D() - from.To2D())).To3D();
        }
        /// <summary>
        /// Is hit wall in this direcion :D Similar version of "collision wall"
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 10)
            {
                Vector2 pos = V3E(start, end, i).To2D();
                if (IsWall(pos)) return true;
            }
            return false;
        }
        /// <summary>
        /// Is current point a wall
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        static bool IsWall(Vector2 pos)
        {
            return (NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Wall || NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Building);
        }
        /// <summary>
        /// No explanation
        /// </summary>
        static void WExploder()
        {
            if (Wmode != WModes.NONE && wPos != default(Vector3))
            {
                W.Cast();
            }
        }

        //And it's all stolen too :C
        static void CastQ(Obj_AI_Base target)
        {
            PredictionOutput prediction;

            if (Vector3.Distance(player.Position, target.Position) < Q1.Range)
            {
                var oldrange = Q1.Range;
                Q1.Range = Q2.Range;
                prediction = Q1.GetPrediction(target, true);
                Q1.Range = oldrange;
            }
            else if (Vector3.Distance(player.Position, target.Position) < Q2.Range)
            {
                var oldrange = Q2.Range;
                Q2.Range = Q3.Range;
                prediction = Q2.GetPrediction(target, true);
                Q2.Range = oldrange;
            }
            else if (Vector3.Distance(player.Position, target.Position) < Q3.Range)
            {
                prediction = Q3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <= Q1.Range + Q1.Width)
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

                    Q1.Cast(p);
                    SMouse.addMouseEvent(p, false, CRB);
                    lastQ = Game.Time;
                }
                else if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <=
                         ((Q1.Range + Q2.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), Q1.Range - 100);

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                        SMouse.addMouseEvent(p.To3D(), false, CRB);
                        lastQ = Game.Time;
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            Q1.Range *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
                                ();

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                        SMouse.addMouseEvent(p.To3D(), false, CRB);
                        lastQ = Game.Time;
                    }
                }
            }
        }

        static bool CheckQCollision(Obj_AI_Base target, Vector3 targetPosition, Vector3 castPosition)
        {
            var direction = (castPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized();
            var firstBouncePosition = castPosition.To2D();
            var secondBouncePosition = firstBouncePosition +
                                       direction * 0.4f *
                                       ObjectManager.Player.ServerPosition.To2D().Distance(firstBouncePosition);
            var thirdBouncePosition = secondBouncePosition +
                                      direction * 0.6f * firstBouncePosition.Distance(secondBouncePosition);

            //TODO: Check for wall collision.

            if (thirdBouncePosition.Distance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the second one.
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.IsValidTarget(3000))
                    {
                        var predictedPos = Q2.GetPrediction(minion);
                        if (predictedPos.UnitPosition.To2D().Distance(secondBouncePosition) <
                            Q2.Width + minion.BoundingRadius)
                        {
                            return true;
                        }
                    }
                }
            }

            if (secondBouncePosition.Distance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius ||
                thirdBouncePosition.Distance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the first one
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.IsValidTarget(3000))
                    {
                        var predictedPos = Q1.GetPrediction(minion);
                        if (predictedPos.UnitPosition.To2D().Distance(firstBouncePosition) <
                            Q1.Width + minion.BoundingRadius)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }
        static double CalculateDamage(AIHeroClient target, bool notR = false)
        {
            double total = 0;
            if (player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready) total += player.GetSpellDamage(target, SpellSlot.Q);
            if (player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready) total += player.GetSpellDamage(target, SpellSlot.W);
            if (player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready) total += player.GetSpellDamage(target, SpellSlot.E);
            if (player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready && !notR) total += player.GetSpellDamage(target, SpellSlot.R);
            if (player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && menu.Item("UseIgnite").GetValue<bool>()) 
                total += player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (DFG.IsReady()) total += player.GetItemDamage(target, Damage.DamageItems.Dfg);
            return total;
        }
    }
    class TUnit
    {
        public Vector3 Position = new Vector3();
        public bool isAggred = false;
        public float LastAggroTime = 0f;//10 секунд тайминг
    }
    class FEnemy
    {
        public int NetworkId = 0;
        public float LastAggroTime = 0f;//10 секунд тайминг
        public FEnemy(AIHeroClient hero)
        {
            LastAggroTime = 0f;
            NetworkId = hero.NetworkId;
        }
    }   
}
