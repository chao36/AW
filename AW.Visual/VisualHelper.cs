using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Point = System.Windows.Point;

namespace AW.Visual
{
    public enum LimitType
    {
        Int = 0, 
        Double = 1
    }

    public static class VisualHelper
    {
        public static Bitmap GetBitmap(UIElement element)
        {
            var rect = new Rect(element.RenderSize);
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            var bitmapRender = new RenderTargetBitmap((int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmapRender.Render(visual);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapRender));
            encoder.Save(stream);

            return new Bitmap(stream);
        }

        public static void LimitInput(TextBox textBox, LimitType limit)
        {
            textBox.Tag = limit == LimitType.Double
                ? (Func<string, bool>)(v => !double.TryParse(v, out double _))
                : (v => !int.TryParse(v, out int _)); ;

            textBox.PreviewTextInput -= TextBoxPreviewTextInput;
            textBox.PreviewTextInput += TextBoxPreviewTextInput;

            DataObject.RemovePastingHandler(textBox, TextBoxPastingHandler);
            DataObject.AddPastingHandler(textBox, TextBoxPastingHandler);
        }

        private static void TextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is Func<string, bool> canChange)
                e.Handled = !string.IsNullOrWhiteSpace(e.Text)
                    && (e.Text != "-" || textBox.CaretIndex != 0)
                    && canChange(textBox.Text.Insert(textBox.CaretIndex, e.Text));
        }

        private static void TextBoxPastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is Func<string, bool> canChange)
            {
                if (e.DataObject.GetDataPresent(typeof(string)))
                {
                    string text = (string)e.DataObject.GetData(typeof(string));

                    if (canChange(textBox.Text.Insert(textBox.CaretIndex, text)))
                        e.CancelCommand();
                }
                else
                    e.CancelCommand();
            }
        }

        public static void ExitOnEnter(TextBox textBox)
        {
            textBox.PreviewKeyUp -= TextBoxPreviewKeyUp;
            textBox.PreviewKeyUp += TextBoxPreviewKeyUp;
        }

        private static void TextBoxPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && sender is TextBox textBox)
            {
                DependencyObject ancestor = textBox.Parent;
                while (ancestor != null)
                {
                    if (ancestor is UIElement element && element.Focusable)
                    {
                        element.Focus();
                        break;
                    }

                    ancestor = VisualTreeHelper.GetParent(ancestor);
                }
                Keyboard.ClearFocus();

                BindingExpression be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                    be.UpdateSource();
            }
        }

        public static void LeftDown(FrameworkElement element, Action<MouseButtonEventArgs> click)
            => element.MouseLeftButtonDown += (s, e) =>
            {
                click?.Invoke(e);
            };

        public static void RightDown(FrameworkElement element, Action<MouseButtonEventArgs> click)
            => element.MouseRightButtonDown += (s, e) =>
            {
                click?.Invoke(e);
            };

        public static void LeftClick(FrameworkElement element, Action<MouseButtonEventArgs> click)
        {
            DateTime clickDate = new DateTime();
            Point point = new Point();

            element.MouseLeftButtonDown += (s, e) =>
            {
                clickDate = DateTime.Now;
                point = e.GetPosition(element);
            };
            element.MouseLeftButtonUp += (s, e) =>
            {
                if (IsTime(clickDate) && IsPoint(point, e.GetPosition(element)))
                    click?.Invoke(e);
            };
        }

        public static void RightClick(FrameworkElement element, Action<MouseButtonEventArgs> click)
        {
            DateTime clickDate = new DateTime();
            Point point = new Point();

            element.MouseRightButtonDown += (s, e) =>
            {
                clickDate = DateTime.Now;
                point = e.GetPosition(element);
            };
            element.MouseRightButtonUp += (s, e) =>
            {
                if (IsTime(clickDate) && IsPoint(point, e.GetPosition(element)))
                    click?.Invoke(e);
            };
        }

        private static bool IsTime(DateTime time)
            => (DateTime.Now - time).TotalSeconds < 0.3;

        private static bool IsPoint(Point p1, Point p2)
            => Math.Abs(p1.X - p2.X) < 5 && Math.Abs(p1.Y - p2.Y) < 5;
    }
}
