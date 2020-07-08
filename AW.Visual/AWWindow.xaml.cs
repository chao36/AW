using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using AW.Visual.ColorTheme;
using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual
{
    public class QuickKey
    {
        public Key Key { get; }
        public Key[] ModifierKeys { get; }

        public QuickKey(Key key, params Key[] modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        public override string ToString()
        {
            string result = null;

            if (ModifierKeys?.Length > 0)
                result = string.Concat(ModifierKeys.Select(k => $"{k} + "));
            
            return $"{result}{Key}";
        }
    }

    public class ActionQuickKey : QuickKey
    {
        public Action Action { get; }

        public ActionQuickKey(Action action, Key key, params Key[] modifierKeys) : base(key, modifierKeys)
        {
            Action = action;
        }
    }

    public partial class AWWindow : Window
    {
        public static List<ActionQuickKey> QuickKeys { get; } = new List<ActionQuickKey>();

        public static string History { get; set; } = "History";
        public static string RenameTitle { get; set; } = "Rename";
        public static string NewNameTitle { get; set; } = "New name";
        public static string AddTitle { get; set; } = "Add";
        public static string RemoveTitle { get; set; } = "Remove";
        public static string OkTitle { get; set; } = "Ok";
        public static string CancelTitle { get; set; } = "Cancel";

        public static AWWindow Current { get; private set; }

        public static void ChangeTheme(bool isDark, Color color)
        {
            IBaseTheme baseTheme = AWTheme.Light;
            if (isDark)
                baseTheme = AWTheme.Dark;

            ITheme theme = Theme.Create(baseTheme, color, color);
            new PaletteHelper().SetTheme(theme);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            Current = this;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            var result = QuickKeys.Where(q => q.Key == e.Key && q.ModifierKeys.All(k => Keyboard.IsKeyDown(k)));
            foreach (ActionQuickKey q in result)
                q.Action?.Invoke();
        }

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
                Toolbar.Children.Remove(Header);
                Toolbar.Children.Add(toolbar);
                Grid.SetColumn(toolbar, 1);
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

        public ImageSource AppIcon
        {
            get => Icon.Source;
            set
            {
                Icon.Visibility = value == null ? Visibility.Hidden : Visibility.Visible;
                Icon.Source = value;
            }
        }

        public Thickness AppIconMargin
        {
            get => Icon.Margin;
            set => Icon.Margin = value;
        }

        public double AppIconSize
        {
            get => Icon.Width;
            set
            {
                Icon.Width = value;
                Icon.Height = value;
            }
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
    }
}
