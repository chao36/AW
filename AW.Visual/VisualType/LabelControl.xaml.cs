using System.Windows;
using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class LabelControl : UserControl
    {
        public LabelControl(bool hideTag)
        {
            InitializeComponent();

            if (hideTag)
                Tag.Visibility = Visibility.Collapsed;
        }
    }

    public class LabelContext : VisualTypeContext
    {
        public LabelContext(string tag, object source, string property, bool? hideTag = null)
            : base(tag, source, property, new LabelControl(hideTag ?? string.IsNullOrEmpty(tag))) { }
    }
}