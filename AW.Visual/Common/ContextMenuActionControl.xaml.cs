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

        void UpdateState();
    }

    public partial class ContextMenuActionControl : BaseControl
    {
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

                    if (Element.EndContent is PackIcon icon)
                    {
                        BindingOperations.ClearBinding(icon, VisibilityProperty);
                        icon.Visibility = Visibility.Collapsed;
                    }
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

        public ContextMenuActionContext(string header, PackIconKind? icon, ICommand command, IEnumerable<IContextMenuAction> actions) : base(header, icon, command)
        {
            Actions = actions;

            if (Actions != null)
                foreach (IContextMenuAction item in Actions)
                    item.Parent = this;
        }

        public ContextMenuActionContext(string header, PackIconKind? icon, Action action, IEnumerable<IContextMenuAction> actions = null) 
            : this(header, icon, new SimpleCommand(action), actions) { }

        public ContextMenuActionContext(string header, PackIconKind? icon, IEnumerable<IContextMenuAction> actions = null) 
            : this(header, icon, action: null, actions) { }

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
        public IEnumerable<IContextMenuAction> Actions { get; }

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
