using AW.Visual;
using AW.Visual.ColorTheme;
using AW.Visual.VisualType;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
            StackPanel grid = new StackPanel
            {
                Width = 200,
            };

            Test test = new Test();

            grid.Children.Add(new CheckBoxContext("Tag", test, "B").Control);
            grid.Children.Add(new DateContext("Tag", "Placeholder", test, "D").Control);
            grid.Children.Add(new TextBoxContext("Tag", "Placeholder", test, "T", TextBoxType.Text).Control);
            grid.Children.Add(new ComboBoxContext("Tag", "Placeholder", test, "C", null, new List<string> { "test1", "test2", "test3", "test4" }).Control);

            Window window = new AWWindow(grid);

            window.Title = "Test";
            window.Show();
        }

        public class Test
        {
            public bool B { get; set; }
            public DateTime D { get; set; } = DateTime.Now;
            public string T { get; set; }
            public string C { get; set; }
        }
    }
}
