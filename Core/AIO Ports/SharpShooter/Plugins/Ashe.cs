using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SharpShooter.Plugins
{
    public class Ashe
    {
        private static Spell _q;
        private static Spell _w;
        private static Spell _r;

        public Ashe()
        {
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W, 1200f) {MinHitChance = HitChance.High};
            _r = new Spell(SpellSlot.R, 2500f, TargetSelector.DamageType.Magical) {MinHitChance = HitChance.High};

            _w.SetSkillshot(0.25f, 60f, 1500f, true, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);

            MenuProvider.Champion.Combo.AddUseQ();
            MenuProvider.Champion.Combo.AddUseW();
            MenuProvider.Champion.Combo.AddUseR();

            MenuProvider.Champion.Harass.AddUseW();
            MenuProvider.Champion.Harass.AddIfMana(61);

            MenuProvider.Champion.Laneclear.AddUseW(false);
            MenuProvider.Champion.Laneclear.AddIfMana(60);

            MenuProvider.Champion.Jungleclear.AddUseQ();
            MenuProvider.Champion.Jungleclear.AddUseW();
            MenuProvider.Champion.Jungleclear.AddIfMana(20);

            MenuProvider.Champion.Misc.AddUseAntiGapcloser();
            MenuProvider.Champion.Misc.AddUseInterrupter();
            MenuProvider.Champion.Misc.AddItem("Auto R on immobile targets", true);

            MenuProvider.Champion.Drawings.AddDrawWrange(Color.FromArgb(100, Color.DeepSkyBlue), false);
            MenuProvider.Champion.Drawings.AddDrawRrange(Color.FromArgb(100, Color.DeepSkyBlue), false);
            MenuProvider.Champion.Drawings.AddDamageIndicator(GetComboDamage);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            //Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Console.WriteLine("Sharpshooter: Ashe Loaded.");
            Chat.Print(
                "<font color = \"#00D8FF\"><b>SharpShooter Reworked:</b></font> <font color = \"#FF007F\">Ashe</font> Loaded." +
                " || Rewrited by Hikigaya");
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.IsAutoAttack() && args.Target is AIHeroClient && Orbwalking.InAutoAttackRange((AIHeroClient)args.Target)
                && _q.IsReadyPerfectly())
            {
                switch (MenuProvider.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (MenuProvider.Champion.Combo.UseQ && ObjectManager.Player.HasBuff("asheqcastready"))
                            {
                                _q.Cast();
                            }

                            break;
                        }
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (MenuProvider.Champion.Jungleclear.UseQ && ObjectManager.Player.HasBuff("asheqcastready"))
                            {
                                var minions = MinionManager.GetMinions(ObjectManager.Player.Position,
                                    ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral)
                                    .FirstOrDefault(o => args.Target.NetworkId == o.NetworkId);

                                if (minions != null)
                                {
                                    _q.Cast();
                                }
                            }
                            break;
                        }
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            switch (MenuProvider.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    {
                        OnCombo();
                        break;
                    }
                case Orbwalking.OrbwalkingMode.Mixed:
                    {
                        OnMixed();
                        break;
                    }
                case Orbwalking.OrbwalkingMode.LaneClear:
                    {
                        OnClear();
                        break;
                    }
            }

            if (MenuProvider.Champion.Misc.GetBoolValue("Auto R on immobile targets") && _r.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsImmobileUntil() > target.Distance(ObjectManager.Player.ServerPosition) / _r.Speed)
                {
                    var pred = _r.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        _r.Cast(pred.CastPosition);
                    }
                }
            }
            
        }

        private static void OnCombo()
        {
            if (MenuProvider.Champion.Combo.UseW && _w.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTargetNoCollision(_w);
                if (target.IsValidTarget(_w.Range))
                {
                    _w.Cast(target);
                }
            }

            if (MenuProvider.Champion.Combo.UseR && _r.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    var pred = _r.GetPrediction(target, true);
                    if (pred.Hitchance >= HitChance.High) 
                    {
                        if (target.IsKillableAndValidTarget(_r.GetDamage(target),TargetSelector.DamageType.Magical,
                            _r.Range) && !Orbwalking.InAutoAttackRange(target))
                        {
                            _r.Cast(pred.CastPosition);
                        }

                        if (target.IsValidTarget(600f))
                        {
                            _r.Cast(pred.CastPosition);
                        }

                        if (target.IsImmobileUntil() >
                            target.ServerPosition.Distance(ObjectManager.Player.ServerPosition)/_r.Speed)
                        {
                            _r.Cast(target);
                        }
                    }
                }
            }
        }

        private static void OnMixed()
        {
            if (MenuProvider.Champion.Harass.UseW &&
                ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Harass.IfMana)
                && _w.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTargetNoCollision(_w);
                if (target.IsValidTarget(_w.Range))
                {
                    _w.Cast(target);
                } 
            }
        }

        private static void OnClear()
        {
            if (MenuProvider.Champion.Laneclear.UseW &&
                ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Laneclear.IfMana)
                && _w.IsReadyPerfectly())
            {
                var farmLocation = _w.GetLineFarmLocation(MinionManager.GetMinions(_w.Range));
                if (farmLocation.MinionsHit >= 1)
                {
                    _w.Cast(farmLocation.Position);
                }
            }
               
            if (MenuProvider.Champion.Jungleclear.UseW &&
                ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Jungleclear.IfMana)
                && _w.IsReadyPerfectly())
            {
                var target = MinionManager.GetMinions(600, MinionTypes.All, 
                    MinionTeam.Neutral,MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x=> x.IsValidTarget(_w.Range));

                if (target != null)
                {
                    var pred = _w.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        _w.Cast(target);
                    }
                }
                    
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (MenuProvider.Champion.Misc.UseAntiGapcloser && gapcloser.Sender.ChampionName.ToLowerInvariant() != "masteryi"
                && gapcloser.End.Distance(ObjectManager.Player.Position) <= 200 
                && gapcloser.Sender.IsValidTarget(_r.Range) 
                && _r.IsReadyPerfectly())
            {
                _r.Cast(gapcloser.Sender);
            }
                               
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (MenuProvider.Champion.Misc.UseInterrupter && args.DangerLevel >= Interrupter2.DangerLevel.High
                && sender.IsValidTarget(_r.Range) && _r.IsReadyPerfectly())
            {
                _r.Cast(sender);
            }                
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                if (MenuProvider.Champion.Drawings.DrawWrange.Active && _w.IsReadyPerfectly())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range,
                        MenuProvider.Champion.Drawings.DrawWrange.Color);
                }

                if (MenuProvider.Champion.Drawings.DrawRrange.Active && _r.IsReadyPerfectly())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range,
                        MenuProvider.Champion.Drawings.DrawRrange.Color);
                }
                    
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                damage += (float) ObjectManager.Player.GetAutoAttackDamage(enemy, true);
            }

            if (_q.IsReadyPerfectly())
            {
                damage += _q.GetDamage(enemy);
            }

            if (_w.IsReadyPerfectly())
            {
                damage += _w.GetDamage(enemy);
            }

            if (_r.IsReadyPerfectly())
            {
                damage += _r.GetDamage(enemy);
            }

            return damage;
        }
    }
}