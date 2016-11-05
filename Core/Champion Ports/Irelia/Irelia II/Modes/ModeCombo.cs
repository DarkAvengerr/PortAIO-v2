using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy;

namespace Irelia.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

                
        public static void Init()
        {
            MenuLocal = new Menu("Combo", "Combo").SetFontStyle(FontStyle.Regular, Color.Aqua);
            MenuLocal.AddItem(new MenuItem("Combo.Mode", "Mode:").SetValue(new StringList(new[] { "Q -> E-> W", "Q -> AA -> E -> AA -> W -> AA" }, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor())).SetTooltip("Irelia's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.Q", "Q:").SetValue(new StringList(new[] {"Off", "On"}, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.Q.KillSteal", "Q Kill Steal:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor())).SetTooltip("Irelia's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.W", "W:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, W.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.E", "E:").SetValue(new StringList(new[] { "Off", "On: Everytime", "On: Just for stun" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.R", "R:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));

            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;

            Drawing.OnDraw += DrawingOnOnDraw;
        }


        private static Dictionary<int, int> JumpingObjects = new Dictionary<int, int>();

        private static void GetJumpingObjects()
        {
;
            var t = ObjectManager.Get<Obj_AI_Base>()
                     .OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition))
                     .FirstOrDefault(
                         obj =>
                             !obj.IsAlly && !obj.IsMe && !obj.IsMinion && (obj is Obj_AI_Turret) &&
                              Game.CursorPos.Distance(obj.ServerPosition) <= Q.Range * 8);

            if (t == null)
            {
                return;
            }
            var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), t.Distance(ObjectManager.Player.Position)), (E.Range / 2) + E.Range / 3).ToPolygon();
            toPolygon.Draw(System.Drawing.Color.Red, 1);

            var startPos = ObjectManager.Player.Position + Vector3.Normalize(ObjectManager.Player.Position- t.ServerPosition) * (Q.Range);
            
            for (var i = 1; i < (ObjectManager.Player.Distance(t.Position) / Q.Range) + 1; i++)
            {
                var targetBehind = startPos + Vector3.Normalize(t.ServerPosition - startPos) * i * Q.Range;


                var existsMinion = JumpingObjects[i];
                
                var minions =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.IsAlly && !m.IsDead)
                        .Where(m => toPolygon.IsInside(m) && m.Distance(targetBehind) < Q.Range && m.NetworkId != existsMinion
                        //&& m.Health < Q.GetDamage(m)
                        )
                        .OrderByDescending(m => m.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();

                if (minions != null)
                {
                    var j = JumpingObjects.Find(x => x.Key == minions.NetworkId);
                    if (minions.NetworkId != j.Key && i != j.Value)
                    {
                        JumpingObjects.Remove(existsMinion);
                    }

                    JumpingObjects.Add(minions.NetworkId, i);

                    Render.Circle.DrawCircle(minions.Position, minions.BoundingRadius, System.Drawing.Color.Red);
                }
                Render.Circle.DrawCircle(targetBehind, Q.Range, System.Drawing.Color.Yellow);
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            return;
            GetJumpingObjects();
        
            return;
            //if (!Q.IsReady())
            //{
            //    return;
            //}

            //var SearchRange = Q.Range * 4;
            //var t = CommonTargetSelector.GetTarget(SearchRange);
            //if (!t.IsValidTarget())
            //{
            //    return;
            //}
            //if (t.IsValidTarget(Q.Range))
            //{
            //    return;
            //}
            //var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), SearchRange), (E.Range / 2) + E.Range / 3).ToPolygon();
            //toPolygon.Draw(System.Drawing.Color.Red, 1);

            //var minions =
            //    ObjectManager.Get<Obj_AI_Base>()
            //        .Where(m => !m.IsAlly && !m.IsDead)
            //        .Where(m => toPolygon.IsInside(m) && m.Health < Q.GetDamage(m))
            //        .OrderByDescending(m => m.IsValidTarget(Q.Range + 150))
            //        .FirstOrDefault();

            //if (minions != null)
            //{
            //    Render.Circle.DrawCircle(minions.Position, 115f, System.Drawing.Color.DarkRed);
            //    if (minions.IsValidTarget(Q.Range))
            //    {
            //        Q.CastOnUnit(minions);
            //    }
            //}
            //Render.Circle.DrawCircle(t.Position, E.Range, System.Drawing.Color.GreenYellow);
        }

        private static int BladesSpellCount
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.Name.ToLower() == "ireliatranscendentbladesspell")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
            }
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            if (!W.IsReady() || Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || MenuLocal.Item("Combo.W").GetValue<StringList>().SelectedIndex == 0)
            {
                return;
            }

            if (Common.CommonHelper.ShouldCastSpell((AIHeroClient) args.Target) && args.Target is AIHeroClient)
            {
                W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            
            //foreach (var b in ObjectManager.Player.Buffs)
            //{
            //    Console.WriteLine(b.DisplayName + " : " + b.Count);
            //}
            //Console.WriteLine("-------------------------------------------------");


            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            ExecuteCombo();
        }

        private static void ExecuteCombo()
        {
            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            var t = CommonTargetSelector.GetTarget(R.Range);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.IsValidTarget(Q.Range) && MenuLocal.Item("Combo.Q.KillSteal").GetValue<StringList>().SelectedIndex == 1)
            {
                var enemy = HeroManager.Enemies.Find(e => Q.CanCast(e) && e.Health < Q.GetDamage(e));
                if (enemy != null)
                {
                    Champion.PlayerSpells.CastQCombo(enemy);
                }
            }

            if (t.IsValidTarget(Q.Range) && MenuLocal.Item("Combo.Q").GetValue<StringList>().SelectedIndex == 1 && t.Health < Q.GetDamage(t))
            {

                var closesMinion =
                    MinionManager.GetMinions(Q.Range)
                        .Where(
                            m =>
                                m.Distance(t.Position) < Orbwalking.GetRealAutoAttackRange(null) &&
                                m.Health < Q.GetDamage(m) - 15)
                        .OrderBy(m1 => m1.Distance(t.Position))
                        .FirstOrDefault();

                if (closesMinion != null)
                {
                    Q.CastOnUnit(closesMinion);
                }
                else
                {
                    Champion.PlayerSpells.CastQCombo(t);
                }
            }

            if (t.IsValidTarget(Q.Range) && MenuLocal.Item("Combo.Q").GetValue<StringList>().SelectedIndex == 1 && !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                Champion.PlayerSpells.CastQCombo(t);
            }

            if (t.IsValidTarget(E.Range))
            {
                switch (MenuLocal.Item("Combo.E").GetValue<StringList>().SelectedIndex)
                {
                    case 1:
                    {
                        Champion.PlayerSpells.CastECombo(t);
                        break;
                    }
                    case 2:
                    {
                        if (t.Health > ObjectManager.Player.Health)
                        {
                            Champion.PlayerSpells.CastECombo(t);
                        }
                        break;
                    }
                }
            }

            if (R.IsReady() && MenuLocal.Item("Combo.R").GetValue<StringList>().SelectedIndex == 1 && t.IsValidTarget(R.Range) && BladesSpellCount >= 0)
            {
                if (!t.IsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null)) && t.Health < R.GetDamage(t) * 4)
                {
                    PredictionOutput rPredictionOutput = R.GetPrediction(t);
                    Vector3 castPosition = rPredictionOutput.CastPosition.Extend(ObjectManager.Player.Position, -(ObjectManager.Player.Distance(t.ServerPosition) >= 450 ? 80 : 120));

                    if (rPredictionOutput.Hitchance >=
                        (ObjectManager.Player.Distance(t.ServerPosition) >= R.Range / 2 ? HitChance.VeryHigh : HitChance.High) &&
                        ObjectManager.Player.Distance(castPosition) < R.Range)
                    {
                        R.Cast(castPosition);
                    }
                }

                if (CommonMath.GetComboDamage(t) > t.Health && t.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    R.Cast(t, false, true);
                }

                if (BladesSpellCount > 0 && BladesSpellCount <= 3)
                {
                    var enemy = HeroManager.Enemies.Find(e => e.Health < R.GetDamage(e)*BladesSpellCount && e.IsValidTarget(R.Range));
                    if (enemy == null)
                    {
                        foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(R.Range)))
                        {
                            R.Cast(e, false, true);
                        }
                    }
                    else
                    {
                        R.Cast(enemy, false, true);
                    }
                    
                }
            }
        }
    }
}
