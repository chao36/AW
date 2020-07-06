using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Menu
{
    public partial class MenuControl : BaseControl
    {
        private PackIcon Group => Element.StartContent as PackIcon;
        private TextBox Edit => Element.CenterContent as TextBox;
        private ContextMenu Menu => Element.ContextMenu;

        public MenuControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IMenuItem item)
            {
                if (item.CanChangeGroup)
                    VisualHelper.LeftDown(Element, _ =>
                    {
                        DragDrop.DoDragDrop(Element, item, DragDropEffects.Move);
                    });

                Element.ContentMargin = new Thickness(item.Left, 0, 0, 0);

                if (item.Actions == null)
                    item.Actions = new List<IContextMenuAction>();

                if (item is IMenuGroup group)
                {
                    Container.AllowDrop = true;

                    Container.Drop -= OnDrop;
                    Container.Drop += OnDrop;

                    Group.Visibility = Visibility.Visible;

                    foreach (IContextMenuAction menuAction in item.Actions.Where(a => a.Header == group.CreateItemHint).ToList())
                        item.Actions.Remove(menuAction);

                    if (group.ViewCreateItem)
                        item.Actions.Add(new ContextMenuActionContext(group.CreateItemHint, PackIconKind.Add, () =>
                        {
                            IsCreate = true;
                            IsCreateGroup = false;

                            Edit.Text = "";
                            HintAssist.SetHint(Edit, group.CreateItemHint);

                            Element.HideHeader = true;
                            Edit.Visibility = Visibility.Visible;

                            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    Edit.Focus();
                                    Keyboard.Focus(Edit);
                                    Edit.CaretIndex = 0;
                                }));
                        }));

                    foreach (IContextMenuAction menuAction in item.Actions.Where(a => a.Header == group.CreateGroupHint).ToList())
                        item.Actions.Remove(menuAction);

                    if (group.ViewCreateGroup)
                        item.Actions.Add(new ContextMenuActionContext(group.CreateGroupHint, PackIconKind.FolderAdd, () =>
                        {
                            IsCreate = false;
                            IsCreateGroup = true;

                            Edit.Text = "";
                            HintAssist.SetHint(Edit, group.CreateGroupHint);

                            Element.HideHeader = true;
                            Edit.Visibility = Visibility.Visible;

                            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    Edit.Focus();
                                    Keyboard.Focus(Edit);
                                    Edit.CaretIndex = 0;
                                }));
                        }));
                }

                foreach (IContextMenuAction menuAction in item.Actions.Where(a => a.Header == AWWindow.RenameTitle).ToList())
                    item.Actions.Remove(menuAction);

                if (item.ViewRename)
                    item.Actions.Add(new ContextMenuActionContext(AWWindow.RenameTitle, PackIconKind.RenameBox, () =>
                    {
                        IsCreate = false;
                        IsCreateGroup = false;

                        Edit.Text = item.Header;
                        HintAssist.SetHint(Edit, AWWindow.NewNameTitle);

                        Element.HideHeader = true;
                        Edit.Visibility = Visibility.Visible;

                        Dispatcher.BeginInvoke(DispatcherPriority.Input,
                            new Action(() =>
                            {
                                Edit.Focus();
                                Keyboard.Focus(Edit);
                                Edit.CaretIndex = item.Header.Length;
                            }));
                    }));

                foreach (IContextMenuAction menuAction in item.Actions.Where(a => a.Header == AWWindow.RemoveTitle).ToList())
                    item.Actions.Remove(menuAction);

                if (item.ViewRemove)
                    item.Actions.Add(new ContextMenuActionContext(AWWindow.RemoveTitle, PackIconKind.Close, () =>
                    {
                        if (item.OnRemove?.Invoke(item) == true)
                            item.Group?.RemoveItem(item);
                    })
                    {
                        SeparatorStyle = SeparatorStyle.Top,
                        IconColor = ColorHelper.RedSet.Color500.ToBrush()
                    });

                foreach (IContextMenuAction menuAction in item.Actions)
                    (menuAction.Command as SimpleCommand).OnExecute = () => Menu.IsOpen = false;

                (item as BaseContext).Notify(nameof(item.Actions));

                VisualHelper.ExitOnEnter(Edit, () =>
                {
                    OnEdit(item);
                });
            }
        }

        private void OnDrop(object sender, DragEventArgs de)
        {
            IMenuItem menuItem = (IMenuItem)de.Data.GetData(typeof(MenuItemContext)) ?? (IMenuItem)de.Data.GetData(typeof(MenuGroupContext));

            if (menuItem != null && DataContext is IMenuGroup group && group.OnItemChangeGroup?.Invoke(menuItem, group) == true)
            {
                menuItem.Group.RemoveItem(menuItem);
                group.AddItem(menuItem);
            }
        }

        private bool IsCreate;
        private bool IsCreateGroup;

        private void OnEdit(IMenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(Edit.Text))
                if (IsCreate || IsCreateGroup)
                {
                    Edit.Visibility = Visibility.Hidden;
                    Element.HideHeader = false;
                    return;
                }
                else
                    return;

            bool complite = false;

            if (menuItem is IMenuGroup group)
            {
                if (IsCreate)
                {
                    IMenuItem item = group.OnCreateItem?.Invoke(group, Edit.Text);

                    if (item != null)
                        group.AddItem(item);

                    complite = true;
                }
                else if (IsCreateGroup)
                {
                    IMenuGroup item = group.OnCreateGroup?.Invoke(group, Edit.Text);

                    if (item != null)
                        group.AddItem(item);

                    complite = true;
                }
            }
            
            if (!IsCreate && !IsCreateGroup && (Edit.Text == menuItem.Header || menuItem.OnRename?.Invoke(menuItem, Edit.Text) == true))
            {
                menuItem.Header = Edit.Text;
                complite = true;
            }

            if (complite)
            {
                Edit.Text = "";

                Edit.Visibility = Visibility.Hidden;
                Element.HideHeader = false;
            }
            else
                Edit.Text = IsCreate || IsCreateGroup ? "" : menuItem.Header;
        }
    }
}
