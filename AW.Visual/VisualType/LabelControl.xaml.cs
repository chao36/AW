using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class LabelControl : UserControl
    {
        public LabelControl() => InitializeComponent();
    }

    public class LabelContext : VisualTypeContext
    {
        public LabelContext(object source, string property)
            : base(null, source, property, new LabelControl()) { }
    }
}
