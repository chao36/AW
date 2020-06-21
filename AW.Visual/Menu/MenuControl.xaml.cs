using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Menu
{
    public partial class MenuControl : BaseControl
    {
        public MenuControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IMenuItem item)
            {
                (ItemContainer.Children[1] as Grid).Margin = new Thickness(item.Left, 0, 0, 0);

                if (item.Actions == null)
                    item.Actions = new List<IAction>();

                if (item is IMenuGroup group)
                {
                    Label.AllowDrop = true;

                    Label.Drop -= OnDrop;
                    Label.Drop += OnDrop;

                    ItemContainer.Style = null;
                    Group.Visibility = Visibility.Visible;
                    UpdateGroup(group.IsOpen);

                    ClickHelper.LeftDown(ItemContainer, _ =>
                    {
                        group.IsOpen = !group.IsOpen;

                        UpdateGroup(group.IsOpen);

                        if (group.CanChangeGroup)
                            DragDrop.DoDragDrop(ItemContainer, group, DragDropEffects.Move);
                    });

                    if (item.Actions is List<IAction> groupActions)
                    {
                        foreach (IAction menuAction in groupActions.Where(a => a.Title == group.CreateHint).ToList())
                            groupActions.Remove(menuAction);

                        if (group.CanCreate)
                            groupActions.Add(new ActionContext(group.CreateHint, PackIconKind.Add, () =>
                            {
                                IsCreate = true;
                                IsCreateGroup = false;

                                Edit.Text = "";
                                HintAssist.SetHint(Edit, group.CreateHint);

                                Label.Visibility = Visibility.Collapsed;
                                Edit.Visibility = Visibility.Visible;

                                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                                    new Action(() =>
                                    {
                                        Edit.Focus();
                                        Keyboard.Focus(Edit);
                                        Edit.CaretIndex = 0;
                                    }));
                            }));

                        foreach (IAction menuAction in groupActions.Where(a => a.Title == group.CreateGroupHint).ToList())
                            groupActions.Remove(menuAction);

                        if (group.CanCreateGroup)
                            groupActions.Add(new ActionContext(group.CreateGroupHint, PackIconKind.FolderAdd, () =>
                            {
                                IsCreate = false;
                                IsCreateGroup = true;

                                Edit.Text = "";
                                HintAssist.SetHint(Edit, group.CreateGroupHint);

                                Label.Visibility = Visibility.Collapsed;
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
                }
                else if (item is IMenuItem menuItem)
                {
                    ClickHelper.LeftDown(ItemContainer, _ =>
                    {
                        if (menuItem.OnSelect?.Invoke(menuItem) == true)
                            menuItem.IsSelect = true;

                        if (menuItem.CanChangeGroup)
                            DragDrop.DoDragDrop(ItemContainer, menuItem, DragDropEffects.Move);
                    });
                }

                if (item.Actions is List<IAction> actions)
                {
                    foreach (IAction menuAction in actions.Where(a => a.Title == AWWindow.RenameTitle).ToList())
                        actions.Remove(menuAction);

                    if (item.CanRename)
                        actions.Add(new ActionContext(AWWindow.RenameTitle, PackIconKind.RenameBox, () =>
                        {
                            IsCreate = false;
                            IsCreateGroup = false;

                            Edit.Text = item.Name;
                            HintAssist.SetHint(Edit, AWWindow.NewNameTitle);

                            Label.Visibility = Visibility.Collapsed;
                            Edit.Visibility = Visibility.Visible;

                            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    Edit.Focus();
                                    Keyboard.Focus(Edit);
                                    Edit.CaretIndex = item.Name.Length;
                                }));
                        }));

                    foreach (IAction menuAction in actions.Where(a => a.Title == AWWindow.RemoveTitle).ToList())
                        actions.Remove(menuAction);

                    if (item.CanRemove)
                        actions.Add(new ActionContext(AWWindow.RemoveTitle, PackIconKind.Close, () =>
                        {
                            item.OnRemove?.Invoke(item);
                            item.Group?.RemoveItem(item);
                        }, iconColor: ColorHelper.RedSet.Color500.ToBrush()));
                }

                if (item.Actions is List<IAction> itemActions)
                    foreach (IAction menuAction in itemActions)
                        (menuAction.Command as SimpleCommand).OnExecute = () => MenuActions.IsOpen = false;

                (item as BaseContext).Notify(nameof(item.Actions));
            }
        }

        private void OnDrop(object sender, DragEventArgs de)
        {
            IMenuItem menuItem = (IMenuItem)de.Data.GetData(typeof(MenuItemContext)) ?? (IMenuItem)de.Data.GetData(typeof(MenuGroupItemContext));

            if (menuItem != null && DataContext is IMenuGroup group && group.OnChangeGroup?.Invoke(menuItem, group) == true)
            {
                menuItem.Group.RemoveItem(menuItem);
                group.AddItem(menuItem);
            }
        }

        private void UpdateGroup(bool value)
        {
            if (value)
            {
                Group.Kind = PackIconKind.MenuDown;
                SubItemContainer.Visibility = Visibility.Visible;
            }
            else
            {
                Group.Kind = PackIconKind.MenuRight;
                SubItemContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void EditKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && DataContext is IMenuItem menuItem)
                OnEdit(menuItem);
        }

        private void EditLostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is IMenuItem menuItem)
                OnEdit(menuItem);
        }

        private bool IsCreate;
        private bool IsCreateGroup;
        private void OnEdit(IMenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(Edit.Text))
                if (IsCreate || IsCreateGroup)
                {
                    Edit.Visibility = Visibility.Collapsed;
                    Label.Visibility = Visibility.Visible;
                    return;
                }
                else
                    return;

            Keyboard.ClearFocus();

            bool con = IsCreate
                ? (menuItem as IMenuGroup).OnCreate?.Invoke(menuItem as IMenuGroup, Edit.Text) == true
                : IsCreateGroup
                    ? (menuItem as IMenuGroup).OnCreateGroup?.Invoke(menuItem as IMenuGroup, Edit.Text) == true
                    : Edit.Text == menuItem.Name || menuItem.OnRename?.Invoke(menuItem, Edit.Text) == true;

            if (con)
            {
                if (!IsCreate && !IsCreateGroup)
                    menuItem.Name = Edit.Text;

                Edit.Text = "";

                Edit.Visibility = Visibility.Collapsed;
                Label.Visibility = Visibility.Visible;
            }
            else
            {
                Edit.Text = IsCreate || IsCreateGroup ? "" : menuItem.Name;

                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new Action(() =>
                    {
                        Edit.Focus();
                        Keyboard.Focus(Edit);
                        Edit.CaretIndex = Edit.Text.Length;
                    }));
            }
        }
    }
}
