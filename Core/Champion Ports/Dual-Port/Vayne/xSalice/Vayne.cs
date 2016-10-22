namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Geometry = Utilities.Geometry;
    using Orbwalking = LeagueSharp.Common.Orbwalking;
    using EloBuddy;
    using Champion = Base.Champion;
    using Utility = LeagueSharp.Common.Utility;
    internal class Vayne : Champion
    {
        public Vayne()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 300);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 550);
            SpellManager.R = new Spell(SpellSlot.R);

            var spellMenu = new Menu("Spell Config", "SpellMenu");
            {
                var qMenu = new Menu("Q Config", "QMenu");
                {
                    qMenu.AddItem(
                        new MenuItem("QMode", "Q Mode: ", true).SetValue(
                            new StringList(new[] { "PRADA", "TO MOUSE" })));
                    qMenu.AddItem(
                        new MenuItem("QMinDist", "Min dist from enemies", true).SetValue(new Slider(375, 325, 525)));
                    qMenu.AddItem(new MenuItem("QChecks", "Q Safety Checks", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("EQ", "Q After E", true).SetValue(false));
                    qMenu.AddItem(new MenuItem("QWall", "Enable Wall Tumble?", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("QR", "Q after Ult", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Q_Min_Stack", "Q Harass Min W Stacks", true).SetValue(new Slider(2, 0, 2)));
                    spellMenu.AddSubMenu(qMenu);
                }

                var eMenu = new Menu("E Config", "EMenu");
                {
                    eMenu.AddItem(
                        new MenuItem("EMode", "E Mode", true).SetValue(
                            new StringList(new[] { "PRADASMART", "PRADAPERFECT", "MARKSMAN", "SHARPSHOOTER", "GOSU", "VHR", "PRADALEGACY", "FASTEST", "OLDPRADA" })));
                    eMenu.AddItem(
                        new MenuItem("EPushDist", "E Push Distance", true).SetValue(new Slider(450, 300, 475)));
                    eMenu.AddItem(new MenuItem("EHitchance", "E % Hitchance", true).SetValue(new Slider(50)));
                    eMenu.AddItem(
                        new MenuItem("ManualE", "Semi-Manual Condemn").SetValue(new KeyBind('E', KeyBindType.Press)));
                    var antigcmenu = eMenu.AddSubMenu(new Menu("Anti-Gapcloser", "antigapcloser"));
                    foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
                    {
                        var championName = hero.CharData.BaseSkinName;
                        antigcmenu.AddItem(new MenuItem("antigc" + championName, championName, true).SetValue(false));
                    }
                    spellMenu.AddSubMenu(eMenu);
                }

                Menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(false));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle, true)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 30);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("UseInt", "Use E to Interrupt", true).SetValue(true));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q Spot", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw Condemn", true).SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                Menu.AddSubMenu(drawMenu);
            }

            var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            var comboDamage = 0d;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + W.GetDamage(target) + Player.GetAutoAttackDamage(target) * 3);
        }

        private void JungleClear()
        {
            var mob =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        m =>
                            m.Team == GameObjectTeam.Neutral && m.Distance(ObjectManager.Player) < 550 &&
                            m.Name.Contains("SRU_") && !m.Name.Contains("Mini") && !m.Name.Contains("Dragon") && !m.Name.Contains("Baron"));

            if (mob != null)
            {
                if (Q.IsReady())
                {
                    Q.Cast(GetJungleSafeTumblePos(mob));
                }
                AttemptSimpleCondemn(mob);
            }
        }

        private Vector3 GetJungleSafeTumblePos(Obj_AI_Minion target)
        {
            var cursorPos = Game.CursorPos;

            if (IsSafeTumblePos(cursorPos))
                return cursorPos;

            if (!target.IsValidTarget())
                return Vector3.Zero;

            var targetPosition = target.ServerPosition;

            var myTumbleRangeCircle =
                new Geometry.Circle(ObjectManager.Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();

            var goodCandidates = from p in myTumbleRangeCircle
                                 select new Vector2(p.X, p.Y).To3D() into v3
                                 let dist = v3.Distance(targetPosition)
                                 where dist > Menu.Item("QMinDist", true).GetValue<Slider>().Value && dist < 500
                                 select v3;

            return goodCandidates.OrderByDescending(candidate => candidate.Distance(cursorPos)).FirstOrDefault();
        }

        private Vector3 GetSafeTumblePos(AIHeroClient target)
        {
            if (!target.IsValidTarget())
                return Vector3.Zero;

            var targetPosition = target.ServerPosition;

            var myTumbleRangeCircle =
                new Geometry.Circle(ObjectManager.Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();

            var goodCandidates = from p in myTumbleRangeCircle
                select new Vector2(p.X, p.Y).To3D() into v3
                let dist = v3.Distance(targetPosition)
                where dist > Menu.Item("QMinDist", true).GetValue<Slider>().Value && dist < 500
                select v3;

            return goodCandidates.OrderBy(candidate => candidate.Distance(Game.CursorPos)).FirstOrDefault();
        }

        private void AttemptSimpleCondemn(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget(550) && target.ServerPosition.Extend(ObjectManager.Player.ServerPosition, -420).IsWall() ||
                target.ServerPosition.Extend(ObjectManager.Player.ServerPosition, -210).IsWall())
            {
                E.Cast(target);
            }
        }

        private bool IsSafeTumblePos(Vector3 position)
        {
            return
                !ObjectManager.Get<AIHeroClient>()
                .Any(e => e.IsEnemy && e.Distance(position) < Menu.Item("QMinDist", true).GetValue<Slider>().Value);
        }

        private AIHeroClient GetEnemyWith2W()
        {
            return ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => GetWBuffCount(h) == 2);
        }

        private int GetWBuffCount(AIHeroClient target)
        {
            var wBuff = target.Buffs.FirstOrDefault(b => b.Name == "vaynesilvereddebuff");
            return wBuff?.Count ?? 0;
        }

        private bool IsCollisionable(Vector3 pos)
        {
            return NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) ||
                NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building);
        }

        public bool UltActive()
        {
            return ObjectManager.Player.Buffs.Any(b => b.Name.ToLower().Contains("vayneinquisition"));
        }

        public bool TumbleActive()
        {
            return ObjectManager.Player.Buffs.Any(b => b.Name.ToLower().Contains("vaynetumblebonus"));
        }

        private bool IsCondemnable(AIHeroClient hero)
        {
            if (!hero.IsValidTarget(550f) || hero.HasBuffOfType(BuffType.SpellShield) ||
                hero.HasBuffOfType(BuffType.SpellImmunity) || hero.IsDashing())
                return false;

            var pP = ObjectManager.Player.ServerPosition;
            var p = hero.ServerPosition;
            var pD = Menu.Item("EPushDist", true).GetValue<Slider>().Value;
            var mode = Menu.Item("EMode", true).GetValue<StringList>().SelectedValue;

            if (mode == "PRADASMART" && (IsCollisionable(p.Extend(pP, -pD)) || IsCollisionable(p.Extend(pP, -pD / 2f)) ||
                 IsCollisionable(p.Extend(pP, -pD / 3f))))
            {
                if (!hero.CanMove ||
                    hero.Spellbook.IsAutoAttacking)
                    return true;

                var enemiesCount = ObjectManager.Player.CountEnemiesInRange(1200);
                if (enemiesCount > 1 && enemiesCount <= 3)
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        var posFlags = NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(
                                    pP.To2D(),
                                    -i)
                                .To3D());
                        if (posFlags.HasFlag(CollisionFlags.Wall) || posFlags.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }

                var hitchance = Menu.Item("EHitchance", true).GetValue<Slider>().Value;
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (IsCollisionable(pP.To2D().Extend(alpha,
                                i)
                            .To3D()) && IsCollisionable(pP.To2D().Extend(beta, i).To3D())) return true;
                }
                return false;
            }

            if (mode == "PRADAPERFECT" &&
                (IsCollisionable(p.Extend(pP, -pD)) || IsCollisionable(p.Extend(pP, -pD / 2f)) ||
                 IsCollisionable(p.Extend(pP, -pD / 3f))))
            {
                if (!hero.CanMove ||
                    (hero.Spellbook.IsAutoAttacking))
                    return true;

                var hitchance = Menu.Item("EHitchance", true).GetValue<Slider>().Value;
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (IsCollisionable(pP.To2D().Extend(alpha,
                            i)
                        .To3D()) && IsCollisionable(pP.To2D().Extend(beta, i).To3D())) return true;
                }
                return false;
            }

            switch (mode)
            {
                case "OLDPRADA":
                    if (!hero.CanMove ||
                        (hero.Spellbook.IsAutoAttacking))
                        return true;

                    var hitchance = Menu.Item("EHitchance", true).GetValue<Slider>().Value;
                    var angle = 0.20 * hitchance;
                    const float travelDistance = 0.5f;
                    var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                        (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                    var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                        (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                    for (var i = 15; i < pD; i += 100)
                    {
                        if (IsCollisionable(pP.To2D().Extend(alpha,
                                    i)
                                .To3D()) || IsCollisionable(pP.To2D().Extend(beta, i).To3D())) return true;
                    }
                    return false;
                case "MARKSMAN":
                {
                    var prediction = E.GetPrediction(hero);
                    return NavMesh.GetCollisionFlags(
                               prediction.UnitPosition.To2D()
                                   .Extend(
                                       pP.To2D(),
                                       -pD)
                                   .To3D()).HasFlag(CollisionFlags.Wall) ||
                           NavMesh.GetCollisionFlags(
                               prediction.UnitPosition.To2D()
                                   .Extend(
                                       pP.To2D(),
                                       -pD / 2f)
                                   .To3D()).HasFlag(CollisionFlags.Wall);
                }
                case "SHARPSHOOTER":
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 100)
                    {
                        var posCF = NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(
                                    pP.To2D(),
                                    -i)
                                .To3D());
                        if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                case "GOSU":
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        var posCF = NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(
                                    pP.To2D(),
                                    -i)
                                .To3D());
                        if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                case "VHR":
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += (int)hero.BoundingRadius) //:frosty:
                    {
                        var posCF = NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(
                                    pP.To2D(),
                                    -i)
                                .To3D());
                        if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                case "PRADALEGACY":
                {
                    var prediction = E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        var posCF = NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(
                                    pP.To2D(),
                                    -i)
                                .To3D());
                        if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            return mode == "FASTEST" && IsCollisionable(p.Extend(pP, -pD)) || IsCollisionable(p.Extend(pP, -pD / 2f)) ||
                   IsCollisionable(p.Extend(pP, -pD / 3f));
        }

        private AIHeroClient GetCondemnableTarget()
        {
            return ObjectManager.Get<AIHeroClient>().OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(hero => IsCondemnable(hero));
        }

        protected override void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    var hero = target as AIHeroClient;

                    if (hero != null && Menu.Item("UseQCombo", true).GetValue<bool>())
                    {
                        if (Q.IsReady())
                        {
                            Q.Cast(GetSafeTumblePos(hero));
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Menu.Item("UseQHarass", true).GetValue<bool>())
                    {
                        var tg = target as AIHeroClient;
                        if (Q.IsReady() && target is AIHeroClient)
                        {
                            var qMin = Menu.Item("Q_Min_Stack", true).GetValue<Slider>().Value;
                            if (qMin <= GetWBuffCount(tg) && IsSafeTumblePos(ObjectManager.Player.Position.Extend(tg.ServerPosition, 300)))
                                Q.Cast(tg.ServerPosition);
                        }
                        if (E.IsReady() && Menu.Item("UseEHarass", true).GetValue<bool>() && target is AIHeroClient && GetWBuffCount(tg) == 2)
                        {
                            E.Cast(tg);
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (ObjectManager.Player.Buffs.Any(b=>b.Name.ToLower().Contains("tumblefade")) && 
                ObjectManager.Get<AIHeroClient>().
                Any(h => h.IsEnemy && h.IsValidTarget() && 
                h.IsMelee && h.Distance(ObjectManager.Player) < 325))
            {
                args.Process = false;
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var cursorPos = Game.CursorPos;

            if (useQ && IsSafeTumblePos(cursorPos) &&
                MinionManager.GetMinions(580).
                Any(m => m.Health < ObjectManager.Player.GetAutoAttackDamage(m) && m.Health > 13))
            {
                Q.Cast(cursorPos);
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (E.IsReady())
            {
                foreach (var en in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy && h.IsValidTarget(550f)))
                {
                    AttemptSimpleCondemn(en);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Farm();
                JungleClear();
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>())
                return;

            if (Player.Distance(unit.Position) < E.Range && spell.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (E.IsReady())
                    E.Cast(unit);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("antigc" + gapcloser.Sender.ChampionName, true).GetValue<bool>())
            {
                if (ObjectManager.Player.Distance(gapcloser.End) < 425)
                {
                    E.Cast(gapcloser.Sender);
                }
            }
        }
    }
}
