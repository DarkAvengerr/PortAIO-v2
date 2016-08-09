using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kindred___YinYang.Spell_Database;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace Kindred___YinYang
{
    class Helper
    {
        private static readonly AIHeroClient Kindred = ObjectManager.Player;
        public static void AntiRengarOnCreate(GameObject sender, EventArgs args)
        {
            if (Program.Config.Item("anti.rengar").GetValue<bool>() && Program.R.IsReady() && sender.IsEnemy && !sender.IsAlly && !sender.IsDead
                && sender.Name == "Rengar_LeapSound.troy" && Kindred.HealthPercent < Program.Config.Item("hp.percent.for.rengar").GetValue<Slider>().Value)
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1000) && x.ChampionName == "Rengar"))
                {
                    Program.R.Cast();
                }
            }
        }

        public static void SpellBreaker()
        {
            if (Program.Config.Item("katarina.r").GetValue<bool>() && Program.R.IsReady() && Kindred.HealthPercent < Program.Config.Item("hp.percent.for.broke").GetValue<Slider>().Value)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.ChampionName == "Katarina" && x.IsValidTarget(Program.R.Range) && x.HasBuff("katarinarsound") && !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
            if (Program.Config.Item("lucian.r").GetValue<bool>() && Program.R.IsReady() && Kindred.HealthPercent < Program.Config.Item("hp.percent.for.broke").GetValue<Slider>().Value)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.ChampionName == "Lucian" && x.IsValidTarget(Program.R.Range) && x.HasBuff("lucianr") && !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
            if (Program.Config.Item("missfortune.r").GetValue<bool>() && Program.R.IsReady() && Kindred.HealthPercent < Program.Config.Item("hp.percent.for.broke").GetValue<Slider>().Value)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.ChampionName == "MissFortune" && x.IsValidTarget(Program.R.Range) && x.HasBuff("missfortunebulletsound") && !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
        }
        public static void AntiGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 300)
            {
                Program.Q.Cast(gapcloser.End.Extend(ObjectManager.Player.ServerPosition, ObjectManager.Player.Distance(gapcloser.End) + Program.Q.Range));
            }
        }
        public static int AaIndicator(AIHeroClient enemy)
        {
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical, Kindred.TotalAttackDamage);
            var killableAaCount = enemy.Health / aCalculator;
            var totalAa = (int)Math.Ceiling(killableAaCount);
            return totalAa;
        }

        public static void Protector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!Program.R.IsReady() && Kindred.IsDead && Kindred.IsZombie && sender.IsAlly && !sender.IsMe && !Program.Config.Item("protector").GetValue<bool>())
            {
                return;
            }
            if (sender is AIHeroClient && Program.R.IsReady() && sender.IsEnemy && !spell.SData.IsAutoAttack()
                && !sender.IsDead && !sender.IsZombie && sender.IsValidTarget(1000))
            {
                foreach (var protector in SpellDatabase.Spells.Where(x => x.spellName == spell.SData.Name
                    && Program.Config.Item("hero." + x.spellName).GetValue<bool>()))
                {
                    if (protector.spellType == SpellType.Circular && Kindred.Distance(spell.End) <= 200 &&
                        sender.GetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                    if (protector.spellType == SpellType.Cone && Kindred.Distance(spell.End) <= 200 &&
                        sender.GetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                    if (protector.spellType == SpellType.Line && Kindred.Distance(spell.End) <= 200
                        && sender.GetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                }
            }
        }

        public static void ClassicUltimate()
        {
            var minHp = Program.Config.Item("min.hp.for.r").GetValue<Slider>().Value;
            foreach (var ally in HeroManager.Allies.Where(o => o.HealthPercent < minHp && !o.IsRecalling() && !o.IsDead && !o.IsZombie
                && Kindred.Distance(o.Position) < Program.R.Range && !o.InFountain()))
            {
                if (Program.Config.Item("respite." + ally.CharData.BaseSkinName).GetValue<bool>() && Kindred.CountEnemiesInRange(1500) >= 1
                    && ally.CountEnemiesInRange(1500) >= 1)
                {
                    Program.R.Cast();
                }
            }
        }
        public static void AdvancedQ(Spell spell, AIHeroClient unit, int count)
        {
            switch (Program.Config.Item("q.combo.style").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Cast(Game.CursorPos);
                    break;
                case 1:
                    CollisionObjectCheckCast(spell,unit,count);
                    break;
                case 2:
                    CastSafePosition(spell, unit);
                    break;
            }
        }

        public static void CastSafePosition(Spell spell, AIHeroClient hero)
        {
            if (Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(), Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), spell.Range, Orbwalking.GetRealAutoAttackRange(hero)).Count() > 0)
            {
                spell.Cast(
                    Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                        Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), spell.Range,
                        Orbwalking.GetRealAutoAttackRange(hero)).MinOrDefault(i => i.Distance(Game.CursorPos)));
            }
            else
            {
                spell.Cast(ObjectManager.Player.ServerPosition.Extend(hero.ServerPosition, -spell.Range));
            }
        }
        private static void CollisionObjectCheckCast(Spell spell, AIHeroClient unit, int count)
        {
            if (spell.GetPrediction(unit).CollisionObjects.Count <= count && 
                (spell.GetPrediction(unit).CollisionObjects[0].IsChampion() || spell.GetPrediction(unit).CollisionObjects[1].IsChampion()
                || spell.GetPrediction(unit).CollisionObjects[2].IsChampion()))
            {
                spell.Cast(Game.CursorPos);
            }
        }
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient &&
                args.Target.IsValid)
            {
                if (Program.Q.IsReady() && Program.Config.Item("q.combo").GetValue<bool>() &&
                    ObjectManager.Player.Distance(args.Target.Position) < Program.Q.Range &&
                    Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    AdvancedQ(Program.Q, (AIHeroClient)args.Target,3);
                }
            }
        }

        
        
    }
}
