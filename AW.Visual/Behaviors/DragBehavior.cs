using System;
using System.Windows;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;

namespace AW.Visual.Behaviors
{
    public interface IDragElementContext
    {
        Action<double, double> SetPosition { get; set; }
        Action<double, double> SetPositionAndUpdate { get; set; }
        Action<double, double> SetPositionAndCapture { get; set; }

        Thickness? Bounds { get; }
        bool CanXChange { get; }
        bool CanYChange { get; }

        void OnStartUpdatePosition();
        void OnUpdatePosition(double X, double y);
        void OnEndUpdatePosition();
    }

    public class DragBehavior : Behavior<UIElement>
    {
        private FrameworkElement Parent;

        private IDragElementContext Element;
        private Thickness? Bounds
        {
            get
            {
                if (Element?.Bounds == null)
                    return null;

                double centerX = AssociatedObject.RenderSize.Width / 2;
                double centerY = AssociatedObject.RenderSize.Height / 2;

                return new Thickness(
                    Element.Bounds.Value.Left - centerX,
                    Element.Bounds.Value.Top - centerY,
                    Element.Bounds.Value.Right - centerX,
                    Element.Bounds.Value.Bottom - centerY
                );
            }
        }
            
        private Point ElementStartPosition;
        private Point MouseStartPosition;

        public readonly TranslateTransform Transform = new TranslateTransform();

        protected override void OnAttached()
        {
            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                AssociatedObject.RenderTransform = Transform;
                Parent = frameworkElement.Parent as FrameworkElement;

                if (frameworkElement.DataContext is IDragElementContext element)
                    SetElement(element);
                else
                    frameworkElement.DataContextChanged += (s, e) =>
                    {
                        if (frameworkElement.DataContext is IDragElementContext element)
                            SetElement(element);
                    };

                AssociatedObject.MouseLeftButtonDown += (s, e) =>
                {
                    MouseStartPosition = e.GetPosition(Parent);
                    Start();
                };

                AssociatedObject.MouseMove += (sender, e) =>
                {
                    if (AssociatedObject.IsMouseCaptured)
                    {
                        Vector diff = e.GetPosition(Parent) - MouseStartPosition;

                        Update(diff.X, diff.Y);
                        Element?.OnUpdatePosition(Transform.X + AssociatedObject.RenderSize.Width / 2, Transform.Y + AssociatedObject.RenderSize.Height / 2);
                    }
                };

                AssociatedObject.MouseLeftButtonUp += (sender, e) =>
                {
                    AssociatedObject.ReleaseMouseCapture();
                    Element?.OnEndUpdatePosition();
                };
            }
        }

        private void SetElement(IDragElementContext element)
        {
            element.SetPosition = SetPosition;
            element.SetPositionAndUpdate = (x, y) =>
            {
                SetPosition(x, y);

                Element?.OnUpdatePosition(x, y);
            };
            element.SetPositionAndCapture = (x, y) =>
            {
                SetPosition(x, y);

                MouseStartPosition = new Point(x, y);
                Start();
                Element?.OnUpdatePosition(x, y);
            };
            Element = element;
        }

        private void SetPosition(double x, double y)
        {
            if (Element.CanXChange)
                Transform.X = x - AssociatedObject.RenderSize.Width / 2;
            if (Element.CanYChange)
                Transform.Y = y - AssociatedObject.RenderSize.Height / 2;
        }

        private void Start()
        {
            ElementStartPosition.X = Transform.X;
            ElementStartPosition.Y = Transform.Y;

            AssociatedObject.CaptureMouse();
            Element?.OnStartUpdatePosition();
        }

        private void Update(double diffX, double diffY)
        {
            double x = ElementStartPosition.X + diffX;
            double y = ElementStartPosition.Y + diffY;

            if (Bounds != null)
            {
                if (Element.CanXChange)
                {
                    if (x < Bounds.Value.Left)
                        x = Bounds.Value.Left;
                    else if (x > Bounds.Value.Right)
                        x = Bounds.Value.Right;
                }
                else 
                    x = 0;

                if (Element.CanYChange)
                {
                    if (y < Bounds.Value.Top)
                        y = Bounds.Value.Top;
                    else if (y > Bounds.Value.Bottom)
                        y = Bounds.Value.Bottom;
                }
                else
                    y = 0;
            }

            Transform.X = x;
            Transform.Y = y;
        }
    }
}
