using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SCommon.Orbwalking;
using SCommon.Database;
using SUtility.Drawings;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class TwistedFate : SCommon.PluginBase.Champion
    {
        private const float Qangle = 28 * (float)Math.PI / 180;

        public TwistedFate()
            : base ("TwistedFate", "SAutoCarry - Twisted Fate")
        {
            Helpers.CardMgr.Initialize(this);
            OnCombo += Combo;
            OnHarass += Harass;
            OnDraw += BeforeDraw;
            OnUpdate += BeforeOrbwalk;

            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.TwistedFate.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.TwistedFate.Combo.UseQImmobile", "Auto Q Immobile").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.TwistedFate.Combo.UseQDashing", "Auto Q Immobile").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.TwistedFate.Combo.UseW", "Use W").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.TwistedFate.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.TwistedFate.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.TwistedFate.Harass.UseW", "Use W (Blue Always)").SetValue(true));

            Menu misc = new Menu("Misc", "SAutoCarry.TwistedFate.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.SelectGold", "Select Gold Card").SetValue(new KeyBind('W', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.SelectBlue", "Select Blue Card").SetValue(new KeyBind('E', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.SelectRed", "Select Red Card").SetValue(new KeyBind('T', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.SelectGoldAfterR", "Select Gold Card After R").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.AntiGapCloser", "Anti Gap Closer").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.Interrupter", "Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.DrawR", "Draw R Range").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.TwistedFate.Misc.DrawRMinimap", "Draw R Range On Minimap").SetValue(true));
            
            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 1450);
            Spells[Q].SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W);

            Spells[E] = new Spell(SpellSlot.E);

            Spells[R] = new Spell(SpellSlot.R);
        }

        public void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[Q].Range * 2f, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if(t != null)
            {
                if(Spells[Q].LSIsReady() && (ComboUseQDashing || ComboUseQImmobile))
                {
                    if ((t.IsImmobilized() && ComboUseQImmobile) || (t.LSIsDashing() && ComboUseQDashing))
                        CastQ(t, t.ServerPosition.LSTo2D());
                }

                if(Spells[W].LSIsReady() && ComboUseW)
                {
                    Helpers.CardMgr.Select(FindCardToSelect(t));
                }
            }
        }

        public void Harass()
        {
            var t = TargetSelector.GetTarget(ObjectManager.Player.AttackRange + 250, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if (t != null)
            {
                if (Spells[W].LSIsReady() && HarassUseW)
                    Helpers.CardMgr.Select(Helpers.CardMgr.Card.Blue);

                if (Spells[Q].LSIsReady() && HarassUseQ)
                    Spells[Q].Cast(t);
            }
        }

        private Helpers.CardMgr.Card FindCardToSelect(AIHeroClient t)
        {
            var blueDamage = Spells[W].GetDamage(t);
            var redDamage = Spells[W].GetDamage(t, 1);
            var goldDamage = Spells[W].GetDamage(t, 2) + (Spells[Q].LSIsReady() && ObjectManager.Player.Mana - Spells[W].ManaCost - Spells[Q].ManaCost >= 0 ? Spells[Q].GetDamage(t) : 0);

            if (ObjectManager.Player.Mana - Spells[W].ManaCost < Spells[Q].ManaCost - 5 && ObjectManager.Player.LSCountAlliesInRange(1000) == 0 && Spells[Q].LSIsReady(1000))
                return Helpers.CardMgr.Card.Blue;

            if (t.HealthPercent - goldDamage > 0 && ObjectManager.Player.Mana - Spells[W].ManaCost - Spells[Q].ManaCost <= 0 && ObjectManager.Player.LSCountAlliesInRange(1000) == 0)
                return Helpers.CardMgr.Card.Blue;

            if (t.Health - blueDamage + 50 < 0)
                return Helpers.CardMgr.Card.Blue;
            /*else if (t.HealthPercent - redDamage + 50 < 0)
                return CardMgr.Card.Red;*/
            else if (t.HealthPercent - goldDamage + 50 < 0)
                return Helpers.CardMgr.Card.Gold;

            return Helpers.CardMgr.Card.Gold;
        }

        #region Kortatu's Q Code
        private void CastQ(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = ObjectManager.Player.ServerPosition.LSTo2D();
            var originalDirection = Spells[Q].Range * (unitPosition - startPoint).LSNormalized();

            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (enemy.LSIsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Spells[Q].GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.LSTo2D());
                        hitBoxes.Add((int)enemy.BoundingRadius);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0) posiblePositions.Add(unitPosition + originalDirection.LSRotated(0));
                if (i == 1) posiblePositions.Add(startPoint + originalDirection.LSRotated(Qangle));
                if (i == 2) posiblePositions.Add(startPoint + originalDirection.LSRotated(-Qangle));
            }


            if (startPoint.LSDistance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).LSNormalized().LSPerpendicular();
                    var k = (2 / 3 * (unit.BoundingRadius + Spells[Q].Width));
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
                }
            }

            var bestPosition = new Vector2();
            var bestHit = -1;

            foreach (var position in posiblePositions)
            {
                var hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit + 1 <= minTargets)
                return;

            Spells[Q].Cast(bestPosition.To3D(), true);
        }

        private int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.LSTo2D();
            var originalDirection = Spells[Q].Range * (position - startPoint).LSNormalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0) endPoint = originalEndPoint;
                    if (k == 1) endPoint = startPoint + originalDirection.LSRotated(Qangle);
                    if (k == 2) endPoint = startPoint + originalDirection.LSRotated(-Qangle);

                    if (point.LSDistance(startPoint, endPoint, true, true) <
                        (Spells[Q].Width + hitBoxes[i]) * (Spells[Q].Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }
        #endregion

        public void BeforeOrbwalk()
        {
            if (SelectGoldCard)
                Helpers.CardMgr.Select(Helpers.CardMgr.Card.Gold);

            if (SelectBlueCard)
                Helpers.CardMgr.Select(Helpers.CardMgr.Card.Blue);

            if (SelectRedCard)
                Helpers.CardMgr.Select(Helpers.CardMgr.Card.Red);
        }

        public void BeforeDraw()
        {
            if(DrawR)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 5500, System.Drawing.Color.FromArgb(100, 255, 255, 255));
        }

        protected override void OrbwalkingEvents_BeforeAttack(BeforeAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
                args.Process = Helpers.CardMgr.CanProcessAttack;
        }

        protected override void OrbwalkingEvents_AfterAttack(AfterAttackArgs args)
        {
            Orbwalker.ForcedTarget = null;
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "gate" && SelectGoldAfterR)
                Helpers.CardMgr.Select(Helpers.CardMgr.Card.Gold);
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if ((Helpers.CardMgr.CurrentCard == Helpers.CardMgr.Card.Gold || Helpers.CardMgr.CurrentCard == Helpers.CardMgr.Card.Red) && gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < ObjectManager.Player.AttackRange && AntiGapCloser)
            {
                Orbwalker.ForcedTarget = gapcloser.Sender;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        protected override void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Helpers.CardMgr.CurrentCard == Helpers.CardMgr.Card.Gold && sender.LSDistance(ObjectManager.Player.ServerPosition) < ObjectManager.Player.AttackRange && Interrupter)
            {
                Orbwalker.ForcedTarget = sender;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if(DrawRMinimap)
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 5500, System.Drawing.Color.FromArgb(255, 255, 255, 255), 1, 23, true);
        }

        public override double CalculateDamageQ(AIHeroClient target)
        {
            return base.CalculateDamageQ(target) * 2;
        }

        public bool ComboUseQImmobile
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Combo.UseQImmobile").GetValue<bool>(); }
        }

        public bool ComboUseQDashing
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Combo.UseQDashing").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Combo.UseW").GetValue<bool>(); }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Harass.UseW").GetValue<bool>(); }
        }

        public bool SelectGoldCard
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.SelectGold").GetValue<KeyBind>().Active; }
        }

        public bool SelectBlueCard
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.SelectBlue").GetValue<KeyBind>().Active; }
        }                                             
                                                      
        public bool SelectRedCard                 
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.SelectRed").GetValue<KeyBind>().Active; }
        }

        public bool SelectGoldAfterR
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.SelectGoldAfterR").GetValue<bool>(); }
        }
        
        public bool AntiGapCloser
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.AntiGapCloser").GetValue<bool>(); }
        }

        public bool Interrupter
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.Interrupter").GetValue<bool>(); }
        }

        public bool DrawR
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.DrawR").GetValue<bool>(); }
        }

        public bool DrawRMinimap
        {
            get { return ConfigMenu.Item("SAutoCarry.TwistedFate.Misc.DrawRMinimap").GetValue<bool>(); }
        }
    }
}
