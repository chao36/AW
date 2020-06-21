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

            Grid grid = new Grid
            {
                Width = 400
            };

            var obj = new TestVisual();
            grid.Children.Add(new ObjectContext(obj).Control);

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
