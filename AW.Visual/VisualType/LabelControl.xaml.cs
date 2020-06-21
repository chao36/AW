using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class LabelControl : UserControl
    {
        public LabelControl() => InitializeComponent();
    }

    public class LabelContext : VisualTypeContext
    {
        public LabelContext(string tag)
            : base(tag, null, null, new LabelControl()) { }
    }
}
