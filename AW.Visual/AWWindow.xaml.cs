using AW.Visual.ColorTheme;

using MaterialDesignThemes.Wpf;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AW.Visual
{
    public partial class AWWindow : Window
    {
        private static bool IsLoadResource = false;

        public AWWindow(FrameworkElement content = null, FrameworkElement toolbar = null)
        {
            if (!IsLoadResource)
            {
                Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/AW.Visual;Component/Resource.xaml", UriKind.Relative)) as ResourceDictionary);
                IsLoadResource = true;
            }

            InitializeComponent();
            
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - SystemParameters.WindowResizeBorderThickness.Top - SystemParameters.WindowResizeBorderThickness.Bottom + 1;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.WindowResizeBorderThickness.Left - SystemParameters.WindowResizeBorderThickness.Right + 1;

            StateChanged -= WindowStateChanged;
            StateChanged += WindowStateChanged;

            if (content != null)
                Content.Content = content;
            
            if (toolbar != null)
            {
                Toolbar.Children.RemoveAt(0);
                Toolbar.Children.Add(toolbar);
            }

            WindowStateChanged(null, EventArgs.Empty);
        }

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
            get => Close.Visibility == Visibility.Visible;
            set => Close.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
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

        public static void ChangeTheme(bool isDark, Color color)
        {
            IBaseTheme baseTheme = AWTheme.Light;
            if (isDark)
                baseTheme = AWTheme.Dark;

            ITheme theme = Theme.Create(baseTheme, color, color);
            new PaletteHelper().SetTheme(theme);
        }
    }
}
