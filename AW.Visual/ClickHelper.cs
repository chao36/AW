using System;
using System.Windows;

namespace AW.Visual
{
    public static class ClickHelper
    {
        public static void LeftDown(FrameworkElement element, Action<object> click)
            => element.MouseLeftButtonDown += (s, e) =>
            {
                click?.Invoke(e.OriginalSource);
            };

        public static void RightDown(FrameworkElement element, Action<object> click)
            => element.MouseRightButtonDown += (s, e) =>
            {
                click?.Invoke(e.OriginalSource);
            };

        public static void LeftClick(FrameworkElement element, Action<object> click)
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
                    click?.Invoke(e.OriginalSource);
            };
        }

        public static void RightClick(FrameworkElement element, Action<object> click)
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
                    click?.Invoke(e.OriginalSource);
            };
        }

        private static bool IsTime(DateTime time)
            => (DateTime.Now - time).TotalSeconds < 0.3;

        private static bool IsPoint(Point p1, Point p2)
            => Math.Abs(p1.X - p2.X) < 5 && Math.Abs(p1.Y - p2.Y) < 5;
    }
}
