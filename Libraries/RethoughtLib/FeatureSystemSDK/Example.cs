using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystemSDK
{
    using System;

    using global::RethoughtLib.FeatureSystemSDK.Abstract_Classes;
    using global::RethoughtLib.FeatureSystemSDK.Implementations;

    using LeagueSharp;

    internal class Example
    {
        private void Setup()
        {
            // The root "menu"
            var root = new SuperParent("Root");

            // A normal Menu
            var comboParent = new Parent("Parent");

            // 2 children containing the same logic
            var child = new ExampleChild("Child");
            var child2 = new ExampleChild("Child2");

            /* "Connects" each composition element
             *
             * compositionElement.AddChild(compositionElement)
             *
            */

            root.AddChild(comboParent);

            comboParent.AddChild(child);
            comboParent.AddChild(child2);

            /* Output:
             * Root > Parent           > Child            > Enabled [On/Off]
             *        Enabled [On/Off]   Child2           > Enabled [On/Off]
             *                           Enabled [On/Off]
            */
        }

        private sealed class ExampleChild : ChildBase
        {
            public ExampleChild(string name)
            {
                this.Name = name;

                this.OnInitializeInvoker();
            }

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            public override string Name { get; set; }

            /// <summary>
            ///     Called when [disable].
            /// </summary>
            protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
            {
                Game.OnUpdate -= this.GameOnOnUpdate;
            }

            /// <summary>
            ///     Called when [enable]
            /// </summary>
            protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
            {
                Game.OnUpdate += this.GameOnOnUpdate;
            }

            private void GameOnOnUpdate(EventArgs args)
            {
                // Some Logic
            }

            /// <summary>
            ///     Called when [load].
            /// </summary>
            protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
            {
                // Add things to the menu for example, it will auto-generate you don't need to create one nor to add it to another menu
            }
        }
    }
}
