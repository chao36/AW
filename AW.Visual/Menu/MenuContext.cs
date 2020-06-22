using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Menu
{
    public interface IMenuItem
    {
        Func<IMenuItem, bool> OnSelect { get; set; }
        Func<IMenuItem, bool> OnRemove { get; set; }
        Func<IMenuItem, string, bool> OnRename { get; set; }

        IMenuGroup Group { get; set; }

        int Left { get; set; }
        bool IsSelect { get; set; }

        bool ViewRemove { get; }
        bool ViewRename { get; }
        bool CanChangeGroup { get; }

        IEnumerable<IAction> Actions { get; set; }

        string Name { get; set; }
        PackIconKind Icon { get; }

        object Content { get; set; }
        object Context { get; set; }

        IEnumerable<string> Names { get; }
    }

    public interface IMenuGroup : IMenuItem
    {
        Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
        Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
        Func<IMenuItem, IMenuGroup, bool> OnChangeGroup { get; set; }

        bool IsOpen { get; set; }

        bool NeedSortItems { get; }
        bool ViewCreateItem { get; }
        bool ViewCreateGroup { get; }

        string CreateItemHint { get; set; }
        string CreateGroupHint { get; set; }

        ObservableCollection<IMenuItem> Source { get; }
        IEnumerable<IMenuItem> Items { get; }
        IEnumerable<IMenuItem> AllItems { get; }

        void AddItem(IMenuItem item);
        void RemoveItem(IMenuItem item);
        void Clear();
    }

    public class MenuItemContext : BaseContext, IMenuItem
    {
        public IMenuGroup Group { get; set; }

        public Func<IMenuItem, bool> OnSelect { get; set; }
        public Func<IMenuItem, bool> OnRemove { get; set; }
        public Func<IMenuItem, string, bool> OnRename { get; set; }

        public int Left { get; set; }

        private bool isSelect;
        public virtual bool IsSelect
        {
            get => isSelect;
            set
            {
                isSelect = value;
                Notify();
            }
        }

        public bool ViewRemove { get; set; } = true;
        public bool ViewRename { get; set; } = true;
        public bool CanChangeGroup { get; set; }

        public IEnumerable<IAction> Actions { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                Notify();
            }
        }
        public PackIconKind Icon { get; set; }

        public object Content { get; set; }
        public object Context { get; set; }

        public virtual IEnumerable<string> Names => new List<string> { Name.ToLower() };
    }

    public class MenuGroupItemContext : MenuItemContext, IMenuGroup
    {
        public MenuGroupItemContext()
            => Group = this;

        public Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
        public Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
        public Func<IMenuItem, IMenuGroup, bool> OnChangeGroup { get; set; }

        public bool IsOpen { get; set; }
        public override bool IsSelect
        {
            get => false;
            set
            {
                foreach (IMenuItem item in Items)
                    item.IsSelect = value;
            }
        }

        public bool NeedSortItems { get; set; } = true;

        public bool ViewCreateItem { get; set; } = true;
        public bool ViewCreateGroup { get; set; } = true;

        public string CreateItemHint { get; set; }
        public string CreateGroupHint { get; set; }

        public ObservableCollection<IMenuItem> Source { get; set; } = new ObservableCollection<IMenuItem>();

        public IEnumerable<IMenuItem> Items => NeedSortItems ? Source.OrderByDescending(i => i is IMenuGroup).ThenBy(i => i.Name) : (IEnumerable<IMenuItem>)Source;
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

        public void AddItem(IMenuItem item)
        {
            item.Left = Left + 40;
            item.Group = this;

            item.OnRemove ??= OnRemove;
            item.OnRename ??= OnRename;
            item.OnSelect ??= OnSelect;

            if (item is IMenuGroup group)
            {
                group.OnCreateItem ??= OnCreateItem;
                group.OnCreateGroup ??= OnCreateGroup;
                group.OnChangeGroup ??= OnChangeGroup;

                group.CreateItemHint ??= CreateItemHint;
                group.CreateGroupHint ??= CreateGroupHint;

                if (group.Items != null)
                    foreach (IMenuItem menuItem in group.Source.ToList())
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
            Source.Remove(item);
            Notify(nameof(Items));
        }

        public void Clear()
        {
            foreach (IMenuItem item in Source.ToList())
                item?.OnRemove?.Invoke(item);

            Source.Clear();
            Notify(nameof(Items));
        }

        public override IEnumerable<string> Names => new List<string> { Name.ToLower() }.Concat(Source.SelectMany(i => i.Names));
    }
}
