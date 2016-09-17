using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Tristana
{

    #region

    using LeagueSharp;
    using LeagueSharp.Common;
    using Item = LeagueSharp.Common.Data.ItemData;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Color = System.Drawing.Color;

    #endregion


    public static class Program
    {
        public static AIHeroClient Player;
        public static Spell Q, W, E, R;
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += LoadEvents;
        }

        private static void LoadEvents(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Tristana")
                return;

            Player = ObjectManager.Player;
            LoadSpells();
            CheckVersion.Check();

            Menu = new Menu("Flowers - Tristana", "FLTA", true);
            Menu.SetFontStyle(FontStyle.Regular, SharpDX.Color.Red);

            Menu.AddItem(new MenuItem("nightmoon.menu.lanuguage", "Language Switch (Need F5): ").SetValue(new StringList(new[] { "English", "Chinese" }, 0)));

            Menu.AddItem(new MenuItem("nightmoon.credit", "Credit : NightMoon"));

            Menu.AddToMainMenu();

            if (Menu.Item("nightmoon.menu.lanuguage").GetValue<StringList>().SelectedIndex == 0)
            {
                LoadEnglish();
            }
            else if (Menu.Item("nightmoon.menu.lanuguage").GetValue<StringList>().SelectedIndex == 1)
            {
                LoadMenu();
            }

            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //Credit ScienceARK
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null || Khazix != null)
                if (Menu.Item("nightmoon.r.rk").GetValue<bool>())
                {
                    if (sender.Name == ("Rengar_LeapSound.troy") && sender.Position.Distance(Player.Position) < R.Range)
                        R.Cast(Rengar);

                    if (sender.Name == ("Khazix_Base_E_Tar.troy") && sender.Position.Distance(Player.Position) <= 300)
                        R.Cast(Khazix);
                }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if(Player.IsDead)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            RKS();
            REKS();
            ELogic();

            if (Menu.Item("nightmoon.w.key").GetValue<KeyBind>().Active)
            {
                W.Cast(Game.CursorPos);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void ELogic()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target == null)
                return;

            if (!target.IsValidTarget())
                return;

            if (Menu.Item("nightmoon.e.key").GetValue<KeyBind>().Active && CanCastE())
            {
                if(target.ECanKill())
                    E.CastOnUnit(target);

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Player.CountEnemiesInRange(1200) == 1)
                    {
                        if (Player.HealthPercent >= target.HealthPercent && Player.Level + 1 >= target.Level)
                        {
                            E.CastOnUnit(target);

                            if (Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ() && !E.IsReady())
                                Q.Cast();
                        }
                        else if (Player.HealthPercent + 20 >= target.HealthPercent && Player.HealthPercent >= 40 && Player.Level + 2 >= target.Level)
                        {
                            E.CastOnUnit(target);

                            if (Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ() && !E.IsReady())
                                Q.Cast();
                        }
                    }

                    if (E.IsInRange(target) && Menu.Item("nightmoon." + target.ChampionName + "euse").GetValue<bool>())
                    {
                        E.CastOnUnit(target);

                        if (Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ() && !E.IsReady())
                            Q.Cast();
                    }
                }
            }
        }

        private static void ItemUse()
        {
            var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if(Target == null)
                return;

            var Borke = Item.Blade_of_the_Ruined_King.GetItem();
            var Youmeng = Item.Youmuus_Ghostblade.GetItem();
            var Blige = Item.Bilgewater_Cutlass.GetItem();

            if(Menu.Item("nightmoon.item.youmeng").GetValue<bool>())
            {
                if (Youmeng.IsReady() && Youmeng.IsOwned(Player))
                {
                    if(Target.IsValidTarget(E.Range))
                    {
                        Youmeng.Cast();

                        if(Menu.Item("nightmoon.q.youmeng").GetValue<bool>() && CanCastQ() && Youmeng.Cast())
                            Q.Cast();
                    }

                    if(Player.CountEnemiesInRange(1200) < 2 && Menu.Item("nightmoon.item.youmeng.dush").GetValue<bool>())
                    {
                        Youmeng.Cast();

                        if (Menu.Item("nightmoon.q.youmeng").GetValue<bool>() && CanCastQ() && Youmeng.Cast())
                            Q.Cast();
                    }
                }
            }


            if(Menu.Item("nightmoon.item.blige").GetValue<bool>())
            {
                if (Blige.IsInRange(Target))
                    if (Target.HealthPercent <= Menu.Item("nightmoon.item.blige.enemyhp").GetValue<Slider>().Value)
                        Blige.Cast(Target);
            }

            if (Menu.Item("nightmoon.item.borke").GetValue<bool>())
            {
                if(Borke.IsReady() && Borke.IsOwned(Player))
                    if (Borke.IsInRange(Target))
                    {
                        if (Target.HealthPercent <= Menu.Item("nightmoon.item.borke.enemyhp").GetValue<Slider>().Value)
                            Borke.Cast(Target);

                        if (Player.HealthPercent <= Menu.Item("nightmoon.item.borke.mehp").GetValue<Slider>().Value)
                            Borke.Cast(Target);
                    }
            }
        }

        private static void REKS()
        {
            foreach (var enemy in from enemy in HeroManager.Enemies.Where(e => R.CanCast(e))
                                  let etargetstacks = enemy.Buffs.Find(buff => buff.Name == "TristanaECharge")
                                  where R.GetDamage(enemy) + E.GetDamage(enemy) + etargetstacks?.Count * 0.30 * E.GetDamage(enemy) >= enemy.Health
                                  select enemy)
            {
                if(CanCastR())
                {
                    R.CastOnUnit(enemy);
                    return;
                }
            }
        }

        private static void RKS()
        {
            if(Menu.Item("nightmoon.r.ks").GetValue<bool>())
            {
                var Target = HeroManager.Enemies.OrderByDescending(x => x.Health).FirstOrDefault(x => x.isKillableAndValidTarget(R.GetDamage(x), TargetSelector.DamageType.Physical, R.Range) && !x.ECanKill());

                if (Target != null)
                    if (CanCastR())
                        R.CastOnUnit(Target);
            }
        }

        private static void Harass()
        {
            if (Menu.Item("nightmoon.e.quickharass").GetValue<bool>())
            {
                foreach (var minion in MinionManager.GetMinions(E.Range).Where(m => E.CanCast(m) && m.Health < Player.GetAutoAttackDamage(m) && m.CountEnemiesInRange(m.BoundingRadius + 150) >= 1))
                {
                    var etarget = E.GetTarget();

                    if (etarget != null)
                        return;

                    if (CanCastE())
                    {
                        E.CastOnUnit(minion);
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target == null)
                return;

            if (!target.IsValidTarget())
                return;

            ItemUse();

            if (!Menu.Item("nightmoon.q.onlye").GetValue<bool>() && Menu.Item("nightmoon.q.combo").GetValue<bool>())
            {
                if(target.IsValidTarget(E.Range))
                    if (CanCastQ())
                        Q.Cast();
            }

            if (E.IsInRange(target) && Menu.Item("nightmoon." + target.ChampionName + "euse").GetValue<bool>() && CanCastE())
            {
                E.CastOnUnit(target);

                if (Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ() && !E.IsReady())
                    Q.Cast();
            }

            if (Menu.Item("nightmoon.r.self").GetValue<Slider>().Value != 0 && Player.HealthPercent <= Menu.Item("nightmoon.r.self").GetValue<Slider>().Value)
            {
                var dangerenemy =HeroManager.Enemies.Where(e => R.CanCast(e)).OrderBy(enemy => enemy.Distance(Player)).FirstOrDefault();
                if (dangerenemy != null)
                    if (CanCastR())
                        R.CastOnUnit(dangerenemy);
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe)
                return;

            Q.Range = 600 + 5 * (Player.Level - 1);
            E.Range = 630 + 7 * (Player.Level - 1);
            R.Range = 630 + 7 * (Player.Level - 1);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("nightmoon.r.gap").GetValue<bool>())
                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200 && gapcloser.Sender.IsValidTarget(R.Range))
                    if (CanCastR())
                        R.CastOnUnit(gapcloser.Sender);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("nightmoon.r.int").GetValue<bool>())
                if (args.DangerLevel >= Interrupter2.DangerLevel.High && sender.IsValidTarget(R.Range))
                    if (CanCastR())
                        R.CastOnUnit(sender);
        }

        public static bool CanCastR()
        {
            if (Player.ManaPercent > Menu.Item("nightmoon.r.mana").GetValue<Slider>().Value && R.IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CanCastE()
        {
            if(Player.ManaPercent > Menu.Item("nightmoon.e.mana").GetValue<Slider>().Value && E.IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CanCastQ()
        {
            if (Player.ManaPercent > Menu.Item("nightmoon.q.mana").GetValue<Slider>().Value && Q.IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if(Menu.Item("nightmoon.e.forcetarget").GetValue<bool>())
            {
                if(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.HasBuff("TristanaEChargeSound")))
                    {
                        TargetSelector.SetTarget(enemy);
                        return;
                    }
            }

            if (args.Unit.IsMe && Orbwalking.InAutoAttackRange(args.Target))
            {
                switch(Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ())
                            {
                                AIHeroClient Target = args.Target.Type == GameObjectType.AIHeroClient ? (AIHeroClient)args.Target : null;

                                if (Target.HasBuff("TristanaEChargeSound") || Target.HasBuff("TristanaECharge"))
                                    Q.Cast();
                            }
                            else if(!Menu.Item("nightmoon.q.onlye").GetValue<bool>() && CanCastQ())
                            {
                                Q.Cast();
                            }
                            break;
                        }
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {

                            if (Menu.Item("nightmoon.q.jc").GetValue<bool>())
                                if (MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any(x => x.NetworkId == args.Target.NetworkId) && CanCastQ())
                                    Q.Cast();
                            break;
                        }
                }
            }
        }
    
        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if(Menu.Item("nightmoon.e.tower").GetValue<bool>())
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    if (unit.IsMe && target != null)
                        if (target.Type == GameObjectType.obj_AI_Turret || target.Type == GameObjectType.obj_Turret)
                            if (CanCastE())
                            {
                                E.CastOnUnit(target as Obj_AI_Base);

                                if (!Player.Spellbook.IsAutoAttacking && Player.CountEnemiesInRange(1000) < 1 && CanCastQ() && Menu.Item("nightmoon.q.tower").GetValue<bool>())
                                    Q.Cast();
                            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(Player.IsDead)
                return;

            if (Menu.Item("nightmoon.draw.e").GetValue<bool>() && !E.IsReady())
            {
                var ETurret = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(t => !t.IsDead && t.HasBuff("TristanaECharge"));
                var ETarget = HeroManager.Enemies.FirstOrDefault(e => !e.IsDead && e.HasBuff("TristanaECharge"));

                if (ETurret != null)
                {
                    var eturretstacks = ETurret.Buffs.Find(buff => buff.Name == "TristanaECharge").Count;

                    if (ETurret.Health < (E.GetDamage(ETurret) + (((eturretstacks * 0.30)) * E.GetDamage(ETurret))))
                    {
                        Drawing.DrawCircle(ETurret.Position, 300 + ETurret.BoundingRadius, Color.Red);
                    }
                    else if (ETurret.Health > (E.GetDamage(ETurret) + (((eturretstacks * 0.30)) * E.GetDamage(ETurret))))
                    {
                        Drawing.DrawCircle(ETurret.Position, 300 + ETurret.BoundingRadius, Color.Orange);
                    }
                }

                if (ETarget != null)
                {
                    var etargetstacks = ETarget.Buffs.Find(buff => buff.Name == "TristanaECharge").Count;

                    if (ETarget.Health < (E.GetDamage(ETarget) + (((etargetstacks * 0.30)) * E.GetDamage(ETarget))))
                    {
                        Drawing.DrawCircle(ETarget.Position, 150 + ETarget.BoundingRadius, Color.Red);
                    }
                    else if (ETarget.Health > (E.GetDamage(ETarget) + (((etargetstacks * 0.30)) * E.GetDamage(ETarget))))
                    {
                        Drawing.DrawCircle(ETarget.Position, 150 + ETarget.BoundingRadius, Color.Orange);
                    }
                }
            }

            if (Menu.Item("nightmoon.draw.eks").GetValue<Circle>().Active)
            {
                foreach (var Target in HeroManager.Enemies.Where(x => x.IsValidTarget() && x.ECanKill()))
                {
                    var TargetPos = Drawing.WorldToScreen(Target.Position);
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, Menu.Item("nightmoon.draw.eks").GetValue<Circle>().Color);
                    Drawing.DrawText(TargetPos.X, TargetPos.Y - 50, Menu.Item("nightmoon.draw.eks").GetValue<Circle>().Color, "Kill For E");
                }
            }

            if (Menu.Item("nightmoon.draw.rks").GetValue<Circle>().Active && CanCastR())
            {
                foreach (var Target in HeroManager.Enemies.Where(x => x.isKillableAndValidTarget(R.GetDamage(x), TargetSelector.DamageType.Magical)))
                {
                    var TargetPos = Drawing.WorldToScreen(Target.Position);
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, Menu.Item("nightmoon.draw.rks").GetValue<Circle>().Color);
                    Drawing.DrawText(TargetPos.X, TargetPos.Y - 20, Menu.Item("nightmoon.draw.rks").GetValue<Circle>().Color, "Kill For R");
                }
            }
        }

        public static bool ECanKill(this Obj_AI_Base target)
        {
            if (target.HasBuff("TristanaECharge"))
            {
                if (target.isKillableAndValidTarget(Damage.GetSpellDamage(ObjectManager.Player, target, SpellSlot.E) * (target.GetBuffCount("TristanaECharge") * 0.30) +  Damage.GetSpellDamage(ObjectManager.Player, target, SpellSlot.E), TargetSelector.DamageType.Physical))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool isKillableAndValidTarget(this Obj_AI_Base Target, double CalculatedDamage, TargetSelector.DamageType damageType, float distance = float.MaxValue)
        {
            if (Target == null || !Target.IsValidTarget(distance) || Target.IsDead || Target.CharData.BaseSkinName == "GangPlankBarrel")
            {
                return false;
            }

            if (Target.HasBuff("KindredRNoDeathBuff") && Target.Health <= Target.MaxHealth * 0.10f)
            {
                return false;
            }

            if (Target.HasBuff("Undying Rage") && Target.Health <= Target.MaxHealth * 0.05f)
            {
                return false;
            }

            if (Target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (Target.HasBuff("BansheesVeil"))
            {
                return false;
            }

            if (Target.HasBuff("SivirShield"))
            {
                return false;
            }

            if (Target.HasBuff("ShroudofDarkness"))
            {
                return false;
            }

            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                CalculatedDamage *= 0.6;
            }

            if (Target.CharData.BaseSkinName == "Blitzcrank")
                if (!Target.HasBuff("ManaBarrierCoolDown"))
                    if (Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) + (Target.Mana * 0.6) + Target.PARRegenRate < CalculatedDamage)
                        return true;

            if (Target.CharData.BaseSkinName == "Garen")
                if (Target.HasBuff("GarenW"))
                    CalculatedDamage *= 0.7;


            if (Target.HasBuff("FerociousHowl"))
                CalculatedDamage *= 0.3;

            BuffInstance dragonSlayerBuff = ObjectManager.Player.GetBuff("s5test_dragonslayerbuff");

            if (dragonSlayerBuff != null)
                if (Target.IsMinion)
                {
                    if (dragonSlayerBuff.Count >= 4)
                        CalculatedDamage += dragonSlayerBuff.Count == 5 ? CalculatedDamage * 0.30 : CalculatedDamage * 0.15;

                    if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("dragon"))
                        CalculatedDamage *= 1 - (dragonSlayerBuff.Count * 0.07);
                }

            if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("baron") && ObjectManager.Player.HasBuff("barontarget"))
                CalculatedDamage *= 0.5;

            return Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) < CalculatedDamage - 2;
        }

        private static void LoadMenu()
        {
            Menu.AddSubMenu(new Menu("[FL] 走砍设置", "nightmoon.orbwalking.setting"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.orbwalking.setting"));

            Menu.AddSubMenu(new Menu("[FL] 技能设置", "nightmoon.spell.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.DarkBlue));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] Q 设置", "nightmoon.q.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.combo", "连招时智能使用Q").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.jc", "使用Q自动清野").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.youmeng", "使用幽梦连招后自动Q").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.onlye", "仅使用E后再用Q").SetTooltip("对方身上有E才用Q攻击").SetValue(false));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.tower", "E塔后自动接Q").SetTooltip("附近木有英雄才这样").SetValue(false));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] W 设置", "nightmoon.w.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.w.setting").AddItem(new MenuItem("nightmoon.w.key", "W跳到鼠标位置").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] E 设置", "nightmoon.e.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.tower", "自动E塔").SetTooltip("自动E塔").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.uselist", "使用E对象:").SetTooltip("自动E英雄列表"));//1
            foreach (var enemy in HeroManager.Enemies)
            {
                Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon." + enemy.ChampionName + "euse", "英雄:" + enemy.ChampionName).SetValue(true));//1
            }
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.key", "手动E按键").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.forcetarget", "集中攻击被E的目标").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.quickharass", "使用E快速骚扰").SetTooltip("当一个要死的小兵附近有英雄并且放E爆炸能吃伤害 就放E给小兵自动击杀小兵").SetValue(true));//1

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] R 设置", "nightmoon.r.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.r.self", "使用R自保-自己Hp最低百分比").SetValue(new Slider(20)));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.r.ks", "R击杀")).SetTooltip("R的伤害足够才释放").SetValue(true);//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.re.ks", "R+E击杀")).SetTooltip("R+E的伤害足够才释放").SetValue(true);//1

            Menu.AddSubMenu(new Menu("[FL] 反突打断", "nightmoon.misc.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.CadetBlue));
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.gap", "使用R反突进")).SetValue(true);//1
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.rk", "使用R反狮子狗跟螳螂")).SetValue(true);//1
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.int", "使用R打断技能")).SetValue(true);//1

            Menu.AddSubMenu(new Menu("[FL] 蓝量管理", "nightmoon.mana.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.LawnGreen));
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.q.mana", "全局使用Q最低蓝量").SetValue(new Slider(10)));//1
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.e.mana", "全局使用E最低蓝量").SetValue(new Slider(15)));//1
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.r.mana", "全局使用R最低蓝量").SetValue(new Slider(10)));//1

            Menu.AddSubMenu(new Menu("[FL] 物品使用", "nightmoon.item.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.SkyBlue));
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.youmeng", "使用幽梦")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.youmeng.dush", "自动追人击杀")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.blige", "使用弯刀")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.blige.enemyhp", "敌人当前Hp").SetValue(new Slider(80)));//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke", "使用破败")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke.enemyhp", "敌人当前Hp").SetValue(new Slider(80)));//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke.mehp", "自己当前Hp").SetValue(new Slider(60)));//1

            Menu.AddSubMenu(new Menu("[FL] 显示设置", "nightmoon.draw.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.e", "显示E爆炸半径")).SetTooltip("仅身上有E才显示 并且根据是否能击杀颜色变化 防御塔也会显示").SetValue(true);//1
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.eks", "显示E击杀目标").SetValue(new Circle(true, Color.Red)));//1
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.rks", "显示R击杀目标").SetValue(new Circle(true, Color.Red)));//1

            //Menu.AddItem(new MenuItem("nightmoon.sound.bool", "开局音效").SetValue(true));

            Menu.AddItem(new MenuItem("nightmoon.Credit", "Credit : NightMoon"));
        }

        private static void LoadEnglish()
        {
            Menu.AddSubMenu(new Menu("[FL] Orbwalker Setting", "nightmoon.orbwalking.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.orbwalking.setting"));

            Menu.AddSubMenu(new Menu("[FL] Spells Setting", "nightmoon.spell.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] Q Setting", "nightmoon.q.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.combo", "Use Q In Combo").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.jc", "Use Q In Jungle").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.youmeng", "Auto Q If Use Ghostblade").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.onlye", "Only Have E buffs Use Q").SetValue(false));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.q.setting").AddItem(new MenuItem("nightmoon.q.tower", "If E Tower Auto Q | Only Not Enemy in Ranges").SetValue(false));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] W Setting", "nightmoon.w.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.w.setting").AddItem(new MenuItem("nightmoon.w.key", "Jump To Mouse").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] E Setting", "nightmoon.e.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.tower", "Auto E Towers").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.uselist", "Use E list(Combo):"));//1
            foreach (var enemy in HeroManager.Enemies)
            {
                Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon." + enemy.ChampionName + "euse", "Heros :" + enemy.ChampionName).SetValue(true));//1
            }
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.key", "Semi-Automatic E Key").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.forcetarget", "Force Attack E Target").SetValue(true));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.e.setting").AddItem(new MenuItem("nightmoon.e.quickharass", "Use E QuickHarass").SetTooltip("if a minion will died and enemy will heart").SetValue(true));//1

            Menu.SubMenu("nightmoon.spell.setting").AddSubMenu(new Menu("[FL] R Setting", "nightmoon.r.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.r.self", "Use R 丨If Hp <=%").SetValue(new Slider(20)));//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.r.ks", "Use R Killsteal")).SetValue(true);//1
            Menu.SubMenu("nightmoon.spell.setting").SubMenu("nightmoon.r.setting").AddItem(new MenuItem("nightmoon.re.ks", "Use R+E Killsteal")).SetValue(true);//1

            Menu.AddSubMenu(new Menu("[FL] Misc Setting", "nightmoon.misc.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.gap", "Use R AntiGapcloser")).SetValue(true);//1
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.rk", "Use R Anti Rengar&Khazix")).SetValue(true);//1
            Menu.SubMenu("nightmoon.misc.setting").AddItem(new MenuItem("nightmoon.r.int", "Use R Interrupter")).SetValue(true);//1

            Menu.AddSubMenu(new Menu("[FL] Mana Manager", "nightmoon.mana.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.q.mana", "Whole Use Q Mana Control").SetValue(new Slider(10)));//1
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.e.mana", "Whole Use E Mana Control").SetValue(new Slider(15)));//1
            Menu.SubMenu("nightmoon.mana.setting").AddItem(new MenuItem("nightmoon.r.mana", "Whole Use R Mana Control").SetValue(new Slider(10)));//1

            Menu.AddSubMenu(new Menu("[FL] Items Use", "nightmoon.item.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.youmeng", "Use Ghostblade")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.youmeng.dush", "Use Ghostblade To 1v1")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.blige", "Use Cutlass")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.blige.enemyhp", "Enemy Hp <=%").SetValue(new Slider(80)));//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke", "Use Borke")).SetValue(true);//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke.enemyhp", "Enemy Hp <=%").SetValue(new Slider(80)));//1
            Menu.SubMenu("nightmoon.item.setting").AddItem(new MenuItem("nightmoon.item.borke.mehp", "My Hp <=%").SetValue(new Slider(60)));//1

            Menu.AddSubMenu(new Menu("[FL] Draw Setting", "nightmoon.draw.setting").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.e", "Draw E Bomb Range")).SetTooltip("Credit God!").SetValue(true);//1
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.eks", "Draw E Killsteal Target").SetValue(new Circle(true, Color.Red)));//1
            Menu.SubMenu("nightmoon.draw.setting").AddItem(new MenuItem("nightmoon.draw.rks", "Draw R Killsteal Target").SetValue(new Circle(true, Color.Red)));//1

            //Menu.AddItem(new MenuItem("nightmoon.sound.bool", "Play Sound").SetValue(true));

            Menu.AddItem(new MenuItem("nightmoon.Credit", "Credit : NightMoon"));
        }

        private static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 900);
            W.SetSkillshot(0.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 700);
        }
    }
}
