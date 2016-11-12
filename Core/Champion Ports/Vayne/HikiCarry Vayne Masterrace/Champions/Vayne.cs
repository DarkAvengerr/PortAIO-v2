using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using HybridCommon;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Vayne_Masterrace.Champions
{
    public class Vayne : BaseChamp
    {
        public Vayne()
            : base ("Vayne")
        {
            
        }

        public override void CreateConfigMenu()
        {
            combo = new Menu("Combo Settings", "Combo Settings");
            combo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("combo.r.count", "R on x Enemy").SetValue(new Slider(3, 1, 5)));

            harass = new Menu("Harass Settings", "Harass Settings");
            harass.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("harass.e", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            jungle = new Menu("Jungle Settings", "Jungle Settings");
            jungle.AddItem(new MenuItem("jungle.q", "Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("jungle.e", "Use E").SetValue(true));
            jungle.AddItem(new MenuItem("jungle.mana", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            CondemnMenu = new Menu("» Condemn Settings «", "Condemn Settings");
            condemnwhitelist = new Menu("» Condemn Whitelist", "Condemn Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                condemnwhitelist.AddItem(new MenuItem("condemnset." + enemy.CharData.BaseSkinName, string.Format("Condemn: {0}", enemy.CharData.BaseSkinName)).SetValue(true));

            }
            CondemnMenu.AddItem(new MenuItem("condemn.distance", "» Condemn Push Distance").SetValue(new Slider(410, 350, 420)));

            misc = new Menu("Miscellaneous", "Miscellaneous");
            customizableinterrupter = new Menu("Customizable Interrupter", "Customizable Interrupter");
            customizableinterrupter.AddItem(new MenuItem("miss.fortune.r", "Miss Fortune (R)").SetValue(true));
            customizableinterrupter.AddItem(new MenuItem("katarina.r", "Katarina (R)").SetValue(true));
            
            misc.AddItem(new MenuItem("auto.orb.buy", "Auto Scrying Orb Buy!").SetValue(true));
            misc.AddItem(new MenuItem("anti.gap", "Anti-Gapcloser (E)!").SetValue(true));
            misc.AddItem(new MenuItem("orb.level", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
            
            activator = new Menu("Activator Settings", "Activator Settings");
            #region QSS Usage
            qss = new Menu("QSS Settings", "QSS Settings");
            qss.AddItem(new MenuItem("use.qss", "Use QSS").SetValue(true));
            #region QSS Debuff List
            qssMenu = new Menu("QSS Debuff List", "QSS Debuff List");
            qssMenu.AddItem(new MenuItem("qss.charm", "Charm").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.flee", "Flee").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.snare", "Snare").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.polymorph", "Polymorph").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.stun", "Stun").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.suppression", "Suppression").SetValue(true));
            qssMenu.AddItem(new MenuItem("qss.taunt", "Taunt").SetValue(true));
            #endregion QSS Debuff List
            #endregion QSS Usage
            #region BOTRK Usage
            botrk = new Menu("BOTRK Settings", "BOTRK Settings");
            botrk.AddItem(new MenuItem("use.botrk", "Use BOTRK").SetValue(true));
            botrk.AddItem(new MenuItem("botrk.vayne.hp", "Use if Vayne HP < %").SetValue(new Slider(20, 0, 100)));
            botrk.AddItem(new MenuItem("botrk.enemy.hp", "Use if Enemy HP < %").SetValue(new Slider(20, 0, 100)));
            #endregion BOTRK Usage
            #region Youmuu Usage
            youmuu = new Menu("Youmuu Settings", "Youmuu Settings");
            youmuu.AddItem(new MenuItem("use.youmuu", "Use Youmuu").SetValue(true));
            #endregion Youmuu Usage
            
            m_evader = new Evader(out evade, EvadeMethods.VayneQ);
            
            Config.AddSubMenu(combo);
            Config.AddSubMenu(harass);
            Config.AddSubMenu(jungle);
            Config.AddSubMenu(evade);
            Config.AddSubMenu(CondemnMenu);
            CondemnMenu.AddSubMenu(condemnwhitelist);
            misc.AddSubMenu(customizableinterrupter);

            Config.AddSubMenu(misc);
            activator.AddSubMenu(qss);
            activator.AddSubMenu(botrk);
            activator.AddSubMenu(youmuu);
            qss.AddSubMenu(qssMenu);
            Config.AddSubMenu(activator);
            
            Config.AddItem(new MenuItem("masterracec0mb0", "                      HikiCarry Masterrace Mode"));
            Config.AddItem(new MenuItem("condemn.style", "Condemn Method").SetValue(new StringList(new[] { "Shine", "Asuna", "360" })));
            Config.AddItem(new MenuItem("condemn.x1", "Condemn Style").SetValue(new StringList(new[] { "Only Combo"})));
            Config.AddItem(new MenuItem("q.type", "Q Type").SetValue(new StringList(new[] {"Cursor Position", "Safe Position"},1)));
            Config.AddItem(new MenuItem("combo.type", "Combo Type").SetValue(new StringList(new[] { "Burst", "Normal" },1)));
            Config.AddItem(new MenuItem("harass.type", "Harass Type").SetValue(new StringList(new[] { "2 Silver Stack + Q", "2 Silver Stack + E" })));
            Config.AddItem(new MenuItem("q.stealth", "(Q) Stealth (ms) )").SetValue(new Slider(1000, 0, 1000)));
            Config.AddToMainMenu();
            
            var drawing = new Menu("Draw Settings", "Draw Settings");
            {
                drawing.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Chartreuse)));
                drawing.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, System.Drawing.Color.Yellow)));
                drawing.AddItem(new MenuItem("aa.indicator", "AA Indicator").SetValue(true));
                Config.AddSubMenu(drawing);
            }

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Drawing.OnDraw += OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;

        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && args.Target.IsEnemy && args.Unit.HasBuff("vaynetumblefade"))
            {
                var stealthtime = Config.Item("q.stealth").GetValue<Slider>().Value;
                var stealthbuff = args.Unit.GetBuff("vaynetumblefade");
                if (stealthbuff.EndTime - Game.Time > stealthbuff.EndTime - stealthbuff.StartTime - stealthtime / 1000)
                {
                    args.Process = false;
                }
            }
            else
            {
                args.Process = true;
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) &&
                sender.Type == GameObjectType.AIHeroClient)
            {
                if (Config.Item("combo.q").GetValue<bool>() && Spells[Q].IsReady() && !args.Target.IsDead && 
                    ((AIHeroClient)args.Target).IsValidTarget(ObjectManager.Player.AttackRange) &&
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    QCast(((AIHeroClient)args.Target));
                }
            }
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var tar = (AIHeroClient)target;
            if (Config.Item("use.botrk").GetValue<bool>() && ((tar.Health / tar.MaxHealth) < Config.Item("botrk.enemy.hp").GetValue<Slider>().Value) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < Config.Item("botrk.vayne.hp").GetValue<Slider>().Value))
            {
                if (Items.CanUseItem(3153))
                {
                    Items.UseItem(3153, tar);
                }
            }
            if (Config.Item("use.youmuu").GetValue<bool>())
            {
                if (Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }
        private void OnDraw(EventArgs args)
        {
            if (Config.Item("qDraw").GetValue<Circle>().Active && Spells[Q].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[Q].Range, Config.Item("qDraw").GetValue<Circle>().Color);
            }
            if (Config.Item("eDraw").GetValue<Circle>().Active && Spells[E].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[E].Range, Config.Item("eDraw").GetValue<Circle>().Color);
            }
            if (Config.Item("aa.indicator").GetValue<bool>())
            {
                HowManyAa();
            }
            Render.Circle.DrawCircle(BlueTurretRock1, 180, Color.White);
        }
        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Jungle();
            }
            if (Config.Item("auto.orb.buy").GetValue<bool>())
            {
                BlueOrb();
            }
            if (Config.Item("use.qss").GetValue<bool>())
            {
                QssUsage();
            }
            if (Config.Item("combo.e").GetValue<bool>() && Spells[E].IsReady())
            {
                SelectedCondemn();
            }
        }
        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 300f);
            Spells[W] = new Spell(SpellSlot.W);
            Spells[E] = new Spell(SpellSlot.E, 550f);
            Spells[E].SetTargetted(0.25f, 1600f);
            Spells[R] = new Spell(SpellSlot.R);

            m_evader.SetEvadeSpell(Spells[Q]);
        }
        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 300 && Config.Item("anti.gap").GetValue<bool>())
            {
                Spells[E].Cast(gapcloser.End.Extend(ObjectManager.Player.ServerPosition, ObjectManager.Player.Distance(gapcloser.End) + Spells[E].Range));
            }
        }
        public void SafePositionQ(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = LeagueSharp.Common.Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(), Spells[Q].Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(Spells[Q].Range - 100) > 0)
                {
                    return;
                }
                Spells[Q].Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Spells[Q].Range);
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                Spells[Q].Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Spells[Q].Range));
            }
        }
        public bool AsunasAllyFountain(Vector3 position)
        {
            float fountainRange = 750;
            var map = LeagueSharp.Common.Utility.Map.GetMap();
            if (map != null && map.Type == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
                ObjectManager.Get<GameObject>().Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.IsAlly).Any(spawnPoint => Vector2.Distance(position.To2D(), spawnPoint.Position.To2D()) < fountainRange);
        }
        public void SelectedCondemn()
        {
            switch (Config.Item("condemn.style").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(Spells[E].Range)))
                    {
                        if (Config.Item("condemnset."+target.CharData.BaseSkinName).GetValue<bool>())
                        {
                            var pushDistance = Config.Item("condemn.distance").GetValue<Slider>().Value;
                            var targetPosition = Spells[E].GetPrediction(target).UnitPosition;
                            var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                            float checkDistance = pushDistance / 40f;
                            for (int i = 0; i < 40; i++)
                            {
                                Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                                var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                                {
                                    Spells[E].Cast(target);
                                }
                            }
                        }
                        
                    }
                    break;
                case 1:
                    foreach (var En in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Spells[E].Range) && !hero.HasBuffOfType(BuffType.SpellShield) && !hero.HasBuffOfType(BuffType.SpellImmunity)))
                    {
                        if (Config.Item("condemnset." + En.CharData.BaseSkinName).GetValue<bool>())
                        {
                            var EPred = Spells[E].GetPrediction(En);
                            int pushDist = Config.Item("condemn.distance").GetValue<Slider>().Value;
                            var FinalPosition = EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -pushDist).To3D();

                            for (int i = 1; i < pushDist; i += (int)En.BoundingRadius)
                            {
                                Vector3 loc3 = EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -i).To3D();

                                if (loc3.IsWall() || AsunasAllyFountain(FinalPosition))
                                    Spells[E].Cast(En);
                            }
                        }
                    }
                    break;
                case 2:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) &&
                        IsCondemable(x)))
                    {
                        if (Config.Item("condemnset." + enemy.CharData.BaseSkinName).GetValue<bool>())
                        {
                            Spells[E].Cast(enemy);
                        }
                    }
                    break;
            }
        }
        public void ComboUltimateLogic()
        {
            if (ObjectManager.Player.CountEnemiesInRange(1000) >= Config.Item("combo.r.count").GetValue<Slider>().Value)
            {
                Spells[R].Cast();
            }
        }
        public void SilverStackE()
        {
            if (Config.Item("harass.type").GetValue<StringList>().SelectedIndex == 1)
            {
                foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                {
                    if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                    {
                        Spells[E].Cast(qTarget);
                    }
                }
            }
            
        }
        public void SilverStackQ()
        {
            if (Config.Item("harass.type").GetValue<StringList>().SelectedIndex == 0)
            {
                foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                {
                    if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                    {
                        Spells[Q].Cast(Game.CursorPos);
                    }
                }
            }
        }

        public void QCast(AIHeroClient enemy)
        {
            switch (Config.Item("q.type").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Spells[Q].Cast(Game.CursorPos);
                    break;
                case 1:
                    SafePositionQ(enemy);
                    break;
            }
        }

        public void QComboMethod()
        {
            switch (Config.Item("combo.type").GetValue<StringList>().SelectedIndex)
            {
                case 0: 
                    foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                    {
                        if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                        {
                            QCast(qTarget);
                        }
                    }
                    break;
                case 1: 
                    foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                    {
                        QCast(qTarget);
                    }
                    break;
            }
        }
        public void BlueOrb()
        {
            if (ObjectManager.Player.Level >= Config.Item("orb.level").GetValue<Slider>().Value && ObjectManager.Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
        public void QssUsage()
        {
            if (ObjectManager.Player.HasBuffOfType(BuffType.Charm) && Config.Item("qss.charm").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Flee) && Config.Item("qss.flee").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Snare) && Config.Item("qss.snare").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Polymorph) && Config.Item("qss.polymorph").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Stun) && Config.Item("qss.stun").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Suppression) && Config.Item("qss.suppression").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Taunt) && Config.Item("qss.taunt").GetValue<bool>())
            {
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                {
                    Items.UseItem(3140);
                }
                if (Items.CanUseItem(3139) && Items.HasItem(3137))
                {
                    Items.UseItem(3139);
                }
            }
        }
        public double WDamage(AIHeroClient hero)
        {
            double dmg = 0.0;
            dmg += CalculateDamageW(hero);
            return dmg;
        }
        public double EDamage(AIHeroClient hero)
        {
            double dmg = 0.0;
            dmg += CalculateDamageE(hero);
            return dmg;
        }
        public static IEnumerable<Vector3> CondemnPosition()
        {
            var pointList = new List<Vector3>();
            var j = 300;
            var offset = (int)(2 * Math.PI * j / 100);
            for (var i = 0; i <= offset; i++)
            {
                var angle = i * Math.PI * 2 / offset;
                var point = new Vector3((float)(ObjectManager.Player.Position.X + j * Math.Cos(angle)),
                    (float)(ObjectManager.Player.Position.Y - j * Math.Sin(angle)),
                    ObjectManager.Player.Position.Z);

                if (!NavMesh.GetCollisionFlags(point).HasFlag(CollisionFlags.Wall))
                    pointList.Add(point);
            }
            return pointList;
        }
        public void Combo()
        {
            if (Config.Item("combo.q").GetValue<bool>() && Spells[Q].IsReady())
            {
               QComboMethod();
            }
            if (Config.Item("combo.e").GetValue<bool>() && Spells[E].IsReady())
            {
                SelectedCondemn();
            }
            if (Config.Item("combo.r").GetValue<bool>() && Spells[R].IsReady())
            {
                ComboUltimateLogic();
            }
        }
        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
                return;
            if (Config.Item("harass.q").GetValue<bool>() && Spells[Q].IsReady())
            {
                SilverStackQ();
            }
            if (Config.Item("harass.e").GetValue<bool>() && Spells[E].IsReady())
            {
                SilverStackE();
            }
            
        }
        public void CondemnJungleMobs()
        {
            foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Spells[E].Range) && o.Team == GameObjectTeam.Neutral && o.IsHPBarRendered && !o.IsDead))  
            {
                if (jungleMobs.CharData.BaseSkinName == "SRU_Razorbeak" || jungleMobs.CharData.BaseSkinName == "SRU_Red" ||
                    jungleMobs.CharData.BaseSkinName == "SRU_Blue" || jungleMobs.CharData.BaseSkinName == "SRU_Dragon" ||
                    jungleMobs.CharData.BaseSkinName == "SRU_Krug" || jungleMobs.CharData.BaseSkinName == "SRU_Gromp" ||
                    jungleMobs.CharData.BaseSkinName == "Sru_Crab")
                {
                    var pushDistance = Config.Item("condemn.distance").GetValue<Slider>().Value;
                    var targetPosition = Spells[E].GetPrediction(jungleMobs).UnitPosition;
                    var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                    float checkDistance = pushDistance / 40f;
                    for (int i = 0; i < 40; i++)
                    {
                        Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                        var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                        if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                        {
                            Spells[E].Cast(jungleMobs);
                        }
                    }
                }
            }
        }
        public void JungleMobsQ()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || (mob.Count == 0))
                return;
            Spells[Q].Cast(Game.CursorPos);
        }
        public void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("jungle.mana").GetValue<Slider>().Value)
                return;
            if (Config.Item("jungle.q").GetValue<bool>() && Spells[Q].IsReady())
            {
                JungleMobsQ();
            }
            if (Config.Item("jungle.e").GetValue<bool>() && Spells[E].IsReady())
            {
                CondemnJungleMobs();
            }
        }
        public void CustomizableInterrupter()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
            {
                if (enemy.HasBuff("KatarinaR") || enemy.HasBuff("katarinarsound") && Config.Item("katarina.r").GetValue<bool>())
                {
                    Spells[E].Cast(enemy);
                }
                if (enemy.HasBuff("missfortunebulletsound") || enemy.HasBuff("MissFortuneBulletTime") && Config.Item("miss.fortune.r").GetValue<bool>())
                {
                    Spells[E].Cast(enemy);
                }
            }
        }
        public bool IsCondemable(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || LastCheck + 50 > Environment.TickCount || ObjectManager.Player.IsDashing()) return false;
            var prediction = Spells[E].GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3>() { pos.To3D() } : new List<Vector3>
                        {
                            unit.ServerPosition,
                            unit.Position,
                            prediction.CastPosition,
                            prediction.UnitPosition
                        };

            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < Config.Item("condemn.distance").GetValue<Slider>().Value; i += (int)unit.BoundingRadius) // 420 = push distance
                {
                    var cPos = ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i).To2D();
                    Points.Add(cPos);
                    if (NavMesh.GetCollisionFlags(cPos.To3D()).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos.To3D()).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if ((wallsFound / predictionsList.Count) >= 33 / 100f)
            {
                return true;
            }

            return false;
        }
        public void HowManyAa()
        {
            foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(1000) && o.IsHPBarRendered && !o.IsDead && !o.IsZombie))
            {
                var basicDamage = ObjectManager.Player.GetAutoAttackDamage(enemy);
                var hikiX1 = new float[] { 0, 20, 30, 40, 50, 60 };
                var hikiX2 = new double[] { 0, .04, .05, .06, .07, .08 };

                var hikiAa = (enemy.Health)/
                              (basicDamage + (hikiX1[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level] + 
                              (enemy.MaxHealth*hikiX2[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level]))/3);
                int totalAa = (int)Math.Ceiling(hikiAa);
                Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Color.Gold,
                                    string.Format("Auto Attacks: {0} ", totalAa));
            }
        }
    }
}
