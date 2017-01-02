using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SharpShooter.Plugins
{
    public class Sivir
    {
        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        private static Spell _r;

        public Sivir()
        {
            _q = new Spell(SpellSlot.Q, 1250f) {MinHitChance = HitChance.High};
            _w = new Spell(SpellSlot.W);
            _e = new Spell(SpellSlot.E);
            _r = new Spell(SpellSlot.R, 1000f);

            _q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            MenuProvider.Champion.Combo.AddUseQ();
            MenuProvider.Champion.Combo.AddUseW();

            MenuProvider.Champion.Harass.AddUseQ();
            MenuProvider.Champion.Harass.AddIfMana(61);

            MenuProvider.Champion.Laneclear.AddUseQ();
            MenuProvider.Champion.Laneclear.AddUseW();
            MenuProvider.Champion.Laneclear.AddIfMana(60);

            MenuProvider.Champion.Jungleclear.AddUseQ();
            MenuProvider.Champion.Jungleclear.AddUseW();
            MenuProvider.Champion.Jungleclear.AddIfMana(20);

            MenuProvider.Champion.Misc.AddItem("Auto Q on immobile Target", true);
            MenuProvider.Champion.Misc.AddItem("Auto E against targeted spells", true);
            MenuProvider.Champion.Misc.AddItem("Use E Humanizer", false);

            MenuProvider.Champion.Drawings.AddDrawQrange(Color.FromArgb(100, Color.DeepSkyBlue), false);
            MenuProvider.Champion.Drawings.AddDrawRrange(Color.FromArgb(100, Color.DeepSkyBlue), false);
            

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += SivirProtector;
            Obj_AI_Base.OnSpellCast += OnSpellCast;


            Console.WriteLine("Sharpshooter: Sivir Loaded.");
            Chat.Print(
                "<font color = \"#00D8FF\"><b>SharpShooter Reworked:</b></font> <font color = \"#FF007F\">Sivir</font> Loaded." +
                " || Fixed by Hikigaya");
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.SData.IsAutoAttack() && args.Target is AIHeroClient && 
                MenuProvider.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (MenuProvider.Champion.Combo.UseW && _w.IsReadyPerfectly())
                {
                    _w.Cast();
                }
            }

            if (args.SData.IsAutoAttack() && args.Target is Obj_AI_Minion &&
                MenuProvider.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (MenuProvider.Champion.Laneclear.UseW && _w.IsReadyPerfectly() 
                    && ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Laneclear.IfMana))
                {
                    var minions = MinionManager.GetMinions(ObjectManager.Player.Position,
                        ObjectManager.Player.AttackRange).FirstOrDefault(o => o.NetworkId == args.Target.NetworkId);
                    if (minions != null)
                    {
                        _w.Cast();
                    }
                }
            }

            if (args.SData.IsAutoAttack() && args.Target is Obj_AI_Minion &&
                 MenuProvider.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (MenuProvider.Champion.Jungleclear.UseW && _w.IsReadyPerfectly()
                    && ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Jungleclear.IfMana))
                {
                    var minions = MinionManager.GetMinions(ObjectManager.Player.Position,
                        ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral)
                        .FirstOrDefault(o => args.Target.NetworkId == o.NetworkId);

                    if (minions != null)
                    {
                        _w.Cast();
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

            if (MenuProvider.Champion.Misc.GetBoolValue("Auto Q on immobile Target") &&
                _q.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);     
                if (target != null)
                {
                    var pred = _q.GetPrediction(target, true);
                    if (pred.Hitchance >= HitChance.Immobile)
                    {
                        _q.Cast(pred.CastPosition);
                    }
                } 
            }
                    
              
        }

        private static void OnCombo()
        {
            if (MenuProvider.Champion.Combo.UseQ && _q.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    var pred = _q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        _q.Cast(pred.CastPosition);
                    }
                }
            }
                
        }

        private static void OnMixed()
        {
            if (MenuProvider.Champion.Harass.UseQ && ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Harass.IfMana)
                && _q.IsReadyPerfectly())
            {
                var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    var pred = _q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        _q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void OnClear()
        {
            if (MenuProvider.Champion.Laneclear.UseQ)
                if (ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Laneclear.IfMana))
                    if (_q.IsReadyPerfectly())
                    {
                        var farmLocation = _q.GetLineFarmLocation(MinionManager.GetMinions(_q.Range));
                        if (farmLocation.MinionsHit >= 4)
                            _q.Cast(farmLocation.Position);
                    }

            if (MenuProvider.Champion.Jungleclear.UseQ)
                if (ObjectManager.Player.IsManaPercentOkay(MenuProvider.Champion.Jungleclear.IfMana))
                    if (_q.IsReadyPerfectly())
                    {
                        var target =
                            MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.Neutral,
                                MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(600));
                        if (target != null)
                            _q.Cast(target);
                    }
        }

        private void SivirProtector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != null && args.Target != null && sender.Type == GameObjectType.AIHeroClient
                && args.Target.IsMe && sender.IsEnemy && MenuProvider.Champion.Misc.GetBoolValue("Auto E against targeted spells") &&
                _e.IsReadyPerfectly() && !args.SData.IsAutoAttack() && !args.SData.Name.Contains("summoner") &&
                !args.SData.Name.Contains("TormentedSoil"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(
                    MenuProvider.Champion.Misc.GetBoolValue("Use E Humanizer")
                    ? new Random().Next(100, 150): 20, () => _e.Cast()
                    );
            }

            if ((sender != null && args.Target != null && sender.Type == GameObjectType.AIHeroClient
                && args.Target.IsMe && sender.IsEnemy && MenuProvider.Champion.Misc.GetBoolValue("Auto E against targeted spells") &&
                _e.IsReadyPerfectly() && !args.SData.IsAutoAttack() && !args.SData.Name.Contains("summoner") &&
                !args.SData.Name.Contains("TormentedSoil")) && args.SData.Name == "BlueCardAttack" 
                || args.SData.Name == "RedCardAttack" || args.SData.Name == "GoldCardAttack")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(
                    MenuProvider.Champion.Misc.GetBoolValue("Use E Humanizer")
                    ? new Random().Next(100, 150)
                    : 20, () => _e.Cast()
                    );
            }

        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                if (MenuProvider.Champion.Drawings.DrawQrange.Active && _q.IsReadyPerfectly())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range,
                        MenuProvider.Champion.Drawings.DrawQrange.Color);
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
                damage += _q.GetDamage(enemy)*1.4f;
            }

            if (_w.IsReadyPerfectly())
            {
                damage += (float) ObjectManager.Player.GetAutoAttackDamage(enemy);
            }

            return damage;
        }
    }
}