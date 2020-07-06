using System.Windows;
using System.Windows.Controls;

using AW.Base;

namespace AW.Visual.VisualType
{
    public partial class TextBoxControl : UserControl
    {
        public TextBoxControl(bool hideTag, TextBoxType textBoxType, AWPropertyAttribute attribute)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;

            if (attribute is AWFilePathAttribute filePathAttribute)
            {
                Element.Visibility = Visibility.Collapsed;
                PathElement.Visibility = Visibility.Visible;

                VisualHelper.LeftClick(PathElement, _ =>
                {
                    if (DataContext is VisualTypeContext context)
                        context.Value = VisualHelper.SelectFile(filePathAttribute.Message, filePathAttribute.Filter, context.Value?.ToString(), filePathAttribute.OnlyFolder);
                });
            }
            else
            {
                if (attribute is AWLimitAttribute limitAttribute && limitAttribute.MaxLength > 0)
                    Element.MaxLength = limitAttribute.MaxLength;

                if (textBoxType != TextBoxType.String)
                    VisualHelper.LimitInput(Element, textBoxType == TextBoxType.Double ? LimitType.Double : LimitType.Int, (attribute as AWLimitAttribute)?.AllowedStrings);

                VisualHelper.ExitOnEnter(Element);
            }
        }
    }

    public enum TextBoxType
    {
        String = 0,
        Double = 1,
        Int = 2
    }

    public class TextBoxContext : VisualTypeContext
    {
        public TextBoxContext(string tag, string placeholder, object source, string property, TextBoxType textBoxType, AWPropertyAttribute attribute = null, bool? hideTag = null)
            : base(tag, source, property, new TextBoxControl(hideTag ?? string.IsNullOrEmpty(tag), textBoxType, attribute))
            => Placeholder = placeholder;

        public string Placeholder { get; }
    }
}