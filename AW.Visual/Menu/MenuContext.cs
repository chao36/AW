using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AW.Base.Serializer.Common;
using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Menu
{
    public interface IMenuItem
    {
        Func<IMenuItem, bool> CanSelect { get; set; }
        Func<IMenuItem, bool> CanRemove { get; set; }
        Func<IMenuItem, string, bool> CanRename { get; set; }

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

    public interface IMenuGroup : IMenuItem, IReference
    {
        Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
        Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
        Func<IMenuItem, IMenuGroup, bool> CanItemChangeGroup { get; set; }

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

    [AWSerializable]
    public class MenuItemContext : BaseContext, IMenuItem
    {
        [AWReference]
        public IMenuGroup Group { get; set; }

        [AWIgnore]
        public Func<IMenuItem, bool> CanSelect { get; set; }
        [AWIgnore]
        public Func<IMenuItem, bool> CanRemove { get; set; }
        [AWIgnore]
        public Func<IMenuItem, string, bool> CanRename { get; set; }

        public int Left { get; set; }

        private bool isSelect;
        [AWIgnore]
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

        [AWIgnore]
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

        [AWIgnore]
        public virtual IEnumerable<string> Names => new List<string> { Name.ToLower() };
    }

    [AWSerializable]
    public class MenuGroupItemContext : MenuItemContext, IMenuGroup
    {
        public int ReferenceId { get; set; }

        public MenuGroupItemContext()
            => Group = this;

        [AWIgnore]
        public Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
        [AWIgnore]
        public Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
        [AWIgnore]
        public Func<IMenuItem, IMenuGroup, bool> CanItemChangeGroup { get; set; }

        public bool IsOpen { get; set; }
        [AWIgnore]
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

        [AWIgnore]
        public IEnumerable<IMenuItem> Items => NeedSortItems ? Source.OrderByDescending(i => i is IMenuGroup).ThenBy(i => i.Name) : (IEnumerable<IMenuItem>)Source;
        [AWIgnore]
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

            item.CanRemove ??= CanRemove;
            item.CanRename ??= CanRename;
            item.CanSelect ??= CanSelect;

            if (item is IMenuGroup group)
            {
                group.OnCreateItem ??= OnCreateItem;
                group.OnCreateGroup ??= OnCreateGroup;
                group.CanItemChangeGroup ??= CanItemChangeGroup;

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
                item?.CanRemove?.Invoke(item);

            Source.Clear();
            Notify(nameof(Items));
        }

        [AWIgnore]
        public override IEnumerable<string> Names => new List<string> { Name.ToLower() }.Concat(Source.SelectMany(i => i.Names));
    }
}
