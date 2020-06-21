using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace AW.Visual.Common
{
    public partial class BaseControl : UserControl
    {
        public BaseControl()
        {
            DataContextChanged += (s, e) => OnDataContextChange();
            Loaded += (s, e) => OnLoaded();
        }

        protected virtual void OnDataContextChange() { }
        protected virtual void OnLoaded() { }
    }

    public class BaseContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SimpleCommand : ICommand
    {
        public Action OnExecute { get; set; }

        private readonly Action Action;
        private readonly Func<bool> CanAction;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SimpleCommand(Action action, Func<bool> canAction = null)
        {
            Action = action;
            CanAction = canAction;
        }

        public virtual bool CanExecute(object parameter)
            => CanAction == null || CanAction();

        public virtual void Execute(object parameter)
        {
            OnExecute?.Invoke();
            Action?.Invoke();
        }
    }

    public class RelayCommand<T> : SimpleCommand
    {
        private readonly Action<T> Action;
        private readonly Func<T, bool> CanAction;

        public RelayCommand(Action<T> action, Func<T, bool> canAction = null) : base(null)
        {
            Action = action;
            CanAction = canAction;
        }

        public override bool CanExecute(object parameter)
            => CanAction == null || CanAction((T)parameter);

        public override void Execute(object parameter)
        {
            OnExecute?.Invoke();
            Action?.Invoke((T)parameter);
        }
    }
}
