//JhinRShot : R Spell Name
//JhinRShot : R Spell Ammo 
#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;

using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    public class MarksmanSpell
    {
        CollisionTypes[] CollisitonType = new CollisionTypes[] {};

        private Spell Q;
        private Spell W;
        private Spell E;
        private Spell R;
        private static CommonGeometry.Polygon toPolygon;
        
        public enum CollisionTypes
        {
            AllyMinion,
            EnemyMinion,
            Mobs,
            AllyHero,
            EnemyHero,
            AllyChampionObjects,
            EnemyChampionObjects,
            Wall,
            YasuoWall,
        }

        public void Cast(AIHeroClient t)
        {
            if (mSkillshotType == SkillshotType.SkillshotCone)
            if (MNSpellSlot == SpellSlot.Q)
            {
                Spell xSpell = new Spell(MNSpellSlot, mRange, mDamageType)
                {
                    Collision = mCollision,
                    Speed = mSpeed,
                    Width = mWidth
                };

                Q.Cast(t);
            }
        }

        public void Cast(Obj_AI_Base target)
        {
            
        }

        public void Cast(Vector3 position)
        {

        }

        public MarksmanSpell(SpellSlot nSpellSlot, TargetSelector.DamageType nDamageType, float nRange, float nDelay, float nWidth, float nSpeed, SkillshotType nSkillshotType, bool nCollision, CollisionTypes[] nCollisions)
        {
            MNSpellSlot = nSpellSlot;
            mDamageType = nDamageType;
            mRange = nRange;
            mDelay = nDelay;
            mWidth = nWidth;
            mSpeed = nSpeed;
            mCollision = nCollision;
            mCollisions = nCollisions;
        }

        public SpellSlot MNSpellSlot { get; set; }
        public TargetSelector.DamageType mDamageType { get; set; }
        public float mRange { get; set; }
        public float mDelay { get; set; }
        public float mWidth { get; set; }
        public float mSpeed { get; set; }
        public SkillshotType mSkillshotType { get; set; }
        public bool mCollision { get; set; }
        public CollisionTypes[] mCollisions { get; set; }

        public MarksmanSpell() { }
    }

    internal class ObjectDraw
    {
        public Color Color { get; set; }
        public Vector3 Position => Object.Position;
        public float Radius => Object.BoundingRadius;
        public GameObject Object { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
    }


    internal class Jhin : Champion
    {
        private Spell Q { get; set; }
        private static Spell W { get; set; }
        private Spell E { get; set; }
        private Spell R { get; set; }
        private Vector3 direction;
        public MarksmanSpell QSpell { private get; set; }
        public MarksmanSpell WSpell { private get; set; }


        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
                Console.WriteLine(args.SData.Name);

            //JhinRShot : R Spell Ammo 
        }




        public Jhin()
        {
            QSpell = new MarksmanSpell
            {
                MNSpellSlot = SpellSlot.Q,
                mRange = 600f
            };

            WSpell = new MarksmanSpell
            {
                MNSpellSlot = SpellSlot.W,
                mRange = 2500f,
                mDelay = 0.90f,
                mWidth = 30f,
                mSpeed = float.MaxValue,
                mSkillshotType = SkillshotType.SkillshotLine,
                mCollision = true,
                mCollisions =
                    new[]
                    {
                        MarksmanSpell.CollisionTypes.AllyHero,
                        MarksmanSpell.CollisionTypes.YasuoWall,
                        MarksmanSpell.CollisionTypes.EnemyHero
                    },
            };

            Q = new Spell
            {
                Slot = SpellSlot.Q,
                Range = 600,
            };
            
            W = new Spell
            {
                Slot = SpellSlot.W,
                Range = 2500,
                Delay = 1.10f,
                Width = 30,
                Collision = false,
                Speed = float.MaxValue,
                Type = SkillshotType.SkillshotLine
            };
            
            E = new Spell
            {
                Slot = SpellSlot.E,
                Range = 760,
                Delay = 1f,
                Width = 130,
                Speed = 1500,
                Type = SkillshotType.SkillshotCircle,
            };
            
            R = new Spell
            {
                Slot = SpellSlot.R,
                Range = 3500,
                Delay = 0.25f,
                Width = 80,
                Speed = 5500,
                Type = SkillshotType.SkillshotLine
            };

            Utils.Utils.PrintMessage("Jhin");
        }

        public override void GameOnUpdate(EventArgs args)
        {
            //Console.WriteLine(Player.Spellbook.GetSpell(SpellSlot.R).Name + " : " + Player.Spellbook.GetSpell(SpellSlot.R).Ammo);

        }

        private static List<Obj_AI_Base> CollisionObjects(Spell spell, AIHeroClient source, AIHeroClient target, CollisionableObjects[] collisionableObjects)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = spell.Width,
                Delay = spell.Delay,
                Speed = spell.Speed,
                CollisionObjects = collisionableObjects,//{[0] = CollisionableObjects.Heroes, [1] = CollisionableObjects.YasuoWall},
            };
            
            return
                Collision.GetCollision(new List<Vector3> { target.Position }, input).Where(obj => obj.NetworkId != target.NetworkId)
                    .OrderBy(obj => obj.Distance(source, false))
                    .ToList();
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
                direction = ObjectManager.Player.ServerPosition.Extend(ObjectManager.Player.Direction, -1500);
            //var radian = (float)Math.PI / 180f;
            //var targetPosition = ObjectManager.Player.Position;

            ////new Geometry.Polygon.Sector(targetPosition, direction, 40f * radian, 450f).Draw(Color.Red);
            ////new CommonGeometry.Rectangle(targetPosition.To2D(), direction.To2D(), 50).ToPolygon().Draw(Colo
            var x = ObjectManager.Player.Direction.To2D();

            var toPolygon =
                        new CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(),
                            ObjectManager.Player.Position.To2D()
                                .Extend(direction.To2D(), 800), 50).ToPolygon();

            toPolygon.Draw(System.Drawing.Color.Red, 1);



            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }
            var xCol = CollisionObjects(W, ObjectManager.Player, t, new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall });
                Console.WriteLine(xCol.Count.ToString());
            foreach (
               var colminion in
                   CollisionObjects(W, ObjectManager.Player, t, new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }))
            {
                Render.Circle.DrawCircle(colminion.Position, 105f, Color.Yellow);
                Console.WriteLine(colminion.CharData.BaseSkinName);
            }
            return;

            foreach (var buff in ObjectManager.Player.Buffs)
            {
                Console.WriteLine(buff.Name + " : " + buff.Count);
            }
            Console.WriteLine("-------------------");
            return;
            Spell[] spellList = { Q, W, E };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("Combo.Use.Q" + Id, "Q:").SetValue(true));
            config.AddItem(new MenuItem("Combo.Use.W" + Id, "W:").SetValue(true));
            config.AddItem(new MenuItem("Combo.Use.E" + Id, "E:").SetValue(true));
            config.AddItem(new MenuItem("Combo.Use.R" + Id, "R: Semi-Auto").SetValue(true)).SetTooltip("Select a target and Press R");
            return true;
        }

        public override void ExecuteCombo()
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.IsValidTarget(Q.Range) && GetValue<bool>("Combo.Use.Q") && Q.IsReady())
            {
                Q.CastOnUnit(t);
            }

            if (GetValue<bool>("Combo.Use.W") && W.IsReady())
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(W.Range) && CollisionObjects(W, ObjectManager.Player, e, new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }).Count == 0))
                {
                    if (e.Health < W.GetDamage(t) * 0.85)
                    {
                        W.CastIfHitchanceGreaterOrEqual(t);
                    }

                    if (e.HasBuff("jhinespotteddebuff"))
                    {
                        if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Knockup) && !t.CanMove)
                        {
                            W.CastIfHitchanceEquals(t, HitChance.High);
                        }
                        else
                        {
                            W.CastIfHitchanceGreaterOrEqual(t);
                        }
                    }
                }
            }

            //if ( W.CanCast(t) && CollisionObjects(W, ObjectManager.Player, t, new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }).Count > 0)
            //{
            //    if (t.Health < W.GetDamage(t))
            //    {
            //        W.CastIfHitchanceGreaterOrEqual(t);
            //    }

            //    if (t.HasBuff("jhinespotteddebuff"))
            //    {
            //        if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Knockup) && !t.CanMove)
            //        {
            //            W.CastIfHitchanceEquals(t, HitChance.High);
            //        }
            //        else
            //        {
            //            W.CastIfHitchanceGreaterOrEqual(t);
            //        }
            //    }
            //}

            if (GetValue<bool>("Combo.Use.E") && E.CanCast(t))
            {
                if (t.HasBuffOfType(BuffType.Slow))
                {
                    if (t.Path.Count() > 1)
                    {
                        var slowEndTime = t.GetSlowEndTime();
                        if (slowEndTime >= E.Delay + 0.5f + Game.Ping / 2f)
                        {
                            E.CastIfHitchanceGreaterOrEqual(t);
                        }
                    }
                }

                if (t.IsHeavilyImpaired())
                {
                    var immobileEndTime = t.GetImpairedEndTime();
                    if (immobileEndTime >= E.Delay + 0.5f + Game.Ping / 2f)
                    {
                        E.CastIfHitchanceGreaterOrEqual(t);
                    }
                }
            }
        }
        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(false));
            config.AddItem(new MenuItem("UseWH" + Id, "Use W").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawQ" + Id, "Q:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("DrawW" + Id, "W:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("DrawE" + Id, "E:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            return true;
        }
        public override bool JungleClearMenu(Menu menuJungle)
        {
            return false;
        }
    }
}
