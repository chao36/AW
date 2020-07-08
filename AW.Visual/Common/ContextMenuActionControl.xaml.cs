using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Common
{
    public enum SeparatorStyle
    {
        None = 0,
        Top = 1,
        Bottom = 2
    }

    public enum SubContextMenuType
    {
        Right = 0,
        Bottom = 1
    }

    public interface IContextMenuAction : IActionContext
    {
        UIElement Element { get; set; }
        IContextMenuAction Parent { get; set; }

        bool IconCollapse { get; set; }

        bool IsOpen { get; set; }

        SeparatorStyle SeparatorStyle { get; set; }
        SubContextMenuType SubContextMenuType { get; set; }

        bool HasItems { get; }
        IEnumerable<IContextMenuAction> Actions { get; }

        string QuickKey { get; }
        void SetQuickKey(QuickKey quickKey);

        void UpdateState();
    }

    public partial class ContextMenuActionControl : BaseControl
    {
        public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.Register(nameof(InnerPadding), typeof(Thickness),
              typeof(ActionControl), new PropertyMetadata(new Thickness(4, 0, 4, 0), InnerPaddingPropertyChanged));

        public Thickness InnerPadding
        {
            get => Element.ContentPadding;
            set => SetValue(InnerPaddingProperty, value);
        }

        private void InnerPaddingPropertyChanged(Thickness value) => Element.ContentPadding = value;
        private static void InnerPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ContextMenuActionControl)d).InnerPaddingPropertyChanged((Thickness)e.NewValue);

        public ContextMenuActionControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IContextMenuAction context)
            {
                if (context.SeparatorStyle == SeparatorStyle.Top)
                {
                    TopSeparator.Visibility = Visibility.Visible;
                    TopSeparator.Margin = new Thickness(context.IconSize + 8, 2, 0, 2);
                }
                else if (context.SeparatorStyle == SeparatorStyle.Bottom)
                {
                    BottomSeparator.Visibility = Visibility.Visible;
                    BottomSeparator.Margin = new Thickness(context.IconSize + 8, 2, 0, 2);
                }

                if (context.SubContextMenuType == SubContextMenuType.Bottom)
                {
                    Popup.Placement = PlacementMode.Bottom;
                    Popup.HorizontalOffset = 0;
                    Popup.VerticalOffset = 0;

                    if (Element.EndContent is UIElement element)
                        element.Visibility = Visibility.Collapsed;
                }

                if (context.IconCollapse)
                    Element.IconVisibility = Visibility.Collapsed;

                context.Element = this;

                MouseEnter += (s, e) => context.IsSelect = true;
                MouseLeave += (s, e) => Task.Run(async () =>
                {
                    await Task.Delay(50);
                    context.UpdateState();
                });

                if (context.HasItems)
                    MouseEnter += (s, e) => context.IsOpen = true;
                else
                    MouseLeave += (s, e) => context.IsSelect = false;
            }
        }
    }

    public class ContextMenuActionContext : ActionContext, IContextMenuAction
    {
        private bool isOpen;

        public ContextMenuActionContext(string header) : base(header, null) { }
        public ContextMenuActionContext(string header, PackIconKind? icon) : base(header, icon) { }
        public ContextMenuActionContext(string header, PackIconKind? icon, IEnumerable<IContextMenuAction> actions) : base(header, icon)
            => SetActions(actions);
        public ContextMenuActionContext(string header, PackIconKind? icon, ICommand command, QuickKey quickKey = null) : base(header, icon, command)
            => SetQuickKey(quickKey);
        public ContextMenuActionContext(string header, PackIconKind? icon, Action action, QuickKey quickKey = null) : base(header, icon, action)
            => SetQuickKey(quickKey);
        public ContextMenuActionContext(string header, PackIconKind? icon, ICommand command, IEnumerable<IContextMenuAction> actions, QuickKey quickKey = null) : base(header, icon, command)
            => Init(actions, quickKey);
        public ContextMenuActionContext(string header, PackIconKind? icon, Action action, IEnumerable<IContextMenuAction> actions, QuickKey quickKey = null) : base(header, icon, action)
            => Init(actions, quickKey);

        private void Init(IEnumerable<IContextMenuAction> actions, QuickKey quickKey)
        {
            SetActions(actions);
            SetQuickKey(quickKey);
        }

        public void SetActions(IEnumerable<IContextMenuAction> actions)
        {
            Actions = actions;

            if (Actions != null)
                foreach (IContextMenuAction item in Actions)
                    item.Parent = this;
        }

        public string QuickKey { get; private set; }
        public void SetQuickKey(QuickKey quickKey)
        {
            if (quickKey == null)
            {
                QuickKey = null;
                Notify(nameof(QuickKey));

                return;
            }

            QuickKey = quickKey.ToString();
            AWWindow.QuickKeys.Add(new ActionQuickKey(() => Command?.Execute(null), quickKey.Key, quickKey.ModifierKeys));
        }

        public UIElement Element { get; set; }
        public IContextMenuAction Parent { get; set; }

        public bool IconCollapse { get; set; }

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;
                Notify();
            }
        }

        public SeparatorStyle SeparatorStyle { get; set; }
        public SubContextMenuType SubContextMenuType { get; set; }

        public bool HasItems => Actions?.Any() == true;
        public IEnumerable<IContextMenuAction> Actions { get; private set; }

        public void UpdateState()
        {
            if (!Element.IsMouseOver && Actions?.Any(i => i.IsSelect || i.IsOpen) == false)
            {
                IsOpen = false;
                IsSelect = false;
            }

            Parent?.UpdateState();
        }
    }
}
