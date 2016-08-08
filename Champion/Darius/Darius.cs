using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Darius
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        public Spell Q, W, E, R;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private AIHeroClient Player { get { return ObjectManager.Player; } }

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 145);
            E = new Spell(SpellSlot.E, 540);
            R = new Spell(SpellSlot.R, 460);

            E.SetSkillshot(0.01f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
            SebbyLib.Orbwalking.AfterAttack += afterAttack;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
        }

        private void LoadMenuOKTW()
        {
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            
            Config.SubMenu(Player.ChampionName).SubMenu("Q option").AddItem(new MenuItem("haras", "Harras Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q option").AddItem(new MenuItem("qOutRange", "Auto Q only out range AA", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoRbuff", "Auto R if darius execute multi cast time out ", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoRdeath", "Auto R if darius execute multi cast and under 10 % hp", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Farm W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));
        }

        private void OnInterruptableSpell(AIHeroClient unit, InterruptableSpell spell)
        {
            if (E.LSIsReady()  && unit.LSIsValidTarget(E.Range))
                E.Cast(unit);
        }


        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Player.Mana < RMANA + WMANA || !W.LSIsReady() || !unit.IsMe)
                return;

            var t = target as AIHeroClient;

            if (t.LSIsValidTarget())
                W.Cast();

        }

        private void BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {


        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (R.LSIsReady() && Config.Item("useR", true).GetValue<KeyBind>().Active )
            {
                var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.True);
                if (targetR.LSIsValidTarget())
                    R.Cast(targetR, true);
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && W.LSIsReady())
                LogicW();
            if (Program.LagFree(2) && Q.LSIsReady())
                LogicQ();
            if (Program.LagFree(3) && E.LSIsReady())
                LogicE();
            if (Program.LagFree(4) && R.LSIsReady() && Config.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicW()
        {
            if (!Player.Spellbook.IsAutoAttacking && Config.Item("farmW", true).GetValue<bool>() && Program.Farm)
            {
                var minions = Cache.GetMinions(Player.Position, Player.AttackRange);

                int countMinions = 0;

                foreach (var minion in minions.Where(minion => minion.Health < W.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions > 0)
                    W.Cast();
            }
        }

        private void LogicE()
        {
            if (Player.Mana > RMANA + EMANA )
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target.LSIsValidTarget() && ((Player.LSUnderTurret(false) && !Player.LSUnderTurret(true)) || Program.Combo) )
                {
                    if (!SebbyLib.Orbwalking.InAutoAttackRange(target))
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (!Config.Item("qOutRange", true).GetValue<bool>() || SebbyLib.Orbwalking.InAutoAttackRange(t))
                {
                    if (Player.Mana > RMANA + QMANA && Program.Combo)
                        Q.Cast();
                    else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA && Config.Item("haras", true).GetValue<bool>())
                        Q.Cast();
                }

                if (!R.LSIsReady() && OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Q.Cast();
            }
            
            else if (Config.Item("farmQ", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + EMANA + WMANA && Program.LaneClear)
            {
                var minionsList = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (minionsList.Any(x => Player.LSDistance(x.ServerPosition) > 300 && x.Health < Q.GetDamage(x) * 0.6))
                    Q.Cast();
                        
            }
        }

        private void LogicR()
        {
            var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.True);
            if (targetR.LSIsValidTarget() && OktwCommon.ValidUlt(targetR) && Config.Item("autoRbuff", true).GetValue<bool>())
            {
                var buffTime = OktwCommon.GetPassiveTime(Player, "dariusexecutemulticast");
                if((buffTime < 2 || (Player.HealthPercent < 10 && Config.Item("autoRdeath", true).GetValue<bool>())) && buffTime > 0)
                    R.Cast(targetR, true);
            }

            foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(R.Range) && OktwCommon.ValidUlt(target) ))
            {

                var dmgR = OktwCommon.GetKsDamage(target, R);
                if (target.HasBuff("dariushemo"))
                    dmgR += R.GetDamage(target) * target.GetBuff("dariushemo").Count * 0.2f;

                if (dmgR > target.Health + target.HPRegenRate)
                {
                    R.Cast(target);
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>() && Q.LSIsReady())
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>() && E.LSIsReady())
                    if (E.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
            } 
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>() && R.LSIsReady())
                    if (R.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
            }
        }
        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.LSIsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }
        
    }
}
