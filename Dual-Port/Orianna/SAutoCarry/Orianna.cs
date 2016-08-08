using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.Database;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Orbwalking;
using SCommon.Evade;
using SUtility.Drawings;
using SharpDX;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Orianna : SCommon.PluginBase.Champion
    {
        private Action[] UltMethods = new Action[3];
        private string[] InitiatorsList = new string[] 
        {   
            "aatroxq", "akalishadowdance", "headbutt", "carpetbomb", "dianateleport", "elisespidereinitial", "fioraq",
            "fizzpiercingstrike", "gnarbige", "gnare", "gragase", "gravesmove", "hecarimult", "ireliagatotsu", "jarvanivdragonstrike",
            "jaxleapstrike", "jaycetotheskies", "riftwalk", "khazixe", "khazixelong", "leblancslide", "leblancslidem",
            "blindmonkqtwo", "leonazenithblade", "luciane", "ufslash", "monkeykingnimbus", "pantheon_leapbash", "poppyheroiccharge",
            "renektonsliceanddice", "riventricleave", "rivenfeint", "sejuaniarcticassault", "shenshadowdash", "shyvanatransformcast",
            "rocketjump", "slashcast", "viq", "xenzhaosweep", "yasuodashwrapper", "zace", "ziggswtoggle"
        };

        private int m_lastLaneClearTick = 0;
        public TargetedSpellEvader m_targetedEvader;
        public Orianna()
            : base("Orianna", "SAutoCarry - Orianna")
        {
            Helpers.BallMgr.Initialize(this);
            Helpers.BallMgr.OnProcessCommand += BallMgr_OnProcessCommand;

            OnDraw += BeforeDraw;
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CUSEQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CUSEW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CUSEE", "Use E For Damage Enemy").SetValue(true));
            //
            Menu ult = new Menu("R Settings", "rsetting");
            ult.AddItem(new MenuItem("CUSER", "Use R").SetValue(true));
            ult.AddItem(new MenuItem("CUSERMETHOD", "R Method").SetValue<StringList>(new StringList(new string[] { "Only If Will Hit >= X Method", "If Will Hit Toggle Selected", "Smart R" }, 2))).ValueChanged += (s, ar) => ult.Item("CUSERHIT").Show(ar.GetNewValue<StringList>().SelectedIndex == 0);
            ult.AddItem(new MenuItem("CUSERHIT", "Use When Enemy Count >=").SetValue<Slider>(new Slider(3, 1, 5))).Show(ult.Item("CUSERMETHOD").GetValue<StringList>().SelectedIndex == 0);
            //
            combo.AddSubMenu(ult);

            Menu harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HUSEQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HUSEW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HUSEE", "Use E For Damage Enemy").SetValue(true));
            harass.AddItem(new MenuItem("HTOGGLE", "Toggle Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("HMANA", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            Menu laneclear = new Menu("Lane/Jungle Clear", "LaneClear");
            laneclear.AddItem(new MenuItem("LUSEQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LUSEW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LMINW", "Min. Minions To W In Range").SetValue(new Slider(3, 1, 12)));
            laneclear.AddItem(new MenuItem("LUSEE", "Use E While Jungle Clear").SetValue(true));
            laneclear.AddItem(new MenuItem("TOGGLESPELL", "Enabled Spell Farm").SetValue(new KeyBind('G', KeyBindType.Toggle, true)));
            laneclear.AddItem(new MenuItem("LMANA", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            Menu misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MANTIGAPEW", "Anti Gap Closer With E->W").SetValue(true));
            misc.AddItem(new MenuItem("MINTIMPORTANT", "Interrupt Important Spells With Q->R").SetValue(true));
            misc.AddItem(new MenuItem("MEINIT", "Cast E On Initiators").SetValue(true));
            misc.AddItem(new MenuItem("MAUTOR", "Enable Auto Ult").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("MAUTORHIT").Show(ar.GetNewValue<bool>());
            misc.AddItem(new MenuItem("MAUTORHIT", "Auto R When Can Hit").SetValue(new Slider(4, 2, 5))).Show(misc.Item("MAUTOR").GetValue<bool>());
            misc.AddItem(new MenuItem("DDRAWBALL", "Draw Ball Position").SetValue(false));
            misc.AddItem(new MenuItem("DDRAWKILL", "Draw Killable Enemy").SetValue(true));

            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);
            m_targetedEvader = new TargetedSpellEvader(TargetedSpell_Evade, misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 825f * 2f);
            Spells[Q].SetSkillshot(0f, 130f, 1400f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 220f);

            Spells[E] = new Spell(SpellSlot.E, 1100);

            Spells[R] = new Spell(SpellSlot.R, 330f);

            UltMethods[0] = () =>
            {
                {
                    if (Helpers.BallMgr.Position.LSCountEnemiesInRange(Spells[R].Range) >= ConfigMenu.Item("CUSERHIT").GetValue<Slider>().Value)
                    {
                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                        return;
                    }
                }

                if (Spells[Q].LSIsReady() && Spells[R].LSIsReady())
                {
                    Vector3 bestQPos = Vector3.Zero;
                    int bestEnemyCount = 0;
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        var enemies = enemy.LSGetEnemiesInRange(Spells[R].Range);
                        if (enemies.Count >= ConfigMenu.Item("CUSERHIT").GetValue<Slider>().Value)
                        {
                            if (enemies.Count > bestEnemyCount)
                            {
                                bestEnemyCount = enemies.Count;
                                //find center of enemies
                                Vector3 pos = Vector3.Zero;
                                enemies.ForEach(p => pos += p.ServerPosition);
                                pos = pos / enemies.Count;
                                if (pos.LSDistance(ObjectManager.Player.ServerPosition) < Spells[Q].Range / 2f && pos.LSCountEnemiesInRange(Spells[R].Range) >= bestEnemyCount)
                                    bestQPos = pos;
                                else
                                    bestQPos = enemy.ServerPosition;
                            }
                        }
                    }

                    if (bestQPos != Vector3.Zero && bestQPos.LSDistance(ObjectManager.Player.ServerPosition) < Spells[Q].Range / 2f)
                    {
                        if (Helpers.BallMgr.IsBallReady && Spells[Q].LSIsReady())
                        {
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, null, bestQPos.LSTo2D());
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                        }
                    }
                }
            };

            UltMethods[1] = () =>
            {
                if (TargetSelector.SelectedTarget != null)
                {
                    if (Spells[Q].LSIsReady() && Spells[R].LSIsReady())
                    {
                        if (TargetSelector.SelectedTarget.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) < Spells[Q].Range / 2f)
                        {
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, TargetSelector.SelectedTarget);
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                        }
                    }
                    else if (Spells[R].LSIsReady())
                    {
                        if (TargetSelector.SelectedTarget.ServerPosition.LSDistance(Helpers.BallMgr.Position) < Spells[R].Range)
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, TargetSelector.SelectedTarget);
                    }
                }
            };

            UltMethods[2] = () =>
            {
                if (Spells[Q].LSIsReady() && Spells[R].LSIsReady())
                {
                    Vector3 bestQPos = Vector3.Zero;
                    int bestPrioSum = 0;
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        var enemies = enemy.LSGetEnemiesInRange(Spells[R].Range);
                        int prio_sum = 0;
                        foreach (var e in enemies)
                        {
                            prio_sum += e.GetPriority();
                            if (e.HealthPercent < 50)
                                prio_sum += 1;
                        }

                        if (prio_sum >= 6)
                        {
                            if (prio_sum > bestPrioSum)
                            {
                                bestPrioSum = prio_sum;
                                //find center of enemies
                                Vector3 pos = Vector3.Zero;
                                enemies.ForEach(p => pos += p.ServerPosition);
                                pos = pos / enemies.Count;

                                var enemies2 = pos.LSGetEnemiesInRange(Spells[R].Range);
                                int prio_sum2 = 0;
                                foreach (var e in enemies2)
                                {
                                    prio_sum2 += e.GetPriority();
                                    if (e.HealthPercent < 50)
                                        prio_sum2 += 1;
                                }

                                if (pos.LSDistance(ObjectManager.Player.ServerPosition) < Spells[Q].Range / 2f && prio_sum2 >= bestPrioSum)
                                    bestQPos = pos;
                                else
                                    bestQPos = enemy.ServerPosition;
                            }
                        }
                    }

                    if (bestQPos != Vector3.Zero && bestQPos.LSDistance(ObjectManager.Player.ServerPosition) < Spells[Q].Range / 2f)
                    {
                        if (Helpers.BallMgr.IsBallReady && Spells[Q].LSIsReady())
                        {
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, null, bestQPos.LSTo2D());
                            Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                        }
                    }
                }

                {
                    int prio_sum = 0;
                    var enemies = HeroManager.Enemies.Where(p => p.ServerPosition.LSDistance(Helpers.BallMgr.Position) <= Spells[R].Range);
                    foreach (var enemy in enemies)
                    {
                        prio_sum += enemy.GetPriority();
                        if (enemy.HealthPercent < 50)
                            prio_sum += 1;
                    }

                    if (prio_sum >= 6)
                    {
                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                        return;
                    }

                    var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical, true, null, Helpers.BallMgr.Position);
                    if (t != null && ObjectManager.Player.LSCountEnemiesInRange(2000) <= 2)
                    {
                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, t);
                        return;
                    }
                }
            };
        }

        public void BeforeDraw()
        {
            if (ConfigMenu.Item("DDRAWKILL").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (!enemy.IsDead && enemy.Health < CalculateComboDamage(enemy))
                    {
                        var killable_pos = Drawing.WorldToScreen(enemy.Position);
                        Drawing.DrawText((int)killable_pos.X - 20, (int)killable_pos.Y + 35, System.Drawing.Color.Red, "Killable");
                    }
                }
            }

            if (ConfigMenu.Item("DDRAWBALL").GetValue<bool>() && Helpers.BallMgr.Position != Vector3.Zero)
                Render.Circle.DrawCircle(Helpers.BallMgr.Position, 130f, System.Drawing.Color.Red, 1);
        }

        public void BeforeOrbwalk()
        {
            if (ObjectManager.Player.HealthPercent < 25 && ObjectManager.Player.LSCountEnemiesInRange(1200) > 0)
                Spells[E].CastOnUnit(ObjectManager.Player);

            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None)
            {
                Helpers.BallMgr.ClearWorkQueue();
                if (ConfigMenu.Item("HTOGGLE").GetValue<KeyBind>().Active)
                    Harass();
            }

            if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None)
            {
                if (Spells[R].LSIsReady() && Helpers.BallMgr.IsBallReady && ConfigMenu.Item("MAUTOR").GetValue<bool>())
                {
                    if (CountEnemiesInRangePredicted(Spells[R].Range, 100, 0.75f) >= ConfigMenu.Item("MAUTORHIT").GetValue<Slider>().Value)
                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                    else
                    {
                        if (Spells[Q].LSIsReady())
                        {
                            List<Vector2> poses = new List<Vector2>();
                            foreach (var enemy in HeroManager.Enemies)
                            {
                                if (enemy.LSIsValidTarget(Spells[Q].Range))
                                {
                                    var pos = LeagueSharp.Common.Prediction.GetPrediction(enemy, 0.75f).UnitPosition.LSTo2D();
                                    if (pos.LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) <= 800)
                                        poses.Add(LeagueSharp.Common.Prediction.GetPrediction(enemy, 0.75f).UnitPosition.LSTo2D());
                                }
                            }

                            foreach (var list in GetCombinations(poses))
                            {
                                if (list.Count >= ConfigMenu.Item("MAUTORHIT").GetValue<Slider>().Value)
                                {
                                    Vector2 center;
                                    float radius;
                                    MEC.FindMinimalBoundingCircle(poses, out center, out radius);
                                    if (radius < Spells[R].Width && center.LSDistance(ObjectManager.Player.ServerPosition) < 825f)
                                    {
                                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, null, center);
                                        Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Combo()
        {
            //R
            if (ConfigMenu.Item("CUSER").GetValue<bool>())
                UltMethods[ConfigMenu.Item("CUSERMETHOD").GetValue<StringList>().SelectedIndex]();
            if (Spells[Q].LSIsReady() && ConfigMenu.Item("CUSEQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range / 2f, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, t);
            }

            if (Spells[W].LSIsReady() && ConfigMenu.Item("CUSEW").GetValue<bool>())
            {
                if (CountEnemiesInRangePredicted(Spells[W].Range, 50, 0.25f) > 0)
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Dissonance, null);
            }

            if (Spells[E].LSIsReady() && !Spells[W].LSIsReady() && ConfigMenu.Item("CUSEE").GetValue<bool>())
            {
                if (Helpers.BallMgr.CheckHeroCollision(ObjectManager.Player.ServerPosition))
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Protect, ObjectManager.Player);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < ConfigMenu.Item("HMANA").GetValue<Slider>().Value)
                return;

            if (Spells[Q].LSIsReady() && ConfigMenu.Item("HUSEQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range / 2f, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Attack, t);
            }

            if (Spells[W].LSIsReady() && ConfigMenu.Item("HUSEW").GetValue<bool>())
            {
                if (CountEnemiesInRangePredicted(Spells[W].Range, 50, 0.25f) > 0)
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Dissonance, null);
            }

            if (Spells[E].LSIsReady() && !Spells[W].LSIsReady() && ConfigMenu.Item("HUSEE").GetValue<bool>())
            {
                if (Helpers.BallMgr.CheckHeroCollision(ObjectManager.Player.ServerPosition))
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Protect, ObjectManager.Player);
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < ConfigMenu.Item("LMANA").GetValue<Slider>().Value || Utils.TickCount - m_lastLaneClearTick < 250 || !ConfigMenu.Item("TOGGLESPELL").GetValue<KeyBind>().Active)
                return;

            m_lastLaneClearTick = Utils.TickCount;

            if (Spells[Q].LSIsReady() && ConfigMenu.Item("LUSEQ").GetValue<bool>())
            {
                var farm = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(Spells[Q].Range / 2f, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None).Select(p => p.ServerPosition.LSTo2D()).ToList(), Spells[Q].Width, Spells[Q].Range);
                if (farm.MinionsHit > 0 && Helpers.BallMgr.IsBallReady)
                    Spells[Q].Cast(farm.Position, true);
            }

            if (Spells[W].LSIsReady() && ConfigMenu.Item("LUSEW").GetValue<bool>())
            {
                if (ObjectManager.Get<Obj_AI_Minion>().Count(p => (p.IsEnemy || p.IsJungleMinion()) && p.ServerPosition.LSDistance(Helpers.BallMgr.Position) <= Spells[W].Range) >= ConfigMenu.Item("LMINW").GetValue<Slider>().Value)
                    Spells[W].Cast(ObjectManager.Player.ServerPosition, true);
            }

            if (Spells[E].LSIsReady() && ConfigMenu.Item("LUSEE").GetValue<bool>())
            {
                if (MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any(p => p.GetJunglePriority() == 1))
                    Spells[E].CastOnUnit(ObjectManager.Player);
            }
        }

        private void BallMgr_OnProcessCommand(Helpers.BallMgr.Command cmd, AIHeroClient target, Vector2 pos)
        {
            if (!Spells[(int)cmd].LSIsReady() || (Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.Mixed && Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.Combo))
                return;

            switch (cmd)
            {
                case Helpers.BallMgr.Command.Attack:
                    {
                        if (target != null)
                            Spells[Q].SPredictionCast(target, HitChance.High, 0, 1, Helpers.BallMgr.Position);
                        else
                            Spells[Q].Cast(pos);
                    }
                    break;

                case Helpers.BallMgr.Command.Dissonance:
                    {
                        Spells[W].Cast(ObjectManager.Player.ServerPosition, true);
                    }
                    break;

                case Helpers.BallMgr.Command.Protect:
                    {
                        Spells[E].CastOnUnit(target);
                    }
                    break;

                case Helpers.BallMgr.Command.Shockwave:
                    {
                        if (CountEnemiesInRangePredicted(Spells[R].Range, 100, 0.75f) > 0)
                            Spells[R].Cast(ObjectManager.Player.ServerPosition, true);
                    }
                    break;
            }
        }

        private int CountEnemiesInRangePredicted(float range, float width, float time)
        {
            int cnt = 0;
            foreach (var enemy in HeroManager.Enemies)
            {
                var prediction = SCommon.Prediction.Prediction.GetPrediction(enemy, width, time, 0, range, false, SkillshotType.SkillshotCircle, enemy.LSGetWaypoints(), enemy.AvgMovChangeTime(), enemy.LastMovChangeTime(), enemy.AvgPathLenght(), 360, Helpers.BallMgr.Position.LSTo2D(), Helpers.BallMgr.Position.LSTo2D());
                if (prediction.HitChance > HitChance.Low)
                {
                    if (prediction.UnitPosition.LSDistance(Helpers.BallMgr.Position.LSTo2D()) < range)
                        cnt++;
                }
            }
            return cnt;
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Spells[E].LSIsReady() && gapcloser.Sender.IsEnemy)
            {
                Spells[E].CastOnUnit(ObjectManager.Player);
                Spells[W].Cast(ObjectManager.Player.ServerPosition, true);
                if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo) //combo anti gap closer self r
                    Helpers.BallMgr.Post(Helpers.BallMgr.Command.Shockwave, null);
            }
        }

        protected override void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsChannelingImportantSpell())
            {
                if (Spells[R].LSIsReady() && Helpers.BallMgr.Position.LSDistance(sender.ServerPosition) < Spells[R].Range)
                    Spells[R].Cast(ObjectManager.Player.ServerPosition, true);
                else if (Spells[Q].LSIsReady() && Spells[R].LSIsReady() && ObjectManager.Player.ServerPosition.LSDistance(sender.ServerPosition) < Spells[Q].Range / 2f)
                {
                    Spells[Q].Cast(sender, true);
                    Spells[W].Cast(ObjectManager.Player.ServerPosition, true);
                }
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly)
            {
                if (InitiatorsList.Contains(args.SData.Name.ToLower()) && sender.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) < Spells[E].Range && ConfigMenu.Item("MEINIT").GetValue<bool>())
                    Spells[E].CastOnUnit(sender, true);
            }
        }

        public override double CalculateDamageQ(AIHeroClient target)
        {
            double dmg = 0.0;
            if (ConfigMenu.Item("CUSEQ").GetValue<bool>() && Spells[Q].LSIsReady())
            {
                dmg = ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q);
                int collCount = Spells[R].GetCollision(Helpers.BallMgr.Position.LSTo2D(), new List<Vector2>() { target.ServerPosition.LSTo2D() }).Count();
                int percent = 10 - (collCount > 6 ? 6 : collCount);
                dmg = dmg * percent * 0.1;
            }
            return dmg;
        }

        private static List<List<Vector2>> GetCombinations(List<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }
            return collection;
        }

        private void TargetedSpell_Evade(DetectedTargetedSpellArgs data)
        {
            if (Spells[E].LSIsReady() && Helpers.BallMgr.IsBallReady)
            {
                if (Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.Combo || !m_targetedEvader.DisableInComboMode)
                    Spells[E].CastOnUnit(ObjectManager.Player);
            }
        }
    }
}
