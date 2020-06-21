using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class CheckBoxControl : UserControl
    {
        public CheckBoxControl(bool hideTag)
        {
            InitializeComponent();

            if (hideTag)
                Element.Content = null;
        }
    }

    public class CheckBoxContext : VisualTypeContext
    {
        public CheckBoxContext(string tag, object source, string property, bool? hideTag = null)
            : base(tag, source, property, new CheckBoxControl(hideTag ?? string.IsNullOrEmpty(tag))) { }
    }
}
