using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SPrediction;
using UnderratedAIO.Helpers;
using UnderratedAIO.Helpers.SkillShot;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using Prediction = LeagueSharp.Common.Prediction;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Olaf
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static Vector3 lastQpos, lastQCast;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Olaf()
        {
            InitOlaf();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Olaf</font>");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Helpers.Jungle.setSmiteSlot();
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Console.WriteLine(Game.IP);
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_axe_totem_team_id_green.troy") &&
                lastQCast.Distance(sender.Position) < 700)
            {
                lastQpos = sender.Position;
            }
        }


        private void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_axe_totem_team_id_green.troy") &&
                lastQpos.Distance(sender.Position) < 150)
            {
                lastQpos = Vector3.Zero;
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Slot == SpellSlot.Q)
            {
                var pos = player.Distance(args.End) > 400 ? args.End : player.Position.Extend(args.End, 400);
                lastQCast = Environment.Map.ClosestWall(player.Position, pos);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (lastQpos.IsValid() && config.Item("drawaxe", true).GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    lastQpos, config.Item("gotoAxeMaxDist", true).GetValue<Slider>().Value, Color.Cyan, 5);
            }
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            orbwalker.SetOrbwalkingPoint(Vector3.Zero);
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
        }

        private void Clear()
        {
            var minis = MinionManager.GetMinions(325f, MinionTypes.All, MinionTeam.NotAlly);
            var killableWithE =
                minis.Where(m => HealthPrediction.GetHealthPrediction(m, 245) < E.GetDamage(m))
                    .OrderByDescending(m => m.MaxHealth)
                    .FirstOrDefault();
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady() && killableWithE != null &&
                (!player.Spellbook.IsAutoAttacking || killableWithE.MaxHealth > 2000))
            {
                E.Cast(killableWithE);
            }
            if (config.Item("gotoAxeLC", true).GetValue<bool>())
            {
                GotoAxe(Game.CursorPos);
            }
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady())
            {
                var minisForQ = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                var farmLocation = Q.GetLineFarmLocation(minisForQ);
                Obj_AI_Base targetMini = null;
                if (config.Item("qMinHit", true).GetValue<Slider>().Value <= farmLocation.MinionsHit)
                {
                    targetMini =
                        minisForQ.Where(m => Q.CountHits(minisForQ, m.Position) >= farmLocation.MinionsHit)
                            .OrderBy(m => m.Distance(player))
                            .FirstOrDefault();
                }
                if (targetMini == null)
                {
                    targetMini =
                        minisForQ.Where(m => minisForQ.Where(b => b.Distance(m) < Q.Width).Sum(b => b.Health) > 700)
                            .OrderByDescending(b => Q.CountHits(minisForQ, b.Position))
                            .FirstOrDefault();
                }
                if (targetMini != null)
                {
                    Q.Cast(targetMini.Position);
                }
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady())
            {
                if (minis.Sum(m => m.Health) > 750 || minis.Count > 3 || player.HealthPercent < 50)
                {
                    W.Cast();
                }
            }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);
            if (config.Item("useeH", true).GetValue<bool>() && target != null && E.CanCast(target) &&
                !player.Spellbook.IsAutoAttacking && Orbwalking.CanMove(100))
            {
                E.Cast(target);
            }
            if (config.Item("gotoAxeH", true).GetValue<bool>() && target != null)
            {
                GotoAxe(target.Position);
            }
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking || target == null || !Orbwalking.CanMove(100))
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.IsReady())
            {
                CastQ(target);
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);
            if (player.Spellbook.IsAutoAttacking || target == null || !Orbwalking.CanMove(100))
            {
                return;
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target))
            {
                CastQ(target);
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target) &&
                (((E.GetDamage(target) > target.Health) || player.HealthPercent > 25) ||
                 Program.IncDamages.GetAllyData(player.NetworkId).IsAboutToDie))
            {
                E.Cast(target);
            }
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady() &&
                player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) + 50)
            {
                W.Cast();
            }
            if (config.Item("userCCed", true).GetValue<bool>() && R.IsReady() && CombatHelper.IsCCed(player))
            {
                R.Cast();
            }
            if (config.Item("userbeforeCCed", true).GetValue<bool>() && R.IsReady() &&
                Program.IncDamages.GetAllyData(player.NetworkId).AnyCC)
            {
                R.Cast();
            }
            if (config.Item("gotoAxe", true).GetValue<bool>())
            {
                GotoAxe(target.Position);
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 1000) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) + 25)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void GotoAxe(Vector3 target)
        {
            if (!lastQpos.IsValid())
            {
                return;
            }
            var maxDist = config.Item("gotoAxeMaxDist", true).GetValue<Slider>().Value;
            var orig = player.Distance(target);
            var ext = player.Distance(lastQpos) + lastQpos.Distance(target);
            if (player.Distance(lastQpos) < maxDist && !lastQpos.UnderTurret(true))
                //ext - orig < maxDist && Orbwalking.CanMove(100)
            {
                orbwalker.SetOrbwalkingPoint(lastQpos);
                //player.IssueOrder(GameObjectOrder.MoveTo, lastQpos);
            }
        }

        private void CastQ(AIHeroClient target)
        {
            var ext = 0;
            if (player.Distance(target.ServerPosition) > 400)
            {
                ext = 100;
            }
            if (Program.IsSPrediction)
            {
                var pred = Q.GetSPrediction(target);
                if (pred.HitChance >= HitChance.Medium)
                {
                    //Console.WriteLine(1);
                    Q.Cast(player.Position.Extend(pred.CastPosition.To3D(), player.Distance(pred.CastPosition) + ext));
                }
            }
            else
            {
                var pred = Q.GetPrediction(target, true);
                var pos = player.Position.Extend(pred.CastPosition, player.Distance(pred.CastPosition) + ext);
                if (pred.CastPosition.IsValid() && target.Distance(pos) < player.Distance(target) &&
                    ((pred.Hitchance >= HitChance.Medium && player.Distance(target) < 500) ||
                     pred.Hitchance >= HitChance.High ||
                     (pred.Hitchance >= HitChance.Low && Q.CountHits(new List<Obj_AI_Base>() { target }, pos) > 0)))
                {
                    //Console.WriteLine(2 + " - " + " - " + pred.Hitchance);
                    Q.Cast(pos);
                }
            }
        }

        private void InitMenu()
        {
            config = new Menu("Olaf ", "Olaf", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 255, 222, 5)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 255, 222, 5)));
            menuD.AddItem(new MenuItem("drawaxe", "Draw Axe position", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("gotoAxe", "Catch axe", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userCCed", "Use on CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userbeforeCCed", "Use before CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(false);
            menuH.AddItem(new MenuItem("gotoAxeH", "Catch axe", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(false);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("gotoAxeLC", "Catch axe", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(false);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(20, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("gotoAxeMaxDist", "Max dist to catch axe", true))
                .SetValue(new Slider(450, 200, 600));
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }

        private static float ComboDamage(AIHeroClient hero)
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

        private void InitOlaf()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            Q.SetSkillshot(0.25f, 105, 1600, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R);
        }
    }
}