using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using AW.Visual.Common;

namespace AW.Visual
{
    public static class DialogHelper
    {
        private static FrameworkElement Dialog { get; set; }
        private static Grid Container { get; set; }

        public static void ShowAlert(Grid container, string message)
            => Show(container, new SimpleDialog(message, false));

        public static void ShowWait(Grid container, string message = "Wait")
            => Show(container, new SimpleDialog(message, true));

        public static void ShowView(Grid container, FrameworkElement view)
            => Show(container, new CustomDialog(view, 0.8, 0.9));

        public static void Hide()
            => Hide(null);

        private static void Show(Grid container, FrameworkElement dialog)
        {
            if (dialog == null)
                return;

            void show()
            {
                Dialog = dialog;
                Container = container;

                Dialog.Opacity = 0;
                Container.Children.Add(Dialog);

                Grid.SetColumn(Dialog, 0);
                Grid.SetRow(Dialog, 0);
                Grid.SetColumnSpan(Dialog, Container.ColumnDefinitions.Count + 1);
                Grid.SetRowSpan(Dialog, Container.RowDefinitions.Count + 1);

                Animate(0, 1, EasingMode.EaseOut);
            }

            if (Dialog != null)
                Hide(show);
            else
                show();
        }

        private static void Hide(Action callback)
        {
            if (Dialog != null)
                Dialog.Opacity = 1;

            Animate(1, 0, EasingMode.EaseOut, () =>
            {
                Container?.Children.Remove(Dialog);
                Dialog = null;

                callback?.Invoke();
            });
        }

        private static void Animate(double from, double to, EasingMode mode, Action complite = null)
        {
            CircleEase easing = new CircleEase
            {
                EasingMode = mode
            };

            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                BeginTime = TimeSpan.FromMilliseconds(100),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = easing
            };

            animation.Completed += (s, a) =>
            {
                if (Dialog != null)
                    Dialog.Opacity = to;

                complite?.Invoke();
            };

            if (Dialog != null)
                Dialog.BeginAnimation(UIElement.OpacityProperty, animation);
            else
                complite?.Invoke();
        }

    }
}
