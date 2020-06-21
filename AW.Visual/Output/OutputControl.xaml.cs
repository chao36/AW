using System.Windows.Controls;

namespace AW.Visual.Output
{
    public partial class OutputControl : UserControl
    {
        public static string Title { get; set; } = "History";

        public OutputControl()
        {
            InitializeComponent();

            HistoryLabel.Text = Title;
            Items.SizeChanged += (sc, ec) =>
            {
                if (LastCount != Items.Items.Count)
                {
                    Scroll.ScrollToEnd();
                    LastCount = Items.Items.Count;
                }
            };

            DataContext = new OutputContext();
        }

        private int LastCount = 0;
    }
}
