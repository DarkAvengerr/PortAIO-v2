using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using JustKatarina;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JustKatarina
{
    internal class Program
    {
        public const string ChampName = "Katarina";
        public const string Menun = "JustKatarina";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static bool InUlt = false;
        private static int eTimer;
        private static SpellSlot Ignite;
        private static GameObject _ward;
        private static long dtLastQCast = 0;
        private static long dtLastECast = 0;
        private static int lastWardCast;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("JustKatarina Loaded | Give feedback on forum", 8000);
            Notifications.AddNotification("Don't forget upvote in AssemblyDB", 12000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 675f);
            W = new Spell(SpellSlot.W, 375f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 550f);

            Config = new Menu(Menun, Menun, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));

            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmode", "Harass Mode").SetValue(
                new StringList(new[] { "Q-E-W", "E-Q-W" }, 1)));
            Config.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hE", "Use E").SetValue(true));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("AutoHarass", "Auto Harass", true).SetValue(new KeyBind("J".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("aQ", "Use Q for Auto Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("aW", "Use W for Auto Harass").SetValue(true));

            //Farm
            Config.AddSubMenu(new Menu("Farm", "Farm"));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("AutoFarm", "Auto Farm", true).SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Config.SubMenu("Farm").AddItem(new MenuItem("fq", "Use Q for Auto Farm").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("fw", "Use W for Auto Farm").SetValue(true));

            //Item
            Config.AddSubMenu(new Menu("Item", "Item"));
            Config.SubMenu("Item").AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            Config.SubMenu("Item")
                .AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));

            //Laneclear
            Config.AddSubMenu(new Menu("Clear", "Clear"));
            Config.SubMenu("Clear").AddItem(new MenuItem("lQ", "Use Q").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("lW", "Use W").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("lE", "Use E").SetValue(true));

            //Draw
            Config.AddSubMenu(new Menu("Draw", "Draw"));
            Config.SubMenu("Draw").AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));

            //WardJump
            Config.AddSubMenu(new Menu("Wardjump", "Wardjump"));
            Config.SubMenu("Wardjump").AddItem(new MenuItem("Wardjump", "Ward Jump").SetValue(new KeyBind("Z".ToArray()[0], KeyBindType.Press)));
            Config.SubMenu("Wardjump").AddItem(new MenuItem("newWard", "Place new ward every time").SetValue(false));
            Config.SubMenu("Wardjump").AddItem(new MenuItem("jumpWard", "Jump to Wards").SetValue(true));
            Config.SubMenu("Wardjump").AddItem(new MenuItem("jumpAlly", "Jump to Ally champions").SetValue(true));
            Config.SubMenu("Wardjump").AddItem(new MenuItem("jumpMinion", "Jump to Ally minions").SetValue(true));

            //Legitirino
            Config.AddSubMenu(new Menu("I'm LCS Player", "Legit"));
            Config.SubMenu("Legit").AddItem(new MenuItem("trylegit", "Try to be legit").SetValue(false));
            Config.SubMenu("Legit")
                .AddItem(new MenuItem("LegitCastDelayQ", "Cast Q Delay").SetValue(new Slider(2000, 0, 5000)));
            Config.SubMenu("Legit").
                AddItem(new MenuItem("LegitCastDelayE", "Cast E Delay").SetValue(new Slider(2000, 0, 5000)));

            //Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksQ", "Killsteal with Q").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksW", "Killsteal with W").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksE", "Killsteal with E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("rcancel", "Cancel R for KS").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("autokill", "Enable E hop for Killsteal (to minion, ally, target)").SetValue(true));
            var dmg = new MenuItem("combodamage", "Damage Indicator").SetValue(true);
            var drawFill = new MenuItem("color", "Fill colour", true).SetValue(new Circle(true, Color.Purple));
            Config.SubMenu("Draw").AddItem(drawFill);
            Config.SubMenu("Draw").AddItem(dmg);

            //DrawDamage.DamageToUnit = GetComboDamage;
            //DrawDamage.Enabled = dmg.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmg.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnPlayAnimation += PlayAnimation;
        }

        private static float GetComboDamage(AIHeroClient enemy)
        {
            double damage = 0d;
            
            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q) + player.GetSpellDamage(enemy, SpellSlot.Q, 1);
            
            if (W.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.W);
           
            if (E.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.E);
            
            if (R.IsReady() || (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Surpressed && R.Level > 0))
                damage += player.GetSpellDamage(enemy, SpellSlot.R) * 8;
           
            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);
            
            return (float)damage;
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (player.Distance(sender.Position) <= 700 && sender.IsAlly &&
                (sender.Name == "VisionWard" || sender.Name == "SightWard"))
            {
                _ward = sender;
            }
        }

        private double MarkDmg(Obj_AI_Base target)
        {
            return target.HasBuff("katarinaqmark") ? player.GetSpellDamage(target, SpellSlot.Q, 1) : 0;
        }

        private static void PlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation == "Spell4")
                {
                    InUlt = true;
                }
                else if (args.Animation == "Run" || args.Animation == "Idle1" || args.Animation == "Attack2" ||
                         args.Animation == "Attack1")
                {
                    InUlt = false;
                }
            }
        }

        private static void combo()
        {
            var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Target == null || !Target.IsValidTarget())
                return;

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && Config.Item("UseQ").GetValue<bool>())
            {
                CastQ(Target);
            }
            if (Target.HasBuff("KatarinaQMark") && W.IsReady() && Target.IsValidTarget(W.Range) && Config.Item("UseW").GetValue<bool>())
            {
                W.Cast();
            }
            if (E.IsReady() && Target.IsValidTarget(E.Range) && Config.Item("UseE").GetValue<bool>() && !Q.IsReady())
            {
                CastE(Target);
            }
            if (R.IsReady() && !InUlt && !E.IsReady() && !Q.IsReady() && !W.IsReady() &&
                Target.IsValidTarget(R.Range) && Config.Item("UseR").GetValue<bool>())
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                R.Cast();
                InUlt = true;
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();
        }

        public static void CastE(Obj_AI_Base unit)
        {
            var legit = Config.Item("trylegit").GetValue<bool>();
            var delaye = Config.Item("LegitCastDelayE").GetValue<Slider>().Value;

            if (legit)
            {
                if (Environment.TickCount > dtLastECast + delaye)
                {
                    E.CastOnUnit(unit);
                    dtLastECast = Environment.TickCount;
                }
            }
            else
            {
                E.CastOnUnit(unit);
                dtLastECast = Environment.TickCount;
            }
        }

        public static void CastQ(Obj_AI_Base unit)
        {
            var legit = Config.Item("trylegit").GetValue<bool>();
            var delayq = Config.Item("LegitCastDelayQ").GetValue<Slider>().Value;

            if (legit)
            {
                if (Environment.TickCount > dtLastQCast + delayq)
                {
                    Q.CastOnUnit(unit);
                    dtLastQCast = Environment.TickCount;
                }
            }
            else
            {
                Q.CastOnUnit(unit);
                dtLastQCast = Environment.TickCount;
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Killsteal()
        {
            if (Config.Item("rcancel").GetValue<bool>() && InUlt)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (Config.Item("ksQ").GetValue<bool>() && Q.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(Q.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.Q));
                if (target != null && target.IsValidTarget(Q.Range))
                {
                    CastQ(target);
                }
            }

            if (Config.Item("ksW").GetValue<bool>() && W.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(W.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.W));
                if (target != null && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }

            if (Config.Item("ksE").GetValue<bool>() && E.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(E.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.E));
                if (target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
            }

            if (Config.Item("autokill").GetValue<bool>())
            {
                var targets = ObjectManager.Get<Obj_AI_Base>().Where(
                    target =>
                        ObjectManager.Player.Distance(target.ServerPosition) <= E.Range
                        && !target.IsMe
                        && target.IsTargetable
                        && !target.IsInvulnerable
                    );
                if (targets.Any())
                {
                    foreach (Obj_AI_Base target in targets)
                    {
                        var focuss = ObjectManager.Get<AIHeroClient>().Where(focus =>
                            focus.Distance(target.ServerPosition) <= Q.Range
                            && focus.IsEnemy
                            && !focus.IsMe
                            && !focus.IsInvulnerable
                            && focus.IsValidTarget()
                            );
                        if (focuss.Any())
                        {
                            foreach (AIHeroClient focus in focuss)
                            {
                                var Qdmg = Q.GetDamage(focus);
                                var Wdmg = W.GetDamage(focus);
                                var MarkDmg = Damage.CalcDamage(player, focus, Damage.DamageType.Magical,
                                    player.FlatMagicDamageMod * 0.15 + player.Level * 15);
                                float Ignitedmg;
                                if (Ignite != SpellSlot.Unknown)
                                {
                                    Ignitedmg =
                                        (float)
                                            Damage.GetSummonerSpellDamage(player, focus, Damage.SummonerSpell.Ignite);
                                }
                                else
                                {
                                    Ignitedmg = 0f;
                                }
                                if (focus.Health - Ignitedmg < 0 && Ignite.IsReady())
                                {
                                    player.Spellbook.CastSpell(Ignite, focus);
                                }
                                if (Config.Item("ksQ").GetValue<bool>() && Q.CanCast(target) && Q.IsReady() &&
                                    Config.Item("ksW").GetValue<bool>() && W.CanCast(focus) && W.IsReady() &&
                                    focus.Distance(target) < W.Range &&
                                    focus.Health - Qdmg - Wdmg - MarkDmg < 0)
                                {
                                    CastQ(focus);
                                }

                                if (focus.Health - Ignitedmg < 0 && Ignite.IsReady())
                                {
                                    player.Spellbook.CastSpell(Ignite, focus);
                                }
                                if (E.CanCast(target) &&
                                     ((Config.Item("ksQ").GetValue<bool>() && W.IsReady() &&
                                     Config.Item("ksE").GetValue<bool>() && E.IsReady() &&
                                     focus.Health - Qdmg - MarkDmg < 0 && target.Distance(focus) < Q.Range) ||
                                     (Config.Item("ksQ").GetValue<bool>() && W.IsReady() &&
                                     Config.Item("ksE").GetValue<bool>() && E.IsReady() &&
                                     Config.Item("ksW").GetValue<bool>() && W.IsReady() &&
                                     focus.Health - Qdmg - MarkDmg - Wdmg < 0 && target.Distance(focus) < W.Range))
                                     )
                                {
                                    CastE(target);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            PutWardToKill();
        }

        private static void PutWardToKill()
        {
            var wardingRange = 600;
            foreach (var hero in HeroManager.Enemies.Where(h => h.IsValidTarget() && h.Distance(player) < E.Range + wardingRange))
            {
                var wardingPlace = player.Position.Extend(hero.Position, wardingRange);
                if (!wardingPlace.IsValid() || wardingPlace.IsWall() || !E.IsReady())
                {
                    return;
                }
                var Qdmg = Q.GetDamage(hero);
                var Wdmg = W.GetDamage(hero);
                var MarkDmg = Damage.CalcDamage(player, hero, Damage.DamageType.Magical,
                    player.FlatMagicDamageMod * 0.15 + player.Level * 15);
                float Ignitedmg;
                if (Ignite != SpellSlot.Unknown)
                {
                    Ignitedmg =
                        (float)
                            Damage.GetSummonerSpellDamage(player, hero, Damage.SummonerSpell.Ignite);
                }
                else
                {
                    Ignitedmg = 0f;
                }
                if ((Config.Item("ksQ").GetValue<bool>() && Q.IsReady() &&
                     Config.Item("ksE").GetValue<bool>() && E.IsReady() &&
                     hero.Health - Qdmg < 0 && wardingPlace.Distance(hero.Position) < Q.Range) ||
                     (Config.Item("ksQ").GetValue<bool>() && Q.IsReady() &&
                     Config.Item("ksE").GetValue<bool>() && E.IsReady() &&
                     Config.Item("ksW").GetValue<bool>() && W.IsReady() &&
                     hero.Health - Qdmg - MarkDmg - Wdmg < 0 && wardingPlace.Distance(hero.Position) < W.Range)||
                    (Config.Item("ksW").GetValue<bool>() && W.IsReady() &&
                     Config.Item("ksE").GetValue<bool>() && E.IsReady() &&
                     hero.Health - Wdmg < 0 && wardingPlace.Distance(hero.Position) < W.Range) ||
                    (Config.Item("ksE").GetValue<bool>() && E.IsReady() && Ignite.IsReady() &&
                     hero.Health - Ignitedmg < 0 && wardingPlace.Distance(hero.Position) < 580)
                     )
                {
                    var wardSlot = Items.GetWardSlot();
                    if (wardSlot.IsValidSlot() &&
                        (player.Spellbook.CanUseSpell(wardSlot.SpellSlot) == SpellState.Ready || wardSlot.Stacks != 0) &&
                        CanCastWard())
                    {
                        lastWardCast = Utils.GameTimeTickCount;
                        player.Spellbook.CastSpell(wardSlot.SpellSlot, wardingPlace);
                    }
                }
            }
        }

        private static void items()
        {
            Ignite = player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;

            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("eL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(player) && target.IsValidTarget(E.Range)
                && Config.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

            if (player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            if (InUlt)
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }

            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            if (Config.SubMenu("Wardjump").Item("Wardjump").GetValue<KeyBind>().Active)
            {
                Wardjump();
            }

            Killsteal();
            var autoHarass = Config.Item("AutoHarass", true).GetValue<KeyBind>().Active;
            if (autoHarass)
                AutoHarass();
            var farm = Config.Item("AutoFarm", true).GetValue<KeyBind>().Active;
            if (farm)
                Farm();
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.Contains("ward") || args.SData.Name.Contains("Trinket"))
                    lastWardCast = Utils.GameTimeTickCount;
            }
        }

        private static bool CanCastWard()
        {
            return E.Instance.Name == "KatarinaE" && Utils.GameTimeTickCount - 2000 > lastWardCast;
        }

        private static Obj_AI_Base GetEscapeObject(Vector3 pos, int range = 700)
        {
            if (_ward != null && _ward.IsValid && !_ward.IsDead && player.Distance(_ward.Position) <= range)
            {
                return _ward as Obj_AI_Base;
            }

            if (Config.SubMenu("Wardjump").Item("newWard").GetValue<bool>() && Config.SubMenu("Wardjump").Item("Wardjump").GetValue<KeyBind>().Active)
            {
                return null;
            }

            var allies =
                HeroManager.Allies.Where(hero => hero.Distance(pos) <= range)
                    .OrderBy(hero => hero.Distance(pos))
                    .ToList();
            var minions =
                MinionManager.GetMinions(pos, range, MinionTypes.All, MinionTeam.Ally)
                    .OrderBy(minion => minion.Distance(pos))
                    .ToList();
            var wards =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        obj =>
                            (obj.Name.Contains("Ward") || obj.Name.Contains("ward") || obj.Name.Contains("Trinket")) &&
                            obj.IsAlly && pos.Distance(obj.ServerPosition) <= range)
                    .OrderBy(obj => obj.Distance(pos))
                    .ToList();

            foreach (var ally in allies.Where(ally => !ally.IsMe).Where(ally => Config.SubMenu("Wardjump").Item("jumpAlly").GetValue<bool>()))
            {
                return ally;
            }

            foreach (var ward in wards.Where(ward => player.Distance(ward.ServerPosition) > 400).Where(ward => Config.SubMenu("Wardjump").Item("jumpWard").GetValue<bool>()))
            {
                return ward;
            }
            return Config.SubMenu("Wardjump").Item("jumpMinion").GetValue<bool>() ? minions.FirstOrDefault() : null;
        }

        private static void Wardjump()
        {
            var escapeObject = GetEscapeObject(Game.CursorPos);
            if (escapeObject != null)
            {
                if (CanCastE())
                {
                    eTimer = Utils.GameTimeTickCount + 3000;
                    E.CastOnUnit(escapeObject);
                }
            }
            else if (E.IsReady())
            {
                var wardSlot = Items.GetWardSlot();
                if (wardSlot.IsValidSlot() &&
                    (player.Spellbook.CanUseSpell(wardSlot.SpellSlot) == SpellState.Ready || wardSlot.Stacks != 0) &&
                    CanCastWard())
                {
                    lastWardCast = Utils.GameTimeTickCount;
                    player.Spellbook.CastSpell(wardSlot.SpellSlot, GetCorrectedMousePosition());
                }
            }
        }

        private static bool CanCastE()
        {
            return E.IsReady() && E.Instance.Name == "KatarinaE";
        }

        private static Vector3 GetCorrectedMousePosition()
        {
            return player.ServerPosition - (player.ServerPosition - Game.CursorPos).Normalized() * 600;
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && Config.Item("aQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
                CastQ(target);

            if (W.IsReady() && Config.Item("aW").GetValue<bool>() && target.IsValidTarget(W.Range))
                W.Cast();
        }

        private static void Farm()
        {
            var useq = Config.Item("fq").GetValue<bool>();
            var usew = Config.Item("fw").GetValue<bool>();
            var minionCount = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if (useq && Q.IsReady()
                        && minion.IsValidTarget(Q.Range)
                        && minion.Health < Q.GetDamage(minion))
                    {
                        Q.CastOnUnit(minion);
                    }

                    if (usew && W.IsReady()
                        && minion.IsValidTarget(W.Range)
                        && minion.Health < W.GetDamage(minion))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            int mode = Config.Item("harassmode").GetValue<StringList>().SelectedIndex;
            if (target == null || !target.IsValidTarget())
                return;

            if (mode == 0)
            {
                if (Q.IsReady() && Config.Item("hQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
                {
                    CastQ(target);
                }
                if (E.IsReady() && Config.Item("hE").GetValue<bool>() && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
                if (W.IsReady() && Config.Item("hW").GetValue<bool>() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }

            else if (mode == 1)
            {
                if (E.IsReady() && Config.Item("hE").GetValue<bool>() && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }

                if (Q.IsReady() && Config.Item("hQ").GetValue<bool>() && target.IsValidTarget(player.AttackRange))
                {
                    CastQ(target);
                }

                if (W.IsReady() && Config.Item("hW").GetValue<bool>() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Clear()
        {
            var minionCount = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if (Config.Item("lQ").GetValue<bool>()
                        && Q.IsReady()
                        && minion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(minion);
                    }

                    if (Config.Item("lW").GetValue<bool>()
                        && W.IsReady()
                        && minion.IsValidTarget(W.Range)
                        )
                    {
                        W.Cast();
                    }

                    if (Config.Item("lE").GetValue<bool>()
                        && E.IsReady()
                        && minion.IsValidTarget(E.Range)
                        )
                    {
                        E.CastOnUnit(minion);
                    }

                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, Q.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Wdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, W.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Edraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, E.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Rdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, R.Range, System.Drawing.Color.White, 3);
        }
    }
}
