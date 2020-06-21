using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class LabelControl : UserControl
    {
        public LabelControl() => InitializeComponent();
    }

    public class LabelContext : VisualTypeContext
    {
        public LabelContext(string tag, object source, string property)
            : base(tag, source, property, new LabelControl()) { }
    }
}