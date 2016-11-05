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
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Champions
{
    public class Ezreal
    {
        public Ezreal()
        {
            EzrealOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void EzrealOnLoad()
        {
            EzrealMenu.Config =
                 new Menu("hikiMarksman:AIO - Ezreal", "hikiMarksman:AIO - Ezreal", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                     SharpDX.Color.Gold);
            {
                EzrealSpells.Init();
                EzrealMenu.OrbwalkerInit();
                EzrealMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Ezreal: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");


            HikiSprite.Add(0);
            HikiSprite.OnDraw();
            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => HikiSprite.Remove());

            Notifications.AddNotification("hikiMarksman:AIO", 4000);
            Notifications.AddNotification(String.Format("{0} Loaded", ObjectManager.Player.ChampionName), 5000);
            Notifications.AddNotification("Gift From Hikigaya",6000);

            Game.OnUpdate += EzrealOnUpdate;
            AIHeroClient.OnSpellCast += EzrealOnProcessSpellCast;
            Drawing.OnDraw += EzrealOnDraw;
        }

        private static void EzrealOnUpdate(EventArgs args)
        {
            switch (EzrealMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    Jungle();
                    break;
            }
            if (Helper.EEnabled("ezreal.q.harass") && EzrealSpells.Q.IsReady() && ObjectManager.Player.ManaPercent > Helper.ESlider("ezreal.harass.mana"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(EzrealSpells.Q.Range) && Helper.EEnabled("ezreal.q.toggle." + x.ChampionName) &&
                    EzrealSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.Q.Cast(enemy);
                }
            }

        }

        private static void Combo()
        {
            if (EzrealSpells.Q.IsReady() && Helper.EEnabled("ezreal.q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(EzrealSpells.Q.Range) &&
                    EzrealSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.Q.Cast(enemy);
                }
            }
            if (EzrealSpells.W.IsReady() && Helper.EEnabled("ezreal.w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(EzrealSpells.W.Range) &&
                    EzrealSpells.W.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.W.Cast(enemy);
                }
            }
            if (EzrealSpells.R.IsReady() && Helper.EEnabled("ezreal.r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(EzrealSpells.R.Range) && ObjectManager.Player.Distance(o.Position) > Helper.ESlider("ezreal.min.ult.distance")
                    && ObjectManager.Player.Distance(o.Position) < Helper.ESlider("ezreal.max.ult.distance") && EzrealSpells.R.GetPrediction(o).Hitchance >= HitChance.High
                    && o.Health < EzrealSpells.R.GetDamage(o)))
                {
                    EzrealSpells.R.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.ESlider("ezreal.harass.mana"))
            {
                return;
            }
            if (EzrealSpells.Q.IsReady() && Helper.EEnabled("ezreal.q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(EzrealSpells.Q.Range) &&
                    EzrealSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.Q.Cast(enemy);
                }
            }
            if (EzrealSpells.W.IsReady() && Helper.EEnabled("ezreal.w.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(EzrealSpells.W.Range) &&
                    EzrealSpells.W.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.W.Cast(enemy);
                }
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.ESlider("ezreal.clear.mana"))
            {
                return;
            }

            if (EzrealSpells.Q.IsReady() && Helper.EEnabled("ezreal.q.harass"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.Position, EzrealSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(x => x.Health < EzrealSpells.Q.GetDamage(x) && EzrealSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    EzrealSpells.Q.Cast(minion);
                }
            }

        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Helper.ESlider("ezreal.jungle.mana") && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth) == null ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Count == 0)
            {
                return;
            }
            if (EzrealSpells.Q.IsReady() && Helper.EEnabled("ezreal.q.jungle"))
            {
                EzrealSpells.Q.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0]);
            }
            if (EzrealSpells.E.IsReady() && Helper.EEnabled("ezreal.e.jungle"))
            {
                EzrealSpells.E.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0].Position);
            }
        }

        private static void EzrealOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (EzrealMenu.Config.Item("ezreal.e.gapclosex").GetValue<StringList>().SelectedIndex == 0)
            {
                Helper.EzrealAntiGapcloser(sender, args);
            }
        }

        private static void EzrealOnDraw(EventArgs args)
        {
            EzrealDrawing.Init();
        }
    }
}
