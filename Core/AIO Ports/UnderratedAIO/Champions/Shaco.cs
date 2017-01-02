using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
using LeagueSharp.Common; 
namespace UnderratedAIO.Champions
{
    internal class Shaco
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static bool hasGhost = false;
        public static bool GhostDelay = false;
        public static int GhostRange = 2200;
        


        public Shaco()
        {
            InitShaco();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Shaco</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            
            Helpers.Jungle.setSmiteSlot();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(
                Q.Range + player.MoveSpeed * 3, TargetSelector.DamageType.Physical);
            if (ShacoStealth && target != null && target.Health > ComboDamage(target) &&
                CombatHelper.IsFacing(target, player.Position) &&
                orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                orbwalker.SetAttack(false);
            }
            else
            {
                orbwalker.SetAttack(true);
            }
            if (false)
            {
                return;
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
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
            if (E.IsReady())
            {
                var ksTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        h => h.IsValidTarget() && !CombatHelper.IsInvulnerable2(h) && h.Health < E.GetDamage(h));
                if (ksTarget != null)
                {
                    if ((config.Item("ks", true).GetValue<bool>() || config.Item("ksq", true).GetValue<bool>()) &&
                        E.CanCast(ksTarget))
                    {
                        E.CastOnUnit(ksTarget);
                    }
                    if (Q.IsReady() && config.Item("ks", true).GetValue<bool>() &&
                        ksTarget.Distance(player) < Q.Range + E.Range && ksTarget.Distance(player) > E.Range &&
                        !player.Position.Extend(ksTarget.Position, Q.Range).IsWall() &&
                        player.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana)
                    {
                        Q.Cast(player.Position.Extend(ksTarget.Position, Q.Range));
                    }
                }
            }

            if (config.Item("stackBox", true).GetValue<KeyBind>().Active && W.IsReady())
            {
                var box =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.Distance(player) < W.Range && m.Name == "Jack In The Box" && !m.IsDead)
                        .OrderBy(m => m.Distance(Game.CursorPos))
                        .FirstOrDefault();

                if (box != null)
                {
                    W.Cast(box.Position);
                }
                else
                {
                    if (player.Distance(Game.CursorPos) < W.Range)
                    {
                        W.Cast(Game.CursorPos);
                    }
                    else
                    {
                        W.Cast(player.Position.Extend(Game.CursorPos, W.Range));
                    }
                }
            }
            if (R.IsReady() && ShacoClone)
            {
                PetHandler.MovePet(config, orbwalker.ActiveMode);
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (config.Item("userCC", true).GetValue<bool>() && R.IsReady() && target != null &&
                player.Distance(target) < Q.Range && data.AnyCC)
            {
                R.Cast();
            }
        }

        private void Combo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            var cmbDmg = ComboDamage(target);
            float dist = (float) (Q.Range + player.MoveSpeed * 2.5);
            if ((config.Item("WaitForStealth", true).GetValue<bool>() && ShacoStealth && cmbDmg < target.Health) ||
                !Orbwalking.CanMove(100))
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, cmbDmg);
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.IsReady() &&
                Game.CursorPos.Distance(target.Position) < 250 && target.Distance(player) < dist &&
                (target.Distance(player) >= config.Item("useqMin", true).GetValue<Slider>().Value ||
                 (cmbDmg > target.Health && player.CountEnemiesInRange(2000) == 1)))
            {
                if (target.Distance(player) < Q.Range)
                {
                    Q.Cast(Prediction.GetPrediction(target, 0.5f).UnitPosition);
                }
                else
                {
                    if (!CheckWalls(target) || Environment.Map.GetPath(player, target.Position) < dist)
                    {
                        Q.Cast(player.Position.Extend(target.Position, Q.Range));
                    }
                }
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady() && !target.UnderTurret(true) &&
                target.Health > cmbDmg && player.Distance(target) < W.Range)
            {
                HandleW(target);
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target))
            {
                E.CastOnUnit(target);
            }
            if (config.Item("user", true).GetValue<bool>() && R.IsReady() && !ShacoClone && target.HealthPercent < 75 &&
                cmbDmg < target.Health && target.HealthPercent > cmbDmg && target.HealthPercent > 25)
            {
                R.Cast();
            }
            if (config.Item("useIgnite").GetValue<bool>() &&
                player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health && hasIgnite)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }


        private bool CheckWalls(AIHeroClient target)
        {
            var step = player.Distance(target) / 15;
            for (int i = 1; i < 16; i++)
            {
                if (player.Position.Extend(target.Position, step * i).IsWall())
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleW(AIHeroClient target)
        {
            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .OrderByDescending(t => t.Distance(target))
                    .FirstOrDefault(t => t.IsEnemy && t.Distance(target) < 3000 && !t.IsDead);
            if (turret != null)
            {
                CastW(target, target.Position, turret.Position);
            }
            else
            {
                if (target.IsMoving)
                {
                    var pred = Prediction.GetPrediction(target, 2);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        CastW(target, target.Position, pred.UnitPosition);
                    }
                }
                else
                {
                    W.Cast(player.Position.Extend(target.Position, W.Range - player.Distance(target)));
                }
            }
        }

        private void CastW(AIHeroClient target, Vector3 from, Vector3 to)
        {
            var positions = new List<Vector3>();

            for (int i = 1; i < 11; i++)
            {
                positions.Add(from.Extend(to, 42 * i));
            }
            var best =
                positions.OrderByDescending(p => p.Distance(target.Position))
                    .FirstOrDefault(
                        p => !p.IsWall() && p.Distance(player.Position) < W.Range && p.Distance(target.Position) > 350);
            if (best != null && best.IsValid())
            {
                W.Cast(best);
            }
        }

        private static bool ShacoClone
        {
            get { return player.Spellbook.GetSpell(SpellSlot.R).Name == "HallucinateGuide"; }
        }

        private static bool ShacoStealth
        {
            get { return player.HasBuff("Deceive"); }
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target))
            {
                E.Cast(target);
            }
        }

        private void Clear()
        {
            MinionManager.FarmLocation bestPosition =
                W.GetCircularFarmLocation(MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly), 300);
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady() &&
                bestPosition.MinionsHit > config.Item("whitLC", true).GetValue<Slider>().Value)
            {
                W.Cast(bestPosition.Position);
            }
            var mob = Jungle.GetNearest(player.Position);

            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady() && mob != null &&
                mob.Health < E.GetDamage(mob))
            {
                E.Cast(mob);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;

            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
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

        private void InitShaco()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 425);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R);
        }

        private void InitMenu()
        {
            config = new Menu("Shaco", "Shaco", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqMin", "   Min range", true).SetValue(new Slider(200, 0, 400)));
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userCC", "   Dodge targeted CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("WaitForStealth", "Block spells in stealth", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("whitLC", "   Min mob", true).SetValue(new Slider(2, 1, 5)));
            menuLC.AddItem(new MenuItem("useeLC", "Use E to secure buff", true)).SetValue(true);
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("ksq", "KS E", true)).SetValue(true);
            menuM.AddItem(new MenuItem("ks", "KS Q+E", true)).SetValue(true);
            menuM.AddItem(new MenuItem("stackBox", "Stack boxes", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}