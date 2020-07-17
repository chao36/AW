using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class ControlView : BaseControl
    {
        public ControlView() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IVisualTypeContext visualType)
                Content = visualType.Control;
        }
    }
}
