using System.Windows.Controls;

using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class PropertyControl : BaseControl
    {
        public PropertyControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IVisualTypeContext visualType)
            {
                if (visualType is ObjectContext || visualType is CollectionContext)
                    Content = visualType.Control;
                else
                {
                    for (int i = 1; i < Container.Children.Count; ++i)
                        Container.Children.RemoveAt(i);

                    Container.Children.Add(visualType.Control);
                    Grid.SetColumn(visualType.Control, 1);
                }
            }
        }
    }
}
