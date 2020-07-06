using System.Windows.Controls;

namespace AW.Visual.Output
{
    public partial class OutputControl : UserControl
    {
        private int LastCount = 0;

        public OutputControl()
        {
            InitializeComponent();

            HistoryLabel.Text = AWWindow.History;
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
    }
}
