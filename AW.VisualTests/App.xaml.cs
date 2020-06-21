using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using AW.Visual;
using AW.Visual.Menu;
using AW.Visual.VisualType;
using MaterialDesignThemes.Wpf;

namespace AW.VisualTests
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AWWindow.ChangeTheme(true, ColorHelper.TealSet.Color700.ToMediaColor());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AWWindow.Init();

            Grid grid = new Grid
            {
                Width = 400
            };

            var menu = new MenuGroupItemContext
            {
                Name = "Test",
                CreateGroupHint = "Add group",
                CreateItemHint = "Add item",

                Icon = PackIconKind.FolderAccount,

                IsOpen = true,
                IsSelect = false,

                NeedSortItems = true,
                CanChangeGroup = false,

                ViewCreateItem = true,
                ViewCreateGroup = false,

                ViewRemove = false,
                ViewRename = true,

                OnChangeGroup = (item, newGroup) => true,

                OnCreateItem = (group, name) =>
                {
                    if (string.IsNullOrEmpty(name))
                        return false;

                    group.AddItem(new MenuItemContext
                    {
                        Name = name,

                        Icon = PackIconKind.Account,
                        ViewRemove = true,
                        ViewRename = true,

                        OnRemove = (item) => true,
                        OnRename = (item, newName) => true,

                        OnSelect = (item) =>
                        {
                            item.IsSelect = true;

                            return true;
                        }
                    });

                    return true;
                },
                //OnCreateGroup

                OnRemove = (item) => true,
                OnRename = (item, newName) => true,
            };

            grid.Children.Add(new MenuControl() { DataContext = menu } );

            AWWindow window = new AWWindow(grid);
            window.Title = "Test";
            window.Show();
        }


        public enum TestEnum
        {
            Value1,
            Value2,
            Value3,
        }

        public class SubTestVisual
        {
            [AWProperty]
            public bool Bool { get; set; }
            [AWProperty]
            public TestEnum Enum { get; set; }
        }

        public class TestVisual
        {
            [AWReadonly]
            [AWProperty]
            public string Name { get; set; } = "Test";
            [AWProperty]
            public bool Bool { get; set; }
            [AWProperty]
            public DateTime Date { get; set; } = DateTime.Now;
            [AWProperty]
            public int Int { get; set; } = 20;
            [AWProperty]
            public string String { get; set; } = "Test";

            [AWProperty]
            public SubTestVisual SubTestVisual { get; set; }

            [AWProperty]
            public List<SubTestVisual> List { get; set; } = new List<SubTestVisual>
            {
                new SubTestVisual
                {
                    Enum = TestEnum.Value2
                }
            };
        }
    }
}
