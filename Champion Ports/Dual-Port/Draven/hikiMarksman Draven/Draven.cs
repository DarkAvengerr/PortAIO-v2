using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Drawings;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Champions
{

    class Draven
    {
        public Draven()
        {
            DravenOnLoad();
        }

        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void DravenOnLoad()
        {
            DravenMenu.Config =
                new Menu("hikiMarksman:AIO - Draven", "hikiMarksman:AIO - Draven", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                DravenSpells.Init();
                DravenMenu.OrbwalkerInit();
                DravenMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Draven: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");

            Notifications.AddNotification("hikiMarksman:AIO", 4000);
            Notifications.AddNotification(String.Format("{0} Loaded", ObjectManager.Player.ChampionName), 5000);
            Notifications.AddNotification("Gift From Hikigaya", 6000);

            Game.OnUpdate += DravenOnUpdate;
            AIHeroClient.OnSpellCast += DravenAxeHelper.AIHeroClient_OnProcessSpellCast;
            GameObject.OnCreate += DravenAxeHelper.OnCreate;
            GameObject.OnDelete += DravenAxeHelper.OnDelete;
            Interrupter2.OnInterruptableTarget += DravenOnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += DravenOnEnemyGapcloser;
            Drawing.OnDraw += DravenOnDraw;
        }

        private static void DravenOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (DravenSpells.E.IsReady() && gapcloser.Sender.IsValidTarget(DravenSpells.E.Range) && Helper.DEnabled("draven.e.antigapcloser"))
            {
                DravenSpells.E.Cast(gapcloser.Sender);
            }
        }

        private static void DravenOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!DravenMenu.Config.Item("draven.e.interrupter").GetValue<bool>() || !sender.IsValidTarget()) return;
            Interrupter2.DangerLevel a;
            switch (DravenMenu.Config.Item("min.interrupter.danger.level").GetValue<StringList>().SelectedValue)
            {
                case "HIGH":
                    a = Interrupter2.DangerLevel.High;
                    break;
                case "MEDIUM":
                    a = Interrupter2.DangerLevel.Medium;
                    break;
                default:
                    a = Interrupter2.DangerLevel.Low;
                    break;
            }

            if (args.DangerLevel == Interrupter2.DangerLevel.High ||
                args.DangerLevel == Interrupter2.DangerLevel.Medium && a != Interrupter2.DangerLevel.High ||
                args.DangerLevel == Interrupter2.DangerLevel.Medium && a != Interrupter2.DangerLevel.Medium &&
                a != Interrupter2.DangerLevel.High)
            {
                if (DravenSpells.E.IsReady() && sender.IsValidTarget(DravenSpells.E.Range))
                {
                    DravenSpells.E.Cast(sender);
                }
            }
        }

        private static void DravenOnUpdate(EventArgs args)
        {
            switch (DravenMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    Jungle();
                    break;
            }
        }



        private static void Combo()
        {
            if (DravenSpells.Q.IsReady() && Helper.DEnabled("draven.q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player))
                    && DravenAxeHelper.LastQ + 100 < Environment.TickCount && DravenAxeHelper.CurrentAxes < Helper.DSlider("draven.q.combo.axe.count")))
                {
                    DravenSpells.Q.Cast();
                }
            }
            if (DravenSpells.E.IsReady() && Helper.DEnabled("draven.e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(DravenSpells.E.Range) &&
                    DravenSpells.E.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    DravenSpells.E.Cast(enemy);
                }
            }
            if (DravenSpells.R.IsReady() && Helper.DEnabled("draven.r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(3000) && ObjectManager.Player.Distance(o.Position) > Helper.DSlider("draven.min.ult.distance") &&
                    ObjectManager.Player.Distance(o.Position) < Helper.DSlider("draven.max.ult.distance") && DravenSpells.R.GetPrediction(o).Hitchance >= HitChance.Medium &&
                    o.Health < DravenSpells.R.GetDamage(o)))
                {
                    DravenSpells.R.Cast(enemy);
                }
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.DSlider("draven.clear.mana"))
            {
                return;
            }

            if (DravenSpells.Q.IsReady() && Helper.DEnabled("draven.q.clear"))
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, EzrealSpells.Q.Range,
                    MinionTypes.All, MinionTeam.NotAlly);
                if (minions.Count > Helper.DSlider("draven.q.minion.count") &&
                    DravenAxeHelper.CurrentAxes < Helper.DSlider("draven.q.lane.clear.axe.count"))
                {
                    DravenSpells.Q.Cast();
                }
            }
        }
        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Helper.DSlider("draven.jungle.mana") && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth) == null ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Count == 0)
            {
                return;
            }
            if (DravenSpells.Q.IsReady() && DravenAxeHelper.CurrentAxes < Helper.DSlider("draven.q.jungle.clear.axe.count") && Helper.DEnabled("draven.q.jungle"))
            {
                DravenSpells.Q.Cast();
            }
            if (DravenSpells.E.IsReady() && Helper.DEnabled("draven.e.jungle"))
            {
                DravenSpells.E.Cast(MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0]);
            }

        }
        private static void DravenOnDraw(EventArgs args)
        {
            DravenDrawing.Init();
        }
    }
}
