
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Module.CatheterTestTool.Views
{
    public class GridSheetControl : Grid
    {
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(GridSheetControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnBorderBrushChanged));

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(GridSheetControl), new FrameworkPropertyMetadata(1D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnBorderThicknessChanged, CoerceBorderThickness));
        public static readonly DependencyProperty CellSpacingProperty = DependencyProperty.Register(nameof(CellSpacing), typeof(double), typeof(GridSheetControl), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnCellSpacingChanged, CoerceCellSpacing));

        public Brush BorderBrush
        {
            get => this.GetValue(BorderBrushProperty) as Brush;
            set => this.SetValue(BorderBrushProperty, value);
        }

        public double BorderThickness
        {
            get => (double)this.GetValue(BorderThicknessProperty);
            set => this.SetValue(BorderThicknessProperty, value);
        }

        public double CellSpacing
        {
            get => (double)this.GetValue(CellSpacingProperty);
            set => this.SetValue(CellSpacingProperty, value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size size = base.ArrangeOverride(arrangeSize);
            double border = this.BorderThickness;
            double doubleBorder = border * 2D;
            double spacing = this.CellSpacing;
            double halfSpacing = spacing * 0.5D;
            if (border > 0D || spacing > 0D)
            {
                foreach (UIElement child in this.InternalChildren)
                {
                    this.GetChildBounds(child, out double left, out double top, out double width, out double height);

                    left += halfSpacing + border;
                    top += halfSpacing + border;
                    height -= spacing + doubleBorder;
                    width -= spacing + doubleBorder;

                    if (width < 0D)
                    {
                        width = 0D;
                    }

                    if (height < 0D)
                    {
                        height = 0D;
                    }

                    left -= left % 0.5D;
                    top -= top % 0.5D;
                    width -= width % 0.5D;
                    height -= height % 0.5D;

                    child.Arrange(new Rect(left, top, width, height));
                }

                if (border > 0D && this.BorderBrush != null)
                {
                    this.InvalidateVisual();
                }
            }

            return size;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.BorderThickness > 0D && this.BorderBrush != null)
            {
                if (this.CellSpacing == 0D)
                {
                    this.DrawCollapsedBorder(dc);
                }
                else
                {
                    this.DrawSeperatedBorder(dc);
                }
            }
        }

        private void DrawSeperatedBorder(DrawingContext dc)
        {
            double spacing = this.CellSpacing;
            double halfSpacing = spacing * 0.5D;


            #region draw border
            Pen pen = new Pen(this.BorderBrush, this.BorderThickness);
            UIElementCollection children = this.InternalChildren;
            foreach (UIElement child in children)
            {
                this.GetChildBounds(child, out double left, out double top, out double width, out double height);
                left += halfSpacing;
                top += halfSpacing;
                width -= spacing;
                height -= spacing;

                dc.DrawRectangle(null, pen, new Rect(left, top, width, height));
            }
            #endregion
        }

        private void DrawCollapsedBorder(DrawingContext dc)
        {
            RowDefinitionCollection rows = this.RowDefinitions;
            ColumnDefinitionCollection columns = this.ColumnDefinitions;
            int rowCount = rows.Count;
            int columnCount = columns.Count;
            const byte BORDER_LEFT = 0x08;
            const byte BORDER_TOP = 0x04;
            const byte BORDER_RIGHT = 0x02;
            const byte BORDER_BOTTOM = 0x01;
            byte[,] borderState = new byte[rowCount, columnCount];
            int column = columnCount - 1;
            int columnSpan;
            int row = rowCount - 1;
            int rowSpan;
            #region generate main border data
            for (int i = 0; i < rowCount; i++)
            {
                borderState[i, 0] = BORDER_LEFT;
                borderState[i, column] = BORDER_RIGHT;
            }

            for (int i = 0; i < columnCount; i++)
            {
                borderState[0, i] |= BORDER_TOP;
                borderState[row, i] |= BORDER_BOTTOM;
            }
            #endregion

            #region generate child border data
            UIElementCollection children = this.InternalChildren;
            foreach (UIElement child in children)
            {
                this.GetChildLayout(child, out row, out rowSpan, out column, out columnSpan);
                for (int i = 0; i < rowSpan; i++)
                {
                    borderState[row + i, column] |= BORDER_LEFT;
                    borderState[row + i, column + columnSpan - 1] |= BORDER_RIGHT;
                }
                for (int i = 0; i < columnSpan; i++)
                {
                    borderState[row, column + i] |= BORDER_TOP;
                    borderState[row + rowSpan - 1, column + i] |= BORDER_BOTTOM;
                }
            }
            #endregion

            #region draw border
            Pen pen = new Pen(this.BorderBrush, this.BorderThickness);
            double left;
            double top;
            double width, height;


            for (int r = 0; r < rowCount; r++)
            {
                RowDefinition v = rows[r];
                top = v.Offset;
                height = v.ActualHeight;
                for (int c = 0; c < columnCount; c++)
                {
                    byte state = borderState[r, c];

                    ColumnDefinition h = columns[c];
                    left = h.Offset;
                    width = h.ActualWidth;
                    if ((state & BORDER_LEFT) == BORDER_LEFT)
                    {
                        dc.DrawLine(pen, new Point(left, top), new Point(left, top + height));
                    }
                    if ((state & BORDER_TOP) == BORDER_TOP)
                    {
                        dc.DrawLine(pen, new Point(left, top), new Point(left + width, top));
                    }
                    if ((state & BORDER_RIGHT) == BORDER_RIGHT && (c + 1 >= columnCount || (borderState[r, c + 1] & BORDER_LEFT) == 0))
                    {
                        dc.DrawLine(pen, new Point(left + width, top), new Point(left + width, top + height));
                    }
                    if ((state & BORDER_BOTTOM) == BORDER_BOTTOM && (r + 1 >= rowCount || (borderState[r + 1, c] & BORDER_TOP) == 0))
                    {
                        dc.DrawLine(pen, new Point(left, top + height), new Point(left + width, top + height));
                    }
                }

            }
            #endregion
        }

        private void GetChildBounds(UIElement child, out double left, out double top, out double width, out double height)
        {
            ColumnDefinitionCollection columns = this.ColumnDefinitions;
            RowDefinitionCollection rows = this.RowDefinitions;
            int rowCount = rows.Count;

            int row = (int)child.GetValue(Grid.RowProperty);
            if (row >= rowCount)
            {
                row = rowCount - 1;
            }

            int rowSpan = (int)child.GetValue(Grid.RowSpanProperty);
            if (row + rowSpan > rowCount)
            {
                rowSpan = rowCount - row;
            }
            int columnCount = columns.Count;

            int column = (int)child.GetValue(Grid.ColumnProperty);
            if (column >= columnCount)
            {
                column = columnCount - 1;
            }

            int columnSpan = (int)child.GetValue(Grid.ColumnSpanProperty);
            if (column + columnSpan > columnCount)
            {
                columnSpan = columnCount - column;
            }

            left = columns[column].Offset;
            top = rows[row].Offset;
            ColumnDefinition right = columns[column + columnSpan - 1];
            width = right.Offset + right.ActualWidth - left;
            RowDefinition bottom = rows[row + rowSpan - 1];
            height = bottom.Offset + bottom.ActualHeight - top;
            if (width < 0D)
            {
                width = 0D;
            }

            if (height < 0D)
            {
                height = 0D;
            }

        }
        private void GetChildLayout(UIElement child, out int row, out int rowSpan, out int column, out int columnSpan)
        {
            int rowCount = this.RowDefinitions.Count;

            row = (int)child.GetValue(Grid.RowProperty);
            if (row >= rowCount)
            {
                row = rowCount - 1;
            }

            rowSpan = (int)child.GetValue(Grid.RowSpanProperty);
            if (row + rowSpan > rowCount)
            {
                rowSpan = rowCount - row;
            }
            int columnCount = this.ColumnDefinitions.Count;

            column = (int)child.GetValue(Grid.ColumnProperty);
            if (column >= columnCount)
            {
                column = columnCount - 1;
            }

            columnSpan = (int)child.GetValue(Grid.ColumnSpanProperty);
            if (column + columnSpan > columnCount)
            {
                columnSpan = columnCount - column;
            }
        }

        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is UIElement element)
            {
                element.InvalidateVisual();
            }
        }

        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is UIElement element)
            {
                element.InvalidateArrange();
            }
        }

        private static void OnCellSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is UIElement element)
            {
                element.InvalidateArrange();
            }
        }

        private static object CoerceBorderThickness(DependencyObject d, object baseValue)
        {
            if (baseValue is double value)
            {
                return value < 0D || double.IsNaN(value) || double.IsInfinity(value) ? 0D : value;
            }

            return 0D;
        }
        private static object CoerceCellSpacing(DependencyObject d, object baseValue)
        {
            if (baseValue is double value)
            {
                return value < 0D || double.IsNaN(value) || double.IsInfinity(value) ? 0D : value;
            }

            return 0D;
        }
    }
}
