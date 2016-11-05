using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.Protector
{
    using System;
    using System.Collections.Generic;

    using global::YasuoMedia.CommonEx;
    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.CommonEx.Objects;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SDK = LeagueSharp.SDK;
    using Color = System.Drawing.Color;

    // TODO
    internal class WindWallProtector : FeatureChild<Protector>
    {
        public WindWallProtector(Protector parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Wind Wall";

        private List<GameObject> detectedObjects; 

        public SweepingBladeLogicProvider Provider;

        public WindWall WindWall;

        public SDK.Tracker Tracker;

        //public SafeZoneLogicProvider Provider;

        protected override void OnEnable()
        {
            Events.OnUpdate += this.OnUpdate;
            //GameObject.OnCreate += this.OnCreate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Events.OnUpdate -= this.OnUpdate;
            //GameObject.OnCreate -= this.OnCreate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override void OnInitialize()
        {
            this.Tracker = new SDK.Tracker();
            this.Provider = new SweepingBladeLogicProvider(GlobalVariables.Spells[SpellSlot.E].Range * 2);
            base.OnInitialize();
        }

        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            var whitelist = new Menu("Whitelist", this.Name + "Whitelist");

            foreach (var x in HeroManager.Allies)
            {
                whitelist.AddItem(new MenuItem(whitelist.Name + x.Name, x.Name).SetValue(true));
            }

            var blacklist = new Menu("Blacklist", this.Name + "Blacklist");

            foreach (var x in HeroManager.Enemies)
            {
                blacklist.AddItem(new MenuItem(blacklist.Name + x.Name, x.Name).SetValue(true));
            }

            this.Menu.AddSubMenu(whitelist);
            this.Menu.AddSubMenu(blacklist);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        public void OnUpdate(EventArgs args)
        {
            if (SDK.Tracker.DetectedSkillshots == null || SDK.Tracker.DetectedSkillshots.Count == 0)
            {
                return;
            }
            
            //var endPos = missile.EndPosition;

            //if (missile.StartPosition.Distance(endPos) < missile.SData.CastRange)
            //{
            //    endPos = missile.StartPosition.Extend(missile.EndPosition, missile.SData.CastRange);
            //}

            //var time = 1000 * missile.Position.Distance(endPos) / missile.SData.MissileSpeed - Game.Ping / 2 + missile.SData.CastFrame / 30f;

            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                //TODO: Does not work because SDK is missing too much. Solution: Pull Request or finding an alternative way of doing this
                
                foreach (var ally in HeroManager.Allies)
                {
                    var endPos = skillshot.EndPosition;

                    if (skillshot.StartPosition.Distance(endPos) < skillshot.SData.Range)
                    {
                        endPos = skillshot.StartPosition.Extend(skillshot.EndPosition, skillshot.SData.Range);
                    }

                    int time = (int)(1000 * skillshot.MissilePosition(false).Distance(endPos) / skillshot.SData.MissileSpeed - Game.Ping / 2);

                    if (skillshot.IsAboutToHit(ally, time))
                    {
                        var gapClosePath = this.Provider.GetPath(ally.ServerPosition);

                        if (gapClosePath != null && gapClosePath.PathCost < time)
                        {
                            //Chat.Print("Can gapclose in time to protect ally");
                        }
                        
                        this.WindWall = new WindWall(skillshot.StartPosition, skillshot.SData.Range, skillshot.SData.Radius);

                        if (this.WindWall != null && this.WindWall.AlliesInside.Contains(ally))
                        {
                            this.Execute(this.WindWall.CastPosition);
                        }
                    }
                }
                    
            }
        }

        public void OnDraw(EventArgs args)
        {
            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                Render.Circle.DrawCircle(skillshot.MissilePosition(false).To3D(), 50, Color.White);
                Drawing.DrawCircle(skillshot.MissilePosition(false).To3D(), 50, Color.White);

                Drawing.DrawText(550, 550, Color.AliceBlue, "Missile Position: " + skillshot.MissilePosition(false));
            }
        }

        public void Execute(Vector2 castPosition)
        {
            if (GlobalVariables.Spells[SpellSlot.W].IsReady() 
                && castPosition.IsValid() && castPosition != Vector2.Zero)
            {
                GlobalVariables.Spells[SpellSlot.W].Cast(castPosition);
            }
        }
    }
}