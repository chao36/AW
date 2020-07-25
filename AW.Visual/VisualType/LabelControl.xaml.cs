using System.Windows;

using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class LabelControl : BaseControl
    {
        public LabelControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is IVisualTypeContext context)
            {
                if (!string.IsNullOrEmpty(context.Style))
                    Element.Style = (Style)FindResource(context.Style);
            }
        }
    }

    public class LabelContext : VisualTypeContext
    {
        public LabelContext(string tag, object source, string property)
            : base(tag, source, property, new LabelControl()) { }
    }
}