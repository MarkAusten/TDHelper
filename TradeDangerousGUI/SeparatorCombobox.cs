/****************************************************************************************************************
(C) Copyright 2007 Zuoliu Ding.  All Rights Reserved.
SeparatorComboBox:	Implementation class
Created by:			05/15/2004, Zuoliu Ding
Note:				For a Combo box with Separators
****************************************************************************************************************/

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SeparatorComboBox
{
    public class SeparatorComboBox : ComboBox
    {
        private ArrayList _separators;

        public SeparatorComboBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            SeparatorStyle = DashStyle.Solid;
            _separators = new ArrayList();

            SeparatorStyle = DashStyle.Solid;
            SeparatorColor = Color.Black;
            SeparatorMargin = 1;
            SeparatorWidth = 1;
            AutoAdjustItemHeight = false;
        }

        [Description("Gets or sets Auto Adjust Item Height"), Category("Separator")]
        public bool AutoAdjustItemHeight { get; set; }

        [Description("Gets or sets the Separator Color"), Category("Separator")]
        public Color SeparatorColor { get; set; }

        [Description("Gets or sets the Separator Margin"), Category("Separator")]
        public int SeparatorMargin { get; set; }

        [Description("Gets or sets the Separator Style"), Category("Separator")]
        public DashStyle SeparatorStyle { get; set; }

        [Description("Gets or sets the Separator Width"), Category("Separator")]
        public int SeparatorWidth { get; set; }

        public void Add(object item)
        {
            Items.Add(item);
        }

        public void AddWithSeparator(object item)
        {
            Items.Add(item);
            _separators.Add(item.ToString());
        }

        public void SetSeparator(int pos)
        {
            _separators.Add(pos);
        }

        public void ClearSeparators()
        {
            _separators.Clear();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (-1 == e.Index) return;

            bool sep = false;
            object o;
            for (int i = 0; !sep && i < _separators.Count; i++)
            {
                o = _separators[i];

                if (o is string)
                {
                    sep = Items[e.Index].ToString() == o as string;
                }
                else
                {
                    int pos = (int)o;

                    if (pos < 0)
                    {
                        pos += Items.Count;
                    }

                    sep = e.Index == pos;
                }
            }

            e.DrawBackground();
            Graphics g = e.Graphics;
            int y = e.Bounds.Location.Y + SeparatorWidth - 1;

            if (sep)
            {
                Pen pen = new Pen(SeparatorColor, SeparatorWidth);
                pen.DashStyle = SeparatorStyle;

                g.DrawLine(pen, e.Bounds.Location.X + SeparatorMargin, y, e.Bounds.Location.X + e.Bounds.Width - SeparatorMargin, y);
                y++;
            }

            Brush br 
                = DrawItemState.Selected == (DrawItemState.Selected & e.State) 
                ? SystemBrushes.HighlightText 
                : new SolidBrush(e.ForeColor);

            g.DrawString(Items[e.Index].ToString(), e.Font, br, e.Bounds.Left, y + 1);
            //			e.DrawFocusRectangle();

            base.OnDrawItem(e);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            if (AutoAdjustItemHeight)
            {
                e.ItemHeight += SeparatorWidth;
            }

            base.OnMeasureItem(e);
        }
    }
}