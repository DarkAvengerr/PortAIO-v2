using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class Graves
    {
        public Graves()
        {
            GravesOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void GravesOnLoad()
        {
            GravesMenu.Config =
                new Menu("hikiMarksman:AIO - Graves", "hikiMarksman:AIO - Graves", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                GravesSpells.Init();
                GravesMenu.OrbwalkerInit();
                GravesMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Graves: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");


            HikiSprite.Add(0);
            HikiSprite.OnDraw();
            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => HikiSprite.Remove());

            Notifications.AddNotification(String.Format("hikiMarksman:AIO - {0} Loaded !", ObjectManager.Player.ChampionName), 4000);

            Game.OnUpdate += GravesOnUpdate;
            AIHeroClient.OnSpellCast += GravesOnProcessSpellCast;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += GravesOnDraw;
        }

        private static void GravesOnDraw(EventArgs args)
        {
            GravesDrawing.Init();
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (GravesMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Helper.Enabled("graves.e.combo") && GravesSpells.E.IsReady()
                && target.IsValidTarget(GravesSpells.Q.Range))
            {
                GravesSpells.E.Cast(Game.CursorPos);
            }
        }

        private static void GravesOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (GravesMenu.Config.Item("graves.e.gapclosex").GetValue<StringList>().SelectedIndex == 0)
            {
                Helper.GravesAntiGapcloser(sender, args);
            }
        }
        private static void GravesOnUpdate(EventArgs args)
        {
            switch (GravesMenu.Orbwalker.ActiveMode)
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
            if (Helper.Enabled("graves.q.harass") && GravesSpells.Q.IsReady() && ObjectManager.Player.ManaPercent > Helper.Slider("graves.harass.mana"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(GravesSpells.Q.Range) && Helper.Enabled("graves.q.toggle."+x.ChampionName) &&
                    GravesSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    GravesSpells.Q.Cast(enemy);
                }
            }
        }

        private static void Combo()
        {
            if (GravesSpells.Q.IsReady() && Helper.Enabled("graves.q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(GravesSpells.Q.Range) && 
                    GravesSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    GravesSpells.Q.Cast(enemy);
                }
            }
            if (GravesSpells.W.IsReady() && Helper.Enabled("graves.w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(GravesSpells.W.Range) &&
                    GravesSpells.W.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    GravesSpells.W.Cast(enemy);
                }
            }
            if (GravesSpells.R.IsReady() && Helper.Enabled("graves.r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(GravesSpells.R.Range) &&
                    GravesSpells.R.GetPrediction(x).Hitchance >= HitChance.High && GravesSpells.R.GetDamage(x) > x.Health))
                {
                    GravesSpells.R.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("graves.harass.mana"))
            {
                return;
            }
            if (GravesSpells.Q.IsReady() && Helper.Enabled("graves.q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(GravesSpells.Q.Range) &&
                    GravesSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    GravesSpells.Q.Cast(enemy);
                }
            }
            if (GravesSpells.W.IsReady() && Helper.Enabled("graves.w.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(GravesSpells.W.Range) &&
                    GravesSpells.W.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    GravesSpells.W.Cast(enemy);
                }
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("graves.clear.mana"))
            {
                return;
            }

            if (GravesSpells.Q.IsReady() && Helper.Enabled("graves.q.clear") && MinionManager.GetMinions(ObjectManager.Player.Position, GravesSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly).Count >= Helper.Slider("graves.q.minion.hit.count")
                && GravesSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, GravesSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.Slider("graves.q.minion.hit.count"))
            {
                GravesSpells.Q.Cast(GravesSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, GravesSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
            }

        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("graves.jungle.mana") && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth) == null ||
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Count == 0)
            {
                return;
            }
            if (GravesSpells.Q.IsReady() && Helper.Enabled("graves.q.jungle"))
            {
                GravesSpells.Q.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0]);
            }
            if (GravesSpells.W.IsReady() && Helper.Enabled("graves.w.jungle"))
            {
                GravesSpells.W.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0].Position);
            }
        }
    }
}
