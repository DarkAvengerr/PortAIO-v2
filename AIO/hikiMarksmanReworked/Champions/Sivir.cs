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
    public class Sivir
    {
        public Sivir()
        {
            SivirOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void SivirOnLoad()
        {
            SivirMenu.Config =
                new Menu("hikiMarksman:AIO - Sivir", "hikiMarksman:AIO - Sivir", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                SivirSpells.Init();
                SivirMenu.OrbwalkerInit();
                SivirMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Sivir: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");


            HikiSprite.Add(0);
            HikiSprite.OnDraw();
            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => HikiSprite.Remove());

            Notifications.AddNotification(String.Format("hikiMarksman:AIO - {0} Loaded !", ObjectManager.Player.ChampionName), 4000);

            Game.OnUpdate += SivirOnUpdate;
            AIHeroClient.OnProcessSpellCast += SivirOnProcessSpellCast;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += SivirOnDraw;
        }

        private static void SivirOnUpdate(EventArgs args)
        {
            switch (SivirMenu.Orbwalker.ActiveMode)
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
            if (Helper.SEnabled("sivir.q.harass") && SivirSpells.Q.IsReady() && ObjectManager.Player.ManaPercent > Helper.SSlider("sivir.harass.mana"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(SivirSpells.Q.Range) && Helper.SEnabled("sivir.q.toggle." + x.ChampionName) &&
                    SivirSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    SivirSpells.Q.Cast(enemy);
                }
            }
        }

        private static void Combo()
        {
            if (SivirSpells.Q.IsReady() && Helper.SEnabled("sivir.q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(SivirSpells.Q.Range) &&
                    SivirSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    SivirSpells.Q.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.SSlider("sivir.harass.mana"))
            {
                return;
            }
            if (SivirSpells.Q.IsReady() && Helper.SEnabled("sivir.q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(SivirSpells.Q.Range) &&
                    SivirSpells.Q.GetPrediction(x).Hitchance >= HitChance.High))
                {
                    SivirSpells.Q.Cast(enemy);
                }
            }
            
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.SSlider("sivir.clear.mana"))
            {
                return;
            }

            if (SivirSpells.Q.IsReady() && Helper.SEnabled("sivir.q.harass") && MinionManager.GetMinions(ObjectManager.Player.Position, SivirSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly).Count >= Helper.SSlider("sivir.q.minion.hit.count")
                && SivirSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, SivirSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.SSlider("sivir.q.minion.hit.count"))
            {
                SivirSpells.Q.Cast(SivirSpells.Q.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, SivirSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
            }
        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Helper.SSlider("sivir.jungle.mana") && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth) == null ||
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Count == 0)
            {
                return;
            }
            if (SivirSpells.Q.IsReady() && Helper.SEnabled("sivir.q.jungle"))
            {
                SivirSpells.Q.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0]);
            }
            if (SivirSpells.W.IsReady() && Helper.SEnabled("sivir.w.jungle"))
            {
                SivirSpells.W.Cast(
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth)[0].Position);
            }
        }

        private static void SivirOnDraw(EventArgs args)
        {
            SivirDrawing.Init();
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (SivirMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Helper.SEnabled("sivir.w.combo") && SivirSpells.W.IsReady()
                 && target.IsValidTarget(SivirSpells.W.Range))
            {
                SivirSpells.W.Cast();
            }
        }

        private static void SivirOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (ObjectManager.Player.Distance(spell.End) <= 250 && sender.IsEnemy)
            {
                foreach (var block in EvadeDb.SpellData.SpellDatabase.Spells.Where(o => o.spellName == spell.SData.Name))
                {
                    if (SivirMenu.Config.Item("block." + block.spellName).GetValue<bool>())
                    {
                        SivirSpells.E.Cast();
                    }
                }
            } 
        }
    }
}
