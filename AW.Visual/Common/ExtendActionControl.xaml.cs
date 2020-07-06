using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Common
{
    public interface IExtendActionControl : IActionContext
    {
        string SubHeader { get; set; }
        string EndText { get; set; }

        double SubFontSize { get; set; }
        double EndFontSize { get; set; }

        IEnumerable<IContextMenuAction> Actions { get; set; }
    }

    public partial class ExtendActionControl : BaseControl
    {
        public ExtendActionControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IExtendActionControl context)
            {
                if (context.Icon == null && Icon.Visibility == Visibility.Visible)
                    Icon.Visibility = Visibility.Hidden;

                if (context.IconColor != null)
                    Icon.Foreground = context.IconColor;
            }
        }
    }

    public class ExtendActionContext : ActionContext, IActionContext
    {
        private string subHeader;
        private string endText;
        private double subFontSize = 14;
        private double endFontSize = 12;

        public ExtendActionContext(string header, string subHeader, string endText, PackIconKind? icon, ICommand command) 
            : base(header, icon, command)
        {
            FontSize = 15;
            IconSize = 30;

            SubHeader = subHeader;
            EndText = endText;
        }

        public ExtendActionContext(string header, string subHeader, string endText, PackIconKind? icon, ICommand command, IEnumerable<IContextMenuAction> actions)
            : this(header, subHeader, endText, icon, command)
            => Actions = actions;

        public ExtendActionContext(string header, string subHeader, string endText, PackIconKind? icon, Action action) 
            : this(header, subHeader, endText, icon, new SimpleCommand(action)) { }

        public ExtendActionContext(string header, string subHeader, string endText, PackIconKind? icon, Action action, IEnumerable<IContextMenuAction> actions)
            : this(header, subHeader, endText, icon, new SimpleCommand(action), actions) { }

        public string SubHeader
        {
            get => subHeader;
            set
            {
                subHeader = value;
                Notify();
            }
        }

        public string EndText
        {
            get => endText;
            set
            {
                endText = value;
                Notify();
            }
        }

        public double SubFontSize
        {
            get => subFontSize;
            set
            {
                subFontSize = value;
                Notify();
            }
        }

        public double EndFontSize
        {
            get => endFontSize;
            set
            {
                endFontSize = value;
                Notify();
            }
        }

        public bool HasItems => Actions?.Any() == true;
        public IEnumerable<IContextMenuAction> Actions { get; private set; }
    }
}
