//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:43 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.Drawings
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Extensions;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    public sealed class DamageDrawingChild : ChildBase, IDamageDrawing, IComparable<DamageDrawingChild>
    {
        #region Fields

        /// <summary>
        ///     The get damage delegate
        /// </summary>
        private readonly GetDamageDelegate getDamageDelegate;

        /// <summary>
        ///     The line
        /// </summary>
        private Line line;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DamageDrawingChild" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="getDamageDelegate">Custom get damage delegate</param>
        public DamageDrawingChild(string name, GetDamageDelegate getDamageDelegate = null)
        {
            this.getDamageDelegate = getDamageDelegate;

            this.Name = name;
        }

        #endregion

        #endregion

        #region Delegates

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        public delegate float GetDamageDelegate(Obj_AI_Base target);

        #endregion

        #region Public Methods and Operators

        #region IComparable<DamageDrawingChild> Members

        public int CompareTo(DamageDrawingChild other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Methods

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            var colorPicker =
                this.Menu.AddItem(
                    new MenuItem(this.Path + "." + "color", nameof(this.Color)).SetValue(
                        new Circle(true, System.Drawing.Color.FromArgb(this.Color.R, this.Color.G, this.Color.B))));

            colorPicker.DontSave();

            colorPicker.ValueChanged += (o, args) => { this.Color = args.GetNewValue<Circle>().Color.ToSharpDxColor(); };

            this.Color = colorPicker.GetValue<Circle>().Color.ToSharpDxColor();
        }

        #endregion

        #region IDamageDrawing Members

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public Color Color { get; set; }

        /// <summary>
        ///     Gets or sets the estimated amount in one combo.
        /// </summary>
        /// <value>
        ///     The estimated amount in one combo.
        /// </value>
        public int EstimatedAmountInOneCombo { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        public void Draw(Vector2 start, Vector2 end, float width, Device device)
        {
            if (start.Equals(end)) return;

            if ((this.line == null) || this.line.IsDisposed)
            {
                this.line = new Line(device);
                return;
            }

            this.line.Width = width;

            this.line.Begin();
            Console.WriteLine("Begin Drawing");
            this.line.Draw(new[] { start, end }, this.Color);
            Console.WriteLine("Drawd Drawing");
            this.line.End();
            Console.WriteLine("End Drawing Drawing");
        }

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        public float GetDamage(Obj_AI_Base target)
        {
            if (!this.Switch.Enabled) return 0f;

            return this.getDamageDelegate?.Invoke(target) ?? 0f;
        }

        #endregion
    }
}