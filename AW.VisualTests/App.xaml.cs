using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using AW.Visual;
using AW.Visual.VisualType;

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

            StackPanel grid = new StackPanel
            {
                Width = 400,
            };
            Button button = new Button();
            button.Click += Button_Click;
            

           // grid.Children.Add(new ObjectContext(test).Control);grid.Children.Add(button);

            Window window = new AWWindow(grid);

            window.Title = "Test";
            window.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        Class2 test = new Class2();



        public enum Enum1
        {
            Value1,
            Value2,
            Value3,
        }

        public class Class1
        {
            [AWProperty]
            public bool Bool { get; set; }
            [AWProperty]
            public DateTime Date { get; set; } = DateTime.Now;
            [AWProperty]
            public Enum1 Enum { get; set; }
        }

        public class Class2
        {
            [AWProperty]
            public bool Bool { get; set; }
            [AWProperty]
            public DateTime Date { get; set; } = DateTime.Now;
            [AWProperty]
            public int Int { get; set; } = 20;
            [AWProperty]
            public string String { get; set; } = "Test";

            [AWProperty]
            public Class1 Class1 { get; set; }

            [AWProperty]
            public List<Class1> List { get; set; } = new List<Class1>
            {
                new Class1()
            };
        }
    }
}
