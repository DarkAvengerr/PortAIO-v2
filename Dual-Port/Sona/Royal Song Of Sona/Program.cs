using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RoyalSona
{
    class Program
    {
        private static readonly AIHeroClient player = ObjectManager.Player;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker SOW;
        private static Menu menu;
        private static bool packets { get { return menu.Item("packets").GetValue<bool>(); } }
        private static List<BuffType> CcTypes = new List<BuffType> { BuffType.Fear, BuffType.Polymorph, BuffType.Snare, BuffType.Stun, BuffType.Suppression, BuffType.Taunt, BuffType.Charm, BuffType.Blind };
        

        public static void Game_OnGameLoad()
        {
            if (player.ChampionName != "Sona") return;

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 1000);

            R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            LoadMenu();
            //Game.OnGameSendPacket += OnSendPacket;
            Game.OnUpdate += Game_OnGameUpdate;
            //AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            Drawing.OnDraw += OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Chat.Print("RoyalSongOfSona loaded!");
        }

        static void OnDraw(EventArgs args)
        {
            //Hardcoding
            if (menu.Item("DrawQ").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(player.Position, menu.Item("QRange").GetValue<Slider>().Value, menu.Item("DrawQ").GetValue<Circle>().Color);
            if (menu.Item("DrawW").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(player.Position, W.Range, menu.Item("DrawW").GetValue<Circle>().Color);
            if (menu.Item("DrawE").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(player.Position, E.Range, menu.Item("DrawE").GetValue<Circle>().Color);
            if (menu.Item("DrawR").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(player.Position, R.Range, menu.Item("DrawR").GetValue<Circle>().Color);
        }

        static void BeforeAttack(LeagueSharp.Common.Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target.Type == GameObjectType.obj_AI_Minion && menu.Item("aa").GetValue<bool>() && AlliesInRange(800) > 0) args.Process = false;
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("gapclose").GetValue<bool>() || !E.LSIsReady()) return;
            E.Cast();
        }

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!unit.IsValid || unit.IsDead || !unit.IsTargetable || unit.IsStunned) return;
            if (R.LSIsReady() && R.IsInRange(unit.Position) && spell.DangerLevel >= InterruptableDangerLevel.High)
            {
                R.Cast(unit.Position, true);
                return;
            }
            else
            {
                if (!menu.Item("exhaust").GetValue<bool>()) return;
				if(unit.LSDistance(player.Position) > 600) return;
                if (player.LSGetSpellSlot("SummonerExhaust") != SpellSlot.Unknown && player.Spellbook.CanUseSpell(player.LSGetSpellSlot("SummonerExhaust")) == SpellState.Ready)
                    player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerExhaust"), unit);
                if ((W.LSIsReady() && GetPassiveCount() == 2) || (LeagueSharp.Common.Utility.LSHasBuff(player, "sonapassiveattack") && player.LastCastedSpellName() == "SonaW") || (LeagueSharp.Common.Utility.LSHasBuff(player, "sonapassiveattack") && W.LSIsReady()))
                {
                    if (W.LSIsReady()) W.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }
            }
        }

        static int GetPassiveCount()
        {
            foreach (BuffInstance buff in player.Buffs)
                if (buff.Name == "sonapassivecount") return buff.Count;
            return 0;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (menu.Item("panic").GetValue<KeyBind>().Active)
                R.Cast(R.GetPrediction(TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical)).CastPosition, packets);

            if (menu.Item("cleanse").GetValue<bool>())
                CCRemove();

            // Combo
            if (SOW.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();

            // Harass
            if (SOW.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Harass();
        }
        
        static void Combo()
        {
            bool useQ = Q.LSIsReady() && menu.Item("UseQC").GetValue<bool>();
            bool useW = W.LSIsReady() && menu.Item("UseWC").GetValue<bool>();
            bool useE = E.LSIsReady() && menu.Item("UseEC").GetValue<bool>();
            bool useR = R.LSIsReady() && menu.Item("UseRC").GetValue<bool>();
            AIHeroClient targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            AIHeroClient targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            foreach (var item in player.InventoryItems)
                if (item.Id == ItemId.Frost_Queens_Claim && player.Spellbook.CanUseSpell((SpellSlot)item.Slot) == SpellState.Ready)
                    player.Spellbook.CastSpell(item.SpellSlot, targetQ);

            if (useQ && targetQ != null && Vector3.Distance(player.Position, targetQ.Position) < menu.Item("QRange").GetValue<Slider>().Value)
                Q.Cast();

            if (useW)
                UseWSmart(menu.Item("healC").GetValue<Slider>().Value, menu.Item("healN").GetValue<Slider>().Value);

            if (useE)
                UseESmart(TargetSelector.GetTarget(1700, TargetSelector.DamageType.Magical));
            if (useR && targetR != null)
                R.CastIfWillHit(targetR, menu.Item("RN").GetValue<Slider>().Value, packets);
        }

        static void Harass()
        {
            bool useQ = Q.LSIsReady() && menu.Item("UseQH").GetValue<bool>();
            bool useW = W.LSIsReady() && menu.Item("UseWH").GetValue<bool>();
            AIHeroClient targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (useQ && targetQ != null && (LeagueSharp.Common.Utility.LSCountEnemiesInRange(player.Position, (int)Q.Range) > 1 || !menu.Item("UseQHF").GetValue<bool>()))
                Q.Cast();

            if (useW)
                UseWSmart(menu.Item("healC").GetValue<Slider>().Value, menu.Item("healN").GetValue<Slider>().Value);
        }

        static void UseWSmart(int percent, int count)
        {
            AIHeroClient ally = MostWoundedAllyInRange(W.Range);
            double wHeal = (10 + 20 * W.Level + .2 * player.FlatMagicDamageMod) * (1 + (player.Health / player.MaxHealth) / 2);
            int allies = AlliesInRange(W.Range);

            if (allies >= count && (ally.Health / ally.MaxHealth) * 100 <= percent)
                W.Cast();
            if (allies < 2 && menu.Item("healmC").GetValue<bool>())
                if (menu.Item("healmC2").GetValue<bool>() && player.MaxHealth - player.Health > wHeal)
                    W.Cast();
                else if ((player.Health / player.MaxHealth) * 100 <= percent) W.Cast(); ;
        }

        //Ty DETUKS, copypasted as fuck :P
        public static void UseESmart(Obj_AI_Base target)
        {
            try
            {

                if (target.Path.Length == 0 || !target.IsMoving)
                    return;
                Vector2 nextEnemPath = target.Path[0].LSTo2D();
                var dist = player.Position.LSTo2D().LSDistance(target.Position.LSTo2D());
                var distToNext = nextEnemPath.LSDistance(player.Position.LSTo2D());
                if (distToNext <= dist)
                    return;
                var msDif = player.MoveSpeed - target.MoveSpeed;
                if (msDif <= 0 && !Orbwalking.InAutoAttackRange(target))
                    E.Cast();

                var reachIn = dist / msDif;
                if (reachIn > 4)
                    E.Cast();
            }
            catch { }

        }

        static AIHeroClient MostWoundedAllyInRange(float range)
        {
            float lastHealth = 9000f;
            AIHeroClient temp = new AIHeroClient();
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
                if (hero.IsAlly && !hero.IsMe && !hero.IsDead && Vector3.Distance(player.Position, hero.Position) <= range && hero.Health < lastHealth)
                {
                    lastHealth = hero.Health;
                    temp = hero;
                }
            return temp;
        }

        static void CCRemove()
        {
            //The best way to do it it's LINQ...
            //Realization taken from h3h3's Support AIO
            if (!Items.HasItem((int)ItemId.Mikaels_Crucible, player) || !Items.CanUseItem((int)ItemId.Mikaels_Crucible) || LeagueSharp.Common.Utility.LSCountEnemiesInRange(player.Position, 1000) < 1) return;
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsDead && Vector3.Distance(player.Position, h.Position) <= 800).OrderByDescending(h => h.FlatPhysicalDamageMod))
                foreach (var buff in CcTypes)
                    if (hero.HasBuffOfType(buff))
                        Items.UseItem((int)ItemId.Mikaels_Crucible, hero);
        }

        static int AlliesInRange(float range)
        {
            int count = 0;
            foreach(AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
                if (hero.IsAlly && !hero.IsMe && Vector3.Distance(player.Position, hero.Position) <= range) count++;
            return count;
        }
                
        static void LoadMenu()
        {
            // Initialize the menu
            menu = new Menu("Royal Song of Sona", "sona", true);

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
            combo.AddItem(new MenuItem("UseQC", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("QRange", "Use Q on range").SetValue(new Slider(850, 500, 850)));//Pointless, but you asked for it
            combo.AddItem(new MenuItem("UseWC", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("UseEC", "Use E (smart)").SetValue(true));
            combo.AddItem(new MenuItem("UseRC", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("RN", "Ulti if hit").SetValue(new Slider(2, 1, 5)));

            // Harass
            Menu harass = new Menu("Harass", "harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("UseQHF", "Q only 2 or more enemies").SetValue(true));
            harass.AddItem(new MenuItem("UseWH", "Use W").SetValue(true));

            Menu heal = new Menu("Heal options", "heal");
            menu.AddSubMenu(heal);
            heal.AddItem(new MenuItem("healC", "Heal only when ally with hp < x%").SetValue(new Slider(60, 5, 100)));
            heal.AddItem(new MenuItem("healN", "Heal only when № of allies in range").SetValue(new Slider(1, 0, 4)));
            heal.AddItem(new MenuItem("healmC", "Heal yourself anyway").SetValue(true));
            heal.AddItem(new MenuItem("healmC2", "^ ON: Fill HP | Same as for others :OFF").SetValue(false));

            Menu misc = new Menu("Misc", "misc");
            menu.AddSubMenu(misc);
            misc.AddItem(new MenuItem("gapclose", "Auto E on enemy gapclose").SetValue(false));
            misc.AddItem(new MenuItem("interrupt", "Interrupt spells").SetValue(true));
            misc.AddItem(new MenuItem("aa", "AA minions only when no allies nearby").SetValue(true));
            misc.AddItem(new MenuItem("exhaust", "Exhaust if not possible to inperrupt").SetValue(true));
            misc.AddItem(new MenuItem("cleanse", "Use Mikaels").SetValue(true)); //DONE
            misc.AddItem(new MenuItem("packets", "Packet cast").SetValue(true));
            misc.AddItem(new MenuItem("panic", "Panic ult key").SetValue(new KeyBind('T', KeyBindType.Press)));

            Menu drawings = new Menu("Drawings", "drawings");
            menu.AddSubMenu(drawings);
            drawings.AddItem(new MenuItem("DrawQ", "Draw Q range").SetValue(new Circle(true, System.Drawing.Color.Cyan, Q.Range)));
            drawings.AddItem(new MenuItem("DrawW", "Draw W range").SetValue(new Circle(true, System.Drawing.Color.ForestGreen, W.Range)));
            drawings.AddItem(new MenuItem("DrawE", "Draw E range").SetValue(new Circle(true, System.Drawing.Color.DeepPink, E.Range)));
            drawings.AddItem(new MenuItem("DrawR", "Draw R range").SetValue(new Circle(true, System.Drawing.Color.Gold, R.Range)));

            // Finalize menu
            menu.AddToMainMenu();
            Console.WriteLine("Menu finalized");
        }
    }
}
