using System;
using System.Collections.Generic;
using System.Linq;

using AW.Base.Serializer.Common;
using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Menu
{
    public interface IMenuItem : IActionContext
    {
        Func<IMenuItem, bool> OnRemove { get; set; }
        Func<IMenuItem, string, bool> OnRename { get; set; }

        int Left { get; set; }
        IMenuGroup Group { get; set; }

        bool ViewRemove { get; }
        bool ViewRename { get; }
        bool CanChangeGroup { get; }

        List<IContextMenuAction> Actions { get; set; }

        object Content { get; set; }
        object Context { get; set; }

        IEnumerable<string> Headers { get; }
    }

    public class MenuItemContext : ActionContext, IMenuItem
    {
        private List<IContextMenuAction> actions;

        public MenuItemContext(string header, PackIconKind? icon) : base(header, icon) { }
        public MenuItemContext(string header, PackIconKind? icon, Action<IMenuItem> action) : this(header, icon)
        {
            Action commandAction = () => action?.Invoke(this);
            Command = new SimpleCommand(commandAction);
        }

        public Func<IMenuItem, bool> OnRemove { get; set; }
        public Func<IMenuItem, string, bool> OnRename { get; set; }

        public int Left { get; set; }
        public IMenuGroup Group { get; set; }

        public bool ViewRemove { get; set; } = true;
        public bool ViewRename { get; set; } = true;
        public bool CanChangeGroup { get; set; }

        public List<IContextMenuAction> Actions 
        {
            get => actions;
            set
            {
                actions = value;
                Notify();
            }
        }

        public object Content { get; set; }
        public object Context { get; set; }

        public virtual IEnumerable<string> Headers => new List<string> { Header };
    }

    public interface IMenuGroup : IMenuItem, IReference
    {
        Func<IMenuGroup, string, IMenuItem> OnCreateItem { get; set; }
        Func<IMenuGroup, string, IMenuGroup> OnCreateGroup { get; set; }
        Func<IMenuItem, IMenuGroup, bool> OnItemChangeGroup { get; set; }

        bool IsOpen { get; set; }
        bool NeedSortItems { get; set; }

        bool ViewCreateItem { get; }
        bool ViewCreateGroup { get; }

        string CreateItemHint { get; set; }
        string CreateGroupHint { get; set; }

        IEnumerable<IMenuItem> Items { get; }

        void AddItem(IMenuItem item);
        void RemoveItem(IMenuItem item);

        IEnumerable<IMenuItem> AllItems { get; }
    }

    public class MenuGroupContext : MenuItemContext, IMenuGroup
    {
        public int ReferenceId { get; set; }

        public MenuGroupContext(string header, PackIconKind? icon) : base(header, icon)
        {
            Group = this;
            Command = new SimpleCommand(() =>
            {
                IsOpen = !IsOpen;
            });
        }

        public MenuGroupContext(string header, PackIconKind? icon, Action<IMenuGroup> action) : this(header, icon)
            => (Command as SimpleCommand).OnExecute = () => action?.Invoke(this);

        public Func<IMenuGroup, string, IMenuItem> OnCreateItem { get; set; }
        public Func<IMenuGroup, string, IMenuGroup> OnCreateGroup { get; set; }
        public Func<IMenuItem, IMenuGroup, bool> OnItemChangeGroup { get; set; }

        private bool isOpen;
        public bool IsOpen 
        {
            get => isOpen;
            set
            {
                isOpen = value;
                Notify();
            }
        }

        public bool NeedSortItems { get; set; } = true;

        public bool ViewCreateItem { get; set; } = true;
        public bool ViewCreateGroup { get; set; } = true;

        public string CreateItemHint { get; set; }
        public string CreateGroupHint { get; set; }

        public List<IMenuItem> Source { get; set; } = new List<IMenuItem>();
        public IEnumerable<IMenuItem> Items => NeedSortItems ? Source.OrderByDescending(i => i is IMenuGroup).ThenBy(i => i.Header) : (IEnumerable<IMenuItem>)Source;
        
        public void AddItem(IMenuItem item)
        {
            item.Left = Left + 40;
            item.Group = this;

            item.OnRemove ??= OnRemove;
            item.OnRename ??= OnRename;

            if (item is IMenuGroup group)
            {
                group.OnCreateItem ??= OnCreateItem;
                group.OnCreateGroup ??= OnCreateGroup;
                group.OnItemChangeGroup ??= OnItemChangeGroup;

                group.CreateItemHint ??= CreateItemHint;
                group.CreateGroupHint ??= CreateGroupHint;

                if (group.Items != null)
                    foreach (IMenuItem menuItem in group.Items.ToList())
                    {
                        group.RemoveItem(menuItem);
                        group.AddItem(menuItem);
                    }
            }

            Source.Add(item);
            Notify(nameof(Items));
        }

        public void RemoveItem(IMenuItem item)
        {
            if (item.OnRemove?.Invoke(item) == true)
            {
                Source.Remove(item);
                Notify(nameof(Items));
            }
        }

        public IEnumerable<IMenuItem> AllItems
        {
            get
            {
                List<IMenuItem> result = new List<IMenuItem>();

                foreach (IMenuItem item in Source)
                    if (item is IMenuGroup group)
                        result.AddRange(group.AllItems);
                    else
                        result.Add(item);

                return result;
            }
        }

        public override IEnumerable<string> Headers => new List<string> { Header }.Concat(Source.SelectMany(i => i.Headers));
    }
}
