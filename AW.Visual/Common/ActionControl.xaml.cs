using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Common
{
    public interface IAction
    {
        public string Title { get; }
        public PackIconKind? Icon { get; }

        public double FontSize { get; }
        public double IconSize { get; }

        public Brush ForegroundColor { get; }
        public Brush IconColor { get; }
        public bool UseForegroundColorForIcon { get;}

        public ICommand Command { get; }
    }

    public partial class ActionControl : BaseControl
    {
        public ActionControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is ActionContext context)
            {
                if (context.Icon == null)
                    Icon.Visibility = Visibility.Hidden;

                if (context.ForegroundColor != null)
                    Title.Foreground = context.ForegroundColor;

                if (context.IconColor != null)
                    Icon.Foreground = context.IconColor;
                else if (context.UseForegroundColorForIcon)
                {
                    Binding binding = new Binding(nameof(Foreground))
                    {
                        Source = Title,
                    };
                    Icon.SetBinding(ForegroundProperty, binding);
                }
            }
        }
    }

    public class ActionContext : IAction
    {
        public ActionContext(
            string title, PackIconKind? icon, 
            Action action, Func<bool> canAction = null, 
            double fontSize = 15, double iconSize = 22, 
            Brush foregroundColor = null, Brush iconColor = null,
            bool useForegroundColorForIcon = false
        )
        {
            Title = title;
            Icon = icon;

            IconSize = iconSize;
            FontSize = fontSize;

            ForegroundColor = foregroundColor;
            IconColor = iconColor;

            UseForegroundColorForIcon = useForegroundColorForIcon;

            Command = new SimpleCommand(action, canAction);
        }

        public string Title { get; }
        public PackIconKind? Icon { get; }

        public double FontSize { get; }
        public double IconSize { get; }

        public Brush ForegroundColor { get; }
        public Brush IconColor { get; }
        public bool UseForegroundColorForIcon { get; }

        public ICommand Command { get; }
    }
}
