using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSaliceResurrected.Managers;
using xSaliceResurrected.Persistence;
using xSaliceResurrected.Utilities;
using Color = System.Drawing.Color;

using EloBuddy; namespace xSaliceResurrected.Mid
{
    class Orianna : Champion
    {
        //ball manager
        private bool _isBallMoving;
        private Vector3 _currentBallPosition;
        private Vector3 _allyDraw;
        private int _ballStatus;

        public Orianna()
        {
            SetupSpells();
            LoadMenu();
        }

        private void SetupSpells()
        {
            //intalize spell
            SpellManager.Q = new Spell(SpellSlot.Q, 825);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 1100);
            SpellManager.R = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(0.25f, 80, 1300, false, SkillshotType.SkillshotCircle);
            SpellManager.W.SetSkillshot(0f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(0.25f, 145, 1700, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.60f, 350, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);
        }

        private void LoadMenu()
        {
            //Keys
            var key = new Menu("Keys", "Keys");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "Farm!", true).SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitQQ", "Last hit with Q", true).SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("escape", "RUN FOR YOUR LIFE!", true).SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Spell Menu
            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                //W
                var wMenu = new Menu("WSpell", "WSpell");
                {
                    wMenu.AddItem(new MenuItem("autoW", "Use W if hit", true).SetValue(new Slider(2, 1, 5)));
                    spellMenu.AddSubMenu(wMenu);
                }

                //E
                var eMenu = new Menu("ESpell", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("saveEMana", "Do not E To save Mana for Q+W", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("UseEDmg", "Use E to Dmg", true).SetValue(true));
                    eMenu.AddSubMenu(new Menu("E Ally Inc Spell", "shield"));
                    eMenu.SubMenu("shield").AddItem(new MenuItem("eAllyIfHP", "If HP < %", true).SetValue(new Slider(40)));
                    foreach (AIHeroClient ally in ObjectManager.Get<AIHeroClient>().Where(ally => ally.IsAlly))
                        eMenu.SubMenu("shield").AddItem(new MenuItem("shield" + ally.CharData.BaseSkinName, ally.CharData.BaseSkinName, true).SetValue(true));

                    spellMenu.AddSubMenu(eMenu);
                }
                //R
                var rMenu = new Menu("RSpell", "RSpell");
                {
                    rMenu.AddItem(new MenuItem("autoR", "Use R if hit (Global check)", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("blockR", "Block R if no enemy", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("overK", "OverKill Check", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("killR", "Use R only if it hits multiple target", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));

                    rMenu.AddSubMenu(new Menu("Auto use R on", "intR"));
                    rMenu.SubMenu("intR").AddItem(new MenuItem("AdditonalTargets", "Require Addition targets", true).SetValue(new Slider(1, 0, 4)));
                    foreach (AIHeroClient enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("intR").AddItem(new MenuItem("intR" + enemy.CharData.BaseSkinName, enemy.CharData.BaseSkinName, true).SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }
                menu.AddSubMenu(spellMenu);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("autoRCombo", "Use R if hit", true).SetValue(new Slider(2, 1, 5)));
                combo.AddSubMenu(HitChanceManager.AddHitChanceMenuCombo(true, false, false, false));
                menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddSubMenu(HitChanceManager.AddHitChanceMenuHarass(true, false, false, false));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(false));
                farm.AddItem(new MenuItem("qFarm", "Only Q/W if > minion", true).SetValue(new Slider(3, 0, 5)));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                menu.AddSubMenu(farm);
            }

            //intiator list:
            var initator = new Menu("Initiator", "Initiator");
            {
                foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
                {
                    foreach (Initiator intiator in Initiator.InitatorList)
                    {
                        if (intiator.HeroName == hero.CharData.BaseSkinName)
                        {
                            initator.AddItem(new MenuItem(intiator.SpellName, intiator.SpellName, true)).SetValue(false);
                        }
                    }
                }
                menu.AddSubMenu(initator);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "Use R to Interrupt", true).SetValue(true));
            }


            //Drawings menu:
            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawing.AddItem(drawComboDamageMenu);
                drawing.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                //add to menu
                menu.AddSubMenu(drawing);
            }

            var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "ComboActive"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "HarassActive"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "HarassActiveT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClearActive"));
                customMenu.AddItem(myCust.AddToMenu("LastHit Active: ", "LastHitQQ"));
                customMenu.AddItem(myCust.AddToMenu("Escape Active: ", "escape"));
                customMenu.AddItem(myCust.AddToMenu("R Multi Only: ", "killR"));
                menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            //if (Q.LSIsReady())
            damage += Player.LSGetSpellDamage(enemy, SpellSlot.Q) * 1.5;

            if (W.LSIsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.W);

            if (E.LSIsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.E);

            if (R.LSIsReady())
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.R) - 25;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            //Orbwalker.SetAttacks(!(Q.LSIsReady()));
            UseSpells(menu.Item("UseQCombo", true).GetValue<bool>(), menu.Item("UseWCombo", true).GetValue<bool>(),
                menu.Item("UseECombo", true).GetValue<bool>(), menu.Item("UseRCombo", true).GetValue<bool>(), "Combo");
        }
        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass", true).GetValue<bool>(), menu.Item("UseWHarass", true).GetValue<bool>(),
                menu.Item("UseEHarass", true).GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, String source)
        {
            if (source == "Harass" && !ManaManager.HasMana("Harass"))
                return;

            var range = E.LSIsReady() ? E.Range : Q.Range;
            AIHeroClient target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

            if (useQ && Q.LSIsReady())
            {
                CastQ(target, source);
            }

            if (_isBallMoving)
                return;

            if (useW && target != null && W.LSIsReady())
            {
                CastW(target);
            }

            //items
            if (source == "Combo")
            {
                var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);
                if (itemTarget != null)
                {
                    var dmg = GetComboDamage(itemTarget);
                    ItemManager.Target = itemTarget;

                    //see if killable
                    if (dmg > itemTarget.Health - 50)
                        ItemManager.KillableTarget = true;

                    ItemManager.UseTargetted = true;
                }
            }

            if (useE && target != null && E.LSIsReady())
            {
                CastE(target);
            }

            if (useR && target != null && R.LSIsReady())
            {
                if (menu.Item("intR" + target.CharData.BaseSkinName, true) != null)
                {
                    foreach (
                        AIHeroClient enemy in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(x => Player.LSDistance(x.Position) < 1500 && x.LSIsValidTarget() && x.IsEnemy && !x.IsDead))
                    {
                        if (!enemy.IsDead && menu.Item("intR" + enemy.CharData.BaseSkinName, true).GetValue<bool>())
                        {
                            CastR(enemy, true);
                            return;
                        }
                    }
                }

                if (!(menu.Item("killR", true).GetValue<KeyBind>().Active)) //check if multi
                {
                    if (menu.Item("overK", true).GetValue<bool>() &&
                        (Player.LSGetSpellDamage(target, SpellSlot.Q) + Player.LSGetSpellDamage(target, SpellSlot.W)) >= target.Health)
                    {
                        return;
                    }
                    if (GetComboDamage(target) >= target.Health - 100 && !target.IsZombie)
                        CastR(target);
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            PredictionOutput prediction = Util.GetPCircle(_currentBallPosition, W, target, true);

            if (W.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) < W.Width)
            {
                W.Cast();
            }

        }

        private void CastR(Obj_AI_Base target, bool checkAdditional = false)
        {
            if (_isBallMoving) return;

            PredictionOutput prediction = Util.GetPCircle(_currentBallPosition, R, target, true);

            if (R.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) <= R.Width)
            {
                if (checkAdditional)
                {
                    var add = menu.Item("AdditonalTargets", true).GetValue<Slider>().Value + 1;

                    if (CountR() >= add)
                        R.Cast();
                }
                else
                {
                    R.Cast();
                }
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            if (menu.Item("saveEMana", true).GetValue<bool>() && Player.Mana - ESpell.SData.Mana < QSpell.SData.Mana + WSpell.SData.Mana)
                return;

            AIHeroClient etarget = Player;

            switch (_ballStatus)
            {
                case 0:
                    if (target != null)
                    {
                        float travelTime = target.LSDistance(Player.ServerPosition) / Q.Speed;
                        float minTravelTime = 10000f;

                        foreach (
                            AIHeroClient ally in
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(x => x.IsAlly && Player.LSDistance(x.ServerPosition) <= E.Range && !x.IsMe))
                        { 
                            //dmg enemy with E
                            if (menu.Item("UseEDmg", true).GetValue<bool>())
                            {
                                PredictionOutput prediction3 = Util.GetP(Player.ServerPosition, E, target, true);
                                Object[] obj = Util.VectorPointProjectionOnLineSegment(Player.ServerPosition.LSTo2D(),
                                    ally.ServerPosition.LSTo2D(), prediction3.UnitPosition.LSTo2D());
                                var isOnseg = (bool)obj[2];
                                var pointLine = (Vector2)obj[1];

                                if (E.LSIsReady() && isOnseg &&
                                    prediction3.UnitPosition.LSDistance(pointLine.To3D()) < E.Width)
                                {
                                    //Console.WriteLine("Dmg 1");
                                    E.CastOnUnit(ally);
                                    return;
                                }
                            }

                            float allyRange = target.LSDistance(ally.ServerPosition) / Q.Speed +
                                                ally.LSDistance(Player.ServerPosition) / E.Speed;
                            if (allyRange < minTravelTime)
                            {
                                etarget = ally;
                                minTravelTime = allyRange;
                            }
                        }

                        if (minTravelTime < travelTime && Player.LSDistance(etarget.ServerPosition) <= E.Range &&
                            E.LSIsReady())
                        {
                            E.CastOnUnit(etarget);
                        }
                    }
                    break;
                case 1:
                    //dmg enemy with E
                    if (menu.Item("UseEDmg", true).GetValue<bool>())
                    {
                        PredictionOutput prediction = Util.GetP(_currentBallPosition, E, target, true);
                        Object[] obj = Util.VectorPointProjectionOnLineSegment(_currentBallPosition.LSTo2D(),
                            Player.ServerPosition.LSTo2D(), prediction.UnitPosition.LSTo2D());
                        var isOnseg = (bool)obj[2];
                        var pointLine = (Vector2)obj[1];

                        if (E.LSIsReady() && isOnseg && prediction.UnitPosition.LSDistance(pointLine.To3D()) < E.Width)
                        {
                            //Console.WriteLine("Dmg 2");
                            E.CastOnUnit(Player);
                            return;
                        }
                    }

                    float travelTime2 = target.LSDistance(_currentBallPosition) / Q.Speed;
                    float minTravelTime2 = target.LSDistance(Player.ServerPosition) / Q.Speed +
                                            Player.LSDistance(_currentBallPosition) / E.Speed;

                    if (minTravelTime2 < travelTime2 && target.LSDistance(Player.ServerPosition) <= Q.Range + Q.Width &&
                        E.LSIsReady())
                    {
                        E.CastOnUnit(Player);
                    }

                    break;
                case 2:
                    float travelTime3 = target.LSDistance(_currentBallPosition) / Q.Speed;
                    float minTravelTime3 = 10000f;

                    foreach (
                        AIHeroClient ally in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(x => x.IsAlly && Player.LSDistance(x.ServerPosition) <= E.Range && !x.IsMe))
                    {
                        //dmg enemy with E
                        if (menu.Item("UseEDmg", true).GetValue<bool>())
                        {
                            PredictionOutput prediction2 = Util.GetP(_currentBallPosition, E, target, true);
                            Object[] obj = Util.VectorPointProjectionOnLineSegment(_currentBallPosition.LSTo2D(),
                                ally.ServerPosition.LSTo2D(), prediction2.UnitPosition.LSTo2D());
                            var isOnseg = (bool)obj[2];
                            var pointLine = (Vector2)obj[1];

                            if (E.LSIsReady() && isOnseg &&
                                prediction2.UnitPosition.LSDistance(pointLine.To3D()) < E.Width)
                            {
                                Console.WriteLine("Dmg 3");
                                E.CastOnUnit(ally);
                                return;
                            }
                        }

                        float allyRange2 = target.LSDistance(ally.ServerPosition) / Q.Speed +
                                            ally.LSDistance(_currentBallPosition) / E.Speed;

                        if (allyRange2 < minTravelTime3)
                        {
                            etarget = ally;
                            minTravelTime3 = allyRange2;
                        }
                    }

                    if (minTravelTime3 < travelTime3 && Player.LSDistance(etarget.ServerPosition) <= E.Range &&
                        E.LSIsReady())
                    {
                        E.CastOnUnit(etarget);
                    }

                    break;
            }
        }

        private void CastQ(Obj_AI_Base target, String source)
        {
            if (_isBallMoving || !target.LSIsValidTarget(Q.Range)) return;

            PredictionOutput prediction = Util.GetP(_currentBallPosition, Q, target,  true);

            if (Q.LSIsReady() && prediction.Hitchance >= HitChanceManager.GetQHitChance(source) && Player.LSDistance(target.Position) <= Q.Range)
            {
                Q.Cast(prediction.CastPosition);
            }
        }

        private void CheckWMec()
        {
            if (!W.LSIsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoW", true).GetValue<Slider>().Value;

            int hit = (from x in ObjectManager.Get<AIHeroClient>().Where(champ => champ.LSIsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, W, x, true)).Count(prediction => W.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) < W.Width);

            if (hit >= minHit && W.LSIsReady())
                W.Cast();
        }

        private void CheckRMec()
        {
            if (!R.LSIsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoRCombo", true).GetValue<Slider>().Value;

            int hit = (from x in ObjectManager.Get<AIHeroClient>().Where(champ => champ.LSIsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, R, x, true)).Count(prediction => R.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) < R.Width);

            if (hit >= minHit && R.LSIsReady())
                R.Cast();
        }

        private void CheckRMecGlobal()
        {
            if (!R.LSIsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoR", true).GetValue<Slider>().Value;

            int hit = (from x in ObjectManager.Get<AIHeroClient>().Where(champ => champ.LSIsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, R, x, true)).Count(prediction => R.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) < R.Width);


            if (hit >= minHit && R.LSIsReady())
                R.Cast();
        }

        private int CountR()
        {
            if (!R.LSIsReady())
                return 0;

            return (from enemy in ObjectManager.Get<AIHeroClient>().Where(champ => champ.LSIsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                    select Util.GetPCircle(_currentBallPosition, R, enemy, true)).Count(prediction => R.LSIsReady() && prediction.UnitPosition.LSDistance(_currentBallPosition) <= R.Width);
        }

        private void LastHit()
        {
            if (!OrbwalkManager.CanMove(40)) return;

            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (Q.LSIsReady())
            {
                foreach (Obj_AI_Base minion in allMinions)
                {
                    if (minion.LSIsValidTarget() &&
                        HealthPrediction.GetHealthPrediction(minion, (int)(Player.LSDistance(minion.Position) * 1000 / 1400)) <
                        Player.LSGetSpellDamage(minion, SpellSlot.Q) - 10)
                    {
                        PredictionOutput prediction = Util.GetP(_currentBallPosition, Q, minion, true);

                        if (prediction.Hitchance >= HitChance.High && Q.LSIsReady())
                            Q.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        private void Farm()
        {
            if (!OrbwalkManager.CanMove(40)) return;

            if (!ManaManager.HasMana("Farm"))
                return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);

            var useQ = menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = menu.Item("UseWFarm", true).GetValue<bool>();
            int min = menu.Item("qFarm", true).GetValue<Slider>().Value;

            if (useQ && Q.LSIsReady())
            {
                Q.From = _currentBallPosition;

                MinionManager.FarmLocation pred = Q.GetCircularFarmLocation(allMinionsQ, Q.Width + 15);

                if (pred.MinionsHit >= min)
                    Q.Cast(pred.Position);
            }

            int hit = 0;
            if (useW && W.LSIsReady())
            {
                hit += allMinionsW.Count(enemy => enemy.LSDistance(_currentBallPosition) < W.Width);

                if (hit >= min && W.LSIsReady())
                    W.Cast();
            }
        }

        private void Escape()
        {
            OrbwalkManager.Orbwalk(null, Game.CursorPos);

            if (_ballStatus == 0 && W.LSIsReady())
                W.Cast();
            else if (E.LSIsReady() && _ballStatus != 0)
                E.CastOnUnit(Player);
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            OnGainBuff();

            CheckRMecGlobal();

            if (menu.Item("escape", true).GetValue<KeyBind>().Active)
            {
                Escape();
            }
            else if (menu.Item("ComboActive", true).GetValue<KeyBind>().Active)
            {
                CheckRMec();
                Combo();
            }
            else
            {
                if (menu.Item("HarassActive", true).GetValue<KeyBind>().Active ||
                    menu.Item("HarassActiveT", true).GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("LaneClearActive", true).GetValue<KeyBind>().Active)
                {
                    Farm();
                }

                if (menu.Item("LastHitQQ", true).GetValue<KeyBind>().Active)
                {
                    LastHit();
                }
            }

            CheckWMec();
        }

        private void OnGainBuff()
        {
            if (Player.LSHasBuff("OrianaGhostSelf"))
            {
                _ballStatus = 0;
                _currentBallPosition = Player.ServerPosition;
                _isBallMoving = false;
                return;
            }

            foreach (AIHeroClient ally in
                ObjectManager.Get<AIHeroClient>()
                    .Where(ally => ally.IsAlly && !ally.IsDead && ally.LSHasBuff("orianaghost", true)))
            {
                _ballStatus = 2;
                _currentBallPosition = ally.ServerPosition;
                _allyDraw = ally.Position;
                _isBallMoving = false;
                return;
            }

            _ballStatus = 1;
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            foreach (Spell spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "Range", true).GetValue<Circle>();
                if ((spell.Slot == SpellSlot.R && menuItem.Active) || (spell.Slot == SpellSlot.W && menuItem.Active))
                {
                    if (_ballStatus == 0)
                        Render.Circle.DrawCircle(Player.Position, spell.Width, spell.LSIsReady() ? Color.Aqua : Color.Red);
                    else if (_ballStatus == 2)
                        Render.Circle.DrawCircle(_allyDraw, spell.Width, spell.LSIsReady() ? Color.Aqua : Color.Red);
                    else
                        Render.Circle.DrawCircle(_currentBallPosition, spell.Width, spell.LSIsReady() ? Color.Aqua : Color.Red);
                }
                else if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, spell.LSIsReady() ? Color.Aqua : Color.Red);
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            //Shield Ally
            if (!menu.Item("saveEMana", true).GetValue<bool>() || Player.Mana - ESpell.SData.Mana >= QSpell.SData.Mana + WSpell.SData.Mana)
            {
                if (unit.IsEnemy && unit.Type == GameObjectType.AIHeroClient && E.LSIsReady())
                {
                    foreach (
                        AIHeroClient ally in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(
                                    x =>
                                        Player.LSDistance(x.Position) < E.Range && Player.LSDistance(unit.Position) < 1500 &&
                                        x.IsAlly && !x.IsDead).OrderBy(x => x.LSDistance(args.End)))
                    {
                        if (menu.Item("shield" + ally.CharData.BaseSkinName, true) != null)
                        {
                            if (menu.Item("shield" + ally.CharData.BaseSkinName, true).GetValue<bool>())
                            {
                                int hp = menu.Item("eAllyIfHP", true).GetValue<Slider>().Value;

                                if (ally.LSDistance(args.End) < 500 && ally.HealthPercent <= hp)
                                {
                                    //Chat.Print("shielding");
                                    E.CastOnUnit(ally);
                                    _isBallMoving = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //intiator
            if (unit.IsAlly)
            {
                if (Initiator.InitatorList.Where(spell => args.SData.Name == spell.SDataName).Where(spell => menu.Item(spell.SpellName, true).GetValue<bool>()).Any(spell => E.LSIsReady() && Player.LSDistance(unit.Position) < E.Range))
                {
                    E.CastOnUnit(unit);
                    _isBallMoving = true;
                    return;
                }
            }

            if (!unit.IsMe) return;

            SpellSlot castedSlot = ObjectManager.Player.LSGetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.Q)
            {
                _isBallMoving = true;
                LeagueSharp.Common.Utility.DelayAction.Add(
                    (int)Math.Max(1, 1000 * (args.End.LSDistance(_currentBallPosition) - Game.Ping - 0.1) / Q.Speed), () =>
                    {
                        _currentBallPosition = args.End;
                        _ballStatus = 1;
                        _isBallMoving = false;
                        //Chat.Print("Stopped");
                    });
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!menu.Item("UseInt", true).GetValue<bool>() || _isBallMoving) return;

            if (Player.LSDistance(unit.Position) < R.Width)
            {
                CastR(unit);
            }
            else
            {
                CastQ(unit, "Combo");
            }
        }

        protected override void SpellbookOnOnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != SpellSlot.R)
                return;

            if (_isBallMoving)
                args.Process = false;

            if (CountR() == 0 && menu.Item("blockR", true).GetValue<bool>())
            {
                //Block packet if enemies hit is 0
                args.Process = false;
            }
        }
    }
}
