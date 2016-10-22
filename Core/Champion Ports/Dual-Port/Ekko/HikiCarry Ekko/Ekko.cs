using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SSHCommon;
using SSHCommon.Activator;
using SPrediction;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Ekko 
{
    public class Ekko : BaseChamp
    {
        public Ekko()
            : base("Ekko")
        {
            
        }

        public override void CreateConfigMenu()
        {
            combo = new Menu("Combo Settings", "Combo Settings");
            {
                combo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
                combo.AddItem(new MenuItem("combo.w", "Use W").SetValue(true));
                combo.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
                combo.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));

                ult = new Menu("R Settings", "rsettings");
                {
                    ult.AddItem(new MenuItem("r.min.hp.combo", "Min. HP Percent for R").SetValue(new Slider(30, 1, 99)));
                    ult.AddItem(new MenuItem("r.enemy.count", "Min. Enemies for R").SetValue(new Slider(3,1,5)));
                    ult.AddItem(new MenuItem("r.method", "R Method").SetValue<StringList>(new StringList(new string[] { "Smart R" }, 0)));
                    combo.AddSubMenu(ult);
                }
                esettings = new Menu("E Settings", "esettings");
                {
                    esettings.AddItem(new MenuItem("e.method", "E Method").SetValue<StringList>(new StringList(new string[] { "Safe Position", "Cursor Position" }, 0)));
                    combo.AddSubMenu(esettings);
                }
                Config.AddSubMenu(combo);
            }
            harass = new Menu("Harass Settings", "Harass Settings");
            {
                harass.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
                harass.AddItem(new MenuItem("harass.mana", "Min. Mana Percent").SetValue(new Slider(50,1,99)));
                Config.AddSubMenu(harass);
            }
            laneclear = new Menu("Clear Settings", "Clear Settings");
            {
                laneclear.AddItem(new MenuItem("clear.q", "Use Q").SetValue(true));
                laneclear.AddItem(new MenuItem("min.minions.for.q", "Min. Minions to use Q ").SetValue<Slider>(new Slider(3, 1, 6)));
                laneclear.AddItem(new MenuItem("clear.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));
                Config.AddSubMenu(laneclear);
            }
            jungle = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungle.AddItem(new MenuItem("jungle.q", "Use Q").SetValue(true));
                jungle.AddItem(new MenuItem("jungle.w", "Use W").SetValue(true));
                jungle.AddItem(new MenuItem("jungle.e", "Use E").SetValue(true));
                jungle.AddItem(new MenuItem("jungle.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));
                Config.AddSubMenu(jungle);
            }
            killsteal = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                killsteal.AddItem(new MenuItem("disable.ks", "Disable KillSteal?").SetValue(false));
                killsteal.AddItem(new MenuItem("ayracx4", "                  KillSteal Settings"));
                killsteal.AddItem(new MenuItem("killsteal.q", "Use Q").SetValue(true));
                killsteal.AddItem(new MenuItem("killsteal.e", "Use E").SetValue(true));
                killsteal.AddItem(new MenuItem("killsteal.r", "Use R").SetValue(true));
                killsteal.AddItem(new MenuItem("min.hp.killsteal", "Min. HP Percent").SetValue(new Slider(50, 1, 99)));
                Config.AddSubMenu(killsteal);
            }
            m_evader = new Evader(out evade, EvadeMethods.EkkoE);
            {

                Config.AddSubMenu(evade);
            }
            protector = new Menu("(R) Protector", "(R) Protector");
            {
                protector.AddItem(new MenuItem("ayracx", "                  Protector Settings"));
                protector.AddItem(new MenuItem("hp.for.protect", "Min. HP Percent").SetValue(new Slider(10, 100, 0)));
                protector.AddItem(new MenuItem("protector", "Disable Protector?").SetValue(true));
                protector.AddItem(new MenuItem("ayrac", "                 Protector Spell List"));
                foreach (var enemy in HeroManager.Enemies)
                {
                    foreach (var skillshot in SSHCommon.Spell_Database.SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                    {
                        protector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                    }
                }
                Config.AddSubMenu(protector);
            }
            activator = new Menu("Activator Settings", "Activator Settings");
            {
                new Smite(TargetSelector.DamageType.Magical, activator);
                new Ignite(TargetSelector.DamageType.Magical, activator);
                Config.AddSubMenu(activator);
            }
            misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("auto.w.immobile", "Auto Cast W If Enemy Immobile").SetValue(true));
                //misc.AddItem(new MenuItem("focus.method", "Focus Method").SetValue<StringList>(new StringList(new string[] { "Normal", "Always Focus 2 Stack Enemy" }, 0)));
                Config.AddSubMenu(misc);
            }
            drawing = new Menu("Draw Settings", "Draw Settings");
            {
                drawing.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Chartreuse)));
                drawing.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Yellow)));
                drawing.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.White)));
                drawing.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.SandyBrown)));
                //drawing.AddItem(new MenuItem("wall.draw", "Wall Jump Draw").SetValue(true));
                Config.AddSubMenu(drawing);
            }
            Config.AddItem(new MenuItem("CRHITCHANCE", "Skillshot Hit Chance").SetValue<StringList>(new StringList(SSHCommon.Utility.HitchanceNameArray, 1)));
            Config.AddToMainMenu();
            DamageIndicator.DamageToUnit = (t) => (float)CalculateComboDamage(t);
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!Spells[R].IsReady() && ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie && sender.IsAlly && !sender.IsMe && !Config.Item("protector").GetValue<bool>())
            {
                return;
            }
            if (sender is AIHeroClient && Spells[R].IsReady() && sender.IsEnemy && !spell.SData.IsAutoAttack()
                && !sender.IsDead && !sender.IsZombie && sender.IsValidTarget(1000))
            {
                foreach (var protector in SSHCommon.Spell_Database.SpellDatabase.Spells.Where(x => x.spellName == spell.SData.Name
                    && Config.Item("hero." + x.spellName).GetValue<bool>()))
                {
                    if (protector.spellType == SSHCommon.Spell_Database.SpellType.Circular && ObjectManager.Player.Distance(spell.End) <= 200 &&
                        sender.GetSpellDamage(ObjectManager.Player, protector.spellName) > ObjectManager.Player.Health)
                    {
                        Spells[R].Cast();
                    }
                    if (protector.spellType == SSHCommon.Spell_Database.SpellType.Cone && ObjectManager.Player.Distance(spell.End) <= 200 &&
                        sender.GetSpellDamage(ObjectManager.Player, protector.spellName) > ObjectManager.Player.Health)
                    {
                        Spells[R].Cast();
                    }
                    if (protector.spellType == SSHCommon.Spell_Database.SpellType.Line && ObjectManager.Player.Distance(spell.End) <= 200
                        && sender.GetSpellDamage(ObjectManager.Player, protector.spellName) > ObjectManager.Player.Health)
                    {
                        Spells[R].Cast();
                    }
                }
            }
        }
        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 950f, TargetSelector.DamageType.Magical);
            Spells[Q].SetSkillshot(0.25f, 60f, 1650f, true, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 1600f, TargetSelector.DamageType.Magical);
            Spells[W].SetSkillshot(3.0f, 425f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 325f);
            Spells[R] = new Spell(SpellSlot.R, 425f);
        }
        private void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
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
            if (!Config.Item("disable.ks").GetValue<bool>())
            {
                Killsteal();
            }
            if (Config.Item("auto.w.immobile").GetValue<bool>())
            {
                Immobile();
            }
        }

        private void Immobile()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells[W].Range) && !x.IsDead && !x.IsZombie && Immobile(x)))
            {
                Spells[W].Cast(enemy.Position);
            }
        }
        public static bool Immobile(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            else
            {
                return false;
            }    
        }
        private void PhaseDiveSettings()
        {
            switch (esettings.Item("e.method").GetValue<StringList>().SelectedIndex)
            {
                case 0: // Safe
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                    {
                        if (LeagueSharp.Common.Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(), LeagueSharp.Common.Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), Spells[E].Range, Orbwalking.GetRealAutoAttackRange(hero)).Count() > 0)
                        {
                            Spells[E].Cast(
                                LeagueSharp.Common.Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                                    LeagueSharp.Common.Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), Spells[E].Range,
                                    Orbwalking.GetRealAutoAttackRange(hero)).MinOrDefault(i => i.Distance(Game.CursorPos)));
                        }
                        else
                        {
                            Spells[E].Cast(ObjectManager.Player.ServerPosition.Extend(hero.ServerPosition, -Spells[E].Range));
                        }
                    }
                    break;
                case 1: // Cursor Position
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                    {
                        Spells[E].Cast(Game.CursorPos);
                        
                    }
                    break;

            }
        }
        private void ChronobreakCast()
        {
            switch (ult.Item("r.method").GetValue<StringList>().SelectedIndex)
            {
                case 0: // smart
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) && !x.UnderTurret(true)))
                    {
                        if (ObjectManager.Player.HealthPercent >= ult.Item("r.min.hp.combo").GetValue<Slider>().Value)
                        {
                            var xx = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == "Ekko");
                            if (xx.Distance(enemy.Position) <= Spells[R].Range - 50 && xx.CountEnemiesInRange(Spells[R].Range - 50) >= ult.Item("r.enemy.count").GetValue<Slider>().Value)
                            {
                                Spells[R].Cast();
                            }
                        }
                    }
                    break;
            }
        }
        private void Killsteal()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            if (ObjectManager.Player.HealthPercent >= killsteal.Item("min.hp.killsteal").GetValue<Slider>().Value)
            {
                if (Spells[Q].IsReady() && killsteal.Item("killsteal.q").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x=> x.Health < CalculateDamageQ(x)))
                    {
                        Spells[Q].SPredictionCast(enemy, HikiChance);
                    }
                }
                if (Spells[E].IsReady() && killsteal.Item("killsteal.e").GetValue<bool>())
                {
                    if (ObjectManager.Player.CountEnemiesInRange(1000) == 1)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.Health < CalculateDamageE(x)))
                        {
                            Spells[E].Cast(enemy.Position);
                        }
                    }
                }
                if (Spells[R].IsReady() && killsteal.Item("killsteal.q").GetValue<bool>())
                {
                    if (ObjectManager.Player.HealthPercent >= ult.Item("r.min.hp.combo").GetValue<Slider>().Value)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x=> x.Health < CalculateDamageR(x)))
                        {
                            var xx = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == "Ekko");
                            if (xx.Distance(enemy.Position) <= Spells[R].Range - 50 && xx.CountEnemiesInRange(Spells[R].Range - 50) >= ult.Item("r.enemy.count").GetValue<Slider>().Value)
                            {
                                Spells[R].Cast();
                            }
                        }
                    }
                }
            }
        }
        private void Combo()
        {
            if (Spells[Q].IsReady() && Config.Item("combo.q").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && !x.IsZombie))
                {
                    Spells[Q].SPredictionCast(enemy, HikiChance);
                }
            }
            if (Spells[W].IsReady() && Config.Item("combo.w").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[W].Range) && !x.IsDead && !x.IsZombie))
                {
                    Spells[W].SPredictionCast(enemy, HitChance.Low);
                }
            }
            if (Spells[E].IsReady() && Config.Item("combo.e").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && !x.IsZombie))
                {
                    PhaseDiveSettings();
                }
            }
            if (Spells[R].IsReady() && Config.Item("combo.r").GetValue<bool>())
            {
                ChronobreakCast();
            }
        }
        private void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Spells[Q].IsReady() && Config.Item("harass.q").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && !x.IsZombie))
                {
                    Spells[Q].SPredictionCast(enemy, HikiChance);
                }
            }
        }
        private void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Spells[Q].IsReady() && Config.Item("clear.q").GetValue<bool>())
            {
                var qMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells[Q].Range, MinionTypes.All,MinionTeam.NotAlly);
                var minionLocation = Spells[Q].GetLineFarmLocation(qMinion);
                if (minionLocation.MinionsHit >= Config.Item("min.minions.for.q").GetValue<Slider>().Value)
                {
                    Spells[Q].Cast(minionLocation.Position);
                }
            }
        }
        private void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("jungle.mana").GetValue<Slider>().Value)
            {
                return;
            }
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells[Q].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob.Count > 0)
            {
                if (Config.Item("jungle.q").GetValue<bool>() && Spells[Q].IsReady())
                {
                    Spells[Q].Cast(mob[0].Position);
                }
                if (Config.Item("jungle.w").GetValue<bool>() && Spells[W].IsReady())
                {
                    Spells[W].Cast(mob[0].Position);
                }
                if (Config.Item("jungle.e").GetValue<bool>() && Spells[E].IsReady())
                {
                    Spells[E].Cast(Game.CursorPos);
                }
            }
            

        }
        private void OnDraw(EventArgs args)
        {
            if (Config.Item("q.draw").GetValue<Circle>().Active && Spells[Q].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[Q].Range, Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && Spells[W].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[W].Range, Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && Spells[E].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[E].Range, Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active && Spells[R].IsReady())
            {
                foreach (var ghost in ObjectManager.Get<Obj_AI_Minion>().Where(x=> x.Name == "Ekko"))
                {
                    Render.Circle.DrawCircle(ghost.Position, Spells[R].Range, Config.Item("r.draw").GetValue<Circle>().Color);
                }
            }
        }
    }
}
