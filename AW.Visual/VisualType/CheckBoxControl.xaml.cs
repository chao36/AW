using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class CheckBoxControl : UserControl
    {
        public CheckBoxControl() => InitializeComponent();
    }

    public class CheckBoxContext : VisualTypeContext
    {
        public CheckBoxContext(string tag, object source, string property)
            : base(tag, source, property, new CheckBoxControl()) { }
    }
}
