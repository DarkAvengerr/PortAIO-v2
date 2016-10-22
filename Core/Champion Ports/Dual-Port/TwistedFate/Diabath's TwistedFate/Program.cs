#region
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace D_TwistedFate
{
    internal class Program
    {
        private static Menu Config;

        private const string ChampionName = "TwistedFate";
        private static Spell Q ,W;
        private static readonly float Qangle = 28 * (float)Math.PI / 180;
        private static Orbwalking.Orbwalker SOW;
        private static Vector2 PingLocation;
        private static int LastPingT = 0;
        private static AIHeroClient Player;
        private static int CastQTick;
        private static SpellSlot _igniteSlot;
        public static Dictionary<SpellSlot, int> LastCast = new Dictionary<SpellSlot, int>();

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(400, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
        }

        private static int timeChangeW;

        private static int timeToggleGold;

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
        }

        private static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;
            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W);
            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);

            _igniteSlot = Player.GetSpellSlot("SummonerDot");
            //Make the menu
            Config = new Menu("Twisted Fate", "TwistedFate", true);

            var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Config.AddSubMenu(TargetSelectorMenu);

            var SowMenu = new Menu("Orbwalking", "Orbwalking");
            SOW = new Orbwalking.Orbwalker(SowMenu);
            Config.AddSubMenu(SowMenu);

            /* Q */
            var q = new Menu("Q - Wildcards", "Q");
            {
                q.AddItem(new MenuItem("QC", "Use Q Combo").SetValue(true));
                q.AddItem(new MenuItem("QFarm", "Use Q Farm").SetValue(true));
                q.AddItem(new MenuItem("Lanemana", "Minimum Mana to Use Q").SetValue(new Slider(60, 1, 100)));
                q.AddItem(new MenuItem("QMinions", "Only use Q to Farm if > X Minions in Range"))
                    .SetValue(new Slider(1, 0, 5));
                q.AddItem(new MenuItem("AutoQI", "Auto-Q Immobile").SetValue(true));
                q.AddItem(new MenuItem("AutoQD", "Auto-Q Dashing").SetValue(true));
                q.AddItem(new MenuItem("CastQ", "Cast Q (Tap)").SetValue(new KeyBind('U', KeyBindType.Press)));
                Config.AddSubMenu(q);
            }

            /* W */
            var w = new Menu("W - Pick a Card", "W");
            {
                w.AddItem(new MenuItem("SelectCard", "Card to Pick ").SetValue(new Slider(2, 0, 2)));
                w.AddItem(new MenuItem("Info", "0 = Blue, 1 = Red, 2 = Gold"));
                w.AddItem(
                  new MenuItem("SelectRed", "Select Red Manual").SetValue(
                      new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
                w.AddItem(
                    new MenuItem("SelectYellow", "Select Yellow Manual").SetValue(
                        new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
                w.AddItem(
                    new MenuItem("SelectBlue", "Select Blue Manual").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                w.AddItem(new MenuItem("ChangeCard", "Change Card Pick").SetValue(new KeyBind('T', KeyBindType.Press)));
                Config.AddSubMenu(w);
                w.AddItem(new MenuItem("WFarm", "W Farm Key").SetValue(true));
                w.AddItem(
                    new MenuItem("AlwaysBlue", "Always use Blue Card to Farm if Mana < x%").SetValue(
                        new Slider(20, 0, 100)));
                w.AddItem(new MenuItem("RedMinions", "Only use Red Card to Farm if > X Minions in Range"))
                    .SetValue(new Slider(1, 0, 5));

                w.AddItem(new MenuItem("AlwaysGold", "Always use Gold Card in Combo?").SetValue(false));
                w.AddItem(new MenuItem("ToggleGold", "^ Toggle ON/OFF").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            }

            var menuItems = new Menu("Items", "Items");
            {
                menuItems.AddItem(new MenuItem("UseIgnitecombo", "Use Ignite").SetValue(true));
                menuItems.AddItem(new MenuItem("ignitehp", "use ignite if Enemy HP%<").SetValue(new Slider(30, 1, 100)));
                menuItems.AddItem(new MenuItem("itemBotrk", "BotRK").SetValue(true));
                menuItems.AddItem(new MenuItem("itemYoumuu", "Youmuu").SetValue(true));
                menuItems.AddItem(
                    new MenuItem("itemMode", "Use Items On").SetValue(
                        new StringList(new[] { "No", "Mixed-mode", "Combo-mode", "Both" }, 2)));

                menuItems.AddSubMenu(new Menu("Potions", "Potions"));
                menuItems.SubMenu("Potions")
                    .AddItem(new MenuItem("usehppotions", "Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"))
                    .SetValue(true);
                menuItems.SubMenu("Potions")
                    .AddItem(new MenuItem("usepotionhp", "If Health % <").SetValue(new Slider(35, 1, 100)));
                menuItems.SubMenu("Potions")
                    .AddItem(new MenuItem("usemppotions", "Use Hunters/Corrupting/Biscuit"))
                    .SetValue(true);
                menuItems.SubMenu("Potions")
                    .AddItem(new MenuItem("usepotionmp", "If Mana % <").SetValue(new Slider(35, 1, 100)));
                Config.AddSubMenu(menuItems);
            }

            var r = new Menu("R - Destiny", "R");
            {
                r.AddItem(new MenuItem("AutoY", "Select Gold Card after R").SetValue(true));
                Config.AddSubMenu(r);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("PingLH", "Ping Low Health Enemies (only local)").SetValue(true));
                misc.AddItem(new MenuItem("DisplayLH", "Notify on Low Health Enemies").SetValue(false));
                Config.AddSubMenu(misc);
            }

            //Damage after combo:
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw Damage after Combo").SetValue(true);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            /*Drawing*/
            var drawings = new Menu("Drawings", "Drawings");
            {
                drawings.AddItem(
                    new MenuItem("Qcircle", "Q Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawings.AddItem(
                    new MenuItem("Rcircle", "R Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
                drawings.AddItem(
                    new MenuItem("Rcircle2", "R Range (on minimap)").SetValue(
                        new Circle(true, Color.FromArgb(255, 255, 255, 255))));
                drawings.AddItem(new MenuItem("drawtext", "Draw Texts")).SetValue(false);
                drawings.AddItem(dmgAfterComboItem);
                Config.AddSubMenu(drawings);
            }

            Config.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddToMainMenu();

            Config.Item("SelectCard").SetValue(new Slider(0, 0, 2));
            timeChangeW = Environment.TickCount;
            timeToggleGold = Environment.TickCount;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
            Chat.Print("<font color='#881df2'>Blm95 TwistedFate reworked by Diabaths</font> Loaded.");
            Chat.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Chat.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
        }
        
        private static void Usepotion()
        {
            var iusehppotion = Config.Item("usehppotions").GetValue<bool>();
            var iusepotionhp = Player.Health
                               <= (Player.MaxHealth * (Config.Item("usepotionhp").GetValue<Slider>().Value) / 100);
            var iusemppotion = Config.Item("usemppotions").GetValue<bool>();
            var iusepotionmp = Player.Mana
                               <= (Player.MaxMana * (Config.Item("usepotionmp").GetValue<Slider>().Value) / 100);
            if (Player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (LeagueSharp.Common.Utility.CountEnemiesInRange(800) > 0)
            {
                if (iusepotionhp && iusehppotion
                    && !(ObjectManager.Player.HasBuff("RegenerationPotion")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")))
                {
                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }

                    if (Items.HasItem(2031) && Items.CanUseItem(2031))
                    {
                        Items.UseItem(2031);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }

                if (iusepotionmp && iusemppotion
                    && !(ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }

                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }
            }
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is AIHeroClient)
                args.Process = CardSelector.Status != SelectStatus.Selecting &&
                               Utils.TickCount - CardSelector.LastWSent > 300;
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Gate" && Config.Item("AutoY").GetValue<bool>())
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var rCircle2 = Config.Item("Rcircle2").GetValue<Circle>();
            if (rCircle2.Active)
            {
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 5500, rCircle2.Color, 1, 23, true);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qCircle = Config.Item("Qcircle").GetValue<Circle>();
            var rCircle = Config.Item("Rcircle").GetValue<Circle>();

            if (qCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, qCircle.Color);
            }

            if (rCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 5500, rCircle.Color);
            }

            if (Config.Item("drawtext").GetValue<bool>())
            {
                Vector2 screenPos = Drawing.WorldToScreen(Player.Position);
                switch (Config.Item("SelectCard").GetValue<Slider>().Value)
                {
                    case 0:
                        Drawing.DrawText(screenPos.X, screenPos.Y, Color.Blue, "Blue Card");
                        break;
                    case 1:
                        Drawing.DrawText(screenPos.X, screenPos.Y, Color.Red, "Red Card");
                        break;
                    case 2:
                        Drawing.DrawText(screenPos.X, screenPos.Y, Color.Yellow, "Gold Card");
                        break;
                }

                if (Config.Item("AlwaysGold").GetValue<bool>())
                {
                    Drawing.DrawText(screenPos.X, screenPos.Y + 13, Color.Yellow, "Always Combo Gold");
                }
            }

            Vector2 screenPoss = Drawing.WorldToScreen(Player.Position);
            if (Config.Item("DisplayLH").GetValue<bool>())
            {
                var ydiff = 13;
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                    h.IsValidTarget() && ComboDamage(h) > h.Health))
                {
                    Drawing.DrawText(screenPoss.X, screenPoss.Y + ydiff + 13, Color.White, enemy.Name);
                    ydiff += 13;
                }
            }
        }


        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range * (position - startPoint).Normalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0) endPoint = originalEndPoint;
                    if (k == 1) endPoint = startPoint + originalDirection.Rotated(Qangle);
                    if (k == 2) endPoint = startPoint + originalDirection.Rotated(-Qangle);

                    if (point.Distance(startPoint, endPoint, true, true) <
                        (Q.Width + hitBoxes[i]) * (Q.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }

        private static void CastQ(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range * (unitPosition - startPoint).Normalized();

            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Q.GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int)enemy.BoundingRadius);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0) posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                if (i == 1) posiblePositions.Add(startPoint + originalDirection.Rotated(Qangle));
                if (i == 2) posiblePositions.Add(startPoint + originalDirection.Rotated(-Qangle));
            }


            if (startPoint.Distance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = (2/3 * (unit.BoundingRadius + Q.Width));
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

            Q.Cast(bestPosition.To3D(), true);
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q) * 2;
            dmg += Player.GetSpellDamage(hero, SpellSlot.W);
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            
            if (Config.Item("PingLH").GetValue<bool>())
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                    h.IsValidTarget() && ComboDamage(h) > h.Health))
                {
                    Ping(enemy.Position.To2D());
                }

            if (Config.Item("CastQ").GetValue<KeyBind>().Active)
            {
                CastQTick = Utils.TickCount;
            }

            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Utils.TickCount - CastQTick < 500)
            {
                if (qTarget != null && Config.Item("QC").GetValue<bool>())
                {
                    Q.Cast(qTarget);
                }
            }

            var combo = Config.Item("Combo").GetValue<KeyBind>().Active;

            //Select cards.
            if (Environment.TickCount - timeChangeW > 120)
            {
                timeChangeW = Environment.TickCount;
                if (Config.Item("ChangeCard").GetValue<KeyBind>().Active)
                {
                    var oldCard = Config.Item("SelectCard").GetValue<Slider>().Value;

                    if (oldCard != 2)
                    {
                        Config.Item("SelectCard").SetValue(new Slider(oldCard + 1, 0, 2));
                    }
                    else
                    {
                        Config.Item("SelectCard").SetValue(new Slider(0, 0, 2));
                    }
                }
            }

            if (Environment.TickCount - timeToggleGold > 300)
            {
                timeToggleGold = Environment.TickCount;
                if (Config.Item("ToggleGold").GetValue<KeyBind>().Active)
                {
                    var CardToggle = Config.Item("AlwaysGold").GetValue<bool>();
                    Config.Item("AlwaysGold").SetValue(!CardToggle);
                    Config.Item("ToggleGold").SetValue(new KeyBind(Config.Item("ToggleGold").GetValue<KeyBind>().Key, KeyBindType.Toggle));
                }
            }

            if (combo)
            {
                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                var qmana = Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                var wmana = Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                if(!t.IsValidTarget(Q.Range)) return;
                if (!Config.Item("AlwaysGold").GetValue<bool>() && Player.Mana > qmana + wmana)
                {
                    switch (Config.Item("SelectCard").GetValue<Slider>().Value)
                    {
                        case 0:
                            CardSelector.StartSelecting(Cards.Blue);
                            break;
                        case 1:
                            CardSelector.StartSelecting(Cards.Red);
                            break;
                        case 2:
                            CardSelector.StartSelecting(Cards.Yellow);
                            break;
                    }
                }
               
                else if (Player.Mana < qmana + wmana)
                {
                    CardSelector.StartSelecting(Cards.Blue);
                }
                else
                {
                    CardSelector.StartSelecting(Cards.Yellow);
                }

                if (qTarget != null && (qTarget.MoveSpeed < 250 || qTarget.IsStunned || !qTarget.CanMove || qTarget.IsRooted ||
                    qTarget.IsCharmed || qTarget.Distance(Player) < 400) && Config.Item("QC").GetValue<bool>())
                {
                    Q.Cast(qTarget);
                }
            }

            if (SOW.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = MinionManager.GetMinions(
                         ObjectManager.Player.ServerPosition,
                         Q.Range,
                         MinionTypes.All);
                foreach (var minion in Minions)
                {
                    if (Config.Item("QFarm").GetValue<bool>() && Q.IsReady()
                        && 100 * (Player.Mana / Player.MaxMana) > Config.Item("Lanemana").GetValue<Slider>().Value)
                    {
                        var farm = Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range));
                        if (farm.MinionsHit >= Config.Item("QMinions").GetValue<Slider>().Value)
                        {
                            Q.Cast(farm.Position, true);
                        }
                    }

                    if (Config.Item("WFarm").GetValue<bool>() && W.IsReady()
                        && minion.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        if (Player.ManaPercent < Config.Item("AlwaysBlue").GetValue<Slider>().Value)
                        {
                            CardSelector.StartSelecting(Cards.Blue);
                        }

                    else
                        {
                            switch (Config.Item("SelectCard").GetValue<Slider>().Value)
                            {
                                case 0:
                                    CardSelector.StartSelecting(Cards.Blue);
                                    break;
                                case 1:
                                    if (Minions.Count > Config.Item("RedMinions").GetValue<Slider>().Value)
                                    {
                                        CardSelector.StartSelecting(Cards.Red);
                                    }

                                    break;
                                case 2:
                                    CardSelector.StartSelecting(Cards.Yellow);
                                    break;
                            }
                        }
                    }
                }
            }

            if (Config.Item("SelectYellow").GetValue<KeyBind>().Active)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }

            if (Config.Item("SelectBlue").GetValue<KeyBind>().Active)
            {
                CardSelector.StartSelecting(Cards.Blue);
            }

            if (Config.Item("SelectRed").GetValue<KeyBind>().Active)
            {
                CardSelector.StartSelecting(Cards.Red);
            }

            /*
                        if (CardSelector.Status == SelectStatus.Selected && combo)
                        {
                            var target = SOW.GetTarget();
                            if (target.IsValidTarget() && target is AIHeroClient && Items.HasItem("DeathfireGrasp") && ComboDamage((AIHeroClient)target) >= target.Health)
                            {
                                Items.UseItem("DeathfireGrasp", (AIHeroClient) target);
                            }
                        }
            */

            //Auto Q
            var autoQI = Config.Item("AutoQI").GetValue<bool>();
            var autoQD = Config.Item("AutoQD").GetValue<bool>();


            if (ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready && (autoQD || autoQI))
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.IsValidTarget(Q.Range * 2))
                    {
                        var pred = Q.GetPrediction(enemy);
                        if ((pred.Hitchance == HitChance.Immobile && autoQI) ||
                            (pred.Hitchance == HitChance.Dashing && autoQD))
                        {
                            CastQ(enemy, pred.UnitPosition.To2D());
                        }
                    }
                }


            var useItemModes = Config.Item("itemMode").GetValue<StringList>().SelectedIndex;
            if (
                !((SOW.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                   (useItemModes == 2 || useItemModes == 3))
                  ||
                  (SOW.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;
            var UseIgnitecombo = Config.Item("UseIgnitecombo").GetValue<bool>();
            var botrk = Config.Item("itemBotrk").GetValue<bool>();
            var youmuu = Config.Item("itemYoumuu").GetValue<bool>();
            var target = SOW.GetTarget() as Obj_AI_Base;

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass ||
                            ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth &&
                            Items.CanUseItem(itemId))
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (youmuu && target != null && target.Type == ObjectManager.Player.Type &&
                Orbwalking.InAutoAttackRange(target) && Items.CanUseItem(3142))
                Items.UseItem(3142);

            if (target.IsValidTarget(600) && _igniteSlot != SpellSlot.Unknown && UseIgnitecombo
                && Player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (target.HealthPercent <= Config.Item("ignitehp").GetValue<Slider>().Value)
                {
                    Player.Spellbook.CastSpell(_igniteSlot, target);
                }

            }

            Usepotion();
        }
    }
}
