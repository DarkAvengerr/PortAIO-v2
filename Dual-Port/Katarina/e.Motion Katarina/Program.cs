using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Microsoft.Win32;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy;
using LeagueSharp.Common;
namespace e.Motion_Katarina
{
    class Program
    {

        #region Declaration

        private static bool ShallJumpNow;
        private static Vector3 JumpPosition;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _menu;
        private static int whenToCancelR;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static AIHeroClient qTarget;
        private static Obj_AI_Base qMinion;
        private static readonly AIHeroClient[] AllEnemy = HeroManager.Enemies.ToArray();
        private static bool WardJumpReady;
        private static SpellSlot IgniteSpellSlot = SpellSlot.Unknown;
        private static readonly List<int> AllEnemyTurret = new List<int>();
        private static readonly List<int> AllAllyTurret = new List<int>();
        private static Dictionary<int, bool> TurretHasAggro = new Dictionary<int, bool>();
        private static int lastLeeQTick;
        private static int tickValue;

        #endregion

        static bool IsTurretPosition(Vector3 pos)
        {
            float mindistance = 2000;
            foreach (int NetID in AllEnemyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead && !TurretHasAggro[NetID])
                {
                    float distance = pos.Distance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;

                    }

                }
            }
            return mindistance <= 950;
        }

        public static void Game_OnGameLoad()
        {
            //Wird aufgerufen, wenn LeagueSharp Injected
            if (Player.ChampionName != "Katarina")
            {
                return;
            }
            #region Spells
            Q = new Spell(SpellSlot.Q, 675, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 375, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 700, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 550, TargetSelector.DamageType.Magical);
            //Get Ignite
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner1;
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner2;
            }
            #endregion

            foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (turret.IsEnemy)
                {
                    AllEnemyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
                if (turret.IsAlly)
                {
                    AllAllyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
            }

            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = CalculateDamage;


            #region Menu
            _menu = new Menu("e.Motion Katarina", "motion.katarina", true);

            //Orbwalker-Menü
            Menu orbwalkerMenu = new Menu("Orbwalker", "motion.katarina.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            _menu.AddSubMenu(orbwalkerMenu);

            //Combo-Menü
            Menu comboMenu = new Menu("Combo", "motion.katarina.combo");
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.useq", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.usew", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.usee", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.user", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.rsafe", "Advanced R Checks").SetValue(true).SetTooltip("Will not Cast R if enemy is not killable"));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.mode", "Combo mode").SetValue(new StringList(new[] { "Smart", "Fast" })));
            comboMenu.AddItem(new MenuItem("motion.katarina.combo.order", "Rotation Order").SetValue(new StringList(new[] { "Q -> E -> W -> R", "E -> Q -> W -> R", "Dynamic" })));
            _menu.AddSubMenu(comboMenu);

            //Harrass-Menü
            Menu harassMenu = new Menu("Harass", "motion.katarina.harrass");
            harassMenu.AddItem(new MenuItem("motion.katarina.harass.useq", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("motion.katarina.harass.usew", "Use W").SetValue(true));
            Menu autoHarassMenu = new Menu("Autoharass", "motion.katarina.autoharass");
            autoHarassMenu.AddItem(new MenuItem("motion.katarina.harass.autoharass.toggle", "Automatic Harrass").SetValue(true));
            autoHarassMenu.AddItem(new MenuItem("motion.katarina.harass.autoharass.key", "Toogle Harrass").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
            autoHarassMenu.AddItem(new MenuItem("motion.katarina.harass.autoharass.useq", "Use Q").SetValue(false));
            autoHarassMenu.AddItem(new MenuItem("motion.katarina.harass.autoharass.usew", "Use W").SetValue(true));
            harassMenu.AddSubMenu(autoHarassMenu);
            _menu.AddSubMenu(harassMenu);

            //Laneclear-Menü
            Menu laneclear = new Menu("Laneclear", "motion.katarina.laneclear");
            laneclear.AddItem(new MenuItem("motion.katarina.laneclear.useq", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("motion.katarina.laneclear.usew", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("motion.katarina.laneclear.minw", "Minimum Minions to use W").SetValue(new Slider(3, 1, 6)));
            laneclear.AddItem(new MenuItem("motion.katarina.laneclear.minwlasthit", "Minimum Minions to Lasthit with W").SetValue(new Slider(2, 0, 6)));
            _menu.AddSubMenu(laneclear);

            //Jungleclear-Menü
            Menu jungleclear = new Menu("Jungleclear", "motion.katarina.jungleclear");
            jungleclear.AddItem(new MenuItem("motion.katarina.jungleclear.useq", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("motion.katarina.jungleclear.usew", "Use W").SetValue(true));
            jungleclear.AddItem(new MenuItem("motion.katarina.jungleclear.usee", "Use E").SetValue(true));
            _menu.AddSubMenu(jungleclear);

            //Lasthit-Menü
            Menu lasthit = new Menu("Lasthit", "motion.katarina.lasthit");
            lasthit.AddItem(new MenuItem("motion.katarina.lasthit.useq", "Use Q").SetValue(true));
            lasthit.AddItem(new MenuItem("motion.katarina.lasthit.usew", "Use W").SetValue(true).SetTooltip("Report on Thread when you still have FPS-Drops"));
            lasthit.AddItem(new MenuItem("motion.katarina.lasthit.usee", "Use E").SetValue(false).SetTooltip("Report on Thread when you still have FPS-Drops"));
            lasthit.AddItem(new MenuItem("motion.katarina.lasthit.noenemiese", "Only use E when no Enemies are around").SetValue(true));
            _menu.AddSubMenu(lasthit);

            //KS-Menü
            Menu ksMenu = new Menu("Killsteal", "motion.katarina.killsteal");
            ksMenu.AddItem(new MenuItem("motion.katarina.killsteal.useq", "Use Q").SetValue(true));
            ksMenu.AddItem(new MenuItem("motion.katarina.killsteal.usew", "Use W").SetValue(true));
            ksMenu.AddItem(new MenuItem("motion.katarina.killsteal.usee", "Use E").SetValue(true));
            ksMenu.AddItem(new MenuItem("motion.katarina.killsteal.usef", "Use Ignite").SetValue(true));
            ksMenu.AddItem(new MenuItem("motion.katarina.killsteal.wardjump", "KS with Wardjump").SetValue(true));
            _menu.AddSubMenu(ksMenu);

            //Drawings-Menü
            Menu drawingsMenu = new Menu("Drawings", "motion.katarina.drawings");
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.drawq", "Draw Q").SetValue(false));
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.draww", "Draw W").SetValue(false));
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.drawe", "Draw E").SetValue(false));
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.drawr", "Draw R").SetValue(false));
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.dmg", "Draw Damage to target").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("motion.katarina.drawings.drawalways", "Draw Always").SetValue(false).SetTooltip("Enable this if you want Drawings while your Skills are on Cooldown"));
            _menu.AddSubMenu(drawingsMenu);

            //Misc-Menü
            Menu miscMenu = new Menu("Miscellanious", "motion.katarina.misc");
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.wardjump", "Use Wardjump").SetValue(true));
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.wardjumpkey", "Wardjump Key").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.noRCancel", "Prevent R Cancel").SetValue(true).SetTooltip("This is preventing you from cancelling R accidentally within the first 0.4 seconds of cast"));
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.cancelR", "Cancel R when no Enemies are around").SetValue(false));
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.kswhileult", "Do Killsteal while Ulting").SetValue(true));
            miscMenu.AddItem(new MenuItem("motion.katarina.misc.allyTurret", "Jump unter Turret for Gapcloser").SetTooltip("Try to Jump under Ally Turret when enemy tries to Gapclose you").SetValue(true));

            Menu performanceMenu = new Menu("Performance", "motion.katarina.performance");
            performanceMenu.AddItem(new MenuItem("motion.katarina.performance.track", "Tracked minions for Lasthitting").SetTooltip("High = accurate Lasthit, Low = least FPS-Drops").SetValue(new Slider(3, 1, 10)));
            performanceMenu.AddItem(new MenuItem("motion.katarina.performance.tickmanager", "Enable Tickmanager").SetValue(false));
            performanceMenu.AddItem(new MenuItem("motion.katarina.performance.ticks", "Update Frequency for Tickmanager").SetTooltip("Time in ms when to track Minions again").SetValue(new Slider(8, 2, 50)));

            lasthit.AddSubMenu(performanceMenu);
            _menu.AddSubMenu(miscMenu);

            //alles zum Hauptmenü hinzufügen
            _menu.AddToMainMenu();

            #endregion
            Chat.Print("<font color='#bb0000'>e</font>.<font color='#0000cc'>Motion</font> Katarina loaded");
            #region Subscriptions
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            //Obj_AI_Base.OnTarget += Turret_OnTarget;
            Obj_AI_Base.OnBuffLose += BuffRemove;


            #endregion
        }

        private static bool KillableByUlt()
        {
            List<AIHeroClient> enemies = Player.Position.GetEnemiesInRange(R.Range);
            if (!_menu.Item("motion.katarina.combo.rsafe").GetValue<bool>() || enemies.Count > 1 || enemies[0].Position.GetAlliesInRange(800).Count > 0 ||
                R.GetDamage(enemies[0], 1) + (enemies[0].HasBuff("katarinaqmark") ? Q.GetDamage(enemies[0], 1) : 0) > enemies[0].Health)
            {
                return true;
            }
            return false;

        }

        private static void OnDraw(EventArgs args)
        {
            if (_menu.Item("motion.katarina.drawings.drawq").GetValue<bool>() && (Q.IsReady() || _menu.Item("motion.katarina.drawings.drawalways").GetValue<bool>()))
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.IndianRed);
            if (_menu.Item("motion.katarina.drawings.draww").GetValue<bool>() && (W.IsReady() || _menu.Item("motion.katarina.drawings.drawalways").GetValue<bool>()))
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.IndianRed);
            if (_menu.Item("motion.katarina.drawings.drawe").GetValue<bool>() && (E.IsReady() || _menu.Item("motion.katarina.drawings.drawalways").GetValue<bool>()))
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
            if (_menu.Item("motion.katarina.drawings.drawr").GetValue<bool>() && (R.IsReady() || _menu.Item("motion.katarina.drawings.drawalways").GetValue<bool>()))
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.IndianRed);
        }


        private static void BuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "BlindMonkQOne")
            {

                //Chat.Print("Player lost Lee Sin Q Buff");
                lastLeeQTick = Utils.TickCount;
            }
        }


        static void Game_OnUpdate(EventArgs args)
        {
            Demark();
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = _menu.Item("motion.katarina.drawings.dmg").GetValue<bool>();
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (HasRBuff())
            {
                _orbwalker.SetAttack(false);
                _orbwalker.SetMovement(false);
                if (_menu.Item("motion.katarina.misc.cancelR").GetValue<bool>() && Player.GetEnemiesInRange(R.Range + 50).Count == 0)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (_menu.Item("motion.katarina.misc.kswhileult").GetValue<bool>())
                    Killsteal();
                return;
            }
            if (ShallJumpNow)
            {
                WardJump(JumpPosition, false, false);
                if (!E.IsReady())
                {
                    ShallJumpNow = false;
                }
            }
            _orbwalker.SetAttack(true);
            _orbwalker.SetMovement(true);
            Killsteal();
            //Combo
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo(Q.IsReady() && _menu.Item("motion.katarina.combo.useq").GetValue<bool>(), W.IsReady() && _menu.Item("motion.katarina.combo.usew").GetValue<bool>(), E.IsReady() && _menu.Item("motion.katarina.combo.usee").GetValue<bool>(), R.IsReady() && _menu.Item("motion.katarina.combo.user").GetValue<bool>());
            //Harass
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Combo(Q.IsReady() && _menu.Item("motion.katarina.harass.useq").GetValue<bool>(), W.IsReady() && _menu.Item("motion.katarina.harass.usew").GetValue<bool>(), false, false, true);
            //Autoharass
            if (_menu.Item("motion.katarina.harass.autoharass.toggle").GetValue<bool>() && _menu.Item("motion.katarina.harass.autoharass.key").GetValue<KeyBind>().Active)
                Combo(Q.IsReady() && _menu.Item("motion.katarina.harass.autoharass.useq").GetValue<bool>(), W.IsReady() && _menu.Item("motion.katarina.harass.autoharass.usew").GetValue<bool>(), false, false, true);
            Lasthit();
            LaneClear();
            JungleClear();
            if (_menu.Item("motion.katarina.misc.wardjumpkey").GetValue<KeyBind>().Active && _menu.Item("motion.katarina.misc.wardjump").GetValue<bool>())
            {
                WardJump(Game.CursorPos);
            }
        }


        static bool HasRBuff()
        {
            return Player.HasBuff("KatarinaR") || Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound");

        }



        static void Combo(bool useq, bool usew, bool usee, bool user, bool anyTarget = false)
        {
            bool startWithQ = _menu.Item("motion.katarina.combo.order").GetValue<StringList>().SelectedIndex == 0 && useq;
            bool dynamic = _menu.Item("motion.katarina.combo.order").GetValue<StringList>().SelectedIndex == 2;
            bool smartcombo = _menu.Item("motion.katarina.combo.mode").GetValue<StringList>().SelectedIndex == 0;
            AIHeroClient target = TargetSelector.GetTarget(!startWithQ || dynamic ? E.Range : Q.Range, TargetSelector.DamageType.Magical);
            if (target != null && !target.IsZombie)
            {
                if (useq && (startWithQ || !usee || dynamic) && target.Distance(Player) < Q.Range)
                {
                    Q.Cast(target);
                    qTarget = target;
                    return;
                }
                if (usee && (usew || user || qTarget != target || !smartcombo))
                {
                    E.Cast(target);
                    return;
                }
                if (anyTarget)
                {
                    List<AIHeroClient> enemies = Player.Position.GetEnemiesInRange(390);
                    if (enemies.Count >= 2)
                    {
                        W.Cast();
                        return;
                    }
                    if (enemies.Count == 1)
                    {
                        target = enemies.ElementAt(0);
                    }
                }
                if (target.Distance(Player) < 390 && usew && (user || qTarget != target || !smartcombo))
                {
                    W.Cast();
                    return;
                }
                if (target.Distance(Player) < R.Range && user && KillableByUlt())
                {
                    R.Cast();
                }
            }
        }

        private static void Turret_OnTarget(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.GetType() == typeof(Obj_AI_Turret))
            {
                TurretHasAggro[sender.NetworkId] = !(args.Target == null || args.Target is Obj_AI_Minion);
                //Chat.Print("Turret with Index[" + sender.Index + "] has Aggro: " + (TurretHasAggro[sender.Index]? "yes" : "no"));
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "KatarinaQ" && args.Target.GetType() == typeof(AIHeroClient))
            {
                qTarget = (AIHeroClient)args.Target;
            }
            if (args.SData.Name == "katarinaE")
            {
                WardJumpReady = false;
            }
            if (sender.IsMe && WardJumpReady)
            {
                E.Cast((Obj_AI_Base)args.Target);
                WardJumpReady = false;
            }
            //Todo Check for Lee Q
            if (args.SData.Name == "blindmonkqtwo")
            {

                if (lastLeeQTick - Utils.TickCount <= 10)
                {
                    //Chat.Print("Trying to Jump undeder Ally Turret - OnProcessSpellCast");
                    JumpUnderTurret(-100, sender.Position);
                }
                lastLeeQTick = Utils.TickCount;
            }
            // Todo Test
            if (args.Target != null && args.Target.IsMe && _menu.Item("motion.katarina.misc.allyTurret").GetValue<bool>())
            {
                switch (args.SData.Name)
                {
                    case "ZedR":
                        JumpUnderTurret(-100, sender.Position);
                        break;
                    case "ViR":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "NocturneParanoia":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "MaokaiUnstableGrowth":
                        JumpUnderTurret(0, sender.Position);
                        break;
                }

            }

        }



        private static void JumpUnderTurret(float extrarange, Vector3 objectPosition)
        {
            float mindistance = 100000;
            //Getting next Turret
            Obj_AI_Turret turretToJump = null;

            foreach (int NetID in AllAllyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead)
                {
                    float distance = Player.Position.Distance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;
                        turretToJump = turret;
                    }

                }
            }
            if (turretToJump != null && !TurretHasAggro[turretToJump.NetworkId] && Player.Position.Distance(turretToJump.Position) < 1500)
            {
                int i = 0;

                do
                {
                    Vector3 extPos = Player.Position.Extend(turretToJump.Position, 685 - i);
                    float dist = objectPosition.Distance(extPos + extrarange);
                    Vector3 predictedPosition = objectPosition.Extend(extPos, dist);
                    if (predictedPosition.Distance(turretToJump.Position) <= 890 && !predictedPosition.IsWall())
                    {
                        WardJump(Player.Position.Extend(turretToJump.Position, 650 - i), false);
                        JumpPosition = Player.Position.Extend(turretToJump.Position, 650 - i);
                        ShallJumpNow = true;
                        break;
                    }

                    i += 50;
                } while (i <= 300 || !Player.Position.Extend(turretToJump.Position, 650 - i).IsWall());
            }

        }


        static void Demark()
        {
            if ((qTarget != null && qTarget.HasBuff("katarinaqmark")) || Q.Cooldown < 3)
            {
                qTarget = null;
            }
        }


        #region WardJumping
        private static void WardJump(Vector3 where, bool move = true, bool placeward = true)
        {
            if (move)
                Orbwalking.MoveTo(Game.CursorPos);
            if (!E.IsReady())
            {
                return;
            }
            Vector3 wardJumpPosition = Player.Position.Distance(where) < 600 ? where : Player.Position.Extend(where, 600);
            var lstGameObjects = ObjectManager.Get<Obj_AI_Base>().ToArray();
            Obj_AI_Base entityToWardJump = lstGameObjects.FirstOrDefault(obj =>
                obj.Position.Distance(wardJumpPosition) < 150
                && (obj is Obj_AI_Minion || obj is AIHeroClient)
                && !obj.IsMe && !obj.IsDead
                && obj.Position.Distance(Player.Position) < E.Range);

            if (entityToWardJump != null)
            {
                E.Cast(entityToWardJump);
            }
            else if (placeward)
            {
                int wardId = GetWardItem();
                if (wardId != -1 && !wardJumpPosition.IsWall())
                {
                    WardJumpReady = true;
                    PutWard(wardJumpPosition.To2D(), (ItemId)wardId);
                }
            }

        }

        public static int GetWardItem()
        {
            int[] wardItems = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
            foreach (var id in wardItems.Where(id => Items.HasItem(id) && Items.CanUseItem(id)))
                return id;
            return -1;
        }

        public static void PutWard(Vector2 pos, ItemId warditem)
        {

            foreach (var slot in Player.InventoryItems.Where(slot => slot.Id == warditem))
            {
                ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, pos.To3D());
                return;
            }
        }
        #endregion
        //Calculating Damage
        static float CalculateDamage(AIHeroClient target)
        {
            double damage = 0d;
            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1);
                //Chat.Print("Q:" + Player.GetSpellDamage(target, SpellSlot.Q));
                //Chat.Print("Q2:" + Player.GetSpellDamage(target, SpellSlot.Q, 1));
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += Player.GetSpellDamage(target, SpellSlot.Q, 1);
                //Chat.Print("Q2:" + Player.GetSpellDamage(target, SpellSlot.Q, 1));
            }
            if (W.IsReady())
            {
                damage += W.GetDamage(target);
                //Chat.Print("W:" + W.GetDamage(target));
                //Chat.Print("TradW:" + Player.GetSpellDamage(target,SpellSlot.W));
            }
            if (E.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.E);
                //Chat.Print("E:" + Player.GetSpellDamage(target, SpellSlot.E));
            }
            if (R.IsReady() || (Player.GetSpell(R.Slot).State == SpellState.Surpressed && R.Level > 0))

            {
                damage += Player.GetSpellDamage(target, SpellSlot.R);
                //Chat.Print("R:" + Player.GetSpellDamage(target, SpellSlot.R));
            }
            if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > 0 && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                //Chat.Print("F:" + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite));
                damage -= target.HPRegenRate * 2.5;
            }
            return (float)damage;
        }

        #region Killsteal
        static int CanKill(AIHeroClient target, bool useq, bool usew, bool usee, bool usef)
        {
            double damage = 0;
            if (!useq && !usew && !usee && !usef)
                return 0;
            if (Q.IsReady() && useq)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
                if ((W.IsReady() && usew) || (E.IsReady() && usee))
                {
                    damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q, 1);
                }
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (W.IsReady() && usew)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady() && usee)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (damage >= target.Health)
            {
                return 1;
            }
            if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > 0 && !target.HasBuff("summonerdot") && !HasRBuff() && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                damage -= target.HPRegenRate * 2.5;
            }
            return damage >= target.Health ? 2 : 0;

        }

        private static void Killsteal()
        {
            foreach (AIHeroClient enemy in AllEnemy)
            {
                if (enemy == null || enemy.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (CanKill(enemy, false, _menu.Item("motion.katarina.killsteal.usew").GetValue<bool>(), false, false) == 1 && enemy.IsValidTarget(390))
                {
                    W.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, false, false, _menu.Item("motion.katarina.killsteal.usee").GetValue<bool>(), false) == 1 && enemy.IsValidTarget(700))
                {
                    E.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, _menu.Item("motion.katarina.killsteal.useq").GetValue<bool>(), false, false, false) == 1 && enemy.IsValidTarget(675))
                {
                    Q.Cast(enemy);
                    qTarget = enemy;
                    return;
                }
                int cankill = CanKill(enemy, _menu.Item("motion.katarina.killsteal.useq").GetValue<bool>(), _menu.Item("motion.katarina.killsteal.usew").GetValue<bool>(), _menu.Item("motion.katarina.killsteal.usee").GetValue<bool>(), _menu.Item("motion.katarina.killsteal.usef").GetValue<bool>());
                if ((cankill == 1 || cankill == 2) && enemy.IsValidTarget(Q.Range))
                {
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (Q.IsReady())
                        Q.Cast(enemy);
                    if (E.IsReady() && (W.IsReady() || qTarget != enemy))
                        E.Cast(enemy);
                    if (W.IsReady() && enemy.IsValidTarget(390) && qTarget != enemy)
                        W.Cast();
                    return;
                }
                //KS with Wardjump
                cankill = CanKill(enemy, true, false, false, _menu.Item("motion.katarina.killsteal.usef").GetValue<bool>());
                if (_menu.Item("motion.katarina.killsteal.wardjump").GetValue<bool>() && (cankill == 1 || cankill == 2) && enemy.IsValidTarget(1300) && Q.IsReady() && E.IsReady())
                {
                    WardJump(enemy.Position, false);
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (enemy.IsValidTarget(675))
                        Q.Cast(enemy);
                    return;
                }
            }
        }
        #endregion



        #region Lasthit

        private static void Lasthit()
        {

            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit || (_menu.Item("motion.katarina.performance.tickmanager").GetValue<bool>() && Utils.TickCount < tickValue))
                return;
            Obj_AI_Base[] sourroundingMinions;
            int tickCount = _menu.Item("motion.katarina.performance.ticks").GetValue<Slider>().Value;
            tickValue = Utils.TickCount + tickCount;
            if (_menu.Item("motion.katarina.lasthit.usew").GetValue<bool>() && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, 390).Take(3).ToArray();
                {
                    //Only Cast W when minion is not killable with Autoattacks
                    if (
                        sourroundingMinions.Any(
                            minion =>
                                !minion.IsDead && _orbwalker.GetTarget() != minion && (qMinion == null || minion != qMinion) &&
                                W.GetDamage(minion) > minion.Health &&
                                HealthPrediction.GetHealthPrediction(minion,
                                    (Player.CanAttack
                                        ? Game.Ping / 2
                                        : Orbwalking.LastAATick - Utils.GameTimeTickCount +
                                          (int)Player.AttackDelay * 1000) + 200 + (_menu.Item("motion.katarina.performance.tickmanager").GetValue<bool>() ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0))
                    {
                        W.Cast();
                    }
                }

            }
            if (_menu.Item("motion.katarina.lasthit.useq").GetValue<bool>() && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead && Q.GetDamage(minion) > minion.Health))
                {
                    Q.Cast(minion);
                    qMinion = minion;
                    break;
                }
            }
            if (_menu.Item("motion.katarina.lasthit.usee").GetValue<bool>() && E.IsReady() && (!_menu.Item("motion.katarina.lasthit.noenemiese").GetValue<bool>() || Player.GetEnemiesInRange(1000).Count == 0))
            {
                //Same Logic with W + not killable with W
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range).Take(_menu.Item("motion.katarina.performance.track").GetValue<Slider>().Value).ToArray();
                {
                    foreach (var minions in sourroundingMinions.Where(
                        minion =>
                            !minion.IsDead && _orbwalker.GetTarget() != minion && (qMinion == null || minion != qMinion) &&
                            E.GetDamage(minion) >= minion.Health &&
                            (!W.IsReady() || !_menu.Item("motion.katarina.lasthit.usew").GetValue<bool>() || Player.Position.Distance(minion.Position) > 390)
                            &&
                            HealthPrediction.GetHealthPrediction(minion,
                                (Player.CanAttack
                                    ? Game.Ping / 2
                                    : Orbwalking.LastAATick - Utils.GameTimeTickCount + (int)Player.AttackDelay * 1000) +
                                200 + (_menu.Item("motion.katarina.performance.tickmanager").GetValue<bool>() ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0
                            &&
                            !IsTurretPosition(Player.Position.Extend(minion.Position,
                                Player.Position.Distance(minion.Position) + 35))))
                    {
                        E.Cast(minions);
                        break;
                    }
                }
            }
        }
        #endregion

        #region LaneClear
        private static void LaneClear()
        {
            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            Obj_AI_Base[] sourroundingMinions;
            if (_menu.Item("motion.katarina.laneclear.usew").GetValue<bool>() && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5).ToArray();
                if (sourroundingMinions.GetLength(0) >= _menu.Item("motion.katarina.laneclear.minw").GetValue<Slider>().Value)
                {
                    int lasthittable = sourroundingMinions.Count(minion => W.GetDamage(minion) + (minion.HasBuff("katarinaqmark") ? Q.GetDamage(minion, 1) : 0) > minion.Health);
                    if (lasthittable >= _menu.Item("motion.katarina.laneclear.minwlasthit").GetValue<Slider>().Value)
                    {
                        W.Cast();
                    }
                }
            }
            if (_menu.Item("motion.katarina.laneclear.useq").GetValue<bool>() && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range - 5).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead))
                {
                    Q.Cast(minion);
                    break;
                }
            }
        }
        #endregion

        #region Jungleclear

        private static void JungleClear()
        {
            Obj_AI_Base[] sourroundingMinions;
            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (_menu.Item("motion.katarina.jungleclear.useq").GetValue<bool>() && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    Q.Cast(sourroundingMinions[chosenminion]);
                }
            }
            if (_menu.Item("motion.katarina.jungleclear.usew").GetValue<bool>() && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5, MinionTypes.All, MinionTeam.Neutral).ToArray();
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    W.Cast();
                }
            }
            if (_menu.Item("motion.katarina.jungleclear.usee").GetValue<bool>() && E.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    E.Cast(sourroundingMinions[chosenminion]);
                }
            }
        }
        #endregion
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && HasRBuff() && Utils.GameTimeTickCount <= whenToCancelR && _menu.Item("motion.katarina.misc.noRCancel").GetValue<bool>())
                args.Process = false;
        }

    }
}