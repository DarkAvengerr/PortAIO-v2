using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Orbwalking;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Azir : SCommon.PluginBase.Champion
    {
        private int lastLaneClearTick = 0;
        private int CastET, CastQT;
        private Vector2 CastQLocation, CastELocation, InsecLocation, InsecTo, JumpTo;
        public Azir()
            : base("Azir", "SAutoCarry - Azir")
        {
            Helpers.SoldierMgr.Initialize(this);
            Orbwalker.RegisterShouldWait(ShouldWait);
            Orbwalker.RegisterCanAttack(CanAttack);
            Orbwalker.RegisterCanOrbwalkTarget(CanOrbwalkTarget);
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
            OnUpdate += BeforeOrbwalk;
            OnDraw += BeforeDraw;
            
            Game.OnWndProc += Game_OnWndProc;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Azir.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseQ", "Use Q").SetValue(true)).ValueChanged += (s, ar) =>
            {
                combo.Item("SAutoCarry.Azir.Combo.UseQOnlyOutOfAA").Show(ar.GetNewValue<bool>());
                combo.Item("SAutoCarry.Azir.Combo.UseQAlwaysMaxRange").Show(ar.GetNewValue<bool>());
                combo.Item("SAutoCarry.Azir.Combo.UseQWhenNoWAmmo").Show(ar.GetNewValue<bool>());
            };
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseQOnlyOutOfAA", "Use Q Only When Enemy out of range").SetValue(true)).Show(combo.Item("SAutoCarry.Azir.Combo.UseQ").GetValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseQAlwaysMaxRange", "Always Cast Q To Max Range").SetValue(false)).Show(combo.Item("SAutoCarry.Azir.Combo.UseQ").GetValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseQWhenNoWAmmo", "Use Q When Out of W Ammo").SetValue(false)).Show(combo.Item("SAutoCarry.Azir.Combo.UseQ").GetValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseE", "Use E If target is killable").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.RMinHit", "Min R Hit").SetValue(new Slider(1, 1, 5)));    
            combo.AddItem(new MenuItem("SAutoCarry.Azir.Combo.RMinHP", "Use R whenever my health < ").SetValue(new Slider(20, 0, 100)));

            Menu harass = new Menu("Harass", "SAutoCarry.Azir.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Azir.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Azir.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Azir.Harass.MaxSoldier", "Max Soldier Count").SetValue(new Slider(1, 1, 3)));
            harass.AddItem(new MenuItem("SAutoCarry.Azir.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('J', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("SAutoCarry.Azir.Harass.ManaPercent", "Min. Mana Percent").SetValue(new Slider(40, 0, 100)));

            Menu laneclear = new Menu("LaneClear", "SAutoCarry.Azir.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Azir.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Azir.LaneClear.MinQMinion", "Q Min. Minions").SetValue(new Slider(3, 1, 5)));
            laneclear.AddItem(new MenuItem("SAutoCarry.Azir.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Azir.LaneClear.Toggle", "Toggle Spellfarm").SetValue(new KeyBind('L', KeyBindType.Toggle, true)));
            laneclear.AddItem(new MenuItem("SAutoCarry.Azir.LaneClear.ManaPercent", "Min. Mana Percent").SetValue(new Slider(40, 0, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.Azir.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Azir.Misc.Jump", "Jump To Cursor (Always Jumps Max Range)").SetValue(new KeyBind('G', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.Azir.Misc.JumpEQ", "Jump To Cursor (Jumps with juke)").SetValue(new KeyBind('A', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.Azir.Misc.Insec", "Insec Selected Target").SetValue(new KeyBind('T', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.Azir.Misc.WQKillSteal", "Use W->Q to KillSteal").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Azir.Misc.BlockR", "Block R if wont hit anyone").SetValue(true));

            Menu antigap = new Menu("AntiGapCloser (R)", "SAutoCarry.Azir.Misc.AntiGapCloser");
            foreach (var enemy in HeroManager.Enemies)
            {
                if (AntiGapcloser.Spells.Any(p => p.ChampionName == enemy.ChampionName))
                {
                    var spells = AntiGapcloser.Spells.Where(p => p.ChampionName == enemy.ChampionName);
                    foreach (var gapcloser in spells)
                        antigap.AddItem(new MenuItem("SAutoCarry.Azir.Misc.AntiGapCloser." + gapcloser.SpellName, String.Format("{0} ({1})", gapcloser.ChampionName, gapcloser.Slot)).SetValue(false));
                }
            }
            antigap.AddItem(new MenuItem("SAutoCarry.Azir.Misc.AntiGapCloser.Enable", "Enabled").SetValue(true));
            misc.AddSubMenu(antigap);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 825f);
            Spells[Q].SetSkillshot(0.25f, 70f, 1600f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 450f);
            Spells[W].SetSkillshot(0.25f, 70f, 0f, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 1250f);
            Spells[E].SetSkillshot(0.25f, 100, 1700f, false, SkillshotType.SkillshotLine);

            Spells[R] = new Spell(SpellSlot.R, 450f);
            Spells[R].SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
        }

        public void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[Q].Range + Helpers.SoldierMgr.SoldierAttackRange - 25, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            var extendedTarget = TargetSelector.GetTarget(Spells[Q].Range + 400, LeagueSharp.Common.TargetSelector.DamageType.Magical);

            if (t != null)
            {
                if (ComboUseR && Spells[R].LSIsReady() && t.LSIsValidTarget(Spells[R].Range) && ShouldCast(SpellSlot.R, t))
                    Spells[R].SPredictionCast(t, HitChance.High);

                if (ComboUseW && Spells[W].LSIsReady() && ShouldCast(SpellSlot.W, t))
                    Spells[W].Cast(ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), 450));

                if (ComboUseQ && Spells[Q].LSIsReady() && ShouldCast(SpellSlot.Q, t))
                {
                    foreach (var soldier in Helpers.SoldierMgr.ActiveSoldiers)
                    {
                        if (ObjectManager.Player.ServerPosition.LSDistance(t.ServerPosition) < Spells[Q].Range)
                        {
                            Spells[Q].UpdateSourcePosition(soldier.Position, ObjectManager.Player.ServerPosition);
                            var predRes = Spells[Q].GetSPrediction(t);
                            if (predRes.HitChance >= HitChance.High)
                            {
                                var pos = predRes.CastPosition;
                                if (ComboUseQAlwaysMaxRange)
                                    pos = ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(pos, Spells[Q].Range);
                                Spells[Q].Cast(pos);
                                return;
                            }
                        }
                    }
                }
            }

            if (extendedTarget != null)
            {
                if (ComboUseE && Spells[E].LSIsReady() && ShouldCast(SpellSlot.E, extendedTarget))
                {
                    foreach (var soldier in Helpers.SoldierMgr.ActiveSoldiers)
                    {
                        if (Spells[E].WillHit(extendedTarget, soldier.Position))
                        {
                            Spells[E].Cast(soldier.Position);
                            return;
                        }
                    }
                }
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if (t == null)
                return;

            if (HarassUseW && Spells[W].LSIsReady() && ShouldCast(SpellSlot.W, t))
                Spells[W].Cast(ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), 450));

            if (HarassUseQ && Spells[Q].LSIsReady() && ShouldCast(SpellSlot.Q, t))
            {
                foreach (var soldier in Helpers.SoldierMgr.ActiveSoldiers)
                {
                    if (ObjectManager.Player.ServerPosition.LSDistance(t.ServerPosition) < Spells[Q].Range)
                    {
                        Spells[Q].UpdateSourcePosition(soldier.Position, ObjectManager.Player.ServerPosition);
                        if (Spells[Q].SPredictionCast(t, HitChance.High))
                            return;
                    }
                }
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana || !LaneClearToggle)
                return;

            if (LaneClearUseW && Spells[W].LSIsReady() && Spells[W].Instance.Ammo != 0)
            {
                var minions = MinionManager.GetMinions(Spells[W].Range + Helpers.SoldierMgr.SoldierAttackRange / 2f);
                if (minions.Count > 1)
                {
                    var loc = MinionManager.GetBestCircularFarmLocation(minions.Select(p => p.ServerPosition.LSTo2D()).ToList(), Helpers.SoldierMgr.SoldierAttackRange, Spells[W].Range);
                    if (loc.MinionsHit > 2)
                        Spells[W].Cast(loc.Position);
                }
            }

            if (LaneClearUseQ && Spells[Q].LSIsReady())
            {
                MinionManager.FarmLocation bestfarm = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(Spells[Q].Range + 100).Select(p => p.ServerPosition.LSTo2D()).ToList(), Helpers.SoldierMgr.SoldierAttackRange, Spells[Q].Range + 100);
                if (bestfarm.MinionsHit >= LaneClearQMinMinion)
                    Spells[Q].Cast(bestfarm.Position);
            }
        }

        public void BeforeOrbwalk()
        {
            if (Spells[R].LSIsReady())
                Spells[R].Width = 133 * (3 + Spells[R].Level);

            if (JumpActive)
                Jump(Game.CursorPos);

            if (JumpEQActive)
                Jump(Game.CursorPos, true);

            if (InsecActive)
                Insec();

            if (WQKillSteal)
                KillSteal();

            if (HarassToggle)
                Harass();
        }

        public void BeforeDraw()
        {
            if (InsecTo.LSIsValid())
                Render.Circle.DrawCircle(InsecTo.To3D2(), 200, System.Drawing.Color.DarkBlue);
        }

        public void KillSteal()
        {
            if (!Spells[Q].LSIsReady() || (Helpers.SoldierMgr.ActiveSoldiers.Count == 0 && !Spells[W].LSIsReady()))
                return;

            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells[Q].Range + 100) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if ((ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q)) > target.Health + 20)
                {
                    if (Helpers.SoldierMgr.ActiveSoldiers.Count == 0)
                        Spells[W].Cast(ObjectManager.Player.Position.LSTo2D().LSExtend(target.Position.LSTo2D(), 450));
                    else
                        Spells[Q].SPredictionCast(target, HitChance.High);
                }
            }
        }

        public void Jump(Vector3 pos, bool juke = false, bool castq = true)
        {
            Orbwalker.Orbwalk(null, pos);
            if (Math.Abs(Spells[E].Cooldown) < 0.00001)
            {
                var extended = ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(pos.LSTo2D(), 800f);
                if (!JumpTo.LSIsValid())
                    JumpTo = pos.LSTo2D();

                if (Spells[W].LSIsReady() && Helpers.SoldierMgr.ActiveSoldiers.Count == 0)
                {
                    if (juke)
                    {
                        var outRadius = 250 / (float)Math.Cos(2 * Math.PI / 12);

                        for (int i = 1; i <= 12; i++)
                        {
                            var angle = i * 2 * Math.PI / 12;
                            float x = ObjectManager.Player.Position.X + outRadius * (float)Math.Cos(angle);
                            float y = ObjectManager.Player.Position.Y + outRadius * (float)Math.Sin(angle);
                            if (NavMesh.GetCollisionFlags(x, y).HasFlag(CollisionFlags.Wall) && !ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(new Vector2(x, y), 500f).LSIsWall())
                            {
                                Spells[W].Cast(ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(new Vector2(x, y), 800f));
                                return;
                            }
                        }
                    }
                    Spells[W].Cast(extended);
                }

                if (Helpers.SoldierMgr.ActiveSoldiers.Count > 0 && Spells[Q].LSIsReady())
                {
                    var closestSoldier = Helpers.SoldierMgr.ActiveSoldiers.MinOrDefault(s => s.Position.LSTo2D().LSDistance(extended, true));
                    CastELocation = closestSoldier.Position.LSTo2D();
                    CastQLocation = closestSoldier.Position.LSTo2D().LSExtend(JumpTo, 800f);

                    if (CastELocation.LSDistance(JumpTo) > ObjectManager.Player.ServerPosition.LSTo2D().LSDistance(JumpTo) && !juke && castq)
                    {
                        CastQLocation = extended;
                        CastET = Utils.TickCount + 250;
                        Spells[Q].Cast(CastQLocation);
                    }
                    else
                    {
                        Spells[E].Cast(CastELocation, true);
                        if (ObjectManager.Player.ServerPosition.LSTo2D().LSDistance(CastELocation) < 700 && castq)
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () => Spells[Q].Cast(CastQLocation, true));
                    }
                }
            }
            else
            {
                if (Spells[Q].LSIsReady() && CastELocation.LSDistance(ObjectManager.Player.ServerPosition) <= 200 && castq)
                    Spells[Q].Cast(CastQLocation, true);

                JumpTo = Vector2.Zero;
            }
        }

        public void Insec()
        {
            if (TargetSelector.SelectedTarget != null)
            {
                if (TargetSelector.SelectedTarget.LSIsValidTarget(900))
                {
                    if (Spells[Q].LSIsReady())
                    {
                        if (Spells[R].LSIsReady())
                        {
                            var direction = (TargetSelector.SelectedTarget.ServerPosition - ObjectManager.Player.ServerPosition).LSTo2D().LSNormalized();
                            var insecPos = TargetSelector.SelectedTarget.ServerPosition.LSTo2D() + (direction * 200f);
                            if (!InsecLocation.LSIsValid())
                                InsecLocation = ObjectManager.Player.ServerPosition.LSTo2D();
                            Jump(insecPos.To3D());
                        }
                    }
                    else if (ObjectManager.Player.ServerPosition.LSDistance(TargetSelector.SelectedTarget.ServerPosition) < 400 && InsecLocation.LSIsValid())
                    {
                        if (InsecTo.LSIsValid() && InsecTo.LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) < 1500)
                            Spells[R].Cast(InsecTo);
                        else
                            Spells[R].Cast(InsecLocation);
                        if (!Spells[R].LSIsReady())
                            InsecLocation = Vector2.Zero;
                    }
                }
                else
                {
                    Orbwalker.Orbwalk(null, Game.CursorPos);
                }
            }
            else
            {
                Orbwalker.Orbwalk(null, Game.CursorPos);
            }
        }


        public bool ShouldCast(SpellSlot slot, AIHeroClient target)
        {
            switch (slot)
            {
                case SpellSlot.Q:
                    {
                        if (ComboUseQOnlyOutOfRange && Helpers.SoldierMgr.InAARange(target))
                            return false;

                        if (Helpers.SoldierMgr.ActiveSoldiers.Count == 0)
                            return false;

                        if (ComboUseQWhenNoWAmmo && Spells[W].Instance.Ammo != 0)
                            return false;

                        return true;
                    }

                case SpellSlot.W:
                    {
                        if (Spells[W].Instance.Ammo == 0)
                            return false;

                        if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed && Helpers.SoldierMgr.ActiveSoldiers.Count >= HarassMaxSoldierCount)
                            return false;

                        return true;
                    }

                case SpellSlot.E:
                    {
                        if (CalculateDamageE(target) + SCommon.Damage.AutoAttack.GetDamage(target) >= target.Health && HeroManager.Enemies.Count(p => p.LSIsValidTarget() && p.LSDistance(target.ServerPosition) < 600) < 2)
                            return true;

                        return false;
                    }

                case SpellSlot.R:
                    {
                        if (CalculateDamageR(target) >= target.Health - 150)
                            return true;

                        if (ObjectManager.Player.HealthPercent < ComboRMinHP)
                            return true;

                        if (IsWallStunable(target.ServerPosition.LSTo2D(), ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(Spells[R].GetSPrediction(target).UnitPosition, 200 - target.BoundingRadius)) && CalculateDamageR(target) >= target.Health / 2f)
                            return true;

                        if (ComboRMinHit > 1 && Spells[R].GetAoeSPrediction().HitCount > ComboRMinHit)
                            return true;

                        return false;
                    }
            }

            return false;
        }

        private bool IsWallStunable(Vector2 from, Vector2 to)
        {
            float count = from.LSDistance(to);
            for (uint i = 0; i <= count; i += 25)
            {
                Vector2 pos = from.LSExtend(ObjectManager.Player.ServerPosition.LSTo2D(), -i);
                var colFlags = NavMesh.GetCollisionFlags(pos.X, pos.Y);
                if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                    return true;
            }
            return false;
        }

        private bool ShouldWait()
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        minion =>
                            (minion.LSIsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            (Helpers.SoldierMgr.InAARange(minion) || SCommon.Orbwalking.Utility.InAARange(minion)) && MinionManager.IsMinion(minion, false) &&
                            HealthPrediction.LaneClearHealthPrediction(
                                    minion, (int)(ObjectManager.Player.AttackDelay * 1000f * 2f + ObjectManager.Player.AttackCastDelay * 1000f), 30) <=
                                SCommon.Damage.AutoAttack.GetDamage(minion, true)));
        }
        private bool CanAttack()
        {
            if (Helpers.SoldierMgr.SoldierAttacking)
                return false;

            return Utils.TickCount + Game.Ping / 2 - Orbwalker.LastAATick >= 1000f / (ObjectManager.Player.GetAttackSpeed() * Orbwalker.BaseAttackSpeed);
        }

        private bool CanOrbwalkTarget(AttackableUnit target)
        {
            if (target.LSIsValidTarget())
            {
                float scalingRange = 0f;
                if (target.Type == GameObjectType.AIHeroClient)
                    scalingRange = (target as AIHeroClient).GetScalingRange();
                if (Helpers.SoldierMgr.ActiveSoldiers.Any(p => p.Position.LSDistance(ObjectManager.Player.ServerPosition) - ObjectManager.Player.BoundingRadius < 900 && p.Position.LSDistance(target.Position) - target.BoundingRadius - scalingRange + 10 < Helpers.SoldierMgr.SoldierAttackRange + p.BoundingRadius))
                    return true;

                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return ObjectManager.Player.LSDistance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() + 10 < ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
                }
                else
                    return ObjectManager.Player.LSDistance(target.Position) - target.BoundingRadius + 10 < ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
            }
            return false;
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "azirq")
                    Orbwalker.ResetAATimer();

                if (JumpActive || InsecActive)
                {
                    if (args.SData.Name == "azire" && Utils.TickCount - CastQT < 500 + Game.Ping)
                    {
                        Spells[Q].Cast(CastQLocation, true);
                        CastQT = 0;
                    }

                    if (args.SData.Name == "azirq" && Utils.TickCount - CastET < 500 + Game.Ping)
                    {
                        Spells[E].Cast(CastELocation, true);
                        CastET = 0;
                    }
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (AntiGapCloserEnabled)
            {
                if (ConfigMenu.Item("SAutoCarry.Azir.Misc.AntiGapCloser." + gapcloser.Sender.Spellbook.GetSpell(gapcloser.Slot).Name).GetValue<bool>())
                {
                    if (gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < 200)
                        Spells[R].Cast(gapcloser.End.LSExtend(gapcloser.Start, 100));
                }
            }

        }

        public override double CalculateDamageW(AIHeroClient target)
        {
            return Helpers.SoldierMgr.GetAADamage(target);
        }


        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                var clickedObject = ObjectManager.Get<Obj_AI_Base>().Where(p => p.Position.LSDistance(Game.CursorPos, true) < 40000 && p.IsAlly).OrderBy(q => q.Position.LSDistance(Game.CursorPos, true)).FirstOrDefault();

                if (clickedObject != null)
                {
                    InsecTo = clickedObject.Position.LSTo2D();
                    if (clickedObject.IsMe)
                        InsecTo = Vector2.Zero;
                }
            }

        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R && BlockR)
            {
                args.Process = false;
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.LSIsValidTarget(Spells[R].Range + 200))
                    {
                        var pred = Spells[R].GetPrediction(enemy);
                        args.Process |= pred.Hitchance > HitChance.Low;
                    }
                }
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseQOnlyOutOfRange
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseQOnlyOutOfAA").GetValue<bool>(); }
        }

        public bool ComboUseQAlwaysMaxRange
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseQAlwaysMaxRange").GetValue<bool>(); }
        }

        public bool ComboUseQWhenNoWAmmo
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseQWhenNoWAmmo").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.UseR").GetValue<bool>(); }
        }

        public int ComboRMinHit
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.RMinHit").GetValue<Slider>().Value; }
        }

        public int ComboRMinHP
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Combo.RMinHP").GetValue<Slider>().Value; }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Harass.UseW").GetValue<bool>(); }
        }

        public int HarassMaxSoldierCount
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Harass.MaxSoldier").GetValue<Slider>().Value; }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Harass.ManaPercent").GetValue<Slider>().Value; }
        }

        public bool LaneClearUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.LaneClear.UseW").GetValue<bool>(); }
        }

        public int LaneClearQMinMinion
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.LaneClear.MinQMinion").GetValue<Slider>().Value; }
        }

        public bool LaneClearToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.LaneClear.Toggle").GetValue<KeyBind>().Active; }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.LaneClear.ManaPercent").GetValue<Slider>().Value; }
        }

        public bool JumpActive
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.Jump").GetValue<KeyBind>().Active; }
        }

        public bool JumpEQActive
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.JumpEQ").GetValue<KeyBind>().Active; }
        }

        public bool InsecActive
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.Insec").GetValue<KeyBind>().Active; }
        }

        public bool WQKillSteal
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.WQKillSteal").GetValue<bool>(); }
        }

        public bool BlockR
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.BlockR").GetValue<bool>(); }
        }

        public bool AntiGapCloserEnabled
        {
            get { return ConfigMenu.Item("SAutoCarry.Azir.Misc.AntiGapCloser.Enable").GetValue<bool>(); }
        }
    }
}
