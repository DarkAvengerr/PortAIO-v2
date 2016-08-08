using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Braum
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        public Spell Q,W,E,R;
        public float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public AIHeroClient Player { get { return ObjectManager.Player; } }

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 1250);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 115f, 1400f, false, SkillshotType.SkillshotLine);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("notif", "Notification (timers)", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show KS notification", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("AGCq", "Anti Gapcloser Q", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    var spell = enemy.Spellbook.Spells[i];
                    if (spell.SData.TargettingType != SpellDataTargetType.Self && spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell.SData.Name.ToLower(), spell.Name).SetValue(true));
                    }
                }
            }

            Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").AddItem(new MenuItem("Edmg", "Shield incoming damage %", true).SetValue(new Slider(20, 100, 0)));

            foreach (var enemy in HeroManager.Allies)
                Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").SubMenu("Use on").AddItem(new MenuItem("Eon" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").SubMenu("Gapcloser").AddItem(new MenuItem("AGC", "Anti Gapcloser E + W", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E W Shield Config").SubMenu("Gapcloser").AddItem(new MenuItem("gapcloser" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCombo", "Always in combo", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCount", "Auto R if hit x enemies", true).SetValue(new Slider(3, 0, 5)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCc", "Auto R immobile enemy korean style", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("OnInterruptableSpell", "OnInterruptableSpell", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Ultimate manager").AddItem(new MenuItem("Rmode" + enemy.ChampionName, enemy.ChampionName, true).SetValue(new StringList(new[] { "Normal ", "Always ", "Never ", "Normal + Gapcloser R" }, 0)));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("Harass").AddItem(new MenuItem("haras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (!Config.Item("gapcloser" + t.ChampionName).GetValue<bool>())
                return;

            if (Config.Item("AGC", true).GetValue<bool>())
            {
                if (W.LSIsReady() && gapcloser.End.LSDistance(Player.Position) < gapcloser.Start.LSDistance(Player.Position))
                {
                    var allyHero = HeroManager.Allies.Where(ally => ally.LSDistance(Player) <= W.Range && !ally.IsMe )
                           .OrderBy(ally => ally.LSDistance(gapcloser.End)).FirstOrDefault();

                    if (allyHero != null && Config.Item("Eon" + allyHero.ChampionName).GetValue<bool>())
                        W.Cast(allyHero);
                }
                if (E.LSIsReady())
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => E.Cast(t.ServerPosition));
            }

            if (Q.LSIsReady() && Config.Item("AGCq", true).GetValue<bool>())
                Q.Cast(t);

            if (R.LSIsReady() && Config.Item("Rmode" + t.ChampionName, true).GetValue<StringList>().SelectedIndex == 3)
                 R.Cast(t);
        }


        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.LSIsReady() && Config.Item("OnInterruptableSpell", true).GetValue<bool>())
            {
                if (sender.LSIsValidTarget(R.Range))
                {
                    R.Cast(sender);
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (R.LSIsReady())
            {
                if (Config.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (t.LSIsValidTarget())
                        R.Cast(t, true, true);
                }
            }

            if (Program.LagFree(2) && Q.LSIsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && R.LSIsReady() && Config.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);

            if (!t.LSIsValidTarget())
                t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (t.LSIsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && Config.Item("haras" + enemy.ChampionName).GetValue<bool>()))
                    {
                        Program.CastSpell(Q, enemy);
                    }
                }
                if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
        }

        private void LogicR()
        {
            var rCount = Config.Item("rCount", true).GetValue<Slider>().Value;
            foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(t)).OrderBy(t => t.Health))
            {
                int Rmode = Config.Item("Rmode" + t.ChampionName, true).GetValue<StringList>().SelectedIndex;

                if (Rmode == 2)
                    continue;
                else if (Rmode == 1)
                    Program.CastSpell(R, t);

                if (rCount > 0)
                    R.CastIfWillHit(t, rCount);

                if (Config.Item("rCc", true).GetValue<bool>() && !OktwCommon.CanMove(t) && t.HealthPercent > 20 * t.LSCountAlliesInRange(500) )

                    LeagueSharp.Common.Utility.DelayAction.Add(800 - (int)(Player.LSDistance(t.Position) / 2) , () => CastRtime(t));

                if (Config.Item("rCombo", true).GetValue<bool>() && Program.Combo)
                {
                    Program.CastSpell(R, t);
                    return;
                }
            }
        }

        private void CastRtime(AIHeroClient t)
        {
            if (OktwCommon.ValidUlt(t))
                R.Cast(t, true, true);
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy)
                return;

            if (Config.Item("spell" + args.SData.Name.ToLower()) != null && !Config.Item("spell" + args.SData.Name.ToLower()).GetValue<bool>())
                return;

            if (E.LSIsReady() && Config.Item("autoE", true).GetValue<bool>() && OktwCommon.CanHitSkillShot(Player, args))
            {
                E.Cast(sender.Position);
            }

            if (W.LSIsReady() && args.SData.MissileSpeed > 0)
            {
                foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && Player.LSDistance(ally.ServerPosition) < W.Range && Config.Item("Eon" + ally.ChampionName).GetValue<bool>()))
                {
                    if (OktwCommon.CanHitSkillShot(ally, args) || OktwCommon.GetIncomingDamage(ally,1) > ally.Health * Config.Item("Edmg", true).GetValue<Slider>().Value * 0.01)
                    {
                        if (E.LSIsReady())
                            LeagueSharp.Common.Utility.DelayAction.Add(200, () => E.Cast(sender.Position));

                        if (Player.HealthPercent < 20 && !ally.IsMe)
                            continue;
                        if (Player.HealthPercent < 50 && !ally.IsMe && ally.LSUnderTurret(true))
                            continue;

                        W.Cast(ally);
                    }
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
