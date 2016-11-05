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
using PortAIO.Properties;

namespace hikiMarksmanRework.Champions
{
    public class Corki
    {
        public Corki()
        {
            CorkiOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void CorkiOnLoad()
        {
            CorkiMenu.Config =
                new Menu("hikiMarksman:AIO - Corki", "hikiMarksman:AIO - Corki", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                CorkiSpells.Init();
                CorkiMenu.OrbwalkerInit();
                CorkiMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Corki: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");


            HikiSprite.Add(0);
            HikiSprite.OnDraw();
            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => HikiSprite.Remove());

            Notifications.AddNotification("hikiMarksman:AIO", 4000);
            Notifications.AddNotification(String.Format("{0} Loaded", ObjectManager.Player.ChampionName), 5000);
            Notifications.AddNotification("Gift From Hikigaya", 6000);

            Game.OnUpdate += CorkiOnUpdate;
            AIHeroClient.OnSpellCast += CorkiOnProcessSpellCast;
            Orbwalking.AfterAttack += CorkiAfterAttack;
            Drawing.OnDraw += CorkiOnDraw;
        }

        private static void CorkiOnUpdate(EventArgs args)
        {
            switch (CorkiMenu.Orbwalker.ActiveMode)
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
        }

        private static void Combo()
        {
            if (CorkiSpells.Q.IsReady() && Helper.CEnabled("corki.q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.Q.Range) &&
                    CorkiSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.Q.Cast(enemy);
                }
            }
            if (CorkiSpells.R.IsReady() && Helper.CEnabled("corki.r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.R.Range) &&
                    CorkiSpells.R.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.R.Cast(enemy);
                }
            }
            if (CorkiSpells.R.IsReady() && Helper.CEnabled("corki.r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.BIG.Range) &&
                    CorkiSpells.BIG.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.BIG.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.CSlider("corki.harass.mana"))
            {
                return;
            }
            if (CorkiSpells.Q.IsReady() && Helper.CEnabled("corki.q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.Q.Range) &&
                    CorkiSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.Q.Cast(enemy);
                }
            }
            if (CorkiSpells.R.IsReady() && Helper.CEnabled("corki.r.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.R.Range) &&
                    CorkiSpells.R.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.R.Cast(enemy);
                }
            }
            if (CorkiSpells.R.IsReady() && Helper.CEnabled("corki.r.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CorkiSpells.BIG.Range) &&
                    CorkiSpells.BIG.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    CorkiSpells.BIG.Cast(enemy);
                }
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.CSlider("corki.clear.mana"))
            {
                return;
            }

            if (CorkiSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, CorkiSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.CSlider("corki.q.minion.hit.count"))
            {
                CorkiSpells.Q.Cast(CorkiSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, CorkiSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
            }
        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Helper.CSlider("corki.jungle.mana") && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth) == null ||
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Count == 0)
            {
                return;
            }

            if (CorkiSpells.Q.IsReady() && Helper.CEnabled("corki.q.jungle"))
            {
                CorkiSpells.Q.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0]);
            }

            if (CorkiSpells.E.IsReady() && Helper.CEnabled("corki.e.jungle"))
            {
                CorkiSpells.E.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0].Position);
            }

            if (CorkiSpells.R.IsReady() && Helper.CEnabled("corki.r.jungle"))
            {
                CorkiSpells.R.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0].Position);
            }
        }

        private static void CorkiOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (CorkiMenu.Config.Item("corki.w.gapclosex").GetValue<StringList>().SelectedIndex == 0)
            {
                Helper.CorkiAntiGapcloser(sender, args);
            }
        }

        private static void CorkiAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (CorkiMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Helper.CEnabled("corki.e.combo") && CorkiSpells.E.IsReady()
                && target.IsValidTarget(CorkiSpells.E.Range-100))
            {
                CorkiSpells.E.Cast(target.Position);
            }
        }

        private static void CorkiOnDraw(EventArgs args)
        {
            CorkiDrawing.Init();
        }
    }
}
