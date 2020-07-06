using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using AW.Visual.ColorTheme;
using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual
{
    public partial class AWWindow : Window
    {
        public AWWindow(FrameworkElement content)
        {
            InitializeComponent();

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - SystemParameters.WindowResizeBorderThickness.Top - SystemParameters.WindowResizeBorderThickness.Bottom + 1;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.WindowResizeBorderThickness.Left - SystemParameters.WindowResizeBorderThickness.Right + 1;

            StateChanged -= WindowStateChanged;
            StateChanged += WindowStateChanged;

            if (content != null)
                ContentControl.Content = content;

            WindowStateChanged(null, EventArgs.Empty);
        }

        public AWWindow(FrameworkElement content, FrameworkElement toolbar) : this(content)
        {
            if (toolbar != null)
            {
                Toolbar.Children.RemoveAt(0);
                Toolbar.Children.Add(toolbar);
            }
        }


        public AWWindow(FrameworkElement content, IEnumerable<IContextMenuAction> topMenu) : this(content, new TopMenuControl
        {
            DataContext = new TopMenuContext(topMenu)
        })
        { }


        public bool ShowMinimized
        {
            get => Min.Visibility == Visibility.Visible;
            set => Min.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowRestore
        {
            get => Restore.Visibility == Visibility.Visible;
            set => Restore.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowClose
        {
            get => CloseBtn.Visibility == Visibility.Visible;
            set => CloseBtn.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            RestoreIcon.Kind = WindowState == WindowState.Maximized ? PackIconKind.WindowRestore : PackIconKind.WindowMaximize;

            Toolbar.Margin = WindowState == WindowState.Maximized
                ? new Thickness(SystemParameters.WindowResizeBorderThickness.Left, SystemParameters.WindowResizeBorderThickness.Top, SystemParameters.WindowResizeBorderThickness.Right, 0)
                : new Thickness(0);
        }

        private void MinClick(object sender, RoutedEventArgs e)
            => WindowState = WindowState = WindowState.Minimized;

        private void RestoreClick(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseClick(object sender, RoutedEventArgs e)
            => Close();

        public void ShowAlert(string message)
            => DialogHelper.ShowAlert(Container, message);

        public void ShowWait(string message)
            => DialogHelper.ShowWait(Container, message);
        
        public void ShowAlert(FrameworkElement view)
            => DialogHelper.ShowView(Container, view);

        public static void ChangeTheme(bool isDark, Color color)
        {
            IBaseTheme baseTheme = AWTheme.Light;
            if (isDark)
                baseTheme = AWTheme.Dark;

            ITheme theme = Theme.Create(baseTheme, color, color);
            new PaletteHelper().SetTheme(theme);
        }

        public static string History { get; set; } = "History";
        public static string RenameTitle { get; set; } = "Rename";
        public static string NewNameTitle { get; set; } = "New name";
        public static string AddTitle { get; set; } = "Add";
        public static string RemoveTitle { get; set; } = "Remove";
        public static string OkTitle { get; set; } = "Ok";
        public static string CancelTitle { get; set; } = "Cancel";
    }
}
