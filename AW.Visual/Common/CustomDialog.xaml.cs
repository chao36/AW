using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Common
{
    public partial class CustomDialog : BaseControl
    {
        public CustomDialog(FrameworkElement view, double width, double height)
        {
            InitializeComponent();

            Card card = new Card
            {
                Content = view,
                Background = (Brush)FindResource("MaterialDesignCardBackground")
            };

            Container.Children.Clear();

            Container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength((1.0 - width) / 2, GridUnitType.Star) });
            Container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width, GridUnitType.Star) });
            Container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength((1.0 - width) / 2, GridUnitType.Star) });

            Container.RowDefinitions.Add(new RowDefinition { Height = new GridLength((1.0 - height) / 2, GridUnitType.Star) });
            Container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height, GridUnitType.Star) });
            Container.RowDefinitions.Add(new RowDefinition { Height = new GridLength((1.0 - height) / 2, GridUnitType.Star) });

            Container.Children.Add(card);
            Grid.SetColumn(card, 1);
            Grid.SetRow(card, 1);

            VisualHelper.LeftClick(Shadow, _ =>
            {
                Container.Children.Remove(view);
                DialogHelper.Hide();
            });
        }
    }
}
