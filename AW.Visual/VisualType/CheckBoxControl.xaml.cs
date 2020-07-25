using System.Windows;

using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class CheckBoxControl : BaseControl
    {
        public CheckBoxControl(bool hideTag)
        {
            InitializeComponent();

            if (hideTag)
                Element.Content = null;
        }

        protected override void OnDataContextChange()
        {
            if (DataContext is IVisualTypeContext context)
            {
                if (!string.IsNullOrEmpty(context.Style))
                    Element.Style = (Style)FindResource(context.Style);
            }
        }
    }

    public class CheckBoxContext : VisualTypeContext
    {
        public CheckBoxContext(string tag, object source, string property, bool? hideTag = null)
            : base(tag, source, property, new CheckBoxControl(hideTag ?? string.IsNullOrEmpty(tag))) { }
    }
}
