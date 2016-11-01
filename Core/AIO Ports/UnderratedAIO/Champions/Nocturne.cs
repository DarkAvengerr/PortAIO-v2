using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Nocturne
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell P, Q, W, E, R;
        public static int[] rRanges = new int[] { 2500, 3250, 4000 };
        private static float lastR;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Nocturne()
        {
            InitNocturne();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Nocturne</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            AIHeroClient target = DrawHelper.GetBetterTarget(Q.Range, TargetSelector.DamageType.Physical);
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
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

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(GetTargetRange(), TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady() && data.AnyCC)
            {
                W.Cast();
            }
            var eTarget = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical);
            var cmbdmg = ComboDamage(target) + ItemHandler.GetItemsDamage(target);
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, cmbdmg);
            }
            var dist = player.Distance(target);
            if (lastR > 4000f)
            {
                lastR = 0f;
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) &&
                dist < config.Item("useqMaxRange", true).GetValue<Slider>().Value && !player.IsDashing())
            {
                if (dist < 550)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(eTarget) &&
                dist < config.Item("useeMaxRange", true).GetValue<Slider>().Value)
            {
                E.Cast(eTarget);
            }
            if (config.Item("user", true).GetValue<bool>() && R.IsReady())
            {
                R.Range = rRanges[R.Level - 1];
            }
            if (config.Item("user", true).GetValue<bool>() && lastR.Equals(0) && !target.UnderTurret(true) &&
                R.CanCast(target) &&
                ((qTrailOnMe && eBuff(target) && target.MoveSpeed > player.MoveSpeed && dist > 360 &&
                  target.HealthPercent < 60) ||
                 (dist < rRanges[R.Level - 1] && dist > 900 &&
                  target.CountAlliesInRange(2000) >= target.CountEnemiesInRange(2000) &&
                  cmbdmg + Environment.Hero.GetAdOverTime(player, target, 5) > target.Health &&
                  (target.Health > Q.GetDamage(target) || !Q.IsReady())) ||
                 (player.HealthPercent < 40 && target.HealthPercent < 40 && target.CountAlliesInRange(1000) == 1 &&
                  target.CountEnemiesInRange(1000) == 1)))
            {
                R.Cast(target);
                lastR = System.Environment.TickCount;
            }
            if (config.Item("user", true).GetValue<bool>() && !lastR.Equals(0) && R.CanCast(target) &&
                ((cmbdmg * 1.6 + Environment.Hero.GetAdOverTime(player, target, 5) > target.Health ||
                  R.GetDamage(target) > target.Health ||
                  (qTrailOnMe && eBuff(target) && target.MoveSpeed > player.MoveSpeed && dist > 360 &&
                   target.HealthPercent < 60))))
            {
                var time = System.Environment.TickCount - lastR;
                if (time > 3500f || player.Distance(target) > E.Range || cmbdmg > target.Health ||
                    (player.HealthPercent < 40 && target.HealthPercent < 40))
                {
                    R.Cast(target);
                    lastR = 0f;
                }
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private float GetTargetRange()
        {
            if (R.IsReady())
            {
                return rRanges[R.Level - 1];
            }
            else
            {
                return Q.Range;
            }
        }

        private void Clear()
        {
            if (Environment.Minion.KillableMinion(player.AttackRange))
            {
                return;
            }
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            MinionManager.FarmLocation bestPositionQ =
                Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady())
            {
                if (bestPositionQ.MinionsHit >= config.Item("qhitLC", true).GetValue<Slider>().Value)
                {
                    Q.Cast(bestPositionQ.Position);
                }
                var jungleMob = Jungle.GetNearest(player.Position, Q.Range * 0.75f);
                if (jungleMob != null && jungleMob.Health > player.GetAutoAttackDamage(jungleMob, true) * 2)
                {
                    Q.Cast(jungleMob.Position);
                }
            }
        }

        private void Harass(AIHeroClient target)
        {
            if (Environment.Minion.KillableMinion(player.AttackRange))
            {
                return;
            }
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                Q.Cast(target);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady() && Q.Instance.SData.Mana < player.Mana)
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady() && E.Instance.SData.Mana < player.Mana)
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R);
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Base))
            {
                return;
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(
                config.Item("drawrr", true).GetValue<Circle>(), R.Level >= 1 ? rRanges[R.Level - 1] : rRanges[0]);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
            if (!config.Item("bestpospas", true).GetValue<bool>())
            {
                return;
            }
            MinionManager.FarmLocation bestPositionP =
                P.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (bestPositionP.Position.IsValid() && bestPositionP.MinionsHit > 2 && uBlades)
            {
                Drawing.DrawCircle(bestPositionP.Position.To3D(), 150f, Color.Crimson);
            }
        }

        private void InitNocturne()
        {
            P = new Spell(SpellSlot.Q, 1000);
            P.SetSkillshot(
                3000, Orbwalking.GetRealAutoAttackRange(player) + 50, 3000, false, SkillshotType.SkillshotCircle);
            Q = new Spell(SpellSlot.Q, 1150);
            Q.SetSkillshot(0.25f, 60f, 1350, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, rRanges[0]);
        }

        private static bool qTrail(AIHeroClient target)
        {
            return target.Buffs.Any(buff => buff.Name == "nocturneduskbringertrail");
        }

        private static bool qTrailOnMe
        {
            get { return player.Buffs.Any(buff => buff.Name == "nocturneduskbringerhaste"); }
        }

        private static bool eBuff(AIHeroClient target)
        {
            return target.Buffs.Any(buff => buff.Name == "NocturneUnspeakableHorror");
        }

        private static bool uBlades
        {
            get { return player.Buffs.Any(buff => buff.Name == "nocturneumbrablades"); }
        }

        private void InitMenu()
        {
            config = new Menu("Nocturne", "Nocturne", true);
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
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("bestpospas", "Best position for passive", true)).SetValue(false);
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqMaxRange", "   Q max Range", true))
                .SetValue(new Slider(1000, 0, (int) Q.Range));
            menuC.AddItem(new MenuItem("usew", "Use W against targeted CC", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeMaxRange", "   E max Range", true))
                .SetValue(new Slider(300, 0, (int) E.Range));
            menuC.AddItem(new MenuItem("user", "Use R in close range", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qhitLC", "   Min hit", true).SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}