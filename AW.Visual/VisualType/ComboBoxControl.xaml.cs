using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class ComboBoxControl : UserControl
    {
        public ComboBoxControl(bool hideTag, string displayMemberPath)
        {
            InitializeComponent();

            if (hideTag)
                Tag.Visibility = Visibility.Collapsed;

            Element.DisplayMemberPath = displayMemberPath;
        }
    }

    public class ComboBoxContext : VisualTypeContext
    {
        public ComboBoxContext(string tag, string placeholder, object source, string property, string displayMemberPath, IEnumerable<object> items, bool? hideTag = null)
            : base(tag, source, property, new ComboBoxControl(hideTag ?? string.IsNullOrEmpty(tag), displayMemberPath))
        {
            Placeholder = placeholder;
            Items = new ObservableCollection<object>(items);
        }

        public string Placeholder { get; }
        public ObservableCollection<object> Items { get; }
    }
}
