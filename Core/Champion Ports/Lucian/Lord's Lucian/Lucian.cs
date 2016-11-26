#region import
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using System.Collections.Generic;
using SharpDX;
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Lord_s_Lucian
{

    internal class Lucian
    {
        #region Intialize
        public bool CanUseSpells = true;
        public Spell E;
        private static AIHeroClient Player = ObjectManager.Player;
        private static string[] select = { "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "TwistedFate", "Twitch", "Urgot", "Varus", "Vayne" };
        public bool BuffGained;
        public Spell Q;
        public Spell Q2;
        public Spell R;
        public bool ActiveR;
        private static bool Dind => Menu.Item("Dind").GetValue<bool>();
        public int TickSpellCast;
        public Spell W;
        public bool RBuffWait;
        public static Menu Menu;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker;
        private static HpBar Indicator = new HpBar();
        public static List<string> ManaManagerList = new List<string>();
        bool ismixed;
        bool islasthit;
        bool islaneclear;
        #endregion
        #region Lucian()
        public Lucian()
        {
            LoadMenu();
            LoadSpells();
            PrintChatLoaded();

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += Game_OnGameUpdate;
        }
        #endregion
        #region AddManaManager
        public void AddManaManager(string menuname, int basicmana)
        {
            Menu.SubMenu(menuname).AddItem(new MenuItem("ManaManager_" + menuname, "Mana-Manager").SetValue(new Slider(basicmana)));
            ManaManagerList.Add("ManaManager_" + menuname);
        }
        #endregion
        #region Print Chat
        public void PrintChatLoaded()
        {
            Chat.Print("<font size='30'>Lord's Lucian</font> <font color='#b756c5'>by LordZEDith</font>");
        }
        #endregion
        #region ManaManager Bool
        public bool ManaManagerAllowCast(Spell spell)
        {

            if (ObjectManager.Player.ChampionName == "Lucian")
            {
                ismixed = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed &&
                          ManaManagerList.Contains("ManaManager_Harass");
                islasthit = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LastHit &&
                            ManaManagerList.Contains("ManaManager_LastHit");
                islaneclear = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear &&
                              ManaManagerList.Contains("ManaManager_LaneClear");
            }

            if (ismixed)
            {
                if ((int)ObjectManager.Player.Spellbook.GetSpell(spell.Slot).SData.Mana <= 1)
                {
                    return true;
                }
                if (GetManaPercent() >= Menu.Item("ManaManager_Harass").GetValue<Slider>().Value)
                {
                    return true;
                }
                return false;
            }
            if (islasthit)
            {
                if ((int)ObjectManager.Player.Spellbook.GetSpell(spell.Slot).SData.Mana <= 1)
                {
                    return true;
                }
                if (GetManaPercent() >= Menu.Item("ManaManager_LastHit").GetValue<Slider>().Value)
                {
                    return true;
                }
                return false;
            }
            if (islaneclear)
            {
                if ((int)ObjectManager.Player.Spellbook.GetSpell(spell.Slot).SData.Mana <= 1)
                {
                    return true;
                }
                if (GetManaPercent() >= Menu.Item("ManaManager_LaneClear").GetValue<Slider>().Value)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion
        #region LoadMenu
        private void LoadMenu()
        {

            Menu = new Menu("Lord's Lucian", "Lord's Lucian_" + ObjectManager.Player.ChampionName, true);
            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new SebbyLib.Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.Item("FarmDelay").SetValue(new Slider(125, 100, 200));
            Menu.AddSubMenu(new Menu("Combo", "Combo"));  
            Menu.SubMenu("Combo").SubMenu("Q Extended settings").AddItem(new MenuItem("QExtendedhero", "Q Only Certain Champions").SetValue(false));
            foreach (var hero in HeroManager.Enemies) { Menu.SubMenu("Combo").SubMenu("Q Extended settings").SubMenu("Certain Champions").AddItem(new MenuItem("autocom" + hero.ChampionName, hero.ChampionName).SetValue(select.Contains(hero.ChampionName))); }
            Menu.SubMenu("Combo").SubMenu("Q Extended settings").AddItem(new MenuItem("manac", "Mana").SetValue(new Slider(33, 100, 0)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useQ_Combo", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useW_Combo", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useE_Combo", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useE_Combo_Range", "E Target in Range").SetValue(new Slider(1100, 2000, 500)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useR_Combo", "Use R").SetValue(false));
            Menu.SubMenu("Combo").AddItem(new MenuItem("useR_Combo_2", "Use R if rest on CD").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("QExtended", "Q Extended").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("useQ_Harass", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("useW_Harass", "Use W").SetValue(true));
            AddManaManager("Harass", 50);

            Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("useQ_LaneClear", "Use Q").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("useW_LaneClear", "Use W").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("useE_LaneClear", "Use E").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("useE_LaneClear_Range", "E Minion in Range").SetValue(new Slider(1100, 2000, 500)));
            AddManaManager("LaneClear", 25);

            Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("useQ_LastHit", "Use Q").SetValue(true));
            AddManaManager("LastHit", 70);

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Dind", "Draw Damage Incidator").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
            Menu.AddToMainMenu();
        }
        #endregion
        #region LoadSpells
        private void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 675f);
            Q.SetTargetted(0.25f, float.MaxValue);

            Q2 = new Spell(SpellSlot.Q, 900f);
            Q2.SetSkillshot(0.35f, 25f, float.MaxValue, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1000f);
            W.SetSkillshot(0.3f, 80f, 1600, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 425f);
            E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 1200f);
            R.SetSkillshot(0.2f, 110f, 2800f, true, SkillshotType.SkillshotLine);
        }
        #endregion
        #region OnUpdate
        private void Game_OnGameUpdate(EventArgs args)
        {
            BuffCheck();
            UltCheck();

            switch (Orbwalker.ActiveMode)
            {
                case SebbyLib.Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.Item("useE_Combo").GetValue<bool>())
                    {
                        CastE();
                    }
                    if (Menu.Item("useQ_Combo").GetValue<bool>())
                    {
                        CastQEnemy();
                    }
                    if (Menu.Item("useW_Combo").GetValue<bool>())
                    {
                        CastWEnemy();
                    }
                    if (Menu.Item("useR_Combo").GetValue<bool>() ||
                         Menu.Item("useR_Combo_2").GetValue<bool>())
                    {
                        CastREnemy();
                    }
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.Mixed:
                    if (Menu.Item("useQ_Harass").GetValue<bool>() && ManaManagerAllowCast(Q))
                    {
                        CastQEnemy();
                    }
                    if (Menu.Item("useW_Harass").GetValue<bool>() && ManaManagerAllowCast(W))
                    {
                        CastWEnemy();
                    }
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.LaneClear:
                    if (Menu.Item("useE_LaneClear").GetValue<bool>() && ManaManagerAllowCast(E))
                    {
                        CastE();
                    }
                    if (Menu.Item("useQ_LaneClear").GetValue<bool>() && ManaManagerAllowCast(Q))
                    {
                        CastQEnemy();
                        CastQMinion();
                    }
                    if (Menu.Item("useW_LaneClear").GetValue<bool>() && ManaManagerAllowCast(W))
                    {
                        CastWEnemy();
                        CastWMinion();
                    }
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.LastHit:
                    if (Menu.Item("useQ_LastHit").GetValue<bool>() && ManaManagerAllowCast(Q))
                    {
                        CastQMinion();
                    }
                    break;
            }
            var qc = Menu.Item("QExtended").GetValue<bool>();
            if (qc)
            {
                var cerco = Menu.Item("QExtendedhero").GetValue<bool>();
                if (cerco)
                {
                    var manc = Menu.Item("manac").GetValue<Slider>().Value;
                    if (Q.IsReady() && (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100 > manc && Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo)
                    {
                        var tcc = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault(hero => Menu.Item("autocom" + hero.ChampionName).GetValue<bool>());
                        QExtendedCast(tcc);
                    }
                }

            }
        }
        #endregion
        #region Draw Events
        private void Drawing_OnDraw(EventArgs args)
        {

            var drawQ = Menu.Item("Draw_Q").IsActive();
            var drawW = Menu.Item("Draw_W").IsActive();
            var drawE = Menu.Item("Draw_E").IsActive();
            var drawR = Menu.Item("Draw_R").IsActive();

            if (Menu.Item("Draw_Disabled").GetValue<bool>())
            {
                return;
            }

            if (drawQ)
            {

                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);


            }

            if (drawW)
            {

                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);


            }

            if (drawE)
            {

                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            }

            if (drawR)
            {

                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, R.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            }
        }
        #endregion
        #region CastQEnemy
        private void CastQEnemy()
        {

            if (!Q.IsReady() || !CanUseSpells)
            {
                return;
            }
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if ((target.IsValidTarget(Q.Range)))
                {
                    Q.Cast(target);
                    UsedSkill();
                }
            }
            target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if ((!target.IsValidTarget(Q2.Range)) || !CanUseSpells || !Q.IsReady())
            {
                return;
            }
            var qCollision = Q2.GetPrediction(target).CollisionObjects;
            foreach (var qCollisionChar in qCollision.Where(qCollisionChar => qCollisionChar.IsValidTarget(Q.Range)))
            {
                Q.Cast(qCollisionChar);
                UsedSkill();
            }
        }
        #endregion
        #region CastQMinion
        private void CastQMinion()
        {
            if (!Q.IsReady() || !CanUseSpells)
            {
                return;
            }
            var lastHit = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LastHit;
            var laneClear = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear;
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (lastHit)
            {
                var minion =
                    allMinions.FirstOrDefault(
                        minionn =>
                            minionn.Distance(ObjectManager.Player) <= Q.Range &&
                            minionn.Health <= ObjectManager.Player.GetSpellDamage(minionn, SpellSlot.Q));
                if (minion == null)
                {
                    return;
                }
                Q.CastOnUnit(minion);
                UsedSkill();
            }
            else if (laneClear)
            {
                var minion = allMinions.FirstOrDefault(minionn => minionn.Distance(ObjectManager.Player) <= Q.Range);
                if (minion == null)
                {
                    return;
                }
                Q.CastOnUnit(minion);
                UsedSkill();
            }
        }
        #endregion
        #region CastWEnemy
        private void CastWEnemy()
        {
            if (!W.IsReady() || !CanUseSpells)
            {
                return;
            }
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (target.IsValidTarget(W.Range) && W.GetPrediction(target).Hitchance >= HitChance.Medium)
            {
                W.Cast(target);
                UsedSkill();
            }
            else if (W.GetPrediction(target).Hitchance == HitChance.Collision)
            {
                var wCollision = W.GetPrediction(target).CollisionObjects;
                foreach (
                    var wCollisionChar in wCollision.Where(wCollisionChar => wCollisionChar.Distance(target) <= 100))
                {
                    W.Cast(wCollisionChar.Position);
                    UsedSkill();
                }
            }
        }
        #endregion
        #region CastWMinion
        private void CastWMinion()
        {
            if (!W.IsReady() || !CanUseSpells)
            {
                return;
            }
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.Position, W.Range + 100, MinionTypes.All, MinionTeam.NotAlly);
            var minion = allMinions.FirstOrDefault(minionn => minionn.IsValidTarget(W.Range));
            if (minion != null)
            {
                W.Cast(minion.Position);
                UsedSkill();
            }
        }
        #endregion
        #region CastREnemy
        private void CastREnemy()
        {
            if ((Menu.Item("useR_Combo_2").GetValue<bool>() && (Q.IsReady() || W.IsReady() || E.IsReady())) ||
                (!R.IsReady() || !CanUseSpells))
            {
                return;
            }
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget(R.Range))
            {
                R.Cast(target);
                UsedSkill();
                ActiveR = true;
            }
        }
        #endregion
        #region CastE
        private void CastE()
        {
            if (!E.IsReady() || !CanUseSpells)
            {
                return;
            }
            var combo = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo;
            var comboRange = Menu.Item("useE_Combo_Range").GetValue<Slider>().Value;

            var laneClear = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear;
            var laneClearRange = Menu.Item("useE_LaneClear_Range").GetValue<Slider>().Value;


            if (combo)
            {
                var target = TargetSelector.GetTarget(comboRange, TargetSelector.DamageType.Physical);
                if (!target.IsValidTarget(comboRange))
                {
                    return;
                }
                E.Cast(Game.CursorPos);
                UsedSkill();
            }
            else if (laneClear)
            {
                var allMinions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, laneClearRange, MinionTypes.All, MinionTeam.NotAlly);
                foreach (var minion in
                    allMinions.Where(minion => minion != null)
                        .Where(minion => minion.IsValidTarget(laneClearRange) && E.IsReady()))
                {
                    E.Cast(Game.CursorPos);
                    UsedSkill();
                }
            }
        }
        #endregion
        #region UsedSkill
        private void UsedSkill()
        {
            if (!CanUseSpells)
            {
                return;
            }
            CanUseSpells = false;
            TickSpellCast = Environment.TickCount;
        }
        #endregion
        #region UltCheck
        private void UltCheck()
        {
            var tempultactive = false;
            foreach (var buff in ObjectManager.Player.Buffs.Where(buff => buff.Name == "LucianR"))
            {
                tempultactive = true;
            }

            if (tempultactive)
            {
                Orbwalker.SetAttack(false);
                ActiveR = true;
            }
            if (!tempultactive)
            {
                Orbwalker.SetAttack(true);
                ActiveR = false;
            }
        }
        #endregion
        #region BuffCheck
        private void BuffCheck()
        {
            if (CanUseSpells == false && RBuffWait == false && BuffGained == false)
            {
                RBuffWait = true;
            }

            if (RBuffWait)
            {
                foreach (var buff in ObjectManager.Player.Buffs.Where(buff => buff.Name == "lucianpassivebuff"))
                {
                    BuffGained = true;
                }
            }

            if (BuffGained)
            {
                RBuffWait = false;
                var tempgotBuff = false;
                foreach (var buff in ObjectManager.Player.Buffs.Where(buff => buff.Name == "lucianpassivebuff"))
                {
                    tempgotBuff = true;
                }
                if (tempgotBuff == false)
                {
                    BuffGained = false;
                    CanUseSpells = true;
                }
            }

            if (TickSpellCast >= Environment.TickCount - 1000 || RBuffWait != true)
            {
                return;
            }
            RBuffWait = false;
            BuffGained = false;
            CanUseSpells = true;
        }
        #endregion
        #region ManaPercent
        public float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }
        #endregion
        #region HpIndicator
        public float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (E.IsReady()) damage = damage + (float)Player.GetAutoAttackDamage(enemy) * 2;
                if (W.IsReady()) damage = damage + W.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);
                if (Q.IsReady())
                {
                    damage = damage + Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);
                }
                damage = damage + (float)Player.GetAutoAttackDamage(enemy);

                return damage;
            }
            return 0;
        }
        public void Drawing_OnEndScene(EventArgs args)
        {
            if (Dind)
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 160));

                }
            }
        }
        #endregion
        #region QExtendedCast
        private void QExtendedCast(AIHeroClient unit)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 675, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in minions)
            {
                if (Q2.WillHit(unit, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, 1150), 0, HitChance.VeryHigh))
                {
                    Q2.CastOnUnit(minion);
                }
            }
        }
        #endregion
    }
}