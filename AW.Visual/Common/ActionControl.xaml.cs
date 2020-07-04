using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Common
{
    public interface IActionContext
    {
        bool IsSelect { get; set; }

        string Header { get; set; }
        PackIconKind? Icon { get; set; }

        ICommand Command { get; set; }

        double FontSize { get; set; }
        double IconSize { get; set; }
        Brush IconColor { get; }
    }

    public partial class ActionControl : BaseControl
    {
        public static readonly DependencyProperty StartContentProperty = DependencyProperty.Register(nameof(StartContent), typeof(object),
            typeof(ActionControl), new PropertyMetadata(null, StartContentPropertyChanged));

        public static readonly DependencyProperty CenterContentProperty = DependencyProperty.Register(nameof(CenterContent), typeof(object),
            typeof(ActionControl), new PropertyMetadata(null, CenterContentPropertyChanged));

        public static readonly DependencyProperty EndContentProperty = DependencyProperty.Register(nameof(EndContent), typeof(object),
            typeof(ActionControl), new PropertyMetadata(null, EndContentPropertyChanged));

        public object StartContent
        {
            get => StartElement.Content;
            set => SetValue(StartContentProperty, value);
        }

        public object CenterContent
        {
            get => CenterElement.Content;
            set => SetValue(StartContentProperty, value);
        }

        public object EndContent
        {
            get => EndElement.Content;
            set => SetValue(StartContentProperty, value);
        }

        private void StartContentPropertyChanged(object content) => StartElement.Content = content;
        private static void StartContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).StartContentPropertyChanged(e.NewValue);

        private void CenterContentPropertyChanged(object content) => CenterElement.Content = content;
        private static void CenterContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((ActionControl)d).CenterContentPropertyChanged(e.NewValue);

        private void EndContentPropertyChanged(object content) => EndElement.Content = content;
        private static void EndContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((ActionControl)d).EndContentPropertyChanged(e.NewValue);

        public static readonly DependencyProperty RippleFeedbackProperty = DependencyProperty.Register(nameof(RippleFeedback), typeof(Brush),
            typeof(ActionControl), new PropertyMetadata(null, RippleFeedbackPropertyChanged));

        public static readonly DependencyProperty HideHeaderProperty = DependencyProperty.Register(nameof(HideHeader), typeof(bool),
            typeof(ActionControl), new PropertyMetadata(false, HideHeaderPropertyChanged));

        public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility),
            typeof(ActionControl), new PropertyMetadata(Visibility.Visible, IconVisibilityPropertyChanged));

        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(nameof(ContentMargin), typeof(Thickness),
           typeof(ActionControl), new PropertyMetadata(new Thickness(0), ContentMarginPropertyChanged));

        public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(nameof(ContentPadding), typeof(Thickness),
           typeof(ActionControl), new PropertyMetadata(new Thickness(4, 0, 4, 0), ContentPaddingPropertyChanged));

        public static readonly DependencyProperty ContentStyleProperty = DependencyProperty.Register(nameof(ContentStyle), typeof(Style),
           typeof(ActionControl), new PropertyMetadata(null, ContentStylePropertyChanged));

        public Brush RippleFeedback
        {
            get => RippleAssist.GetFeedback(Element);
            set => SetValue(RippleFeedbackProperty, value);
        }

        public bool HideHeader
        {
            get => Header.Visibility == Visibility.Collapsed;
            set => SetValue(HideHeaderProperty, value);
        }

        public Visibility IconVisibility
        {
            get => Icon.Visibility;
            set => SetValue(IconVisibilityProperty, value);
        }

        public Thickness ContentMargin
        {
            get => Container.Margin;
            set => SetValue(ContentMarginProperty, value);
        }

        public Thickness ContentPadding
        {
            get => Element.Padding;
            set => SetValue(ContentPaddingProperty, value);
        }

        public Style ContentStyle
        {
            get => Element.Style;
            set => SetValue(ContentStyleProperty, value);
        }

        private void RippleFeedbackPropertyChanged(Brush brush) => RippleAssist.SetFeedback(Element, brush);
        private static void RippleFeedbackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).RippleFeedbackPropertyChanged((Brush)e.NewValue);

        private void HideHeaderPropertyChanged(bool hide) => Header.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        private static void HideHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).HideHeaderPropertyChanged((bool)e.NewValue);

        private void IconVisibilityPropertyChanged(bool hide) => Icon.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        private static void IconVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((ActionControl)d).IconVisibilityPropertyChanged((bool)e.NewValue);

        private void ContentMarginPropertyChanged(Thickness value) => Container.Margin = value;
        private static void ContentMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).ContentMarginPropertyChanged((Thickness)e.NewValue);

        private void ContentPaddingPropertyChanged(Thickness value) => Element.Padding = value;
        private static void ContentPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).ContentPaddingPropertyChanged((Thickness)e.NewValue);

        private void ContentStylePropertyChanged(Style value) => Element.Style = value;
        private static void ContentStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ActionControl)d).ContentStylePropertyChanged((Style)e.NewValue);

        public ActionControl() => InitializeComponent();
       
        protected override void OnDataContextChange()
        {
            if (DataContext is IActionContext context)
            {
                if (context.Icon == null)
                    Icon.Visibility = Visibility.Hidden;

                if (context.IconColor != null)
                    Icon.Foreground = context.IconColor;
            }
        }
    }

    public class ActionContext : BaseContext, IActionContext
    {
        public ActionContext(string header, PackIconKind? icon)
        {
            Header = header;
            Icon = icon;
        }

        public ActionContext(string header, PackIconKind? icon, ICommand command) : this(header, icon)
            => Command = command;

        public ActionContext(string header, PackIconKind? icon, Action action)
            : this(header, icon, new SimpleCommand(action)) { }

        private bool isSelect;
        private string header;
        private PackIconKind? icon;
        private ICommand command;
        private double fontSize = 14;
        private double iconSize = 20;

        public bool IsSelect
        {
            get => isSelect;
            set
            {
                isSelect = value;
                Notify();
            }
        }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                Notify();
            }
        }

        public PackIconKind? Icon
        {
            get => icon;
            set
            {
                icon = value;
                Notify();
            }
        }

        public ICommand Command
        {
            get => command;
            set
            {
                command = value;
                Notify();
            }
        }

        public double FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                Notify();
            }
        }
        public double IconSize 
        {
            get => iconSize;
            set
            {
                iconSize = value;
                Notify();
            }
        }

        public Brush IconColor { get; set; } = null;
    }
}
