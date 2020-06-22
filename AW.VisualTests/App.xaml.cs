using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using AW.Base.Serializer;
using AW.Base.Serializer.Common;
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

            //using AWSerializer serializer = new AWSerializer();
            //string data = SerializerHelper.LoadText();
            //menu = serializer.Deserialize<MenuGroupItemContext>(data);

            //menu.OnSelect = OnMenuSelect;
            //menu.OnRemove = OnPageRemove;
            //menu.OnRename = OnPageEdit;
            //menu.OnCreateItem = OnPageAdd;
            //menu.OnCreateGroup = OnFolderAdd;
            //menu.OnChangeGroup = OnPageGroupChange;

            menu = new MenuGroupItemContext
            {
                Icon = PackIconKind.FileDocumentBoxMultiple,
                Name = "pages",
                CreateItemHint = "add_page",
                CreateGroupHint = "add_folder",
                ViewRemove = false,
                ViewRename = false,
                OnSelect = OnMenuSelect,
                OnRemove = OnPageRemove,
                OnRename = OnPageEdit,
                OnCreateItem = OnPageAdd,
                OnCreateGroup = OnFolderAdd,
                OnChangeGroup = OnPageGroupChange,
                IsOpen = true
            };

            grid.Children.Add(new MenuControl() { DataContext = menu } );

            AWWindow window = new AWWindow(grid);
            window.Title = "Test";
            window.Show();

            window.Closing += Window_Closing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using AWSerializer serializer = new AWSerializer();
            SerializerHelper.SaveText(serializer.Serialize(menu));
        }

        MenuGroupItemContext menu;

        private bool OnMenuSelect(IMenuItem item)
        {
            foreach (IMenuItem menuItem in menu.Items)
                menuItem.IsSelect = false;

            item.IsSelect = true;

            return true;
        }

        private bool OnFolderAdd(IMenuGroup group, string name)
        {
            group.AddItem(GetFolder(name));

            return true;
        }

        private IMenuGroup GetFolder(string name)
            => new MenuGroupItemContext
            {
                Icon = PackIconKind.Folder,
                Name = name,
                CanChangeGroup = true
            };

        private bool OnPageAdd(IMenuGroup group, string name)
        {
            IMenuItem menuItem = GetPage(name);

            group.AddItem(menuItem);
            OnMenuSelect(menuItem);

            return true;
        }

        private IMenuItem GetPage(string name)
            => new MenuItemContext
            {
                Icon = PackIconKind.FileDocumentBoxOutline,
                Name = name,
                CanChangeGroup = true,
            };

        private bool OnPageGroupChange(IMenuItem menuItem, IMenuGroup group)
        {
            if (menuItem == group)
                return false;

            return true;
        }

        private bool OnPageEdit(IMenuItem item, string name)
        {
            return true;
        }

        private bool OnPageRemove(IMenuItem item)
        {
            return true;
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
