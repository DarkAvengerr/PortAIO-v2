using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Fiora
    {
        private static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static AutoLeveler autoLeveler;
        public List<PassiveManager> passives = new List<PassiveManager>();
        public float Qradius = 175f;

        public Fiora()
        {
            InitFiora();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Fiora</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnSpellCast += Game_ProcessSpell;
            Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var passiveType = GetPassive(sender.Name);
            if (passiveType != PassiveType.NULL)
            {
                var enemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(sender.Position) < 50)
                        .OrderBy(e => sender.Position.Distance(e.Position))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                PassiveManager temp = new PassiveManager(enemy);
                var alreadyAdded = passives.FirstOrDefault(p => p.Enemy.NetworkId == enemy.NetworkId);
                if (alreadyAdded != null)
                {
                    alreadyAdded.passives.Add(new Passive(passiveType, System.Environment.TickCount));
                    //Console.WriteLine("Updated: " + sender.Name);
                }
                else
                {
                    temp.passives.Add(new Passive(passiveType, System.Environment.TickCount));
                    passives.Add(temp);
                    //Console.WriteLine("NewAdded: " + sender.Name);
                }
            }
        }


        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var passiveType = GetPassive(sender.Name);
            if (passiveType != PassiveType.NULL)
            {
                var enemy = HeroManager.Enemies.OrderBy(e => sender.Position.Distance(e.Position)).FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var deleted = passives.FirstOrDefault(p => p.Enemy.NetworkId == enemy.NetworkId);
                if (deleted != null)
                {
                    for (int i = 0; i < deleted.passives.Count; i++)
                    {
                        if (deleted.passives[i].Type == passiveType)
                        {
                            deleted.passives.RemoveAt(i);
                        }
                    }
                }
                //Console.WriteLine("Deleted: " + sender.Name);
            }
        }


        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            orbwalker.SetMovement(true);
            ClearList();
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (data != null && W.IsReady() && config.Item("autoW", true).GetValue<bool>() &&
                config.Item("minmanaP", true).GetValue<Slider>().Value < player.ManaPercent)
            {
                AIHeroClient enemy = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (enemy != null && data.ProjectileDamageTaken >= enemy.GetAutoAttackDamage(player) - 5)
                {
                    W.Cast(enemy);
                }
            }
        }

        private void ClearList()
        {
            foreach (var passive in passives)
            {
                for (int i = 0; i < passive.passives.Count; i++)
                {
                    if (System.Environment.TickCount - passive.passives[i].time > 15000)
                    {
                        passive.passives.RemoveAt(i);
                    }
                }
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit targetO)
        {
            if (!(targetO is AIHeroClient))
            {
                return;
            }
            AIHeroClient targ = (AIHeroClient) targetO;
            List<Vector3> passivePositions = GetPassivePositions(targetO);
            bool rapid = player.GetAutoAttackDamage(targ) * 3 + ComboDamage(targ) > targ.Health ||
                         (player.Health < targ.Health && player.Health < player.MaxHealth / 2);
            if (unit.IsMe && E.IsReady() && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                (config.Item("usee", true).GetValue<bool>() ||
                 (unit.IsMe && config.Item("RapidAttack", true).GetValue<KeyBind>().Active || rapid)) &&
                !Orbwalking.CanAttack())
            {
                E.Cast();
            }
            if (unit.IsMe && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Q.IsReady() &&
                (config.Item("RapidAttack", true).GetValue<KeyBind>().Active || rapid) && !Orbwalking.CanAttack() &&
                passivePositions.Any())
            {
                var passive = GetClosestPassivePosition(targ);
                var pos = GetQpoint(targ, passive);
                if (pos.IsValid())
                {
                    Q.Cast(pos);
                }
                else
                {
                    var pos2 = GetQpoint(targ, Prediction.GetPrediction(targ, 2).UnitPosition);
                    if (pos2.IsValid())
                    {
                        Q.Cast(pos2);
                    }
                }
            }
            if (unit.IsMe)
            {
                var pos = GetClosestPassivePosition(targetO);
                AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && targetO.NetworkId == target.NetworkId &&
                    R.IsReady() && R.CanCast(target) &&
                    HealthPrediction.GetHealthPrediction(target, 1000) > player.GetAutoAttackDamage(target) &&
                    ComboDamage(target) + player.GetAutoAttackDamage(target) * 5 > target.Health &&
                    ((config.Item("userally", true).GetValue<Slider>().Value <=
                      HeroManager.Allies.Count(
                          a => a.IsValid && !a.IsDead && a.Distance(target) < 600 && a.HealthPercent < 90) &&
                      config.Item("usertf", true).GetValue<bool>()) ||
                     (player.HealthPercent < 75 && config.Item("user", true).GetValue<bool>())))
                {
                    R.CastOnUnit(target);
                }
            }
        }

        private bool CheckQusage(Vector3 pos, AIHeroClient target)
        {
            return pos.IsValid() && pos.Distance(player.Position) < Q.Range &&
                   (target.HasBuff("fiorapassivemanager") || target.HasBuff("fiorarmark")) && !pos.IsWall() &&
                   Qradius > pos.Distance(target.Position);
        }

        private List<Vector3> GetPassivePositions(AttackableUnit target)
        {
            List<Vector3> temp = new List<Vector3>();
            var query = passives.FirstOrDefault(t => t.Enemy.NetworkId == target.NetworkId);
            if (query != null)
            {
                temp = query.getPositions();
            }
            return temp;
        }

        private Vector3 GetClosestPassivePosition(AttackableUnit target)
        {
            List<Vector3> temp = GetPassivePositions(target);
            return temp.OrderBy(p => p.Distance(player.Position)).FirstOrDefault();
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (config.Item("usewCC", true).GetValue<bool>() && W.IsReady() && data.AnyCC)
            {
                W.Cast(target.Position);
            }
            var closestPassive = GetClosestPassivePosition(target);
            if (closestPassive.IsValid() && config.Item("MoveToVitals", true).GetValue<bool>() &&
                Orbwalking.CanMove(300) && !Orbwalking.CanAttack() && !player.Spellbook.IsAutoAttacking &&
                Game.CursorPos.Distance(target.Position) < 350)
            {
                orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(
                    GameObjectOrder.MoveTo,
                    target.Position.Extend(closestPassive, Math.Max(player.BoundingRadius + target.BoundingRadius, 100)));
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useq", true).GetValue<bool>() && Q.IsReady() && Orbwalking.CanMove(100) &&
                config.Item("useqMin", true).GetValue<Slider>().Value <= player.Distance(target) &&
                (closestPassive.IsValid() || (target.HealthPercent < 30)) && !player.Spellbook.IsAutoAttacking)
            {
                var pos = GetQpoint(target, closestPassive);
                if (pos.IsValid())
                {
                    Q.Cast(pos);
                }
                else if (target.HealthPercent < 30)
                {
                    if (
                        CheckQusage(
                            target.Position.Extend(
                                Prediction.GetPrediction(target, player.Distance(target) / 1600).UnitPosition, Qradius),
                            target))
                    {
                        Q.Cast(
                            target.Position.Extend(
                                Prediction.GetPrediction(target, player.Distance(target) / 1600).UnitPosition, Qradius));
                    }
                }
            }
            if (W.IsReady() && config.Item("usew", true).GetValue<bool>())
            {
                var killable = (target.Distance(player) > 350f && W.GetDamage(target) > target.Health);
                var incAA = data.ProjectileDamageTaken >= target.GetAutoAttackDamage(player) - 5;
                var dangerous = incAA && data.DamageTaken >= player.Health * 0.4f;
                if (killable || (incAA && !config.Item("usewDangerous", true).GetValue<bool>()) ||
                    (config.Item("usewDangerous", true).GetValue<bool>() && dangerous))
                {
                    W.CastIfHitchanceEquals(target, HitChance.Low);
                }
            }
            if (config.Item("useIgnite").GetValue<bool>() && hasIgnite && ComboDamage(target) > target.Health &&
                !Q.IsReady() &&
                (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) || player.HealthPercent < 15))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }


        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (args == null || hero == null)
            {
                return;
            }
            AIHeroClient targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            var spellName = args.SData.Name;
            AIHeroClient target = args.Target as AIHeroClient;
            if (targetW != null)
            {
                hero = targetW;
            }
            if (hero.IsMe)
            {
                if (args.SData.Name == "FioraE")
                {
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        private void Clear()
        {
            float perc = (float) config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }

            MinionManager.FarmLocation bestPositionW =
                W.GetLineFarmLocation(
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (config.Item("usewLC", true).GetValue<bool>() &&
                bestPositionW.MinionsHit >= config.Item("wMinHit", true).GetValue<Slider>().Value)
            {
                W.Cast(bestPositionW.Position);
            }
            if (config.Item("useeLC", true).GetValue<bool>() &&
                Environment.Minion.countMinionsInrange(player.Position, Q.Range) >= 2)
            {
                E.Cast();
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
            return;
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var pas = GetQpoint(target, GetClosestPassivePosition(target));
            if (pas.IsValid())
            {
                Render.Circle.DrawCircle(pas, 100, Color.BlueViolet, 7);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.W);
            }
            if (R.IsReady())
            {
                damage += GetPassiveDamage(hero, 4);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitFiora()
        {
            Q = new Spell(SpellSlot.Q, 400f);
            Q.SetSkillshot(0.25f, 50f, 1600f, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 750f);
            W.SetSkillshot(0.75f, 80, 2000f, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500);
        }

        private PassiveType GetPassive(string name)
        {
            switch (name)
            {
                case "Fiora_Base_Passive_SW.troy":
                    return PassiveType.SW;

                case "Fiora_Base_Passive_SE.troy":
                    return PassiveType.SE;

                case "Fiora_Base_Passive_NW.troy":
                    return PassiveType.NW;

                case "Fiora_Base_Passive_NE.troy":
                    return PassiveType.NE;

                case "Fiora_Base_R_Mark_SW_FioraOnly.troy":
                    return PassiveType.SW;

                case "Fiora_Base_R_Mark_SE_FioraOnly.troy":
                    return PassiveType.SE;

                case "Fiora_Base_R_Mark_NW_FioraOnly.troy":
                    return PassiveType.NW;

                case "Fiora_Base_R_Mark_NE_FioraOnly.troy":
                    return PassiveType.NE;
            }
            return PassiveType.NULL;
        }

        public Vector3 GetQpoint(AIHeroClient target, Vector3 passive)
        {
            var ponts = new List<Vector3>();
            var predEnemy = Prediction.GetPrediction(target, ObjectManager.Player.Distance(target) / 1600).UnitPosition;
            for (int i = 2; i < 8; i++)
            {
                ponts.Add(predEnemy.To2D().Extend(passive.To2D(), i * 25).To3D());
            }

            return
                ponts.Where(p => CheckQusage(p, target))
                    .OrderByDescending(p => p.Distance(target.Position))
                    .FirstOrDefault();
        }

        public static double GetPassiveDamage(Obj_AI_Base target, int passives)
        {
            return passives * (0.03f + 0.027 + 0.001f * player.Level * player.FlatPhysicalDamageMod / 100f) *
                   target.MaxHealth;
        }

        private void InitMenu()
        {
            config = new Menu("Fiora", "Fiora", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);

            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);

            // Draw settings
            Menu menuD = new Menu("Drawings ", "dsettings");
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 58, 100, 150)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 58, 100, 150)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 58, 100, 150)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqMin", "  Min distance", true)).SetValue(new Slider(250, 0, 400));
            menuC.AddItem(new MenuItem("usew", "Use W AA", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewDangerous", "   Only on low health", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewCC", "W to CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "R 1v1", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usertf", "R teamfight", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userally", "  Min allies", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("RapidAttack", "Fast AA Combo", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("MoveToVitals", "Move to vitals", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("wMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("autoW", "Auto W AA", true)).SetValue(true);
            menuM.AddItem(new MenuItem("minmanaP", "Min mana percent", true)).SetValue(new Slider(1, 1, 100));
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }

    internal class PassiveManager
    {
        public AIHeroClient Enemy;
        public List<Passive> passives = new List<Passive>();

        public PassiveManager(AIHeroClient enemy)
        {
            Enemy = enemy;
        }

        public List<Vector3> getPositions()
        {
            List<Vector3> list = new List<Vector3>();
            var predEnemy = Prediction.GetPrediction(Enemy, ObjectManager.Player.Distance(Enemy) / 1600).UnitPosition;
            foreach (var passive in passives)
            {
                switch (passive.Type)
                {
                    case PassiveType.NE:
                        list.Add(new Vector2(predEnemy.X, predEnemy.Y + 100).To3D());
                        break;
                    case PassiveType.NW:
                        list.Add(new Vector2(predEnemy.X + 100, predEnemy.Y).To3D());
                        break;
                    case PassiveType.SW:
                        list.Add(new Vector2(predEnemy.X, predEnemy.Y - 100).To3D());
                        break;
                    case PassiveType.SE:
                        list.Add(new Vector2(predEnemy.X - 100, predEnemy.Y).To3D());
                        break;
                }
            }
            return list;
        }
    }

    public enum PassiveType
    {
        SW,
        SE,
        NW,
        NE,
        NULL
    };

    internal class Passive
    {
        public PassiveType Type;
        public float time;

        public Passive(PassiveType getPassive, int tickCount)
        {
            Type = getPassive;
            time = tickCount;
        }
    }
}