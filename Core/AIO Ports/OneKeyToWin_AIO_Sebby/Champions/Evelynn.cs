using System;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

using EloBuddy; 
using LeagueSharp.Common; 
namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Evelynn : Base
    {
        public Evelynn()
        {
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 250f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("slowW", "Auto W slow", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("rCount", "Auto R x enemies", true).SetValue(new Slider(3, 0, 5)));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("laneQ", "Lane clear Q", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += GameOnOnUpdate;
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            if (Config.Item("useR", true).GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R.CastIfWillHit(t, 2, true);
                    R.Cast(t, true, true);
                }
            }
            if (Program.Combo)
            {
                if (Program.LagFree(1) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                    LogicQ();
                if (Program.LagFree(2) && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                    LogicE();
                if (Program.LagFree(3) && W.IsReady())
                    LogicW();
                if (Program.LagFree(4) && R.IsReady())
                    LogicR();
            }
            else if (Program.LaneClear)
            {
                Jungle();
            }
        }

        private void LogicQ()
        {
            if (Player.CountEnemiesInRange(Q.Range) > 0)
                Q.Cast();
        }

        private void LogicE()
        {
           var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
           if (t.IsValidTarget())
           {
               E.CastOnUnit(t);
           }
        }

        private void LogicW()
        {
            if (Config.Item("autoW", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA && Player.CountEnemiesInRange(W.Range) > 0)
                W.Cast();
            else if (Config.Item("slowW", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA && Player.HasBuffOfType(BuffType.Slow))
                W.Cast();
        }

        private void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                var poutput = R.GetPrediction(t, true);

                var aoeCount = poutput.AoeTargetsHitCount;

                aoeCount = (aoeCount == 0) ? 1 : aoeCount;

                if (Config.Item("rCount", true).GetValue<Slider>().Value > 0 && Config.Item("rCount", true).GetValue<Slider>().Value <= aoeCount)
                    R.Cast(poutput.CastPosition);

                if (Player.HealthPercent < 60)
                {
                    double dmg = OktwCommon.GetIncomingDamage(Player);
                    var enemys = Player.CountEnemiesInRange(700);
                    if (Player.Health - dmg < enemys * Player.Level * 20)
                        R.Cast(poutput.CastPosition);
                    else if (Player.Health - dmg < Player.Level * 10)
                        R.Cast(poutput.CastPosition);
                }
            }      
        }

        private void Jungle()
        {
            if (Player.ManaPercent < Config.Item("Mana", true).GetValue<Slider>().Value)
                return;
            var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Config.Item("jungleE", true).GetValue<bool>() && E.IsReady())
                    E.CastOnUnit(mob);
                if (Config.Item("jungleQ", true).GetValue<bool>() && Q.IsReady())
                    Q.Cast();
            }

            if (Config.Item("laneQ", true).GetValue<bool>() && Q.IsReady())
                Q.Cast();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }

}
