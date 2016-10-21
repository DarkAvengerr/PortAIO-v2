#region
using System;
using System.Collections.Generic;
using LeagueSharp;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp.Common;
using SharpDX;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace D_Lucian
{
    internal class Program
    {
        private const string ChampionName = "Lucian";

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell _q, _q1, _w, _w2, _e, _r;

        private static Menu _config;

        public static bool Qcast, Wcast, Ecast;

        private static AIHeroClient _player;

        private static Items.Item _youmuu, _blade, _bilge;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;

            if (_player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 675);
            _q1 = new Spell(SpellSlot.Q, 900);
            _w = new Spell(SpellSlot.W, 900, TargetSelector.DamageType.Magical);
            _w2 = new Spell(SpellSlot.W, 900, TargetSelector.DamageType.Magical);
            _e = new Spell(SpellSlot.E, 475f);
            _r = new Spell(SpellSlot.R, 1200);


            _q.SetTargetted(0.25f, 1400f);
            _q1.SetSkillshot(0.5f, 50, float.MaxValue, false, SkillshotType.SkillshotLine);
            _w.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            _w2.SetSkillshot(0.30f, 80f, 1600f, false, SkillshotType.SkillshotCircle);
            _r.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);

            _youmuu = new Items.Item(3142, 10);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);

            //D Graves
            _config = new Menu("D-Lucian", "D-Lucian", true);

            //TargetSelector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            //Orbwalker
            _config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("Combo", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQC", "Use Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWC", "Use W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseEC", "Use E")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("useRaim", "Use R(Semi-Manual)").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Harass
            _config.AddSubMenu(new Menu("Harass", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQH", "Use Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W")).SetValue(false);
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("harasstoggle", "AutoHarass (toggle)").SetValue(
                        new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("Harrasmana", "Minimum Mana").SetValue(new Slider(70, 1, 100)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "Harass!").SetValue(
                        new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //Farm
            _config.AddSubMenu(new Menu("Farm", "Farm"));
            //Last Hit
            _config.SubMenu("Farm").AddSubMenu(new Menu("LastHit", "LastHit"));
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseQLH", "Q LastHit")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseWLH", "W LastHit")).SetValue(false);
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(new MenuItem("Lastmana", "Minimum Mana").SetValue(new Slider(70, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(
                    new MenuItem("ActiveLast", "LastHit!").SetValue(
                        new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            //Lane Clear
            _config.SubMenu("Farm").AddSubMenu(new Menu("LaneClear", "LaneClear"));
            _config.SubMenu("Farm").SubMenu("LaneClear").AddItem(new MenuItem("UseQLP", "Q To Harass")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneClear").AddItem(new MenuItem("UseQL", "Q LaneClear")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LaneClear")
                .AddItem(new MenuItem("minminions", "Minimum minions to use Q").SetValue(new Slider(3, 1, 6)));
            _config.SubMenu("Farm").SubMenu("LaneClear").AddItem(new MenuItem("UseWL", "W LaneClear")).SetValue(false);
            _config.SubMenu("Farm")
                .SubMenu("LaneClear")
                .AddItem(new MenuItem("minminionsw", "Minimum minions to use W").SetValue(new Slider(3, 1, 5)));
            _config.SubMenu("Farm")
                .SubMenu("LaneClear")
                .AddItem(new MenuItem("Lanemana", "Minimum Mana").SetValue(new Slider(70, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LaneClear")
                .AddItem(
                    new MenuItem("ActiveLane", "LaneClear!").SetValue(
                        new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            //Jungle clear
            _config.SubMenu("Farm").AddSubMenu(new Menu("JungleClear", "JungleClear"));
            _config.SubMenu("Farm").SubMenu("JungleClear").AddItem(new MenuItem("UseQJ", "Q Jungle")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("JungleClear").AddItem(new MenuItem("UseWJ", "W Jungle")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("JungleClear")
                .AddItem(new MenuItem("Junglemana", "Minimum Mana").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("JungleClear")
                .AddItem(
                    new MenuItem("ActiveJungle", "Jungle key").SetValue(
                        new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));


            //items
            //Offensive
            _config.AddSubMenu(new Menu("items", "items"));
            _config.SubMenu("items").AddSubMenu(new Menu("Offensive", "Offensive"));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Youmuu", "Use Youmuu's")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "Use Bilge")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "If Enemy Hp <").SetValue(new Slider(60, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "Or your Hp < ").SetValue(new Slider(60, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "Use Blade")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "If Enemy Hp <").SetValue(new Slider(60, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "Or Your  Hp <").SetValue(new Slider(60, 1, 100)));

            //Deffensive
            _config.SubMenu("items").AddSubMenu(new Menu("Deffensive", "Deffensive"));
            _config.SubMenu("items").SubMenu("Deffensive").AddSubMenu(new Menu("Cleanse", "Cleanse"));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("useqss", "Use QSS/Mercurial Scimitar/Dervish Blade"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("blind", "Blind"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("charm", "Charm"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("fear", "Fear"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("flee", "Flee"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("snare", "Snare"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("taunt", "Taunt"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("suppression", "Suppression"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("stun", "Stun"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("polymorph", "Polymorph"))
                .SetValue(false);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("silence", "Silence"))
                .SetValue(false);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("zedultexecute", "Zed Ult"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .SubMenu("Cleanse")
                .AddItem(new MenuItem("Cleansemode", "Use Cleanse"))
                .SetValue(new StringList(new string[2] { "Always", "In Combo" }));

            //potions
            _config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usehppotions", "Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usepotionhp", "If Health % <").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usemppotions", "Use Hunters/Corrupting/Biscuit"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usepotionmp", "If Mana % <").SetValue(new Slider(35, 1, 100)));

            //Misc
            _config.AddSubMenu(new Menu("Misc", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("UseQM", "Use Q KillSteal")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseWM", "Use W KillSteal")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Gap_E", "GapClosers E")).SetValue(true);

            //Drawings
            _config.AddSubMenu(new Menu("Drawings", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawharass", "Draw Auto Harass").SetValue(true));

            _config.AddToMainMenu();
            Chat.Print("<font color='#881df2'>D-Lucian by Diabaths</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Chat.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Chat.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_player.IsDead) return;
            if (_config.Item("useRaim").GetValue<KeyBind>().Active && _r.IsReady())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                var t = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(_r.Range) && !_player.HasBuff("LucianR")) _r.Cast(t.Position);
            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (!_config.Item("ActiveCombo").GetValue<KeyBind>().Active
                && (_config.Item("ActiveHarass").GetValue<KeyBind>().Active
                    || _config.Item("harasstoggle").GetValue<KeyBind>().Active)
                && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Harrasmana").GetValue<Slider>().Value)
            {
                Harass();

            }
            if (_config.Item("ActiveLane").GetValue<KeyBind>().Active
                && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lanemana").GetValue<Slider>().Value)
            {
                Laneclear();
            }
            if (_config.Item("ActiveJungle").GetValue<KeyBind>().Active
                && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Junglemana").GetValue<Slider>().Value)
            {
                JungleClear();
            }
            if (_config.Item("ActiveLast").GetValue<KeyBind>().Active
                && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lastmana").GetValue<Slider>().Value)
            {
                LastHit();
            }

            _player = ObjectManager.Player;

            Usecleanse();
            KillSteal();
            Usepotion();
        }

        /* public static bool IsWall(Vector2 vector)
        {
            return NavMesh.GetCollisionFlags(vector.X, vector.Y).HasFlag(CollisionFlags.Wall);
        }*/

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
                if (args.Animation == "Spell1" || args.Animation == "Spell2")
                {
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }

        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                Qcast = true;
                LeagueSharp.Common.Utility.DelayAction.Add(300, () => Qcast = false);
            }
            if (args.Slot == SpellSlot.W)
            {
                Wcast = true;

                LeagueSharp.Common.Utility.DelayAction.Add(300, () => Wcast = false);
            }

            if (args.Slot == SpellSlot.E)
            {
                Ecast = true;
                LeagueSharp.Common.Utility.DelayAction.Add(300, () => Ecast = false);
            }

            if (_player.HasBuff("LucianR"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                //args.Process = false;
            }
        }
    
        private static bool HavePassivee => Qcast || Wcast || Ecast || _player.HasBuff("LucianPassiveBuff");
        
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
               if (args.SData.Name.Equals("LucianW", StringComparison.InvariantCultureIgnoreCase))
                {
                    Wcast = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(100, Orbwalking.ResetAutoAttackTimer);
                    LeagueSharp.Common.Utility.DelayAction.Add(300, () => Wcast = false);
                }
                if (args.SData.Name.Equals("LucianE", StringComparison.InvariantCultureIgnoreCase))
                {
                    Ecast = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(100, Orbwalking.ResetAutoAttackTimer);
                    LeagueSharp.Common.Utility.DelayAction.Add(300, () => Ecast = false);
                }
                if (args.SData.Name.Equals("LucianQ", StringComparison.InvariantCultureIgnoreCase))
                {
                    Qcast = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(100, Orbwalking.ResetAutoAttackTimer);
                    LeagueSharp.Common.Utility.DelayAction.Add(300, () => Qcast = false);
                }
                if (_player.HasBuff("LucianR"))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
        }

        public static void CastQ()
        {
            if (!_q.IsReady()) return;
            var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget(_q.Range))
                return;
            {
                _q.Cast(target);
            }
        }

        public static void ExtendedQ()
        {
            if (!_q.IsReady()) return;
            var target = TargetSelector.GetTarget(_q1.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget(_q1.Range))
                return;

            var qpred = _q1.GetPrediction(target);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All,
                MinionTeam.NotAlly, MinionOrderTypes.None);
            var champions = HeroManager.Enemies.Where(m => m.Distance(ObjectManager.Player) <= _q.Range);
            var monsters = MinionManager.GetMinions(_player.ServerPosition, _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            {
                foreach (var minion in from minion in minions
                                       let polygon = new Geometry.Polygon.Rectangle(
                                            ObjectManager.Player.ServerPosition,
                                           ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, _q1.Range), 65f)
                                       where polygon.IsInside(qpred.CastPosition)
                                       select minion)
                {
                    if (minion.IsValidTarget(_q1.Range))
                        _q.Cast(minion);
                }

                foreach (var champ in from champ in champions
                                      let polygon = new Geometry.Polygon.Rectangle(
                                           ObjectManager.Player.ServerPosition,
                                          ObjectManager.Player.ServerPosition.Extend(champ.ServerPosition, _q1.Range), 65f)
                                      where polygon.IsInside(qpred.CastPosition)
                                      select champ)
                {
                    if (champ.IsValidTarget(_q1.Range))
                        _q.Cast(champ);
                }

                foreach (var monster in from monster in monsters
                                        let polygon = new Geometry.Polygon.Rectangle(
                                             ObjectManager.Player.ServerPosition,
                                            ObjectManager.Player.ServerPosition.Extend(monster.ServerPosition, _q1.Range), 65f)
                                        where polygon.IsInside(qpred.CastPosition)
                                        select monster)
                {
                    if (monster.IsValidTarget(_q1.Range))
                        _q.Cast(monster);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_e.IsReady() && gapcloser.Sender.Distance(_player.ServerPosition) <= 200 &&
                _config.Item("Gap_E").GetValue<bool>())
            {
                _e.Cast(ObjectManager.Player.Position.Extend(gapcloser.Sender.Position, -_e.Range));
            }
        }

        private static void Usecleanse()
        {
            if (_player.IsDead ||
                (_config.Item("Cleansemode").GetValue<StringList>().SelectedIndex == 1 &&
                 !_config.Item("ActiveCombo").GetValue<KeyBind>().Active))
                return;
            if (Cleanse(_player) && _config.Item("useqss").GetValue<bool>())
            {
                if (_player.HasBuff("zedulttargetmark"))
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140))
                        LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3140));
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139))
                        LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3139));
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137))
                        LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3137));
                }
                else
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) Items.UseItem(3140);
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) Items.UseItem(3139);
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) Items.UseItem(3137);
                }
            }
        }

        private static bool Cleanse(AIHeroClient hero)
        {
            var cc = false;
            if (_config.Item("blind").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Blind))
                {
                    cc = true;
                }
            }
            if (_config.Item("charm").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Charm))
                {
                    cc = true;
                }
            }
            if (_config.Item("fear").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Fear))
                {
                    cc = true;
                }
            }
            if (_config.Item("flee").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Flee))
                {
                    cc = true;
                }
            }
            if (_config.Item("snare").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Snare))
                {
                    cc = true;
                }
            }
            if (_config.Item("taunt").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Taunt))
                {
                    cc = true;
                }
            }
            if (_config.Item("suppression").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Suppression))
                {
                    cc = true;
                }
            }
            if (_config.Item("stun").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Stun))
                {
                    cc = true;
                }
            }
            if (_config.Item("polymorph").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Polymorph))
                {
                    cc = true;
                }
            }
            if (_config.Item("silence").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Silence))
                {
                    cc = true;
                }
            }
            if (_config.Item("zedultexecute").GetValue<bool>())
            {
                if (_player.HasBuff("zedulttargetmark"))
                {
                    cc = true;
                }
            }
            return cc;
        }

        private static void Combo()
        {
            var useQ = _config.Item("UseQC").GetValue<bool>();
            var useW = _config.Item("UseWC").GetValue<bool>();
            if (useQ && !HavePassivee && !_player.IsDashing())
            {
                var t = TargetSelector.GetTarget(_q1.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(_q1.Range) && _q.IsReady() && !t.HasBuffOfType(BuffType.Invulnerability))
                    ExtendedQ();
                else if (t.IsValidTarget(_q.Range) && _q.IsReady() && !t.HasBuffOfType(BuffType.Invulnerability))
                    CastQ();
            }
            if (useW && _w.IsReady() && !HavePassivee && !_player.IsDashing() && !_q.IsReady())
            {
                var t = TargetSelector.GetTarget(_w.Range, TargetSelector.DamageType.Magical);
                var predW = _w.GetPrediction(t);
                if (t.IsValidTarget(_w.Range) && predW.Hitchance >= HitChance.Medium && predW.CollisionObjects.Count == 0)
                    _w.Cast(t, false, true);
                else if (t.IsValidTarget(_w2.Range) && predW.Hitchance >= HitChance.Medium)
                {
                    _w2.Cast(t, false, true);
                }
            }
            var useE = _config.Item("UseEC").GetValue<bool>();
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;
            if (useE && _e.IsReady() &&
               !_player.HasBuff("LucianR") && !HavePassivee)
            {
                var ta = TargetSelector.GetTarget(_q1.Range, TargetSelector.DamageType.Physical);
                if (ta == null) return;
                if (ObjectManager.Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                {
                    if (!ta.UnderTurret()&& ta.IsValidTarget(_q.Range) && !Orbwalking.InAutoAttackRange(ta))
                    {
                        _e.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 450));
                    }
                    else if (ta.UnderTurret() && _e.IsReady() && ta.IsValidTarget(_q.Range + _e.Range))
                        if (_q.ManaCost + _e.ManaCost < _player.Mana && ta.Health < _q.GetDamage(ta))
                        {
                            _e.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 450));
                            CastQ();
                        }
                        else if (ta.Health < _player.GetAutoAttackDamage(ta, true) * 2 && ta.IsValidTarget())
                        {
                            _e.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 450));
                        }
                }
            }
            UseItemes();
        }


        private static void Harass()
        {
            var useQ = _config.Item("UseQH").GetValue<bool>();
            var useW = _config.Item("UseWH").GetValue<bool>();

            if (useQ && _q.IsReady() && !HavePassivee && !_player.IsDashing())
            {
                var t = TargetSelector.GetTarget(_q1.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(_q1.Range) && !t.HasBuffOfType(BuffType.Invulnerability))
                    ExtendedQ();
                else if (t.IsValidTarget(_q.Range) && !t.HasBuffOfType(BuffType.Invulnerability))
                    CastQ();
            }
            if (useW && _w.IsReady() && !HavePassivee && !_q.IsReady() && !_player.IsDashing())
            {
                var t = TargetSelector.GetTarget(_w.Range, TargetSelector.DamageType.Magical);
                var predW = _w.GetPrediction(t);
                if (t.IsValidTarget(_w.Range) && predW.Hitchance >= HitChance.High && predW.CollisionObjects.Count == 0)
                    _w.Cast(t, false, true);
                else if (t.IsValidTarget(_w2.Range) && predW.Hitchance >= HitChance.High)
                {
                    _w2.Cast(t, false, true);
                }
            }
        }

        private static void Laneclear()
        {
            if (!Orbwalking.CanMove(40)) return;
            var t = TargetSelector.GetTarget(_q1.Range, TargetSelector.DamageType.Physical);
            var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }
            var minionhitq = _config.Item("minminions").GetValue<Slider>().Value;
            var minionhitw = _config.Item("minminionsw").GetValue<Slider>().Value;
            var useQl = _config.Item("UseQL").GetValue<bool>();
            var useWl = _config.Item("UseWL").GetValue<bool>();
            var useQlP = _config.Item("UseQLP").GetValue<bool>();
            var farmminions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            var minionsq = farmminions.FindAll(qminion => minion.IsValidTarget(_q.Range));
            var minionq = minionsq.Find(minionQ => minionQ.IsValidTarget());

            if (_q.IsReady() && useQl && !HavePassivee)
            {
                foreach (var minionssq in farmminions)
                {
                    var prediction = Prediction.GetPrediction(minionssq, _q.Delay, 10);

                    var collision = _q.GetCollision(_player.Position.To2D(),
                        new List<Vector2> { prediction.UnitPosition.To2D() });
                    foreach (var collisions in collision)
                    {
                        if (collision.Count() >= minionhitq)

                        {
                            if (collision.Last().Distance(_player) - collision[0].Distance(_player) < 600 &&
                                collision[0].Distance(_player) < 500)
                                _q.Cast(collisions);
                        }
                    }
                }
            }
            if (_q.IsReady() && useQlP && !HavePassivee)
                if (_q.IsReady() && t.IsValidTarget(_q1.Range) && !HavePassivee)
                    ExtendedQ();


            if (_w.IsReady() && useWl && !HavePassivee && !_q.IsReady())
            {
                if (_w.GetLineFarmLocation(farmminions).MinionsHit >= minionhitw)
                {
                    _w.Cast(minionq);
                }
            }
        }


        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLH").GetValue<bool>();
            var useW = _config.Item("UseWLH").GetValue<bool>();
            if (allMinions.Count < 3) return;
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q) &&
                    !HavePassivee)
                {
                    _q.Cast(minion);
                }

                if (_w.IsReady() && useW && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W) &&
                    !HavePassivee)
                {
                    _w.Cast(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var useQ = _config.Item("UseQJ").GetValue<bool>();
            var useW = _config.Item("UseWJ").GetValue<bool>();
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady() && mob.IsValidTarget(_q.Range) && !HavePassivee && !mob.Name.Contains("Mini"))
                {
                    _q.Cast(mob);
                }
                if (_w.IsReady() && useW && mob.IsValidTarget(_w.Range) && !HavePassivee && !mob.Name.Contains("Mini") && !_q.IsReady())
                {
                    _w.Cast(mob);
                }
            }
        }

        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iBilge = _config.Item("Bilge").GetValue<bool>();
                var iBilgeEnemyhp = hero.Health <=
                                    (hero.MaxHealth * (_config.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
                var iBilgemyhp = _player.Health <=
                                 (_player.MaxHealth * (_config.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
                var iBlade = _config.Item("Blade").GetValue<bool>();
                var iBladeEnemyhp = hero.Health <=
                                    (hero.MaxHealth * (_config.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
                var iBlademyhp = _player.Health <=
                                 (_player.MaxHealth * (_config.Item("Blademyhp").GetValue<Slider>().Value) / 100);
                var iYoumuu = _config.Item("Youmuu").GetValue<bool>();

                if (hero.IsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);

                }
                if (hero.IsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);

                }
                if (hero.IsValidTarget(450) && iYoumuu && _youmuu.IsReady())
                {
                    _youmuu.Cast();
                }
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range,MinionTypes.All,MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var iusehppotion = _config.Item("usehppotions").GetValue<bool>();
            var iusepotionhp = _player.Health <=
                               (_player.MaxHealth * (_config.Item("usepotionhp").GetValue<Slider>().Value) / 100);
            var iusemppotion = _config.Item("usemppotions").GetValue<bool>();
            var iusepotionmp = _player.Mana <=
                               (_player.MaxMana * (_config.Item("usepotionmp").GetValue<Slider>().Value) / 100);
            if (_player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (LeagueSharp.Common.Utility.CountEnemiesInRange(800) > 0 ||
                (mobs.Count > 0 && _config.Item("ActiveJungle").GetValue<KeyBind>().Active))
            {
                if (iusepotionhp && iusehppotion &&
                    !(ObjectManager.Player.HasBuff("RegenerationPotion") ||
                      ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                      || ObjectManager.Player.HasBuff("ItemCrystalFlask") ||
                      ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                      || ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")))
                {

                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }

                    if (Items.HasItem(2031) && Items.CanUseItem(2031))
                    {
                        Items.UseItem(2031);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }

                if (iusepotionmp && iusemppotion &&
                    !(ObjectManager.Player.HasBuff("ItemDarkCrystalFlask") ||
                      ObjectManager.Player.HasBuff("ItemMiniRegenPotion") ||
                      ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle") ||
                      ObjectManager.Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }

                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }
            }
        }


        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                if (_q.IsReady() && _config.Item("UseQM").GetValue<bool>() && !HavePassivee && !_player.IsDashing())
                {
                    if (_q.GetDamage(hero) > hero.Health && hero.IsValidTarget(_q.Range - 30))
                    {
                        ExtendedQ();
                    }
                    if (_q.GetDamage(hero) > hero.Health && hero.IsValidTarget(_q.Range))
                    {
                        CastQ();
                    }
                }
                if (_w.IsReady() && _config.Item("UseWM").GetValue<bool>() && hero.IsValidTarget(_w.Range)
                    && _w.GetDamage(hero) > hero.Health && !HavePassivee && !_player.IsDashing())
                {
                    var predW = _w.GetPrediction(hero);
                    if (hero.IsValidTarget(_w.Range) && predW.Hitchance >= HitChance.High
                        && predW.CollisionObjects.Count == 0) _w.Cast(hero, false, true);
                    else if (hero.IsValidTarget(_w2.Range) && predW.Hitchance >= HitChance.High)
                    {
                               _w2.Cast(hero, false, true);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = (_config.Item("harasstoggle").GetValue<KeyBind>().Active);

            if (_config.Item("Drawharass").GetValue<bool>())
            {
                if (harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.02f, Drawing.Height * 0.92f, System.Drawing.Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(Drawing.Width * 0.02f, Drawing.Height * 0.92f, System.Drawing.Color.OrangeRed,
                        "Auto harass Disabled");
            }

            if (_config.Item("DrawQ").GetValue<bool>() && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, _q.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.OrangeRed);
            }

            if (_config.Item("DrawW").GetValue<bool>() && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }

            if (_config.Item("DrawE").GetValue<bool>() && _e.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.GreenYellow);
            }

            if (_config.Item("DrawR").GetValue<bool>() && _r.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.GreenYellow);
            }
        }
    }
}