using System.Windows;

namespace AW.Visual.Common
{
    public partial class SimpleDialog : BaseControl
    {
        public SimpleDialog(string message, bool wait)
        {
            InitializeComponent();

            Title.Text = message;

            if (wait)
                Icon.Visibility = Visibility.Collapsed;
            else
            {
                Wait.Visibility = Visibility.Collapsed;

                ClickHelper.LeftClick(Shadow, _ =>
                {
                    DialogHelper.Hide();
                });
            }
        }
    }
}
