using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WarwickII.Common;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal class DangerousSpells
    {
        public string SpellName { get; private set; }
        public string ChampionName { get; private set; }
        public SpellSlot SpellSlot { get; private set; }
        public SkillType Type{ get; private set; }

        public enum SkillType
        {
            Target,
            Zone
        }

        public DangerousSpells(string spellName, string championName, SpellSlot spellSlot, SkillType type)
        {
            SpellName = spellName;
            ChampionName = championName;
            SpellSlot = spellSlot;
            Type = type;
        }
    }

    internal static class ModeJump
    {
        public static Menu MenuLocal { get; private set; }

        public static Menu MenuQJumpBlocker { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell E => Champion.PlayerSpells.E;

        private static Obj_AI_Base JumpObject;
        private static AIHeroClient JumpTarget;

        public static List<DangerousSpells> DangerousSpells = new List<DangerousSpells>();

        private static void InitDangerousSpells()
        {
            DangerousSpells.Add(new DangerousSpells("malzaharR", "malzahar", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));
            DangerousSpells.Add(new DangerousSpells("skarnerR", "skarner", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));
            DangerousSpells.Add(new DangerousSpells("warwickR", "warwick", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));

            DangerousSpells.Add(new DangerousSpells("fiddlesticksR", "fiddlesticks", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("pantheonR", "pantheon", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("shenR", "shen", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("twistedfateR", "twistedfate", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
        }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = new Menu("Q Jump Double / Multi", "QDoubleJump");
            {
                //MenuLocal.AddItem(new MenuItem("Jump.Mode", "Jump Mode:").SetValue(new StringList(new[] { "Double Q Jump", "Multi Q Jump [WIP-Not ready]", "Both" }, 1)));

                MenuLocal.AddItem(new MenuItem("Jump.Mode", "Jump Mode:").SetValue(new StringList(new[] { "Off", "Everytime", "If can stun target", "If can kill target", "Can stun + can kill" }, 4)));
                MenuLocal.AddItem(new MenuItem("Jump.ModeDesc1", CommonHelper.Tab + "Tip: You can change Jump Mode with mouse scroll").SetFontStyle(FontStyle.Regular, Color.GreenYellow));
                MenuLocal.AddItem(new MenuItem("Jump.ModeDesc2", CommonHelper.Tab + "Tip: Jump Mode only works on Combo Mode").SetFontStyle(FontStyle.Regular, Color.GreenYellow));
                //MenuLocal.AddItem(new MenuItem("Jump.Multi", "Q Multi Jump:").SetValue(new StringList(new[] { "Off", "If can stun target", "If can kill target", "Can stun + can kill" }, 2)));

                //MenuLocal.AddItem(new MenuItem("Jump.TurretControl", "Jump Under Enemy Turret:").SetValue(new StringList(new[] {"Don't Jump to under enemy turret", "Jump: If can kill target" }, 1)));

                MenuLocal.AddItem(new MenuItem("Jump.Draw.Arrows", "Draw Jump Arrows").SetValue(true));
                MenuLocal.AddItem(new MenuItem("Jump.Draw.Status", "Show Jump Status").SetValue(true));
                MenuLocal.AddItem(new MenuItem("Jump.Recommended", "Load Recommended Settings").SetValue(true)).SetFontStyle(FontStyle.Regular, Color.GreenYellow).SetTooltip("Return to default settings", Color.AliceBlue);
                MenuLocal.AddItem(new MenuItem("Jump.Enabled", "Enabled:").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTooltip("Double Jump to Enemy Active / Passive", Color.AliceBlue).Permashow(true, ObjectManager.Player.ChampionName + " | Double Jump to Enemy", Color.GreenYellow);
                ParentMenu.AddSubMenu(MenuLocal);

                MenuQJumpBlocker = new Menu("Q Jump Blockable Spells", "Block");
                {

                    MenuQJumpBlocker.AddItem(new MenuItem("Jump.Block.Teleport", "Enemy Teleport:")).SetValue(true);

                    foreach (var d in DangerousSpells)
                    {
                        foreach (var t in HeroManager.Enemies.Where(t => string.Equals(JumpTarget.ChampionName, d.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            MenuQJumpBlocker.AddItem(new MenuItem("Jump.Block." + d.ChampionName + d.SpellSlot, JumpTarget.ChampionName + " : " + d.SpellSlot)).SetValue(true);
                        }
                    }

                    ParentMenu.AddSubMenu(MenuQJumpBlocker);
                }
                 
                InitDangerousSpells();

                Game.OnUpdate += GameOnOnUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Game.OnWndProc += Game_OnWndProc;
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a)
            {
                return;
            }
            var newValue = MenuLocal.Item("Jump.Mode").GetValue<StringList>().SelectedIndex + 1;

            if (newValue == MenuLocal.Item("Jump.Mode").GetValue<StringList>().SList.Length)
            {
                newValue = 0;
            }

            MenuLocal.Item("Jump.Mode").SetValue(new StringList(new[] {"Off", "Everytime", "If can stun target", "If can kill target", "Can stun + can kill"}, newValue));
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            JumpTarget = CommonTargetSelector.GetTarget(Q.Range*3, TargetSelector.DamageType.Physical);
            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (!JumpTarget.IsValidTarget())
            {
                return;
            }
            
            if (!JumpObject.IsValidTarget(Q.Range))
            {
                return;
            }

            if (!Q.IsReady())
            {
                return;
            }

            if (!MenuLocal.Item("Jump.Enabled").GetValue<KeyBind>().Active)
            {
                return;
            }
            var jumpMode = MenuLocal.Item("Jump.Mode").GetValue<StringList>().SelectedIndex;
            if (jumpMode != 0)
            {

                switch (jumpMode)
                {
                    case 1:
                    {
                        Q.CastOnUnit(JumpObject);
                        break;
                    }
                    case 2:
                        {
                            if (JumpTarget.CanStun())
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (JumpTarget.Health < CommonMath.GetComboDamage(JumpTarget))
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (JumpTarget.CanStun() || JumpTarget.Health < CommonMath.GetComboDamage(JumpTarget))
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                }

            }
            
            //if (!JumpTarget.IsValidTarget(Q.Range) && !JumpTarget.IsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65))
            //{
            //    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, JumpTarget);
            //}


            return;

            if (JumpTarget.UnderTurret(true) && MenuLocal.Item("Jump.TurretControl").GetValue<StringList>().SelectedIndex == 0)
            {
                return;
            }


            if (JumpTarget.UnderTurret(true) 
                && MenuLocal.Item("Jump.TurretControl").GetValue<StringList>().SelectedIndex == 1 
                && JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
            {
                Q.CastOnUnit(JumpObject);
            }

            var jumpQ = MenuLocal.Item("Jump.TurretControl").GetValue<StringList>().SelectedIndex;

            switch (jumpQ)
            {
                case 0:
                {
                    Q.CastOnUnit(JumpObject);
                    break;
                }

                case 1:
                {
                    if (JumpTarget.CanStun())
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }

                case 2:
                {
                    if (JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }
                case 3:
                {
                    if (JumpTarget.CanStun() && JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (MenuLocal.Item("Jump.Draw.Status").GetValue<bool>())
            {
                var enabled = MenuLocal.Item("Jump.Enabled").GetValue<KeyBind>().Active;
                var stat = MenuLocal.Item("Jump.Mode").GetValue<StringList>().SelectedValue;
                CommonHelper.DrawText(CommonHelper.TextStatus, "Q Jump: " + stat, (int)ObjectManager.Player.HPBarPosition.X + 145, (int)ObjectManager.Player.HPBarPosition.Y + 5, enabled && stat != "Off" ? Color.White : Color.Gray);
            }

            if (!MenuLocal.Item("Jump.Draw.Arrows").GetValue<bool>())
            {
                return;
            }
            if (JumpTarget.IsValidTarget(Q.Range))
            {
                return;
            }

            if (JumpTarget.IsValidTarget() && ObjectManager.Player.Distance(JumpTarget) > Q.Range)
            {
                
                var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(JumpTarget.Position.To2D(), Q.Range * 3), 250).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 1);
                var otherEnemyObjects =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(m => m.IsEnemy && !m.IsDead && !m.IsZombie && m.IsValidTarget(Q.Range) && m.NetworkId != JumpTarget.NetworkId)
                        .Where(m => toPolygon.IsInside(m))
                        .Where(m => ObjectManager.Player.Distance(JumpTarget) > ObjectManager.Player.Distance(m))
                        .Where(m => m.Health < Q.GetDamage(m))
                        .Where(m => !m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165))
                        .OrderBy(m => m.Distance(JumpTarget.Position));

                JumpObject = otherEnemyObjects.FirstOrDefault(m => m.Distance(JumpTarget.Position) <= Q.Range * 2 && m.Distance(JumpTarget.Position) > Orbwalking.GetRealAutoAttackRange(null));

                if (JumpObject != null)
                {
                    if (JumpObject.IsValidTarget(Q.Range))// && JumpTarget.Health <= ComboDamage(t, R.Instance.Ammo - 1 < 0 ? 0: R.Instance.Ammo - 1) && Utils.UltiChargeCount >= 2)
                    {
                        var startpos = ObjectManager.Player.Position;
                        var endpos = JumpObject.Position;
                        var endpos1 = JumpObject.Position + (startpos - endpos).To2D().Normalized().Rotated(30 * (float)Math.PI / 180).To3D() * ObjectManager.Player.BoundingRadius * 2;
                        var endpos2 = JumpObject.Position + (startpos - endpos).To2D().Normalized().Rotated(-30 * (float)Math.PI / 180).To3D() * ObjectManager.Player.BoundingRadius * 2;

                        var width = 1;

                        var x = new Geometry.Polygon.Line(startpos, endpos); x.Draw(System.Drawing.Color.Blue, width);
                        var y = new Geometry.Polygon.Line(endpos, endpos1); y.Draw(System.Drawing.Color.Blue, width + 1);
                        var z = new Geometry.Polygon.Line(endpos, endpos2); z.Draw(System.Drawing.Color.Blue, width + 1);

                        Vector3[] objectCenter = new[] { ObjectManager.Player.Position, JumpObject.Position };
                        var aX = Drawing.WorldToScreen(new Vector3(Common.CommonHelper.CenterOfVectors(objectCenter).X, Common.CommonHelper.CenterOfVectors(objectCenter).Y, Common.CommonHelper.CenterOfVectors(objectCenter).Z));
                        Drawing.DrawText(aX.X - 15, aX.Y - 15, System.Drawing.Color.White, "1st Jump");

                        /*---------------------------------------------------------------------------------------------------------*/
                        var xStartPos = JumpObject.Position;
                        var xEndPos = JumpTarget.Position;
                        var xEndPos1 = JumpTarget.Position + (xStartPos - xEndPos).To2D().Normalized().Rotated(30 * (float)Math.PI / 180).To3D() * JumpObject.BoundingRadius * 2;
                        var xEndPost2 = JumpTarget.Position + (xStartPos - xEndPos).To2D().Normalized().Rotated(-30 * (float)Math.PI / 180).To3D() * JumpObject.BoundingRadius * 2;

                        var xWidth = 1;

                        var x1 = new Geometry.Polygon.Line(xStartPos, xEndPos); x1.Draw(System.Drawing.Color.IndianRed, xWidth);

                        var y1 = new Geometry.Polygon.Line(xEndPos, xEndPos1); y1.Draw(System.Drawing.Color.IndianRed, xWidth + 1);
                        var z1 = new Geometry.Polygon.Line(xEndPos, xEndPost2); z1.Draw(System.Drawing.Color.IndianRed, xWidth + 1);

                        Vector3[] enemyCenter = new[] { JumpObject.Position, JumpTarget.Position };
                        var bX =
                            Drawing.WorldToScreen(new Vector3(Common.CommonHelper.CenterOfVectors(enemyCenter).X, Common.CommonHelper.CenterOfVectors(enemyCenter).Y,
                                Common.CommonHelper.CenterOfVectors(enemyCenter).Z));
                    }
                }
            }
        }
    }
}
