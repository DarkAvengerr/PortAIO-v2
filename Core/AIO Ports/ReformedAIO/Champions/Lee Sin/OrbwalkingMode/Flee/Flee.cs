using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Flee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal sealed class Flee : OrbwalkingChild
    {
        public override string Name { get; set; } = "Flee";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        public Flee(QSpell qSpell, WSpell wSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
        }

       
        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            FleeJungle();
        }

        private void FleeJungle()
        {
            foreach (var pos in JunglePos)
            {
                qSpell.Spell.Cast(pos);
            }

            var mobs = MinionManager.GetMinions(1000f, MinionTypes.All, MinionTeam.NotAlly).LastOrDefault();

            if (mobs?.Distance(Game.CursorPos) < 700
                && mobs.Distance(ObjectManager.Player) > 400
                && mobs.Health > qSpell.GetDamage(mobs))
            {
                qSpell.Spell.Cast(mobs.Position);
            }

            if (!wSpell.Spell.IsReady()
                || !wSpell.W1 
                || mobs != null 
                || JunglePos.Any())
            {
                return;
            }

            var position = ObjectManager.Player.Position + (Game.CursorPos - ObjectManager.Player.Position).Normalized() * 600;

            wSpell.Jump(position,
                Menu.Item("LeeSin.Flee.W.Minions").GetValue<bool>(),
                Menu.Item("LeeSin.Flee.W.Allies").GetValue<bool>(),
                false);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;

            Drawing.OnEndScene -= OnEndScene;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;

            Drawing.OnEndScene += OnEndScene;
        }

        private void OnEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !Menu.Item("LeeSin.Flee.Draw").GetValue<bool>())
            {
                return;
            }

            foreach (var pos in JunglePos.Where(x => x.Distance(ObjectManager.Player.Position) < 1500))
            {
                Render.Circle.DrawCircle(pos, 50, Color.Cyan);
            }

            Render.Circle.DrawCircle(Game.CursorPos, 600, Color.White);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Flee.Draw", "Draw Vector Positions").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Flee.W.Minions", "Jump To: Minions").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Flee.W.Allies", "Jump To: Allies").SetValue(true));
        }

        public static readonly IEnumerable<Vector3> JunglePos = new List<Vector3>
                                                             {
                                                                 new Vector3(6271.479f, 12181.25f, 56.47668f),
                                                                 new Vector3(6971.269f, 10839.12f, 55.2f),
                                                                 new Vector3(8006.336f, 9517.511f, 52.31763f),
                                                                 new Vector3(10995.34f, 8408.401f, 61.61731f),
                                                                 new Vector3(10895.08f, 7045.215f, 51.72278f),
                                                                 new Vector3(12665.45f, 6466.962f, 51.70544f),

                                                                 // pos of baron
                                                                 new Vector3(5048f, 10460f, -71.2406f),
                                                                 new Vector3(39000.529f, 7901.832f, 51.84973f),
                                                                 new Vector3(2106.111f, 8388.643f, 51.77686f),
                                                                 new Vector3(3753.737f, 6454.71f, 52.46301f),
                                                                 new Vector3(6776.247f, 5542.872f, 55.27625f),
                                                                 new Vector3(7811.688f, 4152.602f, 53.79456f),
                                                                 new Vector3(8528.921f, 2822.875f, 50.92188f),

                                                                 // pos of dragon
                                                                 new Vector3(9802f, 4366f, -71.2406f),
                                                                 new Vector3(3926f, 7918f, 51.74162f)
                                                             }.Where(x => x.Distance(ObjectManager.Player.Position) < 1200 && x.Distance(Game.CursorPos) < 600);

    }
}
