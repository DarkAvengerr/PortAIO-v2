using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace D_RekSai
{
    internal static class Program
    {
        private const string ChampionName = "RekSai";

        private const string Activeq = "RekSaiQ";

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q, _bq, _w, _bw, _e, _be, _r;

        private static Menu _config;

        private static Menu TargetSelectorMenu;

        private static AIHeroClient _player;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static SpellSlot _igniteSlot;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _titanhydra;

        private static SpellSlot _smiteSlot;

        private static Spell _smite;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 300);
            _bq = new Spell(SpellSlot.Q, 1450);
            _w = new Spell(SpellSlot.W, 200f);
            _bw = new Spell(SpellSlot.W, 200f);
            _e = new Spell(SpellSlot.E, 250f);
            _be = new Spell(SpellSlot.E, 700);
            _r = new Spell(SpellSlot.R);

            _bq.SetSkillshot(0.5f, 60, 1950, true, SkillshotType.SkillshotLine);
            _be.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);

            SpellList.Add(_q);
            SpellList.Add(_bq);
            SpellList.Add(_w);
            SpellList.Add(_bw);
            SpellList.Add(_e);
            SpellList.Add(_be);


            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _titanhydra = new Items.Item(3748, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);


            _igniteSlot = _player.GetSpellSlot("SummonerDot");
            if (_player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner1, 570f);
                _smiteSlot = SpellSlot.Summoner1;
            }
            else if (_player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner2, 570f);
                _smiteSlot = SpellSlot.Summoner2;
            }

            //D Rek'Sai
            _config = new Menu("D-RekSai", "D-RekSai", true);

            //TargetSelector
            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            _config.AddSubMenu(TargetSelectorMenu);

            //Orbwalker
            _config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("Combo", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseIgnitecombo", "Use Ignite(rush for it)")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("smitecombo", "Use Smite in target")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q Unburrowed")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQBCombo", "Use Q burrowed")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E Unburrowed")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseEBCombo", "Use E burrowed")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));


            //Items public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, lotis = 3190;
            _config.AddSubMenu(new Menu("items", "items"));
            _config.SubMenu("items").AddSubMenu(new Menu("Offensive", "Offensive"));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Tiamat", "Use Tiamat")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Hydra", "Use Hydra / titanic"))
                .SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "Use Bilge")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "Or your Hp < ").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "Use Blade")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "Or Your  Hp <").SetValue(new Slider(85, 1, 100)));
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
            //_config.SubMenu("items").SubMenu("Deffensive").AddItem(new MenuItem("zhonyas", "Use Zhonyas")).SetValue(true); 
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omen", "Use Randuin Omen"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omenenemys", "Randuin if enemys>").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotis", "Use Iron Solari"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotisminhp", "Solari if Ally Hp<").SetValue(new Slider(35, 1, 100)));

            _config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usehppotions", "Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usepotionhp", "If Health % <").SetValue(new Slider(35, 1, 100)));


            _config.AddSubMenu(new Menu("Harass", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseItemsharass", "Use Items")).SetValue(true);

            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "Harass key").SetValue(
                        new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("harasstoggle", "Harass(toggle)").SetValue(
                        new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));


            _config.AddSubMenu(new Menu("Farm", "Farm"));
            _config.SubMenu("Farm").AddSubMenu(new Menu("Lane", "Lane"));
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseQLane", "Use Q")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseWLane", "Use W")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseELane", "Use E")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseItemslane", "Use Items")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Lane")
                .AddItem(
                    new MenuItem("ActiveLane", "Farm key").SetValue(
                        new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));


            //jungle
            _config.SubMenu("Farm").AddSubMenu(new Menu("Jungle", "Jungle"));
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseQJungle", "Use Q")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseWJungle", "Use W")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseEJungle", "Use E")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("UseItemsjungle", "Usem Items"))
                .SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(
                    new MenuItem("ActiveJungle", "Jungle key").SetValue(
                        new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));


            //Smite 
            _config.AddSubMenu(new Menu("Smite", "Smite"));
            _config.SubMenu("Smite")
                .AddItem(
                    new MenuItem("Usesmite", "Use Smite(toggle)").SetValue(
                        new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
            _config.SubMenu("Smite").AddItem(new MenuItem("Usered", "Smite Red Early ")).SetValue(true);
            _config.SubMenu("Smite")
                .AddItem(new MenuItem("healthJ", "Smite Red Early if HP% <").SetValue(new Slider(35, 1, 100)));

            //Extra
            _config.AddSubMenu(new Menu("Misc", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("AutoW", "Auto W")).SetValue(true);
            _config.SubMenu("Misc")
                .AddItem(new MenuItem("AutoWHP", "Use W if HP is <= "))
                .SetValue(new Slider(25, 1, 100));
            _config.SubMenu("Misc")
                .AddItem(new MenuItem("AutoWMP", "Use W if Fury is >= "))
                .SetValue(new Slider(100, 1, 100));
            _config.SubMenu("Misc").AddItem(new MenuItem("Inter_W", "Use W to Interrupter")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("turnburrowed", "Turn Burrowed if do nothing")).SetValue(true);

            // _config.SubMenu("Misc").AddItem(new MenuItem("Gap_W", "GapClosers W")).SetValue(true);
            _config.SubMenu("Misc")
                .AddItem(new MenuItem("escapeterino", "Escape!!!"))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press));
            //Kill Steal
            _config.AddSubMenu(new Menu("KillSteal", "Ks"));
            _config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "Use KillSteal")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseQKs", "Use Q")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseEKs", "Use E")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "Use Ignite")).SetValue(true);

            //Damage after combo:
            MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            //Drawings
            _config.AddSubMenu(new Menu("Drawings", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(false);
            _config.SubMenu("Drawings").AddItem(dmgAfterComboItem);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawsmite", "Draw smite")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawharass", "Draw AutoHarass")).SetValue(true);

            _config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Chat.Print("<font color='#881df2'>D-Reksai by Diabaths</font> Loaded.");
            Chat.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Chat.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            /*if (_w.Instance.Name.ToLower().Contains("burrowed"))
                burrowed = true;
            else burrowed = false;*/
            _player = ObjectManager.Player;

            _orbwalker.SetAttack(true);
            if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
            {
                Smiteuse();
            }

            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if ((_config.Item("ActiveHarass").GetValue<KeyBind>().Active
                 || _config.Item("harasstoggle").GetValue<KeyBind>().Active)
                && !_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Harass();
            }

            if (_config.Item("ActiveLane").GetValue<KeyBind>().Active)
            {
                Farm();
            }

            if (_config.Item("ActiveJungle").GetValue<KeyBind>().Active)
            {
                JungleClear();
            }

            Usepotion();
            if (_config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }

            Usecleanse();
            if (_config.Item("AutoW").GetValue<bool>()
                && (_config.Item("turnburrowed").GetValue<bool>()
                    && !_config.Item("ActiveCombo").GetValue<KeyBind>().Active
                    || !_config.Item("ActiveHarass").GetValue<KeyBind>().Active
                    || !_config.Item("harasstoggle").GetValue<KeyBind>().Active
                    || !_config.Item("ActiveLane").GetValue<KeyBind>().Active
                    || !_config.Item("ActiveJungle").GetValue<KeyBind>().Active
                    || !_config.Item("escapeterino").GetValue<KeyBind>().Active))
            {
                AutoW();
            }

            if (_config.Item("escapeterino").GetValue<KeyBind>().Active)
            {
                Escapeterino();
            }

            if ((!_config.Item("ActiveCombo").GetValue<KeyBind>().Active
                 || !_config.Item("ActiveHarass").GetValue<KeyBind>().Active
                 || !_config.Item("harasstoggle").GetValue<KeyBind>().Active
                 || !_config.Item("ActiveLane").GetValue<KeyBind>().Active
                 || !_config.Item("ActiveJungle").GetValue<KeyBind>().Active
                 || !_config.Item("escapeterino").GetValue<KeyBind>().Active)
                && _config.Item("turnburrowed").GetValue<bool>() && !IsBurrowed())
            {
                autoburrowed();
            }
        }

        public static bool IsBurrowed()
        {
            return ObjectManager.Player.HasBuff("RekSaiW");
        }
        
        public static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.To2D(), to.To2D(), step);
        }

        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        public static void autoburrowed()
        {
            if (IsBurrowed() || _player.HasBuff("recall") || _player.InFountain()) return;
            if (!IsBurrowed() && _w.IsReady())
            {
                _w.Cast();
            }
        }

        private static void Escapeterino()
        {
            // Walljumper credits to Hellsing

            if (!IsBurrowed() && _w.IsReady() && _be.IsReady()) _w.Cast();

            // We need to define a new move position since jumping over walls
            // requires you to be close to the specified wall. Therefore we set the move
            // point to be that specific piont. People will need to get used to it,
            // but this is how it works.
            var wallCheck = GetFirstWallPoint(_player.Position, Game.CursorPos);

            // Be more precise
            if (wallCheck != null) wallCheck = GetFirstWallPoint((Vector3)wallCheck, Game.CursorPos, 5);

            // Define more position point
            var movePosition = wallCheck != null ? (Vector3)wallCheck : Game.CursorPos;

            // Update fleeTargetPosition
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            var fleeTargetPosition = NavMesh.GridToWorld((short)tempGrid.X, (short)tempGrid.Y);

            // Also check if we want to AA aswell
            Obj_AI_Base target = null;

            // Reset walljump indicators
            var wallJumpPossible = false;

            // Only calculate stuff when our Q is up and there is a wall inbetween
            if (IsBurrowed() && _be.IsReady() && wallCheck != null)
            {
                // Get our wall position to calculate from
                var wallPosition = movePosition;

                // Check 300 units to the cursor position in a 160 degree cone for a valid non-wall spot
                Vector2 direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                float maxAngle = 80;
                float step = maxAngle / 20;
                float currentAngle = 0;
                float currentStep = 0;
                bool jumpTriggered = false;
                while (true)
                {
                    // Validate the counter, break if no valid spot was found in previous loops
                    if (currentStep > maxAngle && currentAngle < 0) break;

                    // Check next angle
                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = (currentStep) * (float)Math.PI / 180;
                        currentStep += step;
                    }

                    else if (currentAngle > 0) currentAngle = -currentAngle;

                    Vector3 checkPoint;

                    // One time only check for direct line of sight without rotating
                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + (_be.Range) * direction.To3D();
                    }
                    // Rotated check
                    else checkPoint = wallPosition + (_be.Range) * direction.Rotated(currentAngle).To3D();

                    // Check if the point is not a wall
                    if (!checkPoint.IsWall())
                    {
                        // Check if there is a wall between the checkPoint and wallPosition
                        wallCheck = GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            // There is a wall inbetween, get the closes point to the wall, as precise as possible
                            Vector3 wallPositionOpposite =
                                (Vector3)GetFirstWallPoint((Vector3)wallCheck, wallPosition, 5);

                            // Check if it's worth to jump considering the path length
                            if (_player.GetPath(wallPositionOpposite).ToList().To2D().PathLength()
                                - _player.Distance(wallPositionOpposite) > 200) //200
                            {
                                // Check the distance to the opposite side of the wall
                                if (_player.Distance(wallPositionOpposite, true)
                                    < Math.Pow((_be.Range + 200) - _player.BoundingRadius / 2, 2))
                                {
                                    // Make the jump happen
                                    _be.Cast(wallPositionOpposite);

                                    // Update jumpTriggered value to not orbwalk now since we want to jump
                                    jumpTriggered = true;

                                    break;
                                }
                                // If we are not able to jump due to the distance, draw the spot to
                                // make the user notice the possibliy
                                else
                                {
                                    // Update indicator values
                                    wallJumpPossible = true;
                                }
                            }

                            else
                            {
                                // yolo
                                Render.Circle.DrawCircle(Game.CursorPos, 35, Color.Red, 2);
                            }
                        }
                    }
                }

                // Check if the loop triggered the jump, if not just orbwalk
                if (!jumpTriggered) Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 0f, false, false);
            }

            // Either no wall or W on cooldown, just move towards to wall then
            else
            {
                Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 0f, false, false);
                if (IsBurrowed() && _be.IsReady()) _be.Cast(Game.CursorPos);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_w.IsReady() && gapcloser.Sender.IsValidTarget(_w.Range) && _config.Item("Gap_W").GetValue<bool>())
            {
                _w.Cast();
            }
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (IsBurrowed() && _bw.IsReady() && unit.IsValidTarget(_q.Range)
                && _config.Item("Inter_W").GetValue<bool>()) _bw.Cast(unit);
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }

            if (spell.Name.ToLower().Contains("reksaiq"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(450, Orbwalking.ResetAutoAttackTimer);
            }

            /*if (sender.IsMe)
            {
                 Chat.Print("Spell name: " + args.SData.Name.ToString());
            }*/
        }

        private static void Usecleanse()
        {
            if (_player.IsDead
                || (_config.Item("Cleansemode").GetValue<StringList>().SelectedIndex == 1
                    && !_config.Item("ActiveCombo").GetValue<KeyBind>().Active)) return;
            if (Cleanse(_player) && _config.Item("useqss").GetValue<bool>())
            {
                if (_player.HasBuff("zedulttargetmark"))
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3140));
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3139));
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Items.UseItem(3137));
                }
                else
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) LeagueSharp.Common.Utility.DelayAction.Add(100, () => Items.UseItem(3140));
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) LeagueSharp.Common.Utility.DelayAction.Add(100, () => Items.UseItem(3139));
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) LeagueSharp.Common.Utility.DelayAction.Add(100, () => Items.UseItem(3137));
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

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var smiteDmg = _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Smite);
                var usesmite = _config.Item("smitecombo").GetValue<bool>();
                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite
                    && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready
                    && hero.IsValidTarget(570))
                {
                    if (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                    }
                    else if (smiteDmg >= hero.Health)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                    }
                }

                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel" && usesmite
                    && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready
                    && hero.IsValidTarget(570))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }
        }

        private static bool Qactive(this AIHeroClient target)
        {
            return target.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == Activeq);
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(_bq.Range, TargetSelector.DamageType.Physical);

            var ignitecombo = _config.Item("UseIgnitecombo").GetValue<bool>();
            var reksaifury = Equals(_player.Mana, _player.MaxMana);

            Smiteontarget();
            if (_igniteSlot != SpellSlot.Unknown && ignitecombo && t.IsValidTarget(600)
                && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (t.Health <= ComboDamage(t))
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);
                }
            }

            if (IsBurrowed())
            {
                if (_config.Item("UseEBCombo").GetValue<bool>())
                {
                    var te = TargetSelector.GetTarget(_be.Range + _bw.Range, TargetSelector.DamageType.Physical);
                    if (_be.IsReady() && te.IsValidTarget(_be.Range + _bw.Range) && _player.Distance(te) > _q.Range)
                    {
                        var predE = _be.GetPrediction(te, true);
                        if (predE.Hitchance >= HitChance.High) _be.Cast(predE.CastPosition.Extend(_player.ServerPosition, -50));
                    }
                }

                if (_config.Item("UseQBCombo").GetValue<bool>())
                {
                    var tbq = TargetSelector.GetTarget(_bq.Range, TargetSelector.DamageType.Magical);
                    if (_bq.IsReady() && t.IsValidTarget(_bq.Range)) _bq.CastIfHitchanceEquals(tbq, HitChance.High);
                }

                if (_config.Item("UseWCombo").GetValue<bool>())
                {
                    var tw = TargetSelector.GetTarget(_w.Range, TargetSelector.DamageType.Physical);
                    if (_w.IsReady() && tw.IsValidTarget(_w.Range) && !_bq.IsReady())
                    {
                        _bw.Cast(t);
                    }
                }
            }

            if (!IsBurrowed())
            {
                if (_config.Item("UseQCombo").GetValue<bool>())
                {
                    var tq = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);
                    if (_q.IsReady() && tq.IsValidTarget(_q.Range)) _q.Cast(t);
                }

                if (_config.Item("UseECombo").GetValue<bool>())
                {
                    var te = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Physical);
                    if (te.IsValidTarget(_e.Range) && _e.IsReady())
                    {
                        if (reksaifury && !Qactive(_player))
                        {
                            _e.Cast(te);
                        }
                        else if (_player.Mana < 100 && t.Health <= EDamage(t))
                        {
                            _e.Cast(te);
                        }
                        else if (_player.Mana == 100 && t.Health <= EDamagetrue(t))
                        {
                            _e.Cast(te);
                        }
                        else if (t.Health <= ComboDamage(t))
                        {
                            _e.Cast(te);
                        }
                    }
                }

                if (_config.Item("UseWCombo").GetValue<bool>() && _w.IsReady())
                {
                    var tw = TargetSelector.GetTarget(_bq.Range, TargetSelector.DamageType.Physical);
                    if (!_q.IsReady() && !tw.IsValidTarget(_e.Range) && tw.IsValidTarget(_bq.Range) && !Qactive(_player)) _w.Cast();
                }
            }

            UseItemes();
        }

        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iBilge = _config.Item("Bilge").GetValue<bool>();
                var iBilgeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (_config.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
                var iBilgemyhp = _player.Health
                                 <= (_player.MaxHealth * (_config.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
                var iBlade = _config.Item("Blade").GetValue<bool>();
                var iBladeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (_config.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
                var iBlademyhp = _player.Health
                                 <= (_player.MaxHealth * (_config.Item("Blademyhp").GetValue<Slider>().Value) / 100);
                var iOmen = _config.Item("Omen").GetValue<bool>();
                var iOmenenemys = hero.CountEnemiesInRange(450) >= _config.Item("Omenenemys").GetValue<Slider>().Value;
                var iTiamat = _config.Item("Tiamat").GetValue<bool>();
                var iHydra = _config.Item("Hydra").GetValue<bool>();

                if (hero.IsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);
                }

                if (hero.IsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);
                }

                if (iTiamat && _tiamat.IsReady() && hero.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra)
                {
                    if (_hydra.IsReady() && hero.IsValidTarget(_hydra.Range))
                    {
                        _hydra.Cast();
                    }

                    if (_titanhydra.IsReady() && hero.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
                    {
                        _titanhydra.Cast();
                    }
                }

                if (iOmenenemys && iOmen && _rand.IsReady() && hero.IsValidTarget(450))
                {
                    _rand.Cast();
                }
            }

            var ilotis = _config.Item("lotis").GetValue<bool>();
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (_config.Item("lotisminhp").GetValue<Slider>().Value) / 100)
                        && hero.Distance(_player.ServerPosition) <= _lotis.Range && _lotis.IsReady()) _lotis.Cast();
                }
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var iusehppotion = _config.Item("usehppotions").GetValue<bool>();
            var iusepotionhp = _player.Health
                               <= (_player.MaxHealth * (_config.Item("usepotionhp").GetValue<Slider>().Value) / 100);
            if (_player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (LeagueSharp.Common.Utility.CountEnemiesInRange(800) > 0
                || (mobs.Count > 0 && _config.Item("ActiveJungle").GetValue<KeyBind>().Active && _smite != null))
            {
                if (iusepotionhp && iusehppotion
                    && !(ObjectManager.Player.HasBuff("RegenerationPotion")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
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
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;

            if (_q.IsReady() && !IsBurrowed()) dmg += QDamage(hero);

            if (IsBurrowed()) dmg += BqDamage(hero);
            if (_w.IsReady() && IsBurrowed()) dmg += WDamage(hero);
            if (_e.IsReady())
                if (_player.Mana < 100)
                {
                    dmg += EDamage(hero);
                }

            if (_player.Mana == 100)
            {
                dmg += EDamagetrue(hero);
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }

            if (Items.HasItem(3077) && Items.CanUseItem(3077)) dmg += _player.GetItemDamage(hero, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074)) dmg += _player.GetItemDamage(hero, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153)) dmg += _player.GetItemDamage(hero, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144)) dmg += _player.GetItemDamage(hero, Damage.DamageItems.Bilgewater);
            dmg += _player.GetAutoAttackDamage(hero, true) * 2;
            return (float)dmg;
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_bq.Range, TargetSelector.DamageType.Magical);
            var targetq = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);
            var targete = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.True);
            var reksaifury = Equals(_player.Mana, _player.MaxMana);
            var useItemsH = _config.Item("UseItemsharass").GetValue<bool>();
            if (_config.Item("UseQHarass").GetValue<bool>())
            {
                if (target.IsValidTarget(_bq.Range) && _bq.IsReady() && IsBurrowed())
                {
                    _bq.CastIfHitchanceEquals(target, HitChance.High);
                }

                if (targetq.IsValidTarget(_q.Range) && _q.IsReady() && !IsBurrowed())
                {
                    _q.Cast();
                }
            }

            if (targete.IsValidTarget(_e.Range) && _config.Item("UseEHarass").GetValue<bool>() && _e.IsReady()
                && !IsBurrowed() && reksaifury)
            {
                _e.Cast(targete);
            }

            if (useItemsH && _tiamat.IsReady() && target.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsH && _hydra.IsReady() && target.IsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }
        }

        private static double EDamage(Obj_AI_Base unit)
        {
            return _e.IsReady() 
                ? _player.CalcDamage(unit, Damage.DamageType.Physical, new double[] { 0.8, 0.9, 1, 1.1, 1.2 }[_e.Level - 1] * _player.TotalAttackDamage * (1 + _player.Mana / _player.MaxMana))
                       : 0d;
        }

        private static double EDamagetrue(Obj_AI_Base unit)
        {
            return _e.IsReady()
                       ? _player.CalcDamage(
                           unit,
                           Damage.DamageType.True,
                           new double[] { 1.6, 1.8, 2, 2.2, 2.4 }[_e.Level - 1] * _player.TotalAttackDamage)
                       : 0d;
        }

        private static double QDamage(Obj_AI_Base unit)
        {
            return _q.IsReady()
                       ? _player.CalcDamage(
                           unit,
                           Damage.DamageType.Physical,
                           new double[] { 45, 75, 105, 135, 165 }[_q.Level - 1] + _player.TotalAttackDamage * 0.6)
                       : 0d;
        }

        private static double BqDamage(Obj_AI_Base unit)
        {
            return _bq.IsReady()
                       ? _player.CalcDamage(
                           unit,
                           Damage.DamageType.Magical,
                           new double[] { 60, 90, 120, 150, 180 }[_bq.Level - 1] + 0.7 * _player.FlatMagicDamageMod)
                       : 0d;
        }

        private static double WDamage(Obj_AI_Base unit)
        {
            return _bq.IsReady()
                       ? _player.CalcDamage(
                           unit,
                           Damage.DamageType.Physical,
                           new double[] { 40, 80, 120, 160, 200 }[_bq.Level - 1] + 0.4 * _player.TotalAttackDamage)
                       : 0d;
        }

        private static void Farm()
        {
            if (!Orbwalking.CanMove(40)) return;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _bq.Range, MinionTypes.All);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLane").GetValue<bool>();
            var useW = _config.Item("UseWLane").GetValue<bool>();
            var useE = _config.Item("UseELane").GetValue<bool>();
            var useItemsl = _config.Item("UseItemslane").GetValue<bool>();
            if (_q.IsReady() && useQ && !IsBurrowed())
            {
                if (
                    allMinionsQ.Where(m => m.Distance(_player.Position) <= _q.Range)
                        .Count(x => _q.GetDamage(x) > x.Health) >= 0)
                {
                    _q.Cast();
                }
            }

            if (_bq.IsReady() && useQ && IsBurrowed())
            {
                if (allMinions.Where(m => m.Distance(_player.Position) <= _bq.Range).Count(x => BqDamage(x) > x.Health)
                    >= 0)
                {
                    _bq.Cast(allMinions.FirstOrDefault());
                }
                else
                    foreach (var minion in allMinions)
                        if (minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q)
                            && minion.IsValidTarget(_bq.Range)) _bq.Cast(minion);
            }

            if (_e.IsReady() && useE && !IsBurrowed())
            {
                foreach (var minione in allMinions) if (minione.Health < EDamage(minione) && !Qactive(_player)) _e.Cast(minione);
            }

            foreach (var minion in allMinions)
            {
                if (useW && !IsBurrowed() && !_q.IsReady() && !_e.IsReady() && Orbwalking.InAutoAttackRange(minion)
                    && !minion.HasBuff("RekSaiKnockupImmune") && !Qactive(_player))
                {
                    _w.Cast();
                }

                if (IsBurrowed() && _bw.IsReady() && useW && minion.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
                {
                    _bw.Cast();
                }
            }

            foreach (var minion in allMinionsQ)
            {
                if (useItemsl && _tiamat.IsReady() && minion.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (useItemsl)
                    if (_hydra.IsReady() && minion.IsValidTarget(_tiamat.Range))
                    {
                        _hydra.Cast();
                    }

                if (_titanhydra.IsReady() && minion.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
                {
                    _titanhydra.Cast();
                }
            }
        }

        public static readonly string[] Smitetype =
            {
                "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
                "s5_summonersmitequick", "itemsmiteaoe", "summonersmite"
            };

        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = _config.Item("ActiveJungle").GetValue<KeyBind>().Active;
            if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var usered = _config.Item("Usered").GetValue<bool>();
            var health = (100 * (_player.Health / _player.MaxHealth)) < _config.Item("healthJ").GetValue<Slider>().Value;
            string[] jungleMinions;
            if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline)
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[]
                                    {
                                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_RiftHerald",
                                        "SRU_Red", "SRU_Krug", "SRU_Dragon_Air", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                                        "SRU_Dragon_Elder", "SRU_Baron"
                                    };
            }

            var minions = MinionManager.GetMinions(_player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline && minion.Health <= smiteDmg
                        && jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }

                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name))
                        && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red"))
                             && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }

        private static void JungleClear()
        {
            var mob =
                MinionManager.GetMinions(
                    _player.ServerPosition,
                    _bq.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useItemsJ = _config.Item("UseItemsjungle").GetValue<bool>();
            var useQ = _config.Item("UseQJungle").GetValue<bool>();
            var useW = _config.Item("UseWJungle").GetValue<bool>();
            var useE = _config.Item("UseEJungle").GetValue<bool>();
            var reksaifury = Equals(_player.Mana, _player.MaxMana);

            if (mob == null) return;
            if (!IsBurrowed())
            {
                if (useQ && _q.IsReady() && Orbwalking.InAutoAttackRange(mob))
                {
                    _q.Cast();
                }

                if (_e.IsReady() && useE && _player.Distance(mob) < _e.Range && !mob.Name.Contains("Mini"))
                {
                    if (reksaifury && !Qactive(_player))
                    {
                        _e.Cast(mob);
                    }
                    else if (mob.Health <= EDamage(mob))
                    {
                        _e.Cast(mob);
                    }
                    // RekSaiKnockupImmune , reksaiknockupimmune
                }

                if (useW && !mob.HasBuff("RekSaiKnockupImmune") && _w.IsReady() && !_q.IsReady() && !_e.IsReady()
                    && mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)) && !Qactive(_player))
                {
                    _w.Cast();
                }
            }

            if (IsBurrowed() && _bq.IsReady() && useQ && _player.Distance(mob) <= _bq.Range)
            {
                _bq.Cast(mob);
            }

            if (IsBurrowed() && _bw.IsReady() && useW && mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
            {
                _bw.Cast();
            }

            if (useItemsJ && _tiamat.IsReady() && mob.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsJ)
            {
                if (_hydra.IsReady() && mob.IsValidTarget(_tiamat.Range))
                {
                    _hydra.Cast();
                }

                if (_titanhydra.IsReady() && mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
                {
                    _titanhydra.Cast();
                }
            }
        }


        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);

                if (hero.IsValidTarget(600) && _config.Item("UseIgnite").GetValue<bool>()
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }

                if (_config.Item("UseQKs").GetValue<bool>())
                {
                    if (_bq.IsReady() && hero.IsValidTarget(_bq.Range) && IsBurrowed())
                    {
                        if (hero.Health <= BqDamage(hero)) _bq.CastIfHitchanceEquals(hero, HitChance.High);
                    }

                    if (_bq.IsReady() && _w.IsReady() && !hero.IsValidTarget(_q.Range) && hero.IsValidTarget(_bq.Range)
                        && hero.Health <= BqDamage(hero))
                    {
                        _w.Cast();
                        _bq.CastIfHitchanceEquals(hero, HitChance.High);
                    }

                    if (_q.IsReady() && hero.IsValidTarget(_q.Range) && !IsBurrowed())
                    {
                        if (hero.Health <= QDamage(hero)) _q.Cast();
                    }
                }

                if (_e.IsReady() && hero.IsValidTarget(_e.Range) && _config.Item("UseEKs").GetValue<bool>()
                    && !IsBurrowed())
                {
                    if (_player.Mana <= 100 && hero.Health <= EDamage(hero))
                    {
                        _e.Cast(hero);
                    }

                    if (_player.Mana == 100 && hero.Health <= EDamagetrue(hero))
                    {
                        _e.Cast(hero);
                    }
                }
            }
        }

        private static void AutoW()
        {
            var reksaiHp = (_player.MaxHealth * (_config.Item("AutoWHP").GetValue<Slider>().Value) / 100);
            var reksaiMp = (_player.MaxMana * (_config.Item("AutoWMP").GetValue<Slider>().Value) / 100);
            if (_player.HasBuff("Recall") || _player.InFountain()) return;
            if (_w.IsReady() && _player.Health <= reksaiHp && !IsBurrowed() && _player.Mana >= reksaiMp)
            {
                _w.Cast();
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = (_config.Item("harasstoggle").GetValue<KeyBind>().Active);
            if (_config.Item("Drawharass").GetValue<bool>())
            {
                if (harass)
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.OrangeRed,
                        "Auto harass Disabled");
            }

            if (_config.Item("Drawsmite").GetValue<bool>() && _smite != null)
            {

                if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Jungle On");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.OrangeRed,
                        "Smite Jungle Off");

                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                    || _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel")
                {
                    if (_config.Item("smitecombo").GetValue<bool>())
                    {
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.GreenYellow,
                            "Smite Target On");
                    }
                    else
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.OrangeRed,
                            "Smite Target Off");
                }
            }

            if (_config.Item("DrawQ").GetValue<bool>() && IsBurrowed() && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _bq.Range, Color.GreenYellow);
            }

            if (_config.Item("DrawE").GetValue<bool>() && _e.Level > 0)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    IsBurrowed() ? _be.Range : _e.Range,
                    System.Drawing.Color.GreenYellow);
            }
        }
    }
}