using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Sion
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, useIgnite, justE;
        public static float qStart;
        public Vector3 lastQPos;
        public const int qWidth = 350;
        public double[] Rwave = new double[] { 50, 70, 90 };

        public Sion()
        {
            InitSion();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Sion</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Game.OnProcessPacket += Game_OnProcessPacket;
            Game.OnWndProc += Game_OnWndProc;
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint) WindowsMessages.WM_RBUTTONDOWN && Q.IsCharging)
            {
                Q.Cast(Game.CursorPos);
            }
        }

        private void Game_OnProcessPacket(GamePacketEventArgs args)
        {
            //Packet info stolen from Trees
            if (config.Item("NoRlock", true).GetValue<bool>() && args.PacketData[0] == 0x83 &&
                args.PacketData[7] == 0x47 && args.PacketData[8] == 0x47)
            {
                args.Process = false;
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("usewgc", true).GetValue<bool>() && gapcloser.End.Distance(player.Position) < 200)
            {
                W.Cast();
            }
        }

        private void InitSion()
        {
            Q = new Spell(SpellSlot.Q, 740);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("SionQ", "SionQ", 350, 740, 0.6f);
            W = new Spell(SpellSlot.W, 490);
            E = new Spell(SpellSlot.E, 775);
            E.SetSkillshot(0.25f, 80f, 1800, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, float.MaxValue);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsCharging || activatedR)
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
            }
            else
            {
                orbwalker.SetAttack(true);
                orbwalker.SetMovement(true);
            }
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
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
            if (data != null && !activatedW && config.Item("AshieldB", true).GetValue<bool>() &&
                data.DamageCount >= config.Item("wMinAggro", true).GetValue<Slider>().Value &&
                player.ManaPercent > config.Item("minmanaAgg", true).GetValue<Slider>().Value)
            {
                W.Cast();
            }
            if (data != null && !activatedW && config.Item("AshieldB", true).GetValue<bool>() && W.IsReady() &&
                (data.DamageTaken > player.Health ||
                 data.DamageTaken > getWShield() / 100f * config.Item("AshieldDmg", true).GetValue<Slider>().Value))
            {
                W.Cast();
            }
            if (data != null && activatedW && data.DamageTaken > player.GetBuff("sionwshieldstacks").Count &&
                data.DamageTaken < player.Health)
            {
                W.Cast();
            }
        }


        private void Harass()
        {
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            AIHeroClient target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical, true);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }
            if (Q.IsReady() && config.Item("useqH", true).GetValue<bool>())
            {
                castQ(target);
            }
            if (config.Item("useeH", true).GetValue<bool>())
            {
                CastEHero(target);
            }
        }

        private void castQ(AIHeroClient target)
        {
            if (Q.IsCharging)
            {
                checkCastedQ(target);
                return;
            }
            else if (Q.CanCast(target) && !player.Spellbook.IsAutoAttacking)
            {
                var qPred = Prediction.GetPrediction(target, 0.3f);
                var qPred2 = Prediction.GetPrediction(target, 0.6f);
                var poly = GetPoly(qPred.UnitPosition);
                if (qPred2.Hitchance >= HitChance.High && poly.IsInside(qPred2.UnitPosition.To2D()) &&
                    poly.IsInside(target.ServerPosition))
                {
                    Q.StartCharging(qPred.CastPosition);
                    return;
                }
            }
        }

        private static bool activatedR
        {
            get { return player.HasBuff("SionR"); }
        }

        private static bool activatedW
        {
            get { return player.Spellbook.GetSpell(SpellSlot.W).Name == "sionwdetonate"; }
        }

        private static bool activatedP
        {
            get { return player.Spellbook.GetSpell(SpellSlot.Q).Name == "sionpassivespeed"; }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionQ = Q.GetLineFarmLocation(minions, qWidth);

                if (bestPositionQ.MinionsHit >= config.Item("qMinHit", true).GetValue<Slider>().Value && !Q.IsCharging)
                {
                    Q.StartCharging(bestPositionQ.Position.To3D());
                    return;
                }
                if (Q.IsCharging && minions.Count(m => HealthPrediction.GetHealthPrediction(m, 500) < 0) > 0)
                {
                    var qMini = minions.FirstOrDefault();
                    if (qMini != null)
                    {
                        Q.Cast(qMini.Position);
                    }
                }
            }

            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady() && !Q.IsCharging)
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionE = E.GetLineFarmLocation(minions);
                if (bestPositionE.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value)
                {
                    E.Cast(bestPositionE.Position);
                    return;
                }
            }
            if (W.IsReady() && !activatedW && config.Item("usewLC", true).GetValue<bool>() &&
                config.Item("wMinHit", true).GetValue<Slider>().Value <=
                Environment.Minion.countMinionsInrange(player.Position, W.Range))
            {
                W.Cast();
            }
            if (W.IsReady() && activatedW && config.Item("usewLC", true).GetValue<bool>() &&
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Count(m => HealthPrediction.GetHealthPrediction(m, 500) < 0) > 0)
            {
                W.Cast();
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical, true);
            if (config.Item("user", true).GetValue<bool>() && R.IsReady())
            {
                var rTarget = TargetSelector.GetTarget(2500, TargetSelector.DamageType.Physical, true);
                if (!activatedR && !player.Spellbook.IsAutoAttacking)
                {
                    if (rTarget != null && !rTarget.IsInvulnerable && !rTarget.MagicImmune &&
                        rTarget.Distance(Game.CursorPos) < 300)
                    {
                        if (player.Distance(rTarget) + 100 > Environment.Map.GetPath(player, rTarget.Position) &&
                            (ComboDamage(rTarget) > rTarget.Health &&
                             !CombatHelper.IsCollidingWith(
                                 player, rTarget.Position.Extend(player.Position, player.BoundingRadius + 15),
                                 player.BoundingRadius,
                                 new[] { CollisionableObjects.Heroes, CollisionableObjects.Walls }) &&
                             (ComboDamage(rTarget) - R.GetDamage(rTarget) < rTarget.Health ||
                              rTarget.Distance(player) > 400 || player.HealthPercent < 25) &&
                             rTarget.CountAlliesInRange(2500) + 1 >= rTarget.CountEnemiesInRange(2500)))
                        {
                            R.Cast(target.Position);
                        }
                    }
                }
            }
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (!activatedW && W.IsReady() && config.Item("usew", true).GetValue<bool>())
            {
                if (data.DamageTaken > player.Health ||
                    (data.DamageTaken > getWShield() / 100 * config.Item("shieldDmg", true).GetValue<Slider>().Value) ||
                    (target.Distance(player) < W.Range && config.Item("usewir", true).GetValue<bool>()))
                {
                    W.Cast();
                }
            }
            if (activatedW && config.Item("usew", true).GetValue<bool>() && W.IsReady() &&
                player.Distance(target) < W.Range &&
                (target.Health < W.GetDamage(target) ||
                 (W.IsInRange(target) && !W.IsInRange(Prediction.GetPrediction(target, 0.2f).UnitPosition))))
            {
                W.Cast();
            }
            var comboDmg = ComboDamage(target);
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (activatedP)
            {
                if (Q.IsReady() && player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                    Q.Cast();
                }
                return;
            }
            if (Q.IsCharging)
            {
                checkCastedQ(target);
                return;
            }
            if (activatedR)
            {
                return;
            }
            if (config.Item("usee", true).GetValue<bool>() && E.IsReady() && !player.Spellbook.IsAutoAttacking)
            {
                CastEHero(target);
                return;
            }
            if (config.Item("useq", true).GetValue<bool>() && !player.Spellbook.IsAutoAttacking)
            {
                castQ(target);
            }
            if (!activatedW && W.IsReady() && data.AnyCC)
            {
                W.Cast();
            }

            if (config.Item("userCC", true).GetValue<bool>() && player.Distance(target) < Q.Range &&
                HeroManager.Enemies.FirstOrDefault(e => e.Distance(Game.CursorPos) < 300) != null && data.AnyCC)
            {
                R.Cast(Game.CursorPos);
            }
        }

        private void checkCastedQ(Obj_AI_Base target)
        {
            if ((justQ && target.Distance(player) > Q.Range) || !target.IsValidTarget())
            {
                return;
            }
            var poly = GetPoly(lastQPos);
            var heroes = HeroManager.Enemies.Where(e => poly.IsInside(e.Position) && e.IsValidTarget());
            var objAiHeroes = heroes as IList<AIHeroClient> ?? heroes.ToList();
            if (objAiHeroes.Any())
            {
                var escaping =
                    objAiHeroes.Count(h => poly.IsOutside(Prediction.GetPrediction(h, 0.2f).UnitPosition.To2D()));
                var data = Program.IncDamages.GetAllyData(player.NetworkId);
                if ((escaping > 0 &&
                     (objAiHeroes.Count() == 1 ||
                      (objAiHeroes.Count() >= 2 && System.Environment.TickCount - qStart > 1000))) ||
                    data.DamageTaken > player.Health || Program.IncDamages.GetAllyData(player.NetworkId).AnyCC ||
                    Program.IncDamages.GetEnemyData(target.NetworkId).DamageTaken > target.Health)
                {
                    Q.Cast(target.Position, true);
                }
            }
        }

        private Geometry.Polygon GetPoly(Vector3 pos)
        {
            var POS = player.ServerPosition.Extend(pos, Q.ChargedMaxRange);
            var direction = (POS.To2D() - player.ServerPosition.To2D()).Normalized();

            var pos1 = (player.ServerPosition.To2D() - direction.Perpendicular() * qWidth / 2f).To3D();

            var pos2 =
                (POS.To2D() + (POS.To2D() - player.ServerPosition.To2D()).Normalized() +
                 direction.Perpendicular() * qWidth / 2f).To3D();

            var pos3 = (player.ServerPosition.To2D() + direction.Perpendicular() * qWidth / 2f).To3D();

            var pos4 =
                (POS.To2D() + (POS.To2D() - player.ServerPosition.To2D()).Normalized() -
                 direction.Perpendicular() * qWidth / 2f).To3D();
            var poly = new Geometry.Polygon();
            poly.Add(pos1);
            poly.Add(pos3);
            poly.Add(pos2);
            poly.Add(pos4);
            return poly;
        }

        private void CastEHero(AIHeroClient target)
        {
            if (E.CanCast(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.High);
                return;
            }
            var pred = Prediction.GetPrediction(
                target, player.ServerPosition.Distance(target.ServerPosition) / E.Speed * 1000);
            if (pred.UnitPosition.Distance(player.Position) > 1400 || pred.Hitchance < HitChance.High)
            {
                return;
            }
            var collision = E.GetCollision(player.Position.To2D(), new List<Vector2>() { pred.CastPosition.To2D() });
            if (collision.Any(c => c.Distance(player) < E.Range) &&
                !CombatHelper.IsCollidingWith(
                    player, pred.CastPosition.Extend(player.Position, W.Width + 15), E.Width,
                    new[] { CollisionableObjects.Heroes, CollisionableObjects.Walls }))
            {
                E.Cast(pred.CastPosition);
                return;
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (activatedP)
            {
                return (float) player.GetAutoAttackDamage(hero, true);
            }
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q) * 2f;
            }
            if (W.IsReady() || activatedW)
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * hero.Distance(player) > 1000 ? 2f : 1.3f;
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static double getWShield()
        {
            var shield = new double[] { 30, 55, 80, 105, 130 }[W.Level - 1] + 0.1f * player.MaxHealth +
                         0.4f * player.FlatMagicDamageMod;
            return shield;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "SionQ")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        qStart = System.Environment.TickCount;
                        lastQPos = player.Position.Extend(args.End, Q.Range);
                        LeagueSharp.Common.Utility.DelayAction.Add(600, () => justQ = false);
                    }
                }
                if (args.SData.Name == "SionE")
                {
                    if (!justE)
                    {
                        justE = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(400, () => justE = false);
                    }
                }
            }
        }

        private void InitMenu()
        {
            config = new Menu("Sion ", "Sion", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewir", "   In range", true)).SetValue(true);
            menuC.AddItem(new MenuItem("shieldDmg", "   Min dmg in shield %", true)).SetValue(new Slider(50, 1, 100));
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userCC", "Use R before CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("wMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("usewgc", "Use W gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("AshieldB", "Auto W", true)).SetValue(false);
            menuM.AddItem(new MenuItem("wMinAggro", "   On aggro", true)).SetValue(new Slider(3, 1, 8));
            menuM.AddItem(new MenuItem("AshieldDmg", "   Min dmg in shield %", true)).SetValue(new Slider(100, 1, 100));
            menuM.AddItem(new MenuItem("minmanaAgg", "   Min mana", true)).SetValue(new Slider(50, 1, 100));
            menuM.AddItem(new MenuItem("NoRlock", "Disable camera lock", true)).SetValue(false);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}