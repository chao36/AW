using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace AW.Visual.Output
{
    public struct Message
    {
        public int Id { get; }
        public bool IsVisible { get; }

        public string Data { get; }
        public PackIconKind Kind { get; }

        public List<MessageAction> Actions { get; }

        public Message(int id, string message, PackIconKind kind, List<MessageAction> actions = null)
        {
            Id = id;
            IsVisible = actions != null && actions.Count > 0;

            Data = message;
            Kind = kind;

            Actions = actions;
        }
    }

    public struct MessageAction
    {
        public string ActionTitle { get; set; }
        public SimpleCommand ActionCommand { get; set; }
    }

    public class OutputContext
    {
        public static PackIconKind ErrorIcon
            = PackIconKind.CloseBox;

        public static PackIconKind InfoIcon
            = PackIconKind.WarningBox;

        public static PackIconKind MessageIcon
            = PackIconKind.CheckboxMarked;

        public static SimpleCommand ClearHistory { get; }
            = new SimpleCommand(Clear, () => History.Count > 0);

        public static ObservableCollection<Message> History { get; }
            = new ObservableCollection<Message>();

        private static int SetOption(PackIconKind kind, string message, string option, List<MessageAction> actions = null)
        {
            int id = History.Count + 1;
            if (option != null)
                message = $"{message}\n{option}";

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                History.Add(new Message(id, message, kind, actions));
            });

            return id;
        }

        public static int Error(string message, string option = null, List<MessageAction> actions = null)
            => SetOption(ErrorIcon, message, option, actions);

        public static int Info(string message, string option = null, List<MessageAction> actions = null)
            => SetOption(InfoIcon, message, option, actions);

        public static int Message(string message, string option = null, List<MessageAction> actions = null)
            => SetOption(MessageIcon, message, option, actions);

        public static void Remove(int Id)
        {
            if (History.Any(h => h.Id == Id))
                History.Remove(History.FirstOrDefault(h => h.Id == Id));
        }

        private static void Clear()
            => History.Clear();
    }
}
