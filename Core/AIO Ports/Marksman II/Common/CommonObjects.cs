using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Common
{
    public class CommonObjects
    {
        public static List<ObjectDatabase> ObjectDatabase = new List<ObjectDatabase>();

        private readonly Dictionary<string, ObjectDatabase> objects = new Dictionary<string, ObjectDatabase>
        {
            { "nunu_base_r_indicator_blue.troy", new ObjectDatabase("nunu", "R", 3f) },
            { "nunu_base_r_indicator_red.troy", new ObjectDatabase("nunu", "R", 3f) },

            { "akali_base_smoke_bomb_tar_team_green.troy", new ObjectDatabase("akali", "W", 8f) },
            { "akali_base_smoke_bomb_tar_team_red.troy", new ObjectDatabase("akali", "W", 8f) },

            //{ "bard_base_e_door.troy", new ObjectDatabase("bard", "E", 10f, Common.ObjectDatabase.DrawingsType.Box) },
            //{ "bard_base_r_stasis_skin_full.troy", new ObjectDatabase("bard", "R", 2.5f) },

            { "eggtimer.troy", new ObjectDatabase("anivia", "Passive", 6f, Common.ObjectDatabase.DrawingsType.Box) },
            { "galio_talion_channel.troy", new ObjectDatabase("galio", "R", 2f) },

            { "infiniteduress_tar.troy", new ObjectDatabase("warwick", "R", 1.8f, Common.ObjectDatabase.DrawingsType.Box) },
            { "karma_base_q_unit_tar.troy", new ObjectDatabase("karthus", "R", 1.5f) },
            { "karma_base_q_impact_R_01.troy", new ObjectDatabase("karthus", "R", 1.5f) },
            { "karma_base_q_impact_R_02.troy", new ObjectDatabase("karthus", "R", 1.5f) },
            { "karthus_base_r_cas.troy", new ObjectDatabase("karthus", "R", 3f) },

            { "leblanc_base_rw_return_indicator.troy", new ObjectDatabase("leblanc", "R W", 4f, Common.ObjectDatabase.DrawingsType.Box) },
            { "leblanc_base_w_return_indicator.troy", new ObjectDatabase("leblanc", "W", 4f, Common.ObjectDatabase.DrawingsType.Box) },
            { "lissandra_base_r_iceblock.troy", new ObjectDatabase("lissandra", "R", 2.5f) },
            { "lissandra_base_r_ring_green.troy", new ObjectDatabase("lissandra", "R", 1.5f) },
            { "lissandra_base_r_ring_red.troy", new ObjectDatabase("lissandra", "R", 1.5f) },
            { "malzahar_base_r_tar.troy", new ObjectDatabase("malzahar", "R", 3f) },
            { "maokai_base_r_aura.troy", new ObjectDatabase("maokai", "R", 10f) },
            { "nickoftime_tar.troy", new ObjectDatabase("zilean", "R", 5f) },
            { "pantheon_base_r_cas.troy", new ObjectDatabase("pantheon", "R", 2f) },
            { "pantheon_base_r_indicator_green.troy", new ObjectDatabase("pantheon", "R", 4.5f) },
            { "pantheon_base_r_indicator_red.troy", new ObjectDatabase("pantheon", "R", 4.5f) },
            { "passive_death_activate.troy", new ObjectDatabase("aatrox", "Passive", 3f) },
            { "shen_standunited_shield_v2.troy", new ObjectDatabase("shen", "R", 3f, Common.ObjectDatabase.DrawingsType.Box) },

            { "jhin_base_e_trap_indicator_enemy.troy", new ObjectDatabase("jax", "R", 2f, Common.ObjectDatabase.DrawingsType.Box) },
            { "jhin_base_e_trap_indicator.troy", new ObjectDatabase("jhin", "E", 120f, Common.ObjectDatabase.DrawingsType.Box) },


            { "thresh_base_lantern_cas_green.troy", new ObjectDatabase("tresh", "W", 6f) },
            { "thresh_base_lantern_cas_red.troy", new ObjectDatabase("tresh", "W", 6f) },

            { "undyingrage_glow.troy", new ObjectDatabase("tryndamere", "R", 5f, Common.ObjectDatabase.DrawingsType.Box) },

            { "zed_base_r_cloneswap_buf.troy", new ObjectDatabase("zed", "R", 7f, Common.ObjectDatabase.DrawingsType.Box) },
            { "zed_base_w_cloneswap_buf.troy", new ObjectDatabase("zed", "W", 4.5f) },
            { "global_ss_teleport_blue.troy", new ObjectDatabase("teleport", "T", 4f) },
            { "global_ss_teleport_target_blue.troy", new ObjectDatabase("teleport", "T", 4f) },
            { "global_ss_teleport_red.troy", new ObjectDatabase("teleport", "T", 4f) },
            { "global_ss_teleport_target_red.troy", new ObjectDatabase("teleport", "T", 4f) },

            #region TwistedFate
            { "gatemarker_green.troy", new ObjectDatabase("twistedfate", "R", 1.5f, Common.ObjectDatabase.DrawingsType.Circle) },
            { "gatemarker_red.troy", new ObjectDatabase("twistedfate", "R", 1.5f, Common.ObjectDatabase.DrawingsType.Circle) },
            #endregion TwistedFate

            //TWISTED FATE W
            //Zohnyas
            //Alistar
            //Guardiang Angle
        };

        private static Spell ChampionSpell;
        public static readonly List<ObjectDraw> ObjectDrawings = new List<ObjectDraw>();
        public CommonObjects()
        {
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Game.OnUpdate += GameOnOnUpdate;

            ChampionSpell = GetSpell();
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            if ((ObjectManager.Player.ChampionName == "Caitlyn" || ObjectManager.Player.ChampionName == "Jinx") &&
                ChampionSpell.IsReady())
            {
                foreach (
                    var ability in
                        ObjectDrawings.Where(d => d.Object.IsValid && d.Position.IsOnScreen() && d.EndTime > Game.Time))
                {
                    ChampionSpell.Cast(ability.Position);
                }
            }
        }


        private static Spell GetSpell()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Caitlyn":
                    {
                        return new Spell(SpellSlot.W, 900);
                    }
                case "Jinx":
                    {
                        return new Spell(SpellSlot.E, 900);
                    }
                case "Draven":
                    {
                        return new Spell(SpellSlot.E, 900);
                    }
            }
            return null;
        }

        private void DrawingOnOnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }


            //var outline = Menu.Item(Menu.Name + "DrawingOutline").GetValue<bool>();
            //var offsetTop = Menu.Item(Menu.Name + "DrawingOffsetTop").GetValue<Slider>().Value;
            //var offsetLeft = Menu.Item(Menu.Name + "DrawingOffsetLeft").GetValue<Slider>().Value;

            foreach (var ability in ObjectDrawings.Where(d => d.Object.IsValid && d.Position.IsOnScreen() && d.EndTime > Game.Time))
            {
                var position = Drawing.WorldToScreen(ability.Position);
                var time = (ability.EndTime - Game.Time).ToString("0.0");

                CommonGeometry.DrawBox(new Vector2(position.X - 50, position.Y - 50), 100, 8, Color.Transparent, 1, Color.Black);
                var buffTime = ability.EndTime - Game.Time;

                float percent = (Math.Abs(ability.EndTime - ability.StartTime) > float.Epsilon) ? buffTime / (ability.EndTime - ability.StartTime) : 1f;
                //Chat.Print(percent.ToString());
                int n = (buffTime > 0) ? (int)(100 * (1f - percent)) : 100;
                string s = string.Format(buffTime < 1f ? "{0:0.0}" : "{0:0}", buffTime);

                CommonGeometry.DrawBox(new Vector2(position.X - 50 + 1, position.Y - 50 + 1), n, 6, ability.Color, 1, Color.Transparent);
                CommonGeometry.DrawText(CommonGeometry.TextPassive, time, position.X - 50 + 105, +position.Y - 52, SharpDX.Color.AntiqueWhite);

                //if (outline)
                //{
                //    _text.DrawText(null, time, (int)position.X + 1 + offsetLeft, (int)position.Y + 1 + offsetTop, Color.Black);
                //    _text.DrawText(null, time, (int)position.X - 1 + offsetLeft, (int)position.Y - 1 + offsetTop, Color.Black);
                //    _text.DrawText(null, time, (int)position.X + 1 + offsetLeft, (int)position.Y + offsetTop, Color.Black);
                //    _text.DrawText(null, time, (int)position.X - 1 + offsetLeft, (int)position.Y + offsetTop, Color.Black);
                //}

                //_text.DrawText(null, time, (int)position.X + offsetLeft, (int)position.Y + offsetTop, ability.Color);
            }
        }

        private void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || sender.Type != GameObjectType.obj_GeneralParticleEmitter || (!sender.IsMe && !sender.IsAlly && !sender.IsEnemy))
            {
                return;
            }

            ObjectDrawings.RemoveAll(i => i.Object.NetworkId == sender.NetworkId || Game.Time > i.EndTime);
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("zyra") || sender.Name.ToLower().Contains("gate"))
                Console.WriteLine(sender.Name);


            if (!sender.IsValid || sender.Type != GameObjectType.obj_GeneralParticleEmitter)
            {
                return;
            }
            if (sender.IsMe)
            {
                Console.WriteLine(sender.Name);
                //Console.WriteLine(sender.Name);
                //Console.WriteLine(sender.Name);
            }
            ObjectDatabase ability;
            if (objects.TryGetValue(sender.Name.ToLower(), out ability))
            {
                ObjectDrawings.Add(new ObjectDraw
                {
                    Object = sender,
                    StartTime = Game.Time,
                    EndTime = Game.Time + ability.Time,
                    Color = ability.Color
                });
            }
        }
    }

    public class ObjectDatabase
    {
        public enum DrawingsType
        {
            Box,
            Circle
        }
        public ObjectDatabase(string champ, string name, float time, DrawingsType drawingsType = DrawingsType.Circle, System.Drawing.Color color = default(System.Drawing.Color))
        {
            Name = name;
            Champ = champ;
            Time = time;
            Enabled = false;
            DrawType = drawingsType;
            Color = color == default(System.Drawing.Color) ? System.Drawing.Color.White : color;
        }

        public string Name { get; private set; }
        public string Champ { get; private set; }
        public float Time { get; private set; }
        public bool Enabled { get; set; }
        public Color Color { get; set; }
        public DrawingsType DrawType { get; set; }

        public ObjectDatabase() { }
    }

    public class ObjectDraw
    {
        public Color Color { get; set; }
        public Vector3 Position => Object.Position;
        public float Radius => Object.BoundingRadius;
        public GameObject Object { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
    }

}
