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

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Champions
{
    class Vayne
    {
        public Vayne()
        {
            VayneOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void VayneOnLoad()
        {
            VayneMenu.Config =
                new Menu("hikiMarksman:AIO - Vayne", "hikiMarksman:AIO - Vayne", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                VayneSpells.Init();
                VayneMenu.OrbwalkerInit();
                VayneMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Lucian: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");

            Notifications.AddNotification("hikiMarksman:AIO", 4000);
            Notifications.AddNotification(String.Format("{0} Loaded", ObjectManager.Player.ChampionName), 5000);
            Notifications.AddNotification("Gift From Hikigaya", 6000);

            Game.OnUpdate += VayneOnUpdate;
            AIHeroClient.OnSpellCast += VayneOnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += VayneOnSpellCast;
            Drawing.OnDraw += VayneOnDraw;
        }

        private static void VayneOnUpdate(EventArgs args)
        {
            switch (VayneMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break; 
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        private static void Clear()
        {
            if (VayneSpells.E.IsReady() && Helper.VEnabled("vayne.condemn.jungle.mobs"))
            {
                foreach (var junglemobs in ObjectManager.Get<Obj_AI_Minion>().Where(x=> x.IsValidTarget(VayneSpells.E.Range) && x.Team == GameObjectTeam.Neutral &&
                    (x.CharData.BaseSkinName == "SRU_Razorbeak" || x.CharData.BaseSkinName == "SRU_Red" ||
                     x.CharData.BaseSkinName == "SRU_Blue" || x.CharData.BaseSkinName == "SRU_Gromp" ||
                     x.CharData.BaseSkinName == "SRU_Krug" || x.CharData.BaseSkinName == "SRU_Murkwolf" ||
                     x.CharData.BaseSkinName == "Sru_Crab")))
                {
                    VayneHelper.VhrBasicJungleCondemn(junglemobs);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.VSlider("vayne.harass.mana"))
            {
                return;
            }

            if (VayneMenu.Config.Item("harass.type").GetValue<StringList>().SelectedIndex == 0 && VayneSpells.Q.IsReady())
            {
                foreach (var qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)
                    && x.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2)))
                {
                    VayneHelper.TumbleCast();
                }
            }
            if (VayneMenu.Config.Item("harass.type").GetValue<StringList>().SelectedIndex == 1 && VayneSpells.E.IsReady())
            {
                foreach (var etarget in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)
                    && x.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2)))
                {
                    VayneSpells.E.CastOnUnit(etarget);
                }
            }
        }

        private static void Combo()
        {
            if (VayneSpells.E.IsReady() && Helper.VEnabled("vayne.e.combo"))
            {
                VayneHelper.CondemnCast();
            }

            if (VayneSpells.R.IsReady() && Helper.VEnabled("vayne.r.combo") 
                && ObjectManager.Player.CountEnemiesInRange(Helper.VSlider("vayne.auto.r.search.range")) >= Helper.VSlider("vayne.auto.r.enemy.count")
                && ObjectManager.Player.HealthPercent <= Helper.VSlider("vayne.auto.r.minimum.health"))
            {
                VayneSpells.R.Cast();
            }
        }

        private static void VayneOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (VayneSpells.E.IsReady())
            {
                Helper.VayneAntiGapcloser(sender,args);
            }
        }

        private static void VayneOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid 
                && VayneMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Helper.VEnabled("vayne.q.combo")
                && Helper.VEnabled("vayne.q.after.aa"))
            {
                VayneHelper.TumbleCast();
            }
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid
                && VayneMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Helper.VEnabled("vayne.q.combo")
                && Helper.VEnabled("vayne.auto.q.if.enemy.has.2.stack") && ((AIHeroClient)args.Target).GetBuffCount("vaynesilvereddebuff") == 2)
            {
                VayneHelper.TumbleCast();
            }

            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid
                && VayneMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Helper.VEnabled("vayne.q.combo")
                && Helper.VEnabled("vayne.tumble.jungle.mobs") && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && (((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Razorbeak" ||((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Red" ||
                ((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Blue" || ((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Gromp" ||
                ((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Krug" || ((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "SRU_Murkwolf" ||
                ((Obj_AI_Minion)args.Target).CharData.BaseSkinName == "Sru_Crab"))
            {
                VayneHelper.TumbleCast();
            }

        }

        private static void VayneOnDraw(EventArgs args)
        {
            VayneDrawing.Init();
        }
        
    }
}
