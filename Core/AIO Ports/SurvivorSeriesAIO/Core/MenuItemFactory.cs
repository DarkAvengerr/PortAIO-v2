// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemFactory.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public class MenuItemFactory
    {
        public MenuItemFactory(Menu parent, string name)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Parent = parent;
            Name = name;
            IsStored = true;
        }

        public MenuItemFactory(Menu parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            Parent = parent;
            IsStored = true;
        }

        private FontWrapper Font { get; set; }

        private bool IsShared { get; set; }

        private bool IsStored { get; set; }

        private string Name { get; set; }

        private Menu Parent { get; set; }

        private string Prefix { get; set; }

        private TooltipWrapper Tooltip { get; set; }

        private object Value { get; set; }

        private string PermaName { get; set; }

        public static MenuItemFactory Create(Menu parent, string name)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return new MenuItemFactory(parent, name);
        }

        public static MenuItemFactory Create(Menu parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            return new MenuItemFactory(parent);
        }

        public MenuItem Build()
        {
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentException($"{nameof(Name)} is invalid - null or empty");

            try
            {
                var item = new MenuItem(Name, Name, true);

                if (Prefix != null)
                    item.DisplayName = $"{Prefix}{Name}";

                if (Value != null)
                    item.SetValue(Value);

                if (PermaName != null)
                    item.Permashow(true, PermaName);

                if (Tooltip != null)
                    item.SetTooltip(Tooltip.Text, Tooltip.Color);

                if (Font != null)
                    item.SetFontStyle(Font.Style, Font.Color);

                if (IsShared)
                    item.SetShared();

                if (!IsStored)
                    item.DontSave();

                Parent.AddItem(item);
                return item;
            }
            finally
            {
                Name = null;
                Prefix = null;
                Value = null;
                PermaName = null;
                Tooltip = null;
                Font = null;
                IsShared = false;
                IsStored = true;
            }
        }

        public MenuItemFactory WithFont(FontStyle style, Color? color = null)
        {
            Font = new FontWrapper(style, color);
            return this;
        }

        public MenuItemFactory WithName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            return this;
        }

        public MenuItemFactory WithParent(Menu parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            Parent = parent;
            return this;
        }

        public MenuItemFactory WithPrefix(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Prefix = name;
            return this;
        }

        public MenuItemFactory WithShared(bool state = true)
        {
            IsShared = state;
            return this;
        }

        public MenuItemFactory WithStored(bool state = true)
        {
            IsStored = state;
            return this;
        }

        public MenuItemFactory WithPerma(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            PermaName = name;
            return this;
        }

        public MenuItemFactory WithTooltip(string text, Color? color = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            Tooltip = new TooltipWrapper(text, color);
            return this;
        }

        public MenuItemFactory WithValue<T>(T value) where T : struct
        {
            Value = value;
            return this;
        }

        public class FontWrapper
        {
            public FontWrapper(FontStyle style, Color? color = null)
            {
                Style = style;
                Color = color;
            }

            public Color? Color { get; }

            public FontStyle Style { get; }
        }

        public class TooltipWrapper
        {
            public TooltipWrapper(string text, Color? color = null)
            {
                if (text == null)
                    throw new ArgumentNullException(nameof(text));

                Text = text;
                Color = color;
            }

            public Color? Color { get; }

            public string Text { get; }
        }
    }
}