using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Common.CommonAllySupport
{
    public static class AllyThreshLantern
    {
        public static Menu MenuLocal { get; private set; }
        private static GameObject Lantern { get; set; }

        public static List<ObjectDatabase> ObjectDatabase = new List<ObjectDatabase>();

        private static readonly List<ObjectDraw> objectDrawings = new List<ObjectDraw>();

        private static readonly Dictionary<string, ObjectDatabase> objects = new Dictionary<string, ObjectDatabase>
        {
            { "thresh_base_lantern_cas_green.troy", new ObjectDatabase("tresh", "W", 6f) },
            { "thresh_base_lantern_cas_red.troy", new ObjectDatabase("tresh", "W", 6f) },
        };


        public static void Init(Menu nParentMenu)
        {
            if (nParentMenu == null)
            {
                return;
            }

            MenuLocal = new Menu("Thresh", "Menu.Thresh");
            {
                MenuLocal.AddItem(new MenuItem("Lantern.Active", "Use").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press, true)));

                var nMenuDraw = new Menu("Drawing", "Lantern.Drawing");
                {
                    nMenuDraw.AddItem(new MenuItem("Lantern.Draw.Enable", "Draw").SetValue(true))
                        .SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow)
                        .ValueChanged += (sender, args) =>
                        {
                            nMenuDraw.Item("Lantern.Draw.Circle").Show(args.GetNewValue<bool>());
                            nMenuDraw.Item("Lantern.Draw.Timer").Show(args.GetNewValue<bool>());
                            nMenuDraw.Item("Lantern.Draw.Direction").Show(args.GetNewValue<bool>());
                        };

                    nMenuDraw.AddItem(new MenuItem("Lantern.Draw.Circle", "Circle:").SetValue(true));
                    nMenuDraw.AddItem(new MenuItem("Lantern.Draw.Timer", "Timer:").SetValue(true));
                    nMenuDraw.AddItem(new MenuItem("Lantern.Draw.Direction", "Direction:").SetValue(true));
                    
                    MenuLocal.AddSubMenu(nMenuDraw);
                }
                

                var useMenuItemName = "Use." + ObjectManager.Player.ChampionName;
                var useMenuItemText = "Use " + ObjectManager.Player.ChampionName;

                if (ChampionSpell != null)
                {
                    MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + ": " + ChampionSpell.Slot.ToString()).SetValue(false));
                }

                //switch (ObjectManager.Player.ChampionName.ToLower())
                //{
                //    case "caitlyn":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + ChampionSpell.Slot.ToString()).SetValue(false));
                //        break;
                //    }

                //    case "corki":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " W").SetValue(false));
                //        break;
                //    }

                //    case "ezreal":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " E").SetValue(false));
                //        break;
                //    }

                //    case "graves":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " E").SetValue(false));
                //        break;
                //    }

                //    case "lucian":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " E").SetValue(false));
                //        break;
                //    }

                //    case "tristana":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " W").SetValue(false));
                //        break;
                //    }

                //    case "vayne":
                //    {
                //        //MenuLocal.AddItem(new MenuItem("Use.Lantern", useMenuItemText + " Q").SetValue(false));
                //        break;
                //    }
                //}


            }
            nParentMenu.AddSubMenu(MenuLocal);
            
            Game.OnUpdate += GameOnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void GameOnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Lantern == null || !Lantern.IsValid || !MenuLocal.Item("Lantern.Active").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (Lantern.Position.Distance(ObjectManager.Player.Position) <= 500)
            {
                ObjectManager.Player.Spellbook.CastSpell((SpellSlot)62, Lantern);
            }
            else if (ChampionSpell != null && ChampionSpell.IsReady() && Lantern.Position.Distance(ObjectManager.Player.Position) <= 500 + ChampionSpell.Range)
            {
                ChampionSpell.Cast(Lantern.Position);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || !sender.IsAlly || sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }

            if (sender.Name.Equals("Lantern", StringComparison.OrdinalIgnoreCase))
            {
                Lantern = sender;
            }
            ObjectDatabase ability;
            if (objects.TryGetValue(sender.Name.ToLower(), out ability))
            {
                objectDrawings.Add(new ObjectDraw
                {
                    Object = sender,
                    StartTime = Game.Time,
                    EndTime = Game.Time + ability.Time,
                    Color = ability.Color
                });
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || Lantern == null)
            {
                return;
            }
            if (sender.NetworkId == Lantern.NetworkId)
            {
                Lantern = null;
            }

            if (!sender.IsValid || sender.Type != GameObjectType.obj_GeneralParticleEmitter || (!sender.IsMe && !sender.IsAlly && !sender.IsEnemy))
            {
                return;
            }

            objectDrawings.RemoveAll(i => i.Object.NetworkId == sender.NetworkId || Game.Time > i.EndTime);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            if (MenuLocal.Item("Lantern.Draw.Enable").GetValue<bool>())
            {
                return;
            }

            foreach (var ability in objectDrawings.Where(d => d.Object.IsValid && d.Position.IsOnScreen() && d.EndTime > Game.Time))
            {
                var position = Drawing.WorldToScreen(ability.Position);

                if (MenuLocal.Item("Lantern.Draw.Timer").GetValue<bool>())
                { 
                    CommonGeometry.DrawBox(new Vector2(position.X - 50, position.Y - 50), 100, 8, Color.Transparent, 1, Color.Black);
                    var buffTime = ability.EndTime - Game.Time;

                    int n = (buffTime > 0)
                        ? (int) (100* (1f - (Math.Abs(ability.EndTime - ability.StartTime) > float.Epsilon ? buffTime/(ability.EndTime - ability.StartTime) : 1f)))
                        : 100;

                    CommonGeometry.DrawBox(new Vector2(position.X - 50 + 1, position.Y - 50 + 1), n, 6, ability.Color, 1,
                        Color.Transparent);

                    CommonGeometry.DrawText(CommonGeometry.TextPassive, (ability.EndTime - Game.Time).ToString("0.0"),
                        position.X - 50 + 105, +position.Y - 52, SharpDX.Color.AntiqueWhite);
                }

                if (MenuLocal.Item("Lantern.Draw.Circle").GetValue<bool>())
                {
                    var circle =
                        new CommonGeometry.Circle2(ability.Position.To2D(), 450,
                            Game.Time*300 - ability.StartTime*300, ability.EndTime*300 - ability.StartTime*300)
                            .ToPolygon();

                    circle.Draw(Color.Red, 2);
                }

                if (MenuLocal.Item("Lantern.Draw.Direction").GetValue<bool>())
                {
                    var color = Color.DarkRed;
                    var width = 4;

                    var startpos = ObjectManager.Player.Position;
                    var endpos = ability.Position;
                    if (startpos.Distance(endpos) > 100)
                    {
                        var endpos1 = ability.Position + (startpos - endpos).To2D().Normalized().Rotated(25*(float) Math.PI/180).To3D()*75;
                        var endpos2 = ability.Position + (startpos - endpos).To2D().Normalized().Rotated(-25*(float) Math.PI/180).To3D()*75;

                        var x1 = new Geometry.Polygon.Line(startpos, endpos);
                        x1.Draw(color, width - 2);

                        var y1 = new Geometry.Polygon.Line(endpos, endpos1);
                        y1.Draw(color, width - 2);

                        var z1 = new Geometry.Polygon.Line(endpos, endpos2);
                        z1.Draw(color, width - 2);
                    }
                }
            }
        }

        private static Spell ChampionSpell
        {
            get
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "caitlyn":
                        return new Spell(SpellSlot.E, 900);

                    case "corki":
                        return new Spell(SpellSlot.W, 600);

                    case "ezreal":
                        return new Spell(SpellSlot.E, 475);

                    case "graves":
                        return new Spell(SpellSlot.E, 425);

                    case "lucian":
                        return new Spell(SpellSlot.E, 425);

                    case "tristana":
                        return new Spell(SpellSlot.W, 900);

                    case "vayne":
                        return new Spell(SpellSlot.Q, 300);
                }
                return null;
            }
        }
    }
}
