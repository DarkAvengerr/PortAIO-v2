using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;
using Hikigaya_Lux.Logic;
using Hikigaya_Lux.Spell_Database;
using SharpDX;
using SharpDX.Direct3D9;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Champions
{
    public class Lux
    {

        public Lux()
        {
            LuxOnLoad();
        }
        static void LuxOnLoad()
        {
            LuxMenu.Config =
                new Menu("Hikigaya - Lux", "Hikigaya - Lux", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    Color.Gold);
            {
                Spells.Init();
                LuxMenu.OrbwalkerInit();
                LuxMenu.MenuInit();
            }

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;

        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender is AIHeroClient && Spells.W.IsReady() && sender.IsEnemy && !spell.SData.IsAutoAttack()
                && !sender.IsDead && sender.IsValidTarget(1000))
            {
                foreach (var ally in from ally in HeroManager.Allies.Where(x=> x.IsAlly && SpellDatabase.Spells.Any(y=> y.spellName == spell.SData.Name))
                                     .Where(ally => ally.Distance(ObjectManager.Player) < Spells.W.Range) 
                                     let exist = SpellDatabase.Spells.FirstOrDefault(y => y.spellName == spell.SData.Name) where exist != null && 
                                     (exist.spellType == SpellType.Cone || exist.spellType == SpellType.Circular || exist.spellType == SpellType.Line)
                                     && sender.GetSpellDamage(ally,exist.spellName) > ally.Health
                                     && LuxMenu.Config.Item("hero." + exist.spellName).GetValue<bool>()
                                     select ally)
                {
                    Spells.W.Cast(ally.Position);
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            Helper.OnDelete(sender,args);
        }
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            Helper.OnCreate(sender,args);
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (LuxMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.q.hit.two.enemy") && Spells.Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    AutoSpells.AutoQIfHit2Target(enemy);
                }
            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.q.if.enemy.immobile") && Spells.Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    AutoSpells.AutoQIfEnemyImmobile(enemy);
                }

            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.q.if.enemy.killable") && Spells.Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    AutoSpells.KillStealWithQ(enemy);
                }
            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.e.hit.x.enemy") && Spells.E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && !x.IsDead && !x.IsZombie))
                {
                   AutoSpells.AutoEIfHitXTarget(enemy);
                }
            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.e.if.enemy.killable") && Spells.E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && !x.IsDead && !x.IsZombie))
                {
                    AutoSpells.KillStealWithE(enemy);
                }
            }
            if (!ObjectManager.Player.IsDead && Helper.Enabled("auto.r.if.enemy.killable.r") && Spells.R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Helper.Slider("auto.r.min.distance.x")) && !x.IsDead && !x.IsZombie))
                {
                    AutoSpells.AutoRIfEnemyKillable(enemy);
                }
            }
            if (LuxMenu.Config.Item("manual.r").GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.R.Range)))
                {
                    if (Spells.R.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Spells.R.Cast(enemy);
                    }
                }
            }

        }
        private static void Combo()
        {
            if (Spells.Q.IsReady() && Helper.Enabled("q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    QLogic.QGeneral(enemy);
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && !x.IsDead && !x.IsZombie))
                {
                    ELogic.NormalE(enemy);
                }
            }
            if (Spells.R.IsReady() && Helper.Enabled("r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Helper.Slider("min.r.distance.y")) && !x.IsDead && !x.IsZombie))
                {
                    RLogic.RGeneral(enemy);
                }
            }
        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("harass.mana"))
            {
                return;
            }
            if (Spells.Q.IsReady() && Helper.Enabled("q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    QLogic.QGeneral(enemy);
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && !x.IsDead && !x.IsZombie))
                {
                    ELogic.NormalE(enemy);
                }
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("clear.mana"))
            {
                return;
            }
            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (minion.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Helper.Enabled("q.clear"))
            {
                if (Spells.Q.CanCast(minion[0]))
                {
                    if (minion[0].Health < Calculators.Q(minion[0]) && minion[1].Health < Calculators.Q(minion[1]) ||
                        minion[0].Health < Calculators.Q(minion[0]) && minion[1].Health > Calculators.Q(minion[1]))
                    {
                        Spells.Q.Cast(Spells.Q.GetPrediction(minion[0]).CastPosition);
                    }
                    if (minion[1].Health < Calculators.Q(minion[1]) && minion[0].Health > Calculators.Q(minion[0]))
                    {
                        Spells.Q.Cast(Spells.Q.GetPrediction(minion[1]).CastPosition);
                    }
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.clear"))
            {
                if (Spells.E.CanCast(minion[0]) && Spells.E.GetCircularFarmLocation(minion).MinionsHit >= Helper.Slider("e.minion.hit.count"))
                {
                    Spells.E.Cast(Spells.E.GetCircularFarmLocation(minion).Position);
                }
                if (Helper.LuxE != null && Helper.EInsCheck() == 2)
                {
                    Spells.E.Cast();
                }
            }
        }
        private static void OnDraw(EventArgs args)
        {
            Drawings.Init();
        }
    }
}
