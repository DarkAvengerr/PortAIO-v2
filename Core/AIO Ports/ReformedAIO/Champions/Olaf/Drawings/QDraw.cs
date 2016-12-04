using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Olaf.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Olaf.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QDraw : ChildBase
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QDraw(QSpell spell)
        {
            this.spell = spell;
        }

        private GameObject AxeObject { get; set; }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }


            Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Spell.Range, spell.Spell.IsReady() 
                ? Color.Cyan 
                : Color.DarkSlateGray,
                4,
                true);

            if (Menu.Item("Axe").GetValue<bool>() && AxeObject != null)
            {
                Render.Circle.DrawCircle(AxeObject.Position, 120, spell.Spell.IsReady()
                    ? Color.Red 
                    : Color.DarkCyan,
                    4,
                    true);
            }
        }
     
        private void GameObjectOnOnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name.ToLower().Contains("olaf_base_q_axe") && obj.Name.ToLower().Contains("ally"))
            {
                AxeObject = obj;
            }
        }
        private void GameObjectOnOnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Name.ToLower().Contains("olaf_base_q_axe") && obj.Name.ToLower().Contains("ally"))
            {
                AxeObject = null;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            GameObject.OnDelete -= GameObjectOnOnDelete;
            GameObject.OnCreate -= GameObjectOnOnCreate;
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            GameObject.OnDelete += GameObjectOnOnDelete;
            GameObject.OnCreate += GameObjectOnOnCreate;
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Axe", "Axe Circle").SetValue(true));
        }
    }
}
